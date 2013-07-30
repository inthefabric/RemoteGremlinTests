namespace RexConnectClient.Test {

	/*================================================================================================*/
	public static class TimingUtil {

		public const string Host = "rexster";


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public static string MillisToString(double pMillis) {
			return pMillis.ToString("#0.00")+"ms";
		}
		
		/*--------------------------------------------------------------------------------------------*/
		public static string MillisToString(double pMillis, int pWidth) {
			return MillisToString(pMillis).PadLeft(pWidth);
		}

	}

}