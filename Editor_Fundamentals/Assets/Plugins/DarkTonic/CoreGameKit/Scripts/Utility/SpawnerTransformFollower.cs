/*! \cond PRIVATE */

// ReSharper disable once CheckNamespace
using UnityEngine;
using System.Collections.Generic;

namespace DarkTonic.CoreGameKit {
    public class SpawnerTransformFollower : MonoBehaviour {
        [Tooltip("This is for diagnostic purposes only. Do not change or assign this field.")]
        public Transform RuntimeFollowingTransform;

        private bool _isFollowing;
        private GameObject _goToFollow;
        private Transform _trans;
        private GameObject _go;
        private Vector3 _fixedOffset;
        private TriggeredSpawnerV2.SpawnerGoneBehavior _spawnerGoneBehavior;
        private List<ParticleSystem> _particlesToStop = new List<ParticleSystem>();

        public void StartFollowing(Transform transToFollow, TriggeredSpawnerV2.SpawnerGoneBehavior spawnerGoneBehavior, bool stopChildParticles) {
            RuntimeFollowingTransform = transToFollow;
            _goToFollow = transToFollow.gameObject;
            _fixedOffset = RuntimeFollowingTransform.position - Trans.position;
            _spawnerGoneBehavior = spawnerGoneBehavior;
            _isFollowing = true;

            _particlesToStop = new List<ParticleSystem>();
            switch (_spawnerGoneBehavior) {
                case TriggeredSpawnerV2.SpawnerGoneBehavior.StopAllParticles:
                    _particlesToStop.Add(Trans.GetComponent<ParticleSystem>());

                    if (stopChildParticles) {
                        _particlesToStop.AddRange(Trans.GetComponentsInChildren<ParticleSystem>());
                    }

                    break;
            }
        }

        private void StopFollowing() {
            RuntimeFollowingTransform = null;
            _isFollowing = false;

            switch (_spawnerGoneBehavior) {
                case TriggeredSpawnerV2.SpawnerGoneBehavior.StopAllParticles:
                    for (var i = 0; i < _particlesToStop.Count; i++) {
                        var particle = _particlesToStop[i];
                        particle.Stop();
                    }
                    break;
            }

            this.enabled = false;
        }

        public void Update() {
            if (!_isFollowing) {
                return;
            }

            if (RuntimeFollowingTransform == null || !CoreMonoHelper.IsActive(_goToFollow)) {
                StopFollowing();
                return;
            }

            Trans.position = RuntimeFollowingTransform.position + _fixedOffset;
        }

        public GameObject GameObj {
            get {
                if (_go != null) {
                    return _go;
                }

                _go = gameObject;
                return _go;
            }
        }

        public Transform Trans {
            get {
                // ReSharper disable once ConvertIfStatementToNullCoalescingExpression
                if (_trans == null) {
                    _trans = transform;
                }

                return _trans;
            }
        }
    }
}
/*! \endcond */
