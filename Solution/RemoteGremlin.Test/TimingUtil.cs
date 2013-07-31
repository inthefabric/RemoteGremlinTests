namespace RexConnectClient.Test {

	/*================================================================================================*/
	public static class TimingUtil {

		public const string Host = "rexster";
		public const string GraphName = "graph";


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public static string MillisToSecString(double pMillis) {
			return (pMillis/1000.0).ToString("#0.0")+" sec";
		}
		
		/*--------------------------------------------------------------------------------------------*/
		public static string MillisToString(double pMillis) {
			return pMillis.ToString("#0.0")+"ms";
		}
		
		/*--------------------------------------------------------------------------------------------*/
		public static string MillisToString(double pMillis, int pWidth) {
			return MillisToString(pMillis).PadLeft(pWidth);
		}

	}

}