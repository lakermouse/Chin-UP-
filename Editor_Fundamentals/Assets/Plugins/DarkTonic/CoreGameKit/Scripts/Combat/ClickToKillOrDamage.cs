/*! \cond PRIVATE */

using UnityEngine;

// ReSharper disable once CheckNamespace
namespace DarkTonic.CoreGameKit {
    [AddComponentMenu("Dark Tonic/Core GameKit/Combat/Click To Kill Or Damage")]
    // ReSharper disable once CheckNamespace
    public class ClickToKillOrDamage : MonoBehaviour {
        // ReSharper disable InconsistentNaming
        public bool killObjects = true;
        public int damagePointsToInflict = 1;
        // ReSharper restore InconsistentNaming

        void Awake() {
#if !PHY3D_ENABLED
            Debug.LogError("ClickToKillOrDamage will not work without Physics package installed and enabled in Master Audio Welcome Window");
#endif
        }

        // ReSharper disable once UnusedMember.Local
        private void Update() {
            var mouseDown = Input.GetMouseButtonDown(0);
            var fingerDown = Input.touches.Length == 1 && Input.touches[0].phase == TouchPhase.Began;

            if (!mouseDown && !fingerDown) {
                return;
            }
#if PHY3D_ENABLED
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (!Physics.Raycast(ray, out hit, Mathf.Infinity)) {
                return;
            }
            if (hit.collider == null) {
                return;
            }

            KillOrDamage(hit.collider.gameObject);
#endif
        }

        private void KillOrDamage(GameObject go) {
            var kill = go.GetComponent<Killable>();
            if (kill == null) {
                return;
            }

            if (killObjects) {
                kill.DestroyKillable();
            } else {
                kill.TakeDamage(damagePointsToInflict, null);
            }
        }
    }
}

/*! \endcond */