using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

namespace DarkTonic.CoreGameKit.EditorScripts
{
    [CustomEditor(typeof(PoolMiniBoss))]
    public class PoolMiniBossInspector : Editor
    {
        private PoolMiniBoss _miniPool;
        private bool _isDirty;

        // ReSharper disable once FunctionComplexityOverflow
        public override void OnInspectorGUI()
        {
            EditorGUI.indentLevel = 0;

            _miniPool = (PoolMiniBoss)target;

            if (DTInspectorUtility.IsInPrefabMode(_miniPool.gameObject))
            {
                DTInspectorUtility.PrefabModeDoNotEdit();
                return;
            }

            _isDirty = false;
            LevelSettings.Instance = null; // clear cached version

            DTInspectorUtility.DrawTexture(CoreGameKitInspectorResources.LogoTexture);
            DTInspectorUtility.HelpHeader("http://www.dtdevtools.com/docs/coregamekit/PoolMiniBoss.htm");

            if (DTInspectorUtility.IsPrefabInProjectView(_miniPool.gameObject))
            {
                DTInspectorUtility.MakePrefabMessage();
                return;
            }

            if (DTInspectorUtility.IsLinkedToDarkTonicPrefabFolder(_miniPool))
            {
                DTInspectorUtility.MakePrefabMessage();
                return;
            }

            var pool = PoolBoss.Instance;
            var hasPoolBoss = pool != null;

            if (!hasPoolBoss)
            {
                DTInspectorUtility.ShowLargeBarAlertBox("You can only use this prefab in a Scene that contains PoolBoss.");
                return;
            }

            List<string> categoryNames = new List<string>();

            for (var i = 0; i < _miniPool._categories.Count; i++)
            {
                var catName = _miniPool._categories[i].CatName;
                if (categoryNames.Contains(catName))
                {
                    continue;
                }

                categoryNames.Add(catName);
            }

            var newAwake = EditorGUILayout.Toggle("Auto-create Items", _miniPool.createOnEnable);
            if (newAwake != _miniPool.createOnEnable)
            {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _miniPool, "toggle Auto-create Items");
                _miniPool.createOnEnable = newAwake;
            }
            if (_miniPool.createOnEnable)
            {
                DTInspectorUtility.ShowColorWarningBox("Items will be created as soon as this object is enabled.");
            }
            else
            {
                DTInspectorUtility.ShowLargeBarAlertBox("You will need to call this object's CreateItems method manually to create the items.");
            }

            var newRemove = EditorGUILayout.Toggle("Auto-remove Items", _miniPool.removeOnDisable);
            if (newRemove != _miniPool.removeOnDisable)
            {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _miniPool, "toggle Auto-remove Items");
                _miniPool.removeOnDisable = newRemove;
            }

            if (_miniPool.removeOnDisable)
            {
                DTInspectorUtility.ShowColorWarningBox("Items will be deleted when this object is disabled or destroyed.");

                var newRemoveType = (PoolMiniBoss.RemoveBehavior) EditorGUILayout.EnumPopup("Remove Behavior Type", _miniPool.removeBehaviorType);
                if (newRemoveType != _miniPool.removeBehaviorType)
                {
                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _miniPool, "toggle Remove Behavior Type");
                    _miniPool.removeBehaviorType = newRemoveType;
                }

