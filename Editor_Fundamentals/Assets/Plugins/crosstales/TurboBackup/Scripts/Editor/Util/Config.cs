#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using Crosstales.Common.Util;

namespace Crosstales.TB.Util
{
   /// <summary>Configuration for the asset.</summary>
   [InitializeOnLoad]
   public static class Config
   {
      #region Variables

      /// <summary>Enable or disable custom location for the backup.</summary>
      public static bool CUSTOM_PATH_BACKUP = Constants.DEFAULT_CUSTOM_PATH_BACKUP;

      /// <summary>Backup path.</summary>
      private static string pathBackup = Constants.DEFAULT_PATH_CACHE;

      public static string PATH_BACKUP
      {
         get => CUSTOM_PATH_BACKUP && !string.IsNullOrEmpty(pathBackup) ? FileHelper.ValidatePath(pathBackup) : Constants.DEFAULT_PATH_CACHE;
         set
         {
            if (CUSTOM_PATH_BACKUP)
            {
               string path = value.Substring(0, value.Length - Constants.BACKUP_DIRNAME.Length - 1);

               if (path.Contains("Library"))
               {
                  Debug.LogError("Backup path can't be inside a path containing 'Library': " + value);
               }
               else if (path.Contains("Assets"))
               {
                  Debug.LogError("Backup path can't be inside a path containing 'Assets': " + value);
               }
               else if (path.Contains("ProjectSettings"))
               {
                  Debug.LogError("Cache path can't be inside a path containing 'ProjectSettings': " + value);
               }
               else
               {
                  pathBackup = value;
               }
            }
            else
            {
               pathBackup = value;
            }
         }
      }

      /// <summary>Selected VCS-system (default: 0, 0 = none, 1 = git, 2 = SVN, 3 Mercurial, 4 = Collab, 5 = PlasticSCM).</summary>
      public static int VCS = Constants.DEFAULT_VCS;

      /// <summary>Uses the legacy switch function.</summary>
      public static bool USE_LEGACY = Constants.DEFAULT_USE_LEGACY;

      /// <summary>Enable or disable batch mode for CLI operations.</summary>
      public static bool BATCHMODE = Constants.DEFAULT_BATCHMODE;

      /// <summary>Enable or disable quit Unity Editor for CLI operations.</summary>
      public static bool QUIT = Constants.DEFAULT_QUIT;

      /// <summary>Enable or disable graphics device in Unity Editor for CLI operations.</summary>
      public static bool NO_GRAPHICS = Constants.DEFAULT_NO_GRAPHICS;

      /// <summary>Execute static method 'ClassName.MethodName' in Unity before a backup.</summary>
      public static string EXECUTE_METHOD_PRE_BACKUP = string.Empty;

      /// <summary>Execute static method 'ClassName.MethodName' in Unity after a backup.</summary>
      public static string EXECUTE_METHOD_BACKUP = string.Empty;

      /// <summary>Execute static method 'ClassName.MethodName' in Unity before a restore.</summary>
      public static string EXECUTE_METHOD_PRE_RESTORE = string.Empty;

      /// <summary>Execute static method 'ClassName.MethodName' in Unity after a restore.</summary>
      public static string EXECUTE_METHOD_RESTORE = string.Empty;

      /// <summary>Enable or disable deleting the 'UnityLockfile'.</summary>
      public static bool DELETE_LOCKFILE = Constants.DEFAULT_DELETE_LOCKFILE;

      /// <summary>Enable or disable copying the 'Assets'-folder.</summary>
      public static bool COPY_ASSETS = Constants.DEFAULT_COPY_ASSETS;

      /// <summary>Enable or disable copying the 'Library'-folder.</summary>
      public static bool COPY_LIBRARY = Constants.DEFAULT_COPY_LIBRARY;

      /// <summary>Enable or disable copying the 'ProjectSettings'-folder.</summary>
      public static bool COPY_SETTINGS = Constants.DEFAULT_COPY_SETTINGS;

      /// <summary>Enable or disable copying the 'UserSettings'-folder.</summary>
      public static bool COPY_USER_SETTINGS = Constants.DEFAULT_COPY_USER_SETTINGS;

      /// <summary>Enable or disable copying the 'Packages'-folder.</summary>
      public static bool COPY_PACKAGES = Constants.DEFAULT_COPY_PACKAGES;

      /// <summary>Enable or disable the backup confirmation dialog.</summary>
      public static bool CONFIRM_BACKUP = Constants.DEFAULT_CONFIRM_BACKUP;

