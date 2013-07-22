using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using RexConnectClient.Core.Transfer;
using ServiceStack.Text;

namespace RexConnectClient.Test {

	/*================================================================================================*/
	public class ResultSet {

		public string Name { get; set; }
		public string Script { get; private set; }
		public Request RexConnRequest { get; private set; }

		public IList<string> JsonResults { get; private set; }
		public IList<IDictionary<string, double>> Executions { get; private set; }
		public IDictionary<string, double> CurrEx { get; private set; }
		

		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public ResultSet(string pScript) {
			Script = pScript;
			JsonResults = new List<string>();
			Executions = new List<IDictionary<string, double>>();
		}
		
		/*--------------------------------------------------------------------------------------------*/
		public ResultSet(Request pRexConnRequest) : this("") {
			RexConnRequest = pRexConnRequest;

			JsConfig.EmitCamelCaseNames = true;
			Script = JsonSerializer.SerializeToString(RexConnRequest);
			JsConfig.EmitCamelCaseNames = false;
		}

		/*--------------------------------------------------------------------------------------------*/
		public ResultSet Clone() {
			if ( RexConnRequest != null ) {
				return new ResultSet(RexConnRequest);
			}

			return new ResultSet(Script);
		}

		/*--------------------------------------------------------------------------------------------*/
		public void SetName(string pName) {
			if ( Name != null ) {
				throw new Exception("ResultSet already has a name: "+Name);
			}

			Name = pName;
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public void AddExecution(string pResult) {
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

	}

}