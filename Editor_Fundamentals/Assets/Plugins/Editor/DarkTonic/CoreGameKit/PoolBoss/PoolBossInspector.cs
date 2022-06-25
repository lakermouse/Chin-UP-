using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;

using UnityEngine;
using UnityEngine.AI;

using Object = UnityEngine.Object;

namespace DarkTonic.CoreGameKit.EditorScripts
{
    [CustomEditor(typeof(PoolBoss))]
    // ReSharper disable once CheckNamespace
    public class PoolBossInspector : Editor
    {
        public const string DoNotDestroyPoolItem = "This will destroy the Pool Item. Pool Items should only be despawned, never destroyed.";

        private PoolBoss _pool;
        private bool _isDirty;

        // ReSharper disable once FunctionComplexityOverflow
        public override void OnInspectorGUI()
        {
            EditorGUI.indentLevel = 0;

            _pool = (PoolBoss)target;

            if (DTInspectorUtility.IsInPrefabMode(_pool.gameObject))
            {
                DTInspectorUtility.PrefabModeDoNotEdit();
                return;
            }

            _isDirty = false;
            LevelSettings.Instance = null; // clear cached version

            DTInspectorUtility.DrawTexture(CoreGameKitInspectorResources.LogoTexture);
            DTInspectorUtility.HelpHeader("http://www.dtdevtools.com/docs/coregamekit/PoolBoss.htm");

            if (DTInspectorUtility.IsPrefabInProjectView(_pool.gameObject))
            {
                DTInspectorUtility.ShowSubGameObjectNotPrefabMessage();
                return;
            }

            var catNames = new List<string>(_pool._categories.Count);
            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < _pool._categories.Count; i++)
            {
                catNames.Add(_pool._categories[i].CatName);
            }

            if (!Application.isPlaying)
            {
                DTInspectorUtility.StartGroupHeader();
                var newCat = EditorGUILayout.TextField("New Category Name", _pool.newCategoryName);
                if (newCat != _pool.newCategoryName)
                {
                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _pool, "change New Category Name");
                    _pool.newCategoryName = newCat;
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

                var selCatIndex = catNames.IndexOf(_pool.addToCategoryName);

                if (selCatIndex == -1)
                {
                    selCatIndex = 0;
                    _isDirty = true;
                }

                GUI.backgroundColor = DTInspectorUtility.BrightButtonColor;

                var newIndex = EditorGUILayout.Popup("Default Item Category", selCatIndex, catNames.ToArray());
                if (newIndex != selCatIndex)
                {
                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _pool, "change Default Item Category");
                    _pool.addToCategoryName = catNames[newIndex];
                }
                GUI.backgroundColor = Color.white;
                GUI.contentColor = Color.white;
            }

            GUI.contentColor = Color.white;

            if (!Application.isPlaying)
            {
                var newCreate = EditorGUILayout.Toggle("Create Items On Start", _pool.autoCreatePools);
                if (newCreate != _pool.autoCreatePools)
                {
                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _pool, "toggle Create Items On Start");
                    _pool.autoCreatePools = newCreate;
                }
                if (!_pool.autoCreatePools)
                {
                    DTInspectorUtility.ShowLargeBarAlertBox("You must call PoolBoss.Initialize method from code to create your pools.");
                }

