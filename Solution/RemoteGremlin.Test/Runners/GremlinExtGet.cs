using System.Web;

namespace RexConnectClient.Test.Runners {

	/*================================================================================================*/
	public class GremlinExtGet : BaseHttpGet {


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		protected override void RunInner(bool pRecordResult=true) {
			vUrl = "http://"+TimingUtil.Host+":8182/graphs/graph/tp/gremlin?script="+
				HttpUtility.UrlEncode(Script);
			base.RunInner(pRecordResult);
		}

	}

}