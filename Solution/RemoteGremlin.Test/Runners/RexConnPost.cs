using System.Collections.Specialized;
using System.Diagnostics;
using System.Net;
using System.Text;

namespace RexConnectClient.Test.Runners {

	/*================================================================================================*/
	public class RexConnPost : Runner {
		

		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public RexConnPost() {
			vUseRexConnReq = true;
		}

		/*--------------------------------------------------------------------------------------------*/
		public override void Run(bool pRecordResult=true) {
			Stopwatch sw1;
			var sw0 = Stopwatch.StartNew();
			const string url = "http://"+TimingUtil.Host+":8182/graphs/graph/fabric/rexconnect";
			byte[] bytes;

			using ( var wc = new WebClient() ) {
				var vals = new NameValueCollection();
				vals["req"] = Script;
				sw0.Stop();

				sw1 = Stopwatch.StartNew();
				bytes = wc.UploadValues(url, "POST", vals);
				sw1.Stop();
			}

			var sw2 = Stopwatch.StartNew();
			string json = Encoding.ASCII.GetString(bytes, 0, bytes.Length);
			sw2.Stop();

			if ( !pRecordResult ) {
				return;
			}

			lock ( Results ) {
				Results.AddExecution(json);
				Results.AttachTime("Init", sw0);
				Results.AttachTime("Exec", sw1);
				Results.AttachTime("ToJson", sw2);
			}
		}

	}

}