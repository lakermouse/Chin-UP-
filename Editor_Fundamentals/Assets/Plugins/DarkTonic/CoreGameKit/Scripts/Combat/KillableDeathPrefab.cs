using DarkTonic.CoreGameKit;
using UnityEngine;
#if ADDRESSABLES_ENABLED
	using UnityEngine.AddressableAssets;
#endif

/*! \cond PRIVATE */
namespace DarkTonic.CoreGameKit {
	[AddComponentMenu("Dark Tonic/Core GameKit/Combat/Killable Death Prefab")]
	[RequireComponent(typeof(Killable))]
	public class KillableDeathPrefab : MonoBehaviour {
	    public WaveSpecifics.SpawnOrigin deathPrefabSource = WaveSpecifics.SpawnOrigin.Specific;
		public PoolBoss.PrefabSource deathPrefabFrom = PoolBoss.PrefabSource.Prefab;
		public int deathPrefabPoolIndex = 0;
	    public string deathPrefabPoolName = null;
	    public Transform deathPrefabSpecific;
#if ADDRESSABLES_ENABLED
		public AssetReference deathPrefabAddressable;
#endif
		public bool shouldDeathPrefabSelectFromPB;
	    public string deathPrefabCategoryName;
		public string deathPrefabPoolItemName;
	    public KillerInt deathPrefabSpawnPercent = new KillerInt(100, 0, 100);
	    public Killable.DeathPrefabSpawnLocation deathPrefabSpawnLocation = Killable.DeathPrefabSpawnLocation.DeathPosition;
	    public KillerInt deathPrefabQty = new KillerInt(1, 0, 100);
	    public Vector3 deathPrefabOffset = Vector3.zero;
	    public Vector3 deathPrefabIncrementalOffset = Vector3.zero;
		public KillerFloat deathPrefabRandomDistX = new KillerFloat(0f, 0f, TriggeredSpawner.MaxDistance);
		public KillerFloat deathPrefabRandomDistY = new KillerFloat(0f, 0f, TriggeredSpawner.MaxDistance);
		public KillerFloat deathPrefabRandomDistZ = new KillerFloat(0f, 0f, TriggeredSpawner.MaxDistance);
		public Killable.RotationMode rotationMode = Killable.RotationMode.UseDeathPrefabRotation;
	    public bool deathPrefabRandomizeXRotation = false;
	    public bool deathPrefabRandomizeYRotation = false;
	    public bool deathPrefabRandomizeZRotation = false;
	    public bool deathPrefabKeepVelocity = true;
	    public Vector3 deathPrefabCustomRotation = Vector3.zero;
	    public bool deathPrefabKeepSameParent = true;
	    public bool isExpanded = true;

		private Transform _trans; 

	    private WavePrefabPool deathPrefabWavePool;

	    public WavePrefabPool DeathPrefabWavePool {
	        get {
	            return deathPrefabWavePool;
	        }
	    }

	    private void Awake() {
	        if (deathPrefabPoolName != null && deathPrefabSource == WaveSpecifics.SpawnOrigin.PrefabPool) {
	            deathPrefabWavePool = LevelSettings.GetFirstMatchingPrefabPool(deathPrefabPoolName);
	            if (deathPrefabWavePool == null) {
	                LevelSettings.LogIfNew("Death Prefab Pool '" + deathPrefabPoolName + "' not found for Killable Death Prefab component on '" + name + "'.");
	            }
	        }

			CheckForValidVariables();
	    }

		private void CheckForValidVariables() {
			// examine all KillerInts
			deathPrefabSpawnPercent.LogIfInvalid(Trans, "Killable Death Prefab Spawn % Chance");
			deathPrefabQty.LogIfInvalid(Trans, "Killable Death Prefab Spawn Quantity");
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