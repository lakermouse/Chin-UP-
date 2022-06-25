using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
#if ADDRESSABLES_ENABLED
using UnityEngine.AddressableAssets;
#endif

namespace DarkTonic.CoreGameKit.EditorScripts
{
    // ReSharper disable once CheckNamespace
    public static class PoolBossEditorUtility
    {
        private static PoolBoss _poolBoss;
        private static int _categoryNum;
        private static int _weightToCreate = 5;

        public static PoolBoss PoBoss {
            get {
                if (_poolBoss != null)
                {
                    return _poolBoss;
                }

                _poolBoss = PoolBoss.Instance;

                return _poolBoss;
            }
        }

        public static bool PrefabIsInPoolBoss(List<PoolBossItem> items, PoolBoss.PrefabSource prefabSource, bool isUnknownType, Transform transPrefab
#if ADDRESSABLES_ENABLED
        , AssetReference prefabAddressable
#endif
        , bool willDisplayPrefab)
        {
            if (items.Count == 0)
            {
                return false;
            }

            PoolBossItem matchingItem = null;

            if (isUnknownType)
            {
                matchingItem = items.Find(delegate (PoolBossItem obj)
                {
                    return obj.prefabSource == PoolBoss.PrefabSource.Prefab &&
                        transPrefab != null && obj.prefabTransform != null && obj.prefabTransform.name == transPrefab.name;
                });

                if (matchingItem == null)
                {
#if ADDRESSABLES_ENABLED

                matchingItem = items.Find(delegate (PoolBossItem obj)
                {
                    return obj.prefabSource == PoolBoss.PrefabSource.Addressable
                        && transPrefab != null 
                        && CGKAddressableEditorHelper.IsAddressableValid(obj.prefabAddressable)
                        && CGKAddressableEditorHelper.EditTimeAddressableName(obj.prefabAddressable) == transPrefab.name;
                });
#endif
                }
            }
            else
            {
                switch (prefabSource)
                {
                    case PoolBoss.PrefabSource.Prefab:
                        matchingItem = items.Find(delegate (PoolBossItem obj)
                        {
                            return transPrefab != null && obj.prefabTransform != null && obj.prefabTransform.name == transPrefab.name;
                        });
                        break;
#if ADDRESSABLES_ENABLED
            case PoolBoss.PrefabSource.Addressable:
                matchingItem = items.Find(delegate (PoolBossItem obj)
                {
                    return obj.prefabSource == PoolBoss.PrefabSource.Addressable
                        && CGKAddressableEditorHelper.IsAddressableValid(obj.prefabAddressable)
                        && CGKAddressableEditorHelper.IsAddressableValid(prefabAddressable)
                        && CGKAddressableEditorHelper.EditTimeAddressableName(obj.prefabAddressable) == CGKAddressableEditorHelper.EditTimeAddressableName(prefabAddressable);
                });
                break;
#endif
                }
            }

            return matchingItem != null;
        }

        public static void DisplayPrefab(ref bool isDirty, Object editorObject, ref Transform prefabInstance, ref string catName,
            ref string poolItemName, ref bool shouldSelectFromPB, ref PoolBoss.PrefabSource prefabSource, bool isUnknown,
            string prefabName,
#if ADDRESSABLES_ENABLED
        ref AssetReference prefabAddressable,
        SerializedObject serializedObject,
        SerializedProperty addressableProp,
#endif
        bool willDisplayPrefab = true)
        {
            bool allowCreateAsAddressableForMissing = isUnknown; // this is for the prefab itself (i.e. Killable in the Killable script).

            var prefabIndexInName = prefabName.LastIndexOf(" Prefab");
            var addressablePrefabName = prefabIndexInName < 0 ? prefabName : prefabName.Substring(0, prefabIndexInName) + " Addressable";
            if (addressablePrefabName == string.Empty || allowCreateAsAddressableForMissing) { } // eliminate warning

            Transform prefab = null;

            if (prefabInstance != null)
            {
                prefab = PrefabUtility.GetCorrespondingObjectFromSource(prefabInstance) as Transform;
            }

            if (prefab == null)
            {
                prefab = prefabInstance; // there is no parent because it was already the one from Hierarchy (prefab, not instance)
            }

            var categories = new List<string>(PoBoss._categories.Count);
            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < PoBoss._categories.Count; i++)
            {
                categories.Add(PoBoss._categories[i].CatName);
            }