                switch (_miniPool.removeBehaviorType)
                {
                    case PoolMiniBoss.RemoveBehavior.Kill:
                        DTInspectorUtility.ShowColorWarningBox("Make sure that for 'Kill' Behavior Type, all Death Prefabs are part of the permanent Pool Boss setup, or they will not be removed properly.");
                        break;
                }
            }
            else
            {
                DTInspectorUtility.ShowLargeBarAlertBox("Items created by this will persist across Scenes if Core GameKit does.");
            }

            var newFrames = EditorGUILayout.IntSlider(new GUIContent("Initialize Time (Frames)", "You can increase this value to make the initial pool creation take more frames. Defaults to 1. Max of 1000."), _miniPool.framesForInit, 1, 1000);
            if (newFrames != _miniPool.framesForInit)
            {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _miniPool, "change Initialize Time (Frames)");
                _miniPool.framesForInit = newFrames;
            }

            DTInspectorUtility.StartGroupHeader(2);
            var newFireCompleted = GUILayout.Toggle(_miniPool.fireCompletedCustomEvent, " Fire 'Items Created' event");
            if (newFireCompleted != _miniPool.fireCompletedCustomEvent)
            {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _miniPool, "toggle Fire 'Items Created' event");
                _miniPool.fireCompletedCustomEvent = newFireCompleted;
            }
            EditorGUILayout.EndVertical();

            if (_miniPool.fireCompletedCustomEvent)
            {
                DTInspectorUtility.BeginGroupedControls();

                DTInspectorUtility.ShowColorWarningBox("When items have finished creating, fire the Custom Events below");

                EditorGUILayout.BeginHorizontal();
                GUI.contentColor = DTInspectorUtility.AddButtonColor;
                GUILayout.Space(6);
                if (GUILayout.Button(new GUIContent("Add", "Click to add a Custom Event"), EditorStyles.toolbarButton, GUILayout.Width(50)))
                {
                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _miniPool, "Add 'Items Created' Custom Event");
                    _miniPool.completedCustomEvents.Add(new CGKCustomEventToFire());
                }
                GUI.contentColor = Color.white;

                EditorGUILayout.EndHorizontal();

                if (_miniPool.completedCustomEvents.Count == 0)
                {
                    DTInspectorUtility.ShowColorWarningBox("You have no Custom Events selected to fire.");
                }

                DTInspectorUtility.VerticalSpace(2);

                int? indexToDelete = null;

                for (var i = 0; i < _miniPool.completedCustomEvents.Count; i++)
                {
                    var anEvent = _miniPool.completedCustomEvents[i].CustomEventName;

                    var buttonClicked = DTInspectorUtility.FunctionButtons.None;
                    anEvent = DTInspectorUtility.SelectCustomEventForVariable(ref _isDirty, anEvent, _miniPool, "Custom Event", ref buttonClicked);

                    if (buttonClicked == DTInspectorUtility.FunctionButtons.Remove)
                    {
                        indexToDelete = i;
                    }

                    if (anEvent == _miniPool.completedCustomEvents[i].CustomEventName)
                    {
                        continue;
                    }

                    _miniPool.completedCustomEvents[i].CustomEventName = anEvent;
                }

                if (indexToDelete.HasValue)
                {
                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _miniPool, "Remove 'Items Created' Custom Event");
                    _miniPool.completedCustomEvents.RemoveAt(indexToDelete.Value);
                }

                DTInspectorUtility.EndGroupedControls();
            }

            EditorGUILayout.EndVertical();

#if MASTERAUDIO_ENABLED
            MasterAudio.MasterAudio.Instance = null;

            var ma = MasterAudio.MasterAudio.Instance;
            var _maInScene = ma != null;

            var _groupNames = new List<string>();
            if (_maInScene)
            {
                // ReSharper disable once PossibleNullReferenceException
                _groupNames = ma.GroupNames;
            }

            EditorGUI.indentLevel = 1;
            DTInspectorUtility.StartGroupHeader();

            var newMA = DTInspectorUtility.Foldout(_miniPool.masterAudioSoundsExpanded, "Master Audio Sounds");
            if (newMA != _miniPool.masterAudioSoundsExpanded)
            {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _miniPool, "toggle expand Master Audio Sounds");
                _miniPool.masterAudioSoundsExpanded = newMA;
            }

            if (_miniPool.masterAudioSoundsExpanded)
            {
                DTInspectorUtility.BeginGroupedControls();
                CGKMAHelper.SoundGroupField(_maInScene, _groupNames, "Items Created Sound", _miniPool, ref _isDirty, ref _miniPool.itemsCreatedSound);
                CGKMAHelper.SoundGroupField(_maInScene, _groupNames, "Items Removed Sound", _miniPool, ref _isDirty, ref _miniPool.itemsRemovedSound);
                DTInspectorUtility.EndGroupedControls();
            }

            DTInspectorUtility.EndGroupHeader();
