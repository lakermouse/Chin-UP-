using UnityEditor;
using UnityEngine;

namespace DarkTonic.CoreGameKit.EditorScripts
{
    [CustomEditor(typeof(KillableDeathPrefab), true)]
    // ReSharper disable once CheckNamespace
    public class KillableDeathPrefabInspector : Editor
    {
        private KillableDeathPrefab _deathPrefab;
        private bool _isDirty;

        // ReSharper disable once FunctionComplexityOverflow
        public override void OnInspectorGUI()
        {
            EditorGUI.indentLevel = 0;

            _deathPrefab = (KillableDeathPrefab)target;

            WorldVariableTracker.ClearInGamePlayerStats();

            LevelSettings.Instance = null; // clear cached version
            DTInspectorUtility.DrawTexture(CoreGameKitInspectorResources.LogoTexture);
            DTInspectorUtility.HelpHeader("http://www.dtdevtools.com/docs/coregamekit/KillableDeathPrefab.htm");

            _isDirty = false;
            var killable = _deathPrefab.GetComponent<Killable>();

            var poolNames = LevelSettings.GetSortedPrefabPoolNames();
            var hasDeathPrefab = true;

            DTInspectorUtility.StartGroupHeader(1, true);
            EditorGUI.indentLevel = 1;
            GUILayout.Label("Death Prefab");

            EditorGUILayout.EndVertical();
            EditorGUI.indentLevel = 0;

            var newDeathSource = (WaveSpecifics.SpawnOrigin)EditorGUILayout.EnumPopup("Death Prefab Type", _deathPrefab.deathPrefabSource);
            if (newDeathSource != _deathPrefab.deathPrefabSource)
            {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _deathPrefab, "change Death Prefab Type");
                _deathPrefab.deathPrefabSource = newDeathSource;
            }

            switch (_deathPrefab.deathPrefabSource)
            {
                case WaveSpecifics.SpawnOrigin.PrefabPool:
                    if (poolNames != null)
                    {
                        var pool = LevelSettings.GetFirstMatchingPrefabPool(_deathPrefab.deathPrefabPoolName);
                        var noDeathPool = false;
                        var illegalDeathPref = false;
                        var noPrefabPools = false;

                        if (pool == null)
                        {
                            if (string.IsNullOrEmpty(_deathPrefab.deathPrefabPoolName))
                            {
                                noDeathPool = true;
                            }
                            else
                            {
                                illegalDeathPref = true;
                            }
                            _deathPrefab.deathPrefabPoolIndex = 0;
                        }
                        else
                        {
                            _deathPrefab.deathPrefabPoolIndex = poolNames.IndexOf(_deathPrefab.deathPrefabPoolName);
                        }

                        if (poolNames.Count > 1)
                        {
                            EditorGUILayout.BeginHorizontal();
                            var newDeathPool = EditorGUILayout.Popup("Death Prefab Pool", _deathPrefab.deathPrefabPoolIndex, poolNames.ToArray());
                            if (newDeathPool != _deathPrefab.deathPrefabPoolIndex)
                            {
                                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _deathPrefab, "change Death Prefab Pool");
                                _deathPrefab.deathPrefabPoolIndex = newDeathPool;
                            }

                            if (_deathPrefab.deathPrefabPoolIndex > 0)
                            {
                                var matchingPool = LevelSettings.GetFirstMatchingPrefabPool(poolNames[_deathPrefab.deathPrefabPoolIndex]);
                                if (matchingPool != null)
                                {
                                    _deathPrefab.deathPrefabPoolName = matchingPool.name;
                                }
                            }
                            else
                            {
                                _deathPrefab.deathPrefabPoolName = string.Empty;
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
                            DTInspectorUtility.ShowRedErrorBox("Death Prefab Pool '" + _deathPrefab.deathPrefabPoolName + "' not found. Select one.");
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
                    PoolBossEditorUtility.DisplayPrefab(ref _isDirty, _deathPrefab, ref _deathPrefab.deathPrefabSpecific, ref _deathPrefab.deathPrefabCategoryName,
                        ref _deathPrefab.deathPrefabPoolItemName,
                        ref _deathPrefab.shouldDeathPrefabSelectFromPB, ref _deathPrefab.deathPrefabFrom, false,
                        "Death Prefab"
#if ADDRESSABLES_ENABLED
    , ref _deathPrefab.deathPrefabAddressable
, serializedObject
, serializedObject.FindProperty(nameof(_deathPrefab.deathPrefabAddressable))
#endif
                        );

                    if (_deathPrefab.deathPrefabSpecific == null)
                    {
                        hasDeathPrefab = false;
                    }

                    break;
            }

            if (hasDeathPrefab)
            {
                var newKeepParent = EditorGUILayout.Toggle("Keep Same Parent", _deathPrefab.deathPrefabKeepSameParent);
                if (newKeepParent != _deathPrefab.deathPrefabKeepSameParent)
                {
                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _deathPrefab, "toggle Keep Same Parent");
                    _deathPrefab.deathPrefabKeepSameParent = newKeepParent;
                }

                KillerVariablesHelper.DisplayKillerInt(ref _isDirty, _deathPrefab.deathPrefabSpawnPercent, "Spawn % Chance", _deathPrefab);

                KillerVariablesHelper.DisplayKillerInt(ref _isDirty, _deathPrefab.deathPrefabQty, "Spawn Quantity", _deathPrefab);

                var newSpawnPosition = (Killable.DeathPrefabSpawnLocation)EditorGUILayout.EnumPopup("Spawn Position", _deathPrefab.deathPrefabSpawnLocation);
                if (newSpawnPosition != _deathPrefab.deathPrefabSpawnLocation)
                {
                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _deathPrefab, "change Spawn Position");
                    _deathPrefab.deathPrefabSpawnLocation = newSpawnPosition;
                }

                var newDeathOffset = EditorGUILayout.Vector3Field("Spawn Offset", _deathPrefab.deathPrefabOffset);
                if (newDeathOffset != _deathPrefab.deathPrefabOffset)
                {
                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _deathPrefab, "change Spawn Offset");
                    _deathPrefab.deathPrefabOffset = newDeathOffset;
                }

