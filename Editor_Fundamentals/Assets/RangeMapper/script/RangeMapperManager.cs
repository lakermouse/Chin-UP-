using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Wuensch {
	/// <summary>
	/// searches the scene for existing RangeMapperCustoms and stores in dictionaries.Used to access all RangeMappersCustoms by unique names.
	/// </summary>
	public class RangeMapperManager : MonoBehaviour {
		[Header ("Call all RangeMapperCustom components in scene by name")]
		[TextArea]
		[Tooltip ("Remark below")]
		public string Remark = "You only need one RangeMapperManager in your scene. Use functions RangemapCustom or RangemapCustomVector2 or RangemapCustomVector3  of this component to call any custom Rangemapper by name.";
		[Tooltip ("Show all activities in Console")]
		public bool logRangeMapperManager;
		[Tooltip ("array with the names of all found RangeMapperCustom components")]

		private RangeMapperCustom[] myRangeMappersFloat;
		private RangeMapperCustomVector3[] myRangeMappersVector3;
		private RangeMapperCustomVector2[] myRangeMappersVector2;

		private Dictionary<string, RangeMapperCustom> DictRangeMappers = new Dictionary<string, RangeMapperCustom> ();
		private Dictionary<string, RangeMapperCustomVector3> DictRangeMappersVector3 = new Dictionary<string, RangeMapperCustomVector3> ();
		private Dictionary<string, RangeMapperCustomVector2> DictRangeMappersVector2 = new Dictionary<string, RangeMapperCustomVector2> ();
		// Use this for initialization
		void Awake () {
			InitDictionaries ();

		}
		void Start () {

		}

		// Update is called once per frame
		void Update () {

		}
		/// <summary>
		/// make a dictionary to access the RamgemapperCustos by name
		/// </summary>
		private void InitDictionaries () {
			myRangeMappersFloat = FindObjectsOfType<RangeMapperCustom> ();
			myRangeMappersVector3 = FindObjectsOfType<RangeMapperCustomVector3> ();
			myRangeMappersVector2 = FindObjectsOfType<RangeMapperCustomVector2> ();

			//Dictionary float Rangemappers

			if (myRangeMappersFloat.Length != 0) {
				List<string> checkNames = new List<string> ();
				foreach (var item in myRangeMappersFloat) {
					if (item is RangeMapperCustom) {

						if (!checkNames.Contains (item.myName)) {
							checkNames.Add (item.myName);
							DictRangeMappers.Add (item.myName, item);
							OutputMessage ("Added RangeMapper:" + item.myName);
						} else {

							if (item.myName == "") {
								OutputMessage ("RangeMappers without name in scene moren then one time!");
							} else {
								OutputMessage ("RangeMapper name present more then one time: " + item.myName);
							}

						}

						
					}
				}
				OutputMessage ("Dictionary created with " + myRangeMappersFloat.Length + " RangeMapperCustom components.");

			} else {

				OutputMessage ("Could not find any components of type RangeMapperCustom in Scene. No Dictionary created.");

			}
			//Dictionary Vector3 Rangemappers
			if (myRangeMappersVector3.Length != 0) {
				List<string> checkNames = new List<string> ();
				foreach (var item in myRangeMappersVector3) {
					if (item is RangeMapperCustomVector3) {
						if (!checkNames.Contains (item.myName)) {
							checkNames.Add (item.myName);
							DictRangeMappersVector3.Add (item.myName, item);
							OutputMessage ("Added RangeMapperVector3:" + item.myName);
						} else {

							if (item.myName == "") {
								OutputMessage ("RangeMapperVector3 without name in scene moren then one time!" );
							} else {
								OutputMessage ("RangeMapperVector3 name present more then one time: " + item.myName);
							}

						}
						
					}
				}

				OutputMessage (("Dictionary created with " + myRangeMappersVector3.Length + " RangeMapperCustomVector3 components."));

			} else {

				OutputMessage ("Could not find any components of type RangeMapperCustomVector3 in Scene. No Dictionary created.");

			}
			//Dictionary Vector2 Rangemappers
			if (myRangeMappersVector2.Length != 0) {
				List<string> checkNames = new List<string> ();
				foreach (var item in myRangeMappersVector2) {
					if (item is RangeMapperCustomVector2) {
							if (!checkNames.Contains (item.myName)) {
							checkNames.Add (item.myName);
							OutputMessage ("Added RangeMapperVector2:" + item.myName);
							DictRangeMappersVector2.Add (item.myName, item);
						} else {

							if (item.myName == "") {
								OutputMessage ("RangeMapperVector2 without name in scene moren then one time! " + item.myName);
							} else {
								OutputMessage ("RangeMapperVector2 name present more then one time: " + item.myName);
							}

						}
						
					}
				}
				OutputMessage ("Dictionary created with " + myRangeMappersVector2.Length + " RangeMapperCustomVector2 components.");

			} else {

				OutputMessage ("Could not find any components of type RangeMapperCustomVector2 in Scene. No Dictionary created.");

			}

		}

		/// <summary>
		/// //uses float RangeMapperCustom  component by name
		/// </summary>
		/// <param name="valueToMap"></param>
		/// <param name="rangeMapperCustomName"></param>
		/// <returns></returns>
		public float RangemapCustom (float valueToMap, string rangeMapperCustomName) {

			if (DictRangeMappers.ContainsKey (rangeMapperCustomName)) {
				RangeMapperCustom tempRangeMapper;
				bool tempEnabled = false; //just a helper if a Rangemapper has been turned off		

				DictRangeMappers.TryGetValue (rangeMapperCustomName, out tempRangeMapper);
				if (!tempRangeMapper.enabled) {
					tempRangeMapper.enabled = true; //store info to turn component off again, we assume this is wanted 
					tempEnabled = true; //temporaily turn the component on
				}
				float result = tempRangeMapper.RangeMapCustom (valueToMap);
				if (tempEnabled) { tempRangeMapper.enabled = false; } //turn component off again because it originally was
				OutputMessage ("RangeMapCustom name: " + rangeMapperCustomName + " in: " + valueToMap + " out: " + result);
				return result;
			} else {
				// error: Rangemapper with that name not found
				OutputMessage ("!!!ERROR: cannot find RangemapperCustom that has the name: " + rangeMapperCustomName + "! Returning 0! Please check your Rangemappers name.", true);
				return 0f;
			}

		}
		/// <summary>
		/// uses Vector3 RangeMapperCustom  component by name
		/// </summary>
		/// <param name="valueToMap"></param>
		/// <param name="rangeMapperCustomName"></param>
		/// <returns></returns>
		public Vector3 RangemapCustomVector3 (Vector3 valueToMap, string rangeMapperCustomName) {

			if (DictRangeMappersVector3.ContainsKey (rangeMapperCustomName)) {
				RangeMapperCustomVector3 tempRangeMapper;
				bool tempEnabled = false; //just a helper if a Rangemapper has been turned off		

				DictRangeMappersVector3.TryGetValue (rangeMapperCustomName, out tempRangeMapper);
				if (!tempRangeMapper.enabled) {
					tempRangeMapper.enabled = true; //store info to turn component off again, we assume this is wanted 
					tempEnabled = true; //temporaily turn the component on
				}
				Vector3 result = tempRangeMapper.RangeMapCustom (valueToMap);
				if (tempEnabled) { tempRangeMapper.enabled = false; } //turn component off again because it originally was
				OutputMessage ("RangeMapCustomVector3 name: " + rangeMapperCustomName + " in: " + valueToMap + " out: " + result);
				return result;
			} else {
				// error: Rangemapper with that name not found
				OutputMessage ("!!!ERROR: cannot find RangemapperCustomVector3 that has the name: " + rangeMapperCustomName + "! Returning Vector3 (0f,0f,0f)! Please check your Rangemappers name.", true);
				return new Vector3 (0f, 0f, 0f);
			}

		}

		/// <summary>
		/// uses Vector2 RangeMapperCustom  component by name
		/// </summary>
		/// <param name="valueToMap"></param>
		/// <param name="rangeMapperCustomName"></param>
		/// <returns></returns>
		public Vector2 RangemapCustomVector2 (Vector2 valueToMap, string rangeMapperCustomName) {

			if (DictRangeMappersVector2.ContainsKey (rangeMapperCustomName)) {
				RangeMapperCustomVector2 tempRangeMapper;
				bool tempEnabled = false; //just a helper if a Rangemapper has been turned off		

				DictRangeMappersVector2.TryGetValue (rangeMapperCustomName, out tempRangeMapper);
				if (!tempRangeMapper.enabled) {
					tempRangeMapper.enabled = true; //store info to turn component off again, we assume this is wanted 
					tempEnabled = true; //temporaily turn the component on
				}
				Vector2 result = tempRangeMapper.RangeMapCustom (valueToMap);
				if (tempEnabled) { tempRangeMapper.enabled = false; } //turn component off again because it originally was
				OutputMessage ("RangeMapCustomVector2 name: " + rangeMapperCustomName + " in: " + valueToMap + " out: " + result);
				return result;
			} else {
				// error: Rangemapper with that name not found
				OutputMessage ("!!!ERROR: cannot find RangemapperCustomVector2 that has the name: " + rangeMapperCustomName + "! Returning Vector2 (0f,0f)! Please check your Rangemappers name.", true);
				return new Vector3 (0f, 0f);
			}

		}

		/// <summary>
		/// if logging with bool logRangeMapperManager is enabled write to console to see whats going on
		/// </summary>
		/// <param name="text"></param>
		/// <param name="overRide"></param>
		private void OutputMessage (string text, bool overRide = false) {
			if (logRangeMapperManager || overRide) {
				Debug.Log ("RangeMapperManager: " + text);
			}

		}
	}
}