#endif

            if (!Application.isPlaying)
            {
                DTInspectorUtility.StartGroupHeader();
                var newCat = EditorGUILayout.TextField("New Category Name", _miniPool.newCategoryName);
                if (newCat != _miniPool.newCategoryName)
                {
                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _miniPool, "change New Category Name");
                    _miniPool.newCategoryName = newCat;
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.BeginHorizontal();
                GUI.contentColor = DTInspectorUtility.BrightButtonColor;
                GUILayout.Space(2);
                if (GUILayout.Button("Create New Category", EditorStyles.toolbarButton, GUILayout.Width(130)))
                {
                    CreateCategory();
                }
                GUI.contentColor = Color.white;

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
                DTInspectorUtility.ResetColors();

                var selCatIndex = categoryNames.IndexOf(_miniPool.addToCategoryName);

                if (selCatIndex == -1)
                {
                    selCatIndex = 0;
                }

                GUI.backgroundColor = DTInspectorUtility.BrightButtonColor;

                var newIndex = EditorGUILayout.Popup("Default Item Category", selCatIndex, categoryNames.ToArray());
                if (newIndex != selCatIndex)
                {
                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _miniPool, "change Default Item Category");
                    _miniPool.addToCategoryName = categoryNames[newIndex];
                }
                GUI.backgroundColor = Color.white;
                GUI.contentColor = Color.white;
            }

            PoolBossItem itemToRemove = null;
            int? indexToInsertAt = null;
            PoolBossCategory selectedCategory = null;
            PoolBossItem itemToClone = null;

            PoolBossCategory catEditing = null;
            PoolBossCategory catRenaming = null;

            PoolBossCategory catToDelete = null;
            int? indexToShiftUp = null;
            int? indexToShiftDown = null;

            var visiblePoolItems = _miniPool.poolItems;

            for (var i = 0; i < visiblePoolItems.Count; i++)
            {
                var item = visiblePoolItems[i];
                if (categoryNames.Contains(item.categoryName))
                {
                    continue;
                }

                item.categoryName = categoryNames[0];
                _isDirty = true;
            }

            var allExpanded = true;
            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < _miniPool._categories.Count; i++)
            {
                if (_miniPool._categories[i].IsExpanded)
                {
                    continue;
                }
                allExpanded = false;
                break;
            }

            var buttonTooltip = allExpanded ? "Click to collapse all categories and items" : "Click to expand all categories and items";
            var buttonText = allExpanded ? "Collapse All" : "Expand All";
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(new GUIContent(buttonText, buttonTooltip), EditorStyles.toolbarButton, GUILayout.Width(80)))
            {
                ExpandCollapseAll(!allExpanded);
            }

            GUILayout.Space(4);
            if (GUILayout.Button(new GUIContent("Import Pool Boss Categories", "Import all the categories from Pool Boss"), EditorStyles.toolbarButton, GUILayout.Width(170)))
            {
                ImportPoolBossCategories(pool);
            }
            EditorGUILayout.EndHorizontal();

            DTInspectorUtility.VerticalSpace(4);
            GUI.backgroundColor = Color.white;

            if (!Application.isPlaying)
            {
#if ADDRESSABLES_ENABLED
                    var newSource = (PoolBoss.PrefabSource)EditorGUILayout.EnumPopup("Create Items As", _miniPool.newItemPrefabSource);
                    if (newSource != _miniPool.newItemPrefabSource)
                    {
                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _miniPool, "change Create Items As");
                        _miniPool.newItemPrefabSource = newSource;
                    }
