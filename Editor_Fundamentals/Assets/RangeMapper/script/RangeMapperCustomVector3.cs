using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wuensch {
    /// <summary>
    /// a custom RangeMapper component for Vector3 with curves modification (optional) when remapping values
    /// </summary>

    public class RangeMapperCustomVector3 : MonoBehaviour {
        [Header ("Custom Vector3 Rangemapper")]
        [Tooltip ("unique name to identify the Rangemapper.")]
        public string myName = "my Name here";
        [Tooltip ("the input value to map")]
        public Vector3 input;
        [Tooltip ("the minimum value of the input range")]
        public Vector3 fromMin = new Vector3 (0f, 0f, 0f);
        [Tooltip ("the maximum value of the input range")]

        public Vector3 fromMax = new Vector3 (1f, 1f, 1f);
        [Tooltip ("the minimum value of the target range")]

        public Vector3 toMin = new Vector3 (0f, 0f, 0f);
        [Tooltip ("the maximum value of the target range")]
        public Vector3 toMax = new Vector3 (100f, 100f, 100f);
        [Tooltip ("should he input minimum be clamped?")]
        public bool clampMin = false;
        [Tooltip ("should he input maximum be clamped?")]
        public bool clampMax = false;
        [Tooltip ("should the input cycle? Useful to convert 360 degree values or clock dials")]
        public bool cycleModulo = false;
        [Tooltip ("should the curve be used to map values? Unused (no points) axis curves are replaced by straight input.")]
        public bool useCurves = false;
        [Tooltip ("an optional curve to manipulate the mapping on axis X between min and max range. If input values are outside min max range the curve defaults to clamping the values ")]
        public AnimationCurve remapCurveX;
        [Tooltip ("an optional curve to manipulate the mapping on axis Y between min and max range. If input values are outside min max range the curve defaults to clamping the values ")]
        public AnimationCurve remapCurveY;
        [Tooltip ("an optional curve to manipulate the mapping on axis Z between min and max range. If input values are outside min max range the curve defaults to clamping the values ")]
        public AnimationCurve remapCurveZ;
        [Tooltip ("the last output value as calculated as result of the RangeMapCustom function.")]
        public Vector3 output;

        // Use this for initialization
        void Start () {
            RangeMapCustom (input);
        }

        // Update is called once per frame
        void Update () {

        }
        /// <summary>
        /// use this custom RangeMapper Vector3
        /// </summary>
        /// <param name="myInput"></param>
        /// <returns></returns>
        public Vector3 RangeMapCustom (Vector3 myInput) {
            input = myInput;
            Vector3 myResult;
            //using curves?
            if (useCurves == true && (remapCurveX.length > 0 || remapCurveY.length > 0 || remapCurveZ.length > 0)) {
                AnimationCurve[] remapCurves = { remapCurveX, remapCurveY, remapCurveZ }; //pass all 3 axis curves in array
                //remap normalized curve value to output range through curves
                myResult = RangeMapper.RemapVector3Curve (input, fromMin, fromMax, toMin, toMax, clampMin, clampMax, cycleModulo, remapCurves);
            } else {
                //no curve set or not to be used
                myResult = RangeMapper.RemapVector3 (input, fromMin, fromMax, toMin, toMax, clampMin, clampMax, cycleModulo);
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
        public Vector3 RangeMapCustom (Vector3 myInputX, Vector3 fromMinX, Vector3 fromMaxX, Vector3 toMinX, Vector3 toMaxX, bool clampMinX = false, bool clampMaxX = false, bool cycleModuloX = false) {
            //copy passed values to public variables
            input = myInputX;
            fromMin = fromMinX;
            fromMax = fromMaxX;
            toMin = toMinX;
            toMax = toMaxX;
            clampMin = clampMinX;
            clampMax = clampMaxX;
            cycleModulo = cycleModuloX;
            Vector3 myResult;
            //using curves?
            if (useCurves == true && (remapCurveX.length > 0 || remapCurveY.length > 0 || remapCurveZ.length > 0)) {
                AnimationCurve[] remapCurves = { remapCurveX, remapCurveY, remapCurveZ }; //pass all 3 axis curves in array
                //remap normalized curve value to output range through curves
                myResult = RangeMapper.RemapVector3Curve (input, fromMin, fromMax, toMin, toMax, clampMin, clampMax, cycleModulo, remapCurves);
            } else {
                //no curve set or not to be used
                myResult = RangeMapper.RemapVector3 (input, fromMin, fromMax, toMin, toMax, clampMin, clampMax, cycleModulo);
            }
            output = myResult;
            return myResult;

        }
    }
}