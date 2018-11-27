using System.Collections.Concurrent;

namespace IASoft.WPFCommons.Background
{
    /// <summary>
    /// Очередь задач, завершение которых нужно ждать при завершении приложения.
    /// </summary>
    public class BackgroundTasksQueue
    {
        public ConcurrentDictionary<string, string> ActiveBackgroundTasks { get; } = new ConcurrentDictionary<string, string>();

        public void AddBackgroundTask(string name)
        {
            this.ActiveBackgroundTasks.TryAdd(name, name);
        }

        public void RemoveBackgroundTask(string name)
        {
            this.ActiveBackgroundTasks.TryRemove(name, out _);
        }
    }
}