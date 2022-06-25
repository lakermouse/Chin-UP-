#if false || CT_DEVELOP
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Crosstales.TPS.Example
{
   /// <summary>Simple test script for all callbacks.</summary>
   [InitializeOnLoad]
   public abstract class EventTester
   {
      #region Constructor

      static EventTester()
      {
         Switcher.OnSwitchStart += onSwitchStart;
         Switcher.OnSwitchComplete += onSwitchComplete;
      }

      private static void onSwitchStart(BuildTarget target, MobileTextureSubtarget subtarget)
      {
         Debug.Log("Switch start: " + target + " - " + subtarget);
      }

      private static void onSwitchComplete(BuildTarget target, MobileTextureSubtarget subtarget, bool success)
      {
         Debug.Log("Switch complete: " + target + " - " + subtarget + " - " + success);
      }

      #endregion
   }
}
#endif
#endif
// © 2020-2022 crosstales LLC (https://www.crosstales.com)