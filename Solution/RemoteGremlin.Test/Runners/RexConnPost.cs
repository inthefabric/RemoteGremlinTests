﻿namespace RexConnectClient.Test.Runners {

	/*================================================================================================*/
	public class RexConnPost : BaseHttpPost {
		

		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public RexConnPost() {
			vUseRexConnReq = true;
			vUrl = "http://"+TimingUtil.Host+":8182/graphs/graph/fabric/rexconnect";
			vScriptVar = "req";
		}

	}

}