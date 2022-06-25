#if ADDRESSABLES_ENABLED
using UnityEngine.AddressableAssets;

namespace DarkTonic.CoreGameKit.EditorScripts {
    public static class CGKAddressableEditorHelper {
        public static AssetReference CreateAssetReferenceFromObject(UnityEngine.Object source) {
            var assetRef = new AssetReference();
            assetRef.SetEditorAsset(source);
            return assetRef;
        }

        public static string EditTimeAddressableName(AssetReference addressable) {
            if (!IsAddressableValid(addressable)) {
                return string.Empty;
            }

            return addressable.editorAsset.name;
        }

        public static bool IsAddressableValid(AssetReference addressable) {
            if (addressable == null) {
                return false;
            }

            return addressable.editorAsset != null;
        }
    }
}
#endif