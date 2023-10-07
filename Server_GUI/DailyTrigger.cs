using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Server_GUI
{
    /// <summary>
    /// Utility class for triggering an event every 24 hours at a specified time of day
    /// </summary>
    public class DailyTrigger : IDisposable
    {
        /// <summary>
        /// Time of day (from 00:00:00) to trigger
        /// </summary>
        TimeSpan TriggerHour { get; }

        /// <summary>
        /// Task cancellation token source to cancel delayed task on disposal
        /// </summary>
        public CancellationTokenSource CancellationToken { get; set; }

        /// <summary>
        /// Reference to the running task
        /// </summary>
        Task RunningTask { get; set; }

        public bool stopTask = false;

        public DailyTrigger(int hour, int minute = 0, int second = 0)
        {
            TriggerHour = new TimeSpan(hour, minute, second);
            CancellationToken = new CancellationTokenSource();

            RunningTask = Task.Run(async () =>
            {
                while (true)
                {
                    var triggerTime = DateTime.Today + TriggerHour - DateTime.Now;
                    if (triggerTime < TimeSpan.Zero)
                        triggerTime = triggerTime.Add(new TimeSpan(24, 0, 0));

                    await Task.Delay(triggerTime, CancellationToken.Token);
                    OnTimeTriggered?.Invoke();

                    if (CancellationToken.Token.IsCancellationRequested)
                    {
                        // another thread decided to cancel
                        Debug.WriteLine("task canceled");
                        break;
                    }

                }
            }, CancellationToken.Token);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            CancellationToken?.Cancel();
            CancellationToken?.Dispose();
            CancellationToken = null;
            RunningTask?.Dispose();
            RunningTask = null;
        }

        /// <summary>
        /// Triggers once every 24 hours on the specified time
        /// </summary>
        public event Action OnTimeTriggered;

        /// <summary>
        /// Finalized to ensure Dispose is called when out of scope
        /// </summary>
        ~DailyTrigger() => Dispose();
    }
}
