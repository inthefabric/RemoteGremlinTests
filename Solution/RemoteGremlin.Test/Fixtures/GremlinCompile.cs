using System;
using System.Collections.Generic;
using NUnit.Framework;
using RexConnectClient.Core.Transfer;
using RexConnectClient.Test.Runners;

namespace RexConnectClient.Test.Fixtures {

	/*================================================================================================*/
	[TestFixture]
	public class GremlinCompile {

		private IList<string> vScripts;
		private int vScriptI;


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		[TestCase(false)]
		[TestCase(true)]
		public void RunBenchmarks(bool pParallel) {
			const int warm = 5;
			const int rounds = 5;
			const int roundSize = 10;
			const int totalScripts = warm+rounds*roundSize;

			var set = new BenchmarkSet("Session Scenarios ("+(pParallel ? "Parallel" : "Serial")+")");

			////
			
			int mapSize = 2;
			vScriptI = 0;

			var b = new Benchmark(pParallel);
			BuildMapScripts(totalScripts*b.Runners.Length, mapSize);
			AddRunnerHooks(b);
			b.Prepare("NonCachedMap"+mapSize, "several different scripts");
			b.Run(warm, rounds, roundSize);
			set.Add(b);

			vScriptI = 0;

			b = new Benchmark(pParallel);
			AddRunnerHooks(b);
			b.Prepare("CachedMap"+mapSize, "the same scripts as NonCachedMap"+mapSize);
			b.Run(warm, rounds, roundSize);
			set.Add(b);

			////

			mapSize = 20;
			vScriptI = 0;

			b = new Benchmark(pParallel);
			BuildMapScripts(totalScripts*b.Runners.Length, mapSize);
			AddRunnerHooks(b);
			b.Prepare("NonCachedMap"+mapSize, "several different scripts");
			b.Run(warm, rounds, roundSize);
			set.Add(b);

			vScriptI = 0;

			b = new Benchmark(pParallel);
			AddRunnerHooks(b);
			b.Prepare("CachedMap"+mapSize, "the same scripts as NonCachedMap"+mapSize);
			b.Run(warm, rounds, roundSize);
			set.Add(b);

			set.Print();
		}

		/*--------------------------------------------------------------------------------------------*/
		private void BuildMapScripts(int pSize, int pMapSize) {
			vScripts = new List<string>(pSize);

			for ( int i = 0 ; i < pSize ; ++i ) {
				vScripts.Add(BuildMapScript(pMapSize));
			}
		}

		/*--------------------------------------------------------------------------------------------*/
		private string BuildMapScript(int pMapSize) {
			string script = "[prop"+Guid.NewGuid().ToString("N")+":0";

			for ( int i = 0 ; i < pMapSize ; ++i ) {
				script += ", val"+i+":"+i;
			}

			return script+", prop"+Guid.NewGuid().ToString("N")+":0]";
		}

		/*--------------------------------------------------------------------------------------------*/
		private void AddRunnerHooks(Benchmark pBenchmark) {
			foreach ( IRunner r in pBenchmark.Runners ) {
				r.PreRun = (run, rec) => {
					string s = vScripts[vScriptI++];
					//Console.WriteLine((vScriptI-1)+" / "+run.Method+" / "+s);

					var req = new Request("1");
					req.AddQuery(s);

					run.Prepare(s, req);
				};
			}
		}

	}

}