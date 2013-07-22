using NUnit.Framework;

namespace RexConnectClient.Test.Fixtures {

	/*================================================================================================*/
	[TestFixture]
	public class QueryG : TimingQueryBase {

		//private const string TestScriptB = "g.v(4)";
		//private const string TestScriptC = "g.V.both.both.count()";
		//private const string TestScriptD = "g.V[0..30]";


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		protected override string GetQuery() {
			return "g";
		}

	}

}