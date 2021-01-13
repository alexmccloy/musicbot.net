using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace Amccloy.MusicBot.Net.Test.Commands
{
    [TestClass]
    public class BaseCommandTests
    {
        [TestMethod]
        public void GenerateSummaryHelpString()
        {
            var cmd = new DummyCommand(new TestSchedulerFactory());
            var helpText = cmd.PrintSummaryHelpText();
            helpText.ShouldBe($"{DummyCommand.Command}: {DummyCommand.SummaryHelp}");
        }
        
        
        [TestMethod]
        public void GenerateFullHelpString()
        {
            var cmd = new DummyCommand(new TestSchedulerFactory());
            var helpText = cmd.PrintFullHelpText();
            helpText.ShouldBe($"Command: {DummyCommand.Command}\n{DummyCommand.FullHelp}");
        }
    }
}