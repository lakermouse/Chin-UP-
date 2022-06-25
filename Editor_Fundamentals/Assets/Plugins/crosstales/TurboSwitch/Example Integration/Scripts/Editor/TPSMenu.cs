#if false || CT_DEVELOP
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Crosstales.TPS.Example
{
   /// <summary>Example editor integration of Turbo Switch for your own scripts.</summary>
   public static class TPSMenu
   {
      [MenuItem("Tools/Switch to Windows #&w")]
      public static void SwitchWindows()
      {
         Debug.Log("Switch to Windows");

         if (!Switcher.Switch(BuildTarget.StandaloneWindows64))
            Debug.LogError("Could not switch to the 'StandaloneWindows64' platform!");
      }

      [MenuItem("Tools/Switch to Android #&m")]
      public static void SwitchAndroid()
      {
         Debug.Log("Switch to Android");

         if (!Switcher.Switch(BuildTarget.Android))
            Debug.LogError("Could not switch to the 'Android' platform!");
      }
   }
}
#endif
#endif
// © 2019-2022 crosstales LLC (https://www.crosstales.com)