using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;


namespace CyberWareASM
{
    /// <summary>
    /// Handles ALL reading and writing of the tasks.json file.
    /// No other class should touch the file directly.
    /// </summary>
    public class TaskStorageHelper
    {
        // tasks.json is created in the same folder as the .exe on first save.
        private const string FilePath = "tasks.json";

        // ──────────────────────────────────────────────────────────────
        //  READ
        // ──────────────────────────────────────────────────────────────

        /// <summary>
        /// Reads tasks.json and deserialises it to a List of CyberTask.
        /// Returns an empty list if the file does not exist or is corrupt.
        /// </summary>
        public List<CyberTask> LoadTasks()
        {
            try
            {
                if (!File.Exists(FilePath))
                    return new List<CyberTask>();

                string json = File.ReadAllText(FilePath);
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                return JsonSerializer.Deserialize<List<CyberTask>>(json, options)
                       ?? new List<CyberTask>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[TaskStorageHelper] LoadTasks error: {ex.Message}");
                return new List<CyberTask>();
            }
        }

        // ──────────────────────────────────────────────────────────────
        //  WRITE (full replace — simplest safe approach)
        // ──────────────────────────────────────────────────────────────

        /// <summary>
        /// Serialises the entire task list and writes it to tasks.json.
        /// This overwrites the file, giving us atomic Create / Update / Delete.
        /// </summary>
        public void SaveTasks(List<CyberTask> tasks)
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(tasks, options);
                File.WriteAllText(FilePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[TaskStorageHelper] SaveTasks error: {ex.Message}");
            }
        }

        // ──────────────────────────────────────────────────────────────
        //  CREATE
        // ──────────────────────────────────────────────────────────────

        /// <summary>
        /// Adds a new task, assigns the next available Id, and persists.
        /// </summary>
        public void AddTask(string title, string description, string reminder)
        {
            try
            {
                var tasks = LoadTasks();

                int nextId = tasks.Count > 0 ? tasks[^1].Id + 1 : 1;

                tasks.Add(new CyberTask
                {
                    Id = nextId,
                    Title = title,
                    Description = description,
                    Reminder = reminder,
                    IsComplete = false,
                    CreatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm")
                });

                SaveTasks(tasks);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[TaskStorageHelper] AddTask error: {ex.Message}");
            }
        }

        // ──────────────────────────────────────────────────────────────
        //  UPDATE
        // ──────────────────────────────────────────────────────────────

        /// <summary>
        /// Sets IsComplete = true for the task with the given Id and persists.
        /// </summary>
        public void MarkAsComplete(int id)
        {
            try
            {
                var tasks = LoadTasks();
                var task = tasks.Find(t => t.Id == id);
                if (task != null)
                {
                    task.IsComplete = true;
                    SaveTasks(tasks);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[TaskStorageHelper] MarkAsComplete error: {ex.Message}");
            }
        }

        // ──────────────────────────────────────────────────────────────
        //  DELETE
        // ──────────────────────────────────────────────────────────────

        /// <summary>
        /// Removes the task with the given Id and persists the updated list.
        /// </summary>
        public void DeleteTask(int id)
        {
            try
            {
                var tasks = LoadTasks();
                tasks.RemoveAll(t => t.Id == id);
                SaveTasks(tasks);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[TaskStorageHelper] DeleteTask error: {ex.Message}");
            }
        }
    }
}
