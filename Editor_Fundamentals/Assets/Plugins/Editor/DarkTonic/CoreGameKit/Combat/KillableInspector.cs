using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
#if MASTERAUDIO_ENABLED
    using DarkTonic.MasterAudio;
#endif

namespace DarkTonic.CoreGameKit.EditorScripts
{
    /// <summary>
    /// Killable inspector.
    /// 3 Steps to make a subclass Inspector (if you're not on Unity 4).
    /// 
    /// 1) Duplicate the KillableInspector file (this one). Open it.
    /// 2) Change "Killable" on line 16 and line 18 to the name of your Killable subclass. Also change the 2 instances of "Killable" on line 25 to the same.
    /// 3) Change the "KillableInspector" on line 20 to your Killable subclass + "Inspector". Also change the filename to the same.
    /// </summary>

    [CustomEditor(typeof(Killable), true)]
    // ReSharper disable once CheckNamespace
    public class KillableInspector : Editor
    {
        private Killable _killable;
        private bool _isDirty;

        // ReSharper disable once FunctionComplexityOverflow
        public override void OnInspectorGUI()
        {
            EditorGUI.indentLevel = 0;

            _killable = (Killable)target;

            WorldVariableTracker.ClearInGamePlayerStats();

            LevelSettings.Instance = null; // clear cached version
            DTInspectorUtility.DrawTexture(CoreGameKitInspectorResources.LogoTexture);
            DTInspectorUtility.HelpHeader("http://www.dtdevtools.com/docs/coregamekit/Killables.htm");

            _isDirty = false;

            var ls = LevelSettings.Instance;

            var _levelSettingsInScene = ls != null;

            if (!_levelSettingsInScene)
            {
                DTInspectorUtility.ShowRedErrorBox("Cannot display Killable configuration without LevelSettings Game Object in the Scene.");
                return;
            }

#if MASTERAUDIO_ENABLED
        MasterAudio.MasterAudio.Instance = null;

        var ma = MasterAudio.MasterAudio.Instance;
        var _maInScene = ma != null;

        var _groupNames = new List<string>();
        if (_maInScene) {
            // ReSharper disable once PossibleNullReferenceException
            _groupNames = ma.GroupNames;
        }
#endif

            var allStats = KillerVariablesHelper.AllStatNames;

            if (Application.isPlaying)
            {
                if (_killable.GameIsOverForKillable)
                {
                    DTInspectorUtility.RedBoldMessage("Killable disabled by Game Over Behavior setting");
                }

                if (!SpawnUtility.IsActive(_killable.gameObject))
                {
                    DTInspectorUtility.RedBoldMessage("Despawned and inactive!");
                }
            }

            EditorGUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);

            GUILayout.Label("Immediate Actions");

            GUILayout.FlexibleSpace();

            if (Application.isPlaying)
            {
                if (SpawnUtility.IsActive(_killable.gameObject))
                {
                    GUI.backgroundColor = DTInspectorUtility.AddButtonColor;
                    if (GUILayout.Button("Kill", EditorStyles.toolbarButton, GUILayout.Width(50)))
                    {
                        _killable.DestroyKillable();
                    }

                    GUILayout.Space(10);

                    if (GUILayout.Button("Despawn", EditorStyles.toolbarButton, GUILayout.Width(60)))
                    {
                        _killable.Despawn(TriggeredSpawner.EventType.CodeTriggered1);
                    }

                    GUILayout.Space(10);

                    if (GUILayout.Button("Take 1 Damage", EditorStyles.toolbarButton, GUILayout.Width(90)))
                    {
                        _killable.TakeDamage(1);
                    }
                    GUILayout.Space(10);
                }
                else
                {
                    GUI.contentColor = DTInspectorUtility.BrightTextColor;
                    GUILayout.Label("Not available when despawned.");
                }
            }

            GUI.contentColor = Color.white;
            GUI.backgroundColor = Color.white;

            if (DTInspectorUtility.ShowRelationsButton())
            {
                DTInspectorUtility.ShowKillableRelations();
            }

            EditorGUILayout.EndHorizontal();

            KillerVariablesHelper.DisplayKillerInt(ref _isDirty, _killable.atckPoints, "Start Attack Points", _killable);

            KillerVariablesHelper.DisplayKillerInt(ref _isDirty, _killable.hitPoints, "Start Hit Points", _killable);

            KillerVariablesHelper.DisplayKillerInt(ref _isDirty, _killable.maxHitPoints, "Max Hit Points", _killable);

            EditorGUI.indentLevel = 1;
            if (_killable.hitPoints.variableSource == LevelSettings.VariableSource.Variable)
            {
                var newSync = EditorGUILayout.Toggle("Sync H.P. Variable", _killable.syncHitPointWorldVariable);
                if (newSync != _killable.syncHitPointWorldVariable)
                {
                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _killable, "toggle Sync H.P. Variable");
                    _killable.syncHitPointWorldVariable = newSync;
                }
            }

            EditorGUI.indentLevel = 0;
            if (Application.isPlaying)
            {
                _killable.currentHitPoints = EditorGUILayout.IntSlider("Remaining Hit Points", _killable.currentHitPoints, 0, Killable.MaxAttackPoints);
            }

#if !PHY3D_ENABLED && !PHY2D_ENABLED
            DTInspectorUtility.ShowLargeBarAlertBox("Collisions will not work since both 3D Physics and 2D Physics are disabled in the Welcome Window.");
#endif


            var newIgnore = EditorGUILayout.Toggle("Ignore Offscreen Hits", _killable.ignoreOffscreenHits);
            if (newIgnore != _killable.ignoreOffscreenHits)
            {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _killable, "toggle Ignore Offscreen Hits");
                _killable.ignoreOffscreenHits = newIgnore;
            }

            var newLog = EditorGUILayout.Toggle("Log Events", _killable.enableLogging);
            if (newLog != _killable.enableLogging)
            {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _killable, "toggle Log Events");
                _killable.enableLogging = newLog;
            }

            var newGO = (TriggeredSpawner.GameOverBehavior)EditorGUILayout.EnumPopup("Game Over Behavior", _killable.gameOverBehavior);
            if (newGO != _killable.gameOverBehavior)
            {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _killable, "change Game Over Behavior");
                _killable.gameOverBehavior = newGO;
            }

            var hadNoListener = _killable.listener == null;
            var newListener = (KillableListener)EditorGUILayout.ObjectField("Listener", _killable.listener, typeof(KillableListener), true);
            if (newListener != _killable.listener)
            {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _killable, "assign Listener");
                _killable.listener = newListener;
                if (hadNoListener && _killable.listener != null)
                {
                    _killable.listener.sourceKillableName = _killable.transform.name;
                }
            }

            var trans = _killable.Trans;
            PoolBoss.PrefabSource src = PoolBoss.PrefabSource.Prefab;

                PoolBossEditorUtility.DisplayPrefab(ref _isDirty, _killable, ref trans, ref _killable.poolBossCategoryName,
                    ref _killable.poolBossItemName,
                    ref _killable.shouldSelectFromPB, ref src, true, "Killable"
    #if ADDRESSABLES_ENABLED
    , ref _killable.prefabAddressable
    , serializedObject
    , serializedObject.FindProperty(nameof(Killable.prefabAddressable))
    #endif
            , false);

            GUI.contentColor = DTInspectorUtility.BrightButtonColor;
            if (GUILayout.Button("Collapse All Sections", EditorStyles.toolbarButton, GUILayout.Width(140)))
            {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _killable, "toggle Collapse All Sections");
                _killable.invincibilityExpanded = false;
                _killable.filtersExpanded = false;
                _killable.dealDamagePrefabExpanded = false;
                _killable.damagePrefabExpanded = false;
                _killable.despawnStatDamageModifiersExpanded = false;
                _killable.showVisibilitySettings = false;
                _killable.deathPrefabSettingsExpanded = false;
                _killable.despawnStatModifiersExpanded = false;
                _killable.showRespawnSettings = false;
                _killable.damageKnockBackExpanded = false;
            }
            GUI.contentColor = Color.white;

            DTInspectorUtility.VerticalSpace(4);


            var state = _killable.invincibilityExpanded;
            var text = "Invinciblity Settings";

            DTInspectorUtility.ShowCollapsibleSection(ref state, text);

            if (state != _killable.invincibilityExpanded)
            {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _killable, "toggle expand Invincibility Settings");
                _killable.invincibilityExpanded = state;
            }
            DTInspectorUtility.AddHelpIconNoStyle("http://www.dtdevtools.com/docs/coregamekit/Killables.htm#Invincibility");
            EditorGUILayout.EndHorizontal();

            var poolNames = LevelSettings.GetSortedPrefabPoolNames();

            if (_killable.invincibilityExpanded)
            {
                DTInspectorUtility.BeginGroupedControls();
                var newInvince = EditorGUILayout.Toggle("Invincible?", _killable.isInvincible);
                if (newInvince != _killable.isInvincible)
                {
                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _killable, "toggle Invincible");
                    _killable.isInvincible = newInvince;
                }

#if MASTERAUDIO_ENABLED
            CGKMAHelper.SoundGroupField(_maInScene, _groupNames, "Invincible Hit Sound", _killable, ref _isDirty, ref _killable.invincibleHitSound);
