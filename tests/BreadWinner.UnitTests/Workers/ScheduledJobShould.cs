using System;
using System.Threading;
using BreadWinner.Threading;
using BreadWinner.UnitTests.TestDoubles;
using FluentAssertions;
using Moq;
using Xunit;

namespace BreadWinner.UnitTests.Workers
{
    public class ScheduledJobShould
    {
        private readonly CancellationTokenSource _cancellatonTokenSource;

        public ScheduledJobShould()
        {
            _cancellatonTokenSource = new CancellationTokenSource();
        }

        [Fact]
        public void Start_wrapped_thread()
        {
            // Arrange
            var threadWrapper = new Mock<IThreadWrapper>();
            var scheduledJob = new ScheduledJob(new TimeSpan(), token => { }, token => { }, threadWrapper.Object);

            // Act
            scheduledJob.Start(_cancellatonTokenSource.Token);

            // Assert
            threadWrapper.Verify(x => x.Start(_cancellatonTokenSource.Token), Times.Once);
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

            var consumer = new ScheduledJob(new TimeSpan(), token => { }, token => { }, threadWrapper);

            // Act & Assert
            consumer.IsAlive.Should().Be(threadWrapperAlive);
        }

        [Fact]
        public void Do_start_up_action()
        {
            // Arrange
            var threadWrapper = new TestThreadWrapper();
            CancellationToken cancellationTokenPassedToStartupAction;

            var consumer = new ScheduledJob(new TimeSpan(), token => { }, token => { cancellationTokenPassedToStartupAction = token; }, threadWrapper);

            // Act
            consumer.TestRun(_cancellatonTokenSource);

            // Assert
            cancellationTokenPassedToStartupAction.Should().Be(_cancellatonTokenSource.Token);
        }

        [Fact]
        public void Do_work_item()
        {
            // Arrange
            var threadWrapper = new TestThreadWrapper();
            CancellationToken cancellationTokenPassedToWorkItem;

            var consumer = new ScheduledJob(new TimeSpan(), token => { cancellationTokenPassedToWorkItem = token; }, token => { }, threadWrapper);

            // Act
            consumer.TestRun(_cancellatonTokenSource);

            // Assert
            cancellationTokenPassedToWorkItem.Should().Be(_cancellatonTokenSource.Token);
        }

        [Fact]
        public void Not_error_when_work_item_errors()
        {
            // Arrange
            var threadWrapper = new TestThreadWrapper();

            var consumer = new ScheduledJob(new TimeSpan(), token => { throw new ApplicationException(); }, token => { }, threadWrapper);

            // Act
            consumer.TestRun(_cancellatonTokenSource);

            // Assert
            threadWrapper.ItemHasThrown.Should().BeFalse();
        }
    }
}

