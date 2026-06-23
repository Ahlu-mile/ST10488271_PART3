namespace CyberWareASM
{
    /// <summary>
    /// Represents a single cybersecurity task stored in tasks.json.
    /// Named CyberTask to avoid collision with System.Threading.Tasks.Task.
    /// </summary>
    public class CyberTask
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Reminder { get; set; } = string.Empty;
        public bool IsComplete { get; set; }
        public string CreatedAt { get; set; } = string.Empty;
    }
}

