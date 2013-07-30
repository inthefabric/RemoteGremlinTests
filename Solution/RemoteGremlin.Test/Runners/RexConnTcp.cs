using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace RexConnectClient.Test.Runners {

	/*================================================================================================*/
	public class RexConnTcp : Runner {
		

		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public RexConnTcp() {
			vUseRexConnReq = true;
		}

		/*--------------------------------------------------------------------------------------------*/
		//Adapted from RexConnectClient's RexConnDataAccess
		protected override void RunInner(bool pRecordResult=true) {
			var sw0 = Stopwatch.StartNew();
			int len = IPAddress.HostToNetworkOrder(Script.Length);
			byte[] dataLen = BitConverter.GetBytes(len);
			byte[] data = Encoding.ASCII.GetBytes(Script+"\0");
			sw0.Stop();

			var sw1 = Stopwatch.StartNew();
			var tcp = new TcpClient(TimingUtil.Host, 8185);
			NetworkStream stream = tcp.GetStream();
			stream.Write(dataLen, 0, dataLen.Length);
			sw1.Stop();

			var sw2 = Stopwatch.StartNew();
			stream.Write(data, 0, data.Length);
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

			if ( !pRecordResult ) {
				return;
			}

			lock ( Results ) {
				Results.AddExecution(json);
				Results.AttachTime("TcpBytes", sw0);
				Results.AttachTime("TcpWrite", sw1);
				Results.AttachTime("DataLen", sw2);
				Results.AttachTime("ToJson", sw3);
			}
		}

	}

}