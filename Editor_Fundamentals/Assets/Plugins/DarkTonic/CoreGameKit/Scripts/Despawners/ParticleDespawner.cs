/*! \cond PRIVATE */
using System.Collections.Generic;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace DarkTonic.CoreGameKit {
    [AddComponentMenu("Dark Tonic/Core GameKit/Despawners/Particle Despawner")]
    [RequireComponent(typeof(ParticleSystem))]
    // ReSharper disable once CheckNamespace
    public class ParticleDespawner : MonoBehaviour {
        public bool WaitOnChildObjectParticles;

        private List<ParticleSystem> _particleSystems;
        private Transform _trans;

        // Update is called once per frame
        // ReSharper disable once UnusedMember.Local
        private void Awake() {
            _trans = transform;
            _particleSystems = new List<ParticleSystem>();
            _particleSystems.Add(GetComponent<ParticleSystem>());

            if (WaitOnChildObjectParticles) {
                _particleSystems.AddRange(GetComponentsInChildren<ParticleSystem>());
            } 
        }

        // ReSharper disable once UnusedMember.Local
        private void Update() {
            for (var i = 0; i < _particleSystems.Count; i++) {
                if (_particleSystems[i].IsAlive()) {
                    return;
                }
            }

            // all are dead
            PoolBoss.Despawn(_trans);
        }
    }
}
/*! \endcond */