            var existingCat = -1;
            var existingItem = -1;

            if (willDisplayPrefab)
            {
                var oldBG = GUI.contentColor;

                if (shouldSelectFromPB)
                {
                    DTInspectorUtility.ShowColorWarningBox("Select the prefab to use from those set up in Pool Boss");

                    existingCat = categories.IndexOf(catName);
                    if (existingCat < 0)
                    {
                        existingCat = 0;
                        isDirty = true;
                        catName = categories[0];
                    }

                    var categoryNum = EditorGUILayout.Popup("Pool Boss Category", existingCat, categories.ToArray());
                    if (categoryNum != existingCat)
                    {
                        UndoHelper.RecordObjectPropertyForUndo(ref isDirty, editorObject, "change Pool Boss Category");
                        catName = categories[categoryNum];
                    }

                    var copyOfCatName = catName;

                    var categoryItems = PoBoss.poolItems.FindAll(delegate (PoolBossItem item)
                    {
                        return item.categoryName == copyOfCatName;
                    });

                    var poolItemNames = new List<string>(categoryItems.Count);
                    for (var i = 0; i < categoryItems.Count; i++)
                    {
                        var item = categoryItems[i];
                        var itemName = "";

                        switch (item.prefabSource)
                        {
                            case PoolBoss.PrefabSource.Prefab:
                                itemName = item.prefabTransform == null ? string.Empty : item.prefabTransform.name;
                                break;
#if ADDRESSABLES_ENABLED
                        case PoolBoss.PrefabSource.Addressable:
                            var addressableName = CGKAddressableEditorHelper.EditTimeAddressableName(item.prefabAddressable);
                            itemName = string.IsNullOrWhiteSpace(addressableName) ? string.Empty : addressableName;
                            break;
#endif
                        }

                        if (string.IsNullOrEmpty(itemName))
                        {
                            continue;
                        }

                        poolItemNames.Add(itemName);
                    }

                    existingItem = poolItemNames.IndexOf(poolItemName);
                    var hasCategoryItems = poolItemNames.Count > 0;

                    if (hasCategoryItems)
                    {
                        if (existingItem < 0)
                        {
                            existingItem = 0;
                            isDirty = true;
                            poolItemName = poolItemNames[0];
                        }

                        var poolItem = EditorGUILayout.Popup("Pool Item In Category", existingItem, poolItemNames.ToArray());

                        if (poolItem != existingItem)
                        {
                            UndoHelper.RecordObjectPropertyForUndo(ref isDirty, editorObject, "change Pool Item In Category");
                            poolItemName = poolItemNames[poolItem];
                        }
                    }
                    else
                    {
                        DTInspectorUtility.ShowColorWarningBox("This category has no Pool Items.");
                    }

                    EditorGUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    GUI.contentColor = DTInspectorUtility.BrightButtonColor;
                    if (hasCategoryItems)
                    {
                        if (GUILayout.Button(new GUIContent("Confirm Selection", "Click to confirm Pool Boss prefab selection"), EditorStyles.toolbarButton, GUILayout.Width(110),
                            GUILayout.Height(16)))
                        {
                            if (ConfirmSelectedPrefab(poolItemName, ref prefabInstance, ref isDirty, editorObject, prefabName, ref prefabSource
#if ADDRESSABLES_ENABLED
                        , ref prefabAddressable
#endif
                        ))
                            {
                                isDirty = true;
                                shouldSelectFromPB = false;
                            }
                        }

                        GUILayout.Space(6);
                    }

                    GUI.contentColor = oldBG;
                    if (GUILayout.Button(new GUIContent(CoreGameKitInspectorResources.CancelTexture, "Click to cancel Pool Boss prefab selection"), EditorStyles.toolbarButton, GUILayout.Width(24),
                        GUILayout.Height(16)))
                    {
                        isDirty = true;
                        shouldSelectFromPB = false;
                    }

                    EditorGUILayout.EndHorizontal();

                    DTInspectorUtility.VerticalSpace(4);
                }
                else
                {
#if ADDRESSABLES_ENABLED
                var newSource = (PoolBoss.PrefabSource)EditorGUILayout.EnumPopup("Prefab Source", prefabSource);
                if (newSource != prefabSource)
                {
                    UndoHelper.RecordObjectPropertyForUndo(ref isDirty, editorObject, "change Prefab Source");
                    prefabSource = newSource;

                    if (newSource == PoolBoss.PrefabSource.Addressable)
                    {
                        prefabInstance = null;
                    }
                }
#endif

                    EditorGUILayout.BeginHorizontal();
                    GUI.backgroundColor = DTInspectorUtility.AddButtonColor;
                    if (GUILayout.Button(new GUIContent(CoreGameKitInspectorResources.HandTexture, "Click to select a prefab from Pool Boss"),
                        EditorStyles.miniButton, GUILayout.Height(16), GUILayout.Width(24)))
                    {
                        UndoHelper.RecordObjectPropertyForUndo(ref isDirty, editorObject, "click Hand icon to select prefab" + prefabName);
                        isDirty = true;
                        shouldSelectFromPB = true;
                    }
                    GUI.backgroundColor = oldBG;

                    var isEmptyPrefab = false;

                    switch (prefabSource)
                    {
                        case PoolBoss.PrefabSource.Prefab:
                            var newPrefab = (Transform)EditorGUILayout.ObjectField(prefabName, prefabInstance, typeof(Transform), true);
                            if (newPrefab != prefabInstance)
                            {
                                UndoHelper.RecordObjectPropertyForUndo(ref isDirty, editorObject, "change " + prefabName);
                                prefabInstance = newPrefab;
                            }

                            if (prefabInstance == null)
                            {
                                isEmptyPrefab = true;
                            }

                            break;
#if ADDRESSABLES_ENABLED
                    case PoolBoss.PrefabSource.Addressable:
                        serializedObject.Update();

                        EditorGUILayout.PropertyField(addressableProp, new GUIContent(addressablePrefabName, "Drag an Addressable prefab here"), true);


                        serializedObject.ApplyModifiedProperties();

                        break;
#endif
                    }

                    EditorGUILayout.EndHorizontal();

                    if (isEmptyPrefab)
                    {
                        DTInspectorUtility.ShowColorWarningBox("Assign " + prefabName + " using the green hand icon or drag it in.");
                    }
                }
            }

