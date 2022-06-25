using UnityEngine;

namespace DarkTonic.CoreGameKit.Examples
{
    public class KW_MenuInstructions : MonoBehaviour
    {
        void OnGUI()
        {
            GUI.Label(new Rect(10, 10, 760, 60), "Every time you get to this scene, your score will be reset to zero.");

            if (GUI.Button(new Rect(10, 50, 760, 60), "Start game"))
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(2);
            }
        }
    }
}