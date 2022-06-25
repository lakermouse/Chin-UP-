using UnityEditor;
using UnityEngine;

namespace DarkTonic.CoreGameKit.EditorScripts
{
    // ReSharper disable once CheckNamespace
    [InitializeOnLoad]
    public class CoreGameKitWelcomeWindow : EditorWindow
    {
        private const string Physics2DSymbol = "PHY2D_ENABLED";
        private const string Physics3DSymbol = "PHY3D_ENABLED";
        private const string AddresablesSymbol = "ADDRESSABLES_ENABLED";
        private const string MasterAudioSymbol = "MASTERAUDIO_ENABLED";

        private static bool showOnStartPrefs { // Records the customer's preference to show the window on start or not.
            get {
                return CoreGameKitSettings.Instance.ShowWelcomeWindowOnStart;
            }
            set {
                CoreGameKitSettings.Instance.ShowWelcomeWindowOnStart = value;
                EditorUtility.SetDirty(CoreGameKitSettings.Instance);
            }
        }
        public bool showOnStart = true;

        [MenuItem("Window/Core GameKit/Welcome Window", false, -2)]
        public static CoreGameKitWelcomeWindow ShowWindow()
        {
            var window = GetWindow<CoreGameKitWelcomeWindow>(false, "Welcome");
#if UNITY_2019_3_OR_NEWER
        var height = 279; 
#else
            var height = 295;
#endif

            height += 12; // add height for Addressables checkbox.

            window.minSize = new Vector2(482, height);
            window.maxSize = new Vector2(482, height);
            window.showOnStart = true; // Can't check EditorPrefs when constructing window, so set this instead.
            return window;
        }

        [InitializeOnLoadMethod]
        private static void InitializeOnLoadMethod()
        {
            RegisterWindowCheck();
        }

        private static void RegisterWindowCheck()
        {
            if (!EditorApplication.isPlayingOrWillChangePlaymode)
            {
                EditorApplication.update += CheckShowWelcomeWindow;
            }
        }

        private static void CheckShowWelcomeWindow()
        {
            EditorApplication.update -= CheckShowWelcomeWindow;
            if (showOnStartPrefs)
            {
                ShowWindow();
            }
        }

        void OnGUI()
        {
            DTInspectorUtility.DrawTexture(CoreGameKitInspectorResources.WelcomeLogoTexture);

            DTInspectorUtility.DrawUILine(DTInspectorUtility.DividerColor);
            GUILayout.Label("Welcome to Core GameKit for Unity! The buttons below are shortcuts to commonly used help options.", EditorStyles.textArea);
            DTInspectorUtility.DrawUILine(DTInspectorUtility.DividerColor);

            GUILayout.Label("Help", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Quick Starts", GUILayout.Width(90)))
            {
                Application.OpenURL("http://www.dtdevtools.com/docs/coregamekit/QuickStartOverview.htm");
            }
            if (GUILayout.Button("Manual", GUILayout.Width(90)))
            {
                Application.OpenURL("http://www.dtdevtools.com/docs/coregamekit/TOC.htm");
            }
            if (GUILayout.Button("Videos", GUILayout.Width(90)))
            {
                Application.OpenURL("https://www.youtube.com/watch?v=aBEkcHO6vZk&list=PLW6fMWQDKB24osBmTuJd0IG8R5tOim6eV&index=10");
            }
            if (GUILayout.Button("Scripting API", GUILayout.Width(90)))
            {
                Application.OpenURL("http://www.dtdevtools.com/API/coregamekit/annotated.html");
            }
            if (GUILayout.Button("Support Forum", GUILayout.Width(100)))
            {
                Application.OpenURL("http://darktonic.freeforums.net/board/2/core-gamekit");
            }
            EditorGUILayout.EndHorizontal();
            DTInspectorUtility.DrawUILine(DTInspectorUtility.DividerColor);

            GUILayout.Label("Optional package support", EditorStyles.boldLabel);
            GUILayout.Label("Enable support for:");

            // physics 2D
            var enable2D = DTCGKDefineHelper.DoesScriptingDefineSymbolExist(Physics2DSymbol);
            var new2D = GUILayout.Toggle(enable2D, " 2D Physics (" + Physics2DSymbol + ")");
            if (new2D != enable2D)
            {
                if (new2D)
                {
                    DTCGKDefineHelper.TryAddScriptingDefineSymbols(Physics2DSymbol);
                }
                else
                {
                    DTCGKDefineHelper.TryRemoveScriptingDefineSymbols(Physics2DSymbol);
                }
            }

            // physics 3D
            var enable3D = DTCGKDefineHelper.DoesScriptingDefineSymbolExist(Physics3DSymbol);
            var new3D = GUILayout.Toggle(enable3D, " 3D Physics (" + Physics3DSymbol + ")");
            if (new3D != enable3D)
            {
                if (new3D)
                {
                    DTCGKDefineHelper.TryAddScriptingDefineSymbols(Physics3DSymbol);
                }
                else
                {
                    DTCGKDefineHelper.TryRemoveScriptingDefineSymbols(Physics3DSymbol);
                }
            }

            // Addressables
            var enableAddress = DTCGKDefineHelper.DoesScriptingDefineSymbolExist(AddresablesSymbol);
            var newAddress = GUILayout.Toggle(enableAddress, " Addressables (" + AddresablesSymbol + ")");
            if (newAddress != enableAddress)
            {
                if (newAddress)
                {
                    DTCGKDefineHelper.TryAddScriptingDefineSymbols(AddresablesSymbol);
                }
                else
                {
                    DTCGKDefineHelper.TryRemoveScriptingDefineSymbols(AddresablesSymbol);
                }
            }

            // physics 3D
            var enableMA = DTCGKDefineHelper.DoesScriptingDefineSymbolExist(MasterAudioSymbol);
            var newMA = GUILayout.Toggle(enableMA, " Master Audio (" + MasterAudioSymbol + ")");
            if (newMA != enableMA)
            {
                if (newMA)
                {
                    DTCGKDefineHelper.TryAddScriptingDefineSymbols(MasterAudioSymbol);
                }
                else
                {
                    DTCGKDefineHelper.TryRemoveScriptingDefineSymbols(MasterAudioSymbol);
                }
            }

            DTInspectorUtility.ShowLargeBarAlertBox("Enabling a package you do not have installed will cause a compile error and you will not be able to use this window to undo until you install the missing package.");

            DTInspectorUtility.DrawUILine(DTInspectorUtility.DividerColor);

            EditorGUILayout.BeginHorizontal();
            var show = showOnStartPrefs;
            var newShow = GUILayout.Toggle(show, " Show at start");
            if (newShow != show)
            {
                showOnStartPrefs = newShow;
            }
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(new GUIContent("Email Support", "support@darktonic.com"), GUILayout.Width(100)))
            {
                Application.OpenURL("mailto:support@darktonic.com");
            }

            EditorGUILayout.EndHorizontal();
        }
    }
}