      /// <summary>Enable or disable the restore confirmation dialog.</summary>
      public static bool CONFIRM_RESTORE = Constants.DEFAULT_CONFIRM_RESTORE;

      /// <summary>Enable or disable the restore warning confirmation dialog.</summary>
      public static bool CONFIRM_WARNING = Constants.DEFAULT_CONFIRM_WARNING;

      /// <summary>Enable or disable debug logging for the asset.</summary>
      public static bool DEBUG = Constants.DEFAULT_DEBUG;

      /// <summary>Enable or disable update-checks for the asset.</summary>
      public static bool UPDATE_CHECK = Constants.DEFAULT_UPDATE_CHECK;

      /// <summary>Enable or disable adding compile define "CT_TB" for the asset.</summary>
      public static bool COMPILE_DEFINES = Constants.DEFAULT_COMPILE_DEFINES;

      /// <summary>Backup date.</summary>
      public static System.DateTime BACKUP_DATE
      {
         get
         {
            if (backupDate.Year < 2020)
            {
               try
               {
                  if (System.IO.File.Exists(backupFile))
                  {
                     System.DateTime.TryParseExact(System.IO.File.ReadAllText(backupFile).Trim(), "yyyyMMddHHmmsss", null, System.Globalization.DateTimeStyles.None, out System.DateTime result);

                     backupDate = result;
                  }
               }
               catch (System.Exception ex)
               {
                  Debug.LogWarning($"Could not read backup date: {ex}");
               }
            }

            return backupDate;
         }

         set
         {
            try
            {
               System.IO.Directory.CreateDirectory(PATH_BACKUP);

               System.IO.File.WriteAllText(backupFile, value.ToString("yyyyMMddHHmmsss"));

               backupDate = value;
            }
            catch (System.Exception ex)
            {
               Debug.LogWarning($"Could not write backup date: {ex}");
            }
         }
      }

      /// <summary>Backup counter.</summary>
      public static int BACKUP_COUNT;

      /// <summary>Restore date.</summary>
      public static System.DateTime RESTORE_DATE;

      /// <summary>Restore counter.</summary>
      public static int RESTORE_COUNT;

      /// <summary>Last setup date.</summary>
      public static System.DateTime SETUP_DATE;

      /// <summary>Enable or disable automatic saving of all scenes.</summary>
      public static bool AUTO_SAVE = Constants.DEFAULT_AUTO_SAVE;

      /// <summary>Auto backup date.</summary>
      public static System.DateTime AUTO_BACKUP_DATE;

      /// <summary>Auto backup interval.</summary>
      public static int AUTO_BACKUP_INTERVAL;

      /// <summary>Is the configuration loaded?</summary>
      public static bool isLoaded;

      private static string assetPath;
      private const string idPath = "Documentation/id/";
      private static readonly string idName = Constants.ASSET_UID + ".txt";

      private static System.DateTime backupDate;

      #endregion


      #region Constructor

      static Config()
      {
         if (!isLoaded)
         {
            Load();

            if (DEBUG)
               Debug.Log("Config data loaded");
         }
      }

      #endregion


      #region Properties

      /// <summary>Returns the path to the asset inside the Unity project.</summary>
      /// <returns>The path to the asset inside the Unity project.</returns>
      public static string ASSET_PATH
      {
         get
         {
            if (assetPath == null)
            {
               try
               {
                  if (System.IO.File.Exists(Application.dataPath + Constants.DEFAULT_ASSET_PATH + idPath + idName))
                  {
                     assetPath = Constants.DEFAULT_ASSET_PATH;
                  }
                  else
                  {
                     string[] files = System.IO.Directory.GetFiles(Application.dataPath, idName, System.IO.SearchOption.AllDirectories);

                     if (files.Length > 0)
                     {
                        string name = files[0].Substring(Application.dataPath.Length);
                        assetPath = name.Substring(0, name.Length - idPath.Length - idName.Length).Replace("\\", "/");
                     }
                     else
                     {
                        Debug.LogWarning("Could not locate the asset! File not found: " + idName);
                        assetPath = Constants.DEFAULT_ASSET_PATH;
                     }
                  }
               }
               catch (System.Exception ex)
               {
                  Debug.LogWarning("Could not locate asset: " + ex);
               }
            }

            return assetPath;
         }
      }

      private static string backupFile => $"{PATH_BACKUP}/Backup.dat";

      #endregion


      #region Public static methods

