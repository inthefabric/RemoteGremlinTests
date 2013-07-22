using System;
using RexConnectClient.Core;
using RexConnectClient.Core.Transfer;

namespace RexConnectClient.Test {

	/*================================================================================================*/
	public class TestRexConnCtx : RexConnContext {
		

		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public TestRexConnCtx(Request pReq, string pHost, int pPort) : base(pReq, pHost, pPort) {}

		/*--------------------------------------------------------------------------------------------*/
		//Ignore all logging
		public override void Log(string pType, string pCategory, string pText,
																		Exception pException=null) {}

	}

}