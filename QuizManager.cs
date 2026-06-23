using System.Collections.Generic;

namespace CyberWareASM
{
    // ══════════════════════════════════════════════════════════════════
    //  MODEL
    // ══════════════════════════════════════════════════════════════════

    public class QuizQuestion
    {
        public string Question { get; set; } = string.Empty;
        public List<string> Options { get; set; } = new();   // e.g. "A) ...", "B) ..."
        public string CorrectAnswer { get; set; } = string.Empty; // "A", "B", "True", "False"
        public string Explanation { get; set; } = string.Empty;
        public bool IsTrueFalse { get; set; }
    }

    // ══════════════════════════════════════════════════════════════════
    //  MANAGER
    // ══════════════════════════════════════════════════════════════════

    /// <summary>
    /// Manages the cybersecurity quiz: questions, scoring, and flow.
    /// One instance per quiz session. Call ResetQuiz() to play again.
    /// </summary>
    public class QuizManager
    {
        private readonly List<QuizQuestion> _questions;
        private int _currentIndex = 0;
        private int _score = 0;

        public QuizManager()
        {
            _questions = BuildQuestions();
        }

        // ──────────────────────────────────────────────────────────────
        //  PUBLIC API
        // ──────────────────────────────────────────────────────────────

        /// <summary>Returns the current question, or null if the quiz is over.</summary>
        public QuizQuestion? GetCurrentQuestion()
            => IsFinished() ? null : _questions[_currentIndex];

        /// <summary>
        /// Checks the supplied answer, advances the index, increments score
        /// if correct, and returns true when the answer is correct.
        /// </summary>
        public bool SubmitAnswer(string answer)
        {
            if (IsFinished()) return false;

            bool correct = answer.Trim().ToUpperInvariant()
                           == _questions[_currentIndex].CorrectAnswer.ToUpperInvariant();

            if (correct) _score++;
            _currentIndex++;
            return correct;
        }

        /// <summary>Returns the explanation for the question that was just answered.</summary>
        public string GetFeedback(bool correct)
        {
            // _currentIndex was already advanced in SubmitAnswer, so look back one
            int idx = _currentIndex - 1;
            if (idx < 0 || idx >= _questions.Count) return string.Empty;

            string prefix = correct ? "✅ Correct! " : "❌ Incorrect. ";
            return prefix + _questions[idx].Explanation;
        }

        public bool IsFinished() => _currentIndex >= _questions.Count;
        public int GetScore() => _score;
        public int GetTotal() => _questions.Count;
        public int GetCurrentNum() => _currentIndex + 1; // 1-based for display

        public string GetFinalScore()
            => $"You scored {_score} out of {_questions.Count}.";

        public string GetFinalMessage()
        {
            double pct = (double)_score / _questions.Count * 100;
            return pct >= 80
                ? "🎉 Excellent work! You're a cybersecurity champion!"
                : pct >= 50
                    ? "👍 Good effort! Keep learning to sharpen your skills."
                    : "📚 Keep studying — cybersecurity knowledge is your best defence!";
        }

        /// <summary>Resets state so the quiz can be played again.</summary>
        public void ResetQuiz()
        {
            _currentIndex = 0;
            _score = 0;
        }

        // ──────────────────────────────────────────────────────────────
        //  QUESTIONS  (12 — covers all required topic areas)
        // ──────────────────────────────────────────────────────────────