            switch (prefabSource)
            {
                case PoolBoss.PrefabSource.Prefab:
                    if (prefabInstance == null)
                    {
                        return; // or inspector blows up in Unity 2018.3
                    }
                    break;
#if ADDRESSABLES_ENABLED
            case PoolBoss.PrefabSource.Addressable:
                if (!CGKAddressableEditorHelper.IsAddressableValid(prefabAddressable))
                {
                    return;
                }
                break;
#endif
            }

            if (shouldSelectFromPB || isDirty)
            {
                return; // "This isn't in Pool Boss" shouldn't appear until you confirm.
            }

            if (Application.isPlaying || PoBoss == null)
            {
                return;
            }

            List<PoolBossItem> items = new List<PoolBossItem>();
            var boss = PoolBoss.Instance;
            if (boss != null)
            {
                items.AddRange(boss.poolItems);
            }

            var miniBosses = GameObject.FindObjectsOfType<PoolMiniBoss>();
            for (var i = 0; i < miniBosses.Length; i++)
            {
                var aMiniBoss = miniBosses[i];
                if (aMiniBoss.poolItems.Count > 0)
                {
                    items.AddRange(aMiniBoss.poolItems);
                }
            }

            if (PrefabIsInPoolBoss(items, prefabSource, isUnknown, prefab,
#if ADDRESSABLES_ENABLED
                prefabAddressable,
#endif
                willDisplayPrefab) || DragAndDrop.paths.Length > 0)
            {

                return;
            }

            DTInspectorUtility.StartGroupHeader();
            DTInspectorUtility.ShowRedErrorBox("This prefab is not configured in Pool Boss. Add it with the controls below or go to Pool Boss and add it manually.");

            existingCat = categories.IndexOf(catName);
            if (existingCat < 0)
            {
                existingCat = 0;
                isDirty = true;
                catName = categories[0];
            }

            _categoryNum = EditorGUILayout.Popup("Category", existingCat, categories.ToArray());
            if (_categoryNum != existingCat)
            {
                UndoHelper.RecordObjectPropertyForUndo(ref isDirty, editorObject, "change category");
                catName = categories[_categoryNum];
            }

            _weightToCreate = EditorGUILayout.IntField("Preload Qty", _weightToCreate);

            EditorGUILayout.BeginHorizontal();

            var oldColor = GUI.contentColor;

