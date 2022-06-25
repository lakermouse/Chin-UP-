using TMPro;
using UnityEngine;

namespace NullSave
{
    public class SetText : MonoBehaviour
    {

        public TMP_InputField textSource;
        public TextMeshProUGUI destination;

        public void DoSetText()
        {
            destination.text = textSource.text;
        }

    }
}