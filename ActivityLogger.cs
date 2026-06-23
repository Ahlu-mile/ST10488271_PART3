using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CyberWareASM
{
    /// <summary>
    /// Records every significant chatbot action with a timestamp.
    /// GetRecentLog() returns the last N entries (default 10).
    /// GetFullLog() returns everything.
    /// </summary>
    public class ActivityLogger
    {
        private readonly List<string> _log = new();

        // ──────────────────────────────────────────────────────────────
        //  WRITE
        // ──────────────────────────────────────────────────────────────

        /// <summary>Adds a timestamped entry to the in-memory log.</summary>
        public void Log(string action)
        {
            string timestamp = DateTime.Now.ToString("[HH:mm] ");
            _log.Add(timestamp + action);
        }

        // ──────────────────────────────────────────────────────────────
        //  READ
        // ──────────────────────────────────────────────────────────────

        /// <summary>
        /// Returns the most recent <paramref name="count"/> entries as a
        /// numbered, newline-separated string. Shows all entries if fewer
        /// than <paramref name="count"/> exist.
        /// </summary>
        public string GetRecentLog(int count = 10)
        {
            if (_log.Count == 0)
                return "No activity recorded yet.";

            var recent = _log.TakeLast(count).ToList();
            var sb = new StringBuilder();

            for (int i = 0; i < recent.Count; i++)
                sb.AppendLine($"{i + 1}. {recent[i]}");

            return sb.ToString().TrimEnd();
        }

        /// <summary>Returns every log entry as a numbered, newline-separated string.</summary>
        public string GetFullLog()
        {
            if (_log.Count == 0)
                return "No activity recorded yet.";

            var sb = new StringBuilder();
            for (int i = 0; i < _log.Count; i++)
                sb.AppendLine($"{i + 1}. {_log[i]}");

            return sb.ToString().TrimEnd();
        }

        /// <summary>Total number of entries logged so far.</summary>
        public int GetCount() => _log.Count;

        /// <summary>True when there are more entries than the default display limit.</summary>
        public bool HasMore(int displayLimit = 10) => _log.Count > displayLimit;
    }
}
