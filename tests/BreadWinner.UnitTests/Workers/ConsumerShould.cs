using System;
using System.Threading;
using BreadWinner.Threading;
using BreadWinner.UnitTests.TestDoubles;
using FluentAssertions;
using Moq;
using Xunit;

namespace BreadWinner.UnitTests.Workers
{
    public class ConsumerShould
    {
        private readonly Func<CancellationToken, IWorkItem> _takeWork;
        private readonly CancellationToken _cancellationToken;
        private readonly Mock<IWorkItem> _workItemMock;

        public ConsumerShould()
        {
            _workItemMock = new Mock<IWorkItem>();
            _takeWork = cancellationToken => _workItemMock.Object;

            _cancellationToken = new CancellationToken(false);
        }

        [Fact]
        public void Start_wrapped_thread()
        {
            // Arrange
            var threadWrapperMock = new Mock<IThreadWrapper>();
            var consumer = new Consumer(_takeWork, threadWrapperMock.Object);

            // Act
            consumer.Start(_cancellationToken);

            // Assert
            threadWrapperMock.Verify(x => x.Start(_cancellationToken), Times.Once);
        }

        [Fact]
        public void Do_startup()
        {
            // Arrange
            var consumer = new Consumer(_takeWork, new TestThreadWrapper());

            // Act
            var result = consumer.TestRun();

            // Assert
            _workItemMock.Verify(x => x.Do(result.CancellationToken), Times.AtLeastOnce);
        }

        [Fact]
        public void Do_work()
        {
            // Arrange
            var consumer = new Consumer(_takeWork, new TestThreadWrapper());

            // Act
            var result = consumer.TestRun();

            // Assert
            _workItemMock.Verify(x => x.Do(result.CancellationToken), Times.AtLeastOnce);
        }

        [Fact]
        public void Stop_when_cancellation_requested()
        {
            // Arrange
            var consumer = new Consumer(_takeWork, new TestThreadWrapper());

            // Act
            var result = consumer.TestRun();

            // Assert
            result.ThreadState.Should().Be(ThreadState.Stopped);
        }

        [Fact]
        public void Not_error_when_workitem_errors()
        {
            // Arrange
            var threadWrapper = new TestThreadWrapper();
            var consumer = new Consumer(_takeWork, threadWrapper);
            _workItemMock.Setup(x => x.Do(It.IsAny<CancellationToken>())).Throws<Exception>();

            // Act
            consumer.TestRun();

            // Assert
            threadWrapper.ItemHasThrown.Should().BeFalse();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Relay_thread_wrapper_alive_status(bool threadWrapperAlive)
        {
            // Arrange
            var threadWrapper = new TestThreadWrapper
            {
                IsAlive = threadWrapperAlive
            };

            var consumer = new Consumer(_takeWork, threadWrapper);

            // Act
            consumer.IsAlive.Should().Be(threadWrapperAlive);
        }
    }
}