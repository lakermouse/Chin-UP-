using TMPro;
using UnityEngine;

namespace NullSave
{
    [System.Obsolete]
    public class ReIconedTMPMonitor : MonoBehaviour
    {

        #region Variables

        public int playerId = 0;
        [TextArea(2, 5)] public string notFoundText = "<sprite=0>";
        public bool tint = true;

        private TextMeshProUGUI tmpText;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            tmpText = GetComponent<TextMeshProUGUI>();
        }

        private void Update()
        {
            if (tmpText.text.Contains("{action:"))
            {
                UpdateUI();
            }
        }

        #endregion

        #region Private Methods

        private void UpdateUI()
        {
            string outputText = tmpText.text;
            string actionName;
            int i, e;
            InputMap map;

            while (true)
            {
                i = outputText.IndexOf("{action:");
                if (i < 0) break;
                e = outputText.IndexOf("}", i + 1);
                if (e < 0) e = outputText.Length;

                actionName = outputText.Substring(i + 8, e - i - 8);
                map = ReIconed.GetActionHardwareInput(actionName, playerId);
                if (map == null)
                {
                    tmpText.spriteAsset = null;
                    outputText = outputText.Substring(0, i) + notFoundText + outputText.Substring(e + 1);
                }
                else
                {
                    tmpText.spriteAsset = map.TMPSpriteAsset;
                    outputText = outputText.Substring(0, i) + "<sprite index=" + map.tmpSpriteIndex + " tint=" + (tint ? "1" : "0") + ">" + outputText.Substring(e + 1);
                }
            }


            tmpText.text = outputText;
        }

        #endregion

    }
}