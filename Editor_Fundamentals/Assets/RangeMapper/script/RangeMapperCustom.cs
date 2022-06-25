using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Wuensch {
	/// <summary>
	/// a custom RangeMapper component for floats with curves modification (optional) when remapping values
	/// </summary>

	public class RangeMapperCustom : MonoBehaviour {
		[Header ("Custom Float Rangemapper")]
		[Tooltip ("unique name to identify the Rangemapper.")]
		public string myName = "my Name here";
		[Tooltip ("the input value to map")]
		public float input;
		[Tooltip ("the minimum value of the input range")]
		public float fromMin = 0f;
		[Tooltip ("the maximum value of the input range")]

		public float fromMax = 1f;
		[Tooltip ("the minimum value of the target range")]

		public float toMin = 0f;
		[Tooltip ("the maximum value of the target range")]
		public float toMax = 100f;
		[Tooltip ("should he input minimum be clamped?")]
		public bool clampMin = false;
		[Tooltip ("should he input maximum be clamped?")]
		public bool clampMax = false;
		[Tooltip ("should the input cycle? Useful to convert 360 degree values or clock dials")]
		public bool cycleModulo = false;
		[Tooltip ("should the curve be used to map values?")]
		public bool useCurve = false;
		[Tooltip ("an optional curve to manipulate the mapping between min and max range. If input values are outside min max range the curve defaults to clamping the values ")]
		public AnimationCurve remapCurve;

		[Tooltip ("the last output value as calculated as result of the RangeMapCustom function.")]
		public float output;
		// Use this for initialization
		void Start () {
			RangeMapCustom (input);

		}

		// Update is called once per frame
		void Update () {

		}
		/// <summary>
		/// use this custom RangeMapper
		/// </summary>
		/// <param name="myInput"></param>
		/// <returns></returns>
		public float RangeMapCustom (float myInput) {
			input = myInput;
			float myResult;
			if (remapCurve.length != 0 && useCurve) {
				//mapping input to curve space
				float myInputNormalized = RangeMapper.Remap (input, fromMin, fromMax, 0f, 1f, clampMin, clampMax, cycleModulo);
				//use normalized value to retrieve curve value
				float curveInputNormalized = remapCurve.Evaluate (myInputNormalized);
				//remap normalized curve value to output range
				myResult = RangeMapper.Remap (curveInputNormalized, 0f, 1f, toMin, toMax, clampMin, clampMax, cycleModulo);
			} else {
				//no curve set or not to be used
				myResult = RangeMapper.Remap (input, fromMin, fromMax, toMin, toMax, clampMin, clampMax, cycleModulo);
			}
			output = myResult;
			return myResult;

		}
		/// <summary>
		/// overload for RangeMapCustom, set all relevant public variables on the component 
		/// </summary>
		/// <param name="myInput"></param>
		/// <param name="fromMin"></param>
		/// <param name="fromMax"></param>
		/// <param name="toMin"></param>
		/// <param name="toMax"></param>
		/// <param name="clampMin"></param>
		/// <param name="clampMax"></param>
		/// <param name="cycleModulo"></param>
		/// <returns></returns>
		public float RangeMapCustom (float myInputX, float fromMinX, float fromMaxX, float toMinX, float toMaxX, bool clampMinX = false, bool clampMaxX = false, bool cycleModuloX = false) {
			//copy passed values to public variables
			input = myInputX;
			fromMin = fromMinX;
			fromMax = fromMaxX;
			toMin = toMinX;
			toMax = toMaxX;
			clampMin = clampMinX;
			clampMax = clampMaxX;
			cycleModulo = cycleModuloX;
			//****now use curve--map value to curvespace and get curve value and calculate
			float myResult;
			if (useCurve && remapCurve.length != 0) {
				myResult = RangeMapper.RemapCurve (input, fromMin, fromMax, toMin, toMax, clampMin, clampMax, cycleModulo, remapCurve);
			} else {
				//no curve set or not to be used
				myResult = RangeMapper.Remap (input, fromMin, fromMax, toMin, toMax, clampMin, clampMax, cycleModulo);
			}
			output = myResult;
			return myResult;

		}
	}
}