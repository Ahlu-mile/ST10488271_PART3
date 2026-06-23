using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Threading;

namespace CyberWareASM
{
    // ══════════════════════════════════════════════════════════════════
    //  Helper ViewModel so the ListView can bind IsComplete as "✔" / "–"
    // ══════════════════════════════════════════════════════════════════
    public class TaskViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Reminder { get; set; } = string.Empty;
        public bool IsComplete { get; set; }
        public string IsCompleteDisplay => IsComplete ? "✔" : "–";
        public string CreatedAt { get; set; } = string.Empty;
    }

    public partial class MainWindow : Window
    {
        // ──────────────────────────────────────────────────────────────
        //  FIELDS
        // ──────────────────────────────────────────────────────────────
        private MemoryStore _memory = new();
        private ActivityLogger _logger = new();
        private TaskManager _taskManager = null!;
        private ChatBot _chatBot = null!;
        private AudioPlayer _audio = new();
        private QuizManager _quiz = null!;

        // ──────────────────────────────────────────────────────────────
        //  CONSTRUCTOR
        // ──────────────────────────────────────────────────────────────
        public MainWindow()
        {
            InitializeComponent();
            Loaded += (_, _) => TxtFirstName.Focus();
        }

        // ══════════════════════════════════════════════════════════════
        //  NAME SCREEN — SUBMIT
        // ══════════════════════════════════════════════════════════════
        private void BtnSubmitName_Click(object sender, RoutedEventArgs e)
        {
            string firstName = TxtFirstName.Text.Trim();
            string surname = TxtSurname.Text.Trim();

            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(surname))
            {
                TxtValidation.Visibility = Visibility.Visible;
                return;
            }
            TxtValidation.Visibility = Visibility.Collapsed;

            // Populate shared state
            _memory.FirstName = firstName;
            _memory.LastName = surname;

            // Wire up Part 3 classes
            _taskManager = new TaskManager(_logger);
            _chatBot = new ChatBot(_memory, _taskManager, _logger);
            _quiz = new QuizManager();

            // Update sidebar
            TxtSidebarName.Text = _memory.FullName;

            // Transition to chat screen
            NameScreen.Visibility = Visibility.Collapsed;
            ChatScreen.Visibility = Visibility.Visible;

            // Load saved tasks immediately so the task panel is populated
            RefreshTaskList();

            _audio.PlayGreeting();
            AppendBotMessage(_chatBot.GetGreeting());
            TxtUserInput.Focus();
        }

        // ══════════════════════════════════════════════════════════════
        //  CHAT — SEND
        // ══════════════════════════════════════════════════════════════
        private void BtnSend_Click(object sender, RoutedEventArgs e) => SendMessage();
        private void TxtUserInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) SendMessage();
        }

        private void SendMessage()
        {
            if (_chatBot == null) return;

            string userText = TxtUserInput.Text.Trim();
            if (string.IsNullOrWhiteSpace(userText)) return;

            AppendUserMessage(userText);
            TxtUserInput.Clear();

            string botResponse = _chatBot.ProcessInput(userText);
            HandleBotResponse(botResponse);

            SyncSidebarTopic();
        }

        /// <summary>
        /// Central dispatcher for ChatBot responses.
        /// Intercepts special signal prefixes before displaying.
        /// </summary>
        private void HandleBotResponse(string response)
        {
            if (response == "LAUNCH_QUIZ")
            {
                OpenQuizPanel();
                AppendBotMessage("🎮 Launching the Cybersecurity Quiz! Good luck!");
                return;
            }

            if (response.StartsWith("TASK_ADDED|"))
            {
                string msg = response["TASK_ADDED|".Length..];
                AppendBotMessage(msg);
                RefreshTaskList();
                return;
            }

            AppendBotMessage(response);
        }

        // ══════════════════════════════════════════════════════════════
        //  END CONVERSATION
        // ══════════════════════════════════════════════════════════════
        private void BtnEndConvo_Click(object sender, RoutedEventArgs e)
        {
            AppendBotMessage($"🔒  Session terminated. Stay safe out there, {_memory.FirstName}. Goodbye!");
            TxtUserInput.IsEnabled = false;
            BtnSend.IsEnabled = false;
            BtnQuiz.IsEnabled = false;
            BtnEndConvo.IsEnabled = false;

            var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(2.5) };
            timer.Tick += (_, _) => { timer.Stop(); Application.Current.Shutdown(); };
            timer.Start();
        }

        // ══════════════════════════════════════════════════════════════
        //  SIDEBAR TOPIC & QUICK-ASK BUTTONS
        // ══════════════════════════════════════════════════════════════
        private void TopicBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_chatBot == null || sender is not Button btn) return;
            string query = btn.Tag?.ToString() ?? btn.Content?.ToString() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(query)) return;

            ShowChatArea();
            AppendUserMessage(btn.Content?.ToString() ?? query);
            HandleBotResponse(_chatBot.ProcessInput(query));
            SyncSidebarTopic();
        }

        private void QuickBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_chatBot == null || sender is not Button btn) return;
            string query = btn.Tag?.ToString() ?? btn.Content?.ToString() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(query)) return;

            ShowChatArea();
            AppendUserMessage(btn.Content?.ToString() ?? query);
            HandleBotResponse(_chatBot.ProcessInput(query));
            SyncSidebarTopic();
        }

        // ══════════════════════════════════════════════════════════════
        //  PANEL NAVIGATION HELPERS
        // ══════════════════════════════════════════════════════════════

        private void ShowChatArea()
        {
            ChatAreaBorder.Visibility = Visibility.Visible;
            TaskPanel.Visibility = Visibility.Collapsed;
            QuizPanel.Visibility = Visibility.Collapsed;
        }

        private void ShowTaskPanel()
        {
            ChatAreaBorder.Visibility = Visibility.Collapsed;
            QuizPanel.Visibility = Visibility.Collapsed;
            TaskPanel.Visibility = Visibility.Visible;
        }

        private void ShowQuizPanel()
        {
            ChatAreaBorder.Visibility = Visibility.Collapsed;
            TaskPanel.Visibility = Visibility.Collapsed;
            QuizPanel.Visibility = Visibility.Visible;
        }

        // ══════════════════════════════════════════════════════════════
        //  TASK ASSISTANT PANEL
        // ══════════════════════════════════════════════════════════════

        private void BtnOpenTaskPanel_Click(object sender, RoutedEventArgs e)
        {
            RefreshTaskList();
            ShowTaskPanel();
        }

        private void BtnCloseTaskPanel_Click(object sender, RoutedEventArgs e)
            => ShowChatArea();

        private void BtnAddTask_Click(object sender, RoutedEventArgs e)
        {
            string title = TxtTaskTitle.Text.Trim();
            string description = TxtTaskDescription.Text.Trim();
            string reminder = TxtTaskReminder.Text.Trim();

            if (string.IsNullOrWhiteSpace(title))
            {
                AppendBotMessage("⚠️ Please enter a task title before adding.");
                return;
            }

            if (string.IsNullOrWhiteSpace(description))
                description = $"Complete the cybersecurity task: {title}.";

            string result = _taskManager.AddTask(title, description, reminder);

            // Clear form
            TxtTaskTitle.Clear();
            TxtTaskDescription.Clear();
            TxtTaskReminder.Clear();

            RefreshTaskList();

            // Echo confirmation into chat
            AppendBotMessage(result);
        }

        private void BtnMarkComplete_Click(object sender, RoutedEventArgs e)
        {
            if (TaskListView.SelectedItem is not TaskViewModel selected) return;

            _taskManager.MarkAsComplete(selected.Id);
            RefreshTaskList();
            AppendBotMessage($"✔️ Task '{selected.Title}' marked as complete!");
        }

        private void BtnDeleteTask_Click(object sender, RoutedEventArgs e)
        {
            if (TaskListView.SelectedItem is not TaskViewModel selected) return;

            _taskManager.DeleteTask(selected.Id);
            RefreshTaskList();
            AppendBotMessage($"🗑️ Task '{selected.Title}' has been deleted.");
        }

        /// <summary>Reloads tasks from JSON and rebuilds the ListView ItemsSource.</summary>
        private void RefreshTaskList()
        {
            if (_taskManager == null) return;

            var tasks = _taskManager.GetAllTasks();
            TaskListView.ItemsSource = tasks.Select(t => new TaskViewModel
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                Reminder = t.Reminder,
                IsComplete = t.IsComplete,
                CreatedAt = t.CreatedAt
            }).ToList();
        }

        // ══════════════════════════════════════════════════════════════
        //  QUIZ PANEL
        // ══════════════════════════════════════════════════════════════

        private void BtnOpenQuizPanel_Click(object sender, RoutedEventArgs e)
        {
            OpenQuizPanel();
        }

        private void OpenQuizPanel()
        {
            if (_quiz == null) return;

            _quiz.ResetQuiz();
            _logger.Log("Quiz started");
            ShowQuizPanel();
            ResultsBorder.Visibility = Visibility.Collapsed;
            FeedbackBorder.Visibility = Visibility.Collapsed;
            BtnNextQuestion.Visibility = Visibility.Collapsed;
            BtnSubmitAnswer.Visibility = Visibility.Visible;
            LoadCurrentQuestion();
        }

        private void BtnCloseQuizPanel_Click(object sender, RoutedEventArgs e)
            => ShowChatArea();

        private void BtnSubmitAnswer_Click(object sender, RoutedEventArgs e)
        {
            if (_quiz == null || _quiz.IsFinished()) return;

            // Determine selected answer
            string? answer = GetSelectedAnswer();
            if (answer == null)
            {
                FeedbackBorder.Visibility = Visibility.Visible;
                TxtQuizFeedback.Text = "⚠️ Please select an answer before submitting.";
                FeedbackBorder.Background = new SolidColorBrush(Color.FromRgb(45, 10, 10));
                return;
            }

            bool correct = _quiz.SubmitAnswer(answer);
            string feedback = _quiz.GetFeedback(correct);

            // Show feedback
            FeedbackBorder.Visibility = Visibility.Visible;
            TxtQuizFeedback.Text = feedback;
            FeedbackBorder.Background = correct
                ? new SolidColorBrush(Color.FromRgb(10, 40, 10))
                : new SolidColorBrush(Color.FromRgb(45, 10, 10));

            // Disable radio buttons so answer can't be changed
            SetOptionsEnabled(false);

            UpdateQuizProgress();

            if (_quiz.IsFinished())
            {
                ShowQuizResults();
            }
            else
            {
                BtnSubmitAnswer.Visibility = Visibility.Collapsed;
                BtnNextQuestion.Visibility = Visibility.Visible;
            }
        }

        private void BtnNextQuestion_Click(object sender, RoutedEventArgs e)
        {
            FeedbackBorder.Visibility = Visibility.Collapsed;
            BtnNextQuestion.Visibility = Visibility.Collapsed;
            BtnSubmitAnswer.Visibility = Visibility.Visible;
            LoadCurrentQuestion();
        }

        private void BtnPlayAgain_Click(object sender, RoutedEventArgs e)
        {
            OpenQuizPanel();
        }

        // ──────────────────────────────────────────────────────────────
        //  QUIZ HELPERS
        // ──────────────────────────────────────────────────────────────

        private void LoadCurrentQuestion()
        {
            var q = _quiz.GetCurrentQuestion();
            if (q == null) return;

            TxtQuizQuestion.Text = q.Question;

            // Clear all radio states
            RbA.IsChecked = RbB.IsChecked = RbC.IsChecked = RbD.IsChecked = false;
            RbTrue.IsChecked = RbFalse.IsChecked = false;
            SetOptionsEnabled(true);

            if (q.IsTrueFalse)
            {
                OptionsPanel.Visibility = Visibility.Collapsed;
                TrueFalsePanel.Visibility = Visibility.Visible;
            }
            else
            {
                OptionsPanel.Visibility = Visibility.Visible;
                TrueFalsePanel.Visibility = Visibility.Collapsed;

                // Assign option text
                var opts = q.Options;
                RbA.Content = opts.Count > 0 ? opts[0] : string.Empty;
                RbB.Content = opts.Count > 1 ? opts[1] : string.Empty;
                RbC.Content = opts.Count > 2 ? opts[2] : string.Empty;
                RbD.Content = opts.Count > 3 ? opts[3] : string.Empty;

                // Hide unused options
                RbC.Visibility = opts.Count > 2 ? Visibility.Visible : Visibility.Collapsed;
                RbD.Visibility = opts.Count > 3 ? Visibility.Visible : Visibility.Collapsed;
            }

            UpdateQuizProgress();
        }

        private string? GetSelectedAnswer()
        {
            var q = _quiz?.GetCurrentQuestion();
            if (q == null) return null;

            if (q.IsTrueFalse)
            {
                if (RbTrue.IsChecked == true) return "True";
                if (RbFalse.IsChecked == true) return "False";
                return null;
            }

            // Map RadioButton to letter answer
            if (RbA.IsChecked == true) return "A";
            if (RbB.IsChecked == true) return "B";
            if (RbC.IsChecked == true) return "C";
            if (RbD.IsChecked == true) return "D";
            return null;
        }

        private void SetOptionsEnabled(bool enabled)
        {
            RbA.IsEnabled = enabled;
            RbB.IsEnabled = enabled;
            RbC.IsEnabled = enabled;
            RbD.IsEnabled = enabled;
            RbTrue.IsEnabled = enabled;
            RbFalse.IsEnabled = enabled;
        }

        private void UpdateQuizProgress()
        {
            int displayed = Math.Min(_quiz.GetCurrentNum(), _quiz.GetTotal());
            TxtQuizProgress.Text =
                $"Question {displayed} of {_quiz.GetTotal()}  |  Score: {_quiz.GetScore()}";
        }

        private void ShowQuizResults()
        {
            OptionsPanel.Visibility = Visibility.Collapsed;
            TrueFalsePanel.Visibility = Visibility.Collapsed;
            BtnSubmitAnswer.Visibility = Visibility.Collapsed;
            BtnNextQuestion.Visibility = Visibility.Collapsed;
            FeedbackBorder.Visibility = Visibility.Collapsed;
            ResultsBorder.Visibility = Visibility.Visible;

            TxtFinalScore.Text = _quiz.GetFinalScore();
            TxtFinalMessage.Text = _quiz.GetFinalMessage();

            string logEntry = $"Quiz completed — score: {_quiz.GetScore()} out of {_quiz.GetTotal()}";
            _logger.Log(logEntry);
            AppendBotMessage($"🎮 Quiz finished! {_quiz.GetFinalScore()} {_quiz.GetFinalMessage()}");
        }

        // ══════════════════════════════════════════════════════════════
        //  CHAT BUBBLE HELPERS  (Part 2 preserved)
        // ══════════════════════════════════════════════════════════════
        private void AppendBotMessage(string text)
        {
            var row = new Grid { Margin = new Thickness(0, 6, 0, 6) };
            row.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(80) });

            var avatar = new Border
            {
                Width = 36,
                Height = 36,
                CornerRadius = new CornerRadius(18),
                Margin = new Thickness(0, 2, 10, 0),
                VerticalAlignment = VerticalAlignment.Top,
                Background = new LinearGradientBrush(
                    Color.FromRgb(59, 130, 246),
                    Color.FromRgb(168, 85, 247),
                    new Point(0, 0), new Point(1, 1))
            };
            avatar.Child = new TextBlock
            {
                Text = "🤖",
                FontSize = 17,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            Grid.SetColumn(avatar, 0);

            var bubble = new Border
            {
                CornerRadius = new CornerRadius(4, 16, 16, 16),
                Padding = new Thickness(16, 10, 16, 10),
                Background = new SolidColorBrush(Color.FromRgb(18, 18, 46)),
                Effect = new DropShadowEffect
                {
                    BlurRadius = 12,
                    ShadowDepth = 0,
                    Color = Color.FromRgb(59, 130, 246),
                    Opacity = 0.14
                }
            };
            bubble.Child = new TextBlock
            {
                Text = text,
                Foreground = new SolidColorBrush(Color.FromRgb(241, 245, 249)),
                FontSize = 14,
                TextWrapping = TextWrapping.Wrap,
                LineHeight = 22
            };
            Grid.SetColumn(bubble, 1);

            row.Children.Add(avatar);
            row.Children.Add(bubble);
            ChatMessagesPanel.Children.Add(row);
            ScrollToBottom();
        }

        private void AppendUserMessage(string text)
        {
            var row = new Grid { Margin = new Thickness(0, 6, 0, 6) };
            row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(80) });
            row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            row.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            var bubble = new Border
            {
                CornerRadius = new CornerRadius(16, 4, 16, 16),
                Padding = new Thickness(16, 10, 16, 10),
                HorizontalAlignment = HorizontalAlignment.Right,
                Background = new LinearGradientBrush(
                    Color.FromRgb(30, 27, 75),
                    Color.FromRgb(45, 27, 105),
                    new Point(0, 0), new Point(1, 1)),
                Effect = new DropShadowEffect
                {
                    BlurRadius = 12,
                    ShadowDepth = 0,
                    Color = Color.FromRgb(168, 85, 247),
                    Opacity = 0.20
                }
            };
            bubble.Child = new TextBlock
            {
                Text = text,
                Foreground = new SolidColorBrush(Color.FromRgb(241, 245, 249)),
                FontSize = 14,
                TextWrapping = TextWrapping.Wrap,
                LineHeight = 22
            };
            Grid.SetColumn(bubble, 1);

            var avatar = new Border
            {
                Width = 36,
                Height = 36,
                CornerRadius = new CornerRadius(18),
                Margin = new Thickness(10, 2, 0, 0),
                VerticalAlignment = VerticalAlignment.Top,
                Background = new LinearGradientBrush(
                    Color.FromRgb(236, 72, 153),
                    Color.FromRgb(168, 85, 247),
                    new Point(0, 0), new Point(1, 1))
            };
            avatar.Child = new TextBlock
            {
                Text = "👤",
                FontSize = 17,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            Grid.SetColumn(avatar, 2);

            row.Children.Add(bubble);
            row.Children.Add(avatar);
            ChatMessagesPanel.Children.Add(row);
            ScrollToBottom();
        }

        private void ScrollToBottom()
        {
            Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle,
                new Action(() => ChatScrollViewer.ScrollToBottom()));
        }

        private void SyncSidebarTopic()
        {
            TxtSidebarTopic.Text = string.IsNullOrWhiteSpace(_memory.FavouriteTopic)
                ? "Not set yet"
                : _memory.FavouriteTopic;
        }
    }
}