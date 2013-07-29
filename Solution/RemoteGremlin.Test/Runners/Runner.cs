using RexConnectClient.Core.Transfer;
using RexConnectClient.Test.Results;
using ServiceStack.Text;

namespace RexConnectClient.Test.Runners {

	/*================================================================================================*/
	public abstract class Runner : IRunner {

		public string Method { get; set; }
		public string TestName { get; private set; }
		public string Script { get; private set; }
		public Request RexConnRequest { get; private set; }
		public ResultSet Results { get; private set; }

		protected bool vUseRexConnReq;


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		protected Runner() {
			Results = new ResultSet();
			Method = GetType().Name;
		}

		/*--------------------------------------------------------------------------------------------*/
		public void Prepare(string pTestName, string pScript, Request pRexConnRequest) {
			TestName = pTestName;
			Script = pScript;

			if ( vUseRexConnReq ) {
				RexConnRequest = pRexConnRequest;

				JsConfig.EmitCamelCaseNames = true;
				Script = JsonSerializer.SerializeToString(RexConnRequest);
				JsConfig.EmitCamelCaseNames = false;
			}
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public abstract void Run(bool pRecordResult=true);

		/*--------------------------------------------------------------------------------------------*/
		public void RunMany(int pCount, bool pRecordResult=true) {
			for ( int i = 0 ; i < pCount ; ++i ) {
				Run(pRecordResult);
			}
		}

	}

}