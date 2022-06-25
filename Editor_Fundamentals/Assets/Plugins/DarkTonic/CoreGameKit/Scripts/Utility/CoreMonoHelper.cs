using UnityEngine;

/*! \cond PRIVATE */
namespace DarkTonic.CoreGameKit {
	public static class CoreMonoHelper {
		public static Transform GetChildTransform(this Transform transParent, string childName) {
			return transParent.Find(childName);
		}

		public static void SetLayerOnAllChildren(this Transform trans, int layer) {
			var go = trans.gameObject;

			go.layer = layer;

			for (var i = 0; i < trans.childCount; i++) {
				trans.GetChild(i).SetLayerOnAllChildren(layer);
			}
		}

        /// <summary>
        /// This is a cross-Unity-version method to tell you if a GameObject is active in the Scene.
        /// </summary>
        /// <param name="go">The GameObject you're asking about.</param>
        /// <returns>True or false</returns>
        public static bool IsActive(GameObject go) {
            return go.activeInHierarchy;
        }
    }
}
/*! \endcond */