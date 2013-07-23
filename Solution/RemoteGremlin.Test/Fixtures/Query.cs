﻿using NUnit.Framework;

namespace RexConnectClient.Test.Fixtures {

	/*================================================================================================*/
	[TestFixture]
	public class GetGraph : TimingQueryBase {


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		protected override string GetQuery() {
			return "g";
		}

	}


	/*================================================================================================*/
	[TestFixture]
	public class GetVertex : TimingQueryBase {


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		protected override string GetQuery() {
			return "g.v(4)";
		}

	}


	/*================================================================================================*/
	[TestFixture]
	public class GetBothCount : TimingQueryBase {


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		protected override string GetQuery() {
			return "g.V.both.count()";
		}

		/*--------------------------------------------------------------------------------------------*/
		protected override int GetRunCount() {
			return 200;
		}

	}


	/*================================================================================================*/
	[TestFixture]
	public class GetVertices : TimingQueryBase {


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		protected override string GetQuery() {
			return "g.V[0..30]";
		}

	}

}