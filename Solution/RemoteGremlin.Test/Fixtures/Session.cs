using System.Collections.Generic;
using System.Diagnostics;
using NUnit.Framework;
using RexConnectClient.Core;
using RexConnectClient.Core.Result;
using RexConnectClient.Core.Transfer;
using RexConnectClient.Test.Results;
using RexConnectClient.Test.Runners;
using Rexster;
using RexProClient = Rexster.RexProClient;

namespace RexConnectClient.Test.Fixtures {

	/*================================================================================================*/
	[TestFixture]
	public class Session {


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		[TestCase(false)]
		[TestCase(true)]
		public void RunBenchmarks(bool pParallel) {
			const int warm = 5;
			const int rounds = 10;
			const int roundSize = 100;

			var set = new BenchmarkSet("Session Scenario ("+(pParallel ? "Parallel" : "Serial")+")");
			
			var runners = new IRunner[] {
				new Runners.RexProClient(),
				new RexConnClientTcp(),
				new RexConnClientPost()
			};

			runners[0].CustomRunner = RunRexPro;
			runners[1].CustomRunner = RunRexConnClientTcp;
			runners[2].CustomRunner = RunRexConnClientPost;

			var b = new Benchmark(pParallel);
			b.OverrideRunners(runners);
			b.Prepare("RunSession", "test");
			set.Add(b);

			set.RunAll(warm, rounds, roundSize);
			set.Print();
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public void RunRexPro(IRunner pRunner, bool pRecordResult) {
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

			if ( !pRecordResult ) {
				return;
			}

			ResultSet res = pRunner.Results;

			lock ( res ) {
				res.AddExecution("");
				res.AttachTime("Init", sw0);
				res.AttachTime("Start", sw1);
				res.AttachTime("Hello", sw2);
				res.AttachTime("Worked", sw3);
				res.AttachTime("Null", sw4);
				res.AttachTime("Skip", sw5);
				res.AttachTime("Six", sw6);
				res.AttachTime("Kill", sw7);
			}
		}

		/*--------------------------------------------------------------------------------------------*/
		public static void RunRexConnClientPost(IRunner pRunner, bool pRecordResult) {
			RunRexConnClientInner(pRunner, pRecordResult, true);
		}

		/*--------------------------------------------------------------------------------------------*/
		public static void RunRexConnClientTcp(IRunner pRunner, bool pRecordResult) {
			RunRexConnClientInner(pRunner, pRecordResult, false);
		}

		/*--------------------------------------------------------------------------------------------*/
		private static void RunRexConnClientInner(IRunner pRunner, bool pRecordResult, bool pUseHttp) {
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

			var ctx = new RexConnContext(r, TimingUtil.Host, (pUseHttp ? 8182 : 8185));
			ctx.UseHttp = pUseHttp;
			ctx.Logger = (level, category, text, ex) => { };

			var da = new RexConnDataAccess(ctx);
			sw0.Stop();

			var sw1 = Stopwatch.StartNew();
			IResponseResult res = da.Execute();
			sw1.Stop();

			var sw2 = Stopwatch.StartNew();
			string json = res.ResponseJson;
			sw2.Stop();

			IList<ResponseCmd> cmdList = res.Response.CmdList;
			IList<ITextResultList> tr = res.GetTextResults();
			Assert.AreEqual(12, cmdList.Count);
			Assert.AreEqual("hello", tr[1].ToString(0));
			Assert.AreEqual("worked", tr[2].ToString(0));
			Assert.AreEqual(0, tr[3].Values.Count);
			Assert.AreEqual(0, cmdList[3].Results.Count);
			Assert.Null(tr[4]);
			Assert.Null(cmdList[4].Results);
			Assert.AreEqual("A", tr[5].ToString(0));
			Assert.AreEqual("B", tr[6].ToString(0));
			Assert.AreEqual("C", tr[7].ToString(0));
			Assert.AreEqual("D", tr[8].ToString(0));
			Assert.AreEqual("E", tr[9].ToString(0));
			Assert.AreEqual("F", tr[10].ToString(0));

			if ( !pRecordResult ) {
				return;
			}

			ResultSet rs = pRunner.Results;

			lock ( res ) {
				rs.AddExecution(json);
				rs.AttachTime("Init", sw0);
				rs.AttachTime("Exec", sw1);
				rs.AttachTime("ToJson", sw2);
			}
		}

	}

}