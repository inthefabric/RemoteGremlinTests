using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using RexConnectClient.Core;
using RexConnectClient.Core.Result;
using Rexster;
using Rexster.Messages;
using ServiceStack.Text;

namespace RexConnectClient.Test {

	/*================================================================================================*/
	public static class TimingUtil {

		public const string Host = "rexster";
		

		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		private static void WarmDb(Action<ResultSet> pRun, ResultSet pResults) {
			ResultSet rs = pResults.Clone();
			pRun(rs);
			pRun(rs);
			pRun(rs);
		}

		/*--------------------------------------------------------------------------------------------*/
		public static void ExecuteSerial(Action<ResultSet> pRun, ResultSet pResults, int pCount) {
			WarmDb(pRun, pResults);

			for ( int i = 0 ; i < pCount ; ++i ) {
				pRun(pResults);
			}
		}

		/*--------------------------------------------------------------------------------------------*/
		public static void ExecuteParallel(Action<ResultSet> pRun, ResultSet pResults, int pCount) {
			WarmDb(pRun, pResults);
			Parallel.For(0, pCount, (x => pRun(pResults)));
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public static string MillisToString(double pMillis) {
			return pMillis.ToString("#0.0000")+"ms";
		}
		
		/*--------------------------------------------------------------------------------------------*/
		public static string MillisToString(double pMillis, int pWidth) {
			return MillisToString(pMillis).PadLeft(pWidth);
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public static void RunGremlinExt(ResultSet pResults) {
			var sw0 = Stopwatch.StartNew();
			var req = HttpWebRequest.Create(
				"http://rexster:8182/graphs/graph/tp/gremlin?script="+pResults.Script);
			sw0.Stop();

			var sw1 = Stopwatch.StartNew();
			WebResponse resp = req.GetResponse();
			sw1.Stop();

			var sw2 = Stopwatch.StartNew();
			var sr = new StreamReader(resp.GetResponseStream());
			sw2.Stop();

			var sw3 = Stopwatch.StartNew();
			string json = sr.ReadToEnd();
			sw3.Stop();

			lock ( pResults ) {
				pResults.AddExecution(json);
				pResults.AttachTime("Init", sw0);
				pResults.AttachTime("Exec", sw1);
				pResults.AttachTime("Read", sw2);
				pResults.AttachTime("ToJson", sw3);
			}
		}

		/*--------------------------------------------------------------------------------------------*/
		public static void RunRexPro(ResultSet pResults) {
			var sw0 = Stopwatch.StartNew();
			var rexPro = new RexProClient(Host, 8184);
			sw0.Stop();

			var sw1 = Stopwatch.StartNew();
			var sr = new ScriptRequest { Script = pResults.Script };
			sw1.Stop();

			var sw2 = Stopwatch.StartNew();
			ScriptResponse resp = rexPro.ExecuteScript(sr);
			sw2.Stop();

			var sw3 = Stopwatch.StartNew();
			string json = resp.Dump();
			sw3.Stop();

			lock ( pResults ) {
				pResults.AddExecution(json);
				pResults.AttachTime("Init", sw0);
				pResults.AttachTime("Script", sw1);
				pResults.AttachTime("Exec", sw2);
				pResults.AttachTime("ToJson", sw3);
			}
		}

		/*--------------------------------------------------------------------------------------------*/
		public static void RunRexConnClient(ResultSet pResults) {
			var sw0 = Stopwatch.StartNew();
			var ctx = new TestRexConnCtx(pResults.RexConnRequest, Host, 8185);
			var da = new RexConnDataAccess(ctx);
			sw0.Stop();

			var sw1 = Stopwatch.StartNew();
			IResponseResult res = da.Execute();
			sw1.Stop();

			var sw2 = Stopwatch.StartNew();
			string json = res.ResponseJson;
			sw2.Stop();

			lock ( pResults ) {
				pResults.AddExecution(json);
				pResults.AttachTime("Init", sw0);
				pResults.AttachTime("Exec", sw1);
				pResults.AttachTime("ToJson", sw2);
			}
		}

		/*--------------------------------------------------------------------------------------------*/
		//Adapted from RexConnectClient's RexConnDataAccess
		public static void RunRexConnTcp(ResultSet pResults) {
			var sw0 = Stopwatch.StartNew();
			int len = IPAddress.HostToNetworkOrder(pResults.Script.Length);
			byte[] dataLen = BitConverter.GetBytes(len);
			byte[] data = Encoding.ASCII.GetBytes(pResults.Script);
			sw0.Stop();

			var sw1 = Stopwatch.StartNew();
			var tcp = new TcpClient(Host, 8185);
			NetworkStream stream = tcp.GetStream();
			stream.Write(dataLen, 0, dataLen.Length);
			stream.Write(data, 0, data.Length);
			sw1.Stop();

			var sw2 = Stopwatch.StartNew();
			data = new byte[4];
			stream.Read(data, 0, data.Length);
			Array.Reverse(data);
			int respLen = BitConverter.ToInt32(data, 0);
			sw2.Stop();

			var sw3 = Stopwatch.StartNew();
			var sb = new StringBuilder(respLen);

			while ( sb.Length < respLen ) {
				data = new byte[respLen];
				int bytes = stream.Read(data, 0, data.Length);
				sb.Append(Encoding.ASCII.GetString(data, 0, bytes));
			}

			string json = sb.ToString();
			sw3.Stop();

			lock ( pResults ) {
				pResults.AddExecution(json);
				pResults.AttachTime("TcpBytes", sw0);
				pResults.AttachTime("TcpWrite", sw1);
				pResults.AttachTime("DataLen", sw2);
				pResults.AttachTime("ToJson", sw3);
			}
		}

		/*--------------------------------------------------------------------------------------------*/
		public static void RunRexConnHttp(ResultSet pResults) {
			var sw0 = Stopwatch.StartNew();
			var req = HttpWebRequest.Create(
				"http://rexster:8182/graphs/graph/fabric/rexconnect?req="+pResults.Script);
			sw0.Stop();

			var sw1 = Stopwatch.StartNew();
			WebResponse resp = req.GetResponse();
			sw1.Stop();

			var sw2 = Stopwatch.StartNew();
			var sr = new StreamReader(resp.GetResponseStream());
			sw2.Stop();

			var sw3 = Stopwatch.StartNew();
			string json = sr.ReadToEnd();
			sw3.Stop();

			lock ( pResults ) {
				pResults.AddExecution(json);
				pResults.AttachTime("Init", sw0);
				pResults.AttachTime("Exec", sw1);
				pResults.AttachTime("Read", sw2);
				pResults.AttachTime("ToJson", sw3);
			}
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public static void PrintResultSets(string pTitle, int pRunCount, IList<ResultSet> pResultSets) {
			Console.WriteLine();
			Console.WriteLine("# "+pTitle+" Timing Results");
			Console.WriteLine();
			Console.WriteLine("### Summary");
			Console.WriteLine();
			Console.WriteLine("- **Script:** `"+pResultSets[0].Script+"`");
			Console.WriteLine("- **Executed On:** "+DateTime.Now);
			Console.WriteLine("- **Run Count:** "+pRunCount);
			Console.WriteLine("- **Notes:** *None*");
			Console.WriteLine();
			Console.WriteLine("|Method|Avg Total|Chart|");
			Console.WriteLine("|:--|--:|:--|");

			foreach ( ResultSet rs in pResultSets ) {
				double ms = rs.GetAverageTimeSum();

				Console.WriteLine(
					"|**"+rs.Name+"**|"+
					TimingUtil.MillisToString(ms, 8)+"|`"+
					new string('=', (int)Math.Min(50, ms))+(ms > 50 ? "..." : "")+"`|"
				);
			}

			Console.WriteLine();
			Console.WriteLine("### Details");
			Console.WriteLine();

			foreach ( ResultSet rs in pResultSets ) {
				Console.WriteLine("#### "+rs.Name);
				Console.WriteLine();
				Console.WriteLine("|Section|Min|Avg|Max|");
				Console.WriteLine("|:--|--:|--:|--:|");

				foreach ( string key in rs.Executions[0].Keys ) {
					IList<double> times = rs.GetTimes(key);

					Console.WriteLine(
						"|"+key+"|"+
						TimingUtil.MillisToString(times.Min())+"|"+
						TimingUtil.MillisToString(times.Average())+"|"+
						TimingUtil.MillisToString(times.Max())+"|"
					);
				}

				IList<double> sums = rs.GetTimeSums();

				Console.WriteLine(
					"|**Total**|"+
					"**"+TimingUtil.MillisToString(sums.Min())+"**|"+
					"**"+TimingUtil.MillisToString(sums.Average())+"**|"+
					"**"+TimingUtil.MillisToString(sums.Max())+"**|"
				);

				Console.WriteLine();
			}

			/*foreach ( ResultSet rs in pResultSets ) {
				Console.WriteLine("#### "+rs.Name);
				Console.WriteLine();

				foreach ( string json in rs.JsonResults ) {
					Console.WriteLine(json.Replace("\n", "").Replace("\r", ""));
				}

				Console.WriteLine();
			}*/
		}

	}

}