using RexConnectClient.Core.Transfer;

namespace RexConnectClient.Test.Fixtures {

	/*================================================================================================*/
	public abstract class TimingQueryBase : TimingBase {


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		protected abstract string GetQuery();


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		protected override ResultSet GetResultSet() {
			return new ResultSet(GetQuery());
		}

		/*--------------------------------------------------------------------------------------------*/
		protected override ResultSet GetResultSetWithRexConnRequest() {
			var r = new Request("123");
			r.AddQuery(GetQuery());
			return new ResultSet(r);
		}

		/*--------------------------------------------------------------------------------------------*/
		protected override int GetRunCount() {
			return 200;
		}

	}

}