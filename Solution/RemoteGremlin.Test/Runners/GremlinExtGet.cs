using System.Web;

namespace RexConnectClient.Test.Runners {

	/*================================================================================================*/
	public class GremlinExtGet : BaseHttpGet {


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public override void Run(bool pRecordResult=true) {
			vUrl = "http://"+TimingUtil.Host+":8182/graphs/graph/tp/gremlin?script="+
				HttpUtility.UrlEncode(Script);
			base.Run(pRecordResult);
		}

	}

}