using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace BreadWinner.Threading
{
    [ExcludeFromCodeCoverage]
    internal class ThreadWrapper : IThreadWrapper
    {
        private Action<CancellationToken> _startup;
        private Action<CancellationToken> _loop;

        protected Thread WrappedThread { get; set; }
        public bool IsAlive => WrappedThread.IsAlive;

        void IThreadWrapper.Setup(Action<CancellationToken> loop, Action<CancellationToken> startup)
        {
            _loop = loop;
            _startup = startup;
        }

        public void Start(CancellationToken cancellationToken)
        {
            if (_loop == null) throw new ArgumentNullException(nameof(_loop));

            if (cancellationToken != CancellationToken.None)
            {
                cancellationToken.Register(Stop);
            }

            WrappedThread = new Thread(GetThreadAction(cancellationToken))
            {
                IsBackground = true,
                Priority = ConfigHelper.ThreadPriority
            };

            WrappedThread.Start();
        }

        private void Stop()
        {
            if (!WrappedThread.Join(ConfigHelper.ThreadWaitTime))
            {
                WrappedThread.Abort();
            }
        }

        private ThreadStart GetThreadAction(CancellationToken cancellationToken)
        {
            return () =>
            {
                try
                {
                    _startup?.Invoke(cancellationToken);

                    _loop(cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    // Thread was cancelled
                }
                catch (Exception ex)
                {
                    // We don't want our threads to die
                    Trace.WriteLine($"BreadWinner - Unhandled exception in implementor: {ex}");
                    if (ConfigHelper.ThrowExceptions)
                    {
                        throw;
                    }
                }
            };
        }
    }
}