      /// <summary>Resets all changeable variables to their default value.</summary>
      public static void Reset()
      {
         assetPath = null;

         CUSTOM_PATH_BACKUP = Constants.DEFAULT_CUSTOM_PATH_BACKUP;
         pathBackup = Constants.DEFAULT_PATH_CACHE;
         VCS = Constants.DEFAULT_VCS;
         USE_LEGACY = Constants.DEFAULT_USE_LEGACY;
         BATCHMODE = Constants.DEFAULT_BATCHMODE;
         QUIT = Constants.DEFAULT_QUIT;
         NO_GRAPHICS = Constants.DEFAULT_NO_GRAPHICS;
         EXECUTE_METHOD_PRE_BACKUP = string.Empty;
         EXECUTE_METHOD_BACKUP = string.Empty;
         EXECUTE_METHOD_PRE_RESTORE = string.Empty;
         EXECUTE_METHOD_RESTORE = string.Empty;
         DELETE_LOCKFILE = Constants.DEFAULT_DELETE_LOCKFILE;
         COPY_ASSETS = Constants.DEFAULT_COPY_ASSETS;
         COPY_LIBRARY = Constants.DEFAULT_COPY_LIBRARY;
         COPY_SETTINGS = Constants.DEFAULT_COPY_SETTINGS;
         COPY_USER_SETTINGS = Constants.DEFAULT_COPY_USER_SETTINGS;
         COPY_PACKAGES = Constants.DEFAULT_COPY_PACKAGES;
         CONFIRM_BACKUP = Constants.DEFAULT_CONFIRM_BACKUP;
         CONFIRM_RESTORE = Constants.DEFAULT_CONFIRM_RESTORE;
         CONFIRM_WARNING = Constants.DEFAULT_CONFIRM_WARNING;

         if (!Constants.DEV_DEBUG)
            DEBUG = Constants.DEFAULT_DEBUG;

         UPDATE_CHECK = Constants.DEFAULT_UPDATE_CHECK;
         COMPILE_DEFINES = Constants.DEFAULT_COMPILE_DEFINES;
         AUTO_SAVE = Constants.DEFAULT_AUTO_SAVE;
         AUTO_BACKUP_INTERVAL = 0;
      }

