using System;
using System.Security.Cryptography.X509Certificates;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;
using RengeGames.HealthBars.Extensions;
using Random = UnityEngine.Random;

namespace RengeGames.HealthBars.Editors {
    [CanEditMultipleObjects]
    [CustomEditor(typeof(RadialSegmentedHealthBar))]
    public class RadialSegmentedHealthBarEditor : Editor {
        #region serializedproperties

        private SerializedProperty parentName;
        private SerializedProperty hbName;
        private SerializedProperty usingSpriteRenderer;
        private SerializedProperty forceBuiltInShader;
        private SerializedProperty overlayColor;
        private SerializedProperty innerColor;
        private SerializedProperty borderColor;
        private SerializedProperty emptyColor;
        private SerializedProperty spaceColor;
        private SerializedProperty segmentCount;
        private SerializedProperty removeSegments;
        private SerializedProperty segmentSpacing;
        private SerializedProperty arc;
        private SerializedProperty radius;
        private SerializedProperty lineWidth;
        private SerializedProperty rotation;
        private SerializedProperty offset;
        private SerializedProperty borderWidth;
        private SerializedProperty borderSpacing;
        private SerializedProperty removeBorder;
        private SerializedProperty overlayNoiseEnabled;
        private SerializedProperty overlayNoiseScale;
        private SerializedProperty overlayNoiseStrength;
        private SerializedProperty overlayNoiseOffset;
        private SerializedProperty emptyNoiseEnabled;
        private SerializedProperty emptyNoiseScale;
        private SerializedProperty emptyNoiseStrength;
        private SerializedProperty emptyNoiseOffset;
        private SerializedProperty contentNoiseEnabled;
        private SerializedProperty contentNoiseScale;
        private SerializedProperty contentNoiseStrength;
        private SerializedProperty contentNoiseOffset;
        private SerializedProperty pulsateWhenLow;
        private SerializedProperty pulseSpeed;
        private SerializedProperty pulseActivationThreshold;
        private SerializedProperty overlayTextureEnabled;
        private SerializedProperty overlayTexture;
        private SerializedProperty overlayTextureOpacity;
        private SerializedProperty overlayTextureTiling;
        private SerializedProperty overlayTextureOffset;
        private SerializedProperty innerTextureEnabled;
        private SerializedProperty innerTexture;
        private SerializedProperty alignInnerTexture;
        private SerializedProperty innerTextureScaleWithSegments;
        private SerializedProperty innerTextureOpacity;
        private SerializedProperty innerTextureTiling;
        private SerializedProperty innerTextureOffset;
        private SerializedProperty borderTextureEnabled;
        private SerializedProperty borderTexture;
        private SerializedProperty alignBorderTexture;
        private SerializedProperty borderTextureScaleWithSegments;
        private SerializedProperty borderTextureOpacity;
        private SerializedProperty borderTextureTiling;
        private SerializedProperty borderTextureOffset;
        private SerializedProperty emptyTextureEnabled;
        private SerializedProperty emptyTexture;
        private SerializedProperty alignEmptyTexture;
        private SerializedProperty emptyTextureScaleWithSegments;
        private SerializedProperty emptyTextureOpacity;
        private SerializedProperty emptyTextureTiling;
        private SerializedProperty emptyTextureOffset;
        private SerializedProperty spaceTextureEnabled;
        private SerializedProperty spaceTexture;
        private SerializedProperty alignSpaceTexture;
        private SerializedProperty spaceTextureOpacity;
        private SerializedProperty spaceTextureTiling;
        private SerializedProperty spaceTextureOffset;
        private SerializedProperty innerGradient;
        private SerializedProperty innerGradientEnabled;
        private SerializedProperty valueAsGradientTimeInner;
        private SerializedProperty emptyGradient;
        private SerializedProperty emptyGradientEnabled;
        private SerializedProperty valueAsGradientTimeEmpty;
        private SerializedProperty variableWidthCurve;
        private SerializedProperty fillClockwise;

        #endregion

