using System.Collections.Generic;

namespace CyberWareASM
{
    /// <summary>
    /// Business-logic layer that sits between the GUI and TaskStorageHelper.
    /// Every mutating operation is also logged via ActivityLogger.
    /// </summary>
    public class TaskManager
    {
        private readonly TaskStorageHelper _storage;
        private readonly ActivityLogger _logger;

        public TaskManager(ActivityLogger logger)
        {
            _storage = new TaskStorageHelper();
            _logger = logger;
        }

        // ──────────────────────────────────────────────────────────────
        //  CREATE
        // ──────────────────────────────────────────────────────────────

        /// <summary>
        /// Adds a task, persists it, logs the action, and returns a
        /// user-friendly confirmation string for display in the chat.
        /// </summary>
        public string AddTask(string title, string description, string reminder = "")
        {
            _storage.AddTask(title, description, reminder);

            string logEntry = string.IsNullOrWhiteSpace(reminder)
                ? $"Task added: '{title}' (no reminder set)"
                : $"Task added: '{title}' (Reminder: {reminder})";

            _logger.Log(logEntry);

            string reply = $"✅ Task added: **'{title}'**\n📝 {description}";
            if (!string.IsNullOrWhiteSpace(reminder))
                reply += $"\n⏰ Reminder: {reminder}";
            else
                reply += "\n\nWould you like to set a reminder for this task? Just say 'remind me in X days'.";

            return reply;
        }

        // ──────────────────────────────────────────────────────────────
        //  READ
        // ──────────────────────────────────────────────────────────────

        /// <summary>Returns all tasks from storage.</summary>
        public List<CyberTask> GetAllTasks() => _storage.LoadTasks();

        // ──────────────────────────────────────────────────────────────
        //  UPDATE
        // ──────────────────────────────────────────────────────────────

        /// <summary>Marks a task complete by Id and logs the action.</summary>
        public void MarkAsComplete(int id)
        {
            var tasks = _storage.LoadTasks();
            var task = tasks.Find(t => t.Id == id);
            if (task == null) return;

            _storage.MarkAsComplete(id);
            _logger.Log($"Task marked complete: '{task.Title}'");
        }

        // ──────────────────────────────────────────────────────────────
        //  DELETE
        // ──────────────────────────────────────────────────────────────

        /// <summary>Deletes a task by Id and logs the action.</summary>
        public void DeleteTask(int id)
        {
            var tasks = _storage.LoadTasks();
            var task = tasks.Find(t => t.Id == id);
            if (task == null) return;

            _storage.DeleteTask(id);
            _logger.Log($"Task deleted: '{task.Title}'");
        }
    }
}
