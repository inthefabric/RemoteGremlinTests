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
	public class SessionScenarios {

		private int vSize;


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		[TestCase(false)]
		[TestCase(true)]
		public void RunBenchmarks(bool pParallel) {
			const int warm = 5;
			const int rounds = 10;
			const int roundSize = 40;
			const string name = "RunSession";

			var set = new BenchmarkSet("Session Scenarios ("+(pParallel ? "Parallel" : "Serial")+")");

			vSize = 1;
			var b = new Benchmark(pParallel);
			b.OverrideRunners(BuildRunners());
			b.Prepare(name+vSize, "see code");
			b.Run(warm, rounds, roundSize);
			set.Add(b);

			vSize = 10;
			b = new Benchmark(pParallel);
			b.OverrideRunners(BuildRunners());
			b.Prepare(name+vSize, "see code");
			b.Run(warm, rounds, roundSize);
			set.Add(b);

			vSize = 100;
			b = new Benchmark(pParallel);
			b.OverrideRunners(BuildRunners());
			b.Prepare(name+vSize, "see code");
			b.Run(warm, rounds, roundSize);
			set.Add(b);
			
			set.Print();
		}

		/*--------------------------------------------------------------------------------------------*/
		private IRunner[] BuildRunners() {
			var runs = new IRunner[] {
				new Runners.RexProClient(),
				new RexConnClientTcp(),
				new RexConnClientPost()
			};

			runs[0].CustomRunner = RunRexPro;
			runs[1].CustomRunner = RunRexConnClientTcp;
			runs[2].CustomRunner = RunRexConnClientPost;

			return runs;
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		private void RunRexPro(IRunner pRunner, bool pRecordResult) {
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
			var sizeResults = new string[vSize];

			for ( int i = 0 ; i < vSize ; ++i ) {
				sizeResults[i] = rexPro.Query<string>("x='A"+i+"';", null, sess);
			}

			sw6.Stop();

			for ( int i = 0 ; i < vSize ; ++i ) {
				Assert.AreEqual("A"+i, sizeResults[i]);
			}

			var sw7 = Stopwatch.StartNew();
			rexPro.KillSession(sess);
			sw7.Stop();

			if ( !pRecordResult ) {
				return;
			}

			lock ( pRunner ) {
				ResultSet res = pRunner.Results;
				res.AddExecution("");
				res.AttachTime("Init", sw0);
				res.AttachTime("Start", sw1);
				res.AttachTime("Hello", sw2);
				res.AttachTime("Worked", sw3);
				res.AttachTime("Null", sw4);
				res.AttachTime("Skip", sw5);
				res.AttachTime("Size", sw6);
				res.AttachTime("Kill", sw7);
			}
		}

		/*--------------------------------------------------------------------------------------------*/
		private void RunRexConnClientPost(IRunner pRunner, bool pRecordResult) {
			RunRexConnClientInner(pRunner, pRecordResult, true);
		}

		/*--------------------------------------------------------------------------------------------*/
		private void RunRexConnClientTcp(IRunner pRunner, bool pRecordResult) {
			RunRexConnClientInner(pRunner, pRecordResult, false);
		}

		/*--------------------------------------------------------------------------------------------*/
		private void RunRexConnClientInner(IRunner pRunner, bool pRecordResult, bool pUseHttp) {
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

			for ( int i = 0 ; i < vSize ; ++i ) {
				r.AddQuery("x='A"+i+"';");
			}

			r.AddSessionAction(RexConn.SessionAction.Close);

			var ctx = new RexConnContext(r, TimingUtil.Host, (pUseHttp ? 8182 : 8185));
			ctx.SetHttpMode(pUseHttp, TimingUtil.GraphName);
			ctx.Logger = (level, category, text, ex) => {};

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
			Assert.AreEqual(6+vSize, cmdList.Count);
			Assert.AreEqual("hello", tr[1].ToString(0));
			Assert.AreEqual("worked", tr[2].ToString(0));
			Assert.AreEqual(0, tr[3].Values.Count);
			Assert.AreEqual(0, cmdList[3].Results.Count);
			Assert.Null(tr[4]);
			Assert.Null(cmdList[4].Results);

			for ( int i = 0 ; i < vSize ; ++i ) {
				Assert.AreEqual("A"+i, tr[5+i].ToString(0));
			}

			if ( !pRecordResult ) {
				return;
			}

			lock ( pRunner ) {
				ResultSet rs = pRunner.Results;
				rs.AddExecution(json);
				rs.AttachTime("Init", sw0);
				rs.AttachTime("Exec", sw1);
				rs.AttachTime("ToJson", sw2);
			}
		}

	}

}