                var newOffset = EditorGUILayout.Vector3Field("Incremental Offset", _deathPrefab.deathPrefabIncrementalOffset);
                if (newOffset != _deathPrefab.deathPrefabIncrementalOffset)
                {
                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _deathPrefab, "change Incremental Offset");
                    _deathPrefab.deathPrefabIncrementalOffset = newOffset;
                }

                KillerVariablesHelper.DisplayKillerFloat(ref _isDirty, _deathPrefab.deathPrefabRandomDistX, "Rand. Offset X", _deathPrefab);
                KillerVariablesHelper.DisplayKillerFloat(ref _isDirty, _deathPrefab.deathPrefabRandomDistY, "Rand. Offset Y", _deathPrefab);
                KillerVariablesHelper.DisplayKillerFloat(ref _isDirty, _deathPrefab.deathPrefabRandomDistZ, "Rand. Offset Z", _deathPrefab);

                if (!killable.IsGravBody)
                {
                    DTInspectorUtility.ShowColorWarningBox("Inherit Velocity can only be used on gravity rigidbodies");
                }
                else
                {
                    var newKeep = EditorGUILayout.Toggle("Inherit Velocity", _deathPrefab.deathPrefabKeepVelocity);
                    if (newKeep != _deathPrefab.deathPrefabKeepVelocity)
                    {
                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _deathPrefab, "toggle Inherit Velocity");
                        _deathPrefab.deathPrefabKeepVelocity = newKeep;
                    }
                }
            }

            var newMode = (Killable.RotationMode)EditorGUILayout.EnumPopup("Rotation Mode", _deathPrefab.rotationMode);
            if (newMode != _deathPrefab.rotationMode)
            {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _deathPrefab, "change Rotation Mode");
                _deathPrefab.rotationMode = newMode;
            }
            if (_deathPrefab.rotationMode == Killable.RotationMode.CustomRotation)
            {
                var newCustomRot = EditorGUILayout.Vector3Field("Custom Rotation Euler", _deathPrefab.deathPrefabCustomRotation);
                if (newCustomRot != _deathPrefab.deathPrefabCustomRotation)
                {
                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _deathPrefab, "change Custom Rotation Euler");
                    _deathPrefab.deathPrefabCustomRotation = newCustomRot;
                }
            }

            EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(100));
            EditorGUILayout.LabelField("Random Rotation");

            var newRandomX = GUILayout.Toggle(_deathPrefab.deathPrefabRandomizeXRotation, "X");
            if (newRandomX != _deathPrefab.deathPrefabRandomizeXRotation)
            {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _deathPrefab, "toggle Random X Rotation");
                _deathPrefab.deathPrefabRandomizeXRotation = newRandomX;
            }
            GUILayout.Space(10);
            var newRandomY = GUILayout.Toggle(_deathPrefab.deathPrefabRandomizeYRotation, "Y");
            if (newRandomY != _deathPrefab.deathPrefabRandomizeYRotation)
            {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _deathPrefab, "toggle Random Y Rotation");
                _deathPrefab.deathPrefabRandomizeYRotation = newRandomY;
            }
            GUILayout.Space(10);
            var newRandomZ = GUILayout.Toggle(_deathPrefab.deathPrefabRandomizeZRotation, "Z");
            if (newRandomZ != _deathPrefab.deathPrefabRandomizeZRotation)
            {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _deathPrefab, "toggle Random Z Rotation");
                _deathPrefab.deathPrefabRandomizeZRotation = newRandomZ;
            }
            EditorGUILayout.EndHorizontal();


            EditorGUILayout.EndVertical();

            if (GUI.changed || _isDirty)
            {
                EditorUtility.SetDirty(target); // or it won't save the data!!
            }

            //DrawDefaultInspector();
        }
    }
}