#endif

                DTInspectorUtility.StartGroupHeader();
                var prefabSource = (Killable.SpawnSource)EditorGUILayout.EnumPopup("Invince Hit Prefab Type", _killable.invinceHitPrefabSource);
                if (prefabSource != _killable.invinceHitPrefabSource)
                {
                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _killable, "change Invince Hit Prefab Type");
                    _killable.invinceHitPrefabSource = prefabSource;
                }

                EditorGUILayout.EndVertical();

                var isValid = true;

                switch (_killable.invinceHitPrefabSource)
                {
                    case Killable.SpawnSource.PrefabPool:
                        if (poolNames != null)
                        {
                            var pool = LevelSettings.GetFirstMatchingPrefabPool(_killable.invinceHitPrefabPoolName);
                            var noDmgPool = false;
                            var invalidDmgPool = false;
                            var noPrefabPools = false;

                            if (pool == null)
                            {
                                if (string.IsNullOrEmpty(_killable.invinceHitPrefabPoolName))
                                {
                                    noDmgPool = true;
                                }
                                else
                                {
                                    invalidDmgPool = true;
                                }
                                _killable.invinceHitPrefabPoolIndex = 0;
                            }
                            else
                            {
                                _killable.invinceHitPrefabPoolIndex = poolNames.IndexOf(_killable.invinceHitPrefabPoolName);
                            }

                            if (poolNames.Count > 1)
                            {
                                EditorGUILayout.BeginHorizontal();
                                var newPoolIndex = EditorGUILayout.Popup("Invince Hit Prefab Pool", _killable.invinceHitPrefabPoolIndex, poolNames.ToArray());
                                if (newPoolIndex != _killable.invinceHitPrefabPoolIndex)
                                {
                                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _killable, "change Invince Hit Prefab Pool");
                                    _killable.invinceHitPrefabPoolIndex = newPoolIndex;
                                }

                                if (_killable.invinceHitPrefabPoolIndex > 0)
                                {
                                    var matchingPool = LevelSettings.GetFirstMatchingPrefabPool(poolNames[_killable.invinceHitPrefabPoolIndex]);
                                    if (matchingPool != null)
                                    {
                                        _killable.invinceHitPrefabPoolName = matchingPool.name;
                                    }
                                }
                                else
                                {
                                    _killable.invinceHitPrefabPoolName = string.Empty;
                                }

                                if (newPoolIndex > 0)
                                {
                                    if (DTInspectorUtility.AddControlButtons("Prefab Pool") ==
                                        DTInspectorUtility.FunctionButtons.Edit)
                                    {
                                        Selection.activeGameObject = pool.gameObject;
                                    }
                                }
                                EditorGUILayout.EndHorizontal();
                            }
                            else
                            {
                                noPrefabPools = true;
                            }

                            if (noPrefabPools)
                            {
                                DTInspectorUtility.ShowRedErrorBox("You have no Prefab Pools. Create one first.");
                                isValid = false;
                            }
                            else if (noDmgPool)
                            {
                                DTInspectorUtility.ShowRedErrorBox("No Invince Hit Prefab Pool selected.");
                                isValid = false;
                            }
                            else if (invalidDmgPool)
                            {
                                DTInspectorUtility.ShowRedErrorBox("Invince Hit Prefab Pool '" + _killable.invinceHitPrefabPoolName + "' not found. Select one.");
                                isValid = false;
                            }
                        }
                        else
                        {
                            DTInspectorUtility.ShowRedErrorBox(LevelSettings.NoPrefabPoolsContainerAlert);
                            DTInspectorUtility.ShowRedErrorBox(LevelSettings.RevertLevelSettingsAlert);
                            isValid = false;
                        }

                        break;
                    case Killable.SpawnSource.Specific:
                        PoolBossEditorUtility.DisplayPrefab(ref _isDirty, _killable, ref _killable.invinceHitPrefabSpecific, ref _killable.invinceHitPrefabCategoryName,
                            ref _killable.invinceHitPrefabPoolItemName,
                            ref _killable.shouldInvinceHitPrefabSelectFromPB, ref _killable.invinceHitPrefabFrom, false, "Invince Hit Prefab"
#if ADDRESSABLES_ENABLED
    , ref _killable.invinceHitPrefabAddressable
    , serializedObject
    , serializedObject.FindProperty(nameof(Killable.invinceHitPrefabAddressable))
#endif
                        );

                        if (_killable.invinceHitPrefabSpecific == null)
                        {
                            isValid = false;
                        }
                        break;
                    case Killable.SpawnSource.None:
                        isValid = false;
                        break;
                }

                if (isValid)
                {
                    EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(100));
                    EditorGUILayout.LabelField("Random Rotation");

                    var newRandomX = GUILayout.Toggle(_killable.invinceHitPrefabRandomizeXRotation, "X");
                    if (newRandomX != _killable.invinceHitPrefabRandomizeXRotation)
                    {
                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _killable, "toggle Random X Rotation");
                        _killable.invinceHitPrefabRandomizeXRotation = newRandomX;
                    }
                    GUILayout.Space(10);
                    var newRandomY = GUILayout.Toggle(_killable.invinceHitPrefabRandomizeYRotation, "Y");
                    if (newRandomY != _killable.invinceHitPrefabRandomizeYRotation)
                    {
                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _killable, "toggle Random Y Rotation");
                        _killable.invinceHitPrefabRandomizeYRotation = newRandomY;
                    }
                    GUILayout.Space(10);
                    var newRandomZ = GUILayout.Toggle(_killable.invinceHitPrefabRandomizeZRotation, "Z");
                    if (newRandomZ != _killable.invinceHitPrefabRandomizeZRotation)
                    {
                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _killable, "toggle Random Z Rotation");
                        _killable.invinceHitPrefabRandomizeZRotation = newRandomZ;
                    }


                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndVertical();

                DTInspectorUtility.StartGroupHeader();
                newInvince = GUILayout.Toggle(_killable.invincibleWhileChildrenKillablesExist, "Inv. While Children Alive");
                if (newInvince != _killable.invincibleWhileChildrenKillablesExist)
                {
                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _killable, "toggle Inv. While Children Alive");
                    _killable.invincibleWhileChildrenKillablesExist = newInvince;
                }
                EditorGUILayout.EndVertical();

                if (_killable.invincibleWhileChildrenKillablesExist)
                {
                    EditorGUI.indentLevel = 0;

                    var newDisable = EditorGUILayout.Toggle("Disable Colliders Also", _killable.disableCollidersWhileChildrenKillablesExist);
                    if (newDisable != _killable.disableCollidersWhileChildrenKillablesExist)
                    {
                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _killable, "toggle Disable Colliders Also");
                        _killable.disableCollidersWhileChildrenKillablesExist = newDisable;
                    }
                }
                EditorGUILayout.EndVertical();

                DTInspectorUtility.StartGroupHeader();
                EditorGUI.indentLevel = 0;
                newInvince = GUILayout.Toggle(_killable.invincibleOnSpawn, "Invincible On Spawn");
                if (newInvince != _killable.invincibleOnSpawn)
                {
                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _killable, "toggle Invincible On Spawn");
                    _killable.invincibleOnSpawn = newInvince;
                }
                EditorGUILayout.EndVertical();

                if (_killable.invincibleOnSpawn)
                {
                    EditorGUI.indentLevel = 0;
                    KillerVariablesHelper.DisplayKillerFloat(ref _isDirty, _killable.invincibleTimeSpawn, "Invincibility Time (sec)", _killable);
                }
                EditorGUILayout.EndVertical();

                DTInspectorUtility.StartGroupHeader();
                EditorGUI.indentLevel = 0;
                newInvince = GUILayout.Toggle(_killable.invincibleWhenDamaged, "Invincible After Damaged");
                if (newInvince != _killable.invincibleWhenDamaged)
                {
                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _killable, "toggle Invincible After Damaged");
                    _killable.invincibleWhenDamaged = newInvince;
                }
                EditorGUILayout.EndVertical();

                if (_killable.invincibleWhenDamaged)
                {
                    EditorGUI.indentLevel = 0;
                    KillerVariablesHelper.DisplayKillerFloat(ref _isDirty, _killable.invincibleDamageTime, "Invincibility Time (sec)", _killable);
                }
                EditorGUILayout.EndVertical();

                DTInspectorUtility.EndGroupedControls();
            }

            // layer / tag / limit filters
            EditorGUI.indentLevel = 0;
            DTInspectorUtility.VerticalSpace(2);

            state = _killable.filtersExpanded;
            text = "Layer and Tag filters";

            DTInspectorUtility.ShowCollapsibleSection(ref state, text);

            if (state != _killable.filtersExpanded)
            {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _killable, "toggle expand Layer and Tag filters");
                _killable.filtersExpanded = state;
            }

            DTInspectorUtility.AddHelpIconNoStyle("http://www.dtdevtools.com/docs/coregamekit/Killables.htm#LayerTagFilter");

            EditorGUILayout.EndHorizontal();

            if (_killable.filtersExpanded)
            {
                DTInspectorUtility.BeginGroupedControls();
                EditorGUI.indentLevel = 0;
                DTInspectorUtility.ShowColorWarningBox("This section controls which other Killables can damage this one.");

                var newIgnoreSpawned = EditorGUILayout.Toggle("Ignore Killables I Spawn", _killable.ignoreKillablesSpawnedByMe);
                if (_killable.ignoreKillablesSpawnedByMe != newIgnoreSpawned)
                {
                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _killable, "toggle Ignore Killables I Spawn");
                    _killable.ignoreKillablesSpawnedByMe = newIgnoreSpawned;
                }

                DTInspectorUtility.StartGroupHeader();
                var newUseLayer = EditorGUILayout.BeginToggleGroup(" Layer Filter", _killable.useLayerFilter);
                if (newUseLayer != _killable.useLayerFilter)
                {
                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _killable, "toggle Layer Filter");
                    _killable.useLayerFilter = newUseLayer;
                }
                DTInspectorUtility.EndGroupHeader();
                if (_killable.useLayerFilter)
                {
                    for (var i = 0; i < _killable.matchingLayers.Count; i++)
                    {
                        var newLayer = EditorGUILayout.LayerField("Layer Match " + (i + 1), _killable.matchingLayers[i]);
                        if (newLayer == _killable.matchingLayers[i])
                        {
                            continue;
                        }
                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _killable, "change Layer Match");
                        _killable.matchingLayers[i] = newLayer;
                    }
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(2);
                    GUI.contentColor = DTInspectorUtility.BrightButtonColor;
                    if (GUILayout.Button(new GUIContent("Add", "Click to add a Layer Match at the end"), EditorStyles.toolbarButton, GUILayout.Width(60)))
                    {
                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _killable, "add Layer Match");
                        _killable.matchingLayers.Add(0);
                    }
                    GUILayout.Space(10);
                    if (_killable.matchingLayers.Count > 1)
                    {
                        if (GUILayout.Button(new GUIContent("Remove", "Click to remove the last Layer Match"), EditorStyles.toolbarButton, GUILayout.Width(60)))
                        {
                            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _killable, "remove Layer Match");
                            _killable.matchingLayers.RemoveAt(_killable.matchingLayers.Count - 1);
                        }
                    }
                    GUI.contentColor = Color.white;
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndToggleGroup();

                DTInspectorUtility.StartGroupHeader();
                state = EditorGUILayout.BeginToggleGroup(" Tag Filter", _killable.useTagFilter);
                if (state != _killable.useTagFilter)
                {
                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _killable, "toggle Tag Filter");
                    _killable.useTagFilter = state;
                }
                DTInspectorUtility.EndGroupHeader();
                if (_killable.useTagFilter)
                {
                    for (var i = 0; i < _killable.matchingTags.Count; i++)
                    {
                        var newTag = EditorGUILayout.TagField("Tag Match " + (i + 1), _killable.matchingTags[i]);
                        if (newTag == _killable.matchingTags[i])
                        {
                            continue;
                        }
                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _killable, "change Tag Match");
                        _killable.matchingTags[i] = newTag;
                    }
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(2);
                    GUI.contentColor = DTInspectorUtility.BrightButtonColor;
                    if (GUILayout.Button(new GUIContent("Add", "Click to add a Tag Match at the end"), EditorStyles.toolbarButton, GUILayout.Width(60)))
                    {
                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _killable, "add Tag Match");
                        _killable.matchingTags.Add("Untagged");
                    }
                    GUILayout.Space(10);
                    if (_killable.matchingTags.Count > 1)
                    {
                        if (GUILayout.Button(new GUIContent("Remove", "Click to remove the last Tag Match"), EditorStyles.toolbarButton, GUILayout.Width(60)))
                        {
                            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _killable, "remove Tag Match");
                            _killable.matchingTags.RemoveAt(_killable.matchingLayers.Count - 1);
                        }
                    }
                    GUI.contentColor = Color.white;
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndToggleGroup();

                DTInspectorUtility.EndGroupedControls();
            }

            // deal damage prefab section
            DTInspectorUtility.VerticalSpace(2);
            EditorGUI.indentLevel = 0;

            state = _killable.dealDamagePrefabExpanded;
            text = "Deal Damage Settings";

            DTInspectorUtility.ShowCollapsibleSection(ref state, text);

            if (state != _killable.dealDamagePrefabExpanded)
            {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _killable, "toggle expand Deal Damage Prefab Settings");
                _killable.dealDamagePrefabExpanded = state;
            }

            DTInspectorUtility.AddHelpIconNoStyle("http://www.dtdevtools.com/docs/coregamekit/Killables.htm#DealDamage");

            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel = 0;
            if (_killable.dealDamagePrefabExpanded)
            {
                DTInspectorUtility.BeginGroupedControls();

#if MASTERAUDIO_ENABLED
            CGKMAHelper.SoundGroupField(_maInScene, _groupNames, "Deal Damage Sound", _killable, ref _isDirty, ref _killable.dealDamageSound);
#endif

                var dmgMode = (Killable.DealDamageMode)EditorGUILayout.EnumPopup("Deal Damage Mode", _killable.dealDamageMode);
                if (dmgMode != _killable.dealDamageMode)
                {
                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _killable, "change Deal Damage Mode");
                    _killable.dealDamageMode = dmgMode;
                }

                switch (_killable.dealDamageMode)
                {
                    case Killable.DealDamageMode.Calculation:
                        var dmgCalcMode = (Killable.DeathVelocityDamageCalcMode)EditorGUILayout.EnumPopup("Deal Damage Calc. Mode", _killable.dealDamageCalcMode);
                        if (dmgCalcMode != _killable.dealDamageCalcMode)
                        {
                            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _killable, "change Deal Damage Calc. Mode");
                            _killable.dealDamageCalcMode = dmgCalcMode;
                        }

                        switch (_killable.dealDamageCalcMode)
                        {
                            case Killable.DeathVelocityDamageCalcMode.FixedTimesVelocity:
                            case Killable.DeathVelocityDamageCalcMode.FixedTimesVelocityTimesMass:
                                if (!_killable.IsGravBody)
                                {
                                    DTInspectorUtility.ShowRedErrorBox("Modes with Velocity ignore Velocity in the calculation since this is not a gravity Rigidbody.");
                                }
                                break;
                        }

                        KillerVariablesHelper.DisplayKillerFloat(ref _isDirty, _killable.dealDamageFixed, "Fixed Damage", _killable);

                        break;
                }

                var dmgSource = (Killable.SpawnSource)EditorGUILayout.EnumPopup("Deal Damage Spawn Source", _killable.dealDamagePrefabSource);
                if (dmgSource != _killable.dealDamagePrefabSource)
                {
                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _killable, "change Deal Damage Spawn Source");
                    _killable.dealDamagePrefabSource = dmgSource;
                }

                var isValid = true;

                switch (_killable.dealDamagePrefabSource)
                {
                    case Killable.SpawnSource.PrefabPool:
                        if (poolNames != null)
                        {
                            var pool = LevelSettings.GetFirstMatchingPrefabPool(_killable.dealDamagePrefabPoolName);
                            var noDmgPool = false;
                            var invalidDmgPool = false;
                            var noPrefabPools = false;

                            if (pool == null)
                            {
                                if (string.IsNullOrEmpty(_killable.dealDamagePrefabPoolName))
                                {
                                    noDmgPool = true;
                                }
                                else
                                {
                                    invalidDmgPool = true;
                                }
                                _killable.dealDamagePrefabPoolIndex = 0;
                            }
                            else
                            {
                                _killable.dealDamagePrefabPoolIndex = poolNames.IndexOf(_killable.dealDamagePrefabPoolName);
                            }

                            if (poolNames.Count > 1)
                            {
                                EditorGUILayout.BeginHorizontal();
                                var newPoolIndex = EditorGUILayout.Popup("Deal Damage Prefab Pool", _killable.dealDamagePrefabPoolIndex, poolNames.ToArray());
                                if (newPoolIndex != _killable.dealDamagePrefabPoolIndex)
                                {
                                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _killable, "change Damage Prefab Pool");
                                    _killable.dealDamagePrefabPoolIndex = newPoolIndex;
                                }

                                if (_killable.dealDamagePrefabPoolIndex > 0)
                                {
                                    var matchingPool = LevelSettings.GetFirstMatchingPrefabPool(poolNames[_killable.dealDamagePrefabPoolIndex]);
                                    if (matchingPool != null)
                                    {
                                        _killable.dealDamagePrefabPoolName = matchingPool.name;
                                    }
                                }
                                else
                                {
                                    _killable.dealDamagePrefabPoolName = string.Empty;
                                }

                                if (newPoolIndex > 0)
                                {
                                    if (DTInspectorUtility.AddControlButtons("Prefab Pool") ==
                                        DTInspectorUtility.FunctionButtons.Edit)
                                    {
                                        Selection.activeGameObject = pool.gameObject;
                                    }
                                }
                                EditorGUILayout.EndHorizontal();
                            }
                            else
                            {
                                noPrefabPools = true;
                            }

                            if (noPrefabPools)
                            {
                                DTInspectorUtility.ShowRedErrorBox("You have no Prefab Pools. Create one first.");
                                isValid = false;
                            }
                            else if (noDmgPool)
                            {
                                DTInspectorUtility.ShowRedErrorBox("No Damage Prefab Pool selected.");
                                isValid = false;
                            }
                            else if (invalidDmgPool)
                            {
                                DTInspectorUtility.ShowRedErrorBox("Damage Prefab Pool '" + _killable.dealDamagePrefabPoolName + "' not found. Select one.");
                                isValid = false;
                            }
                        }
                        else
                        {
                            DTInspectorUtility.ShowRedErrorBox(LevelSettings.NoPrefabPoolsContainerAlert);
                            DTInspectorUtility.ShowRedErrorBox(LevelSettings.RevertLevelSettingsAlert);
                            isValid = false;
                        }

                        break;
                    case Killable.SpawnSource.Specific:
                        PoolBossEditorUtility.DisplayPrefab(ref _isDirty, _killable, ref _killable.dealDamagePrefabSpecific, ref _killable.dealDamagePrefabCategoryName,
                            ref _killable.dealDamagePrefabPoolItemName,
                            ref _killable.shouldDealDamagePrefabSelectFromPB, ref _killable.dealDamagePrefabFrom, false, "Deal Damage Prefab"
#if ADDRESSABLES_ENABLED
    , ref _killable.dealDamagePrefabAddressable
    , serializedObject
    , serializedObject.FindProperty(nameof(Killable.dealDamagePrefabAddressable))
#endif
                        );

                        if (_killable.dealDamagePrefabSpecific == null)
                        {
                            isValid = false;
                        }
                        break;
                    case Killable.SpawnSource.None:
                        isValid = false;
                        break;
                }

                if (isValid)
                {
                    EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(100));
                    EditorGUILayout.LabelField("Random Rotation");

                    var newRandomX = GUILayout.Toggle(_killable.dealDamagePrefabRandomizeXRotation, "X");
                    if (newRandomX != _killable.dealDamagePrefabRandomizeXRotation)
                    {
                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _killable, "toggle Random X Rotation");
                        _killable.dealDamagePrefabRandomizeXRotation = newRandomX;
                    }
                    GUILayout.Space(10);
                    var newRandomY = GUILayout.Toggle(_killable.dealDamagePrefabRandomizeYRotation, "Y");
                    if (newRandomY != _killable.dealDamagePrefabRandomizeYRotation)
                    {
                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _killable, "toggle Random Y Rotation");
                        _killable.dealDamagePrefabRandomizeYRotation = newRandomY;
                    }
                    GUILayout.Space(10);
                    var newRandomZ = GUILayout.Toggle(_killable.dealDamagePrefabRandomizeZRotation, "Z");
                    if (newRandomZ != _killable.dealDamagePrefabRandomizeZRotation)
                    {
                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _killable, "toggle Random Z Rotation");
                        _killable.dealDamagePrefabRandomizeZRotation = newRandomZ;
                    }
                    EditorGUILayout.EndHorizontal();
                }

                var newLast = EditorGUILayout.Toggle("Spawn/Fire on Death Hit", _killable.dealDamagePrefabOnDeathHit);
                if (newLast != _killable.dealDamagePrefabOnDeathHit)
                {
                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _killable, "toggle Spawn/Fire on Death Hit");
                    _killable.dealDamagePrefabOnDeathHit = newLast;
                }

                DTInspectorUtility.StartGroupHeader(0, false);
                var newExp = EditorGUILayout.Toggle("Deal Damage Cust. Events", _killable.dealDamageFireEvents);
                if (newExp != _killable.dealDamageFireEvents)
                {
                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _killable, "toggle Deal Damage Cust. Events");
                    _killable.dealDamageFireEvents = newExp;
                }

                if (_killable.dealDamageFireEvents)
                {
                    DTInspectorUtility.ShowColorWarningBox(
                        "When this deals damage (even if invincible recipient), fire the Custom Events below");

                    EditorGUILayout.BeginHorizontal();
                    GUI.contentColor = DTInspectorUtility.AddButtonColor;
                    GUILayout.Space(2);
                    if (GUILayout.Button(new GUIContent("Add", "Click to add a Custom Event"), EditorStyles.toolbarButton,
                        GUILayout.Width(50)))
                    {
                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _killable, "Add Deal Damage Custom Event");
                        _killable.dealDamageCustomEvents.Add(new CGKCustomEventToFire());
                    }
                    GUI.contentColor = Color.white;

                    EditorGUILayout.EndHorizontal();

                    if (_killable.dealDamageCustomEvents.Count == 0)
                    {
                        DTInspectorUtility.ShowColorWarningBox("You have no Custom Events selected to fire.");
                    }

                    DTInspectorUtility.VerticalSpace(2);

                    int? indexToDelete = null;

                    // ReSharper disable once ForCanBeConvertedToForeach
                    for (var i = 0; i < _killable.dealDamageCustomEvents.Count; i++)
                    {
                        var anEvent = _killable.dealDamageCustomEvents[i].CustomEventName;

                        var buttonClicked = DTInspectorUtility.FunctionButtons.None;
                        anEvent = DTInspectorUtility.SelectCustomEventForVariable(ref _isDirty, anEvent, _killable,
                            "Custom Event", ref buttonClicked);

                        if (buttonClicked == DTInspectorUtility.FunctionButtons.Remove)
                        {
                            indexToDelete = i;
                        }

                        if (anEvent == _killable.dealDamageCustomEvents[i].CustomEventName)
                        {
                            continue;
                        }

                        _killable.dealDamageCustomEvents[i].CustomEventName = anEvent;
                    }

                    if (indexToDelete.HasValue)
                    {
                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _killable, "Remove Deal Damage Custom Event");
                        _killable.dealDamageCustomEvents.RemoveAt(indexToDelete.Value);
                    }
                }
                EditorGUILayout.EndVertical();

                DTInspectorUtility.EndGroupedControls();
            }

            // damage prefab section
            DTInspectorUtility.VerticalSpace(2);
            EditorGUI.indentLevel = 0;

            state = _killable.damagePrefabExpanded;
            text = "Damage Prefab Settings & Events";

            DTInspectorUtility.ShowCollapsibleSection(ref state, text);

            if (state != _killable.damagePrefabExpanded)
            {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _killable, "toggle expand Damage Prefab Settings & Events");
                _killable.damagePrefabExpanded = state;
            }

            DTInspectorUtility.AddHelpIconNoStyle("http://www.dtdevtools.com/docs/coregamekit/Killables.htm#DamagePrefab");

            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel = 0;
            if (_killable.damagePrefabExpanded)
            {
                DTInspectorUtility.BeginGroupedControls();

#if MASTERAUDIO_ENABLED
            CGKMAHelper.SoundGroupField(_maInScene, _groupNames, "Damage Prefab Sound", _killable, ref _isDirty, ref _killable.damagedSound);
#endif

                var newSpawnMode = (Killable.DamagePrefabSpawnMode)EditorGUILayout.EnumPopup("Spawn Frequency", _killable.damagePrefabSpawnMode);
                if (newSpawnMode != _killable.damagePrefabSpawnMode)
                {
                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _killable, "change Spawn Frequency");
                    _killable.damagePrefabSpawnMode = newSpawnMode;
                }

                if (_killable.damagePrefabSpawnMode != Killable.DamagePrefabSpawnMode.None)
                {
                    if (_killable.damagePrefabSpawnMode == Killable.DamagePrefabSpawnMode.PerGroupHitPointsLost)
                    {
                        KillerVariablesHelper.DisplayKillerInt(ref _isDirty, _killable.damageGroupsize, "Group H.P. Amount", _killable);
                    }

                    KillerVariablesHelper.DisplayKillerInt(ref _isDirty, _killable.damagePrefabSpawnQuantity, "Spawn Quantity", _killable);

                    var newDmgSource = (Killable.SpawnSource)EditorGUILayout.EnumPopup("Damage Prefab Type", _killable.damagePrefabSource);
                    if (newDmgSource != _killable.damagePrefabSource)
                    {
                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _killable, "change Damage Prefab Type");
                        _killable.damagePrefabSource = newDmgSource;
                    }
                    switch (_killable.damagePrefabSource)
                    {
                        case Killable.SpawnSource.PrefabPool:
                            if (poolNames != null)
                            {
                                var pool = LevelSettings.GetFirstMatchingPrefabPool(_killable.damagePrefabPoolName);
                                var noDmgPool = false;
                                var invalidDmgPool = false;
                                var noPrefabPools = false;

                                if (pool == null)
                                {
                                    if (string.IsNullOrEmpty(_killable.damagePrefabPoolName))
                                    {
                                        noDmgPool = true;
                                    }
                                    else
                                    {
                                        invalidDmgPool = true;
                                    }
                                    _killable.damagePrefabPoolIndex = 0;
                                }
                                else
                                {
                                    _killable.damagePrefabPoolIndex = poolNames.IndexOf(_killable.damagePrefabPoolName);
                                }

                                if (poolNames.Count > 1)
                                {
                                    EditorGUILayout.BeginHorizontal();
                                    var newPoolIndex = EditorGUILayout.Popup("Damage Prefab Pool", _killable.damagePrefabPoolIndex, poolNames.ToArray());
                                    if (newPoolIndex != _killable.damagePrefabPoolIndex)
                                    {
                                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _killable, "change Damage Prefab Pool");
                                        _killable.damagePrefabPoolIndex = newPoolIndex;
                                    }

                                    if (_killable.damagePrefabPoolIndex > 0)
                                    {
                                        var matchingPool = LevelSettings.GetFirstMatchingPrefabPool(poolNames[_killable.damagePrefabPoolIndex]);
                                        if (matchingPool != null)
                                        {
                                            _killable.damagePrefabPoolName = matchingPool.name;
                                        }
                                    }
                                    else
                                    {
                                        _killable.damagePrefabPoolName = string.Empty;
                                    }

                                    if (newPoolIndex > 0)
                                    {
                                        if (DTInspectorUtility.AddControlButtons("Prefab Pool") ==
                                            DTInspectorUtility.FunctionButtons.Edit)
                                        {
                                            Selection.activeGameObject = pool.gameObject;
                                        }
                                    }
                                    EditorGUILayout.EndHorizontal();
                                }
                                else
                                {
                                    noPrefabPools = true;
                                }

                                if (noPrefabPools)
                                {
                                    DTInspectorUtility.ShowRedErrorBox("You have no Prefab Pools. Create one first.");
                                }
                                else if (noDmgPool)
                                {
                                    DTInspectorUtility.ShowRedErrorBox("No Damage Prefab Pool selected.");
                                }
                                else if (invalidDmgPool)
                                {
                                    DTInspectorUtility.ShowRedErrorBox("Damage Prefab Pool '" + _killable.damagePrefabPoolName + "' not found. Select one.");
                                }
                            }
                            else
                            {
                                DTInspectorUtility.ShowRedErrorBox(LevelSettings.NoPrefabPoolsContainerAlert);
                                DTInspectorUtility.ShowRedErrorBox(LevelSettings.RevertLevelSettingsAlert);
                            }

                            break;
                        case Killable.SpawnSource.Specific:
                            PoolBossEditorUtility.DisplayPrefab(ref _isDirty, _killable, ref _killable.damagePrefabSpecific, ref _killable.damagePrefabCategoryName,
                                ref _killable.damagePrefabPoolItemName,
                                ref _killable.shouldDamagePrefabSelectFromPB, ref _killable.damagePrefabFrom, false, "Damage Prefab"
#if ADDRESSABLES_ENABLED
    , ref _killable.damagePrefabAddressable
    , serializedObject
    , serializedObject.FindProperty(nameof(Killable.damagePrefabAddressable))
#endif
                            );

                            break;
                    }

                    if (_killable.damagePrefabSource != Killable.SpawnSource.None)
                    {
                        var newOffset = EditorGUILayout.Vector3Field("Spawn Offset", _killable.damagePrefabOffset);
                        if (newOffset != _killable.damagePrefabOffset)
                        {
                            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _killable, "change Damage Prefab Spawn Offset");
                            _killable.damagePrefabOffset = newOffset;
                        }

                        newOffset = EditorGUILayout.Vector3Field("Incremental Offset", _killable.damagePrefabIncrementalOffset);
                        if (newOffset != _killable.damagePrefabIncrementalOffset)
                        {
                            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _killable, "change Incremental Offset");
                            _killable.damagePrefabIncrementalOffset = newOffset;
                        }

                        EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(100));
                        EditorGUILayout.LabelField("Random Rotation");

                        var newRandomX = GUILayout.Toggle(_killable.damagePrefabRandomizeXRotation, "X");
                        if (newRandomX != _killable.damagePrefabRandomizeXRotation)
                        {
                            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _killable, "toggle Random X Rotation");
                            _killable.damagePrefabRandomizeXRotation = newRandomX;
                        }
                        GUILayout.Space(10);
                        var newRandomY = GUILayout.Toggle(_killable.damagePrefabRandomizeYRotation, "Y");
                        if (newRandomY != _killable.damagePrefabRandomizeYRotation)
                        {
                            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _killable, "toggle Random Y Rotation");
                            _killable.damagePrefabRandomizeYRotation = newRandomY;
                        }
                        GUILayout.Space(10);
                        var newRandomZ = GUILayout.Toggle(_killable.damagePrefabRandomizeZRotation, "Z");
                        if (newRandomZ != _killable.damagePrefabRandomizeZRotation)
                        {
                            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _killable, "toggle Random Z Rotation");
                            _killable.damagePrefabRandomizeZRotation = newRandomZ;
                        }
                        EditorGUILayout.EndHorizontal();
                    }

                    var newLast = EditorGUILayout.Toggle("Spawn/Fire on Death Hit", _killable.damagePrefabOnDeathHit);
                    if (newLast != _killable.damagePrefabOnDeathHit)
                    {
                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _killable, "toggle Spawn/Fire on Death Hit");
                        _killable.damagePrefabOnDeathHit = newLast;
                    }

                    DTInspectorUtility.StartGroupHeader(0, false);
                    var newExp = EditorGUILayout.Toggle("Damage Cust. Events", _killable.damageFireEvents);
                    if (newExp != _killable.damageFireEvents)
                    {
                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _killable, "toggle Damage Cust. Events");
                        _killable.damageFireEvents = newExp;
                    }

                    if (_killable.damageFireEvents)
                    {
                        DTInspectorUtility.ShowColorWarningBox("When damage would happen (even if invincible), fire the Custom Events below");

                        EditorGUILayout.BeginHorizontal();
                        GUI.contentColor = DTInspectorUtility.AddButtonColor;
                        GUILayout.Space(2);
                        if (GUILayout.Button(new GUIContent("Add", "Click to add a Custom Event"), EditorStyles.toolbarButton, GUILayout.Width(50)))
                        {
                            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _killable, "Add Damage Custom Event");
                            _killable.damageCustomEvents.Add(new CGKCustomEventToFire());
                        }
                        GUI.contentColor = Color.white;

                        EditorGUILayout.EndHorizontal();

                        if (_killable.damageCustomEvents.Count == 0)
                        {
                            DTInspectorUtility.ShowColorWarningBox("You have no Custom Events selected to fire.");
                        }

                        DTInspectorUtility.VerticalSpace(2);

                        int? indexToDelete = null;

                        // ReSharper disable once ForCanBeConvertedToForeach
                        for (var i = 0; i < _killable.damageCustomEvents.Count; i++)
                        {
                            var anEvent = _killable.damageCustomEvents[i].CustomEventName;

                            var buttonClicked = DTInspectorUtility.FunctionButtons.None;
                            anEvent = DTInspectorUtility.SelectCustomEventForVariable(ref _isDirty, anEvent, _killable, "Custom Event", ref buttonClicked);

                            if (buttonClicked == DTInspectorUtility.FunctionButtons.Remove)
                            {
                                indexToDelete = i;
                            }

                            if (anEvent == _killable.damageCustomEvents[i].CustomEventName)
                            {
                                continue;
                            }

                            _killable.damageCustomEvents[i].CustomEventName = anEvent;
                        }

                        if (indexToDelete.HasValue)
                        {
                            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _killable, "Remove Damage Custom Event");
                            _killable.damageCustomEvents.RemoveAt(indexToDelete.Value);
                        }
                    }
                    EditorGUILayout.EndVertical();
                }
                else
                {
                    DTInspectorUtility.ShowColorWarningBox("Change Spawn Frequency to show more settings.");
                }

                DTInspectorUtility.EndGroupedControls();
            }

            // knockback section
            DTInspectorUtility.VerticalSpace(2);
            EditorGUI.indentLevel = 0;

            state = _killable.damageKnockBackExpanded;
            text = "Damage Knockback Settings";

            DTInspectorUtility.ShowCollapsibleSection(ref state, text);

            if (state != _killable.damageKnockBackExpanded)
            {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _killable, "toggle expand Damage Knockback Settings");
                _killable.damageKnockBackExpanded = state;
            }

            DTInspectorUtility.AddHelpIconNoStyle("http://www.dtdevtools.com/docs/coregamekit/Killables.htm#Knockback");

            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel = 0;
            if (_killable.damageKnockBackExpanded)
            {
                DTInspectorUtility.BeginGroupedControls();
                DTInspectorUtility.StartGroupHeader();
                var use = GUILayout.Toggle(_killable.sendDamageKnockback, " Send Knockback");
                if (use != _killable.sendDamageKnockback)
                {
                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _killable, "toggle Send Knockback");
                    _killable.sendDamageKnockback = use;
                }
                EditorGUILayout.EndVertical();

                if (_killable.sendDamageKnockback)
                {
#if MASTERAUDIO_ENABLED
                CGKMAHelper.SoundGroupField(_maInScene, _groupNames, "Knockback Sound", _killable, ref _isDirty, ref _killable.knockbackSound);
#endif

                    KillerVariablesHelper.DisplayKillerFloat(ref _isDirty, _killable.damageKnockBackFactor,
                        "Knock Back Force",
                        _killable);
                    KillerVariablesHelper.DisplayKillerFloat(ref _isDirty, _killable.damageKnockUpMeters, "Knock Up Force",
                        _killable);
                }
                EditorGUILayout.EndVertical();

                if (_killable.CanReceiveKnockback)
                {
                    DTInspectorUtility.VerticalSpace(3);

                    use = GUILayout.Toggle(_killable.receiveKnockbackWhenDamaged, " Receive Knockback When Damaged");
                    if (use != _killable.receiveKnockbackWhenDamaged)
                    {
                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _killable, "toggle Receive Knockback When Damaged");
                        _killable.receiveKnockbackWhenDamaged = use;
                    }
                    use = GUILayout.Toggle(_killable.receiveKnockbackWhenInvince, " Receive Knockback When Invincible");
                    if (use != _killable.receiveKnockbackWhenInvince)
                    {
                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _killable, "toggle Receive Knockback When Invincible");
                        _killable.receiveKnockbackWhenInvince = use;
                    }
                }
                else
                {
                    DTInspectorUtility.ShowColorWarningBox("Cannot receive knockback unless using a gravity Rigidbody or you have a CharacterController.");
                }

                DTInspectorUtility.EndGroupedControls();
            }


            // player stat damage modifiers
            EditorGUI.indentLevel = 0;
            DTInspectorUtility.VerticalSpace(2);

            state = _killable.despawnStatDamageModifiersExpanded;
            text = "Damage World Variable Modifiers";

            DTInspectorUtility.ShowCollapsibleSection(ref state, text);

            if (state != _killable.despawnStatDamageModifiersExpanded)
            {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _killable, "toggle expand Damage World Variable Modifiers");
                _killable.despawnStatDamageModifiersExpanded = state;
            }

            DTInspectorUtility.AddHelpIconNoStyle("http://www.dtdevtools.com/docs/coregamekit/Killables.htm#DamageVars");

            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel = 0;
            if (_killable.despawnStatDamageModifiersExpanded)
            {
                DTInspectorUtility.BeginGroupedControls();
                var missingStatNames = new List<string>();
                missingStatNames.AddRange(allStats);
                missingStatNames.RemoveAll(delegate (string obj)
                {
                    return _killable.playerStatDamageModifiers.HasKey(obj);
                });

                var newStat = EditorGUILayout.Popup("Add Variable Modifer", 0, missingStatNames.ToArray());
                if (newStat != 0)
                {
                    AddStatModifier(missingStatNames[newStat], _killable.playerStatDamageModifiers);
                }

                if (_killable.playerStatDamageModifiers.statMods.Count == 0)
                {
                    DTInspectorUtility.ShowColorWarningBox("You currently have no damage modifiers for this prefab.");
                }
                else
                {
                    EditorGUILayout.Separator();

                    int? indexToDelete = null;

                    for (var i = 0; i < _killable.playerStatDamageModifiers.statMods.Count; i++)
                    {
                        var modifier = _killable.playerStatDamageModifiers.statMods[i];

                        var buttonPressed = DTInspectorUtility.FunctionButtons.None;
                        switch (modifier._varTypeToUse)
                        {
                            case WorldVariableTracker.VariableType._integer:
                                buttonPressed = KillerVariablesHelper.DisplayKillerInt(ref _isDirty, modifier._modValueIntAmt, modifier._statName, _killable, true, true);
                                break;
                            case WorldVariableTracker.VariableType._float:
                                buttonPressed = KillerVariablesHelper.DisplayKillerFloat(ref _isDirty, modifier._modValueFloatAmt, modifier._statName, _killable, true, true);
                                break;
                            default:
                                Debug.LogError("Add code for varType: " + modifier._varTypeToUse.ToString());
                                break;
                        }

                        KillerVariablesHelper.ShowErrorIfMissingVariable(modifier._statName);

                        if (buttonPressed == DTInspectorUtility.FunctionButtons.Remove)
                        {
                            indexToDelete = i;
                        }
                    }

                    if (indexToDelete.HasValue)
                    {
                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _killable, "delete Modifier");
                        _killable.playerStatDamageModifiers.DeleteByIndex(indexToDelete.Value);
                    }
                }

                DTInspectorUtility.EndGroupedControls();
            }

            // despawn trigger section
            EditorGUI.indentLevel = 0;
            DTInspectorUtility.VerticalSpace(2);

            state = _killable.showVisibilitySettings;
            text = "Despawn & Death Triggers";

            DTInspectorUtility.ShowCollapsibleSection(ref state, text);

            if (state != _killable.showVisibilitySettings)
            {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _killable, "toggle expand Despawn Triggers");
                _killable.showVisibilitySettings = state;
            }

            DTInspectorUtility.AddHelpIconNoStyle("http://www.dtdevtools.com/docs/coregamekit/Killables.htm#DeathTriggers");

            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel = 0;
            if (_killable.showVisibilitySettings)
            {
                DTInspectorUtility.BeginGroupedControls();
                var newSpawnerDest = (Killable.SpawnerDestroyedBehavior)EditorGUILayout.EnumPopup("If Spawner Killed? ", _killable.spawnerDestroyedAction);
                if (newSpawnerDest != _killable.spawnerDestroyedAction)
                {
					UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _killable, "change If Spawner Killed");
                    _killable.spawnerDestroyedAction = newSpawnerDest;
                }

                DTInspectorUtility.StartGroupHeader();
				var newDieWhenParent = (Killable.SpawnerDestroyedBehavior)EditorGUILayout.EnumPopup("If Parent Killed?", _killable.parentDestroyedAction);
                if (newDieWhenParent != _killable.parentDestroyedAction)
                {
					UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _killable, "change If Parent Killed");
                    _killable.parentDestroyedAction = newDieWhenParent;
                }

                EditorGUILayout.EndVertical();
                if (_killable.parentDestroyedAction != Killable.SpawnerDestroyedBehavior.DoNothing)
                {
                    if (_killable.parentKillableForParentDestroyed == null)
                    {
                        var par = _killable.Trans.parent;
                        Killable parKill = null;
                        if (par != null)
                        {
                            parKill = par.GetComponent<Killable>();
                        }

                        if (parKill != null)
                        {
                            _killable.parentKillableForParentDestroyed = parKill;
                        }
                    }

                    var newParent = (Killable)EditorGUILayout.ObjectField("Parent Killable", _killable.parentKillableForParentDestroyed,
                        typeof(Killable), false);
                    if (newParent != _killable.parentKillableForParentDestroyed)
                    {
                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _killable, "change Parent Killable");
                        _killable.parentKillableForParentDestroyed = newParent;
                    }
                }

                EditorGUILayout.EndVertical();

                DTInspectorUtility.StartGroupHeader();
                var newTimer = EditorGUILayout.Toggle("Use Death Timer", _killable.timerDeathEnabled);
                if (newTimer != _killable.timerDeathEnabled)
                {
                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _killable, "toggle Use Death Timer");
                    _killable.timerDeathEnabled = newTimer;
                }
                EditorGUILayout.EndVertical();

                if (_killable.timerDeathEnabled)
                {
                    EditorGUI.indentLevel = 0;
                    KillerVariablesHelper.DisplayKillerFloat(ref _isDirty, _killable.timerDeathSeconds, "Death Timer (sec)", _killable);
                    var newTimerAction = (Killable.SpawnerDestroyedBehavior)EditorGUILayout.EnumPopup("Time Up Action", _killable.timeUpAction);
                    if (newTimerAction != _killable.timeUpAction)
                    {
                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _killable, "change Time Up Action");
                        _killable.timeUpAction = newTimerAction;
                    }
                }
                EditorGUILayout.EndVertical();

                DTInspectorUtility.StartGroupHeader();
                var newDist = EditorGUILayout.Toggle("Use Death Distance", _killable.distanceDeathEnabled);
                if (newDist != _killable.distanceDeathEnabled)
                {
                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _killable, "toggle Use Death Distance");
                    _killable.distanceDeathEnabled = newDist;
                }
                EditorGUILayout.EndVertical();

                if (_killable.distanceDeathEnabled)
                {
                    EditorGUI.indentLevel = 0;
                    KillerVariablesHelper.DisplayKillerFloat(ref _isDirty, _killable.tooFarDistance, "Death Distance", _killable);
                    var newDeathAction = (Killable.SpawnerDestroyedBehavior)EditorGUILayout.EnumPopup("Distance Passed Action", _killable.distanceDeathAction);
                    if (newDeathAction != _killable.distanceDeathAction)
                    {
                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _killable, "change Distance Passed Action");
                        _killable.distanceDeathAction = newDeathAction;
                    }
                }
                EditorGUILayout.EndVertical();


                DTInspectorUtility.StartGroupHeader();
                var newVelocity = EditorGUILayout.Toggle("Use Death / Damage Velocity", _killable.useDeathVelocity);
                if (newVelocity != _killable.useDeathVelocity)
                {
                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _killable, "toggle Use Death / Damage Velocity");
                    _killable.useDeathVelocity = newVelocity;
                }
                EditorGUILayout.EndVertical();

                if (_killable.useDeathVelocity)
                {
                    EditorGUI.indentLevel = 0;
                    if (!_killable.IsGravBody)
                    {
                        DTInspectorUtility.ShowRedErrorBox("Only gravity Rigidbody Game Objects can use this setting.");
                    }
                    else
                    {
                        KillerVariablesHelper.DisplayKillerFloat(ref _isDirty, _killable.deathVelocity, "Death / Damage Velocity", _killable);
                        var newVelCheck = (Killable.DeathVelocityCheckEvent)EditorGUILayout.EnumPopup(new GUIContent("Velocity Check Event", "This specifies when the velocity check occurs"), _killable.deathVelocityCheckEvent);
                        if (newVelCheck != _killable.deathVelocityCheckEvent)
                        {
                            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _killable, "change Velocity Check Event");
                            _killable.deathVelocityCheckEvent = newVelCheck;
                        }

                        switch (_killable.deathVelocityCheckEvent)
                        {
                            case Killable.DeathVelocityCheckEvent.Continuous:
                            case Killable.DeathVelocityCheckEvent.Collision:
                                DTInspectorUtility.StartGroupHeader(1);
                                var newRetrigger = (TriggeredSpawner.RetriggerLimitMode)EditorGUILayout.EnumPopup("Retrigger Limit Mode", _killable.deathVelocityRetriggerLimitMode);
                                if (newRetrigger != _killable.deathVelocityRetriggerLimitMode)
                                {
                                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _killable, "change Retrigger Limit Mode");
                                    _killable.deathVelocityRetriggerLimitMode = newRetrigger;
                                }
                                EditorGUILayout.EndVertical();

                                switch (_killable.deathVelocityRetriggerLimitMode)
                                {
                                    case TriggeredSpawner.RetriggerLimitMode.FrameBased:
                                        KillerVariablesHelper.DisplayKillerInt(ref _isDirty, _killable.deathVelLimitPerXFrm, "Min Frames Between", _killable);
                                        break;
                                    case TriggeredSpawner.RetriggerLimitMode.TimeBased:
                                        KillerVariablesHelper.DisplayKillerFloat(ref _isDirty, _killable.deathVelLimitPerXSec, "Min Seconds Between", _killable);
                                        break;
                                }
                                EditorGUILayout.EndVertical();

                                break;
                            default:
                                Debug.LogError("Unhandled velocity check event: " + _killable.deathVelocityCheckEvent);
                                break;
                        }

                        var newVelMode = (Killable.DeathVelocityMode)EditorGUILayout.EnumPopup("Death Velocity Mode", _killable.deathVelocityMode);
                        if (newVelMode != _killable.deathVelocityMode)
                        {
                            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _killable, "change Death Velocity Mode");
                            _killable.deathVelocityMode = newVelMode;
                        }

                        switch (_killable.deathVelocityMode)
                        {
                            case Killable.DeathVelocityMode.Damage:
                                var newVelCalcMode = (Killable.DeathVelocityDamageCalcMode)EditorGUILayout.EnumPopup("Damage Calc. Mode", _killable.deathVelocityDamageCalcMode);
                                if (newVelCalcMode != _killable.deathVelocityDamageCalcMode)
                                {
                                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _killable, "change Damage Calc. Mode");
                                    _killable.deathVelocityDamageCalcMode = newVelCalcMode;
                                }

                                KillerVariablesHelper.DisplayKillerFloat(ref _isDirty, _killable.deathVelDamageFixed, "Fixed Damage", _killable);

                                break;
                        }
                    }
                }
                EditorGUILayout.EndVertical();

                EditorGUI.indentLevel = 0;
                DTInspectorUtility.StartGroupHeader();
                EditorGUILayout.LabelField("Despawn Triggers");
                EditorGUILayout.EndVertical();

                EditorGUI.indentLevel = 0;
                var newOffscreen = EditorGUILayout.Toggle("Invisible Event", _killable.despawnWhenOffscreen);
                if (newOffscreen != _killable.despawnWhenOffscreen)
                {
                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _killable, "toggle Invisible Event");
                    _killable.despawnWhenOffscreen = newOffscreen;
                }

                var newNotVisible = EditorGUILayout.Toggle("Not Visible Too Long", _killable.despawnIfNotVisible);
                if (newNotVisible != _killable.despawnIfNotVisible)
                {
                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _killable, "toggle Not Visible Too Long");
                    _killable.despawnIfNotVisible = newNotVisible;
                }

                if (_killable.despawnIfNotVisible)
                {
                    KillerVariablesHelper.DisplayKillerFloat(ref _isDirty, _killable.despawnIfNotVisibleForSec, "Not Visible Max Time", _killable);
                }

