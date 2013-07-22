using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace RexConnectClient.Test.Fixtures {

	/*================================================================================================*/
	public abstract class TimingBase {

		private string vTitle;
		private IList<ResultSet> vResultSets;


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		[SetUp]
		public void SetUp() {
			vResultSets = new List<ResultSet>();
		}

		/*--------------------------------------------------------------------------------------------*/
		[TearDown]
		public void TearDown() {
			Console.WriteLine("# "+vTitle+" Timing Results");
			Console.WriteLine();
			Console.WriteLine("- **Executed:** "+DateTime.Now);
			Console.WriteLine("- **Run Count:** "+GetRunCount());
			Console.WriteLine();
			Console.WriteLine("### Summary");

			foreach ( ResultSet rs in vResultSets ) {
				double ms = rs.GetAverageTimeSum();
				Console.WriteLine("- "+rs.Name.PadRight(20)+" | "+TimingUtil.MillisToString(ms, 8)+" | "+
					new string('#', (int)ms*10));
			}

			Console.WriteLine();

			foreach ( ResultSet rs in vResultSets ) {
				Console.WriteLine("### "+rs.Name);
				Console.WriteLine();
				Console.WriteLine("- **Average:** "+TimingUtil.MillisToString(rs.GetAverageTimeSum()));
				Console.WriteLine();

				string header = "|";
				string cols = "|";

				foreach ( string key in rs.Executions[0].Keys ) {
					header += key+"|";
					cols += "--:|";
				}

				Console.WriteLine(header);
				Console.WriteLine(cols);

				foreach ( var ex in rs.Executions ) {
					string row = "|";

					foreach ( string key in ex.Keys ) {
						row += TimingUtil.MillisToString(ex[key])+"|";
					}

					Console.WriteLine(row);
				}

				Console.WriteLine();
			}
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
			vTitle = "Serial";

			GremlinExtSerial();
			RexProSerial();
			RexConnClientSerial();
			RexConnTcpSerial();
			RexConnHttpSerial();
		}

		/*--------------------------------------------------------------------------------------------*/
		[Test]
		public void CompareParallel() {
			vTitle = "Parallel";

			GremlinExtParallel();
			RexProParallel();
			RexConnClientParallel();
			RexConnTcpParallel();
			RexConnHttpParallel();
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
			var rs = GetResultSet();
			rs.SetName("RexConnTcpSerial");
			TimingUtil.ExecuteSerial(TimingUtil.RunRexConnTcp, rs, GetRunCount());
			StoreResult(rs);
		}

		/*--------------------------------------------------------------------------------------------*/
		public void RexConnHttpSerial() {
			var rs = GetResultSet();
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
			var rs = GetResultSet();
			rs.SetName("RexConnTcpParallel");
			TimingUtil.ExecuteParallel(TimingUtil.RunRexConnTcp, rs, GetRunCount());
			StoreResult(rs);
		}

		/*--------------------------------------------------------------------------------------------*/
		public void RexConnHttpParallel() {
			var rs = GetResultSet();
			rs.SetName("RexConnHttpParallel");
			TimingUtil.ExecuteParallel(TimingUtil.RunRexConnHttp, rs, GetRunCount());
			StoreResult(rs);
		}

	}

}