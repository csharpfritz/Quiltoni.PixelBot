using Quiltoni.PixelBot.Commands;
using Xunit;
using Xunit.Abstractions;

namespace Quiltoni.Test.GuessGameTests
{
    public class GuessGameDiagramShould
    {
        private readonly ITestOutputHelper _output;

        public GuessGameDiagramShould(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void OutputADiagram()
        {
            var sut = new GuessGame();
            _output.WriteLine(sut.AsDiagram());
        }
    }
}
