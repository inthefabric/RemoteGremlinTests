using System;
using System.Collections.Generic;
using System.Linq;

namespace RexConnectClient.Test {

	/*================================================================================================*/
	public class BenchmarkSet {

		public string Name { get; private set; }
		public IList<Benchmark> Benchmarks { get; private set; }


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public BenchmarkSet(string pName) {
			Name = pName;
			Benchmarks = new List<Benchmark>();
		}

		/*--------------------------------------------------------------------------------------------*/
		public void Add(Benchmark pBenchmark) {
			Benchmarks.Add(pBenchmark);
		}

		/*--------------------------------------------------------------------------------------------*/
		public void RunAll(int pWarm, int pRounds, int pRoundSize) {
			foreach ( Benchmark b in Benchmarks ) {
				b.Run(pWarm, pRounds, pRoundSize);
			}
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public void Print() {
			double runTime = Benchmarks.Sum(b => b.RunTime);

			Console.WriteLine();
			Console.WriteLine("The following **"+Name+"** timing tests...");
			Console.WriteLine("- Executed in [Environment A v1.1]"+
				"(https://github.com/inthefabric/RemoteGremlinTests/wiki/Testing-Environments)");
			Console.WriteLine("- Completed on "+DateTime.Now.ToString("R"));
			Console.WriteLine("- Executed in "+TimingUtil.MillisToSecString(runTime));
			Console.WriteLine();

			foreach ( Benchmark b in Benchmarks ) {
				b.Print();
			}
		}

	}

}