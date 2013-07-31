using System;
using RexConnectClient.Core.Transfer;
using RexConnectClient.Test.Results;
using ServiceStack.Text;

namespace RexConnectClient.Test.Runners {

	/*================================================================================================*/
	public abstract class Runner : IRunner {

		public string Method { get; set; }
		public string Script { get; private set; }
		public Request RexConnRequest { get; private set; }
		public ResultSet Results { get; private set; }

		public Action<IRunner, bool> PreRun { get; set; }
		public Action<IRunner, bool> CustomRunner { get; set; }
		public Action<IRunner, bool> PostRun { get; set; }

		protected bool vUseRexConnReq;


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		protected Runner() {
			Results = new ResultSet();
			Method = GetType().Name;
		}

		/*--------------------------------------------------------------------------------------------*/
		public void Prepare(string pScript, Request pRexConnRequest) {
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
		public void Run(bool pRecordResult=true) {
			if ( PreRun != null ) {
				PreRun(this, pRecordResult);
			}

			if ( CustomRunner != null ) {
				CustomRunner(this, pRecordResult);
				return;
			}

			RunInner(pRecordResult);

			if ( PostRun != null ) {
				PostRun(this, pRecordResult);
			}
		}

		/*--------------------------------------------------------------------------------------------*/
		protected abstract void RunInner(bool pRecordResult=true);

		/*--------------------------------------------------------------------------------------------*/
		public void RunMany(int pCount, bool pRecordResult=true) {
			for ( int i = 0 ; i < pCount ; ++i ) {
				RunInner(pRecordResult);
			}
		}

	}

}