using System;
using System.Diagnostics;
using RexConnectClient.Core.Transfer;
using RexConnectClient.Test.Runners;

namespace RexConnectClient.Test {

	/*================================================================================================*/
	public class Benchmark {

		public IRunner[] Runners { get; private set; }
		public string TestName { get; private set; }
		public string Script { get; private set; }
		public Request RexConnRequest { get; private set; }

		public int Warm { get; private set; }
		public int Rounds { get; private set; }
		public int RoundSize { get; private set; }


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public Benchmark() {
			TestName = "Unknown";

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
				Console.WriteLine("// Round "+(round+1)+"/"+pRounds+": \t"+sw.ElapsedMilliseconds+"ms");

				foreach ( IRunner run in Runners ) {
					for ( int i = 0 ; i < RoundSize ; ++i ) {
						run.Run();
					}
				}
			}

			Console.WriteLine();
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public void Print() {
			Console.WriteLine("### "+TestName);
			Console.WriteLine("- **Script:** `"+Script+"`");
			Console.WriteLine("- **Plan:** Warm "+Warm+", Rounds "+Rounds+", RoundSize "+RoundSize);
			Console.WriteLine();
			Console.WriteLine("|Method|Avg Total|Times|");
			Console.WriteLine("|:--|--:|:--|");

			int maxZ = 0;

			foreach ( IRunner run in Runners ) {
				var hist = run.Results.GetHistogram();
				maxZ = Math.Max(maxZ, hist.MaxOccurence);
			}

			foreach ( IRunner run in Runners ) {
				double ms = run.Results.GetAverageTimeSum();
				var hist = run.Results.GetHistogram();
				string histo = "";

				for ( int x = 0 ; x <= hist.MaxTime ; ++x ) {
					int z = (hist.ContainsKey(x) ? hist[x] : 0);
					double perc = 1-(z/(double)maxZ);
					byte r = (byte)(220*perc);
					byte g = (byte)(245*perc);
					byte b = (byte)(255*perc);
					string rgb = r.ToString("X2")+g.ToString("X2")+b.ToString("X2");
					string text = "Occurrences at "+x+"ms: "+z;
					histo += "![ ](http://dummyimage.com/3x20/"+rgb+"/"+rgb+".png \""+text+"\")";
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