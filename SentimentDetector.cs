using System.Collections.Generic;

namespace CyberWareASM
{
    /// <summary>Emotional tones the chatbot can recognise.</summary>
    public enum Sentiment { Neutral, Worried, Curious, Frustrated, Happy }

    /// <summary>
    /// Detects the emotional tone in a user's message and provides
    /// an empathetic opening sentence before any cybersecurity response.
    /// </summary>
    public class SentimentDetector
    {
        private readonly Dictionary<Sentiment, List<string>> _triggers = new()
        {
            [Sentiment.Worried] = new List<string>
            {
                "worried", "scared", "afraid", "anxious", "nervous",
                "unsafe", "frightened", "concerned", "fear", "terrified",
                "panicking", "stressed", "alarmed", "uneasy", "dreading"
            },
            [Sentiment.Curious] = new List<string>
            {
                "curious", "wondering", "interested", "want to know",
                "how does", "what is", "can you explain", "tell me about",
                "fascinated", "intrigued", "learning", "explore", "understand"
            },
            [Sentiment.Frustrated] = new List<string>
            {
                "frustrated", "annoyed", "confused", "don't understand",
                "no idea", "lost", "hate", "difficult", "complicated",
                "can't figure out", "makes no sense", "fed up", "struggling"
            },
            [Sentiment.Happy] = new List<string>
            {
                "great", "thanks", "helpful", "awesome", "love it",
                "amazing", "brilliant", "excellent", "perfect", "fantastic",
                "happy", "glad", "pleased", "wonderful", "appreciate"
            }
        };

        /// <summary>
        /// Scans the input string and returns the dominant sentiment.
        /// Returns Neutral if no trigger words are found.
        /// </summary>
        public Sentiment Detect(string input)
        {
            string lower = input.ToLowerInvariant();
            foreach (var kvp in _triggers)
                foreach (var word in kvp.Value)
                    if (lower.Contains(word))
                        return kvp.Key;

            return Sentiment.Neutral;
        }

        /// <summary>
        /// Returns a warm, empathetic sentence to prepend to a cybersecurity tip
        /// when a non-neutral sentiment is detected.
        /// </summary>
        public string GetSentimentResponse(Sentiment s) => s switch
        {
            Sentiment.Worried => "I completely understand your concern — that's a very valid feeling. Let me help put your mind at ease. ",
            Sentiment.Curious => "I love your curiosity! That's exactly the mindset that keeps you safe online. Here's what you need to know: ",
            Sentiment.Frustrated => "I hear you — cybersecurity can feel overwhelming at first. Let me break this down as simply as possible. ",
            Sentiment.Happy => "That's wonderful to hear! Let's keep that positive energy going with some useful info. ",
            _ => string.Empty
        };
    }
}
