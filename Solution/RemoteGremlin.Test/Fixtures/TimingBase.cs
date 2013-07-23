using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace RexConnectClient.Test.Fixtures {

	/*================================================================================================*/
	public abstract class TimingBase {

		private IList<ResultSet> vResultSets;


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		[SetUp]
		public void SetUp() {
			vResultSets = new List<ResultSet>();
		}

		/*--------------------------------------------------------------------------------------------*/
		public void PrintResultSets(string pTitle) {
			Console.WriteLine();
			Console.WriteLine("# "+pTitle+" Timing Results");
			Console.WriteLine();
			Console.WriteLine("### Summary");
			Console.WriteLine();
			Console.WriteLine("- **Script:** `"+vResultSets[0].Script+"`");
			Console.WriteLine("- **Executed On:** "+DateTime.Now);
			Console.WriteLine("- **Run Count:** "+GetRunCount());
			Console.WriteLine("- **Notes:** *None*");
			Console.WriteLine();
			Console.WriteLine("|Method|Avg Total|Chart|");
			Console.WriteLine("|:--|--:|:--|");

			foreach ( ResultSet rs in vResultSets ) {
				double ms = rs.GetAverageTimeSum();

				Console.WriteLine(
					"|**"+rs.Name+"**|"+
					TimingUtil.MillisToString(ms, 8)+"|`"+
					new string('=', (int)Math.Min(50,ms))+(ms > 50 ? "..." : "")+"`|"
				);
			}

			Console.WriteLine();
			Console.WriteLine("### Details");
			Console.WriteLine();

			foreach ( ResultSet rs in vResultSets ) {
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

			/*foreach ( ResultSet rs in vResultSets ) {
				Console.WriteLine("#### "+rs.Name);
				Console.WriteLine();

				foreach ( string json in rs.JsonResults ) {
					Console.WriteLine(json.Replace("\n", "").Replace("\r", ""));
				}

				Console.WriteLine();
			}*/
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		protected virtual void StoreResult(ResultSet pResults) {
			vResultSets.Add(pResults);
		}
	
		/*--------------------------------------------------------------------------------------------*/
		protected abstract ResultSet GetResultSet();
		protected abstract ResultSet GetResultSetWithRexConnRequest();
		protected abstract int GetRunCount();


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		[Test]
		public void CompareSerial() {
			GremlinExtSerial();
			RexProSerial();
			RexConnClientSerial();
			RexConnTcpSerial();
			RexConnHttpSerial();
			PrintResultSets("Serial");
		}

		/*--------------------------------------------------------------------------------------------*/
		[Test]
		public void CompareParallel() {
			GremlinExtParallel();
			RexProParallel();
			RexConnClientParallel();
			RexConnTcpParallel();
			RexConnHttpParallel();
			PrintResultSets("Parallel");
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public void GremlinExtSerial() {
			var rs = GetResultSet();
			rs.SetName("GremlinExtSerial");
			TimingUtil.ExecuteSerial(TimingUtil.RunGremlinExt, rs, GetRunCount());
			StoreResult(rs);
		}
		
		/*--------------------------------------------------------------------------------------------*/
		public void RexProSerial() {
			var rs = GetResultSet();
			rs.SetName("RexProSerial");
			TimingUtil.ExecuteSerial(TimingUtil.RunRexPro, rs, GetRunCount());
			StoreResult(rs);
		}

		/*--------------------------------------------------------------------------------------------*/
		public void RexConnClientSerial() {
			var rs = GetResultSetWithRexConnRequest();
			rs.SetName("RexConnClientSerial");
			TimingUtil.ExecuteSerial(TimingUtil.RunRexConnClient, rs, GetRunCount());
			StoreResult(rs);
		}

		/*--------------------------------------------------------------------------------------------*/
		public void RexConnTcpSerial() {
			var rs = GetResultSetWithRexConnRequest();
			rs.SetName("RexConnTcpSerial");
			TimingUtil.ExecuteSerial(TimingUtil.RunRexConnTcp, rs, GetRunCount());
			StoreResult(rs);
		}

		/*--------------------------------------------------------------------------------------------*/
		public void RexConnHttpSerial() {
			var rs = GetResultSetWithRexConnRequest();
			rs.SetName("RexConnHttpSerial");
			TimingUtil.ExecuteSerial(TimingUtil.RunRexConnHttp, rs, GetRunCount());
			StoreResult(rs);
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public void GremlinExtParallel() {
			var rs = GetResultSet();
			rs.SetName("GremlinExtParallel");
			TimingUtil.ExecuteParallel(TimingUtil.RunGremlinExt, rs, GetRunCount());
			StoreResult(rs);
		}
		
		/*--------------------------------------------------------------------------------------------*/
		public void RexProParallel() {
			var rs = GetResultSet();
			rs.SetName("RexProParallel");
			TimingUtil.ExecuteParallel(TimingUtil.RunRexPro, rs, GetRunCount());
			StoreResult(rs);
		}

		/*--------------------------------------------------------------------------------------------*/
		public void RexConnClientParallel() {
			var rs = GetResultSetWithRexConnRequest();
			rs.SetName("RexConnClientParallel");
			TimingUtil.ExecuteParallel(TimingUtil.RunRexConnClient, rs, GetRunCount());
			StoreResult(rs);
		}

		/*--------------------------------------------------------------------------------------------*/
		public void RexConnTcpParallel() {
			var rs = GetResultSetWithRexConnRequest();
			rs.SetName("RexConnTcpParallel");
			TimingUtil.ExecuteParallel(TimingUtil.RunRexConnTcp, rs, GetRunCount());
			StoreResult(rs);
		}

		/*--------------------------------------------------------------------------------------------*/
		public void RexConnHttpParallel() {
			var rs = GetResultSetWithRexConnRequest();
			rs.SetName("RexConnHttpParallel");
			TimingUtil.ExecuteParallel(TimingUtil.RunRexConnHttp, rs, GetRunCount());
			StoreResult(rs);
		}

	}

}