            GUILayout.Space(2);
            GUI.contentColor = DTInspectorUtility.AddButtonColor;
            if (GUILayout.Button("Create Pool Boss Item", EditorStyles.toolbarButton, GUILayout.Width(135)))
            {
                UndoHelper.RecordObjectPropertyForUndo(ref isDirty, PoBoss, "Create Pool Boss item");

                var newItem = new PoolBossItem
                {
                    instancesToPreload = _weightToCreate,
                    categoryName = catName
                };

                newItem.prefabSource = prefabSource;

                if (isUnknown)
                {
                    // for Killable base prefab
                    newItem.prefabSource = PoolBoss.PrefabSource.Prefab;
                    newItem.prefabTransform = prefab;
                }
                else
                {
                    switch (prefabSource)
                    {
                        case PoolBoss.PrefabSource.Prefab:
                            newItem.prefabTransform = prefab;
                            break;
#if ADDRESSABLES_ENABLED
                case PoolBoss.PrefabSource.Addressable:
                    newItem.prefabAddressable = new AssetReference(prefabAddressable.RuntimeKey.ToString());
                    break;
#endif
                    }
                }

                PoBoss.poolItems.Add(newItem);
            }
            GUILayout.Space(10);

#if ADDRESSABLES_ENABLED
        if (allowCreateAsAddressableForMissing)
        {
            if (GUILayout.Button("Create As Addressable", EditorStyles.toolbarButton, GUILayout.Width(135)))
            {
                UndoHelper.RecordObjectPropertyForUndo(ref isDirty, PoBoss, "Create As Addressable");

                var path = AssetDatabase.GetAssetPath(prefab);
                var assetId = AssetDatabase.AssetPathToGUID(path);

                var newItem = new PoolBossItem
                {
                    prefabSource = PoolBoss.PrefabSource.Addressable,
                    instancesToPreload = _weightToCreate,
                    categoryName = catName,
                    prefabAddressable = new AssetReference(assetId)
                };

                PoBoss.poolItems.Add(newItem);
            };

            GUILayout.Space(10);
        }
#endif

            GUI.contentColor = DTInspectorUtility.BrightButtonColor;
            if (GUILayout.Button("Go to Pool Boss", EditorStyles.toolbarButton, GUILayout.Width(130)))
            {
                Selection.activeGameObject = PoBoss.gameObject;
            }

            GUI.contentColor = oldColor;
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndVertical();
        }

        private static bool ConfirmSelectedPrefab(string poolItemName, ref Transform prefabInstance, ref bool isDirty, Object editorObject, string prefabName,
            ref PoolBoss.PrefabSource prefabSource
#if ADDRESSABLES_ENABLED
        , ref AssetReference prefabAddressable
#endif
        )
        {
            var matchingItem = PoBoss.poolItems.Find(delegate (PoolBossItem item)
            {
                var itemName = string.Empty;

                switch (item.prefabSource)
                {
                    case PoolBoss.PrefabSource.Prefab:
                        itemName = item.prefabTransform == null ? string.Empty : item.prefabTransform.name;
                        break;
#if ADDRESSABLES_ENABLED
                case PoolBoss.PrefabSource.Addressable:
                    var addressableName = CGKAddressableEditorHelper.EditTimeAddressableName(item.prefabAddressable);
                    itemName = string.IsNullOrWhiteSpace(addressableName) ? string.Empty : addressableName;
                    break;
#endif
            }

                return itemName == poolItemName;
            });

            if (matchingItem == null)
            {
                var errorMsg = "Could not locate prefab in Pool Boss with name '" + poolItemName + "'.";
                if (Application.isPlaying)
                {
                    LevelSettings.LogIfNew(errorMsg);
                }
                else
                {
                    DTInspectorUtility.ShowAlert(errorMsg);
                }

                return false;
            }

            UndoHelper.RecordObjectPropertyForUndo(ref isDirty, editorObject, "change " + prefabName + " selection");

            prefabSource = matchingItem.prefabSource;

            switch (matchingItem.prefabSource)
            {
                case PoolBoss.PrefabSource.Prefab:
                    prefabInstance = matchingItem.prefabTransform;
                    break;

#if ADDRESSABLES_ENABLED
            case PoolBoss.PrefabSource.Addressable:
                prefabInstance = null;
                prefabAddressable = new AssetReference(matchingItem.prefabAddressable.RuntimeKey.ToString());
                break;
#endif
            }

            return true;
        }
    }
}