      /// <summary>Loads the all changeable variables.</summary>
      public static void Load()
      {
         assetPath = null;

         if (CTPlayerPrefs.HasKey(Constants.KEY_CUSTOM_PATH_CACHE))
            CUSTOM_PATH_BACKUP = CTPlayerPrefs.GetBool(Constants.KEY_CUSTOM_PATH_CACHE);

         if (CTPlayerPrefs.HasKey(Constants.KEY_PATH_CACHE))
            PATH_BACKUP = CTPlayerPrefs.GetString(Constants.KEY_PATH_CACHE);

         if (CTPlayerPrefs.HasKey(Constants.KEY_VCS))
            VCS = CTPlayerPrefs.GetInt(Constants.KEY_VCS);

         if (CTPlayerPrefs.HasKey(Constants.KEY_USE_LEGACY))
            USE_LEGACY = CTPlayerPrefs.GetBool(Constants.KEY_USE_LEGACY);

         if (CTPlayerPrefs.HasKey(Constants.KEY_BATCHMODE))
            BATCHMODE = CTPlayerPrefs.GetBool(Constants.KEY_BATCHMODE);

         if (CTPlayerPrefs.HasKey(Constants.KEY_QUIT))
            QUIT = CTPlayerPrefs.GetBool(Constants.KEY_QUIT);

         if (CTPlayerPrefs.HasKey(Constants.KEY_NO_GRAPHICS))
            NO_GRAPHICS = CTPlayerPrefs.GetBool(Constants.KEY_NO_GRAPHICS);

         if (CTPlayerPrefs.HasKey(Constants.KEY_EXECUTE_METHOD_PRE_BACKUP))
            EXECUTE_METHOD_PRE_BACKUP = CTPlayerPrefs.GetString(Constants.KEY_EXECUTE_METHOD_PRE_BACKUP);

         if (CTPlayerPrefs.HasKey(Constants.KEY_EXECUTE_METHOD_BACKUP))
            EXECUTE_METHOD_BACKUP = CTPlayerPrefs.GetString(Constants.KEY_EXECUTE_METHOD_BACKUP);

         if (CTPlayerPrefs.HasKey(Constants.KEY_EXECUTE_METHOD_PRE_RESTORE))
            EXECUTE_METHOD_PRE_RESTORE = CTPlayerPrefs.GetString(Constants.KEY_EXECUTE_METHOD_PRE_RESTORE);

         if (CTPlayerPrefs.HasKey(Constants.KEY_EXECUTE_METHOD_RESTORE))
            EXECUTE_METHOD_RESTORE = CTPlayerPrefs.GetString(Constants.KEY_EXECUTE_METHOD_RESTORE);

         if (CTPlayerPrefs.HasKey(Constants.KEY_DELETE_LOCKFILE))
            DELETE_LOCKFILE = CTPlayerPrefs.GetBool(Constants.KEY_DELETE_LOCKFILE);

         if (CTPlayerPrefs.HasKey(Constants.KEY_COPY_ASSETS))
            COPY_ASSETS = CTPlayerPrefs.GetBool(Constants.KEY_COPY_ASSETS);

         if (CTPlayerPrefs.HasKey(Constants.KEY_COPY_LIBRARY))
            COPY_LIBRARY = CTPlayerPrefs.GetBool(Constants.KEY_COPY_LIBRARY);

         if (CTPlayerPrefs.HasKey(Constants.KEY_COPY_SETTINGS))
            COPY_SETTINGS = CTPlayerPrefs.GetBool(Constants.KEY_COPY_SETTINGS);

         if (CTPlayerPrefs.HasKey(Constants.KEY_COPY_USER_SETTINGS))
            COPY_USER_SETTINGS = CTPlayerPrefs.GetBool(Constants.KEY_COPY_USER_SETTINGS);

         if (CTPlayerPrefs.HasKey(Constants.KEY_COPY_PACKAGES))
            COPY_PACKAGES = CTPlayerPrefs.GetBool(Constants.KEY_COPY_PACKAGES);

         if (CTPlayerPrefs.HasKey(Constants.KEY_CONFIRM_BACKUP))
            CONFIRM_BACKUP = CTPlayerPrefs.GetBool(Constants.KEY_CONFIRM_BACKUP);

         if (CTPlayerPrefs.HasKey(Constants.KEY_CONFIRM_RESTORE))
            CONFIRM_RESTORE = CTPlayerPrefs.GetBool(Constants.KEY_CONFIRM_RESTORE);

         if (CTPlayerPrefs.HasKey(Constants.KEY_CONFIRM_WARNING))
            CONFIRM_WARNING = CTPlayerPrefs.GetBool(Constants.KEY_CONFIRM_WARNING);

         if (!Constants.DEV_DEBUG)
         {
            if (CTPlayerPrefs.HasKey(Constants.KEY_DEBUG))
               DEBUG = CTPlayerPrefs.GetBool(Constants.KEY_DEBUG);
         }
         else
         {
            DEBUG = Constants.DEV_DEBUG;
         }

         if (CTPlayerPrefs.HasKey(Constants.KEY_UPDATE_CHECK))
            UPDATE_CHECK = CTPlayerPrefs.GetBool(Constants.KEY_UPDATE_CHECK);

         if (CTPlayerPrefs.HasKey(Constants.KEY_COMPILE_DEFINES))
            COMPILE_DEFINES = CTPlayerPrefs.GetBool(Constants.KEY_COMPILE_DEFINES);

         //if (CTPlayerPrefs.HasKey(Constants.KEY_BACKUP_DATE))
         //   BACKUP_DATE = CTPlayerPrefs.GetDate(Constants.KEY_BACKUP_DATE);

         if (CTPlayerPrefs.HasKey(Constants.KEY_BACKUP_COUNT))
            BACKUP_COUNT = CTPlayerPrefs.GetInt(Constants.KEY_BACKUP_COUNT);

         if (CTPlayerPrefs.HasKey(Constants.KEY_RESTORE_DATE))
            RESTORE_DATE = CTPlayerPrefs.GetDate(Constants.KEY_RESTORE_DATE);

         if (CTPlayerPrefs.HasKey(Constants.KEY_RESTORE_COUNT))
            RESTORE_COUNT = CTPlayerPrefs.GetInt(Constants.KEY_RESTORE_COUNT);

         if (CTPlayerPrefs.HasKey(Constants.KEY_SETUP_DATE))
            SETUP_DATE = CTPlayerPrefs.GetDate(Constants.KEY_SETUP_DATE);

         if (CTPlayerPrefs.HasKey(Constants.KEY_AUTO_SAVE))
            AUTO_SAVE = CTPlayerPrefs.GetBool(Constants.KEY_AUTO_SAVE);

         if (CTPlayerPrefs.HasKey(Constants.KEY_AUTO_BACKUP_DATE))
            AUTO_BACKUP_DATE = CTPlayerPrefs.GetDate(Constants.KEY_AUTO_BACKUP_DATE);

         if (CTPlayerPrefs.HasKey(Constants.KEY_AUTO_BACKUP_INTERVAL))
            AUTO_BACKUP_INTERVAL = CTPlayerPrefs.GetInt(Constants.KEY_AUTO_BACKUP_INTERVAL);

         isLoaded = true;
      }

