using System.Text;
using Datamigratie.Common.Helpers;

namespace Datamigratie.Tests.Common.Helpers
{
    public class StreamHelpersTests
    {
        [Fact]
        public async Task CopyBytesToAsync_CopiesExactBytes_WhenSourceHasEnoughData()
        {
            const string Input = "This is a test string for PipeHelpers.";
            const string ExpectedOutput = "This is a test string";
            await RunTest(Input, ExpectedOutput);
        }

        [Fact]
        public async Task CopyBytesToAsync_WritesCorrectDataAcrossMultipleBuffers()
        {
            var input = new string('A', 100_000); // Large enough to span multiple rented buffers
            var expectedOutput = new string('A', 98_000);
            await RunTest(input, expectedOutput);
        }

        private static async Task RunTest(string input, string expectedOutput)
        {
            await using var source = new MemoryStream(Encoding.UTF8.GetBytes(input));
            await using var destination = new MemoryStream();
            await source.CopyBytesToAsync(destination, Encoding.UTF8.GetByteCount(expectedOutput), CancellationToken.None);
            var result = Encoding.UTF8.GetString(destination.ToArray());
            Assert.Equal(expectedOutput, result);
        }
    }
}
