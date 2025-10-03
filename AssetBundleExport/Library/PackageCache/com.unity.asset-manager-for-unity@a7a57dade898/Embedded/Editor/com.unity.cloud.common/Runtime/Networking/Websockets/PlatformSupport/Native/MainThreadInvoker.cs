using System;
using System.Threading;
using System.Threading.Tasks;

namespace Unity.Cloud.CommonEmbedded.Runtime
{
    /// <summary>
    /// Utility class used to have methods run on the main thread
    /// </summary>
    class MainThreadInvoker
    {
        /// <summary>
        /// Cancellation token used to cancel the task started on the main thread
        /// </summary>
        readonly CancellationToken m_CancellationToken;

        /// <summary>
        /// Reference to task scheduler used to queue the tasks onto threads
        /// </summary>
        readonly TaskScheduler m_Scheduler;

        /// <summary>
        /// Default logging tool for Unity Cloud
        /// </summary>
        static readonly UCLogger s_Logger = LoggerProvider.GetLogger<MainThreadInvoker>();

        public MainThreadInvoker(CancellationToken cancellationToken)
            : this(TaskScheduler.FromCurrentSynchronizationContext(), cancellationToken)
        { }

        internal MainThreadInvoker(TaskScheduler scheduler, CancellationToken cancellationToken)
        {
            m_Scheduler = scheduler;
            m_CancellationToken = cancellationToken;
        }

        /// <summary>
        /// Invokes a method with no arguments, on the main thread
        /// </summary>
        /// <param name="eventInvoke"></param>
        public void InvokeMainThreadEvent(Action eventInvoke)
        {
            // Null check inside the task to avoid thread safety issues
            Task.Factory.StartNew(args =>
            {
                try
                {
                    eventInvoke?.Invoke();
                }
                catch (Exception err)
                {
                    s_Logger.LogError(err.Message);  // Otherwise silently discarded
                    throw;
                }
            }, null, m_CancellationToken, TaskCreationOptions.DenyChildAttach, m_Scheduler);
        }

        /// <summary>
        /// Invokes a method with a single arg, on the main thread
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="eventInvoke"></param>
        /// <param name="eventArg"></param>
        public void InvokeMainThreadEventWithArg<T>(Action<T> eventInvoke, T eventArg)
        {
            // Null check inside the task to avoid thread safety issues
            Task.Factory.StartNew(args =>
            {
                try
                {
                    eventInvoke?.Invoke((T)args);
                }
                catch (Exception err)
                {
                    s_Logger.LogError(err.Message);  // Otherwise silently discarded
                    throw;
                }
            }, eventArg, m_CancellationToken, TaskCreationOptions.DenyChildAttach, m_Scheduler);
        }
    }
}
