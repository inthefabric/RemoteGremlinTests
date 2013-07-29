using System;
using System.Collections.Generic;

namespace RexConnectClient.Test.Results {

	/*================================================================================================*/
	public class ResultSetHistogram : Dictionary<int, int> {

		public int MinTime { get; set; }
		public int MaxTime { get; set; }
		public int MaxOccurence { get; set; }
		

		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public ResultSetHistogram() {
			MinTime = int.MaxValue;
			MaxTime = 0;
			MaxOccurence = 0;
		}

		/*--------------------------------------------------------------------------------------------*/
		public void Build(ResultSet pResult) {
			foreach ( double sum in pResult.GetTimeSums() ) {
				int s = (int)sum;

				if ( !ContainsKey(s) ) {
					Add(s, 0);
				}

				this[s]++;
				MinTime = Math.Min(MinTime, s);
				MaxTime = Math.Max(MaxTime, s);
				MaxOccurence = Math.Max(MaxOccurence, this[s]);
			}
		}

	}

}