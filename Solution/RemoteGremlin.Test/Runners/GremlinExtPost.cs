namespace RexConnectClient.Test.Runners {

	/*================================================================================================*/
	public class GremlinExtPost : BaseHttpPost {
		

		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public GremlinExtPost() {
			vUrl = "http://"+TimingUtil.Host+":8182/graphs/graph/tp/gremlin";
			vScriptVar = "script";
		}

	}

}