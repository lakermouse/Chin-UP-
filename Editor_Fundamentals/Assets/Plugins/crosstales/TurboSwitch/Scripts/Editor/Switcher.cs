#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using Crosstales.TPS.Util;

namespace Crosstales.TPS
{
   /// <summary>Platform switcher.</summary>
   public static class Switcher
   {
      #region Variables

      private static bool hasConfigChanged;
      private static string origExecuteMethod;
      private static bool origBatchmode;
      private static bool origQuit;
      private static bool origNoGraphics;
      private static bool origCopySettings;

      #endregion

      #region Properties

      /// <summary>The current switch target.</summary>
      public static BuildTarget CurrentSwitchTarget { get; private set; } = BuildTarget.NoTarget;

      /// <summary>True if the Switcher is busy.</summary>
      public static bool isBusy { get; private set; }

      #endregion


      #region Events

      public delegate void SwitchStart(BuildTarget target, MobileTextureSubtarget subTarget);

      public delegate void SwitchComplete(BuildTarget target, MobileTextureSubtarget subTarget, bool success);

      /// <summary>An event triggered whenever a switch is started.</summary>
      public static event SwitchStart OnSwitchStart;

      /// <summary>An event triggered whenever a switch is completed.</summary>
      public static event SwitchComplete OnSwitchComplete;

      #endregion


      #region Public methods

      /// <summary>Switches the current platform to the target via CLI.</summary>
      public static void SwitchCLI()
      {
         Switch(Helper.GetArgument("-tpsBuild"), Helper.GetArgument("-tpsExecuteMethod"), "true".CTEquals(Helper.GetArgument("-tpsBatchmode")), !"false".CTEquals(Helper.GetArgument("-tpsQuit")), "true".CTEquals(Helper.GetArgument("-tpsNoGraphics")), "true".CTEquals(Helper.GetArgument("-tpsCopySettings")));
      }

      /// <summary>Switches the current platform to the target.</summary>
      /// <param name="build">Build type name for Unity, like 'win64'</param>
      /// <param name="executeMethod">Execute method after switch (optional)</param>
      /// <param name="batchmode">Start Unity in batch-mode (default: false, optional)</param>
      /// <param name="quit">Quit Unity in batch-mode (default: true, optional)</param>
      /// <param name="noGraphics">Disable graphic devices in batch-mode (default: false, optional)</param>
      /// <param name="copySettings">Copy the project settings (default: false, optional)</param>
      /// <returns>True if the switch was successful.</returns>
      public static bool Switch(string build, string executeMethod = "", bool batchmode = false, bool quit = true, bool noGraphics = false, bool copySettings = false)
      {
         origExecuteMethod = Config.EXECUTE_METHOD;
         origBatchmode = Config.BATCHMODE;
         origQuit = Config.QUIT;
         origNoGraphics = Config.NO_GRAPHICS;
         origCopySettings = Config.COPY_SETTINGS;

         Config.EXECUTE_METHOD = executeMethod;
         Config.BATCHMODE = batchmode;
         Config.QUIT = quit;
         Config.NO_GRAPHICS = noGraphics;
         Config.COPY_SETTINGS = copySettings;
         hasConfigChanged = true;

         return Switch(Helper.GetBuildTargetForBuildName(build));
      }

      /// <summary>Switches the current platform to the target.</summary>
      /// <param name="target">Target platform for the switch</param>
      /// <param name="subTarget">Texture format (Android, optional)</param>
      /// <returns>True if the switch was successful.</returns>
      public static bool Switch(BuildTarget target, MobileTextureSubtarget subTarget = MobileTextureSubtarget.Generic)
      {
         isBusy = true;

         OnSwitchStart?.Invoke(target, subTarget);

         CurrentSwitchTarget = target;
         bool success = false;

         if (target == EditorUserBuildSettings.activeBuildTarget) //ignore switch
         {
            Debug.LogWarning("Target platform is equals the current platform - switch ignored.");

            if (!string.IsNullOrEmpty(Config.EXECUTE_METHOD_PRE_SWITCH))
               Crosstales.Common.EditorUtil.BaseEditorHelper.InvokeMethod(Config.EXECUTE_METHOD_PRE_SWITCH.Substring(0, Config.EXECUTE_METHOD_PRE_SWITCH.CTLastIndexOf(".")), Config.EXECUTE_METHOD_PRE_SWITCH.Substring(Config.EXECUTE_METHOD_PRE_SWITCH.CTLastIndexOf(".") + 1));

            if (!string.IsNullOrEmpty(Config.EXECUTE_METHOD))
               Crosstales.Common.EditorUtil.BaseEditorHelper.InvokeMethod(Config.EXECUTE_METHOD.Substring(0, Config.EXECUTE_METHOD.CTLastIndexOf(".")), Config.EXECUTE_METHOD.Substring(Config.EXECUTE_METHOD.CTLastIndexOf(".") + 1));

            success = true;
         }
         else
         {
            success = Config.USE_LEGACY ? Helper.SwitchPlatform(target, subTarget) : Helper.SwitchPlatformNew(target, subTarget);
         }

         CurrentSwitchTarget = BuildTarget.NoTarget;

         ResetConfig();

         OnSwitchComplete?.Invoke(target, subTarget, success);

         isBusy = false;

         if (Application.isBatchMode && !Config.USE_LEGACY && Config.QUIT)
            EditorApplication.Exit(0);

         return success;
      }

      /// <summary>Reset any configuration changes.</summary>
      public static void ResetConfig()
      {
         if (hasConfigChanged)
         {
            Config.EXECUTE_METHOD = origExecuteMethod;
            Config.BATCHMODE = origBatchmode;
            Config.QUIT = origQuit;
            Config.NO_GRAPHICS = origNoGraphics;
            Config.COPY_SETTINGS = origCopySettings;

            hasConfigChanged = false;

            Config.Save();
         }
      }

      /// <summary>Test switching with an execute method.</summary>
      public static void SayHello()
      {
         Debug.LogWarning("Hello everybody, 'SayHello' was called!");

         if (Config.DEBUG)
            Debug.Log("CurrentSwitchTarget: " + CurrentSwitchTarget);
      }

      /// <summary>Test method (before switching).</summary>
      public static void MethodBeforeSwitch()
      {
         Debug.LogWarning("'MethodBeforeSwitch' was called!");
      }

      /// <summary>Test method (after switching).</summary>
      public static void MethodAfterSwitch()
      {
         Debug.LogWarning("'MethodAfterSwitch' was called");
      }

      /// <summary>Default method after switching.</summary>
      public static void DefaultMethodAfterSwitch()
      {
         //Debug.LogWarning("'DefaultMethodAfterSwitch' was called");
         OnSwitchComplete?.Invoke(EditorUserBuildSettings.activeBuildTarget, MobileTextureSubtarget.Generic, true);
      }

      #endregion
   }
}
#endif
// © 2018-2022 crosstales LLC (https://www.crosstales.com)