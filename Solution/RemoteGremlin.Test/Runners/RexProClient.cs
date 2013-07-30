using System.Diagnostics;
using Rexster.Messages;
using ServiceStack.Text;

namespace RexConnectClient.Test.Runners {

	/*================================================================================================*/
	public class RexProClient : Runner {


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public override void Run(bool pRecordResult=true) {
			var sw0 = Stopwatch.StartNew();
			var rp = new Rexster.RexProClient(TimingUtil.Host, 8184);
			sw0.Stop();

			var sw1 = Stopwatch.StartNew();
			var sr = new ScriptRequest { Script = Script };
			sw1.Stop();

			var sw2 = Stopwatch.StartNew();
			ScriptResponse resp = rp.ExecuteScript(sr);
			sw2.Stop();

			var sw3 = Stopwatch.StartNew();
			string json = resp.SerializeToString();
			sw3.Stop();

			if ( !pRecordResult ) {
				return;
			}

			lock ( Results ) {
				Results.AddExecution(json);
				Results.AttachTime("Init", sw0);
				Results.AttachTime("Script", sw1);
				Results.AttachTime("Exec", sw2);
				Results.AttachTime("ToJson", sw3);
			}
		}

	}

}