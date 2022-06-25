using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Wuensch.Demo {
    public class DemoClockFastClock : MonoBehaviour {
        [Tooltip("RangeMapperManager is automatically set up")]
        public RangeMapperManager myManager;
        [Tooltip("The gameobjects to animate")]
        public GameObject SecondsGameObject;
        [Tooltip("The gameobjects to animate")]
        public GameObject MinutesGameObject;
        [Tooltip("The gameobjects to animate")]
        public GameObject HoursGameObject;
        [Tooltip("The Rangemappers for the clock as reference. Because they are easier to assign they are on different Gameobjects.")]
		public RangeMapperCustom SecondsMapper;
        [Tooltip("The Rangemappers for the clock as reference. Because they are easier to assign they are on different Gameobjects.")]
		public RangeMapperCustom MinutesMapper;
        [Tooltip("The Rangemappers for the clock as reference. Because they are easier to assign they are on different Gameobjects.")]
		public RangeMapperCustom HoursMapper;
        private float virtualSeconds;
        // Use this for initialization
        void Awake () {
            myManager = Object.FindObjectOfType<RangeMapperManager> ();

        }
        void Start () {
            
            InvokeRepeating ("MapFastTimeToClock", 0.07f, 0.07f); //to save performance and for the nice clock stutter effect only execute every 0.07f second
        }

        // Update is called once per frame
        void Update () {
            
        }
        private void MapFastTimeToClock () {
            virtualSeconds = Time.time*500; //count up virtual seconds faster then real

            //**This clock applie sthe real time from System */
            //***HOURS */ call by Name example
            //using names is a bit slower then storing a direct reference 
            //to the component and the calling, but it makesit easy to identify components 
            //of the same type stored on the same GameObject
            //we a custom Rangemapper to map the hours to the Euler Rotation of the clock
            //call this custom Rangemapper by NAME via RangeMapperManager 

            float DegreesHours = HoursMapper.RangeMapCustom (virtualSeconds);

            //Hours is in 12 hours system.  24 hours has 86400 seconds, the
            //RangeMapperCustom will only process half of that as fromMin to mimic a 12 hour clock.

            HoursGameObject.transform.localEulerAngles = new Vector3 (0, 0, DegreesHours);

            //**Minutes and Seconds */ call by name example
            //60 seconds in a minute

            float DegreesMinutes = MinutesMapper.RangeMapCustom (virtualSeconds);
            MinutesGameObject.transform.localEulerAngles = new Vector3 (0, 0, DegreesMinutes);
            float DegreesSeconds = SecondsMapper.RangeMapCustom (virtualSeconds);
            SecondsGameObject.transform.localEulerAngles = new Vector3 (0, 0, DegreesSeconds);
        }
    }
}