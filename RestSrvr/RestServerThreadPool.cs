/*
 * Copyright (C) 2019 - 2020, Fyfe Software Inc. and the SanteSuite Contributors (See NOTICE.md)
 * 
 * Licensed under the Apache License, Version 2.0 (the "License"); you 
 * may not use this file except in compliance with the License. You may 
 * obtain a copy of the License at 
 * 
 * http://www.apache.org/licenses/LICENSE-2.0 
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the 
 * License for the specific language governing permissions and limitations under 
 * the License.
 * 
 * User: fyfej
 * Date: 2019-11-27
 */
using RestSrvr.Description;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace RestSrvr
{
    /// <summary>
    /// Represents a rest server thread pool
    /// </summary>
    internal sealed class RestServerThreadPool : IDisposable
    {

        // Lock object
        private static object s_lock = new object();

        // Tracer
        private TraceSource m_tracer = new TraceSource(TraceSources.ThreadingTraceSourceName);

        // Number of threads to keep alive
        private int m_concurrencyLevel = System.Environment.ProcessorCount * 8;

        // Queue of work items
        private ConcurrentQueue<WorkItem> m_queue = null;

        // Active threads
        private Thread[] m_threadPool = null;

        // True when the thread pool is being disposed
        private bool m_disposing = false;

        // Object for pulsing
        private ManualResetEventSlim m_resetEvent = new ManualResetEventSlim(false);

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
                    lock (s_lock) // only want one
                        s_current = s_current ?? new RestServerThreadPool();
                return s_current;
            }
        }

        /// <summary>
        /// Creates a new instance of the wait thread pool
        /// </summary>
        private RestServerThreadPool()
        {
            this.EnsureStarted(); // Ensure thread pool threads are started
            this.m_queue = new ConcurrentQueue<WorkItem>();
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

                this.m_queue.Enqueue(wd);
                this.m_resetEvent.Set();
            }
            catch (Exception e)
            {
                try
                {
                    this.m_tracer.TraceEvent(TraceEventType.Error, e.HResult, "Error queueing work item: {0}", e);
                }
                catch { }
            }
        }

        /// <summary>
        /// Ensure the thread pool threads are started
        /// </summary>
        private void EnsureStarted()
        {
            if (m_threadPool == null)
            {
                m_threadPool = new Thread[m_concurrencyLevel];
                for (int i = 0; i < m_threadPool.Length; i++)
                {
                    m_threadPool[i] = this.CreateThreadPoolThread();
                    m_threadPool[i].Start();
                }
            }
        }

        /// <summary>
        /// Create a thread pool thread
        /// </summary>
        private Thread CreateThreadPoolThread()
        {
            return new Thread(this.DispatchLoop)
            {
                Name = String.Format("RSRVR-ThreadPoolThread"),
                IsBackground = true,
                Priority = ThreadPriority.AboveNormal
            };
        }

        /// <summary>
        /// Dispatch loop
        /// </summary>
        private void DispatchLoop()
        {
            while (!this.m_disposing)
            {
                try
                {
                    this.m_resetEvent.Wait(1000);
                    while (this.m_queue.TryDequeue(out WorkItem wi))
                    {
                        wi.Callback(wi.State);
                    }
                    this.m_resetEvent.Reset();
                }
                catch (Exception e)
                {
                    this.m_tracer.TraceEvent(TraceEventType.Error, e.HResult, "Error in dispatchloop {0}", e);
                }
            }
        }

        /// <summary>
        /// Throw an exception if the object is disposed
        /// </summary>
        private void ThrowIfDisposed()
        {
            if (this.m_disposing) throw new ObjectDisposedException(nameof(RestServerThreadPool));
        }

        #region IDisposable Members

        /// <summary>
        /// Dispose the object
        /// </summary>
        public void Dispose()
        {

            if (this.m_disposing) return;

            this.m_disposing = true;

            this.m_resetEvent.Set();

            if (m_threadPool != null)
            {
                for (int i = 0; i < m_threadPool.Length; i++)
                {
                    if (!m_threadPool[i].Join(1000))
                        m_threadPool[i].Abort();
                    m_threadPool[i] = null;
                }
            }
        }
    }

    #endregion

}
