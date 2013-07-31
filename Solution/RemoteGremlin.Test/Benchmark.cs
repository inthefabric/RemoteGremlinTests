using System;
using System.Diagnostics;
using System.Threading.Tasks;
using RexConnectClient.Core.Transfer;
using RexConnectClient.Test.Runners;

namespace RexConnectClient.Test {

	/*================================================================================================*/
	public class Benchmark {

		public IRunner[] Runners { get; private set; }
		public bool IsParallel { get; private set; }
		public string TestName { get; private set; }
		public string Script { get; private set; }
		public Request RexConnRequest { get; private set; }

		public int Warm { get; private set; }
		public int Rounds { get; private set; }
		public int RoundSize { get; private set; }

		public double RunTime { get; private set; }

		private readonly ParallelOptions vParOpt;


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public Benchmark(bool pIsParallel) {
			IsParallel = pIsParallel;
			TestName = "Unknown";
			RunTime = 0;

			Runners = new IRunner[] {
				new GremlinExtGet(),
				//new GremlinExtPost(),
				new RexProClient(),
				new RexConnClientTcp(),
				//new RexConnTcp(),
				new RexConnClientPost(),
				new RexConnGet(),
				//new RexConnPost()
			};

			if ( IsParallel ) {
				vParOpt = new ParallelOptions();
				vParOpt.MaxDegreeOfParallelism = 8;
			}
		}

		/*--------------------------------------------------------------------------------------------*/
		public void OverrideRunners(IRunner[] pRunners) {
			Runners = pRunners;
		}

		/*--------------------------------------------------------------------------------------------*/
		public void Prepare(string pTestName, string pScript) {
			var r = new Request("1");
			r.AddQuery(pScript);
			Prepare(pTestName, pScript, r);
		}

		/*--------------------------------------------------------------------------------------------*/
		public void Prepare(string pTestName, string pScript, Request pRexConnRequest) {
			TestName = pTestName;
			Script = pScript;
			RexConnRequest = pRexConnRequest;

			foreach ( IRunner r in Runners ) {
				r.Prepare(pScript, pRexConnRequest);
			}
		}

		/*--------------------------------------------------------------------------------------------*/
		public void Run(int pWarm, int pRounds, int pRoundSize) {
			Warm = pWarm;
			Rounds = pRounds;
			RoundSize = pRoundSize;

			Console.WriteLine("// Starting "+TestName+": W="+Warm+", R="+Rounds+", RS="+RoundSize);
			var sw = Stopwatch.StartNew();

			foreach ( IRunner run in Runners ) {
				for ( int i = 0 ; i < Warm ; ++i ) {
					run.Run(false);
				}
			}

			for ( int round = 0 ; round < Rounds ; ++round ) {
				Console.WriteLine("// Round "+(round+1)+"/"+pRounds+":  "+sw.ElapsedMilliseconds+"ms");

				foreach ( IRunner run in Runners ) {
					if ( IsParallel ) {
						IRunner r = run;
						Parallel.For(0, RoundSize, vParOpt, (x => r.Run()));
						continue;
					}

					for ( int i = 0 ; i < RoundSize ; ++i ) {
						run.Run();
					}
				}
			}

			Console.WriteLine();
			RunTime += sw.Elapsed.TotalMilliseconds;
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public void Print() {
			Console.WriteLine("### "+TestName);
			Console.WriteLine("- **Script:** `"+Script+"`");
			Console.WriteLine("- **Plan:** Warms "+Warm+", Rounds "+Rounds+", RoundSize "+RoundSize);
			Console.WriteLine("- **Execution Time:** "+TimingUtil.MillisToSecString(RunTime));
			Console.WriteLine();
			Console.WriteLine("|Method|Avg Total|Times|");
			Console.WriteLine("|:--|--:|:--|");

			const string url = "http://dummyimage.com";
			int maxZ = 0;

			foreach ( IRunner run in Runners ) {
				var hist = run.Results.GetHistogram();
				maxZ = Math.Max(maxZ, hist.MaxOccurence);
			}

			foreach ( IRunner run in Runners ) {
				double ms = run.Results.GetAverageTimeSum();
				var hist = run.Results.GetHistogram();
				string histo = "";

				for ( int x = 0 ; x <= hist.MaxTime+1 ; ++x ) {
					int z = (hist.ContainsKey(x) ? hist[x] : 0);
					double perc = z/(double)maxZ;
					int h = (z == 0 ? 1 : (int)(19*perc)+1);
					string rgb = (z == 0 ? "dddddd" : "6dbe51"); //"4183c4");

					if ( x == (int)Math.Round(ms, MidpointRounding.AwayFromZero) ) {
						histo += "![ ]("+url+"/1x20/000000/000000.png \"Average\")";
					}

					string text = "Occurrences at "+x+"ms: "+z;
					histo += "![ ]("+url+"/3x"+h+"/"+rgb+"/"+rgb+".png \""+text+"\")";
				}

				Console.WriteLine("|"+run.Method+"|"+TimingUtil.MillisToString(ms)+"|"+histo+"|");
			}
			
			Console.WriteLine();

			/*Console.WriteLine();
			Console.WriteLine("### Details");
			Console.WriteLine();

			foreach ( IRunner run in Runners ) {
				Console.WriteLine("#### "+run.Method);
				Console.WriteLine();
				Console.WriteLine("|Section|Min|Avg|Max|");
				Console.WriteLine("|:--|--:|--:|--:|");

				foreach ( string key in run.Results.Executions[0].Keys ) {
					IList<double> times = run.Results.GetTimes(key);

					Console.WriteLine(
						"|"+key+"|"+
						MillisToString(times.Min())+"|"+
						MillisToString(times.Average())+"|"+
						MillisToString(times.Max())+"|"
					);
				}

				IList<double> sums = run.Results.GetTimeSums();

				Console.WriteLine(
					"|**Total**|"+
					"**"+MillisToString(sums.Min())+"**|"+
					"**"+MillisToString(sums.Average())+"**|"+
					"**"+MillisToString(sums.Max())+"**|"
				);

				Console.WriteLine();
			}*/

			/*Console.WriteLine();

			foreach ( IRunner run in Runners ) {
				Console.WriteLine("#### "+run.Method);
				Console.WriteLine();

				foreach ( string json in run.Results.JsonResults ) {
					Console.WriteLine(json.Replace("\n", "").Replace("\r", ""));
				}

				Console.WriteLine();
			}*/
		}

	}

}