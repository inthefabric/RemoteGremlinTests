using RexConnectClient.Core.Transfer;
using RexConnectClient.Test.Results;

namespace RexConnectClient.Test.Runners {

	/*================================================================================================*/
	public interface IRunner {

		string Method { get; }
		string TestName { get; }
		string Script { get; }
		Request RexConnRequest { get; }
		ResultSet Results { get; }


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		void Prepare(string pTestName, string pScript, Request pRexConnRequest);

		/*--------------------------------------------------------------------------------------------*/
		void Run(bool pRecordResult=true);

		/*--------------------------------------------------------------------------------------------*/
		void RunMany(int pCount, bool pRecordResult=true);

	}

}