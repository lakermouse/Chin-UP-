#if MASTERAUDIO_ENABLED
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using DarkTonic.MasterAudio;
using DarkTonic.MasterAudio.EditorScripts;

namespace DarkTonic.CoreGameKit.EditorScripts
{
    public static class CGKMAHelper
    {
        public static void SoundGroupField(bool _maInScene, List<string> groupNames, string label, Object owner, ref bool isDirty, ref string soundGroupField)
        {
            if (_maInScene)
            {
                var existingIndex = groupNames.IndexOf(soundGroupField);

                int? groupIndex = null;

                EditorGUI.indentLevel = 0;

                var noGroup = false;
                var noMatch = false;

                if (existingIndex >= 1)
                {
                    EditorGUILayout.BeginHorizontal();
                    groupIndex = EditorGUILayout.Popup(label, existingIndex, groupNames.ToArray());
                    if (existingIndex == 1)
                    {
                        noGroup = true;
                    }

                    if (groupIndex > MasterAudio.MasterAudio.HardCodedBusOptions - 1)
                    {
                        var button = DTGUIHelper.AddSettingsButton(label);
                        switch (button)
                        {
                            case DTGUIHelper.DTFunctionButtons.Go:
                                var grp = groupNames[existingIndex];
                                var trs = MasterAudio.MasterAudio.FindGroupTransform(grp);
                                if (trs != null)
                                {
                                    Selection.activeObject = trs;
                                }
                                break;
                        }

                        var buttonPress = DTGUIHelper.AddDynamicVariationButtons();
                        var sType = groupNames[existingIndex];

                        switch (buttonPress)
                        {
                            case DTGUIHelper.DTFunctionButtons.Play:
                                DTGUIHelper.PreviewSoundGroup(sType);
                                break;
                            case DTGUIHelper.DTFunctionButtons.Stop:
                                DTGUIHelper.StopPreview(sType);
                                break;
                        }
                    }

                    EditorGUILayout.EndHorizontal();
                }
                else if (existingIndex == -1 && soundGroupField == MasterAudio.MasterAudio.NoGroupName)
                {
                    groupIndex = EditorGUILayout.Popup(label, existingIndex, groupNames.ToArray());
                }
                else
                { // non-match
                    noMatch = true;
                    var newSound = EditorGUILayout.TextField(label, soundGroupField);
                    if (newSound != soundGroupField)
                    {
                        AudioUndoHelper.RecordObjectPropertyForUndo(ref isDirty, owner, "change " + label);
                        soundGroupField = newSound;
                    }

                    var newIndex = EditorGUILayout.Popup("All Sound Groups", -1, groupNames.ToArray());
                    if (newIndex >= 0)
                    {
                        groupIndex = newIndex;
                    }
                }

                if (noGroup)
                {
                    // show nothing. takes up space on UI.
                    //DTGUIHelper.ShowColorWarning("No Sound Group specified.");
                }
                else if (noMatch)
                {
                    DTGUIHelper.ShowColorWarning("Sound Group found no match. Type in or choose one.");
                }

                if (groupIndex.HasValue)
                {
                    if (existingIndex != groupIndex.Value)
                    {
                        AudioUndoHelper.RecordObjectPropertyForUndo(ref isDirty, owner, "change " + label);
                    }
                    switch (groupIndex.Value)
                    {
                        case -1:
                            soundGroupField = MasterAudio.MasterAudio.NoGroupName;
                            break;
                        default:
                            soundGroupField = groupNames[groupIndex.Value];
                            break;
                    }
                }
            }
            else
            {
                var newSType = EditorGUILayout.TextField(label, soundGroupField);
                if (newSType != soundGroupField)
                {
                    AudioUndoHelper.RecordObjectPropertyForUndo(ref isDirty, owner, "change " + label);
                    soundGroupField = newSType;
                }
            }
        }
    }
}
#endif