#endif

                EditorGUILayout.BeginVertical();
                var anEvent = Event.current;

                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(4);
                GUI.color = DTInspectorUtility.DragAreaColor;
                var dragArea = GUILayoutUtility.GetRect(0f, 30f, GUILayout.ExpandWidth(true));
                GUI.Box(dragArea, "Drag prefabs here in bulk to add them to the Pool!");
                GUI.color = Color.white;

                switch (anEvent.type)
                {
                    case EventType.DragUpdated:
                    case EventType.DragPerform:
                        if (!dragArea.Contains(anEvent.mousePosition))
                        {
                            break;
                        }

                        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                        if (anEvent.type == EventType.DragPerform)
                        {
                            DragAndDrop.AcceptDrag();

                            foreach (var dragged in DragAndDrop.objectReferences)
                            {
                                AddPoolItem(dragged);
                            }
                        }
                        Event.current.Use();
                        break;
                }
                GUILayout.Space(4);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();

                DTInspectorUtility.VerticalSpace(4);
            }

            GUI.backgroundColor = Color.white;
            GUI.contentColor = Color.white;

            // ReSharper disable once ForCanBeConvertedToForeach
            for (var c = 0; c < _miniPool._categories.Count; c++)
            {
                var cat = _miniPool._categories[c];

                EditorGUI.indentLevel = 0;

                var matchingItems = new List<PoolBossItem>();
                matchingItems.AddRange(visiblePoolItems);
                matchingItems.RemoveAll(delegate (PoolBossItem x)
                {
                    return x.categoryName != cat.CatName;
                });

                var hasItems = matchingItems.Count > 0;

                if (!cat.IsEditing || Application.isPlaying)
                {
                    var catName = cat.CatName;

                    catName += ": " + matchingItems.Count + " item" + ((matchingItems.Count != 1) ? "s" : "");

                    var state = cat.IsExpanded;
                    var text = catName;

                    DTInspectorUtility.ShowCollapsibleSectionInline(ref state, text);

                    var headerStyle = new GUIStyle();
                    headerStyle.margin = new RectOffset(0, 0, 0, 0);
                    headerStyle.padding = new RectOffset(0, 0, 0, 0);
                    headerStyle.fixedHeight = 20;

                    EditorGUILayout.BeginHorizontal(headerStyle, GUILayout.MaxWidth(50));

                    if (state != cat.IsExpanded)
                    {
                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _miniPool, "toggle expand Pool Boss Category");
                        cat.IsExpanded = state;
                    }

                    var catItemsCollapsed = true;

                    for (var i = 0; i < visiblePoolItems.Count; i++)
                    {
                        var item = visiblePoolItems[i];
                        if (item.categoryName != cat.CatName)
                        {
                            continue;
                        }

                        if (!item.isExpanded)
                        {
                            continue;
                        }
                        catItemsCollapsed = false;
                        break;
                    }

                    GUI.backgroundColor = Color.white;

                    var tooltip = catItemsCollapsed ? "Click to expand all items in this category" : "Click to collapse all items in this category";
                    var btnText = catItemsCollapsed ? "Expand" : "Collapse";

                    GUI.contentColor = DTInspectorUtility.BrightButtonColor;
                    if (GUILayout.Button(new GUIContent(btnText, tooltip), EditorStyles.toolbarButton, GUILayout.Width(60), GUILayout.Height(16)))
                    {
                        ExpandCollapseCategory(cat.CatName, catItemsCollapsed);
                    }
                    GUI.contentColor = Color.white;

                    if (!Application.isPlaying)
                    {
                        if (c > 0)
                        {
                            // the up arrow.
                            var upArrow = CoreGameKitInspectorResources.UpArrowTexture;
                            if (GUILayout.Button(new GUIContent(upArrow, "Click to shift Category up"),
                                EditorStyles.toolbarButton, GUILayout.Width(24), GUILayout.Height(16)))
                            {
                                indexToShiftUp = c;
                            }
                        }
                        else
                        {
                            GUILayout.Button("", EditorStyles.toolbarButton, GUILayout.Width(24), GUILayout.Height(16));
                        }

                        if (c < _miniPool._categories.Count - 1)
                        {
                            // The down arrow will move things towards the end of the List
                            var dnArrow = CoreGameKitInspectorResources.DownArrowTexture;
                            if (GUILayout.Button(new GUIContent(dnArrow, "Click to shift Category down"),
                                EditorStyles.toolbarButton, GUILayout.Width(24), GUILayout.Height(16)))
                            {
                                indexToShiftDown = c;
                            }
                        }
                        else
                        {
                            GUILayout.Button("", EditorStyles.toolbarButton, GUILayout.Width(24), GUILayout.Height(16));
                        }

                        var settingsIcon = new GUIContent(CoreGameKitInspectorResources.SettingsTexture,
                            "Click to edit Category");

                        GUI.backgroundColor = Color.white;
                        if (GUILayout.Button(settingsIcon, EditorStyles.toolbarButton, GUILayout.Width(24),
                            GUILayout.Height(16)))
                        {
                            catEditing = cat;
                        }
                        GUI.backgroundColor = DTInspectorUtility.DeleteButtonColor;
                        if (GUILayout.Button(new GUIContent("Delete", "Click to delete Category"),
                            EditorStyles.miniButton, GUILayout.MaxWidth(51)))
                        {
                            catToDelete = cat;
                        }

                        DTInspectorUtility.AddHelpIconNoStyle("http://www.dtdevtools.com/docs/coregamekit/PoolBoss.htm#ItemSettings", 1);
                    }
                    else
                    {
                        GUI.contentColor = DTInspectorUtility.BrightButtonColor;

                        if (GUILayout.Button(new GUIContent(CoreGameKitInspectorResources.DamageTexture, "Click to damage all Killables in this Category"), EditorStyles.toolbarButton, GUILayout.Width(24)))
                        {
                            SpawnUtility.DamageAllPrefabsInCategory(cat.CatName, 1);
                            _isDirty = true;
                        }
                        if (GUILayout.Button(new GUIContent(CoreGameKitInspectorResources.KillTexture, "Click to kill all Killables in this Category"), EditorStyles.toolbarButton, GUILayout.Width(24)))
                        {
                            SpawnUtility.KillAllPrefabsInCategory(cat.CatName);
                            _isDirty = true;
                        }
                        if (GUILayout.Button(new GUIContent(CoreGameKitInspectorResources.DespawnTexture, "Click to despawn all prefabs in this Category"), EditorStyles.toolbarButton, GUILayout.Width(24)))
                        {
                            SpawnUtility.DespawnAllPrefabsInCategory(cat.CatName);
                            _isDirty = true;
                        }

                        var itemsSpawned = PoolBoss.CategoryItemsSpawned(cat.CatName);
                        var categoryHasItemsSpawned = itemsSpawned > 0;
                        var theBtnText = itemsSpawned.ToString();
                        var btnColor = categoryHasItemsSpawned ? DTInspectorUtility.BrightTextColor : DTInspectorUtility.DeleteButtonColor;
                        GUI.backgroundColor = btnColor;

                        var btnWidth = 32;
                        if (theBtnText.Length > 3)
                        {
                            btnWidth = 11 * theBtnText.Length;
                        }
                        if (GUILayout.Button(theBtnText, EditorStyles.miniButtonRight, GUILayout.MaxWidth(btnWidth)) && categoryHasItemsSpawned)
                        {
                            var catItems = PoolBoss.CategoryActiveItems(cat.CatName);

                            if (catItems.Count > 0)
                            {
                                var gos = new List<GameObject>(catItems.Count);
                                for (var i = 0; i < catItems.Count; i++)
                                {
                                    gos.Add(catItems[i].gameObject);
                                }

                                Selection.objects = gos.ToArray();
                            }
                        }

                        GUI.backgroundColor = Color.white;
                        GUI.contentColor = Color.white;
                        GUILayout.Space(4);
                    }
                }
                else
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.BeginHorizontal();
                    GUI.backgroundColor = DTInspectorUtility.BrightTextColor;
                    var tex = EditorGUILayout.TextField("", cat.ProspectiveName);
                    if (tex != cat.ProspectiveName)
                    {
                        cat.ProspectiveName = tex;
                        _isDirty = true;
                    }

                    var buttonPressed = DTInspectorUtility.AddCancelSaveButtons("category");

                    switch (buttonPressed)
                    {
                        case DTInspectorUtility.FunctionButtons.Cancel:
                            cat.IsEditing = false;
                            cat.ProspectiveName = cat.CatName;
                            _isDirty = true;
                            break;
                        case DTInspectorUtility.FunctionButtons.Save:
                            catRenaming = cat;
                            break;
                    }

                    GUILayout.Space(15);
                }

                EditorGUILayout.EndHorizontal();
                GUI.backgroundColor = Color.white;
                EditorGUILayout.EndHorizontal();

                if (cat.IsEditing)
                {
                    DTInspectorUtility.VerticalSpace(2);
                }

                matchingItems.Sort(delegate (PoolBossItem x, PoolBossItem y)
                {
                    return string.Compare(PoolBossItemName(x), PoolBossItemName(y), StringComparison.Ordinal);
                });

                bool hasOpenBox = false;

                if (!hasItems)
                {
                    DTInspectorUtility.BeginGroupedControls();
                    DTInspectorUtility.ShowLargeBarAlertBox("This Category is empty. Add / move some items or you may delete it.");
                    DTInspectorUtility.EndGroupedControls();
                }

                if (cat.IsExpanded)
                {
                    if (matchingItems.Count > 0 && !hasOpenBox)
                    {
                        DTInspectorUtility.BeginGroupedControls();
                    }

                    for (var i = 0; i < matchingItems.Count; i++)
                    {
                        var poolItem = matchingItems[i];

                        DTInspectorUtility.StartGroupHeader();

                        if (poolItem.prefabTransform != null)
                        {
                            var rend = poolItem.prefabTransform.GetComponent<TrailRenderer>();
                            if (rend != null && rend.autodestruct)
                            {
                                DTInspectorUtility.ShowRedErrorBox(
                                    "This prefab contains a Trail Renderer with auto-destruct enabled. " + PoolBossInspector.DoNotDestroyPoolItem);
                            }
                        }

                        EditorGUI.indentLevel = 1;
                        EditorGUILayout.BeginHorizontal();

                        string itemName = string.Empty;

                        switch (poolItem.prefabSource)
                        {
                            case PoolBoss.PrefabSource.Prefab:
                                itemName = poolItem.prefabTransform == null ? "[NO PREFAB]" : poolItem.prefabTransform.name;
                                break;
#if ADDRESSABLES_ENABLED
                                case PoolBoss.PrefabSource.Addressable:
                                    var addressableName = CGKAddressableEditorHelper.EditTimeAddressableName(poolItem.prefabAddressable);
                                    itemName = string.IsNullOrWhiteSpace(addressableName) ? "[NO PREFAB]" : addressableName;
                                    break;
#endif
                        }

                        var state = DTInspectorUtility.Foldout(poolItem.isExpanded, itemName);
                        if (state != poolItem.isExpanded)
                        {
                            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _miniPool, "toggle expand Pool Item");
                            poolItem.isExpanded = state;
                        }

                        GUI.backgroundColor = DTInspectorUtility.BrightButtonColor;
                        var selCatIndex = categoryNames.IndexOf(poolItem.categoryName);
                        var newCat = EditorGUILayout.Popup(selCatIndex, categoryNames.ToArray(), GUILayout.Width(130));
                        if (newCat != selCatIndex)
                        {
                            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _miniPool, "change Pool Item Category");
                            poolItem.categoryName = categoryNames[newCat];
                        }
                        GUI.backgroundColor = Color.white;

                        switch (poolItem.prefabSource)
                        {
                            case PoolBoss.PrefabSource.Prefab:
                                DTInspectorUtility.FocusInProjectViewButton("Pool Item prefab", poolItem.prefabTransform == null ? null : poolItem.prefabTransform.gameObject);
                                break;
#if ADDRESSABLES_ENABLED
                                    case PoolBoss.PrefabSource.Addressable:
                                        DTInspectorUtility.FocusAddressableInProjectViewButton("Pool Item prefab", poolItem.prefabAddressable);
                                        break;
#endif
                        }

                        var buttonPressed = DTInspectorUtility.AddFoldOutListItemButtons(i, matchingItems.Count,
                            "Pool Item", false, null, true, false, true);
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.EndVertical();

                        if (poolItem.isExpanded)
                        {
                            EditorGUI.indentLevel = 0;

#if ADDRESSABLES_ENABLED
                                var newSource = (PoolBoss.PrefabSource)EditorGUILayout.EnumPopup("Prefab Source", poolItem.prefabSource);
                                if (newSource != poolItem.prefabSource)
                                {
                                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _miniPool, "change Prefab Source");
                                    poolItem.prefabSource = newSource;

                                    if (poolItem.prefabSource == PoolBoss.PrefabSource.Addressable)
                                    {
                                        poolItem.prefabTransform = null; // clear it out to eliminate references
                                    }
                                }