        private static List<QuizQuestion> BuildQuestions() => new()
        {
            // ── PHISHING ─────────────────────────────────────────────
            new QuizQuestion
            {
                Question      = "What should you do if you receive an email asking for your password?",
                Options       = new() { "A) Reply with your password", "B) Delete the email",
                                        "C) Report the email as phishing", "D) Ignore it" },
                CorrectAnswer = "C",
                Explanation   = "Reporting phishing emails helps protect others and allows your email provider or IT team to act.",
                IsTrueFalse   = false
            },
            new QuizQuestion
            {
                Question      = "Phishing emails always contain obvious spelling mistakes and are easy to spot.",
                Options       = new() { "True", "False" },
                CorrectAnswer = "False",
                Explanation   = "Modern phishing attacks are increasingly sophisticated and can closely mimic legitimate organisations.",
                IsTrueFalse   = true
            },

            // ── PASSWORD SAFETY ──────────────────────────────────────
            new QuizQuestion
            {
                Question      = "Which of the following is the strongest password?",
                Options       = new() { "A) password123", "B) MyDog2020", "C) Tr0ub4dor&3", "D) 123456" },
                CorrectAnswer = "C",
                Explanation   = "A strong password mixes uppercase, lowercase, numbers, and symbols and is not a dictionary word.",
                IsTrueFalse   = false
            },
            new QuizQuestion
            {
                Question      = "It is safe to reuse the same password across multiple websites.",
                Options       = new() { "True", "False" },
                CorrectAnswer = "False",
                Explanation   = "If one site is breached, attackers use credential-stuffing to access your other accounts.",
                IsTrueFalse   = true
            },

            // ── TWO-FACTOR AUTHENTICATION ────────────────────────────
            new QuizQuestion
            {
                Question      = "What does two-factor authentication (2FA) add to a login?",
                Options       = new() { "A) A longer password requirement",
                                        "B) A second verification step beyond just a password",
                                        "C) A CAPTCHA challenge",
                                        "D) Automatic logout after 5 minutes" },
                CorrectAnswer = "B",
                Explanation   = "2FA requires something you know (password) plus something you have or are, making accounts far harder to compromise.",
                IsTrueFalse   = false
            },

            // ── SAFE BROWSING ────────────────────────────────────────
            new QuizQuestion
            {
                Question      = "What does HTTPS in a website address indicate?",
                Options       = new() { "A) The site is owned by a government",
                                        "B) The connection is encrypted",
                                        "C) The site is free of malware",
                                        "D) The site loads faster" },
                CorrectAnswer = "B",
                Explanation   = "HTTPS means the data between your browser and the server is encrypted. It does not guarantee the site is safe, but the connection is secure.",
                IsTrueFalse   = false
            },
            new QuizQuestion
            {
                Question      = "Using public Wi-Fi without a VPN is always completely safe for online banking.",
                Options       = new() { "True", "False" },
                CorrectAnswer = "False",
                Explanation   = "Public Wi-Fi can be monitored by attackers. A VPN encrypts your traffic, reducing the risk significantly.",
                IsTrueFalse   = true
            },

            // ── SOCIAL ENGINEERING ───────────────────────────────────
            new QuizQuestion
            {
                Question      = "A caller claims to be IT support and asks for your login credentials to fix an issue. What should you do?",
                Options       = new() { "A) Provide the credentials — they need them to help",
                                        "B) Refuse and report the call to your real IT department",
                                        "C) Give a fake password to test them",
                                        "D) Ask them to email you instead" },
                CorrectAnswer = "B",
                Explanation   = "Legitimate IT staff never need your password. This is a classic social engineering (vishing) tactic.",
                IsTrueFalse   = false
            },
            new QuizQuestion
            {
                Question      = "Social engineering attacks always rely on technical exploits rather than human psychology.",
                Options       = new() { "True", "False" },
                CorrectAnswer = "False",
                Explanation   = "Social engineering targets human behaviour — trust, urgency, and authority — not just software vulnerabilities.",
                IsTrueFalse   = true
            },

            // ── MALWARE & RANSOMWARE ─────────────────────────────────
            new QuizQuestion
            {
                Question      = "What is the primary purpose of ransomware?",
                Options       = new() { "A) To display advertisements",
                                        "B) To steal passwords silently",
                                        "C) To encrypt files and demand payment for the decryption key",
                                        "D) To slow down your computer" },
                CorrectAnswer = "C",
                Explanation   = "Ransomware locks or encrypts your data, then demands a ransom — usually in cryptocurrency — for the decryption key.",
                IsTrueFalse   = false
            },

            // ── PRIVACY SETTINGS ─────────────────────────────────────
            new QuizQuestion
            {
                Question      = "Why should you regularly review the privacy settings on your social media accounts?",
                Options       = new() { "A) To increase your follower count",
                                        "B) To ensure only intended audiences can see your personal information",
                                        "C) Social media companies set them securely by default",
                                        "D) It is not necessary if you have nothing to hide" },
                CorrectAnswer = "B",
                Explanation   = "Default privacy settings are often permissive. Regular reviews ensure your data is only shared with people you trust.",
                IsTrueFalse   = false
            },

            // ── DATA BACKUP ──────────────────────────────────────────
            new QuizQuestion
            {
                Question      = "What is the recommended '3-2-1' backup strategy?",
                Options       = new() { "A) 3 passwords, 2 devices, 1 cloud account",
                                        "B) 3 copies of data, on 2 different media, with 1 stored offsite",
                                        "C) Back up every 3 days to 2 locations",
                                        "D) 3 antivirus scans, 2 firewalls, 1 VPN" },
                CorrectAnswer = "B",
                Explanation   = "The 3-2-1 rule ensures redundancy: 3 copies, 2 different media types, and 1 offsite backup protects against most failure scenarios.",
                IsTrueFalse   = false
            }
        };
    }
}
