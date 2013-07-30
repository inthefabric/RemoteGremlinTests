using System.Web;

namespace RexConnectClient.Test.Runners {

	/*================================================================================================*/
	public class RexConnGet : BaseHttpGet {
		

		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public RexConnGet() {
			vUseRexConnReq = true;
		}

		/*--------------------------------------------------------------------------------------------*/
		public override void Run(bool pRecordResult=true) {
			vUrl = "http://"+TimingUtil.Host+":8182/graphs/graph/fabric/rexconnect?req="+
				HttpUtility.UrlEncode(Script);
			base.Run(pRecordResult);
		}

	}

}