                var newFrames = EditorGUILayout.IntSlider(new GUIContent("Initialize Time (Frames)", "You can increase this value to make the initial pool creation take more frames. Defaults to 1. Max of the 100 or number of different prefabs, whichever is less."), _pool.framesForInit, 1, 1000);
                if (newFrames != _pool.framesForInit)
                {
                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _pool, "change Initialize Time (Frames)");
                    _pool.framesForInit = newFrames;
                }
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

            var visiblePoolItems = _pool.poolItems;

            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < visiblePoolItems.Count; i++)
            {
                var item = visiblePoolItems[i];
                if (catNames.Contains(item.categoryName))
                {
                    continue;
                }

                item.categoryName = catNames[0];
                _isDirty = true;
            }

            var newAutoAdd = EditorGUILayout.Toggle(new GUIContent("Auto-Add Missing Items", "Auto-Add Missing Items to top Category"), _pool.autoAddMissingPoolItems);
            if (newAutoAdd != _pool.autoAddMissingPoolItems)
            {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _pool, "toggle Auto-Add Missing Items");
                _pool.autoAddMissingPoolItems = newAutoAdd;
            }

            var newAllowDisabled = EditorGUILayout.Toggle(new GUIContent("Can Disabled Obj. Despawn", "Allow Disabled Game Objects To Despawn"), _pool.allowDespawningInactive);
            if (newAllowDisabled != _pool.allowDespawningInactive)
            {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _pool, "toggle Can Disabled Obj. Despawn");
                _pool.allowDespawningInactive = newAllowDisabled;
            }

			var newAllowScene = EditorGUILayout.Toggle("Register In-Scene Items", _pool.allowInScenePoolables);
			if (newAllowScene != _pool.allowInScenePoolables)
			{
				UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _pool, "toggle Register In-Scene Items");
				_pool.allowInScenePoolables = newAllowScene;
			}

			var newLog = EditorGUILayout.Toggle("Log Messages", _pool.logMessages);
            if (newLog != _pool.logMessages)
            {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _pool, "toggle Log Messages");
                _pool.logMessages = newLog;
            }

            var newFilter = EditorGUILayout.Toggle("Use Text Item Filter", _pool.useTextFilter);
            if (newFilter != _pool.useTextFilter)
            {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _pool, "toggle Use Text Item Filter");
                _pool.useTextFilter = newFilter;
            }

            bool hasFiltered = false;

            if (_pool.useTextFilter)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(10);
                GUILayout.Label("Text Item Filter", GUILayout.Width(140));
                var newTextFilter = GUILayout.TextField(_pool.textFilter, GUILayout.Width(180));
                if (newTextFilter != _pool.textFilter)
                {
                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _pool, "change Text Item Filter");
                    _pool.textFilter = newTextFilter;
                }
                GUILayout.Space(10);
                GUI.contentColor = DTInspectorUtility.BrightButtonColor;
                if (GUILayout.Button("Clear", EditorStyles.toolbarButton, GUILayout.Width(70)))
                {
                    _pool.textFilter = string.Empty;
                }
                GUI.contentColor = Color.white;
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Separator();

                var unfilteredCount = visiblePoolItems.Count;

                if (!string.IsNullOrEmpty(_pool.textFilter))
                {
                    visiblePoolItems = visiblePoolItems.FindAll(delegate (PoolBossItem x)
                    {
                        return x.prefabTransform != null && x.prefabTransform.name.IndexOf(_pool.textFilter, StringComparison.OrdinalIgnoreCase) >= 0;
                    });
                }

                var hiddenCount = unfilteredCount - visiblePoolItems.Count;
                if (hiddenCount > 0)
                {
                    DTInspectorUtility.ShowLargeBarAlertBox(string.Format("{0}/{1} item(s) filtered out.", hiddenCount, unfilteredCount));
                    hasFiltered = true;
                }
            }

            var hadNoListener = _pool.listener == null;
            var newListener = (PoolBossListener)EditorGUILayout.ObjectField("Listener", _pool.listener, typeof(PoolBossListener), true);
            if (newListener != _pool.listener)
            {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _pool, "assign Listener");
                _pool.listener = newListener;
                if (hadNoListener && _pool.listener != null)
                {
                    _pool.listener.sourceTransName = _pool.transform.name;
                }
            }

            DTInspectorUtility.StartGroupHeader(2);
            var newFireCompleted = GUILayout.Toggle(_pool.fireCompletedCustomEvent, " Fire 'Items Created' event");
            if (newFireCompleted != _pool.fireCompletedCustomEvent)
            {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _pool, "toggle Fire 'Items Created' event");
                _pool.fireCompletedCustomEvent = newFireCompleted;
            }
            EditorGUILayout.EndVertical();
            if (_pool.fireCompletedCustomEvent)
            {
                DTInspectorUtility.BeginGroupedControls();

                DTInspectorUtility.ShowColorWarningBox("When items have finished creating, fire the Custom Events below");

                EditorGUILayout.BeginHorizontal();
                GUI.contentColor = DTInspectorUtility.AddButtonColor;
                GUILayout.Space(6);
                if (GUILayout.Button(new GUIContent("Add", "Click to add a Custom Event"), EditorStyles.toolbarButton, GUILayout.Width(50)))
                {
                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _pool, "Add 'Items Created' Custom Event");
                    _pool.completedCustomEvents.Add(new CGKCustomEventToFire());
                }
                GUI.contentColor = Color.white;

                EditorGUILayout.EndHorizontal();

                if (_pool.completedCustomEvents.Count == 0)
                {
                    DTInspectorUtility.ShowColorWarningBox("You have no Custom Events selected to fire.");
                }

                DTInspectorUtility.VerticalSpace(2);

                int? indexToDelete = null;

                for (var i = 0; i < _pool.completedCustomEvents.Count; i++)
                {
                    var anEvent = _pool.completedCustomEvents[i].CustomEventName;

                    var buttonClicked = DTInspectorUtility.FunctionButtons.None;
                    anEvent = DTInspectorUtility.SelectCustomEventForVariable(ref _isDirty, anEvent, _pool, "Custom Event", ref buttonClicked);

                    if (buttonClicked == DTInspectorUtility.FunctionButtons.Remove)
                    {
                        indexToDelete = i;
                    }

                    if (anEvent == _pool.completedCustomEvents[i].CustomEventName)
                    {
                        continue;
                    }

                    _pool.completedCustomEvents[i].CustomEventName = anEvent;
                }

                if (indexToDelete.HasValue)
                {
                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _pool, "Remove 'Items Created' Custom Event");
                    _pool.completedCustomEvents.RemoveAt(indexToDelete.Value);
                }

                DTInspectorUtility.EndGroupedControls();
            }

            EditorGUILayout.EndVertical();

            var newShow = EditorGUILayout.Toggle("Show Legend", _pool.showLegend);
            if (newShow != _pool.showLegend)
            {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _pool, "toggle Show Legend");
                _pool.showLegend = newShow;
            }

            if (_pool.showLegend)
            {
                GUI.contentColor = DTInspectorUtility.BrightButtonColor;
                DTInspectorUtility.BeginGroupedControls();

                GUILayout.Label("Legend", EditorStyles.boldLabel);

                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(6);
                GUILayout.Button(
                    new GUIContent(CoreGameKitInspectorResources.DamageTexture,
                        "Click to deal 1 damage to all Killables"), EditorStyles.toolbarButton, GUILayout.Width(24));
                GUILayout.Label("Deal 1 Damage To All");

                GUILayout.Space(6);
                GUILayout.Button(
                    new GUIContent(CoreGameKitInspectorResources.KillTexture, "Click to kill all Killables"),
                    EditorStyles.toolbarButton, GUILayout.Width(24));
                GUILayout.Label("Kill All");

                GUILayout.Space(6);
                GUILayout.Button(
                    new GUIContent(CoreGameKitInspectorResources.DespawnTexture, "Click to despawn prefabs"),
                    EditorStyles.toolbarButton, GUILayout.Width(24));
                GUILayout.Label("Despawn All");

                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
                DTInspectorUtility.EndGroupedControls();
                EditorGUILayout.Separator();
            }

            EditorGUI.indentLevel = 0;
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Actions", GUILayout.Width(100));
            GUI.contentColor = DTInspectorUtility.BrightButtonColor;

            GUILayout.FlexibleSpace();

            var allExpanded = true;
            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < _pool._categories.Count; i++)
            {
                if (_pool._categories[i].IsExpanded)
                {
                    continue;
                }
                allExpanded = false;
                break;
            }

            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < visiblePoolItems.Count; i++)
            {
                if (visiblePoolItems[i].isExpanded)
                {
                    continue;
                }
                allExpanded = false;
                break;
            }

            if (Application.isPlaying)
            {
                if (GUILayout.Button(new GUIContent("Clear Peaks"), EditorStyles.toolbarButton, GUILayout.Width(80)))
                {
                    ClearAllPeaks();
                    _isDirty = true;
                }

                GUILayout.Space(6);
            }

            var buttonTooltip = allExpanded ? "Click to collapse all categories and items" : "Click to expand all categories and items";
            var buttonText = allExpanded ? "Collapse All" : "Expand All";
            if (GUILayout.Button(new GUIContent(buttonText, buttonTooltip), EditorStyles.toolbarButton, GUILayout.Width(80)))
            {
                ExpandCollapseAll(!allExpanded);
            }

            if (Application.isPlaying)
            {
                if (GUILayout.Button(new GUIContent(CoreGameKitInspectorResources.DamageTexture, "Click to deal 1 damage to all Killables"), EditorStyles.toolbarButton, GUILayout.Width(24)))
                {
                    SpawnUtility.DamageAllPrefabs(1);
                    _isDirty = true;
                }

                if (GUILayout.Button(new GUIContent(CoreGameKitInspectorResources.KillTexture, "Click to kill all Killables"), EditorStyles.toolbarButton, GUILayout.Width(24)))
                {
                    SpawnUtility.KillAllPrefabs();
                    _isDirty = true;
                }

                if (GUILayout.Button(new GUIContent(CoreGameKitInspectorResources.DespawnTexture, "Click to despawn prefabs"), EditorStyles.toolbarButton, GUILayout.Width(24)))
                {
                    SpawnUtility.DespawnAllPrefabs();
                    _isDirty = true;
                }
                GUILayout.Space(6);
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Separator();

            GUI.backgroundColor = Color.white;

            if (!Application.isPlaying)
            {
#if ADDRESSABLES_ENABLED
            var newSource = (PoolBoss.PrefabSource)EditorGUILayout.EnumPopup("Create Items As", _pool.newItemPrefabSource);
            if (newSource != _pool.newItemPrefabSource)
            {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _pool, "change Create Items As");
                _pool.newItemPrefabSource = newSource;
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
            for (var c = 0; c < _pool._categories.Count; c++)
            {
                var cat = _pool._categories[c];

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
                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _pool, "toggle expand Pool Boss Category");
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

                        if (c < _pool._categories.Count - 1)
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

                var catItemsFiltered = 0;
                if (hasFiltered)
                {
                    var totalCount = _pool.poolItems.FindAll(delegate (PoolBossItem x)
                    {
                        return cat.CatName == x.categoryName;
                    }).Count;

                    catItemsFiltered = totalCount - matchingItems.Count;
                }

                bool hasOpenBox = false;

                if (catItemsFiltered > 0)
                {
                    DTInspectorUtility.BeginGroupedControls();
                    DTInspectorUtility.ShowLargeBarAlertBox(string.Format("This Category has {0} items filtered out.", catItemsFiltered));
                    hasOpenBox = true;
                }
                else if (!hasItems)
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
                                    "This prefab contains a Trail Renderer with auto-destruct enabled. " + DoNotDestroyPoolItem);
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
                            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _pool, "toggle expand Pool Item");
                            poolItem.isExpanded = state;
                        }

                        if (Application.isPlaying)
                        {
                            GUILayout.FlexibleSpace();

                            var hasPrefab = false;
                            switch (poolItem.prefabSource)
                            {
                                case PoolBoss.PrefabSource.Prefab:
                                    hasPrefab = poolItem.prefabTransform != null;
                                    break;
#if ADDRESSABLES_ENABLED
                            case PoolBoss.PrefabSource.Addressable:
                                hasPrefab = CGKAddressableEditorHelper.IsAddressableValid(poolItem.prefabAddressable);
                                break;
#endif
                            }

                            if (hasPrefab)
                            {
                                var itemInfo = PoolBoss.PoolItemInfoByName(itemName);
                                GUI.contentColor = DTInspectorUtility.BrightButtonColor;

                                if (itemInfo != null && itemInfo.SpawnedClones.Count > 0)
                                {
									if (poolItem.gameObject != null && poolItem.gameObject.GetComponent<Killable>() != null)
                                    {
                                        if (GUILayout.Button(new GUIContent(CoreGameKitInspectorResources.DamageTexture, "Click to damage all of this prefab"), EditorStyles.toolbarButton, GUILayout.Width(24)))
                                        {
                                            SpawnUtility.DamageAllOfPrefab(poolItem.prefabTransform, 1);
                                            _isDirty = true;
                                        }
                                        if (GUILayout.Button(new GUIContent(CoreGameKitInspectorResources.KillTexture, "Click to kill all of this prefab"), EditorStyles.toolbarButton, GUILayout.Width(24)))
                                        {
                                            SpawnUtility.KillAllOfPrefab(poolItem.prefabTransform);
                                            _isDirty = true;
                                        }
                                    }
                                    if (GUILayout.Button(new GUIContent(CoreGameKitInspectorResources.DespawnTexture, "Click to despawn all of this prefab"),
                                        EditorStyles.toolbarButton, GUILayout.Width(24)))
                                    {
                                        SpawnUtility.DespawnAllOfPrefab(poolItem.prefabTransform);
                                        _isDirty = true;
                                    }
                                }

                                GUI.contentColor = DTInspectorUtility.BrightTextColor;
                                if (itemInfo != null)
                                {
                                    var spawnedCount = itemInfo.SpawnedClones.Count;
                                    var despawnedCount = itemInfo.DespawnedClones.Count;
                                    // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
                                    if (spawnedCount == 0)
                                    {
                                        GUI.backgroundColor = DTInspectorUtility.DeleteButtonColor;
                                    }
                                    else
                                    {
                                        GUI.backgroundColor = DTInspectorUtility.BrightTextColor;
                                    }
                                    var content = new GUIContent(string.Format("{0} / {1} Spawned", spawnedCount, despawnedCount + spawnedCount),
                                            "Click here to select all spawned items.");
                                    if (GUILayout.Button(content, EditorStyles.toolbarButton, GUILayout.Width(110)))
                                    {
                                        var obj = new List<GameObject>();
                                        foreach (var t in itemInfo.SpawnedClones)
                                        {
                                            if (t == null)
                                            {
                                                LevelSettings.LogIfNew("1 of more of your pooled Game Object has been destroyed! Please check for any scripts that destroy Game Objects. Pool Boss cannot recover from this.");
                                                continue;
                                            }
                                            obj.Add(t.gameObject);
                                        }

                                        if (obj.Count > 0)
                                        {
                                            Selection.objects = obj.ToArray();
                                        }
                                    }

                                    content = new GUIContent("Pk: " + itemInfo.Peak, "Click to reset peak to zero.");
                                    if (Time.realtimeSinceStartup - itemInfo.PeakTime < .2f)
                                    {
                                        GUI.backgroundColor = DTInspectorUtility.AddButtonColor;
                                    }
                                    else if (itemInfo.Peak == 0)
                                    {
                                        GUI.backgroundColor = DTInspectorUtility.DeleteButtonColor;
                                    }
                                    else
                                    {
                                        GUI.backgroundColor = DTInspectorUtility.BrightTextColor;
                                    }

                                    if (GUILayout.Button(content, EditorStyles.miniButton, GUILayout.Width(64)))
                                    {
                                        itemInfo.Peak = Math.Max(0, itemInfo.SpawnedClones.Count);
                                        itemInfo.PeakTime = Time.realtimeSinceStartup;
                                        _isDirty = true;
                                        _pool._changes++;
                                    }
                                    GUI.backgroundColor = Color.white;
                                }
                            }
                            GUI.contentColor = Color.white;
                        }
                        else
                        {
                            GUI.backgroundColor = DTInspectorUtility.BrightButtonColor;
                            var selCatIndex = catNames.IndexOf(poolItem.categoryName);
                            var newCat = EditorGUILayout.Popup(selCatIndex, catNames.ToArray(), GUILayout.Width(130));
                            if (newCat != selCatIndex)
                            {
                                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _pool, "change Pool Item Category");
                                poolItem.categoryName = catNames[newCat];
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
                            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _pool, "change Prefab Source");
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
                                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _pool, "change Pool Item Prefab");
                                        poolItem.prefabTransform = newPrefab;
                                    }
                                    break;
