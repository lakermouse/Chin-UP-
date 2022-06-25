#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using Crosstales.TPS.Util;

namespace Crosstales.TPS.Task
{
   /// <summary>Disables the Unity cache server.</summary>
   [InitializeOnLoad]
   public static class DisableUnityCacheServer
   {
      #region Constructor

      static DisableUnityCacheServer()
      {
         if (EditorPrefs.GetInt("CacheServerMode") != 2)
         {
            EditorPrefs.SetInt("CacheServerMode", 2);

            Debug.LogWarning(Helper.CreateString("-", 400));
            Debug.LogWarning("<b>+++ 'Unity Cache Server' has been disabled for <color=blue>" + Constants.ASSET_NAME + "</color>! +++</b>");
            Debug.LogWarning(Helper.CreateString("-", 400));
         }
      }

      #endregion
   }
}
#endif
// © 2018-2022 crosstales LLC (https://www.crosstales.com)