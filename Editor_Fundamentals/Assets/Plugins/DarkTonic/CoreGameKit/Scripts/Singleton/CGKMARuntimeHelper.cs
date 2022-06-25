#if MASTERAUDIO_ENABLED
using UnityEngine;

namespace DarkTonic.CoreGameKit
{
    public static class CGKMARuntimeHelper
    {
        public static void PlaySoundFollowTransform(string soundGroup, Transform transform)
        {
            if (string.IsNullOrEmpty(soundGroup) || soundGroup == MasterAudio.MasterAudio.NoGroupName)
            {
                return;
            }

            if (MasterAudio.MasterAudio.SafeInstance != null && !MasterAudio.MasterAudio.SoundsReady)
            {
                return;
            }

            MasterAudio.MasterAudio.PlaySound3DFollowTransform(soundGroup, transform);
        }
    }
}
#endif