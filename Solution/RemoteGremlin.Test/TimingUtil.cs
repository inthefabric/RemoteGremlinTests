using System;
using System.Collections.Generic;
using RexConnectClient.Core.Transfer;
using RexConnectClient.Test.Runners;

namespace RexConnectClient.Test {

	/*================================================================================================*/
	public static class TimingUtil {

		public const string Host = "rexster";


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public static IRunner[] GetAllRunners() {
			return new IRunner[] {
				new GremlinExt(),
				new RexProClient(),
				new RexConnClientTcp(),
				new RexConnTcp(),
				new RexConnClientHttp(),
				new RexConnHttp(),
				new RexConnPost()
			};
		}

		/*--------------------------------------------------------------------------------------------*/
		public static IRunner[] GetAllRunners(string pTestName, string pScript) {
			var req = new Request("1");
			req.AddQuery(pScript);
			return GetAllRunners(pTestName, pScript, req);
		}

		/*--------------------------------------------------------------------------------------------*/
		public static IRunner[] GetAllRunners(string pTestName, string pScript, Request pRexConnReq) {
			IRunner[] runs = GetAllRunners();

			foreach ( IRunner r in runs ) {
				r.Prepare(pTestName, pScript, pRexConnReq);
			}

			return runs;
		}

		/*--------------------------------------------------------------------------------------------*/
		public static void RunRounds(IRunner[] pRunners, int pWarm, int pRounds, int pRoundSize) {
			foreach ( IRunner run in pRunners ) {
				//Console.WriteLine("++ Warming "+run.Method+"."+run.TestName+" ("+pWarm+" count) ++");

				for ( int i = 0 ; i < pWarm ; ++i ) {
					run.Run(false);
				}
			}

			for ( int round = 0 ; round < pRounds ; ++round ) {
				//Console.WriteLine();

				foreach ( IRunner run in pRunners ) {
					//Console.WriteLine("++ Running "+run.Method+"."+run.TestName+
					//	" (round "+(round+1)+"/"+pRounds+"; "+pRoundSize+" count) ++");

					for ( int i = 0 ; i < pRoundSize ; ++i ) {
						run.Run();
					}
				}
			}
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public static string MillisToString(double pMillis) {
			return pMillis.ToString("#0.00")+"ms";
		}
		
		/*--------------------------------------------------------------------------------------------*/
		public static string MillisToString(double pMillis, int pWidth) {
			return MillisToString(pMillis).PadLeft(pWidth);
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public static void PrintResultSets(IList<IRunner> pRunners) {
			IRunner run0 = pRunners[0];

			Console.WriteLine();
			Console.WriteLine("### "+run0.TestName);
			Console.WriteLine("- **Script:** `"+run0.Script+"`");
			Console.WriteLine("- **Run Count:** "+run0.Results.Executions.Count);
			Console.WriteLine("- **Date:** "+DateTime.Now);
			Console.WriteLine();
			Console.WriteLine("|Method|Avg Total|Times|");
			Console.WriteLine("|:--|--:|:--|");

			int maxZ = 0;

			foreach ( IRunner run in pRunners ) {
				var hist = run.Results.GetHistogram();
				maxZ = Math.Max(maxZ, hist.MaxOccurence);
			}

			foreach ( IRunner run in pRunners ) {
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

				Console.WriteLine("|"+run.Method+"|"+MillisToString(ms)+"|"+histo+"|");
			}

			/*Console.WriteLine();
			Console.WriteLine("### Details");
			Console.WriteLine();

			foreach ( IRunner run in pRunners ) {
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

			foreach ( IRunner run in pRunners ) {
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