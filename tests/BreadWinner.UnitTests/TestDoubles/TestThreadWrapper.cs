using System;
using System.Threading;
using BreadWinner.Threading;

namespace BreadWinner.UnitTests.TestDoubles
{
    internal class TestThreadWrapper : IThreadWrapper
    {
        private Action<CancellationToken> _startup;
        private Action<CancellationToken> _loop;
        public bool IsAlive { get; set; }
        public bool ItemHasThrown { get; private set; }

        public void Start(CancellationToken cancellationToken)
        {
            try
            {
                _startup?.Invoke(cancellationToken);

                Loop(cancellationToken);
            }
            catch (Exception)
            {
                ItemHasThrown = true;
            }
        }

        public void Loop(CancellationToken cancellationToken)
        {
            _loop(cancellationToken);
        }

        public void Setup(Action<CancellationToken> loop, Action<CancellationToken> startup = null)
        {
            _loop = loop;
            _startup = startup;
        }
    }
}