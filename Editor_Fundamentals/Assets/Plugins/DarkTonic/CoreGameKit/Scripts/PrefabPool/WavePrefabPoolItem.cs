/*! \cond PRIVATE */
using System;
using UnityEngine;
#if ADDRESSABLES_ENABLED
    using UnityEngine.AddressableAssets;
#endif

// ReSharper disable once CheckNamespace
namespace DarkTonic.CoreGameKit {
    [Serializable]
    // ReSharper disable once CheckNamespace
    public class WavePrefabPoolItem {
        // ReSharper disable InconsistentNaming
        public PoolBoss.PrefabSource prefabFrom = PoolBoss.PrefabSource.Prefab;
        public Transform prefabToSpawn;
#if ADDRESSABLES_ENABLED
        public AssetReference prefabAddressable;
#endif
        public string prefabPoolBossCategory;
        public string prefabPoolBossPoolItemName;
        public bool shouldSelectFromPB = false;
        public LevelSettings.ActiveItemMode activeMode = LevelSettings.ActiveItemMode.Always;
        public WorldVariableRangeCollection activeItemCriteria = new WorldVariableRangeCollection();
        public KillerInt thisWeight = new KillerInt(1, 0, 256);
        public bool isExpanded = true;
        // ReSharper restore InconsistentNaming
    }
}
/*! \endcond */