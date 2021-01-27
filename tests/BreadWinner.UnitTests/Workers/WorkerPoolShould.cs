using System;
using System.Threading;
using FluentAssertions;
using Moq;
using Xunit;

namespace BreadWinner.UnitTests.Workers
{
    public class WorkerPoolShould
    {
        private readonly WorkerPool _workerPool;
        private readonly Mock<IWorker> _firstWorker;
        private readonly Mock<IWorker> _secondWorker;
        private readonly CancellationToken _cancellationToken;

        public WorkerPoolShould()
        {
            _workerPool = new WorkerPool();
            _firstWorker = new Mock<IWorker>();
            _secondWorker = new Mock<IWorker>();
            _cancellationToken = new CancellationToken(false);
            _workerPool.Add(_firstWorker.Object, _secondWorker.Object);
        }

        [Fact]
        public void Start_registered_workers()
        {
            // Act
            _workerPool.Start(_cancellationToken);

            // Assert
            _firstWorker.Verify(x => x.Start(_cancellationToken), Times.Once());
            _secondWorker.Verify(x => x.Start(_cancellationToken), Times.Once());
        }

        [Fact]
        public void Not_allow_to_register_workers_when_started()
        {
            // Act
            _workerPool.Start(_cancellationToken);
            Action addAfterStart = () => _workerPool.Add(new Mock<IWorker>().Object);

            // Assert
            addAfterStart.ShouldThrow<ApplicationException>();
        }

        [Fact]
        public void Start_once()
        {
            // Act
            _workerPool.Start(_cancellationToken);
            Action startAgain = () => _workerPool.Start(_cancellationToken);

            // Assert
            startAgain.ShouldThrow<ApplicationException>();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Relay_workers_alive_status(bool secondWorkerAlive)
        {
            // Arrange
            _firstWorker.SetupGet(x => x.IsAlive).Returns(true);
            _secondWorker.SetupGet(x => x.IsAlive).Returns(secondWorkerAlive);

            // Act
            _workerPool.IsAlive.Should().Be(secondWorkerAlive);
        }
    }
}
