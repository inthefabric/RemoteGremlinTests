using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using NUnit.Framework;
using RexConnectClient.Core;
using RexConnectClient.Core.Transfer;
using RexConnectClient.Test.Runners;
using RexConnTcp = RexConnectClient.Core.RexConnTcp;

namespace RexConnectClient.Test.Fixtures {

	/*================================================================================================*/
	[TestFixture]
	public class KeepAlive {

		private static TcpClient SharedTcp;


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		[TestCase(false)]
		public void RunBenchmarks(bool pParallel) {
			const int warm = 5;
			const int rounds = 10;
			const int roundSize = 200;

			var set = new BenchmarkSet("Keep Alive ("+(pParallel ? "Parallel" : "Serial")+")");

			SharedTcp = new TcpClient(TimingUtil.Host, 8185);

			////

			var b = new Benchmark(pParallel);
			b.OverrideRunners(new IRunner[] {
				new RexConnClientTcp(),
				new Runners.RexConnTcp()
			});
			b.Runners[0].CustomRunner = RunRecConnClientTcpAlive;
			b.Runners[1].CustomRunner = RunRexConnTcpAlive;
			b.Prepare("GetNumber", "x=99");
			b.Run(warm, rounds, roundSize);
			set.Add(b);

			set.Print();
			b.PrintDetails();
		}

		/*--------------------------------------------------------------------------------------------*/
		private static void RunRecConnClientTcpAlive(IRunner pRunner, bool pRecordResult=true) {
			var sw = Stopwatch.StartNew();
			var ctx = new RexConnContextKeepAlive(pRunner.RexConnRequest, TimingUtil.Host, 8185);
			var da = new RexConnDataAccess(ctx);
			double tm0 = sw.Elapsed.TotalMilliseconds;
			sw.Stop();

			sw = Stopwatch.StartNew();
			string json = da.ExecuteRaw();
			double tm1 = sw.Elapsed.TotalMilliseconds;
			sw.Stop();

			if ( !pRecordResult ) {
				return;
			}

			lock ( pRunner ) {
				pRunner.Results.AddExecution(json);
				pRunner.Results.AttachTime("Init", tm0);
				pRunner.Results.AttachTime("Exec", tm1);
			}
		}

		/*--------------------------------------------------------------------------------------------*/
		//Adapted from RexConnectClient's RexConnDataAccess
		private static void RunRexConnTcpAlive(IRunner pRunner, bool pRecordResult=true) {
			var sw = Stopwatch.StartNew();
			int len = IPAddress.HostToNetworkOrder(pRunner.Script.Length);
			byte[] dataLen = BitConverter.GetBytes(len);
			byte[] data = Encoding.UTF8.GetBytes(pRunner.Script);
			double tm0 = sw.Elapsed.TotalMilliseconds;
			sw.Stop();

			////

			sw = Stopwatch.StartNew();
			NetworkStream stream = SharedTcp.GetStream();
			double tm1 = sw.Elapsed.TotalMilliseconds;
			sw.Stop();

			////
			
			sw = Stopwatch.StartNew();
			stream.Write(dataLen, 0, dataLen.Length);
			stream.Write(data, 0, data.Length);
			double tm15 = sw.Elapsed.TotalMilliseconds;
			sw.Stop();

			////

			sw = Stopwatch.StartNew();
			data = new byte[4];
			stream.Read(data, 0, data.Length);
			double tm2 = sw.Elapsed.TotalMilliseconds;
			sw.Stop();

			////

			sw = Stopwatch.StartNew();
			Array.Reverse(data);
			int respLen = BitConverter.ToInt32(data, 0);
			double tm25 = sw.Elapsed.TotalMilliseconds;
			sw.Stop();

			////

			sw = Stopwatch.StartNew();
			data = new byte[respLen];
			int currLen = 0;

			while ( currLen < respLen ) {
				currLen += stream.Read(data, currLen, data.Length);
			}
			double tm3 = sw.Elapsed.TotalMilliseconds;
			sw.Stop();

			////

			sw = Stopwatch.StartNew();
			string json = Encoding.UTF8.GetString(data, 0, respLen);
			double tm4 = sw.Elapsed.TotalMilliseconds;
			sw.Stop();

			////

			if ( !pRecordResult ) {
				return;
			}

			lock ( pRunner ) {
				pRunner.Results.AddExecution(json);
				pRunner.Results.AttachTime("ReqBytes", tm0);
				pRunner.Results.AttachTime("GetStream", tm1);
				pRunner.Results.AttachTime("Write", tm15);
				pRunner.Results.AttachTime("ReadLen", tm2);
				pRunner.Results.AttachTime("LenToInt", tm25);
				pRunner.Results.AttachTime("GetBytes", tm3);
				pRunner.Results.AttachTime("ToJson", tm4);
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