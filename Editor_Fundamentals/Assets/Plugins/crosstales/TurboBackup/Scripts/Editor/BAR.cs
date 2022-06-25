#if UNITY_EDITOR
using UnityEngine;
using Crosstales.TB.Util;

namespace Crosstales.TB
{
   /// <summary>Backup and restore methods.</summary>
   public static class BAR
   {
      #region Properties

      /// <summary>True if the BAR is busy.</summary>
      public static bool isBusy { get; private set; }

      #endregion


      #region Events

      public delegate void BackupStart();

      public delegate void BackupComplete(bool success);

      public delegate void RestoreStart();

      public delegate void RestoreComplete(bool success);

      /// <summary>An event triggered whenever the backup is started.</summary>
      public static event BackupStart OnBackupStart;

      /// <summary>An event triggered whenever the backup is completed.</summary>
      public static event BackupComplete OnBackupComplete;

      /// <summary>An event triggered whenever the restore is started.</summary>
      public static event RestoreStart OnRestoreStart;

      /// <summary>An event triggered whenever the restore is completed.</summary>
      public static event RestoreComplete OnRestoreComplete;

      #endregion


      #region Public methods

      /// <summary>Backup the current project via CLI.</summary>
      public static void BackupCLI()
      {
         Backup(Helper.GetArgument("-tbExecuteMethod"), "true".CTEquals(Helper.GetArgument("-tbBatchmode")), !"false".CTEquals(Helper.GetArgument("-tbQuit")), "true".CTEquals(Helper.GetArgument("-tbNoGraphics")), "true".CTEquals(Helper.GetArgument("-tbCopyAssets")), "true".CTEquals(Helper.GetArgument("-tbCopyLibrary")), "true".CTEquals(Helper.GetArgument("-tbCopySettings")), "true".CTEquals(Helper.GetArgument("-tbCopyPackages")), Helper.GetArgument("-tbPath"), "true".CTEquals(Helper.GetArgument("-tbCopyUserSettings")));
      }

      /// <summary>Restore the current project via CLI.</summary>
      public static void RestoreCLI()
      {
         Restore(Helper.GetArgument("-tbExecuteMethod"), "true".CTEquals(Helper.GetArgument("-tbBatchmode")), !"false".CTEquals(Helper.GetArgument("-tbQuit")), "true".CTEquals(Helper.GetArgument("-tbNoGraphics")), "true".CTEquals(Helper.GetArgument("-tbCopyAssets")), "true".CTEquals(Helper.GetArgument("-tbCopyLibrary")), "true".CTEquals(Helper.GetArgument("-tbCopySettings")), "true".CTEquals(Helper.GetArgument("-tbCopyPackages")), Helper.GetArgument("-tbPath"), "true".CTEquals(Helper.GetArgument("-tbCopyUserSettings")));
      }

      /// <summary>Backup the current project.</summary>
      /// <param name="executeMethod">Execute method after backup</param>
      /// <param name="batchmode">Start Unity in batch-mode (default: false, optional)</param>
      /// <param name="quit">Quit Unity in batch-mode (default: true, optional)</param>
      /// <param name="noGraphics">Disable graphic devices in batch-mode (default: false, optional)</param>
      /// <param name="backupAssets">Copy the 'Assets'-folder (default: true, optional)</param>
      /// <param name="backupLibrary">Copy the 'Library'-folder (default: false, optional)</param>
      /// <param name="backupSettings">Copy the 'ProjectSettings"-folder (default: true, optional)</param>
      /// <param name="backupPackages">Copy the 'Packages"-folder (default: true, optional)</param>
      /// <param name="backupPath">Set the backup path (default: "", optional)</param>
      /// <param name="backupUserSettings">Copy the 'UserSettings"-folder (default: true, optional)</param>
      /// <returns>True if the backup was successful.</returns>
      public static bool Backup(string executeMethod, bool batchmode = false, bool quit = true, bool noGraphics = false, bool backupAssets = true, bool backupLibrary = false, bool backupSettings = true, bool backupPackages = true, string backupPath = "", bool backupUserSettings = true)
      {
         Config.EXECUTE_METHOD_BACKUP = executeMethod;
         Config.BATCHMODE = batchmode;
         Config.QUIT = quit;
         Config.NO_GRAPHICS = noGraphics;
         Config.COPY_ASSETS = backupAssets;
         Config.COPY_LIBRARY = backupLibrary;
         Config.COPY_SETTINGS = backupSettings;
         Config.COPY_PACKAGES = backupPackages;
         Config.COPY_USER_SETTINGS = backupUserSettings;

         if (!string.IsNullOrEmpty(backupPath))
            Config.PATH_BACKUP = backupPath;

         return Backup();
      }

