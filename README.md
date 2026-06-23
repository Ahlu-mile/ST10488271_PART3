# Project Description

CyberWare With ASM is a desktop chatbot that helps users learn about and
practise good cybersecurity habits. It greets the user by name, holds a
natural conversation about cybersecurity topics, detects the emotional tone
of messages, remembers the user's favourite topic, manages a personal task
list with reminders, runs a 12-question cybersecurity quiz, and keeps a
timestamped log of everything it does.

## Features by Part
### Part 1 — Console Foundations

ASCII art banner and voice greeting on launch
Name capture and personalised greeting
Keyword-based cybersecurity responses (phishing, passwords, malware, etc.)
Random response variation so the bot doesn't feel repetitive

### Part 2 — WPF GUI, Memory & Sentiment

Full graphical interface (WPF/XAML) replacing the console
Sentiment detection — the bot notices worry, frustration, curiosity, etc.,
and responds with empathy before giving advice
Memory of the user's name and favourite topic, used to personalise replies
"Tell me more" follow-up flow that continues the last topic discussed
Casual conversation handling (greetings, thanks, goodbyes, small talk)

### Part 3 — Task Assistant, Quiz, NLP & Activity Log

Task Assistant with Reminders: add, view, complete, and delete
cybersecurity tasks from either the chat or a dedicated Task panel. All
tasks are persisted to tasks.json next to the executable.
Cybersecurity Quiz: a 12-question interactive quiz covering phishing,
password safety, safe browsing, social engineering, 2FA, malware/ransomware,
privacy settings, and data backup. One question at a time, immediate
feedback with an explanation, running score, and a final results screen.
Launch it from the chat ("start quiz"), the sidebar, or the QUIZ button
next to Send.

NLP Simulation: keyword and phrase detection (using string.Contains())
lets the bot recognise task, reminder, quiz, and log requests phrased many
different ways — not just one exact sentence.
Activity Log: every significant action (task added/completed/deleted,
reminder set, quiz started/finished, keyword matched) is logged with a
timestamp. Type "show activity log" or "what have you done for me?" to see
the last 10 entries, with a "show more" option for the full history.

## How to Use the App

Enter your first name and surname on the welcome screen.
Chat naturally — ask about phishing, passwords, malware, privacy, VPNs,
2FA, and more.
Say "tell me more" to continue the last topic.
Say "add task - [title]" (e.g. "Add a task to enable two-factor
authentication") to create a task. The bot will ask if you'd like a
reminder.
Open the Task Assistant panel from the sidebar to view, complete, or
delete tasks directly.
Click the 🎮 QUIZ button (next to Send) or say "start quiz" to
launch the cybersecurity quiz.
Say "show activity log" or "what have you done for me?" to review
recent actions.
Click ⏻ END to close the session with a farewell message.

## Project Structure

CybersecurityChatbot/
 ├─ MainWindow.xaml / MainWindow.xaml.cs   # GUI shell, all event handlers
 ├─ ChatBot.cs                              # Conversation engine + NLP routing
 ├─ KeywordResponder.cs                     # Cybersecurity keyword responses
 ├─ TopicStore.cs                           # Extended topic responses
 ├─ SentimentDetector.cs                    # Sentiment analysis (unchanged from Part 2)
 ├─ MemoryStore.cs                          # User name & favourite topic (unchanged from Part 2)
 ├─ AudioPlayer.cs                          # Plays the greeting sound
 ├─ CyberTask.cs                            # Task data model
 ├─ TaskStorageHelper.cs                    # tasks.json read/write (CRUD)
 ├─ TaskManager.cs                          # Task business logic + logging
 ├─ QuizManager.cs                          # Quiz questions, scoring, flow
 ├─ ActivityLogger.cs                       # Timestamped activity log
 ├─ tasks.json                              # Auto-created task storage
 └─ Ahlumile.wav                            # Greeting sound