        private GUIStyle headerStyle;
        private bool generalFoldout = true;
        private bool overlayFoldout = false;
        private bool hbFoldout = true;
        private bool borderFoldout = false;
        private bool depletedFoldout = false;
        private bool emptyFoldout = false;

        private void OnEnable() {
            parentName = serializedObject.FindProperty("parentName");
            hbName = serializedObject.FindProperty("hbName");
            usingSpriteRenderer = serializedObject.FindProperty("usingSpriteRenderer");
            forceBuiltInShader = serializedObject.FindProperty("forceBuiltInShader");


            overlayColor = FindSerializedPropertyFromAutoProperty(serializedObject, RadialHealthBarProperties.OverlayColor);
            innerColor = FindSerializedPropertyFromAutoProperty(serializedObject, RadialHealthBarProperties.InnerColor);
            borderColor = FindSerializedPropertyFromAutoProperty(serializedObject, RadialHealthBarProperties.BorderColor);
            emptyColor = FindSerializedPropertyFromAutoProperty(serializedObject, RadialHealthBarProperties.EmptyColor);
            spaceColor = FindSerializedPropertyFromAutoProperty(serializedObject, RadialHealthBarProperties.SpaceColor);
            segmentCount = FindSerializedPropertyFromAutoProperty(serializedObject, RadialHealthBarProperties.SegmentCount);
            removeSegments = FindSerializedPropertyFromAutoProperty(serializedObject, RadialHealthBarProperties.RemoveSegments);
            segmentSpacing = FindSerializedPropertyFromAutoProperty(serializedObject, RadialHealthBarProperties.SegmentSpacing);
            arc = FindSerializedPropertyFromAutoProperty(serializedObject, RadialHealthBarProperties.Arc);
            radius = FindSerializedPropertyFromAutoProperty(serializedObject, RadialHealthBarProperties.Radius);
            lineWidth = FindSerializedPropertyFromAutoProperty(serializedObject, RadialHealthBarProperties.LineWidth);
            rotation = FindSerializedPropertyFromAutoProperty(serializedObject, RadialHealthBarProperties.Rotation);
            offset = FindSerializedPropertyFromAutoProperty(serializedObject, RadialHealthBarProperties.Offset);
            borderWidth = FindSerializedPropertyFromAutoProperty(serializedObject, RadialHealthBarProperties.BorderWidth);
            borderSpacing = FindSerializedPropertyFromAutoProperty(serializedObject, RadialHealthBarProperties.BorderSpacing);
            removeBorder = FindSerializedPropertyFromAutoProperty(serializedObject, RadialHealthBarProperties.RemoveBorder);
            overlayNoiseEnabled = FindSerializedPropertyFromAutoProperty(serializedObject, RadialHealthBarProperties.OverlayNoiseEnabled);
            overlayNoiseScale = FindSerializedPropertyFromAutoProperty(serializedObject, RadialHealthBarProperties.OverlayNoiseScale);
            overlayNoiseStrength = FindSerializedPropertyFromAutoProperty(serializedObject, RadialHealthBarProperties.OverlayNoiseStrength);
            overlayNoiseOffset = FindSerializedPropertyFromAutoProperty(serializedObject, RadialHealthBarProperties.OverlayNoiseOffset);
            emptyNoiseEnabled = FindSerializedPropertyFromAutoProperty(serializedObject, RadialHealthBarProperties.EmptyNoiseEnabled);
            emptyNoiseScale = FindSerializedPropertyFromAutoProperty(serializedObject, RadialHealthBarProperties.EmptyNoiseScale);
            emptyNoiseStrength = FindSerializedPropertyFromAutoProperty(serializedObject, RadialHealthBarProperties.EmptyNoiseStrength);
            emptyNoiseOffset = FindSerializedPropertyFromAutoProperty(serializedObject, RadialHealthBarProperties.EmptyNoiseOffset);
            contentNoiseEnabled = FindSerializedPropertyFromAutoProperty(serializedObject, RadialHealthBarProperties.ContentNoiseEnabled);
            contentNoiseScale = FindSerializedPropertyFromAutoProperty(serializedObject, RadialHealthBarProperties.ContentNoiseScale);
            contentNoiseStrength = FindSerializedPropertyFromAutoProperty(serializedObject, RadialHealthBarProperties.ContentNoiseStrength);
            contentNoiseOffset = FindSerializedPropertyFromAutoProperty(serializedObject, RadialHealthBarProperties.ContentNoiseOffset);
            pulsateWhenLow = FindSerializedPropertyFromAutoProperty(serializedObject, RadialHealthBarProperties.PulsateWhenLow);
            pulseSpeed = FindSerializedPropertyFromAutoProperty(serializedObject, RadialHealthBarProperties.PulseSpeed);
            pulseActivationThreshold = FindSerializedPropertyFromAutoProperty(serializedObject, RadialHealthBarProperties.PulseActivationThreshold);
            overlayTextureEnabled = FindSerializedPropertyFromAutoProperty(serializedObject, RadialHealthBarKeywords.OverlayTextureEnabled);
            overlayTexture = FindSerializedPropertyFromAutoProperty(serializedObject, RadialHealthBarProperties.OverlayTexture);
            overlayTextureOpacity = FindSerializedPropertyFromAutoProperty(serializedObject, RadialHealthBarProperties.OverlayTextureOpacity);
            overlayTextureTiling = FindSerializedPropertyFromAutoProperty(serializedObject, RadialHealthBarProperties.OverlayTextureTiling);
            overlayTextureOffset = FindSerializedPropertyFromAutoProperty(serializedObject, RadialHealthBarProperties.OverlayTextureOffset);
            innerTextureEnabled = FindSerializedPropertyFromAutoProperty(serializedObject, RadialHealthBarKeywords.InnerTextureEnabled);
            innerTexture = FindSerializedPropertyFromAutoProperty(serializedObject, RadialHealthBarProperties.InnerTexture);
            alignInnerTexture = FindSerializedPropertyFromAutoProperty(serializedObject, RadialHealthBarProperties.AlignInnerTexture);
            innerTextureScaleWithSegments = FindSerializedPropertyFromAutoProperty(serializedObject, RadialHealthBarProperties.InnerTextureScaleWithSegments);
            innerTextureOpacity = FindSerializedPropertyFromAutoProperty(serializedObject, RadialHealthBarProperties.InnerTextureOpacity);
            innerTextureTiling = FindSerializedPropertyFromAutoProperty(serializedObject, RadialHealthBarProperties.InnerTextureTiling);
            innerTextureOffset = FindSerializedPropertyFromAutoProperty(serializedObject, RadialHealthBarProperties.InnerTextureOffset);
            borderTextureEnabled = FindSerializedPropertyFromAutoProperty(serializedObject, RadialHealthBarKeywords.BorderTextureEnabled);
            borderTexture = FindSerializedPropertyFromAutoProperty(serializedObject, RadialHealthBarProperties.BorderTexture);
            alignBorderTexture = FindSerializedPropertyFromAutoProperty(serializedObject, RadialHealthBarProperties.AlignBorderTexture);
            borderTextureScaleWithSegments = FindSerializedPropertyFromAutoProperty(serializedObject, RadialHealthBarProperties.BorderTextureScaleWithSegments);
            borderTextureOpacity = FindSerializedPropertyFromAutoProperty(serializedObject, RadialHealthBarProperties.BorderTextureOpacity);
            borderTextureTiling = FindSerializedPropertyFromAutoProperty(serializedObject, RadialHealthBarProperties.BorderTextureTiling);
            borderTextureOffset = FindSerializedPropertyFromAutoProperty(serializedObject, RadialHealthBarProperties.BorderTextureOffset);
            emptyTextureEnabled = FindSerializedPropertyFromAutoProperty(serializedObject, RadialHealthBarKeywords.EmptyTextureEnabled);
            emptyTexture = FindSerializedPropertyFromAutoProperty(serializedObject, RadialHealthBarProperties.EmptyTexture);
            alignEmptyTexture = FindSerializedPropertyFromAutoProperty(serializedObject, RadialHealthBarProperties.AlignEmptyTexture);
            emptyTextureScaleWithSegments = FindSerializedPropertyFromAutoProperty(serializedObject, RadialHealthBarProperties.EmptyTextureScaleWithSegments);
            emptyTextureOpacity = FindSerializedPropertyFromAutoProperty(serializedObject, RadialHealthBarProperties.EmptyTextureOpacity);
            emptyTextureTiling = FindSerializedPropertyFromAutoProperty(serializedObject, RadialHealthBarProperties.EmptyTextureTiling);
            emptyTextureOffset = FindSerializedPropertyFromAutoProperty(serializedObject, RadialHealthBarProperties.EmptyTextureOffset);
            spaceTextureEnabled = FindSerializedPropertyFromAutoProperty(serializedObject, RadialHealthBarKeywords.SpaceTextureEnabled);
            spaceTexture = FindSerializedPropertyFromAutoProperty(serializedObject, RadialHealthBarProperties.SpaceTexture);
            alignSpaceTexture = FindSerializedPropertyFromAutoProperty(serializedObject, RadialHealthBarProperties.AlignSpaceTexture);
            spaceTextureOpacity = FindSerializedPropertyFromAutoProperty(serializedObject, RadialHealthBarProperties.SpaceTextureOpacity);
            spaceTextureTiling = FindSerializedPropertyFromAutoProperty(serializedObject, RadialHealthBarProperties.SpaceTextureTiling);
            spaceTextureOffset = FindSerializedPropertyFromAutoProperty(serializedObject, RadialHealthBarProperties.SpaceTextureOffset);
            innerGradient = FindSerializedPropertyFromAutoProperty(serializedObject, RadialHealthBarProperties.InnerGradient);
            innerGradientEnabled = FindSerializedPropertyFromAutoProperty(serializedObject, RadialHealthBarProperties.InnerGradientEnabled);
            valueAsGradientTimeInner = FindSerializedPropertyFromAutoProperty(serializedObject, RadialHealthBarProperties.ValueAsGradientTimeInner);
            emptyGradient = FindSerializedPropertyFromAutoProperty(serializedObject, RadialHealthBarProperties.EmptyGradient);
            emptyGradientEnabled = FindSerializedPropertyFromAutoProperty(serializedObject, RadialHealthBarProperties.EmptyGradientEnabled);
            valueAsGradientTimeEmpty = FindSerializedPropertyFromAutoProperty(serializedObject, RadialHealthBarProperties.ValueAsGradientTimeEmpty);
            variableWidthCurve = FindSerializedPropertyFromAutoProperty(serializedObject, RadialHealthBarProperties.VariableWidthCurve);
            fillClockwise = FindSerializedPropertyFromAutoProperty(serializedObject, RadialHealthBarProperties.FillClockwise);
        }

