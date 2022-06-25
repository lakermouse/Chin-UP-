using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Wuensch;
namespace Wuensch.Demo {
	public class DemoCubeWobbler : MonoBehaviour {
		public RangeMapperCustomVector3 myRangeMapperVector3;
		public Slider mySlider;
		public GameObject myCube;
		private Vector3 tempVector = new Vector3 (0f, 0f,0f);

		// Use this for initialization
		void Start () {
	

		}

		/// <summary>
		/// use slider value to control cubes X and Y position via RangeMapperCustom curves
		/// </summary>
		void Update () {
			
			//construct a Vector3 to pass to the RangeMapper
			tempVector.x = mySlider.value;
			tempVector.y = mySlider.value;
			tempVector.z=0f;//Z position should not change, that is why the curve for z is flat value 0 also

			//apply resulting vector to position of cube
			myCube.transform.position = myRangeMapperVector3.RangeMapCustom (tempVector);
		}
	}
}