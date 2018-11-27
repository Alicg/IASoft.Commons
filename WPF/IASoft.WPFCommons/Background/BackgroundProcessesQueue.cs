using System.Collections.Generic;
using System.Threading;
using Utils.Extensions;

namespace IASoft.WPFCommons.Background
{
    /// <summary>
    /// Очередь процессов, которые можно принудительно завершить при завершении приложения.
    /// </summary>
    public class BackgroundProcessesQueue
    {
        private readonly IList<CancellationTokenSource> cancellationTokenSources = new List<CancellationTokenSource>();

        public void AddPendingProcessCancellationToken(CancellationTokenSource cancellationTokenSource)
        {
            this.cancellationTokenSources.Add(cancellationTokenSource);
        }

        public IEnumerable<CancellationTokenSource> GetAllCancellationTokenSources()
        {
            return this.cancellationTokenSources;
        }

        public void CancelAll()
        {
            this.cancellationTokenSources.ForEach(v => v.Cancel());
        }
    }
}