using System.Collections.Generic;

namespace CyberWareASM
{
    /// <summary>
    /// Stores and recalls information about the current user session.
    /// Tracks: first name, last name, full name and favourite cybersecurity topic.
    /// </summary>
    public class MemoryStore
    {
        // ── Core user identity ────────────────────────────────────────────────
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName => $"{FirstName} {LastName}".Trim();

        // ── Conversation preferences ──────────────────────────────────────────
        public string FavouriteTopic { get; set; } = string.Empty;

        // ── General-purpose key-value store ───────────────────────────────────
        private readonly Dictionary<string, string> _store = new();

        /// <summary>Saves any arbitrary key-value pair for later recall.</summary>
        public void Store(string key, string value)
            => _store[key.ToLowerInvariant()] = value;

        /// <summary>Retrieves a previously stored value, or empty string.</summary>
        public string Recall(string key)
            => _store.TryGetValue(key.ToLowerInvariant(), out var val) ? val : string.Empty;

        /// <summary>
        /// Returns a personalised sentence opener based on what we know.
        /// E.g. "As someone interested in phishing, "
        /// </summary>
        public string GetPersonalisedOpener()
        {
            if (!string.IsNullOrWhiteSpace(FavouriteTopic))
                return $"As someone interested in {FavouriteTopic}, ";
            return string.Empty;
        }

        /// <summary>Clears all stored data (useful for End Convo).</summary>
        public void Clear()
        {
            FirstName = string.Empty;
            LastName = string.Empty;
            FavouriteTopic = string.Empty;
            _store.Clear();
        }
    }
}
