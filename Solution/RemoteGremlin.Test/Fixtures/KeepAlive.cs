using System;
using System.Diagnostics;
using NUnit.Framework;
using RexConnectClient.Core;
using RexConnectClient.Core.Transfer;
using RexConnectClient.Test.Runners;
using RexConnTcp = RexConnectClient.Core.RexConnTcp;

namespace RexConnectClient.Test.Fixtures {

	/*================================================================================================*/
	[TestFixture]
	public class KeepAlive {


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		[TestCase(false)]
		public void RunBenchmarks(bool pParallel) {
			const int warm = 5;
			const int rounds = 10;
			const int roundSize = 100;

			var set = new BenchmarkSet("Keep Alive ("+(pParallel ? "Parallel" : "Serial")+")");

			////

			var b = new Benchmark(pParallel);
			b.OverrideRunners(new IRunner[] { new RexConnClientTcp() });
			b.Runners[0].CustomRunner = RunRecConnClientTcpAlive;
			b.Prepare("GetNumber", "x=99");
			b.Run(warm, rounds, roundSize);
			set.Add(b);

			set.Print();
		}

		/*--------------------------------------------------------------------------------------------*/
		private static void RunRecConnClientTcpAlive(IRunner pRunner, bool pRecordResult=true) {
			var sw0 = Stopwatch.StartNew();

			var ctx = new RexConnContextKeepAlive(pRunner.RexConnRequest, TimingUtil.Host, 8185);
			var da = new RexConnDataAccess(ctx);
			sw0.Stop();

			var sw1 = Stopwatch.StartNew();
			string json = da.ExecuteRaw();
			sw1.Stop();

			if ( !pRecordResult ) {
				return;
			}

			lock ( pRunner ) {
				pRunner.Results.AddExecution(json);
				pRunner.Results.AttachTime("Init", sw0);
				pRunner.Results.AttachTime("Exec", sw1);
			}
		}

	}


	/*================================================================================================*/
	public class RexConnContextKeepAlive : RexConnContext {

		private static IRexConnTcp Tcp;


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public RexConnContextKeepAlive(Request pRequest, string pHostName, int pPort) : 
																	base(pRequest, pHostName, pPort) {
			Logger = (level, category, text, ex) => {};

			if ( Tcp == null ) {
				Tcp = new RexConnTcp(pHostName, pPort);
			}
		}

		/*--------------------------------------------------------------------------------------------*/
		public override IRexConnTcp CreateTcpClient() {
			return Tcp;
		}

	}

}