using Amccloy.MusicBot.Asp.Net.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace Amccloy.MusicBot.Net.Test.Utils
{
    [TestClass]
    public class ArgumentParsingTests
    {
        [TestMethod]
        public void NoQuotes()
        {
            string[] args = new[] {"arg1", "arg2", "arg3"};
            var parsedArgs = ArgumentParseUtils.ParseArgsWithQuotes(args);

            for (int i = 0; i < args.Length; i++)
            {
                parsedArgs[i].ShouldBe(args[i]);
            }
        }
    }
}