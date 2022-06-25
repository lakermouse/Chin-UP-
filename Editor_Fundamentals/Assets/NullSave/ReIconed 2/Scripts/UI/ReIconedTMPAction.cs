using Rewired;
using TMPro;
using UnityEngine;

namespace NullSave
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class ReIconedTMPAction : MonoBehaviour
    {

        #region Variables

        [Tooltip("Rewired Player Id")] public int playerId;
        [Tooltip("How should we lookup this action?")] public ReIconedActionLookup actionLookup;
        [Tooltip("Which Action should we monitor?")] public string actionName;
        [Tooltip("Which Action Id should we monitor?")] public int actionId;
        [Tooltip("What text do we want to update with the action?"), TextArea(2, 5)] public string formatText;
        [Tooltip("What text do we want to use if we can't find the action?"), TextArea(2, 5)] public string notFoundText;
        [Tooltip("Allow sprite tinting?")] public bool tint;

        private TextMeshProUGUI tmpText;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            tmpText = GetComponent<TextMeshProUGUI>();
        }

        private void Start()
        {
            ReInput.InputSourceUpdateEvent += UpdateUI;
            UpdateUI();
        }

        private void Reset()
        {
            playerId = 0;
            actionName = "Horizontal";
            actionId = 0;
            formatText = "Horizontal: {action}";
            notFoundText = "<sprite=0>";
            tint = true;
        }

        #endregion

        #region Public Methods

        public void SetFormatText(string value)
        {
            formatText = value;
            UpdateUI();
        }

        #endregion

        #region Private Methods

        private void UpdateUI()
        {
            InputMap map = actionLookup == ReIconedActionLookup.Name ? ReIconed.GetActionHardwareInput(actionName, playerId) : ReIconed.GetActionHardwareInput(actionId, playerId);

            if (map == null)
            {
                tmpText.spriteAsset = null;
                tmpText.text = formatText.Replace("{action}", notFoundText);
            }
            else
            {
                tmpText.spriteAsset = map.TMPSpriteAsset;
                tmpText.text = formatText.Replace("{action}", "<sprite=" + map.tmpSpriteIndex + " tint=" + (tint ? "1" : "0") + ">");
            }
        }

        #endregion

    }
}