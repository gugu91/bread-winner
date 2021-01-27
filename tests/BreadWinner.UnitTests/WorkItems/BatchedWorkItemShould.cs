using System;
using System.Threading;
using FluentAssertions;
using Moq;
using Xunit;

namespace BreadWinner.UnitTests.WorkItems
{
    public class BatchedWorkItemShould
    {
        private const string WorkItemId = "givenId";
        private readonly Mock<IWorkBatch> _workBatchMock;
        private readonly Mock<IWorkItem> _workItemMock;
        private readonly CancellationToken _cancellationToken;

        public BatchedWorkItemShould()
        {
            _workBatchMock = new Mock<IWorkBatch>();
            _workItemMock = new Mock<IWorkItem>();
            _cancellationToken = new CancellationToken();
        }

        [Fact]
        public void Do_work_item()
        {
            // Arrange
            var batchedWorkItem = new BatchedWorkItem(_workBatchMock.Object, _workItemMock.Object);

            // Act
            batchedWorkItem.Do(_cancellationToken);

            // Assert
            _workItemMock.Verify(x => x.Do(_cancellationToken), Times.Once);
        }

        [Fact]
        public void Pass_result_to_batch()
        {
            // Arrange
            var batchedWorkItem = new BatchedWorkItem(_workBatchMock.Object, _workItemMock.Object);

            var result = new WorkItemResult(WorkStatus.Successful);
            _workItemMock.Setup(x => x.Do(It.Is<CancellationToken>(y => y == _cancellationToken))).Returns(result);

            // Act
            batchedWorkItem.Do(_cancellationToken);

            // Assert
            _workBatchMock.Verify(x => x.DoFinally(result, _cancellationToken), Times.Once);
        }

        [Fact]
        public void Execute_function_when_created_with_lambda_overload()
        {
            // Arrange
            var functionMockCalled = false;
            Func<CancellationToken, WorkItemResult>  functionMock = cancellationToken =>
            {
                functionMockCalled = true;
                return new WorkItemResult(WorkStatus.Successful);
            };
            var batchedWorkItem = new BatchedWorkItem(_workBatchMock.Object, functionMock);

            // Act
            batchedWorkItem.Do(_cancellationToken);

            // Assert
            functionMockCalled.Should().BeTrue();
        }

        [Fact]
        public void Preserve_given_id()
        {
            // Arrange
            const string workItemId = WorkItemId;
            _workItemMock.SetupGet(x => x.Id).Returns(workItemId);
            var batchedWorkItem = new BatchedWorkItem(_workBatchMock.Object, _workItemMock.Object);

            // Act & Assert
            batchedWorkItem.Id.Should().Be(workItemId);
        }

        [Fact]
        public void Preserve_given_id_when_created_with_lambda_overload()
        {
            // Arrange
            const string workItemId = WorkItemId;
            Func<CancellationToken, WorkItemResult> functionMock = cancellationToken => new WorkItemResult(WorkStatus.Successful);
            var batchedWorkItem = new BatchedWorkItem(_workBatchMock.Object, functionMock, workItemId);

            // Act & Assert
            batchedWorkItem.Id.Should().Be(workItemId);
        }
    }
}