      /// <summary>Backup the current project.</summary>
      /// <returns>True if the backup was successful.</returns>
      public static bool Backup()
      {
         isBusy = true;

         OnBackupStart?.Invoke();

         bool success = false;

         if (Config.COPY_ASSETS || Config.COPY_LIBRARY || Config.COPY_SETTINGS || Config.COPY_PACKAGES)
         {
            success = Config.USE_LEGACY ? Helper.Backup() : Helper.BackupNew();
         }
         else
         {
            Debug.LogError("No folders selected - backup not possible!");
#if UNITY_2018_2_OR_NEWER
            if (Application.isBatchMode)
#else
            if (UnityEditorInternal.InternalEditorUtility.inBatchMode)
#endif
               throw new System.Exception("No folders selected - backup not possible!");
            //EditorApplication.Exit(0);
         }

         OnBackupComplete?.Invoke(success);

         isBusy = false;

         return success;
      }

      /// <summary>Restore the current project.</summary>
      /// <param name="executeMethod">Execute method after restore</param>
      /// <param name="batchmode">Start Unity in batch-mode (default: false, optional)</param>
      /// <param name="quit">Quit Unity in batch-mode (default: true, optional)</param>
      /// <param name="noGraphics">Disable graphic devices in batch-mode (default: false, optional)</param>
      /// <param name="restoreAssets">Restore the 'Assets'-folder (default: true, optional)</param>
      /// <param name="restoreLibrary">Restore the 'Library'-folder (default: false, optional)</param>
      /// <param name="restoreSettings">Restore the 'ProjectSettings"-folder (default: true, optional)</param>
      /// <param name="restorePackages">Restore the 'Packages"-folder (default: true, optional)</param>
      /// <param name="backupPath">Set the backup path (default: "", optional)</param>
      /// <param name="backupUserSettings">Copy the 'UserSettings"-folder (default: true, optional)</param>
      /// <returns>True if the restore was successful.</returns>
      public static bool Restore(string executeMethod, bool batchmode = false, bool quit = true, bool noGraphics = false, bool restoreAssets = true, bool restoreLibrary = false, bool restoreSettings = true, bool restorePackages = true, string backupPath = "", bool backupUserSettings = true)
      {
         Config.EXECUTE_METHOD_RESTORE = executeMethod;
         Config.BATCHMODE = batchmode;
         Config.QUIT = quit;
         Config.NO_GRAPHICS = noGraphics;
         Config.COPY_ASSETS = restoreAssets;
         Config.COPY_LIBRARY = restoreLibrary;
         Config.COPY_SETTINGS = restoreSettings;
         Config.COPY_PACKAGES = restorePackages;
         Config.COPY_USER_SETTINGS = backupUserSettings;

         if (!string.IsNullOrEmpty(backupPath))
            Config.PATH_BACKUP = backupPath;

         return Restore();
      }

      /// <summary>Restore the current project.</summary>
      /// <returns>True if the restore was successful.</returns>
      public static bool Restore()
      {
         isBusy = true;

         OnRestoreStart?.Invoke();

         bool success = false;

         if (Config.COPY_ASSETS || Config.COPY_LIBRARY || Config.COPY_SETTINGS || Config.COPY_PACKAGES)
         {
            success = Config.USE_LEGACY ? Helper.Restore() : Helper.RestoreNew();
         }
         else
         {
            Debug.LogError("No folders selected - restore not possible!");
#if UNITY_2018_2_OR_NEWER
            if (Application.isBatchMode)
#else
            if (UnityEditorInternal.InternalEditorUtility.inBatchMode)
#endif
               throw new System.Exception("No folders selected - restore not possible!");
            //EditorApplication.Exit(0);
         }

         OnRestoreComplete?.Invoke(success);

         isBusy = false;

         return success;
      }

      /// <summary>Test the backup/restore with an execute method.</summary>
      public static void SayHello()
      {
         Debug.LogError("Hello everybody, I was called by " + Constants.ASSET_NAME);
      }

      /// <summary>Test method (before backup).</summary>
      public static void MethodBeforeBackup()
      {
         Debug.LogWarning("'MethodBeforeBackup' was called!");
      }

      /// <summary>Test method (after backup).</summary>
      public static void MethodAfterBackup()
      {
         Debug.LogWarning("'MethodAfterBackup' was called");
      }

      /// <summary>Test method (before restore).</summary>
      public static void MethodBeforeRestore()
      {
         Debug.LogWarning("'MethodBeforeRestore' was called!");
      }

      /// <summary>Test method (after restore).</summary>
      public static void MethodAfterRestore()
      {
         Debug.LogWarning("'MethodAfterRestore' was called");
      }

      /// <summary>Default method after backup.</summary>
      public static void DefaultMethodAfterBackup()
      {
         //Debug.LogWarning("'DefaultMethodAfterBackup' was called");
         OnBackupComplete?.Invoke(true);
      }

      /// <summary>Default method after restore.</summary>
      public static void DefaultMethodAfterRestore()
      {
         //Debug.LogWarning("'DefaultMethodAfterRestore' was called");
         OnRestoreComplete?.Invoke(true);
      }

      #endregion
   }
}
#endif
// © 2018-2022 crosstales LLC (https://www.crosstales.com)