#endif
                            switch (poolItem.prefabSource)
                            {
                                case PoolBoss.PrefabSource.Prefab:
                                    var newPrefab =
                                        (Transform)
                                            EditorGUILayout.ObjectField("Prefab", poolItem.prefabTransform, typeof(Transform),
                                                false);
                                    if (newPrefab != poolItem.prefabTransform)
                                    {
                                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _miniPool, "change Pool Item Prefab");
                                        poolItem.prefabTransform = newPrefab;
                                    }
                                    break;
#if ADDRESSABLES_ENABLED
                                    case PoolBoss.PrefabSource.Addressable:
                                        var itemNumber = _miniPool.poolItems.FindIndex(delegate (PoolBossItem item)
                                        {
                                            return item == poolItem;
                                        });

                                        serializedObject.Update();

                                        var poolItemsProp = serializedObject.FindProperty(nameof(PoolBoss.poolItems));
                                        var poolItemProp = poolItemsProp.GetArrayElementAtIndex(itemNumber).FindPropertyRelative(nameof(PoolBossItem.prefabAddressable));

                                        EditorGUILayout.PropertyField(poolItemProp, new GUIContent("Prefab Addressable", "Drag an Addressable prefab here"), true);

                                        serializedObject.ApplyModifiedProperties(); 
                                        break;
