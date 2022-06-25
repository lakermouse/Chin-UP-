using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Wuensch {
	/// <summary>
	/// static base class to handle the mapping of floats from one range into another, can process float, Vector2 and Vector3
	/// </summary>
	public static class RangeMapper {

		/// <summary>
		/// remap float value from one range of numbers to another
		/// </summary>
		/// <param name="input"></param>
		/// <param name="fromMin"></param>
		/// <param name="fromMax"></param>
		/// <param name="toMin"></param>
		/// <param name="toMax"></param>
		/// <param name="clampMin"></param>
		/// <param name="clampMax"></param>
		/// <param name="cycleModulo"></param>
		/// <returns></returns>
		public static float Remap (float input, float fromMin, float fromMax, float toMin, float toMax, bool clampMin = false, bool clampMax = false, bool cycleModulo = false) {
			//**MODULO */
			/* if cycleModulo is active input will cycle. This means if input is bigger then toMax 
			input will be calculated relative to toMin. This is useful for example
			when working with 360 degrees (361 degrees will translate to 1 degree then)*/

			if (cycleModulo) {
				if (input > fromMax || input < fromMin) {
					//get the distance 

					float distIn = GetDistance (fromMin, input);
					float distBase = GetDistance (fromMin, fromMax);
					float xMod = GetModulo (distIn, distBase);
					//Debug.Log ("DistIn: " + distIn + " distBase: " + distBase + " xMod: " + xMod); //output

					//apply 
					input = fromMin + xMod;
				}

			}

			//**CLAMP */
			/* restrict the input value to the input range.Separate for fromMin and fromMax. 
	
			If the input is outside the range, it is clamped to the fromMin or fromMax value.
			For example, suppose the input range is 0 to 360 and the output 
			range is 0 to 100. An input value of 380 would be clamped to 360 
			(the upper limit of the input range), leading to a remapped output of 100. */

			if (clampMin && input < fromMin) {
				input = fromMin;
			}
			if (clampMax && input > fromMax) {
				input = fromMax;
			}

			//now  remap the input

			var fromVal = input - fromMin;
			var fromMaxVal = fromMax - fromMin; //this could lead to a division by 0 if they are identical
			float normalizedValue;
			//avoid division by Zero
			if (fromMaxVal != 0f) {
				normalizedValue = fromVal / fromMaxVal;
			} else {
				normalizedValue = 0f;
			}

			var toMaxVal = toMax - toMin;
			var toVal = toMaxVal * normalizedValue;

			var result = toVal + toMin;

			return result;
		}
		/// <summary>
		/// returns the distance of two floats, no matter if they are positive or negative
		/// </summary>
		/// <param name="x"></param>
		/// <param name="n"></param>
		/// <returns></returns>
		public static float GetDistance (float x, float n) {
			float res = Mathf.Abs (x - n);
			return res;

		}
		/// <summary>
		/// returns the remainder of an equation of 2 floats (modulo operation)
		/// </summary>
		/// <param name="x"></param>
		/// <param name="n"></param>
		/// <returns></returns>
		public static float GetModulo (float x, float n) {
			float res = (x % n + n) % n;
			return res;
		}

		/// <summary>
		/// remap Vector3 values from one range to another
		/// </summary>
		/// <param name="input"></param>
		/// <param name="fromMin"></param>
		/// <param name="fromMax"></param>
		/// <param name="toMin"></param>
		/// <param name="toMax"></param>
		/// <param name="clampMin"></param>
		/// <param name="clampMax"></param>
		/// <param name="cycleModulo"></param>
		/// <returns></returns>

		public static Vector3 RemapVector3 (Vector3 input, Vector3 fromMin, Vector3 fromMax, Vector3 toMin, Vector3 toMax, bool clampMin = false, bool clampMax = false, bool cycleModulo = false) {
			Vector3 result = new Vector3 (0, 0, 0);
			result.x = Remap (input.x, fromMin.x, fromMax.x, toMin.x, toMax.x, clampMin, clampMax, cycleModulo);
			result.y = Remap (input.y, fromMin.y, fromMax.y, toMin.y, toMax.y, clampMin, clampMax, cycleModulo);
			result.z = Remap (input.z, fromMin.z, fromMax.z, toMin.z, toMax.z, clampMin, clampMax, cycleModulo);
			return result;

		}
		/// <summary>
		/// remap Vector2 values from one range to another
		/// </summary>
		/// <param name="input"></param>
		/// <param name="fromMin"></param>
		/// <param name="fromMax"></param>
		/// <param name="toMin"></param>
		/// <param name="toMax"></param>
		/// <param name="clampMin"></param>
		/// <param name="clampMax"></param>
		/// <param name="cycleModulo"></param>
		/// <returns></returns>
		public static Vector2 RemapVector2 (Vector2 input, Vector2 fromMin, Vector2 fromMax, Vector2 toMin, Vector2 toMax, bool clampMin = false, bool clampMax = false, bool cycleModulo = false) {
			Vector2 result = new Vector3 (0, 0);
			result.x = Remap (input.x, fromMin.x, fromMax.x, toMin.x, toMax.x, clampMin, clampMax, cycleModulo);
			result.y = Remap (input.y, fromMin.y, fromMax.y, toMin.y, toMax.y, clampMin, clampMax, cycleModulo);
			return result;

		}
		/// <summary>
		/// Remap Float and apply curves for each axis
		/// </summary>
		/// <param name="input"></param>
		/// <param name="fromMin"></param>
		/// <param name="fromMax"></param>
		/// <param name="toMin"></param>
		/// <param name="toMax"></param>
		/// <param name="clampMin"></param>
		/// <param name="clampMax"></param>
		/// <param name="cycleModulo"></param>
		/// <param name="remapCurve"></param>
		/// <returns></returns>

		public static float RemapCurve (float input, float fromMin, float fromMax, float toMin, float toMax, bool clampMin = false, bool clampMax = false, bool cycleModulo = false, AnimationCurve remapCurve = null) {
			if (remapCurve != null && remapCurve.length != 0) {
				//mapping input to curve space
				float myInputNormalized = RangeMapper.Remap (input, fromMin, fromMax, 0f, 1f, clampMin, clampMax, cycleModulo);
				//use normalized value to retrieve curve value
				float curveInputNormalized = remapCurve.Evaluate (myInputNormalized);
				//remap normalized curve value to output range
				return RangeMapper.Remap (curveInputNormalized, 0f, 1f, toMin, toMax, clampMin, clampMax, cycleModulo);

			} else {
				//no curve set or not to be used
				return RangeMapper.Remap (input, fromMin, fromMax, toMin, toMax, clampMin, clampMax, cycleModulo);
			}

		}

		/// <summary>
		/// Remap Vector3 and apply curves for each axis
		/// </summary>
		/// <param name="input"></param>
		/// <param name="fromMin"></param>
		/// <param name="fromMax"></param>
		/// <param name="toMin"></param>
		/// <param name="toMax"></param>
		/// <param name="clampMin"></param>
		/// <param name="clampMax"></param>
		/// <param name="cycleModulo"></param>
		/// <param name="remapCurves"></param>
		/// <returns></returns>
		public static Vector3 RemapVector3Curve (Vector3 input, Vector3 fromMin, Vector3 fromMax, Vector3 toMin, Vector3 toMax, bool clampMin = false, bool clampMax = false, bool cycleModulo = false, AnimationCurve[] remapCurves = null) {

			Vector3 curveInputNormalized = new Vector3 (0f, 0f, 0f);
			Vector3 myResult = new Vector3 (0f, 0f, 0f);
			if (remapCurves != null && remapCurves.Length >= 3) { //curves for all 3 axis are present
				AnimationCurve remapCurveX = remapCurves[0];
				AnimationCurve remapCurveY = remapCurves[1];
				AnimationCurve remapCurveZ = remapCurves[2];
				Vector3 ZeroVect = new Vector3 (0f, 0f, 0f);
				Vector3 OneVect = new Vector3 (1f, 1f, 1f);

				//mapping input to curve space
				Vector3 myInputNormalized = RemapVector3 (input, fromMin, fromMax, ZeroVect, OneVect, clampMin, clampMax, cycleModulo);
				//use normalized value to retrieve curve value, check if curve is set first
				if (remapCurveX.length > 0) {
					curveInputNormalized.x = remapCurveX.Evaluate (myInputNormalized.x);
				} else {
					curveInputNormalized.x = myInputNormalized.x;
				}

				if (remapCurveY.length > 0) {
					curveInputNormalized.y = remapCurveY.Evaluate (myInputNormalized.y);
				} else {
					curveInputNormalized.y = myInputNormalized.y;
				}

				if (remapCurveZ.length > 0) {
					curveInputNormalized.z = remapCurveZ.Evaluate (myInputNormalized.z);
				} else {
					curveInputNormalized.z = myInputNormalized.z;
				}
				//remap normalized curve value to output range
				myResult = RemapVector3 (curveInputNormalized, ZeroVect, OneVect, toMin, toMax, clampMin, clampMax, cycleModulo);

			} else {
				//no curve set or not to be used, return Remap without curves
				myResult = RemapVector3 (curveInputNormalized, fromMin, fromMax, toMin, toMax, clampMin, clampMax, cycleModulo);
			}

			return myResult;
		}
		/// <summary>
		/// Remap Vector2 and apply curves for each axis
		/// </summary>
		/// <param name="input"></param>
		/// <param name="fromMin"></param>
		/// <param name="fromMax"></param>
		/// <param name="toMin"></param>
		/// <param name="toMax"></param>
		/// <param name="clampMin"></param>
		/// <param name="clampMax"></param>
		/// <param name="cycleModulo"></param>
		/// <param name="remapCurves"></param>
		/// <returns></returns>
		public static Vector2 RemapVector2Curve (Vector2 input, Vector2 fromMin, Vector2 fromMax, Vector2 toMin, Vector2 toMax, bool clampMin = false, bool clampMax = false, bool cycleModulo = false, AnimationCurve[] remapCurves = null) {
			Vector2 curveInputNormalized = new Vector2 (0f, 0f);
			Vector2 myResult = new Vector2 (0f, 0f);
			if (remapCurves != null && remapCurves.Length >= 2) { //curves for all 2 axis are present
				AnimationCurve remapCurveX = remapCurves[0];
				AnimationCurve remapCurveY = remapCurves[1];

				Vector2 ZeroVect = new Vector2 (0f, 0f);
				Vector2 OneVect = new Vector2 (1f, 1f);

				//mapping input to curve space
				Vector3 myInputNormalized = RemapVector2 (input, fromMin, fromMax, ZeroVect, OneVect, clampMin, clampMax, cycleModulo);
				//use normalized value to retrieve curve value, check if curve is set first
				if (remapCurveX.length > 0) {
					curveInputNormalized.x = remapCurveX.Evaluate (myInputNormalized.x);
				} else {
					curveInputNormalized.x = myInputNormalized.x;
				}

				if (remapCurveY.length > 0) {
					curveInputNormalized.y = remapCurveY.Evaluate (myInputNormalized.y);
				} else {
					curveInputNormalized.y = myInputNormalized.y;
				}

				//remap normalized curve value to output range
				myResult = RemapVector2 (curveInputNormalized, ZeroVect, OneVect, toMin, toMax, clampMin, clampMax, cycleModulo);

			} else {
				//no curve set or not to be used, return Remap without curves
				myResult = RemapVector2 (curveInputNormalized, fromMin, fromMax, toMin, toMax, clampMin, clampMax, cycleModulo);
			}

			return myResult;
		}
		//***** */

	}
}