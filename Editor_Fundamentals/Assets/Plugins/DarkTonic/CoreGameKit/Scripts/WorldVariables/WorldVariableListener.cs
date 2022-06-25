using UnityEngine;
using System.Text;

using UnityEngine.UI;

namespace DarkTonic.CoreGameKit {
    [RequireComponent(typeof(Text))]
    [AddComponentMenu("Dark Tonic/Core GameKit/Listeners/World Variable Listener")]
    // ReSharper disable once CheckNamespace
    /// <summary>
    /// This class is used to listen to key events for a World Variable. 
    /// </summary>
    public class WorldVariableListener : MonoBehaviour {
        // ReSharper disable InconsistentNaming
        /*! \cond PRIVATE */
        public string variableName = "";
        public WorldVariableTracker.VariableType vType = WorldVariableTracker.VariableType._integer;
        public bool displayVariableName = false;
        public int decimalPlaces = 1;
        public bool useCommaFormatting = true;
        public bool useFixedNumberOfDigits;
        public int fixedDigitCount = 8;
        // ReSharper restore InconsistentNaming
        /*! \endcond */

        private int _variableValue;
        private float _variableFloatValue;

        private Text _text;

        // ReSharper disable once UnusedMember.Local
        void Awake() {
            _text = GetComponent<Text>();
        }

        void OnEnable() {
            WorldVariableTracker.UpdateAllListeners();
        }

        /// <summary>
        /// This Gets called when the integer World Variable's value changes.
        /// </summary>
        /// <param name="newValue">new value</param>
        /// <param name="oldVal">old value</param>
        public virtual void UpdateValue(int newValue, int oldVal) {
            _variableValue = newValue;
            var valFormatted = new StringBuilder(string.Format("{0}{1}", displayVariableName ? variableName + ": " : "", _variableValue.ToString("N0")));

            if (!useCommaFormatting) {
                valFormatted = valFormatted.Replace(",", "");
            }

            if (_text == null || !SpawnUtility.IsActive(_text.gameObject)) {
                return;
            }

            if (useFixedNumberOfDigits) {
                while (valFormatted.Length < fixedDigitCount) {
                    valFormatted.Insert(0, "0");
                }
            }

            _text.text = valFormatted.ToString();
        }

        /// <summary>
        /// This Gets called when the float World Variable's value changes.
        /// </summary>
        /// <param name="newValue">new value</param>
        /// <param name="oldVal">old value</param>
        public virtual void UpdateFloatValue(float newValue, float oldVal) {
            _variableFloatValue = newValue;
            var valFormatted = new StringBuilder(string.Format("{0}{1}", displayVariableName ? variableName + ": " : "", _variableFloatValue.ToString("N" + decimalPlaces)));

            if (!useCommaFormatting) {
                valFormatted = valFormatted.Replace(",", "");
            }

            if (useFixedNumberOfDigits) {
                while (valFormatted.Length < fixedDigitCount) {
                    valFormatted.Insert(0, "0");
                }
            }

            _text.text = valFormatted.ToString();
        }
    }
}