#if ADDRESSABLES_ENABLED
                            case PoolBoss.PrefabSource.Addressable:
                                var itemNumber = _pool.poolItems.FindIndex(delegate (PoolBossItem item)
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
                                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _pool,
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
                                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _pool,
                                    "toggle Allow Instantiate More");
                                poolItem.allowInstantiateMore = newAllow;
                            }

                            if (poolItem.allowInstantiateMore)
                            {
                                var newLimit = EditorGUILayout.IntSlider("Item Limit", poolItem.itemHardLimit,
                                    poolItem.instancesToPreload, 10000);
                                if (newLimit != poolItem.itemHardLimit)
                                {
                                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _pool, "change Item Limit");
                                    poolItem.itemHardLimit = newLimit;
                                }
                            }
                            else
                            {
                                var newRecycle = EditorGUILayout.Toggle("Recycle Oldest", poolItem.allowRecycle);
                                if (newRecycle != poolItem.allowRecycle)
                                {
                                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _pool, "toggle Recycle Oldest");
                                    poolItem.allowRecycle = newRecycle;
                                }
                            }

                            if (poolItem.prefabTransform != null) {
                                var navMeshAgent = poolItem.prefabTransform.GetComponent<NavMeshAgent>();
                                if (navMeshAgent != null) {
                                    var newAgent = EditorGUILayout.Toggle(new GUIContent("Enable NavMeshAgent", "Check this to enable NavMeshAgent component whenever spawned"), poolItem.enableNavMeshAgentOnSpawn);
                                    if (newAgent != poolItem.enableNavMeshAgentOnSpawn) {
                                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _pool, "toggle Enable NavMeshAgent On Spawn");
                                        poolItem.enableNavMeshAgentOnSpawn = newAgent;
                                    }

                                    if (poolItem.enableNavMeshAgentOnSpawn) {
                                        var newDelay = EditorGUILayout.IntSlider("NavMeshAgent Frames Delay", poolItem.delayNavMeshEnableByFrames, 0, 200);
                                        if (newDelay != poolItem.delayNavMeshEnableByFrames) {
                                            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _pool, "change NavMeshAgent Frames Delay");
                                            poolItem.delayNavMeshEnableByFrames = newDelay;
                                        }
                                    }
                                }
                            }

							var n = PoolBoss.GetPrefabName(poolItem.prefabTransform);
							if (n == null) { }

							var poolableInfo = poolItem.prefabTransform.GetComponent<PoolableInfo>();

							if (_pool.allowInScenePoolables)
							{
								var newAllowInScene = EditorGUILayout.Toggle("Register In-Scene Items",
									poolableInfo.AllowInScenePoolables);
								if (newAllowInScene != poolableInfo.AllowInScenePoolables)
								{
									UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, poolableInfo,
										"toggle Register In-Scene Items");
									poolableInfo.AllowInScenePoolables = newAllowInScene;
								}
							}

							newLog = EditorGUILayout.Toggle("Log Messages", poolItem.logMessages);
                            if (newLog != poolItem.logMessages)
                            {
                                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _pool, "toggle Log Messages");
                                poolItem.logMessages = newLog;
                            }
                        }

                        switch (buttonPressed)
                        {
                            case DTInspectorUtility.FunctionButtons.Remove:
                                itemToRemove = poolItem;
                                break;
                            case DTInspectorUtility.FunctionButtons.Add:
                                indexToInsertAt = _pool.poolItems.IndexOf(poolItem);
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
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _pool, "shift up Category");
                var item = _pool._categories[indexToShiftUp.Value];
                _pool._categories.Insert(indexToShiftUp.Value - 1, item);
                _pool._categories.RemoveAt(indexToShiftUp.Value + 1);
                _isDirty = true;
            }

            if (indexToShiftDown.HasValue)
            {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _pool, "shift down Category");
                var index = indexToShiftDown.Value + 1;
                var item = _pool._categories[index];
                _pool._categories.Insert(index - 1, item);
                _pool._categories.RemoveAt(index + 1);
                _isDirty = true;
            }

            if (catToDelete != null)
            {
                if (_pool.poolItems.FindAll(delegate (PoolBossItem x)
                {
                    return x.categoryName == catToDelete.CatName;
                }).Count > 0)
                {
                    DTInspectorUtility.ShowAlert("You cannot delete a Category with Pool Items in it. Move or delete the items first.");
                }
                else if (_pool._categories.Count <= 1)
                {
                    DTInspectorUtility.ShowAlert("You cannot delete the last Category.");
                }
                else
                {
                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _pool, "Delete Category");
                    _pool._categories.Remove(catToDelete);
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
                for (var c = 0; c < _pool._categories.Count; c++)
                {
                    var cat = _pool._categories[c];
                    // ReSharper disable once InvertIf
                    if (cat != catRenaming && cat.CatName == catRenaming.ProspectiveName)
                    {
                        isValidName = false;
                        DTInspectorUtility.ShowAlert("You already have a Category named '" + catRenaming.ProspectiveName + "'. Category names must be unique.");
                    }
                }

                if (isValidName)
                {
                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _pool, "Undo change Category name.");

                    // ReSharper disable once ForCanBeConvertedToForeach
                    for (var i = 0; i < _pool.poolItems.Count; i++)
                    {
                        var item = _pool.poolItems[i];
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
                for (var c = 0; c < _pool._categories.Count; c++)
                {
                    var cat = _pool._categories[c];
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
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _pool, "remove Pool Item");
                _pool.poolItems.Remove(itemToRemove);
            }
            if (itemToClone != null)
            {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _pool, "clone Pool Item");
                var newItem = itemToClone.Clone();

                var oldIndex = _pool.poolItems.IndexOf(itemToClone);

                _pool.poolItems.Insert(oldIndex, newItem);
            }

            if (indexToInsertAt.HasValue)
            {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _pool, "insert Pool Item");
                _pool.poolItems.Insert(indexToInsertAt.Value, new PoolBossItem
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
            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _pool, "toggle expand / collapse all Pool Boss Items");

            foreach (var cat in _pool._categories)
            {
                cat.IsExpanded = isExpand;
            }

            foreach (var item in _pool.poolItems)
            {
                item.isExpanded = isExpand;
            }
        }

        private void ExpandCollapseCategory(string category, bool isExpand)
        {
            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _pool, "toggle expand / collapse all items in Category");

            foreach (var item in _pool.poolItems)
            {
                if (item.categoryName != category)
                {
                    continue;
                }

                item.isExpanded = isExpand;
            }
        }

        private void AddPoolItem(Object o)
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
                categoryName = _pool.addToCategoryName,
                prefabSource = _pool.newItemPrefabSource
            };

            switch (_pool.newItemPrefabSource)
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

            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _pool, "add Pool Boss Item");

            _pool.poolItems.Add(newItem);
        }

        private void ClearAllPeaks()
        {
            for (var i = 0; i < _pool.poolItems.Count; i++)
            {
                var poolItem = _pool.poolItems[i].prefabTransform;
                if (poolItem == null)
                {
                    continue;
                }

                var item = PoolBoss.PoolItemInfoByName(poolItem.name);
                item.Peak = Math.Max(0, item.SpawnedClones.Count);
                item.PeakTime = Time.realtimeSinceStartup;
            }

            _isDirty = true;
            _pool._changes++;
        }

        private void CreateCategory()
        {
            if (string.IsNullOrEmpty(_pool.newCategoryName))
            {
                DTInspectorUtility.ShowAlert("You cannot have a blank Category name.");
                return;
            }

            // ReSharper disable once ForCanBeConvertedToForeach
            for (var c = 0; c < _pool._categories.Count; c++)
            {
                var cat = _pool._categories[c];
                // ReSharper disable once InvertIf
                if (cat.CatName == _pool.newCategoryName)
                {
                    DTInspectorUtility.ShowAlert("You already have a Category named '" + _pool.newCategoryName + "'. Category names must be unique.");
                    return;
                }
            }

            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _pool, "Create New Category");

            var newCat = new PoolBossCategory
            {
                CatName = _pool.newCategoryName,
                ProspectiveName = _pool.newCategoryName,
            };

            _pool._categories.Add(newCat);
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