using System;
using System.Collections.Generic;
using BeFaster.App.Solutions.CHK;
using BeFaster.App.Solutions.CHL;
using BeFaster.App.Solutions.FIZ;
using BeFaster.App.Solutions.HLO;
using BeFaster.App.Solutions.SUM;
using BeFaster.App.Solutions.ARRS;
using BeFaster.App.Solutions.IRNG;
using BeFaster.Runner;
using BeFaster.Runner.Extensions;
using BeFaster.Runner.Utils;
using Newtonsoft.Json.Linq;
using TDL.Client;
using TDL.Client.Runner;

namespace BeFaster.App
{
    internal class SendCommandToServer
    {
        /// <summary>
        /// ~~~~~~~~~~ Running the system: ~~~~~~~~~~~~~
        ///
        ///   From IDE:
        ///      Configure the "BeFaster.App" solution to Run on External Console then run.
        ///
        ///   From command line:
        ///      msbuild befaster.sln; src\BeFaster.App\bin\Debug\BeFaster.App.exe
        ///        or
        ///      msbuild befaster.sln; mono src/BeFaster.App/bin/Debug/BeFaster.App.exe
        ///
        ///   To run your unit tests locally:
        ///      Run the "BeFaster.App.Tests - Unit Tests" configuration.
        ///
        /// ~~~~~~~~~~ The workflow ~~~~~~~~~~~~~
        ///
        ///   By running this file you interact with a challenge server.
        ///   The interaction follows a request-response pattern:
        ///        * You are presented with your current progress and a list of actions.
        ///        * You trigger one of the actions by typing it on the console.
        ///        * After the action feedback is presented, the execution will stop.
        ///
        ///   +------+-----------------------------------------------------------------------+
        ///   | Step | The usual workflow                                                    |
        ///   +------+-----------------------------------------------------------------------+
        ///   |  1.  | Run this file.                                                        |
        ///   |  2.  | Start a challenge by typing "start".                                  |
        ///   |  3.  | Read the description from the "challenges" folder.                    |
        ///   |  4.  | Locate the file corresponding to your current challenge in:           |
        ///   |      |   .\src\BeFaster.App\Solutions                                        |
        ///   |  5.  | Replace the following placeholder exception with your solution:       |
        ///   |      |   throw new SolutionNotImplementedException()                         |
        ///   |  6.  | Deploy to production by typing "deploy".                              |
        ///   |  7.  | Observe the output, check for failed requests.                        |
        ///   |  8.  | If passed, go to step 1.                                              |
        ///   +------+-----------------------------------------------------------------------+
        ///
        ///   You are encouraged to change this project as you please:
        ///        * You can use your preferred libraries.
        ///        * You can use your own test framework.
        ///        * You can change the file structure.
        ///        * Anything really, provided that this file stays runnable.
        ///
        /// </summary>
        /// <param name="args">Action.</param>
        private static void Main(string[] args)
        {
            var runner = new QueueBasedImplementationRunner.Builder().
                SetConfig(Utils.GetRunnerConfig()).
                WithSolutionFor("sum", (List<JToken> p) => SumSolution.Sum(p[0].ToObject<int>(), p[1].ToObject<int>())).
                WithSolutionFor("hello", (List<JToken> p) => HelloSolution.Hello(p[0].ToObject<string>())).
                WithSolutionFor("array_sum", (List<JToken> p) => ArraySumSolution.Compute((p[0].ToObject<List<int>>()))).
                WithSolutionFor("int_range", (List<JToken> p) => IntRangeSolution.Generate(p[0].ToObject<int>(), p[1].ToObject<int>())).
                WithSolutionFor("fizz_buzz", (List<JToken> p) => FizzBuzzSolution.FizzBuzz(p[0].ToObject<int>())).
                WithSolutionFor("checkout", (List<JToken> p) => CheckoutSolution.ComputePrice(p[0].ToObject<string>())).
                WithSolutionFor("checklite", (List<JToken> p) => CheckliteSolution.ComputePrice(p[0].ToObject<string>())).
                Create();

            ChallengeSession.ForRunner(runner)
                .WithConfig(Utils.GetConfig())
                .WithActionProvider(new UserInputAction(args))
                .Start();

            Console.Write("Press any key to continue . . . ");
            Console.ReadKey();
        }
    }
}
