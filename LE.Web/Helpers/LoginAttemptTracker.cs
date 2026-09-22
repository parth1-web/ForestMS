using System;
using System.Collections.Generic;

namespace LE.Web.Helpers
{
    /// <summary>
    /// In-memory failed-login throttler. Blocks a username (optionally combined with
    /// the client IP) after too many consecutive failed attempts within a time window.
    /// Successful login clears the counter. Suitable for single-server deployments.
    /// </summary>
    public class LoginAttemptTracker
    {
        private const int MAX_FAILED_ATTEMPTS = 5;
        private static readonly TimeSpan LOCKOUT_WINDOW = TimeSpan.FromMinutes(15);

        private class AttemptRecord
        {
            public int FailedCount;
            public DateTime FirstFailedAt;
            public DateTime? LockedUntil;
        }

        // Use a simple dictionary with lock instead of ConcurrentDictionary
        private static readonly Dictionary<string, AttemptRecord> _attempts = new Dictionary<string, AttemptRecord>();
        private static readonly object _lock = new object();

        static LoginAttemptTracker()
        {
            // Ensure static fields are initialized
            if (_attempts == null) _attempts = new Dictionary<string, AttemptRecord>();
            if (_lock == null) _lock = new object();
        }

        private static string keyFor(string username, string remoteIp)
        {
            return (username ?? string.Empty).Trim().ToLowerInvariant() + "|" + (remoteIp ?? "unknown");
        }

        /// <summary>
        /// True when the username/IP combination is currently locked out.
        /// </summary>
        public bool IsLockedOut(string username, string remoteIp)
        {
            lock (_lock)
            {
                string key = keyFor(username, remoteIp);
                if (!_attempts.TryGetValue(key, out AttemptRecord record))
                {
                    return false;
                }

                if (record.LockedUntil.HasValue)
                {
                    if (DateTime.UtcNow < record.LockedUntil.Value)
                    {
                        return true;
                    }
                    // Lockout expired; start fresh.
                    _attempts.Remove(key);
                    return false;
                }

                // Window expired without reaching the threshold; start fresh.
                if (DateTime.UtcNow - record.FirstFailedAt > LOCKOUT_WINDOW)
                {
                    _attempts.Remove(key);
                }
                return false;
            }
        }

        /// <summary>
        /// Records a failed attempt and locks the combination out when the
        /// threshold is reached inside the window.
        /// </summary>
        public void RecordFailure(string username, string remoteIp)
        {
            lock (_lock)
            {
                string key = keyFor(username, remoteIp);
                if (!_attempts.TryGetValue(key, out AttemptRecord record))
                {
                    record = new AttemptRecord
                    {
                        FailedCount = 0,
                        FirstFailedAt = DateTime.UtcNow
                    };
                    _attempts[key] = record;
                }

                if (DateTime.UtcNow - record.FirstFailedAt > LOCKOUT_WINDOW)
                {
                    record.FailedCount = 0;
                    record.FirstFailedAt = DateTime.UtcNow;
                    record.LockedUntil = null;
                }

                record.FailedCount++;

                if (record.FailedCount >= MAX_FAILED_ATTEMPTS)
                {
                    record.LockedUntil = DateTime.UtcNow.Add(LOCKOUT_WINDOW);
                }
            }
        }

        /// <summary>
        /// Clears the failure counter after a successful login.
        /// </summary>
        public void RecordSuccess(string username, string remoteIp)
        {
            lock (_lock)
            {
                _attempts.Remove(keyFor(username, remoteIp));
            }
        }
    }
}
