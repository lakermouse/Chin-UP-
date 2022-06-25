using UnityEngine;

/*! \cond PRIVATE */
namespace DarkTonic.CoreGameKit
{
    public static class TransformHelper {
        public static Vector3 GetGlobalScale(this Transform trans) {
            var totalScale = trans.localScale;

            while (true) {
                trans = trans.parent;
                if (trans == null) {
                    break;
                }

                totalScale = new Vector3(trans.localScale.x * totalScale.x, trans.localScale.y * totalScale.y, trans.localScale.z * totalScale.z);
            }

            return totalScale;
        }

        public static Transform GetRootTransform(this Transform child)
        {
            var root = child;
            while (root.parent != null)
            {
                root = root.parent;
            }

            return root;
        }
    }
}
/*! \endcond */
