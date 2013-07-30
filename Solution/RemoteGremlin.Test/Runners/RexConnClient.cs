using System.Diagnostics;
using RexConnectClient.Core;

namespace RexConnectClient.Test.Runners {

	/*================================================================================================*/
	public abstract class RexConnClient : Runner {

		protected bool vUseHttp;
		

		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		protected RexConnClient() {
			vUseRexConnReq = true;
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		protected override void RunInner(bool pRecordResult=true) {
			var sw0 = Stopwatch.StartNew();
			var ctx = new RexConnContext(RexConnRequest, TimingUtil.Host, (vUseHttp ? 8182 : 8185));
			ctx.UseHttp = vUseHttp;
			ctx.Logger = (level, category, text, ex) => {};
			var da = new RexConnDataAccess(ctx);
			sw0.Stop();

			var sw1 = Stopwatch.StartNew();
			string json = da.ExecuteRaw();
			sw1.Stop();

			if ( !pRecordResult ) {
				return;
			}

			lock ( Results ) {
				Results.AddExecution(json);
				Results.AttachTime("Init", sw0);
				Results.AttachTime("Exec", sw1);
			}
		}

	}


	/*================================================================================================*/
	public class RexConnClientTcp : RexConnClient {


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public RexConnClientTcp() {
			vUseHttp = false;
		}

	}


	/*================================================================================================*/
	public class RexConnClientPost : RexConnClient {


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public RexConnClientPost() {
			vUseHttp = true;
		}

	}

}