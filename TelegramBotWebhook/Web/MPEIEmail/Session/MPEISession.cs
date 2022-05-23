using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Timers;
using TelegramBotWebhook.Services;

namespace TelegramBotWebhook
{
    public sealed class MPEISession
    {
        private const int _lifeTime = 900_000;

        private static ConcurrentDictionary<long, MPEISession> _sessions = new ConcurrentDictionary<long, MPEISession>();
        private static ConcurrentBag<long> _finishedSessions = new ConcurrentBag<long>();

        private System.Timers.Timer _sessionLifeTimer = new System.Timers.Timer(_lifeTime);

        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long UserId { get; }
        [NotMapped]
        public string? UserKey { get; set; }
        [NotMapped]
        public string? UnlogKey { get; set; }
        public string? Login { get; set; }
        [NotMapped]
        public string? Password { get; set; }
        [Required]
        public bool SaveCredentials { get; set; }

        private MPEISession(long userId, bool saveCredentials = false)
        {
            UserId = userId;
            SaveCredentials = saveCredentials;
            _sessionLifeTimer.Elapsed += AddFinishedSession;
            _sessionLifeTimer.Start();
        }

        private void ClearSession()
        {
            UserKey = null;
            UnlogKey = null;
            Password = null;
        }
        private void UpdateLifeTimer()
        {
            _sessionLifeTimer.Stop();
            _sessionLifeTimer.Elapsed -= AddFinishedSession;

            _sessionLifeTimer = new System.Timers.Timer(_lifeTime);
            _sessionLifeTimer.Elapsed += AddFinishedSession;
            _sessionLifeTimer.Start();
        }
        private void AddFinishedSession(object? o, ElapsedEventArgs args)
        {
            _sessionLifeTimer.Stop();
            _finishedSessions.Add(UserId);
        }
        private static bool TryAddSession(long userId, MPEISession session)
        {
            return _sessions.TryAdd(userId, session);
        }
        private static bool TryGetSession(long userId, out MPEISession? session)
        {
            bool success = _sessions.TryGetValue(userId, out session);

            session?.UpdateLifeTimer();
            session?._sessionLifeTimer.Start();

            return success;
        }

        public class MPEISessionService : ISessionService<MPEISession>, IDisposable
        {
            private readonly MPEISessionDbContext _dbContext;

            public MPEISessionService(MPEISessionDbContext dbContext)
            {
                _dbContext = dbContext;

                if (!_finishedSessions.IsEmpty)
                    ClearFinishedSessions();
            }

            private void ClearFinishedSessions()
            {
                using var transaction = _dbContext.Database.BeginTransaction();

                Parallel.ForEach(_finishedSessions, (userId) =>
                {
                    MPEISession? session;
                    _sessions.Remove(userId, out session);

                    if (!session!.SaveCredentials)
                    {
                        session.Login = null;
                    }
                    
                    session.ClearSession();

                    _dbContext.MPEISessions.Update(session);
                    _dbContext.SaveChanges();
                });
                
                transaction.Commit();

                _finishedSessions.Clear();
            }

            public async Task StartSession(long userId) 
            {
                await GetSession(userId);
            }
            public async Task<MPEISession> GetSession(long userId)
            {
                MPEISession? session;
                if (TryGetSession(userId, out session))
                {
                    return session!;
                }
                else
                {
                    session = _dbContext.MPEISessions.Find(userId);

                    if (session is null)
                    {
                        session = await CreateSession(userId); 
                    }
                    TryAddSession(userId, session);

                    return session;
                }
            }
            private async Task<MPEISession> CreateSession(long userId)
            {
                var session = new MPEISession(userId);

                await _dbContext.AddAsync(session);
                await _dbContext.SaveChangesAsync();

                return session;
            }
            public void Dispose()
            {
                _dbContext.Dispose();
            }
        }
    }
}