      /// <summary>Saves the all changeable variables.</summary>
      public static void Save()
      {
         CTPlayerPrefs.SetBool(Constants.KEY_CUSTOM_PATH_CACHE, CUSTOM_PATH_BACKUP);
         CTPlayerPrefs.SetString(Constants.KEY_PATH_CACHE, PATH_BACKUP);
         CTPlayerPrefs.SetInt(Constants.KEY_VCS, VCS);
         CTPlayerPrefs.SetBool(Constants.KEY_USE_LEGACY, USE_LEGACY);

         CTPlayerPrefs.SetBool(Constants.KEY_BATCHMODE, BATCHMODE);
         CTPlayerPrefs.SetBool(Constants.KEY_QUIT, QUIT);
         CTPlayerPrefs.SetBool(Constants.KEY_NO_GRAPHICS, NO_GRAPHICS);

         CTPlayerPrefs.SetString(Constants.KEY_EXECUTE_METHOD_PRE_BACKUP, EXECUTE_METHOD_PRE_BACKUP);
         CTPlayerPrefs.SetString(Constants.KEY_EXECUTE_METHOD_BACKUP, EXECUTE_METHOD_BACKUP);
         CTPlayerPrefs.SetString(Constants.KEY_EXECUTE_METHOD_PRE_RESTORE, EXECUTE_METHOD_PRE_RESTORE);
         CTPlayerPrefs.SetString(Constants.KEY_EXECUTE_METHOD_RESTORE, EXECUTE_METHOD_RESTORE);
         CTPlayerPrefs.SetBool(Constants.KEY_DELETE_LOCKFILE, DELETE_LOCKFILE);
         CTPlayerPrefs.SetBool(Constants.KEY_COPY_ASSETS, COPY_ASSETS);
         CTPlayerPrefs.SetBool(Constants.KEY_COPY_LIBRARY, COPY_LIBRARY);
         CTPlayerPrefs.SetBool(Constants.KEY_COPY_SETTINGS, COPY_SETTINGS);
         CTPlayerPrefs.SetBool(Constants.KEY_COPY_USER_SETTINGS, COPY_USER_SETTINGS);
         CTPlayerPrefs.SetBool(Constants.KEY_COPY_PACKAGES, COPY_PACKAGES);
         CTPlayerPrefs.SetBool(Constants.KEY_CONFIRM_BACKUP, CONFIRM_BACKUP);
         CTPlayerPrefs.SetBool(Constants.KEY_CONFIRM_RESTORE, CONFIRM_RESTORE);
         CTPlayerPrefs.SetBool(Constants.KEY_CONFIRM_WARNING, CONFIRM_WARNING);

         if (!Constants.DEV_DEBUG)
            CTPlayerPrefs.SetBool(Constants.KEY_DEBUG, DEBUG);

         CTPlayerPrefs.SetBool(Constants.KEY_UPDATE_CHECK, UPDATE_CHECK);
         CTPlayerPrefs.SetBool(Constants.KEY_COMPILE_DEFINES, COMPILE_DEFINES);

         //CTPlayerPrefs.SetDate(Constants.KEY_BACKUP_DATE, BACKUP_DATE);
         CTPlayerPrefs.SetInt(Constants.KEY_BACKUP_COUNT, BACKUP_COUNT);
         CTPlayerPrefs.SetDate(Constants.KEY_RESTORE_DATE, RESTORE_DATE);
         CTPlayerPrefs.SetInt(Constants.KEY_RESTORE_COUNT, RESTORE_COUNT);

         CTPlayerPrefs.SetDate(Constants.KEY_SETUP_DATE, SETUP_DATE);

         CTPlayerPrefs.SetBool(Constants.KEY_AUTO_SAVE, AUTO_SAVE);

         CTPlayerPrefs.SetDate(Constants.KEY_AUTO_BACKUP_DATE, AUTO_BACKUP_DATE);
         CTPlayerPrefs.SetInt(Constants.KEY_AUTO_BACKUP_INTERVAL, AUTO_BACKUP_INTERVAL);

         CTPlayerPrefs.Save();
      }

      #endregion
   }
}
#endif
// © 2018-2022 crosstales LLC (https://www.crosstales.com)