#endif
                            }

                            var newPreloadQty = EditorGUILayout.IntSlider("Preload Qty", poolItem.instancesToPreload, 0,
                                10000);
                            if (newPreloadQty != poolItem.instancesToPreload)
                            {
                                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _miniPool,
                                    "change Pool Item Preload Qty");
                                poolItem.instancesToPreload = newPreloadQty;
                            }
                            if (poolItem.instancesToPreload == 0)
                            {
                                DTInspectorUtility.ShowColorWarningBox(
                                    "You have set the Preload Qty to 0. This prefab will not be in the Pool.");
                            }

                            var newAllow = EditorGUILayout.Toggle("Allow Instantiate More",
                                poolItem.allowInstantiateMore);
                            if (newAllow != poolItem.allowInstantiateMore)
                            {
                                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _miniPool,
                                    "toggle Allow Instantiate More");
                                poolItem.allowInstantiateMore = newAllow;
                            }

                            if (poolItem.allowInstantiateMore)
                            {
                                var newLimit = EditorGUILayout.IntSlider("Item Limit", poolItem.itemHardLimit,
                                    poolItem.instancesToPreload, 10000);
                                if (newLimit != poolItem.itemHardLimit)
                                {
                                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _miniPool, "change Item Limit");
                                    poolItem.itemHardLimit = newLimit;
                                }
                            }
                            else
                            {
                                var newRecycle = EditorGUILayout.Toggle("Recycle Oldest", poolItem.allowRecycle);
                                if (newRecycle != poolItem.allowRecycle)
                                {
                                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _miniPool, "toggle Recycle Oldest");
                                    poolItem.allowRecycle = newRecycle;
                                }
                            }

                            if (poolItem.prefabTransform != null) {
                                var navMeshAgent = poolItem.prefabTransform.GetComponent<NavMeshAgent>();
                                if (navMeshAgent != null) {
                                    var newAgent = EditorGUILayout.Toggle(new GUIContent("Enable NavMeshAgent", "Check this to enable NavMeshAgent component whenever spawned"), poolItem.enableNavMeshAgentOnSpawn);
                                    if (newAgent != poolItem.enableNavMeshAgentOnSpawn) {
										UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _miniPool, "toggle Enable NavMeshAgent On Spawn");
                                        poolItem.enableNavMeshAgentOnSpawn = newAgent;
                                    }

                                    if (poolItem.enableNavMeshAgentOnSpawn) {
                                        var newDelay = EditorGUILayout.IntSlider("NavMeshAgent Frames Delay", poolItem.delayNavMeshEnableByFrames, 0, 200);
                                        if (newDelay != poolItem.delayNavMeshEnableByFrames) {
											UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _miniPool, "change NavMeshAgent Frames Delay");
                                            poolItem.delayNavMeshEnableByFrames = newDelay;
                                        }
                                    }
                                }
                            }
                        }

                        switch (buttonPressed)
                        {
                            case DTInspectorUtility.FunctionButtons.Remove:
                                itemToRemove = poolItem;
                                break;
                            case DTInspectorUtility.FunctionButtons.Add:
                                indexToInsertAt = _miniPool.poolItems.IndexOf(poolItem);
                                selectedCategory = cat;
                                break;
                            case DTInspectorUtility.FunctionButtons.DespawnAll:
                                PoolBoss.DespawnAllOfPrefab(poolItem.prefabTransform);
                                break;
                            case DTInspectorUtility.FunctionButtons.Copy:
                                itemToClone = poolItem;
                                break;
                        }

                        EditorGUILayout.EndVertical();
                    }

                    if (matchingItems.Count > 0 && !hasOpenBox)
                    {
                        DTInspectorUtility.EndGroupedControls();
                        DTInspectorUtility.VerticalSpace(2);
                    }
                }

                if (hasOpenBox)
                {
                    DTInspectorUtility.EndGroupedControls();
                }

                DTInspectorUtility.VerticalSpace(2);
            }

            if (indexToShiftUp.HasValue)
            {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _miniPool, "shift up Category");
                var item = _miniPool._categories[indexToShiftUp.Value];
                _miniPool._categories.Insert(indexToShiftUp.Value - 1, item);
                _miniPool._categories.RemoveAt(indexToShiftUp.Value + 1);
                _isDirty = true;
            }

            if (indexToShiftDown.HasValue)
            {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _miniPool, "shift down Category");
                var index = indexToShiftDown.Value + 1;
                var item = _miniPool._categories[index];
                _miniPool._categories.Insert(index - 1, item);
                _miniPool._categories.RemoveAt(index + 1);
                _isDirty = true;
            }

            if (catToDelete != null)
            {
                if (_miniPool.poolItems.FindAll(delegate (PoolBossItem x)
                {
                    return x.categoryName == catToDelete.CatName;
                }).Count > 0)
                {
                    DTInspectorUtility.ShowAlert("You cannot delete a Category with Pool Items in it. Move or delete the items first.");
                }
                else if (_miniPool._categories.Count <= 1)
                {
                    DTInspectorUtility.ShowAlert("You cannot delete the last Category.");
                }
                else
                {
                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _miniPool, "Delete Category");
                    _miniPool._categories.Remove(catToDelete);
                    _isDirty = true;
                }
            }

            if (catRenaming != null)
            {
                // ReSharper disable once ForCanBeConvertedToForeach
                var isValidName = true;

                if (string.IsNullOrEmpty(catRenaming.ProspectiveName))
                {
                    isValidName = false;
                    DTInspectorUtility.ShowAlert("You cannot have a blank Category name.");
                }

                // ReSharper disable once ForCanBeConvertedToForeach
                for (var c = 0; c < _miniPool._categories.Count; c++)
                {
                    var cat = _miniPool._categories[c];
                    // ReSharper disable once InvertIf
                    if (cat != catRenaming && cat.CatName == catRenaming.ProspectiveName)
                    {
                        isValidName = false;
                        DTInspectorUtility.ShowAlert("You already have a Category named '" + catRenaming.ProspectiveName + "'. Category names must be unique.");
                    }
                }

                if (isValidName)
                {
                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _miniPool, "Undo change Category name.");

                    // ReSharper disable once ForCanBeConvertedToForeach
                    for (var i = 0; i < _miniPool.poolItems.Count; i++)
                    {
                        var item = _miniPool.poolItems[i];
                        if (item.categoryName == catRenaming.CatName)
                        {
                            item.categoryName = catRenaming.ProspectiveName;
                        }
                    }

                    catRenaming.CatName = catRenaming.ProspectiveName;
                    catRenaming.IsEditing = false;
                    _isDirty = true;
                }
            }

            if (catEditing != null)
            {
                // ReSharper disable once ForCanBeConvertedToForeach
                for (var c = 0; c < _miniPool._categories.Count; c++)
                {
                    var cat = _miniPool._categories[c];
                    // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
                    if (catEditing == cat)
                    {
                        cat.IsEditing = true;
                    }
                    else
                    {
                        cat.IsEditing = false;
                    }

                    _isDirty = true;
                }
            }

            if (itemToRemove != null)
            {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _miniPool, "remove Pool Item");
                _miniPool.poolItems.Remove(itemToRemove);
            }

            if (itemToClone != null)
            {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _miniPool, "clone Pool Item");
                var newItem = itemToClone.Clone();

                var oldIndex = _miniPool.poolItems.IndexOf(itemToClone);

                _miniPool.poolItems.Insert(oldIndex, newItem);
            }

            if (indexToInsertAt.HasValue)
            {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _miniPool, "insert Pool Item");
                _miniPool.poolItems.Insert(indexToInsertAt.Value, new PoolBossItem
                {
                    categoryName = selectedCategory.CatName
                });
            }

            if (GUI.changed || _isDirty)
            {
                EditorUtility.SetDirty(target); // or it won't save the data!!
            }

            //DrawDefaultInspector();
        }

        private void ExpandCollapseAll(bool isExpand)
        {
            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _miniPool, "toggle expand / collapse all Pool Boss Items");

            foreach (var cat in _miniPool._categories)
            {
                cat.IsExpanded = isExpand;
            }

            foreach (var item in _miniPool.poolItems)
            {
                item.isExpanded = isExpand;
            }
        }

        private void ExpandCollapseCategory(string category, bool isExpand)
        {
            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _miniPool, "toggle expand / collapse all items in Category");

            foreach (var item in _miniPool.poolItems)
            {
                if (item.categoryName != category)
                {
                    continue;
                }

                item.isExpanded = isExpand;
            }
        }

        private void AddPoolItem(UnityEngine.Object o)
        {
            // ReSharper disable once PossibleNullReferenceException
            var go = (o as GameObject);
            if (go == null)
            {
                DTInspectorUtility.ShowAlert("You dragged an object which was not a Game Object. Not adding to Pool Boss.");
                return;
            }

			var prefabName = PoolBoss.GetPrefabName(go.transform); // add PoolableInfo
			if (prefabName == "") { }

            var newItem = new PoolBossItem
            {
                categoryName = _miniPool.addToCategoryName,
                prefabSource = _miniPool.newItemPrefabSource
            };

            switch (_miniPool.newItemPrefabSource)
            {
                case PoolBoss.PrefabSource.Prefab:
                    newItem.prefabTransform = go.transform;
                    break;
#if ADDRESSABLES_ENABLED
                case PoolBoss.PrefabSource.Addressable: 
                    newItem.prefabAddressable = CGKAddressableEditorHelper.CreateAssetReferenceFromObject(go.transform);
                    break;
#endif
            }

            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _miniPool, "add Pool Boss Item");

            _miniPool.poolItems.Add(newItem);
        }

        private void CreateCategory()
        {
            if (string.IsNullOrEmpty(_miniPool.newCategoryName))
            {
                DTInspectorUtility.ShowAlert("You cannot have a blank Category name.");
                return;
            }

            // ReSharper disable once ForCanBeConvertedToForeach
            for (var c = 0; c < _miniPool._categories.Count; c++)
            {
                var cat = _miniPool._categories[c];
                // ReSharper disable once InvertIf
                if (cat.CatName == _miniPool.newCategoryName)
                {
                    DTInspectorUtility.ShowAlert("You already have a Category named '" + _miniPool.newCategoryName + "'. Category names must be unique.");
                    return;
                }
            }

            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _miniPool, "Create New Category");

            var newCat = new PoolBossCategory
            {
                CatName = _miniPool.newCategoryName,
                ProspectiveName = _miniPool.newCategoryName,
            };

            _miniPool._categories.Add(newCat);
        }

        private void ImportPoolBossCategories(PoolBoss pool)
        {
            for (var i = 0; i < pool._categories.Count; i++)
            {
                var catName = pool._categories[i].CatName;
                var match = _miniPool._categories.Find(delegate (PoolBossCategory cat)
                {
                    return cat.CatName == catName;
                });
                if (match != null) { 
                    continue;
                }

                _miniPool._categories.Add(new PoolBossCategory
                {
                    CatName = catName,
                    IsEditing = false,
                    IsExpanded = true,
                    CatIsTemporary = false,
                    ProspectiveName = catName
                });

                _isDirty = true;
            }
        }

        private string PoolBossItemName(PoolBossItem item)
        {
            switch (item.prefabSource)
            {
                case PoolBoss.PrefabSource.Prefab:
                    return item.prefabTransform == null ? string.Empty : item.prefabTransform.name;
#if ADDRESSABLES_ENABLED
                case PoolBoss.PrefabSource.Addressable:
                    return CGKAddressableEditorHelper.EditTimeAddressableName(item.prefabAddressable);
#endif
                default:
                    throw new KeyNotFoundException(item.prefabSource.ToString());
            }
        }
    }
}