using System.Collections.Generic;
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
		protected void PrintResultSets(string pTitle) {
			TimingUtil.PrintResultSets(pTitle, GetRunCount(), vResultSets);
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
			RexConnClientTcpSerial();
			RexConnTcpSerial();
			RexConnClientHttpSerial();
			RexConnHttpSerial();
			PrintResultSets("Serial");
		}

		/*--------------------------------------------------------------------------------------------*/
		[Test]
		public void CompareParallel() {
			GremlinExtParallel();
			RexProParallel();
			RexConnClientTcpParallel();
			RexConnTcpParallel();
			RexConnClientHttpParallel();
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
		public void RexConnClientHttpSerial() {
			var rs = GetResultSetWithRexConnRequest();
			rs.SetName("RexConnClientHttpSerial");
			TimingUtil.ExecuteSerial(TimingUtil.RunRexConnClientHttp, rs, GetRunCount());
			StoreResult(rs);
		}

		/*--------------------------------------------------------------------------------------------*/
		public void RexConnClientTcpSerial() {
			var rs = GetResultSetWithRexConnRequest();
			rs.SetName("RexConnClientTcpSerial");
			TimingUtil.ExecuteSerial(TimingUtil.RunRexConnClientTcp, rs, GetRunCount());
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
		public void RexConnClientHttpParallel() {
			var rs = GetResultSetWithRexConnRequest();
			rs.SetName("RexConnClientHttpParallel");
			TimingUtil.ExecuteParallel(TimingUtil.RunRexConnClientHttp, rs, GetRunCount());
			StoreResult(rs);
		}

		/*--------------------------------------------------------------------------------------------*/
		public void RexConnClientTcpParallel() {
			var rs = GetResultSetWithRexConnRequest();
			rs.SetName("RexConnClientTcpParallel");
			TimingUtil.ExecuteParallel(TimingUtil.RunRexConnClientTcp, rs, GetRunCount());
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