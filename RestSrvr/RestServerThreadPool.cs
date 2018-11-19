﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RestSrvr
{
    /// <summary>
    /// Represents a rest server thread pool
    /// </summary>
    internal sealed class RestServerThreadPool : IDisposable
    {

        // Tracer
        private TraceSource m_tracer = new TraceSource(TraceSources.ThreadingTraceSourceName);

        // Number of threads to keep alive
        private int m_concurrencyLevel = System.Environment.ProcessorCount * 4;
        
        // Queue of work items
        private Queue<WorkItem> m_queue = null;

        // Active threads
        private Thread[] m_threadPool = null;
        // Hint of the number of threads waiting to be executed
        private int m_threadWait = 0;
        // True when the thread pool is being disposed
        private bool m_disposing = false;

        /// <summary>
        /// Concurrency
        /// </summary>
        public int Concurrency { get { return this.m_concurrencyLevel; } }

        // Get current thread pool
        private static RestServerThreadPool s_current;

        /// <summary>
        /// Get the singleton threadpool
        /// </summary>
        public static RestServerThreadPool Current
        {
            get
            {
                if (s_current == null)
                    s_current = new RestServerThreadPool();
                return s_current;
            }
        }

        /// <summary>
        /// Creates a new instance of the wait thread pool
        /// </summary>
        private RestServerThreadPool()
        {
            this.m_queue = new Queue<WorkItem>(this.m_concurrencyLevel);
        }

        /// <summary>
        /// Worker data structure
        /// </summary>
        private struct WorkItem
        {
            /// <summary>
            /// The callback to execute on the worker
            /// </summary>
            public Action<Object> Callback { get; set; }
            /// <summary>
            /// The state or parameter to the worker
            /// </summary>
            public object State { get; set; }
            /// <summary>
            /// The execution context
            /// </summary>
            public ExecutionContext ExecutionContext { get; set; }
        }

        // Number of remaining work items
        private int m_remainingWorkItems = 1;
        
        // Thread is done reset event
        private ManualResetEvent m_threadDoneResetEvent = new ManualResetEvent(false);

        /// <summary>
        /// Queue a work item to be completed
        /// </summary>
        public void QueueUserWorkItem(Action<Object> callback)
        {
            QueueUserWorkItem(callback, null);
        }

        /// <summary>
        /// Queue a user work item with the specified parameters
        /// </summary>
        public void QueueUserWorkItem(Action<Object> callback, object state)
        {
            this.QueueWorkItemInternal(callback, state);
        }

        /// <summary>
        /// Perform queue of workitem internally
        /// </summary>
        private void QueueWorkItemInternal(Action<Object> callback, object state)
        {
            ThrowIfDisposed();

            try
            {
                WorkItem wd = new WorkItem()
                {
                    Callback = callback,
                    State = state,
                    ExecutionContext = ExecutionContext.Capture()
                };
                lock (this.m_threadDoneResetEvent) this.m_remainingWorkItems++;
                this.EnsureStarted(); // Ensure thread pool threads are started
                lock (m_queue)
                {
                    m_queue.Enqueue(wd);

                    if (m_threadWait > 0)
                        Monitor.Pulse(m_queue);
                }
            }
            catch (Exception e)
            {
                this.m_tracer.TraceEvent(TraceEventType.Error, e.HResult, "Error queueing work item: {0}", e);
            }
        }

        /// <summary>
        /// Ensure the thread pool threads are started
        /// </summary>
        private void EnsureStarted()
        {
            if (m_threadPool == null)
            {
                lock (m_queue)
                    if (m_threadPool == null)
                    {
                        m_threadPool = new Thread[m_concurrencyLevel];
                        for (int i = 0; i < m_threadPool.Length; i++)
                        {
                            m_threadPool[i] = new Thread(DispatchLoop);
                            m_threadPool[i].Name = String.Format("RSRVR-ThreadPoolThread-{0}", i);
                            m_threadPool[i].IsBackground = true;
                            m_threadPool[i].Start();
                        }
                    }
            }
        }

        /// <summary>
        /// Dispatch loop
        /// </summary>
        private void DispatchLoop()
        {
            while (true)
            {
                WorkItem wi = default(WorkItem);
                lock (m_queue)
                {
                    try
                    {
                        if (m_disposing) return; // Shutdown requested
                        while (m_queue.Count == 0)
                        {
                            m_threadWait++;
                            try { Monitor.Wait(m_queue); }
                            finally { m_threadWait--; }
                            if (m_disposing)
                                return;
                        }
                        wi = m_queue.Dequeue();
                    }
                    catch (Exception e)
                    {
                        this.m_tracer.TraceEvent(TraceEventType.Error, e.HResult, "Error in dispatchloop {0}", e);
                    }
                }
                DoWorkItem(wi);
            }
        }


        /// <summary>
        /// Wait until the thread is complete
        /// </summary>
        /// <returns></returns>
        public bool WaitOne() { return WaitOne(-1, false); }

        /// <summary>
        /// Wait until the thread is complete or the specified timeout elapses
        /// </summary>
        public bool WaitOne(TimeSpan timeout, bool exitContext)
        {
            return WaitOne((int)timeout.TotalMilliseconds, exitContext);
        }

        /// <summary>
        /// Wait until the thread is completed
        /// </summary>
        public bool WaitOne(int timeout, bool exitContext)
        {
            ThrowIfDisposed();
            DoneWorkItem();
            bool rv = this.m_threadDoneResetEvent.WaitOne(timeout, exitContext);
            lock (this.m_threadDoneResetEvent)
            {
                if (rv)
                {
                    this.m_remainingWorkItems = 1;
                    this.m_threadDoneResetEvent.Reset();
                }
                else this.m_remainingWorkItems++;
            }
            return rv;
        }

        /// <summary>
        /// Perform the work if the specified work data
        /// </summary>
        private void DoWorkItem(WorkItem state)
        {
            this.m_tracer.TraceEvent(TraceEventType.Verbose, 0, "Starting task on {0} ---> {1}", Thread.CurrentThread.Name, state.Callback.Target.ToString());
            var worker = (WorkItem)state;
            try
            {
                worker.Callback(worker.State);
            }
            catch (Exception e)
            {
                this.m_tracer.TraceEvent(TraceEventType.Error, e.HResult, "!!!!!! 0118 999 881 999 119 7253 : THREAD DEATH !!!!!!!\r\nUncaught Exception on worker thread: {0}", e);
            }
            finally
            {
                DoneWorkItem();
            }
        }

        /// <summary>
        /// Complete a workf item
        /// </summary>
        private void DoneWorkItem()
        {
            lock (this.m_threadDoneResetEvent)
            {
                --this.m_remainingWorkItems;
                if (this.m_remainingWorkItems == 0) this.m_threadDoneResetEvent.Set();
            }
        }

        /// <summary>
        /// Throw an exception if the object is disposed
        /// </summary>
        private void ThrowIfDisposed()
        {
            if (this.m_threadDoneResetEvent == null) throw new ObjectDisposedException(this.GetType().Name);
        }

        #region IDisposable Members

        /// <summary>
        /// Dispose the object
        /// </summary>
        public void Dispose()
        {
            if (this.m_threadDoneResetEvent != null)
            {
                if (this.m_remainingWorkItems > 0)
                    this.WaitOne();

                ((IDisposable)m_threadDoneResetEvent).Dispose();
                this.m_threadDoneResetEvent = null;
                m_disposing = true;
                lock (m_queue)
                    Monitor.PulseAll(m_queue);

                if (m_threadPool != null)
                    for (int i = 0; i < m_threadPool.Length; i++)
                    {
                        m_threadPool[i].Join();
                        m_threadPool[i] = null;
                    }
            }
        }

        #endregion

    }
}
