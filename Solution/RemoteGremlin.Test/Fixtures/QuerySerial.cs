using NUnit.Framework;
using RexConnectClient.Test.Runners;

namespace RexConnectClient.Test.Fixtures {

	/*================================================================================================*/
	[TestFixture]
	public class QuerySerial {


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		[Test]
		public void RunBenchmarks() {
			IRunner[] runs = TimingUtil.GetAllRunners("GetGraph", "g");
			TimingUtil.RunRounds(runs, 5, 10, 50);
			TimingUtil.PrintResultSets(runs);

			runs = TimingUtil.GetAllRunners("GetVertices", "g.V[0..10]");
			TimingUtil.RunRounds(runs, 5, 10, 50);
			TimingUtil.PrintResultSets(runs);
		}

	}

}