using System;
using System.Linq;
using System.Threading;
using FluentAssertions;
using Moq;
using Xunit;

namespace BreadWinner.UnitTests.WorkItems
{
    public class WorkBatchShould
    {
        #region Constants
        private const string BatchId = "givenId";
        private const string DummyData = "Test";
        private readonly WorkItemResult[] _successfulBatch = new[]
        {
            new WorkItemResult(WorkStatus.Successful, new [] { $"{DummyData} 0" }),
            new WorkItemResult(WorkStatus.Successful, new [] { $"{DummyData} 1" }),
            new WorkItemResult(WorkStatus.Successful, new [] { $"{DummyData} 2"})
        };

        private readonly WorkItemResult[] _failedBatch = new[]
        {
            new WorkItemResult(WorkStatus.Successful, new [] { $"{DummyData} 0" }),
            new WorkItemResult(WorkStatus.Failed),
            new WorkItemResult(WorkStatus.Successful, new [] { $"{DummyData} 2" })
        };
        #endregion

        [Fact]
        public void Error_when_trying_to_create_a_batch_of_size_lower_than_one()
        {
            // Arrange
            Action createBatch = () => new WorkBatch(-1, new Mock<IWorkBatch>().Object);

            // Act & Assert
            createBatch.ShouldThrow<ArgumentException>();
        }

        [Fact]
        public void Error_when_trying_to_create_a_batch_with_no_inner_batch()
        {
            // Arrange
            Action createBatch = () => new WorkBatch(1, batch: null);

            // Act & Assert
            createBatch.ShouldThrow<ArgumentException>();
        }

        [Fact]
        public void Preserve_given_id()
        {
            // Arrange & Act
            var workBatch = new WorkBatch(1, new TestWorkBatch(BatchId));

            // Assert
            workBatch.Id.Should().Be(BatchId);
        }

        [Fact]
        public void Preserve_given_precomputed_results()
        {
            // Arrange
            var result = new WorkItemResult(WorkStatus.Successful);
            var preComputedResult = new [] { DummyData };
            var testBatch = new TestWorkBatch(BatchId);
            var workBatch = new WorkBatch(1, testBatch, preComputedResult);

            // Act
            workBatch.DoFinally(result, CancellationToken.None);

            // Assert
            testBatch.Result.Data.Should().Contain(DummyData);
        }

        [Fact]
        public void Succeded_when_workitems_succeded()
        {
            // Arrange
            var results = _successfulBatch;
            var testBatch = new TestWorkBatch(BatchId);
            var workBatch = new WorkBatch(results.Length, testBatch);

            // Act
            foreach (var result in results)
            {
                workBatch.DoFinally(result, CancellationToken.None);
            }

            // Assert
           testBatch.Result.Status.Should().Be(WorkStatus.Successful);
        }

        [Fact]
        public void Fail_when_workitems_failed()
        {
            // Arrange
            var results = _failedBatch;
            var testBatch = new TestWorkBatch(BatchId);
            var workBatch = new WorkBatch(results.Length, testBatch);

            // Act
            foreach (var result in results)
            {
                workBatch.DoFinally(result, CancellationToken.None);
            }

            // Assert
            testBatch.Result.Status.Should().Be(WorkStatus.Failed);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Execute_doFinally_only_once(bool successful)
        {
            // Arrange
            var results = successful ? _successfulBatch :_failedBatch;
            var testBatch = new TestWorkBatch(BatchId);
            var workBatch = new WorkBatch(results.Length, testBatch);

            // Act
            foreach (var result in results)
            {
                workBatch.DoFinally(result, CancellationToken.None);
            }

            // Assert
            testBatch.Executions.Should().Be(1);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Store_results(bool successful)
        {
            // Arrange
            var results = successful ? _successfulBatch : _failedBatch;
            var testBatch = new TestWorkBatch(BatchId);
            var workBatch = new WorkBatch(results.Length, testBatch);

            // Act
            foreach (var result in results)
            {
                workBatch.DoFinally(result, CancellationToken.None);
            }

            // Assert
            testBatch.Result.Data.Should()
                .Contain(results.Where(x => x.Data != null)
                    .Select(y => y.Data.Cast<string>().First()));
        }

        [Fact]
        public void Error_when_trying_to_add_a_result_to_a_completed_batch()
        {
            // Arrange
            var results = new[]
            {
                new WorkItemResult(WorkStatus.Successful),
                new WorkItemResult(WorkStatus.Failed),
                new WorkItemResult(WorkStatus.Successful)
            };
            var testBatch = new TestWorkBatch(BatchId);
            var workBatch = new WorkBatch(results.Length, testBatch);

            foreach (var result in results)
            {
                workBatch.DoFinally(result, CancellationToken.None);
            }

            Action addToClosedBatch = () => workBatch.DoFinally(new WorkItemResult(WorkStatus.Successful), CancellationToken.None);

            // Act & Assert
            addToClosedBatch.ShouldThrow<ApplicationException>();
        }
    }
}
