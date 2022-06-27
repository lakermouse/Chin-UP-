

using UnityEditor;
using UnityEngine;

using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine.SceneManagement;



[AddComponentMenu("Nemisindo/Footsteps controller")]
public class Footsteps_Controller : MonoBehaviour
{

#if UNITY_IOS
      [DllImport("__Internal")]
#else
    [DllImport("audioplugin_Footsteps")] static extern void setFloatValue(float channel, int index, float value);
#endif

#if UNITY_IOS
      [DllImport("__Internal")]
#else
    [DllImport("audioplugin_Footsteps")] static extern float getFloatValue(int index);
#endif

#if UNITY_IOS
      [DllImport("__Internal")]
#else
    [DllImport("audioplugin_Footsteps")] static extern float setPace (float channel, float value);
#endif

#if UNITY_IOS
      [DllImport("__Internal")]
#else
    [DllImport("audioplugin_Footsteps")] static extern float setFirmness (float channel, float value);
#endif

#if UNITY_IOS
      [DllImport("__Internal")]
#else
    [DllImport("audioplugin_Footsteps")] static extern float setSteadiness (float channel, float value);
#endif

#if UNITY_IOS
      [DllImport("__Internal")]
#else
    [DllImport("audioplugin_Footsteps")] static extern float setShoeType (float channel, float value);
#endif

#if UNITY_IOS
      [DllImport("__Internal")]
#else
    [DllImport("audioplugin_Footsteps")] static extern float setSurfaceType (float channel, float value);
#endif

#if UNITY_IOS
      [DllImport("__Internal")]
#else
    [DllImport("audioplugin_Footsteps")] static extern float seTerrainType (float channel, float value);
#endif

#if UNITY_IOS
      [DllImport("__Internal")]
#else
    [DllImport("audioplugin_Footsteps")] static extern float setVolume (float channel, float value);
#endif

#if UNITY_IOS
      [DllImport("__Internal")]
#else
    [DllImport("audioplugin_Footsteps")] static extern float setStart(float channel);
#endif

#if UNITY_IOS
      [DllImport("__Internal")]
#else
    [DllImport("audioplugin_Footsteps")] static extern float setStop(float channel);
#endif

#if UNITY_IOS
      [DllImport("__Internal")]
#else
    [DllImport("audioplugin_Footsteps")] static extern float setTrigger(float channel);
#endif

#if UNITY_IOS
      [DllImport("__Internal")]
#else
    [DllImport("audioplugin_Footsteps")] static extern float setHeelGain(float channel, float value);
#endif

#if UNITY_IOS
      [DllImport("__Internal")]
#else
    [DllImport("audioplugin_Footsteps")] static extern float setHeelAttack(float channel, float value);
#endif

#if UNITY_IOS
      [DllImport("__Internal")]
#else
    [DllImport("audioplugin_Footsteps")] static extern float setHeelDecay(float channel, float value);
#endif

#if UNITY_IOS
      [DllImport("__Internal")]
#else
    [DllImport("audioplugin_Footsteps")] static extern float setHeelSustain(float channel, float value);
#endif

#if UNITY_IOS
      [DllImport("__Internal")]
#else
    [DllImport("audioplugin_Footsteps")] static extern float setHeelRelease(float channel, float value);
#endif

#if UNITY_IOS
      [DllImport("__Internal")]
#else
    [DllImport("audioplugin_Footsteps")] static extern float setBallGain(float channel, float value);
#endif

#if UNITY_IOS
      [DllImport("__Internal")]
#else
    [DllImport("audioplugin_Footsteps")] static extern float setBallAttack(float channel, float value);
#endif

#if UNITY_IOS
      [DllImport("__Internal")]
#else
    [DllImport("audioplugin_Footsteps")] static extern float setBallDecay(float channel, float value);
#endif

#if UNITY_IOS
      [DllImport("__Internal")]
#else
    [DllImport("audioplugin_Footsteps")] static extern float setBallSustain(float channel, float value);
#endif

#if UNITY_IOS
      [DllImport("__Internal")]
#else
    [DllImport("audioplugin_Footsteps")] static extern float setBallRelease(float channel, float value);
#endif

#if UNITY_IOS
      [DllImport("__Internal")]
#else
    [DllImport("audioplugin_Footsteps")] static extern float setRollGain(float channel, float value);
#endif

    public float flag1 = 0;
    public float flag2 = 0;
    // Start is called before the first frame update
    void Start()
    {
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                //Select stage    
                if (hit.transform.name == "Trigger")
                {
                    if (flag1==0 || flag1 % 2==0)
                    {
                        setStart(0);
                    }
                    if (flag1 == 1 || flag1 % 2==1)
                    {
                        setStop(0);
                    }
                    flag1++;
                }
                if (hit.transform.name == "Trigger 2")
                {
                    if (flag2 == 0 || flag2 % 2 == 0)
                    {
                        setStart(1);
                    }
                    if (flag2 == 1 || flag2 % 2 == 1)
                    {
                        setStop(1);
                    }
                    flag2++;
                }
            }
        }

    }
}