#if MASTERAUDIO_ENABLED
            CGKMAHelper.SoundGroupField(_maInScene, _groupNames, "Vanish Sound", _killable, ref _isDirty, ref _killable.vanishSound);
#endif

                var newMode = (Killable.SpawnSource)EditorGUILayout.EnumPopup(new GUIContent("Vanish Prefab Type", "This will spawn when the Killable is only despawned and not destroyed."), _killable.vanishPrefabSource);
                if (newMode != _killable.vanishPrefabSource)
                {
                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _killable, "change Vanish Prefab Type");
                    _killable.vanishPrefabSource = newMode;
                }

                var isValid = true;

                switch (newMode)
                {
                    case Killable.SpawnSource.PrefabPool:
                        if (poolNames != null)
                        {
                            var pool = LevelSettings.GetFirstMatchingPrefabPool(_killable.vanishPrefabPoolName);
                            var noVanishPool = false;
                            var invalidVanishPool = false;
                            var noPrefabPools = false;

                            if (pool == null)
                            {
                                if (string.IsNullOrEmpty(_killable.vanishPrefabPoolName))
                                {
                                    noVanishPool = true;
                                }
                                else
                                {
                                    invalidVanishPool = true;
                                }
                                _killable.vanishPrefabPoolIndex = 0;
                            }
                            else
                            {
                                _killable.vanishPrefabPoolIndex = poolNames.IndexOf(_killable.vanishPrefabPoolName);
                            }

                            if (poolNames.Count > 1)
                            {
                                EditorGUILayout.BeginHorizontal();
                                var newPoolIndex = EditorGUILayout.Popup("Vanish Prefab Pool", _killable.vanishPrefabPoolIndex, poolNames.ToArray());
                                if (newPoolIndex != _killable.vanishPrefabPoolIndex)
                                {
                                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _killable, "change Vanish Prefab Pool");
                                    _killable.vanishPrefabPoolIndex = newPoolIndex;
                                }

                                if (_killable.vanishPrefabPoolIndex > 0)
                                {
                                    var matchingPool = LevelSettings.GetFirstMatchingPrefabPool(poolNames[_killable.vanishPrefabPoolIndex]);
                                    if (matchingPool != null)
                                    {
                                        _killable.vanishPrefabPoolName = matchingPool.name;
                                    }
                                }
                                else
                                {
                                    _killable.vanishPrefabPoolName = string.Empty;
                                }

                                if (newPoolIndex > 0)
                                {
                                    if (DTInspectorUtility.AddControlButtons("Prefab Pool") ==
                                        DTInspectorUtility.FunctionButtons.Edit)
                                    {
                                        Selection.activeGameObject = pool.gameObject;
                                    }
                                }
                                EditorGUILayout.EndHorizontal();
                            }
                            else
                            {
                                noPrefabPools = true;
                            }

                            if (noPrefabPools)
                            {
                                DTInspectorUtility.ShowRedErrorBox("You have no Prefab Pools. Create one first.");
                                isValid = false;
                            }
                            else if (noVanishPool)
                            {
                                DTInspectorUtility.ShowRedErrorBox("No Vanish Prefab Pool selected.");
                                isValid = false;
                            }
                            else if (invalidVanishPool)
                            {
                                DTInspectorUtility.ShowRedErrorBox("Vanish Prefab Pool '" + _killable.vanishPrefabPoolName + "' not found. Select one.");
                                isValid = false;
                            }
                        }
                        else
                        {
                            DTInspectorUtility.ShowRedErrorBox(LevelSettings.NoPrefabPoolsContainerAlert);
                            DTInspectorUtility.ShowRedErrorBox(LevelSettings.RevertLevelSettingsAlert);
                            isValid = false;
                        }

                        break;
                    case Killable.SpawnSource.Specific:
                        PoolBossEditorUtility.DisplayPrefab(ref _isDirty, _killable, ref _killable.vanishPrefabSpecific, ref _killable.vanishPrefabCategoryName,
                            ref _killable.vanishPrefabPoolItemName,
                            ref _killable.shouldVanishPrefabSelectFromPB, ref _killable.vanishPrefabFrom, false, "Vanish Prefab"
#if ADDRESSABLES_ENABLED
    , ref _killable.vanishPrefabAddressable
    , serializedObject
    , serializedObject.FindProperty(nameof(Killable.vanishPrefabAddressable))
#endif
                        );

                        if (_killable.vanishPrefabSpecific == null)
                        {
                            isValid = false;
                        }
                        break;
                    case Killable.SpawnSource.None:
                        isValid = false;
                        break;
                }

                if (isValid)
                {
                    EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(100));
                    EditorGUILayout.LabelField("Vanish Random Rotation");

                    var newRandomX = GUILayout.Toggle(_killable.vanishPrefabRandomizeXRotation, "X");
                    if (newRandomX != _killable.vanishPrefabRandomizeXRotation)
                    {
                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _killable, "toggle Random X Rotation");
                        _killable.vanishPrefabRandomizeXRotation = newRandomX;
                    }
                    GUILayout.Space(10);
                    var newRandomY = GUILayout.Toggle(_killable.vanishPrefabRandomizeYRotation, "Y");
                    if (newRandomY != _killable.vanishPrefabRandomizeYRotation)
                    {
                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _killable, "toggle Random Y Rotation");
                        _killable.vanishPrefabRandomizeYRotation = newRandomY;
                    }
                    GUILayout.Space(10);
                    var newRandomZ = GUILayout.Toggle(_killable.vanishPrefabRandomizeZRotation, "Z");
                    if (newRandomZ != _killable.vanishPrefabRandomizeZRotation)
                    {
                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _killable, "toggle Random Z Rotation");
                        _killable.vanishPrefabRandomizeZRotation = newRandomZ;
                    }

                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.EndVertical();

                EditorGUI.indentLevel = 0;
                DTInspectorUtility.StartGroupHeader();
                EditorGUILayout.LabelField("Death Triggers");
                EditorGUILayout.EndVertical();

                EditorGUI.indentLevel = 0;
                var newClick = EditorGUILayout.Toggle("MouseDown Event", _killable.despawnOnMouseClick);
                if (newClick != _killable.despawnOnMouseClick)
                {
                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _killable, "toggle MouseDown Event");
                    _killable.despawnOnMouseClick = newClick;
                }

                newClick = EditorGUILayout.Toggle("OnClick Event (NGUI)", _killable.despawnOnClick);
                if (newClick != _killable.despawnOnClick)
                {
                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _killable, "toggle OnClick Event (NGUI)");
                    _killable.despawnOnClick = newClick;
                }

                var newDespawn = (Killable.DespawnMode)EditorGUILayout.EnumPopup("HP Death Mode", _killable.despawnMode);
                if (newDespawn != _killable.despawnMode)
                {
                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _killable, "change HP Death Mode");
                    _killable.despawnMode = newDespawn;
                }

                if (_killable.despawnMode == Killable.DespawnMode.CollisionOrTrigger)
                {
                    var newInc = EditorGUILayout.Toggle("Allow Non-Killable Hits", _killable.includeNonKillables);
                    if (newInc != _killable.includeNonKillables)
                    {
                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _killable, "toggle Allow Non-Killable Hits");
                        _killable.includeNonKillables = newInc;
                    }
                }

                EditorGUILayout.EndVertical();
                DTInspectorUtility.EndGroupedControls();
            }

            // death prefab section
            EditorGUI.indentLevel = 0;
            DTInspectorUtility.VerticalSpace(2);

            state = _killable.deathPrefabSettingsExpanded;
            text = "Death Prefab Settings & Events";

            DTInspectorUtility.ShowCollapsibleSection(ref state, text);

            KillableDeathPrefab deathPrefabToDelete = null;

            if (state != _killable.deathPrefabSettingsExpanded)
            {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _killable, "toggle expand Death Prefab Settings & Events");
                _killable.deathPrefabSettingsExpanded = state;
            }
            DTInspectorUtility.AddHelpIconNoStyle("http://www.dtdevtools.com/docs/coregamekit/Killables.htm#DeathPrefab");
            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel = 0;
            if (_killable.deathPrefabSettingsExpanded)
            {
                DTInspectorUtility.BeginGroupedControls();
                KillerVariablesHelper.DisplayKillerFloat(ref _isDirty, _killable.deathDelay, "Death Delay (sec)", _killable);

#if MASTERAUDIO_ENABLED
            CGKMAHelper.SoundGroupField(_maInScene, _groupNames, "Death Sound", _killable, ref _isDirty, ref _killable.deathSound);
#endif

                DTInspectorUtility.StartGroupHeader(1, true);

                EditorGUI.indentLevel = 1;
                EditorGUILayout.BeginHorizontal();
                var newExpDP = DTInspectorUtility.Foldout(_killable.deathPrefabExpanded, "Death Prefab #1");

                if (newExpDP != _killable.deathPrefabExpanded)
                {
                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _killable, "toggle expand Death Prefab #1");
                    _killable.deathPrefabExpanded = newExpDP;
                }

                if (!Application.isPlaying)
                {
                    if (GUILayout.Button("Add Death Prefab", EditorStyles.toolbarButton, GUILayout.Width(110)))
                    {
                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _killable.gameObject, "add Death Prefab");
                        _killable.gameObject.AddComponent<KillableDeathPrefab>();
                        _isDirty = true;
                    }
                }

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
                EditorGUI.indentLevel = 0;

                var hasDeathPrefab = true;
                if (_killable.deathPrefabExpanded)
                {
                    var newDeathSource = (WaveSpecifics.SpawnOrigin)EditorGUILayout.EnumPopup("Death Prefab Type", _killable.deathPrefabSource);
                    if (newDeathSource != _killable.deathPrefabSource)
                    {
                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _killable, "change Death Prefab Type");
                        _killable.deathPrefabSource = newDeathSource;
                    }

                    switch (_killable.deathPrefabSource)
                    {
                        case WaveSpecifics.SpawnOrigin.PrefabPool:
                            if (poolNames != null)
                            {
                                var pool = LevelSettings.GetFirstMatchingPrefabPool(_killable.deathPrefabPoolName);
                                var noDeathPool = false;
                                var illegalDeathPref = false;
                                var noPrefabPools = false;

                                if (pool == null)
                                {
                                    if (string.IsNullOrEmpty(_killable.deathPrefabPoolName))
                                    {
                                        noDeathPool = true;
                                    }
                                    else
                                    {
                                        illegalDeathPref = true;
                                    }
                                    _killable.deathPrefabPoolIndex = 0;
                                }
                                else
                                {
                                    _killable.deathPrefabPoolIndex = poolNames.IndexOf(_killable.deathPrefabPoolName);
                                }

                                if (poolNames.Count > 1)
                                {
                                    EditorGUILayout.BeginHorizontal();
                                    var newDeathPool = EditorGUILayout.Popup("Death Prefab Pool", _killable.deathPrefabPoolIndex, poolNames.ToArray());
                                    if (newDeathPool != _killable.deathPrefabPoolIndex)
                                    {
                                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _killable, "change Death Prefab Pool");
                                        _killable.deathPrefabPoolIndex = newDeathPool;
                                    }

                                    if (_killable.deathPrefabPoolIndex > 0)
                                    {
                                        var matchingPool = LevelSettings.GetFirstMatchingPrefabPool(poolNames[_killable.deathPrefabPoolIndex]);
                                        if (matchingPool != null)
                                        {
                                            _killable.deathPrefabPoolName = matchingPool.name;
                                        }
                                    }
                                    else
                                    {
                                        _killable.deathPrefabPoolName = string.Empty;
                                    }

                                    if (newDeathPool > 0)
                                    {
                                        if (DTInspectorUtility.AddControlButtons("Prefab Pool") ==
                                            DTInspectorUtility.FunctionButtons.Edit)
                                        {
                                            Selection.activeGameObject = pool.gameObject;
                                        }
                                    }
                                    EditorGUILayout.EndHorizontal();

                                }
                                else
                                {
                                    noPrefabPools = true;
                                }

                                if (noPrefabPools)
                                {
                                    DTInspectorUtility.ShowRedErrorBox("You have no Prefab Pools. Create one first.");
                                    hasDeathPrefab = false;
                                }
                                else if (noDeathPool)
                                {
                                    DTInspectorUtility.ShowRedErrorBox("No Death Prefab Pool selected.");
                                    hasDeathPrefab = false;
                                }
                                else if (illegalDeathPref)
                                {
                                    DTInspectorUtility.ShowRedErrorBox("Death Prefab Pool '" + _killable.deathPrefabPoolName + "' not found. Select one.");
                                    hasDeathPrefab = false;
                                }
                            }
                            else
                            {
                                DTInspectorUtility.ShowRedErrorBox(LevelSettings.NoPrefabPoolsContainerAlert);
                                DTInspectorUtility.ShowRedErrorBox(LevelSettings.RevertLevelSettingsAlert);
                                hasDeathPrefab = false;
                            }
                            break;
                        case WaveSpecifics.SpawnOrigin.Specific:
                            PoolBossEditorUtility.DisplayPrefab(ref _isDirty, _killable, ref _killable.deathPrefabSpecific, ref _killable.deathPrefabCategoryName,
                                ref _killable.deathPrefabPoolItemName,
                                ref _killable.shouldDeathPrefabSelectFromPB, ref _killable.deathPrefabFrom, false, "Death Prefab"
#if ADDRESSABLES_ENABLED
    , ref _killable.deathPrefabAddressable
    , serializedObject
    , serializedObject.FindProperty(nameof(Killable.deathPrefabAddressable))
#endif
                            );

                            if (_killable.deathPrefabSpecific == null)
                            {
                                hasDeathPrefab = false;
                            }

                            break;
                    }

                    if (hasDeathPrefab)
                    {
                        var newKeepParent = EditorGUILayout.Toggle("Keep Same Parent", _killable.deathPrefabKeepSameParent);
                        if (newKeepParent != _killable.deathPrefabKeepSameParent)
                        {
                            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _killable, "toggle Keep Same Parent");
                            _killable.deathPrefabKeepSameParent = newKeepParent;
                        }

                        KillerVariablesHelper.DisplayKillerInt(ref _isDirty, _killable.deathPrefabSpawnPercent, "Spawn % Chance", _killable);

                        KillerVariablesHelper.DisplayKillerInt(ref _isDirty, _killable.deathPrefabQty, "Spawn Quantity", _killable);

                        var newSpawnPosition = (Killable.DeathPrefabSpawnLocation)EditorGUILayout.EnumPopup("Spawn Position", _killable.deathPrefabSpawnLocation);
                        if (newSpawnPosition != _killable.deathPrefabSpawnLocation)
                        {
                            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _killable, "change Spawn Position");
                            _killable.deathPrefabSpawnLocation = newSpawnPosition;
                        }

                        var newDeathOffset = EditorGUILayout.Vector3Field("Spawn Offset", _killable.deathPrefabOffset);
                        if (newDeathOffset != _killable.deathPrefabOffset)
                        {
                            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _killable, "change Spawn Offset");
                            _killable.deathPrefabOffset = newDeathOffset;
                        }

                        var newOffset = EditorGUILayout.Vector3Field("Incremental Offset", _killable.deathPrefabIncrementalOffset);
                        if (newOffset != _killable.deathPrefabIncrementalOffset)
                        {
                            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _killable, "change Incremental Offset");
                            _killable.deathPrefabIncrementalOffset = newOffset;
                        }

                        KillerVariablesHelper.DisplayKillerFloat(ref _isDirty, _killable.deathPrefabRandomDistX, "Rand. Offset X", _killable);
                        KillerVariablesHelper.DisplayKillerFloat(ref _isDirty, _killable.deathPrefabRandomDistY, "Rand. Offset Y", _killable);
                        KillerVariablesHelper.DisplayKillerFloat(ref _isDirty, _killable.deathPrefabRandomDistZ, "Rand. Offset Z", _killable);

                        if (!_killable.IsGravBody)
                        {
                            DTInspectorUtility.ShowColorWarningBox("Inherit Velocity can only be used on gravity rigidbodies");
                        }
                        else
                        {
                            var newKeep = EditorGUILayout.Toggle("Inherit Velocity", _killable.deathPrefabKeepVelocity);
                            if (newKeep != _killable.deathPrefabKeepVelocity)
                            {
                                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _killable, "toggle Inherit Velocity");
                                _killable.deathPrefabKeepVelocity = newKeep;
                            }
                        }
                    }

                    var newMode = (Killable.RotationMode)EditorGUILayout.EnumPopup("Rotation Mode", _killable.rotationMode);
                    if (newMode != _killable.rotationMode)
                    {
                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _killable, "change Rotation Mode");
                        _killable.rotationMode = newMode;
                    }
                    if (_killable.rotationMode == Killable.RotationMode.CustomRotation)
                    {
                        var newCustomRot = EditorGUILayout.Vector3Field("Custom Rotation Euler", _killable.deathPrefabCustomRotation);
                        if (newCustomRot != _killable.deathPrefabCustomRotation)
                        {
                            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _killable, "change Custom Rotation Euler");
                            _killable.deathPrefabCustomRotation = newCustomRot;
                        }
                    }

                    EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(100));
                    EditorGUILayout.LabelField("Random Rotation");

                    var newRandomX = GUILayout.Toggle(_killable.deathPrefabRandomizeXRotation, "X");
                    if (newRandomX != _killable.deathPrefabRandomizeXRotation)
                    {
                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _killable, "toggle Random X Rotation");
                        _killable.deathPrefabRandomizeXRotation = newRandomX;
                    }
                    GUILayout.Space(10);
                    var newRandomY = GUILayout.Toggle(_killable.deathPrefabRandomizeYRotation, "Y");
                    if (newRandomY != _killable.deathPrefabRandomizeYRotation)
                    {
                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _killable, "toggle Random Y Rotation");
                        _killable.deathPrefabRandomizeYRotation = newRandomY;
                    }
                    GUILayout.Space(10);
                    var newRandomZ = GUILayout.Toggle(_killable.deathPrefabRandomizeZRotation, "Z");
                    if (newRandomZ != _killable.deathPrefabRandomizeZRotation)
                    {
                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _killable, "toggle Random Z Rotation");
                        _killable.deathPrefabRandomizeZRotation = newRandomZ;
                    }
                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.EndVertical();

                var otherDeathPrefabs = _killable.gameObject.GetComponents<KillableDeathPrefab>();
                if (otherDeathPrefabs.Length > 0)
                {
                    DTInspectorUtility.ShowColorWarningBox("You have " + otherDeathPrefabs.Length + " additional Killable Death Prefab component(s) on this Game Object. They need to be configured on the Inspector for that component.");
                }

                if (hasDeathPrefab)
                {
                    DTInspectorUtility.StartGroupHeader(0, false);
                    var newExp = EditorGUILayout.Toggle("Death Cust. Events", _killable.deathFireEvents);
                    if (newExp != _killable.deathFireEvents)
                    {
                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _killable, "toggle Death Cust. Events");
                        _killable.deathFireEvents = newExp;
                    }

                    if (_killable.deathFireEvents)
                    {
                        DTInspectorUtility.ShowColorWarningBox("When destroyed, fire the Custom Events below");

                        EditorGUILayout.BeginHorizontal();
                        GUI.contentColor = DTInspectorUtility.AddButtonColor;
                        GUILayout.Space(2);
                        if (GUILayout.Button(new GUIContent("Add", "Click to add a Custom Event"), EditorStyles.toolbarButton, GUILayout.Width(50)))
                        {
                            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _killable, "Add Death Custom Event");
                            _killable.deathCustomEvents.Add(new CGKCustomEventToFire());
                        }
                        GUI.contentColor = Color.white;

                        EditorGUILayout.EndHorizontal();

                        if (_killable.deathCustomEvents.Count == 0)
                        {
                            DTInspectorUtility.ShowColorWarningBox("You have no Custom Events selected to fire.");
                        }

                        DTInspectorUtility.VerticalSpace(2);

                        int? indexToDelete = null;

                        // ReSharper disable once ForCanBeConvertedToForeach
                        for (var i = 0; i < _killable.deathCustomEvents.Count; i++)
                        {
                            var anEvent = _killable.deathCustomEvents[i].CustomEventName;

                            var buttonClicked = DTInspectorUtility.FunctionButtons.None;
                            anEvent = DTInspectorUtility.SelectCustomEventForVariable(ref _isDirty, anEvent, _killable, "Custom Event", ref buttonClicked);

                            if (buttonClicked == DTInspectorUtility.FunctionButtons.Remove)
                            {
                                indexToDelete = i;
                            }

                            if (anEvent == _killable.deathCustomEvents[i].CustomEventName)
                            {
                                continue;
                            }

                            _killable.deathCustomEvents[i].CustomEventName = anEvent;
                        }

                        if (indexToDelete.HasValue)
                        {
                            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _killable, "Remove Death Custom Event");
                            _killable.deathCustomEvents.RemoveAt(indexToDelete.Value);
                        }
                    }
                    EditorGUILayout.EndVertical();
                }

                DTInspectorUtility.EndGroupedControls();
            }

            // player stat modifiers
            EditorGUI.indentLevel = 0;

            DTInspectorUtility.VerticalSpace(2);

            state = _killable.despawnStatModifiersExpanded;
            text = "Death World Variable Modifier Scenarios";

            DTInspectorUtility.ShowCollapsibleSection(ref state, text);

            if (state != _killable.despawnStatModifiersExpanded)
            {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _killable, "toggle expand Death World Variable Modifier Scenarios");
                _killable.despawnStatModifiersExpanded = state;
            }
            DTInspectorUtility.AddHelpIconNoStyle("http://www.dtdevtools.com/docs/coregamekit/Killables.htm#DeathVars");
            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel = 0;
            if (_killable.despawnStatModifiersExpanded)
            {
                DTInspectorUtility.BeginGroupedControls();
                DTInspectorUtility.StartGroupHeader(1, false);

                EditorGUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
                EditorGUILayout.LabelField("If \"" + Killable.DestroyedText + "\"");
                GUI.backgroundColor = DTInspectorUtility.AddButtonColor;
                if (GUILayout.Button(new GUIContent("Add Else"), EditorStyles.miniButtonMid, GUILayout.MaxWidth(80)))
                {
                    AddModifierElse(_killable);
                }
                GUI.backgroundColor = Color.white;
                EditorGUILayout.EndHorizontal();

                EditorGUI.indentLevel = 0;

                var missingStatNames = new List<string>();
                missingStatNames.AddRange(allStats);
                missingStatNames.RemoveAll(delegate (string obj)
                {
                    return _killable.playerStatDespawnModifiers.HasKey(obj);
                });

                var newStat = EditorGUILayout.Popup("Add Variable Modifer", 0, missingStatNames.ToArray());
                if (newStat != 0)
                {
                    AddStatModifier(missingStatNames[newStat], _killable.playerStatDespawnModifiers);
                }

                if (_killable.playerStatDespawnModifiers.statMods.Count == 0)
                {
                    DTInspectorUtility.ShowColorWarningBox("You currently have no death modifiers for this prefab.");
                }
                else
                {
                    EditorGUILayout.Separator();

                    int? indexToDelete = null;

                    for (var i = 0; i < _killable.playerStatDespawnModifiers.statMods.Count; i++)
                    {
                        var modifier = _killable.playerStatDespawnModifiers.statMods[i];

                        var buttonPressed = DTInspectorUtility.FunctionButtons.None;
                        switch (modifier._varTypeToUse)
                        {
                            case WorldVariableTracker.VariableType._integer:
                                buttonPressed = KillerVariablesHelper.DisplayKillerInt(ref _isDirty, modifier._modValueIntAmt, modifier._statName, _killable, true, true);
                                break;
                            case WorldVariableTracker.VariableType._float:
                                buttonPressed = KillerVariablesHelper.DisplayKillerFloat(ref _isDirty, modifier._modValueFloatAmt, modifier._statName, _killable, true, true);
                                break;
                            default:
                                Debug.LogError("Add code for varType: " + modifier._varTypeToUse.ToString());
                                break;
                        }

                        KillerVariablesHelper.ShowErrorIfMissingVariable(modifier._statName);

                        if (buttonPressed == DTInspectorUtility.FunctionButtons.Remove)
                        {
                            indexToDelete = i;
                        }
                    }

                    if (indexToDelete.HasValue)
                    {
                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _killable, "delete Modifier");
                        _killable.playerStatDespawnModifiers.DeleteByIndex(indexToDelete.Value);
                    }

                    EditorGUILayout.Separator();
                }
                EditorGUILayout.EndVertical();

                // alternate cases
                int? iElseToDelete = null;
                for (var i = 0; i < _killable.alternateModifiers.Count; i++)
                {
                    var alternate = _killable.alternateModifiers[i];

                    EditorGUI.indentLevel = 0;
                    DTInspectorUtility.StartGroupHeader(1, false);
                    EditorGUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
                    GUILayout.Label("Else If", GUILayout.Width(40));
                    var newScen = EditorGUILayout.TextField(alternate.scenarioName, GUILayout.MaxWidth(150));
                    if (newScen != alternate.scenarioName)
                    {
                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _killable, "change Scenario name");
                        alternate.scenarioName = newScen;
                    }
                    GUILayout.FlexibleSpace();
                    GUI.backgroundColor = DTInspectorUtility.DeleteButtonColor;
                    if (GUILayout.Button(new GUIContent("Delete Else"), EditorStyles.miniButton, GUILayout.MaxWidth(80)))
                    {
                        iElseToDelete = i;
                    }
                    GUI.backgroundColor = Color.white;
                    EditorGUILayout.EndHorizontal();

                    EditorGUI.indentLevel = 0;
                    // display modifers

                    missingStatNames = new List<string>();
                    missingStatNames.AddRange(allStats);
                    missingStatNames.RemoveAll(delegate (string obj)
                    {
                        return alternate.HasKey(obj);
                    });

                    var newMod = EditorGUILayout.Popup("Add Variable Modifer", 0, missingStatNames.ToArray());
                    if (newMod != 0)
                    {
                        AddStatModifier(missingStatNames[newMod], alternate);
                    }

                    if (alternate.statMods.Count == 0)
                    {
                        DTInspectorUtility.ShowColorWarningBox("You currently are using no Modifiers for this prefab.");
                    }
                    else
                    {
                        EditorGUILayout.Separator();

                        int? indexToDelete = null;

                        foreach (var modifier in alternate.statMods)
                        {
                            var buttonPressed = DTInspectorUtility.FunctionButtons.None;
                            switch (modifier._varTypeToUse)
                            {
                                case WorldVariableTracker.VariableType._integer:
                                    buttonPressed = KillerVariablesHelper.DisplayKillerInt(ref _isDirty, modifier._modValueIntAmt, modifier._statName, _killable, true, true);
                                    break;
                                case WorldVariableTracker.VariableType._float:
                                    buttonPressed = KillerVariablesHelper.DisplayKillerFloat(ref _isDirty, modifier._modValueFloatAmt, modifier._statName, _killable, true, true);
                                    break;
                                default:
                                    Debug.LogError("Add code for varType: " + modifier._varTypeToUse.ToString());
                                    break;
                            }

                            KillerVariablesHelper.ShowErrorIfMissingVariable(modifier._statName);

                            if (buttonPressed == DTInspectorUtility.FunctionButtons.Remove)
                            {
                                indexToDelete = i;
                            }
                        }

                        if (indexToDelete.HasValue)
                        {
                            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _killable, "delete Modifier");
                            alternate.DeleteByIndex(indexToDelete.Value);
                        }

                        EditorGUILayout.Separator();
                    }

                    EditorGUILayout.EndVertical();
                }

                if (iElseToDelete.HasValue)
                {
                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _killable, "delete Scenario");
                    _killable.alternateModifiers.RemoveAt(iElseToDelete.Value);
                }
                DTInspectorUtility.EndGroupedControls();
            }


            // respawn settings section
            EditorGUI.indentLevel = 0;

            DTInspectorUtility.VerticalSpace(2);

            state = _killable.showRespawnSettings;
            text = "Respawn Settings & Events";

            DTInspectorUtility.ShowCollapsibleSection(ref state, text);

            if (state != _killable.showRespawnSettings)
            {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _killable, "toggle expand Respawn Settings");
                _killable.showRespawnSettings = state;
            }
            DTInspectorUtility.AddHelpIconNoStyle("http://www.dtdevtools.com/docs/coregamekit/Killables.htm#Respawn");
            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel = 0;
            if (_killable.showRespawnSettings)
            {
                DTInspectorUtility.BeginGroupedControls();
                var newRespawn = (Killable.RespawnType)EditorGUILayout.EnumPopup("Death Respawn Type", _killable.respawnType);
                if (newRespawn != _killable.respawnType)
                {
                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _killable, "toggle Death Respawn Type");
                    _killable.respawnType = newRespawn;
                }

                if (_killable.respawnType != Killable.RespawnType.None)
                {
#if MASTERAUDIO_ENABLED
                CGKMAHelper.SoundGroupField(_maInScene, _groupNames, "Respawn Sound", _killable, ref _isDirty, ref _killable.respawnSound);
#endif
                }

                if (_killable.respawnType == Killable.RespawnType.SetNumber)
                {
                    var newTimes = EditorGUILayout.IntSlider("Times to Respawn", _killable.timesToRespawn, 1, int.MaxValue);
                    if (newTimes != _killable.timesToRespawn)
                    {
                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _killable, "change Times to Respawn");
                        _killable.timesToRespawn = newTimes;
                    }

                    if (Application.isPlaying)
                    {
                        GUI.contentColor = DTInspectorUtility.BrightTextColor;
                        GUILayout.Label("Times Respawned: " + _killable.TimesRespawned);
                        GUI.contentColor = Color.white;
                    }
                }

                if (_killable.respawnType != Killable.RespawnType.None)
                {
                    KillerVariablesHelper.DisplayKillerFloat(ref _isDirty, _killable.respawnDelay, "Respawn Delay (sec)", _killable);

                    DTInspectorUtility.StartGroupHeader(0, false);
                    var newExp = EditorGUILayout.Toggle("Respawn Cust. Events", _killable.respawnFireEvents);
                    if (newExp != _killable.respawnFireEvents)
                    {
                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _killable, "toggle Respawn Cust. Events");
                        _killable.respawnFireEvents = newExp;
                    }

                    if (_killable.respawnFireEvents)
                    {
                        DTInspectorUtility.ShowColorWarningBox("When respawned, fire the Custom Events below");

                        EditorGUILayout.BeginHorizontal();
                        GUI.contentColor = DTInspectorUtility.AddButtonColor;
                        GUILayout.Space(2);
                        if (GUILayout.Button(new GUIContent("Add", "Click to add a Custom Event"), EditorStyles.toolbarButton, GUILayout.Width(50)))
                        {
                            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _killable, "Add Damage Custom Event");
                            _killable.respawnCustomEvents.Add(new CGKCustomEventToFire());
                        }
                        GUI.contentColor = Color.white;

                        EditorGUILayout.EndHorizontal();

                        if (_killable.respawnCustomEvents.Count == 0)
                        {
                            DTInspectorUtility.ShowColorWarningBox("You have no Custom Events selected to fire.");
                        }

                        DTInspectorUtility.VerticalSpace(2);

                        int? indexToDelete = null;

                        // ReSharper disable once ForCanBeConvertedToForeach
                        for (var i = 0; i < _killable.respawnCustomEvents.Count; i++)
                        {
                            var anEvent = _killable.respawnCustomEvents[i].CustomEventName;

                            var buttonClicked = DTInspectorUtility.FunctionButtons.None;
                            anEvent = DTInspectorUtility.SelectCustomEventForVariable(ref _isDirty, anEvent, _killable, "Custom Event", ref buttonClicked);

                            if (buttonClicked == DTInspectorUtility.FunctionButtons.Remove)
                            {
                                indexToDelete = i;
                            }

                            if (anEvent == _killable.respawnCustomEvents[i].CustomEventName)
                            {
                                continue;
                            }

                            _killable.respawnCustomEvents[i].CustomEventName = anEvent;
                        }

                        if (indexToDelete.HasValue)
                        {
                            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _killable, "Remove last Damage Custom Event");
                            _killable.respawnCustomEvents.RemoveAt(indexToDelete.Value);
                        }
                    }
                    EditorGUILayout.EndVertical();
                }

                DTInspectorUtility.EndGroupedControls();
            }

            if (GUI.changed || _isDirty)
            {
                EditorUtility.SetDirty(target); // or it won't save the data!!
            }

            if (deathPrefabToDelete != null)
            {
                DestroyImmediate(deathPrefabToDelete);
            }

            //DrawDefaultInspector();
        }

        private void AddModifierElse(Killable kil)
        {
            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, kil, "add Else");

            kil.alternateModifiers.Add(new WorldVariableCollection());
        }

        private void AddStatModifier(string modifierName, WorldVariableCollection modifiers)
        {
            if (modifiers.HasKey(modifierName))
            {
                DTInspectorUtility.ShowAlert("This Killable already has a modifier for World Variable: " + modifierName + ". Please modify that instead.");
                return;
            }

            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _killable, "add Modifier");

            var myVar = WorldVariableTracker.GetWorldVariableScript(modifierName);

            modifiers.statMods.Add(new WorldVariableModifier(modifierName, myVar.varType));
        }
    }
}