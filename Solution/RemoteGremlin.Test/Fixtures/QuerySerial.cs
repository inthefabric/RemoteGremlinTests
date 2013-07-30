﻿using NUnit.Framework;

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

			var set = new BenchmarkSet("Simple Query (Serial)");
			
			var b = new Benchmark();
			b.Prepare("GetGraph", "g");
			set.Add(b);

			b = new Benchmark();
			b.Prepare("GetNumber", "x=99");
			set.Add(b);
			
			b = new Benchmark();
			b.Prepare("GetVertices", "g.V[0..30]");
			set.Add(b);

			b = new Benchmark();
			b.Prepare("GetBothCount", "g.V.both.count()");
			set.Add(b);

			set.RunAll(warm, rounds, roundSize);
			set.Print();
		}

	}

}