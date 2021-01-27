using System;
using System.Collections.Generic;
using System.Threading;
using BreadWinner.Threading;
using BreadWinner.UnitTests.TestDoubles;
using FluentAssertions;
using Moq;
using Xunit;

namespace BreadWinner.UnitTests.Workers
{
    public class AbstractProducerShould
    {
        private readonly CancellationToken _cancellationToken;

        public AbstractProducerShould()
        {
            _cancellationToken = new CancellationToken();
        }

        [Fact]
        public void Start_wrapped_thread()
        {
            // Arrange
            var threadWrapperMock = new Mock<IThreadWrapper>();
            var producer = new TestProducer(threadWrapperMock.Object);

            // Act
            producer.Start(_cancellationToken);

            // Assert
            threadWrapperMock.Verify(x => x.Start(_cancellationToken), Times.Once);
        }

        [Fact]
        public void Do_startup()
        {
            // Arrange
            var testProducer = new TestProducer(new TestThreadWrapper());

            // Act
            var result = testProducer.TestRun();

            // Assert
            testProducer.StartupCalledWith.Should().Be(result.CancellationToken);
        }

        [Fact]
        public void Wait_for_work()
        {
            // Arrange
            var testProducer = new TestProducer(new TestThreadWrapper());

            // Act
            var result = testProducer.TestRun();

            // Assert
            testProducer.WaitForWorkOrCancellationCalledWith.Should().Be(result.CancellationToken);
        }

        [Fact]
        public void Queue_work()
        {
            // Arrange
            Action<IEnumerable<IWorkItem>, CancellationToken> testAction = (workItems, cancellationToken) => { };
            var testProducer = new TestProducer(new TestThreadWrapper())
            {
                AddWork = testAction
            };

            // Act
            var result = testProducer.TestRun();

            // Assert
            testProducer.QueueWorkCalledWith.Item1.Should().Be(testAction);
            testProducer.QueueWorkCalledWith.Item2.Should().Be(result.CancellationToken);
        }

        [Fact]
        public void Stop_when_cancellation_requested()
        {
            // Arrange
            var testProducer = new TestProducer(new TestThreadWrapper());

            // Act
            var result = testProducer.TestRun();

            // Assert
            result.ThreadState.Should().Be(ThreadState.Stopped);
        }

        [Fact]
        public void Stop_and_not_queue_work_when_wait_cancelled()
        {
            // Arrange
            var testProducer = new TestProducer(new TestThreadWrapper());
            testProducer.SetupWaitForWorkOrCancellation(cancellationToken => true);

            // Act
            var result = testProducer.TestRun();

            // Assert
            result.ThreadState.Should().Be(ThreadState.Stopped);
            testProducer.QueueWorkCalledWith.Should().BeNull();
        }

        [Fact]
        public void Not_error_when_wait_for_work_errors()
        {
            // Arrange
            var testThreadWrapper = new TestThreadWrapper();
            var testProducer = new TestProducer(testThreadWrapper);
            testProducer.SetupWaitForWorkOrCancellation(cancellationToken => { throw new Exception(); });

            // Act
            testProducer.TestRun();

            // Assert
            testThreadWrapper.ItemHasThrown.Should().BeFalse();
        }

        [Fact]
        public void Not_error_when_queue_work_errors()
        {
            // Arrange
            var testThreadWrapper = new TestThreadWrapper();
            var testProducer = new TestProducer(testThreadWrapper);
            testProducer.SetupQueueWork(cancellationToken => { throw new Exception(); });

            // Act
            testProducer.TestRun();

            // Assert
            testThreadWrapper.ItemHasThrown.Should().BeFalse();
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

            var testProducer = new TestProducer(threadWrapper);

            // Act
            testProducer.IsAlive.Should().Be(threadWrapperAlive);
        }
    }
}