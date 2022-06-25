#if UNITY_EDITOR

/*! \cond PRIVATE */
namespace DarkTonic.CoreGameKit {
    public class CoreGameKitSettings : CGKSingletonScriptable<CoreGameKitSettings> {
        public const string AssetName = "CoreGameKitSettings.asset";
        public const string AssetFolder = "Assets/Resources/CoreGameKit";
        public const string ResourcePath = "CoreGameKit/CoreGameKitSettings";

        public bool ShowWelcomeWindowOnStart = true;

        static CoreGameKitSettings()
        {
            AssetNameToLoad = string.Format("{0}/{1}", AssetFolder, AssetName);
            ResourceNameToLoad = ResourcePath;
            FoldersToCreate = new System.Collections.Generic.List<string> {
                "Assets/Resources",
                "Assets/Resources/CoreGameKit"
            };
        }
    }
}
/*! \endcond */

#endif