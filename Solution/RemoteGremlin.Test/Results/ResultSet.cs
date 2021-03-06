﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace RexConnectClient.Test.Results {

	/*================================================================================================*/
	public class ResultSet {

		public IList<string> JsonResults { get; private set; }
		public IList<IDictionary<string, double>> Executions { get; private set; }
		public IDictionary<string, double> CurrEx { get; private set; }
		

		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public ResultSet() {
			JsonResults = new List<string>();
			Executions = new List<IDictionary<string, double>>();
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public void AddExecution(string pResult) {
			/*if ( JsonResults.Count == 0 ) {
				Console.WriteLine("First result: "+pResult.Replace("\n", "").Replace("\r", ""));
			}*/

			JsonResults.Add(pResult);

			CurrEx = new Dictionary<string, double>();
			Executions.Add(CurrEx);

			/*Console.WriteLine("Result: "+pResult);

			if ( pResult != vResults[0] ) {
				throw new Exception("Results differ:\n\n"+
					"Result A: "+vResults[0]+"\n"+
					"Result B: "+pResult);
			}*/
		}

		/*--------------------------------------------------------------------------------------------*/
		public void AttachTime(string pKey, double pTime) {
			CurrEx.Add(pKey, pTime);
		}

		/*--------------------------------------------------------------------------------------------*/
		public void AttachTime(string pKey, Stopwatch pWatch) {
			AttachTime(pKey, pWatch.Elapsed.TotalMilliseconds);
		}

		/*--------------------------------------------------------------------------------------------*/
		public void AttachTime(string pKey, TimeSpan pSpan) {
			AttachTime(pKey, pSpan.TotalMilliseconds);
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public IList<double> GetTimes(string pKey) {
			return Executions.Select(ex => ex[pKey]).ToList();
		}

		/*--------------------------------------------------------------------------------------------*/
		public IList<double> GetTimeSums() {
			return Executions.Select(ex => ex.Values.Sum()).ToList();
		}

		/*--------------------------------------------------------------------------------------------*/
		public double GetAverageTime(string pKey) {
			return GetTimes(pKey).Average();
		}

		/*--------------------------------------------------------------------------------------------*/
		public double GetAverageTimeSum() {
			return GetTimeSums().Average();
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public ResultSetHistogram GetHistogram() {
			var hist = new ResultSetHistogram();
			hist.Build(this);
			return hist;
		}

	}

}