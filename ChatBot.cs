using CyberWare_With_ASM_PART2;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CyberWareASM
{
    public class ChatBot
    {
        // ── Injected dependencies ──────────────────────────────────────
        private readonly KeywordResponder _keywords;
        private readonly TopicStore _topics;
        private readonly SentimentDetector _sentiment;
        private readonly MemoryStore _memory;
        private readonly TaskManager _taskManager;
        private readonly ActivityLogger _logger;
        private readonly Random _rng = new();

        // ── Conversation state ─────────────────────────────────────────
        private string _lastTopic = string.Empty;
        private bool _awaitingReminder = false;   // true after adding a task without a reminder
        private string _pendingTaskTitle = string.Empty;

        // ── Fallback responses ─────────────────────────────────────────
        private readonly List<string> _fallbacks = new()
        {
            "Hmm, I'm not sure how to answer that. Try asking about phishing, passwords, malware or VPNs!",
            "That's outside my current knowledge. Type 'help' for a list of topics I can assist with.",
            "I didn't quite catch that. Could you rephrase, or ask a cybersecurity-related question?",
            "I'm still learning! Try topics like encryption, firewalls, ransomware or data breaches.",
            "Not sure about that one. Ask me about social engineering, cloud security, or 2FA — I'll be more helpful!"
        };

        // ── Casual conversation responses ──────────────────────────────
        private readonly Dictionary<string, List<string>> _casual = new()
        {
            ["how are you"] = new List<string>
            {
                "I'm doing great, thanks for asking! All systems are online and ready to help you stay secure. 🛡️",
                "Running at full capacity! No vulnerabilities detected in my circuits today. 😄 How can I help you?",
                "Fantastic! I'm here, alert and ready to tackle any cybersecurity question you throw at me. 🤖"
            },
            ["what is your name"] = new List<string>
            {
                "I'm CyberWare — your personal cybersecurity assistant, built with ASM! 🔐",
                "My name is CyberWare. I was created to keep you informed and secure in the digital world."
            },
            ["who are you"] = new List<string>
            {
                "I'm CyberWare — an AI-powered cybersecurity chatbot designed to educate and empower you online. 🤖🛡️",
                "I'm your digital security companion! Ask me anything about staying safe in the digital world."
            },
            ["what can you do"] = new List<string>
            {
                "I can answer questions on phishing, passwords, malware, privacy, scams, VPNs, encryption, firewalls and much more!\n\nI can also:\n• 📋 Add and manage cybersecurity tasks\n• 🎮 Run an interactive quiz\n• 📜 Show your activity log\n\nType 'help' for the full topic list. 🚀",
                "I detect your emotional tone, remember your name and favourite topic, manage your tasks, quiz you on cybersecurity, and give you tailored advice. Ask away!"
            },
            ["help"] = new List<string>
            {
                "Here's what I know about:\n• Phishing & Scams\n• Passwords & 2FA\n• Malware & Ransomware\n• Privacy & VPN\n• Firewalls & Network Security\n• Encryption & Cryptography\n• Social Engineering\n• Cloud Security\n• Data Breaches\n• Cyber Awareness\n\n📋 Task commands:\n• 'add task - [title]'\n• 'remind me to [action]'\n\n🎮 Quiz: 'start quiz'\n📜 Log: 'show activity log'\n\nJust type any topic and I'll help! 💬",
                "Type any cybersecurity topic and I'll respond! You can also say 'tell me more' to expand on the last topic, 'start quiz' to test your knowledge, or 'show activity log' to see recent actions. 🔐"
            },
            ["thank"] = new List<string>
            {
                "You're very welcome! Cybersecurity is everyone's responsibility — glad I could help. 🛡️",
                "My pleasure! Stay safe out there, and feel free to ask more anytime. 😊",
                "Anytime! Knowledge is your strongest defence. 💪"
            },
            ["bye"] = new List<string>
            {
                "Goodbye! Stay vigilant and secure out there! 👋🔐",
                "Take care! Remember — strong passwords, updated software, and healthy scepticism. 👋",
                "See you next time! Stay cyber-safe! 🛡️"
            },
            ["hello"] = new List<string>
            {
                "Hello! Great to have you here. What cybersecurity topic can I help you with today? 👋",
                "Hey there! CyberWare is ready. What would you like to learn about? 😊"
            },
            ["hi"] = new List<string>
            {
                "Hi! Ready to explore some cybersecurity knowledge? 🔐",
                "Hey! Ask me anything security-related and I'll do my best to help. 🤖"
            },
            ["good morning"] = new List<string>
            {
                "Good morning! Starting the day with some cybersecurity awareness is a great habit. ☀️",
                "Morning! Let's make today a secure one. What would you like to know? 🌅"
            },
            ["good afternoon"] = new List<string>
            {
                "Good afternoon! What cybersecurity topic are we exploring today? ☀️",
                "Afternoon! Ready to help you stay safe in the digital world. 🔐"
            },
            ["good evening"] = new List<string>
            {
                "Good evening! Stay safe online tonight. What can I help you with? 🌙",
                "Evening! Perfect time to brush up on some cyber hygiene. What do you need? 🌙"
            },
            ["purpose"] = new List<string>
            {
                "My purpose is to educate and empower you to stay safe in the digital world. Knowledge is your greatest defence! 🔐",
                "I was created to be your personal cybersecurity guide — helping you understand threats, best practices and how to stay protected online."
            },
            ["who created you"] = new List<string>
            {
                "I was built as part of the CyberWare With ASM project — a cybersecurity chatbot designed to make digital safety accessible to everyone. 🛡️",
                "I was created by ASM as an intelligent cybersecurity assistant. My mission is to make you safer online! 🤖"
            },
            ["interesting"] = new List<string>
            {
                "Cybersecurity is genuinely fascinating! The cat-and-mouse between attackers and defenders never stops evolving. 💡",
                "Right?! The more you learn about it, the more you realise how much there is to discover. Keep the curiosity going! 🔍"
            },
            ["okay"] = new List<string>
            {
                "Great! Feel free to ask me anything else. I'm here whenever you need. 😊",
                "Sounds good! What else can I help you with? 🔐"
            }
        };

        // ── Constructor ────────────────────────────────────────────────

        public ChatBot(MemoryStore memory, TaskManager taskManager, ActivityLogger logger)
        {
            _memory = memory;
            _taskManager = taskManager;
            _logger = logger;
            _keywords = new KeywordResponder();
            _topics = new TopicStore();
            _sentiment = new SentimentDetector();
        }

        // ── Public API ─────────────────────────────────────────────────

        public string GetGreeting()
        {
            return $"👋 Welcome, {_memory.FirstName}! I'm CyberWare — your intelligent cybersecurity assistant.\n\n" +
                   $"I'm here to help you navigate the digital world safely. You can ask me about:\n" +
                   $"• Phishing, Passwords, Malware, Privacy & Scams\n" +
                   $"• VPNs, Encryption, Firewalls & Network Security\n" +
                   $"• Social Engineering, Cloud Security & much more!\n\n" +
                   $"📋 Type 'add task - [title]' to create a cybersecurity task.\n" +
                   $"🎮 Type 'start quiz' or press the Quiz button to test your knowledge.\n" +
                   $"📜 Type 'show activity log' to see recent actions.\n\n" +
                   $"Type 'help' at any time to see all available topics. Let's get started, {_memory.FirstName}! 🛡️";
        }

        public string ProcessInput(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return "Please type a message so I can help you! 😊";

            string lower = input.Trim().ToLowerInvariant();

            // ══════════════════════════════════════════════════════════
            //  STEP 1 — REMINDER AWAITING (must be first!)
            //  After adding a task without a reminder, we wait for the
            //  user's follow-up to capture the reminder text.
            // ══════════════════════════════════════════════════════════
            if (_awaitingReminder)
            {
                _awaitingReminder = false;

                if (lower.Contains("no") || lower.Contains("skip") || lower.Contains("none"))
                {
                    _logger.Log($"No reminder set for task: '{_pendingTaskTitle}'");
                    _pendingTaskTitle = string.Empty;
                    return "No problem! Your task has been saved without a reminder. ✅";
                }

                // Treat the entire user input as the reminder text
                string reminder = input.Trim();
                _storage_SetReminder(_pendingTaskTitle, reminder);
                string title = _pendingTaskTitle;
                _pendingTaskTitle = string.Empty;
                _logger.Log($"Reminder set for task '{title}': {reminder}");
                return $"Got it! I'll remind you: **{reminder}** for '{title}'. ⏰";
            }

            // ══════════════════════════════════════════════════════════
            //  STEP 2 — ADD TASK INTENT
            //  Phrases: 'add task', 'add a task', 'create task',
            //           'i need to', 'remind me to', 'set up', 'enable'
            // ══════════════════════════════════════════════════════════
            if (IsAddTaskIntent(lower))
            {
                string taskTitle = ExtractTaskTitle(lower, input);
                string description = GenerateTaskDescription(taskTitle);

                // Check if a reminder was included in the same message
                string inlineReminder = ExtractInlineReminder(lower);

                string response = _taskManager.AddTask(taskTitle, description, inlineReminder);
                _logger.Log($"NLP recognised task intent from: '{input.Trim()}'");

                // If no inline reminder, ask for one
                if (string.IsNullOrWhiteSpace(inlineReminder))
                {
                    _awaitingReminder = true;
                    _pendingTaskTitle = taskTitle;
                    response += "\n\nWould you like a reminder? Say 'remind me in X days' or 'no'.";
                }

                // Signal to MainWindow to refresh the task list
                return "TASK_ADDED|" + response;
            }

            // ══════════════════════════════════════════════════════════
            //  STEP 3 — REMINDER INTENT (standalone)
            //  'remind me in 3 days', 'set a reminder for ...'
            // ══════════════════════════════════════════════════════════
            if (IsReminderIntent(lower) && !IsAddTaskIntent(lower))
            {
                string reminder = input.Trim();
                _logger.Log($"Reminder set: '{reminder}'");
                return $"⏰ Reminder noted: **{reminder}**\n\nIf you'd like to attach this to a specific task, say 'add task - [title]' and I'll set the reminder at the same time.";
            }

            // ══════════════════════════════════════════════════════════
            //  STEP 4 — QUIZ INTENT
            //  'start quiz', 'take quiz', 'test my knowledge', 'quiz me'
            // ══════════════════════════════════════════════════════════
            if (IsQuizIntent(lower))
            {
                _logger.Log("Quiz started");
                return "LAUNCH_QUIZ";   // MainWindow intercepts this signal
            }

            // ══════════════════════════════════════════════════════════
            //  STEP 5 — ACTIVITY LOG INTENT
            //  'show activity log', 'what have you done', 'show log'
            // ══════════════════════════════════════════════════════════
            if (IsLogIntent(lower))
            {
                string log = _logger.GetRecentLog(10);
                bool hasMore = _logger.HasMore(10);
                _logger.Log("Activity log viewed");

                string reply = $"📜 Here's a summary of recent actions:\n\n{log}";
                if (hasMore)
                    reply += "\n\n💡 Type 'show more log' to see the full history.";
                return reply;
            }

            if (lower.Contains("show more log") || lower.Contains("show full log"))
            {
                return $"📜 Full activity history:\n\n{_logger.GetFullLog()}";
            }

            // ══════════════════════════════════════════════════════════
            //  STEP 6 — FOLLOW-UP HANDLING  (Part 2 preserved)
            // ══════════════════════════════════════════════════════════
            if (IsFollowUp(lower))
            {
                if (!string.IsNullOrEmpty(_lastTopic))
                {
                    string? more = _keywords.GetResponse(_lastTopic)
                                   ?? _topics.GetResponse(_lastTopic);
                    if (more != null)
                    {
                        _logger.Log($"Follow-up expanded on topic: {CapFirst(_lastTopic)}");
                        return $"Sure! Here's more on **{CapFirst(_lastTopic)}**:\n\n{more}\n\n💡 Say 'tell me more' again for another tip on this topic!";
                    }
                }
                return "I don't have a previous topic saved to expand on. What would you like to know more about? 🤔";
            }

            // ══════════════════════════════════════════════════════════
            //  STEP 7 — CASUAL CONVERSATION  (Part 2 preserved)
            // ══════════════════════════════════════════════════════════
            foreach (var kvp in _casual)
            {
                if (lower.Contains(kvp.Key))
                {
                    int idx = _rng.Next(kvp.Value.Count);
                    string response = kvp.Value[idx];
                    if (!lower.Contains("name") && _rng.Next(3) == 0)
                        response += $" (You're in safe hands, {_memory.FirstName} 😊)";
                    return response;
                }
            }

            // ══════════════════════════════════════════════════════════
            //  STEP 8 — FAVOURITE TOPIC DETECTION  (Part 2 preserved)
            // ══════════════════════════════════════════════════════════
            DetectFavouriteTopic(lower);

            // ══════════════════════════════════════════════════════════
            //  STEP 9 — SENTIMENT DETECTION  (Part 2 preserved)
            // ══════════════════════════════════════════════════════════
            var sentiment = _sentiment.Detect(lower);
            string opener = _sentiment.GetSentimentResponse(sentiment);

            // ══════════════════════════════════════════════════════════
            //  STEP 10 — KEYWORD LOOKUP  (Part 2 preserved)
            // ══════════════════════════════════════════════════════════
            string? keywordResponse = _keywords.GetResponse(lower);
            if (keywordResponse != null)
            {
                SetLastTopic(lower, _keywords.GetAllKeywords());
                _logger.Log($"Keyword matched: {_lastTopic} — response delivered");
                string personalised = _memory.GetPersonalisedOpener();
                return $"{opener}{personalised}{keywordResponse}\n\n💡 Type 'tell me more' for another tip on this topic!";
            }

            // ══════════════════════════════════════════════════════════
            //  STEP 11 — TOPIC STORE LOOKUP  (Part 2 preserved)
            // ══════════════════════════════════════════════════════════
            string? topicResponse = _topics.GetResponse(lower);
            if (topicResponse != null)
            {
                SetLastTopic(lower, _topics.GetAllTopics());
                _logger.Log($"Topic matched: {_lastTopic} — response delivered");
                string personalised = _memory.GetPersonalisedOpener();
                return $"{opener}{personalised}{topicResponse}\n\n💡 Type 'tell me more' for another tip on this topic!";
            }

            // ══════════════════════════════════════════════════════════
            //  STEP 12 — SENTIMENT-ONLY FALLBACK  (Part 2 preserved)
            // ══════════════════════════════════════════════════════════
            if (sentiment != Sentiment.Neutral)
            {
                return $"{opener}I noticed something in your message. Here's a general tip, {_memory.FirstName}:\n\n" +
                       "🛡️ Always keep your software updated, use unique strong passwords for every account, " +
                       "enable 2FA where possible, and stay sceptical of unexpected messages. " +
                       "Feel free to ask about a specific topic for detailed advice!";
            }

            // ══════════════════════════════════════════════════════════
            //  STEP 13 — RANDOM FALLBACK
            // ══════════════════════════════════════════════════════════
            return _fallbacks[_rng.Next(_fallbacks.Count)];
        }

        // ──────────────────────────────────────────────────────────────
        //  NLP INTENT DETECTION
        // ──────────────────────────────────────────────────────────────

        private static bool IsAddTaskIntent(string lower)
            => lower.Contains("add task")
            || lower.Contains("add a task")
            || lower.Contains("create task")
            || lower.Contains("create a task")
            || lower.Contains("new task")
            || lower.Contains("i need to")
            || lower.Contains("set up")
            || lower.Contains("enable")
            || (lower.Contains("remind me to") && !lower.Contains("remind me in"));

        private static bool IsReminderIntent(string lower)
            => lower.Contains("remind me")
            || lower.Contains("reminder")
            || lower.Contains("set a reminder")
            || lower.Contains("don't forget")
            || lower.Contains("dont forget")
            || lower.Contains("remind me in");

        private static bool IsQuizIntent(string lower)
            => lower.Contains("start quiz")
            || lower.Contains("take quiz")
            || lower.Contains("begin quiz")
            || lower.Contains("test my knowledge")
            || lower.Contains("quiz me")
            || lower.Contains("play the game")
            || lower.Contains("play quiz")
            || lower.Contains("launch quiz");

        private static bool IsLogIntent(string lower)
            => lower.Contains("show activity log")
            || lower.Contains("show log")
            || lower.Contains("activity log")
            || lower.Contains("what have you done")
            || lower.Contains("what did you do")
            || lower.Contains("recent actions")
            || lower.Contains("show history");

        // ──────────────────────────────────────────────────────────────
        //  NLP HELPERS
        // ──────────────────────────────────────────────────────────────

        /// <summary>
        /// Strips intent-trigger phrases to surface the actual task title.
        /// Falls back to the raw input if nothing can be extracted.
        /// </summary>
        private static string ExtractTaskTitle(string lower, string original)
        {
            // Ordered by specificity — longest phrases first
            string[] prefixes =
            {
                "add task - ", "add task -", "add a task to ", "add a task -", "add task to ",
                "create a task to ", "create task to ", "create task - ", "create a task - ",
                "new task - ", "new task to ", "remind me to ", "i need to ", "set up ", "enable "
            };

            string working = lower;
            foreach (var p in prefixes)
            {
                int idx = working.IndexOf(p, StringComparison.Ordinal);
                if (idx >= 0)
                {
                    // Grab the remainder from the original (preserves casing)
                    string title = original.Substring(idx + p.Length).Trim();
                    // Remove trailing inline reminder fragments
                    foreach (var rem in new[] { " in ", " tomorrow", " next week", " on " })
                    {
                        int cut = title.ToLowerInvariant().IndexOf(rem, StringComparison.Ordinal);
                        if (cut > 0) { title = title[..cut].Trim(); break; }
                    }
                    return CapFirst(title);
                }
            }

            // Last resort: capitalise the whole input
            return CapFirst(original.Trim());
        }

        /// <summary>
        /// Generates a helpful, cybersecurity-relevant description for a task
        /// based on keywords found in the title.
        /// </summary>
        private static string GenerateTaskDescription(string title)
        {
            string t = title.ToLowerInvariant();

            if (t.Contains("2fa") || t.Contains("two-factor") || t.Contains("two factor"))
                return "Set up two-factor authentication on all important accounts to add an extra layer of security.";
            if (t.Contains("password"))
                return "Review and update your passwords. Use a password manager and ensure each account has a unique, strong password.";
            if (t.Contains("privacy"))
                return "Review account privacy settings to ensure your personal information is only visible to intended audiences.";
            if (t.Contains("vpn"))
                return "Configure and use a VPN, especially on public Wi-Fi, to encrypt your internet traffic.";
            if (t.Contains("antivirus") || t.Contains("malware"))
                return "Run or update your antivirus / anti-malware software to ensure your device is protected.";
            if (t.Contains("backup"))
                return "Back up your important data to at least two locations, including one offsite or cloud backup.";
            if (t.Contains("update") || t.Contains("patch"))
                return "Keep your software and operating system up to date to protect against known vulnerabilities.";
            if (t.Contains("firewall"))
                return "Check and configure your firewall settings to block unauthorised network access.";
            if (t.Contains("phishing"))
                return "Review how to spot phishing emails and report suspicious messages to your email provider or IT team.";

            // Generic fallback
            return $"Complete the cybersecurity task: {title}.";
        }

        /// <summary>Extracts a reminder phrase embedded in the same message as the task.</summary>
        private static string ExtractInlineReminder(string lower)
        {
            string[] triggers = { "remind me in ", "reminder in ", "in " };

            foreach (var t in triggers)
            {
                int idx = lower.IndexOf(t, StringComparison.Ordinal);
                if (idx >= 0)
                {
                    string candidate = lower.Substring(idx).Trim();
                    // Only use it if it looks time-related
                    if (candidate.Contains("day") || candidate.Contains("week")
                        || candidate.Contains("hour") || candidate.Contains("month")
                        || candidate.Contains("tomorrow"))
                        return CapFirst(candidate);
                }
            }

            if (lower.Contains("tomorrow"))
                return "Tomorrow";

            return string.Empty;
        }

        /// <summary>
        /// Updates the reminder for an already-added task.
        /// Used when the user answers "Yes" to the "Would you like a reminder?" prompt.
        /// This loads the most recent matching task by title and updates it in storage.
        /// </summary>
        private void _storage_SetReminder(string taskTitle, string reminder)
        {
            // Reach into storage through the TaskManager's underlying helper
            // by re-adding via TaskManager with the reminder attached.
            // Because TaskManager calls _storage directly we need to do this via
            // the storage the TaskManager owns.  The cleanest approach without
            // exposing internals is to load/save through a fresh helper here.
            var helper = new TaskStorageHelper();
            var tasks = helper.LoadTasks();

            // Find the last task whose title matches
            for (int i = tasks.Count - 1; i >= 0; i--)
            {
                if (string.Equals(tasks[i].Title, taskTitle, StringComparison.OrdinalIgnoreCase))
                {
                    tasks[i].Reminder = reminder;
                    helper.SaveTasks(tasks);
                    return;
                }
            }
        }

        // ──────────────────────────────────────────────────────────────
        //  PART 2 PRESERVED HELPERS
        // ──────────────────────────────────────────────────────────────

        private static bool IsFollowUp(string lower)
            => lower.Contains("tell me more")
            || lower.Contains("explain more")
            || lower.Contains("more info")
            || lower.Contains("elaborate")
            || lower.Contains("expand on that")
            || lower.Contains("give me more");

        private void DetectFavouriteTopic(string lower)
        {
            var triggerPhrases = new[] { "interested in", "love", "favourite topic", "favorite topic", "i like", "i enjoy" };
            foreach (var phrase in triggerPhrases)
            {
                if (!lower.Contains(phrase)) continue;

                var allTopics = _keywords.GetAllKeywords().Concat(_topics.GetAllTopics());
                foreach (var topic in allTopics)
                {
                    if (lower.Contains(topic.ToLowerInvariant()))
                    {
                        _memory.FavouriteTopic = CapFirst(topic);
                        _lastTopic = topic;
                        return;
                    }
                }
            }
        }

        private void SetLastTopic(string lower, List<string> topicList)
        {
            foreach (var topic in topicList)
                if (lower.Contains(topic.ToLowerInvariant()))
                {
                    _lastTopic = topic;
                    return;
                }
        }

        private static string CapFirst(string s)
            => string.IsNullOrEmpty(s) ? s : char.ToUpper(s[0]) + s[1..];
    }
}