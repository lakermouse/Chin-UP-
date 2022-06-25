using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wuensch.Demo {
	public class DemoClock : MonoBehaviour {
		[Tooltip("link to RangeMapperManager set")]
		public RangeMapperManager myManager;
		[Tooltip("The gameobjects to animate")]
		public GameObject SecondsGameObject;
		[Tooltip("The gameobjects to animate")]
		public GameObject MinutesGameObject;
		[Tooltip("The gameobjects to animate")]
		public GameObject HoursGameObject;
		

		// Use this for initialization
		void Awake () {
			myManager = Object.FindObjectOfType<RangeMapperManager> ();

		}
		void Start () {
			InvokeRepeating ("MapRealTimeToClock", 1f, 1f); //to save performance only execute every second
		}

		// Update is called once per frame
		void Update () {

		}
		private void MapRealTimeToClock () {
			//***HOURS */ call by Name example
			//using names is a bit slower then storing a direct reference 
			//to the component and the calling, but it makesit easy to identify components 
			//of the same type stored on the same GameObject
			//we a custom Rangemapper to map the hours to the Euler Rotation of the clock
			//call this custom Rangemapper by NAME via RangeMapperManager 

			float DegreesHours = myManager.RangemapCustom (System.DateTime.Now.Hour, "Hours");

			//Hours is range 0 to 23 from System
			//the RangeMapperCustom is set to an input range of 0 to 11 and cycle modulo to mimic a 12 hours clock
			//EulerAngles should never be incremented above 360, so the cycle modulo option of the RangeMapperCustom is also useful for this.
			//always pass a full vector to set the Eulerangles, passing only one axis can lead to unwanted results

			HoursGameObject.transform.localEulerAngles = new Vector3 (0, 0, DegreesHours);

			//**Minutes and Seconds */ call by name example
			

			float DegreesMinutes = myManager.RangemapCustom (System.DateTime.Now.Minute, "SecondsMinutes");
			MinutesGameObject.transform.localEulerAngles = new Vector3 (0, 0, DegreesMinutes);
			float DegreesSeconds = myManager.RangemapCustom (System.DateTime.Now.Second, "SecondsMinutes");
			SecondsGameObject.transform.localEulerAngles = new Vector3 (0, 0, DegreesSeconds);
		}
	}
}