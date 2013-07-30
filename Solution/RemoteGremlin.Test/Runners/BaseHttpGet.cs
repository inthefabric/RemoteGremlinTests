using System.Diagnostics;
using System.IO;
using System.Net;

namespace RexConnectClient.Test.Runners {

	/*================================================================================================*/
	public class BaseHttpGet : Runner {
		
		protected string vUrl;


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		protected override void RunInner(bool pRecordResult=true) {
			var sw0 = Stopwatch.StartNew();
			var req = HttpWebRequest.Create(vUrl);
			sw0.Stop();

			var sw1 = Stopwatch.StartNew();
			WebResponse resp = req.GetResponse();
			sw1.Stop();

			var sw2 = Stopwatch.StartNew();
			var sr = new StreamReader(resp.GetResponseStream());
			sw2.Stop();

			var sw3 = Stopwatch.StartNew();
			string json = sr.ReadToEnd();
			sw3.Stop();

			if ( !pRecordResult ) {
				return;
			}

			lock ( Results ) {
				Results.AddExecution(json);
				Results.AttachTime("Init", sw0);
				Results.AttachTime("Exec", sw1);
				Results.AttachTime("Read", sw2);
				Results.AttachTime("ToJson", sw3);
			}
		}

	}

}