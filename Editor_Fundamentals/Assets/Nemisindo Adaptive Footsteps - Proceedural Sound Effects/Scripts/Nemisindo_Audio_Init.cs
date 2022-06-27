using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Nemisindo
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(AudioSource))]
    [AddComponentMenu("Nemisindo/Audio Init")]
    public class Nemisindo_Audio_Init : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            AudioClip one = AudioClip.Create("Nemisindo_Audio", 1, 1, AudioSettings.outputSampleRate, false);
            one.SetData(new float[] { 0.0001f }, 0);
            GetComponent<AudioSource>().clip = one;
            GetComponent<AudioSource>().loop = true;
            if (Application.isPlaying)
                GetComponent<AudioSource>().Play();
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}