        private SerializedProperty FindSerializedPropertyFromAutoProperty(SerializedObject obj, string propertyName) => obj.FindProperty($"<{propertyName}>k__BackingField");
        private SerializedProperty FindValueProperty(SerializedProperty parent) => parent.FindPropertyRelative("_value");

        public override void OnInspectorGUI() {
            serializedObject.Update();

            headerStyle = GUI.skin.label;
            headerStyle.fontStyle = FontStyle.Bold;

            GUILayout.Label("Naming and Access", headerStyle);

            EditorGUILayout.PropertyField(parentName, new GUIContent() { text = "Parent Name" });
            EditorGUILayout.PropertyField(hbName, new GUIContent() { text = "Health Bar Name" });

            GUILayout.Space(10);
            GUILayout.Label("Data", headerStyle);
            EditorGUILayout.PropertyField(segmentCount, new GUIContent() { text = "Segment Count" });
            EditorGUILayout.PropertyField(removeSegments, new GUIContent() { text = "Remove Segments" });

            GUILayout.Space(10);
            GUILayout.Label("Appearance", headerStyle);

            generalFoldout = EditorGUILayout.Foldout(generalFoldout, "General Appearance");
            if (generalFoldout) {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(fillClockwise, new GUIContent("Fill Clockwise"));
                EditorGUILayout.PropertyField(segmentSpacing, new GUIContent() { text = "Segment Spacing" });
                SliderPropertyField("Arc", FindValueProperty(arc), 0, 1);
                EditorGUILayout.PropertyField(radius, new GUIContent() { text = "Radius" });
                SliderPropertyField("Line Width", FindValueProperty(lineWidth), 0, 1);
                EditorGUILayout.PropertyField(variableWidthCurve, new GUIContent() { text = "Line Width Curve" });
                SliderPropertyField("Rotation", FindValueProperty(rotation), 0, 360);
                EditorGUILayout.PropertyField(offset, new GUIContent() { text = "Offset" });
                EditorGUI.indentLevel--;
            }

            GUILayout.Space(10);
            EditorGUILayout.BeginHorizontal();
            {
                hbFoldout = EditorGUILayout.Foldout(hbFoldout, "Health Portion");
                EditorGUILayout.PropertyField(innerColor, new GUIContent() { text = "Health Color" });
            }
            EditorGUILayout.EndHorizontal();
            if (hbFoldout) {
                GUILayout.BeginVertical();
                {
                    EditorGUILayout.PropertyField(pulsateWhenLow, new GUIContent() { text = "Pulsate When Low" });
                    if (FindValueProperty(pulsateWhenLow).boolValue) {
                        EditorGUI.indentLevel++;
                        SliderPropertyField("Percent Health Activation", FindValueProperty(pulseActivationThreshold), 0, 1);
                        EditorGUILayout.PropertyField(pulseSpeed, new GUIContent() { text = "Pulse Speed" });
                        EditorGUI.indentLevel--;
                    }

                    EditorGUILayout.PropertyField(innerTextureEnabled, new GUIContent() { text = "Use Texture" });
                    if (!innerTextureEnabled.hasMultipleDifferentValues && FindValueProperty(innerTextureEnabled).boolValue) {
                        EditorGUI.indentLevel++;
                        //Texture stuff
                        EditorGUILayout.PropertyField(innerTexture, new GUIContent() { text = "Texture" });
                        EditorGUILayout.PropertyField(innerTextureScaleWithSegments, new GUIContent() { text = "Scale Texture with Segments" });
                        EditorGUILayout.PropertyField(alignInnerTexture, new GUIContent() { text = "Align Texture" });
                        SliderPropertyField("Texture Opacity", FindValueProperty(innerTextureOpacity), 0, 1);
                        EditorGUILayout.PropertyField(innerTextureTiling, new GUIContent() { text = "Texture Tiling" });
                        EditorGUILayout.PropertyField(innerTextureOffset, new GUIContent() { text = "Texture Offset" });

                        //Gradient stuff
                        EditorGUILayout.PropertyField(innerGradientEnabled, new GUIContent() { text = "Use Gradient" });
                        if (!innerGradientEnabled.hasMultipleDifferentValues && FindValueProperty(innerGradientEnabled).boolValue) {
                            EditorGUILayout.PropertyField(innerGradient, new GUIContent() { text = "Gradient" });
                            EditorGUILayout.PropertyField(valueAsGradientTimeInner, new GUIContent() { text = "Value As Gradient Time" });
                        }

                        EditorGUI.indentLevel--;
                    }

                    EditorGUILayout.PropertyField(contentNoiseEnabled, new GUIContent() { text = "Use Noise" });
                    if (!contentNoiseEnabled.hasMultipleDifferentValues && FindValueProperty(contentNoiseEnabled).boolValue) {
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(contentNoiseStrength, new GUIContent() { text = "Noise Strength" });
                        EditorGUILayout.PropertyField(contentNoiseScale, new GUIContent() { text = "Noise Scale" });
                        EditorGUILayout.PropertyField(contentNoiseOffset, new GUIContent() { text = "Noise Offset" });
                        EditorGUI.indentLevel--;
                    }
                }
                GUILayout.EndVertical();
            }

            GUILayout.Space(10);
            EditorGUILayout.BeginHorizontal();
            {
                overlayFoldout = EditorGUILayout.Foldout(overlayFoldout, "Overlay Portion");
                EditorGUILayout.PropertyField(overlayColor, new GUIContent() { text = "Overlay Color" });
            }
            EditorGUILayout.EndHorizontal();
            if (overlayFoldout) {
                GUILayout.BeginVertical();
                {
                    EditorGUILayout.PropertyField(overlayTextureEnabled, new GUIContent() { text = "Use Texture" });
                    if (FindValueProperty(overlayTextureEnabled).boolValue) {
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(overlayTexture, new GUIContent() { text = "Texture" });
                        SliderPropertyField("Texture Opacity", FindValueProperty(overlayTextureOpacity), 0, 1);
                        EditorGUILayout.PropertyField(overlayTextureTiling, new GUIContent() { text = "Texture Tiling" });
                        EditorGUILayout.PropertyField(overlayTextureOffset, new GUIContent() { text = "Texture Offset" });
                        EditorGUI.indentLevel--;
                    }

                    EditorGUILayout.PropertyField(overlayNoiseEnabled, new GUIContent() { text = "Use Noise" });
                    if (FindValueProperty(overlayNoiseEnabled).boolValue) {
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(overlayNoiseStrength, new GUIContent() { text = "Noise Strength" });
                        EditorGUILayout.PropertyField(overlayNoiseScale, new GUIContent() { text = "Noise Scale" });
                        EditorGUILayout.PropertyField(overlayNoiseOffset, new GUIContent() { text = "Noise Offset" });
                        EditorGUI.indentLevel--;
                    }
                }
                GUILayout.EndVertical();
            }

            GUILayout.Space(10);
            EditorGUILayout.BeginHorizontal();
            {
                borderFoldout = EditorGUILayout.Foldout(borderFoldout, "Border");
                EditorGUILayout.PropertyField(borderColor, new GUIContent() { text = "Border Color" });
            }
            EditorGUILayout.EndHorizontal();
            if (borderFoldout) {
                GUILayout.BeginVertical();
                {
                    SliderPropertyField("Border Width", FindValueProperty(borderWidth), 0, 1);
                    EditorGUILayout.PropertyField(borderSpacing, new GUIContent() { text = "Border Spacing" });

                    EditorGUILayout.PropertyField(borderTextureEnabled, new GUIContent() { text = "Use Texture" });
                    if (FindValueProperty(borderTextureEnabled).boolValue) {
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(borderTexture, new GUIContent() { text = "Texture" });
                        EditorGUILayout.PropertyField(borderTextureScaleWithSegments, new GUIContent() { text = "Scale Texture with Segments" });
                        EditorGUILayout.PropertyField(alignBorderTexture, new GUIContent() { text = "Align Texture" });
                        SliderPropertyField("Texture Opacity", FindValueProperty(borderTextureOpacity), 0, 1);
                        EditorGUILayout.PropertyField(borderTextureTiling, new GUIContent() { text = "Texture Tiling" });
                        EditorGUILayout.PropertyField(borderTextureOffset, new GUIContent() { text = "Texture Offset" });
                        EditorGUI.indentLevel--;
                    }
                }
                GUILayout.EndVertical();
            }

            GUILayout.Space(10);
            EditorGUILayout.BeginHorizontal();
            {
                depletedFoldout = EditorGUILayout.Foldout(depletedFoldout, "Depleted Portion");
                EditorGUILayout.PropertyField(emptyColor, new GUIContent() { text = "Depleted Color" });
            }
            EditorGUILayout.EndHorizontal();
            if (depletedFoldout) {
                GUILayout.BeginVertical();
                {
                    SliderPropertyField("Depleted Transparency", FindValueProperty(removeBorder), 0, 1);
                    EditorGUILayout.PropertyField(emptyTextureEnabled, new GUIContent() { text = "Use Texture" });
                    if (FindValueProperty(emptyTextureEnabled).boolValue) {
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(emptyTexture, new GUIContent() { text = "Texture" });
                        EditorGUILayout.PropertyField(emptyTextureScaleWithSegments, new GUIContent() { text = "Scale Texture with Segments" });
                        EditorGUILayout.PropertyField(alignEmptyTexture, new GUIContent() { text = "Align Texture" });
                        SliderPropertyField("Texture Opacity", FindValueProperty(emptyTextureOpacity), 0, 1);
                        EditorGUILayout.PropertyField(emptyTextureTiling, new GUIContent() { text = "Texture Tiling" });
                        EditorGUILayout.PropertyField(emptyTextureOffset, new GUIContent() { text = "Texture Offset" });
                        EditorGUI.indentLevel--;

                        //Gradient stuff
                        EditorGUILayout.PropertyField(emptyGradientEnabled, new GUIContent() { text = "Use Gradient" });
                        if (!emptyGradientEnabled.hasMultipleDifferentValues && FindValueProperty(emptyGradientEnabled).boolValue) {
                            EditorGUILayout.PropertyField(emptyGradient, new GUIContent() { text = "Gradient" });
                            EditorGUILayout.PropertyField(valueAsGradientTimeEmpty, new GUIContent() { text = "Value As Gradient Time" });
                        }
                    }

                    EditorGUILayout.PropertyField(emptyNoiseEnabled, new GUIContent() { text = "Use Noise" });
                    if (FindValueProperty(emptyNoiseEnabled).boolValue) {
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(emptyNoiseStrength, new GUIContent() { text = "Noise Strength" });
                        EditorGUILayout.PropertyField(emptyNoiseScale, new GUIContent() { text = "Noise Scale" });
                        EditorGUILayout.PropertyField(emptyNoiseOffset, new GUIContent() { text = "Noise Offset" });
                        EditorGUI.indentLevel--;
                    }
                }
                GUILayout.EndVertical();
            }

            GUILayout.Space(10);
            EditorGUILayout.BeginHorizontal();
            {
                emptyFoldout = EditorGUILayout.Foldout(emptyFoldout, "Empty Space");
                EditorGUILayout.PropertyField(spaceColor, new GUIContent() { text = "Empty Space Color" });
            }
            EditorGUILayout.EndHorizontal();
            if (emptyFoldout) {
                GUILayout.BeginVertical();
                {
                    //FindValueProperty(spaceTextureEnabled).boolValue = EditorGUILayout.Toggle("Use Texture", FindValueProperty(spaceTextureEnabled).boolValue);
                    EditorGUILayout.PropertyField(spaceTextureEnabled, new GUIContent() { text = "Use Texture" });
                    if (FindValueProperty(spaceTextureEnabled).boolValue) {
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(spaceTexture, new GUIContent() { text = "Texture" });
                        EditorGUILayout.PropertyField(alignSpaceTexture, new GUIContent() { text = "Align Texture" });
                        FindValueProperty(spaceTextureOpacity).floatValue = EditorGUILayout.Slider("Texture Opacity", FindValueProperty(spaceTextureOpacity).floatValue, 0, 1);
                        EditorGUILayout.PropertyField(spaceTextureTiling, new GUIContent() { text = "Texture Tiling" });
                        EditorGUILayout.PropertyField(spaceTextureOffset, new GUIContent() { text = "Texture Offset" });
                        EditorGUI.indentLevel--;
                    }
                }
                GUILayout.EndVertical();
            }

            EditorGUILayout.Separator();
            GUILayout.Label("Other", headerStyle);
            GUILayout.BeginHorizontal();
            {
                GUILayout.BeginVertical();
                {
                    if (GUILayout.Button("Reset to defaults")) {
                        Undo.RecordObjects(serializedObject.targetObjects, "Reset RSHB");
                        foreach (var serializedObjectTargetObject in serializedObject.targetObjects) {
                            (serializedObjectTargetObject as RadialSegmentedHealthBar)?.ResetPropertiesToDefault();
                        }
                    }

                    if (GUILayout.Button("Duplicate Shader")) {
                        foreach (var serializedObjectTargetObject in serializedObject.targetObjects) {
                            if (serializedObjectTargetObject != null) {
                                var obj = serializedObjectTargetObject as RadialSegmentedHealthBar;
                                GameObject.Instantiate(obj, obj.transform.parent);
                            }
                        }
                    }

                    if (GUILayout.Button("Randomize All")) {
                        Color c = new Color();
                        Undo.RecordObjects(serializedObject.targetObjects, "Randomize RSHB");
                        foreach (var serializedObjectTargetObject in serializedObject.targetObjects) {
                            if (serializedObjectTargetObject != null) {
                                var obj = serializedObjectTargetObject as RadialSegmentedHealthBar;

                                obj.SegmentCount.Value = Mathf.Round(Random.value * 10 + 1);
                                obj.RemoveSegments.Value = Mathf.Floor(Random.value * (obj.SegmentCount.Value / 2));
                                obj.SegmentSpacing.Value = Random.value * 0.1f;
                                obj.Arc.Value = Mathf.Round(Random.value * 5) / 10;
                                obj.Radius.Value = Random.Range(0.2f, 0.4f);
                                obj.LineWidth.Value = Random.Range(0.02f, 0.1f);
                                obj.Rotation.Value = Random.value * 360;
                                obj.InnerColor.Value = c.RandomColor();
                                obj.BorderWidth.Value = Random.value * 0.05f;
                                obj.BorderSpacing.Value = Random.Range(0, obj.BorderWidth.Value + obj.Arc.Value * .1f);
                                obj.BorderColor.Value = c.RandomColor();
                                obj.RemoveBorder.Value = Random.value;
                                obj.EmptyColor.Value = c.RandomColor();
                                obj.SpaceColor.Value = c.RandomColor(true);

                                EditorApplication.QueuePlayerLoopUpdate();
                            }
                        }
                    }
                }
                GUILayout.EndVertical();
                GUILayout.BeginVertical();
                {

                    if (GUILayout.Button("Use Sprite Renderer")) {
                        foreach (var serializedObjectTargetObject in serializedObject.targetObjects) {
                            if (serializedObjectTargetObject != null) {
                                var obj = serializedObjectTargetObject as RadialSegmentedHealthBar;
                                obj.UsingSpriteRenderer = true;
                            }
                        }
                    }

                    if (GUILayout.Button("Use Image")) {
                        foreach (var serializedObjectTargetObject in serializedObject.targetObjects) {
                            if (serializedObjectTargetObject != null) {
                                var obj = serializedObjectTargetObject as RadialSegmentedHealthBar;
                                obj.UsingSpriteRenderer = false;
                            }
                        }
                    }

                    string currentlyUsing = "-";
                    if (!usingSpriteRenderer.hasMultipleDifferentValues) {
                        if(usingSpriteRenderer.boolValue)
                            currentlyUsing = "Sprite Renderer";
                        else
                            currentlyUsing = "Image";
                    }
                    GUILayout.Label("Currently using: " + currentlyUsing);

                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(forceBuiltInShader, new GUIContent("Force Built In Shader", "Toggle this if you are having render issues in your canvas!"));
                    if (EditorGUI.EndChangeCheck()) {
                        foreach (var serializedObjectTargetObject in serializedObject.targetObjects) {
                            if (serializedObjectTargetObject != null) {
                                var obj = serializedObjectTargetObject as RadialSegmentedHealthBar;
                                obj.ForceBuiltInShader = forceBuiltInShader.boolValue;
                            }
                        }
                    }
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();

            //base.OnInspectorGUI();
        }

        void SliderPropertyField(string text, SerializedProperty property, float min, float max) {
            var floatVal = property.floatValue;
            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = property.hasMultipleDifferentValues;
            floatVal = EditorGUILayout.Slider(text, floatVal, min, max);
            EditorGUI.showMixedValue = false;
            if (EditorGUI.EndChangeCheck()) {
                property.floatValue = floatVal;
            }
        }
    }
}