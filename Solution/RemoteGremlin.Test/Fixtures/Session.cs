using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using NUnit.Framework;
using RexConnectClient.Core;
using RexConnectClient.Core.Result;
using RexConnectClient.Core.Transfer;
using Rexster;

namespace RexConnectClient.Test.Fixtures {

	/*================================================================================================*/
	[TestFixture]
	public class Session {

		private IList<ResultSet> vResultSets;
		private int vRunCount;


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		[SetUp]
		public void SetUp() {
			vResultSets = new List<ResultSet>();
			vRunCount = 40;
		}

		/*--------------------------------------------------------------------------------------------*/
		protected void PrintResultSets(string pTitle) {
			TimingUtil.PrintResultSets(pTitle, vRunCount, vResultSets);
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		protected virtual void StoreResult(ResultSet pResults) {
			vResultSets.Add(pResults);
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		[Test]
		public void CompareSerial() {
			var rs = new ResultSet("");
			rs.SetName("RexPro");
			TimingUtil.ExecuteSerial(RunRexPro, rs, vRunCount);
			vResultSets.Add(rs);

			rs = new ResultSet("");
			rs.SetName("RexConnClient");
			TimingUtil.ExecuteSerial(RunRexConnClient, rs, vRunCount);
			vResultSets.Add(rs);

			PrintResultSets("Serial");
		}

		/*--------------------------------------------------------------------------------------------*/
		public void RunRexPro(ResultSet pResults) {
			var sw0 = Stopwatch.StartNew();
			var rexPro = new RexProClient(TimingUtil.Host, 8184);
			sw0.Stop();

			var sw1 = Stopwatch.StartNew();
			RexProSession sess = rexPro.StartSession();
			sw1.Stop();

			var sw2 = Stopwatch.StartNew();
			string result = rexPro.Query<string>("x='hello';", null, sess);
			sw2.Stop();
			Assert.AreEqual("hello", result);

			var sw3 = Stopwatch.StartNew();
			if ( result == "hello" ) {
				result = rexPro.Query<string>("x='worked';", null, sess);
			}
			sw3.Stop();
			Assert.AreEqual("worked", result);

			var sw4 = Stopwatch.StartNew();
			result = rexPro.Query<string>("x=null;", null, sess);
			sw4.Stop();
			Assert.Null(result);

			//this is only for symmetry with RexConnect test
			var sw5 = Stopwatch.StartNew();
			if ( result != null ) {
				result = rexPro.Query<string>("throw new Exception();", null, sess);
			}
			sw5.Stop();

			var sw6 = Stopwatch.StartNew();
			string resultA = rexPro.Query<string>("x='A';", null, sess);
			string resultB = rexPro.Query<string>("x='B';", null, sess);
			string resultC = rexPro.Query<string>("x='C';", null, sess);
			string resultD = rexPro.Query<string>("x='D';", null, sess);
			string resultE = rexPro.Query<string>("x='E';", null, sess);
			string resultF = rexPro.Query<string>("x='F';", null, sess);
			sw6.Stop();
			Assert.AreEqual("A", resultA);
			Assert.AreEqual("B", resultB);
			Assert.AreEqual("C", resultC);
			Assert.AreEqual("D", resultD);
			Assert.AreEqual("E", resultE);
			Assert.AreEqual("F", resultF);

			var sw7 = Stopwatch.StartNew();
			rexPro.KillSession(sess);
			sw7.Stop();

			lock ( pResults ) {
				pResults.AddExecution("");
				pResults.AttachTime("Init", sw0);
				pResults.AttachTime("Start", sw1);
				pResults.AttachTime("Hello", sw2);
				pResults.AttachTime("Worked", sw3);
				pResults.AttachTime("Null", sw4);
				pResults.AttachTime("Skip", sw5);
				pResults.AttachTime("Six", sw6);
				pResults.AttachTime("Kill", sw7);
			}
		}

		/*--------------------------------------------------------------------------------------------*/
		public static void RunRexConnClient(ResultSet pResults) {
			var sw0 = Stopwatch.StartNew();
			var r = new Request("123");
			r.AddSessionAction(RexConn.SessionAction.Start);
			
			RequestCmd cmd = r.AddQuery("x='hello';");
			cmd.CmdId = "a";

			cmd = r.AddQuery("x='worked';");
			cmd.AddConditionalCommandId("a");

			cmd = r.AddQuery("x=null;");
			cmd.CmdId = "b";

			cmd = r.AddQuery("throw new Exception();");
			cmd.AddConditionalCommandId("b");

			r.AddQuery("x='A';");
			r.AddQuery("x='B';");
			r.AddQuery("x='C';");
			r.AddQuery("x='D';");
			r.AddQuery("x='E';");
			r.AddQuery("x='F';");

			r.AddSessionAction(RexConn.SessionAction.Close);

			var ctx = new TestRexConnCtx(r, TimingUtil.Host, 8185);
			var da = new RexConnDataAccess(ctx);
			sw0.Stop();

			var sw1 = Stopwatch.StartNew();
			IResponseResult res = da.Execute();
			sw1.Stop();

			var sw2 = Stopwatch.StartNew();
			string json = res.ResponseJson;
			sw2.Stop();

			IList<ResponseCmd> cmdList = res.Response.CmdList;
			IList<ITextResultList> text = res.GetTextResults();
			Assert.AreEqual(12, cmdList.Count);
			Assert.AreEqual("hello", text[1].ToString(0));
			Assert.AreEqual("worked", text[2].ToString(0));
			Assert.AreEqual(0, text[3].Values.Count);
			Assert.AreEqual(0, cmdList[3].Results.Count);
			Assert.Null(text[4]);
			Assert.Null(cmdList[4].Results);
			Assert.AreEqual("A", text[5].ToString(0));
			Assert.AreEqual("B", text[6].ToString(0));
			Assert.AreEqual("C", text[7].ToString(0));
			Assert.AreEqual("D", text[8].ToString(0));
			Assert.AreEqual("E", text[9].ToString(0));
			Assert.AreEqual("F", text[10].ToString(0));

			lock ( pResults ) {
				pResults.AddExecution(json);
				pResults.AttachTime("Init", sw0);
				pResults.AttachTime("Exec", sw1);
				pResults.AttachTime("ToJson", sw2);
			}
		}

	}

}