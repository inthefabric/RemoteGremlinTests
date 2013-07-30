using NUnit.Framework;

namespace RexConnectClient.Test.Fixtures {

	/*================================================================================================*/
	[TestFixture]
	public class QuerySerial {


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		[Test]
		public void RunBenchmarks() {
			const int warm = 5;
			const int rounds = 10;
			const int roundSize = 100;

			var b = new Benchmark();
			b.Prepare("GetGraph", "g");
			b.Run(warm, rounds, roundSize);
			b.Print();

			b = new Benchmark();
			b.Prepare("GetNumber", "99");
			b.Run(warm, rounds, roundSize);
			b.Print();

			b = new Benchmark();
			b.Prepare("GetVertices", "g.V[0..30]");
			b.Run(warm, rounds, roundSize);
			b.Print();

			b = new Benchmark();
			b.Prepare("GetBothCount", "g.V.both.count()");
			b.Run(warm, rounds, roundSize);
			b.Print();
		}

	}

}