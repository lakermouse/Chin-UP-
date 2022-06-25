using UnityEditor;
using UnityEngine;

namespace Rowlan.AnimationPreview
{
    /// <summary>
    /// Animation preview editor which allows you to select clips of an Animator and play them inside the Unity Editor.
    /// </summary>
    [ExecuteInEditMode]
    [CustomEditor(typeof(AnimationPreview))]
    public class AnimationPreviewEditor : Editor
    {

        AnimationPreviewEditor editor;
        AnimationPreview editorTarget;

        SerializedProperty clipIndex;
        SerializedProperty clipName;
        SerializedProperty animator;
        SerializedProperty controller;

        private AnimationClip previewClip;
        private bool isPlaying = false;

        public void OnEnable()
        {
            editor = this;
            editorTarget = (AnimationPreview)target;

            clipIndex = serializedObject.FindProperty("clipIndex");
            clipName = serializedObject.FindProperty("clipName");
            animator = serializedObject.FindProperty("animator");
            controller = serializedObject.FindProperty("controller");

            // try to get the animator from the gameobject if none is specified
            if ( !HasAnimator())
            {
                editorTarget.animator = editorTarget.GetComponent<Animator>();
            }
            
            if( HasAnimator() && !HasAnimatorController())
            {
                Debug.LogWarning( $"Runtime animator controller not found for animator {editorTarget.animator.name}");
            }

            UpdateClipName();
        }

        public void OnDisable()
        {
            EditorApplication.update -= DoPreview;
        }

        #region Inspector
        public override void OnInspectorGUI()
        {
            editor.serializedObject.Update();

            bool animatorChanged = false;

            // help
            EditorGUILayout.HelpBox( 
                "Play animator clips inside the Unity editor. Press Play or the clip button to play the selected animation. Press Stop to stop continuous playing."
                + "\n\n"
                + "Setup: Create an animator controller, drag animations into the controller, assign the controller to an animator of a gameobject and drag the gameobject into the Animator slot."
                , MessageType.Info);

            // data
            EditorGUILayout.BeginVertical();
            {
                EditorGUILayout.LabelField("Clip Data", GUIStyles.BoxTitleStyle);

                EditorGUI.BeginChangeCheck();
                {
                    GUI.backgroundColor = HasAnimator() && HasAnimatorController() ? GUIStyles.DefaultBackgroundColor : GUIStyles.ErrorBackgroundColor;
                    {
                        EditorGUILayout.PropertyField(animator);

                        // visualize the controller
                        GUI.enabled = false;
                        EditorGUILayout.PropertyField(controller);
                        GUI.enabled = true;

                        if (!HasAnimator() || !HasAnimatorController())
                        {
                            EditorGUILayout.HelpBox("The animator must have a controller. Use a gameobject with an attached Animator and Controller.", MessageType.Error);
                        }

                    }
                    GUI.backgroundColor = GUIStyles.DefaultBackgroundColor;


                    if (!HasAnimatorController())
                    {
                        EditorGUILayout.HelpBox("Quick solution to get a preview of all animations:\n1. Create > Animator Controller\n2. Drag all animations into the controller\n3. Add controller to your animator ", MessageType.Info);
                    }

                }

                // stop clip in case the animator changes
                if (EditorGUI.EndChangeCheck())
                {
                    animatorChanged = true;

                    // update the controller
                    controller.objectReferenceValue = GetAnimatorController();
                }

                GUI.enabled = false;
                EditorGUILayout.PropertyField(clipIndex);
                EditorGUILayout.PropertyField(clipName);
                GUI.enabled = true;

            }
            EditorGUILayout.EndVertical();

            // control
            EditorGUILayout.BeginVertical( "box");
            {
                EditorGUILayout.LabelField("Control", GUIStyles.BoxTitleStyle);

                EditorGUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("Previous"))
                    {
                        PreviousClip();
                    }
                    if (GUILayout.Button("Next"))
                    {
                        NextClip();
                    }
                    
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                {
                    {   // Play button has special background color handling
                        GUI.backgroundColor = isPlaying ? GUIStyles.PlayBackgroundColor : GUIStyles.DefaultBackgroundColor;
                        if (GUILayout.Button("Play"))
                        {
                            PlayClip();
                        }
                        GUI.backgroundColor = GUIStyles.DefaultBackgroundColor;
                    }

                    if (GUILayout.Button("Reset"))
                    {
                        ResetClip();
                    }
                    if (GUILayout.Button("Stop"))
                    {
                        StopClip();
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();

            // clip list
            EditorGUILayout.BeginVertical();
            {
                EditorGUILayout.LabelField("Clip List", GUIStyles.BoxTitleStyle);

                if (HasAnimator() && HasAnimatorController())
                {
                    AnimationClip[] clips = GetAnimatorController().animationClips;
                    for (int i = 0; i < clips.Length; i++)
                    {
                        AnimationClip clip = clips[i];

                        bool isCurrentClip = i == editorTarget.clipIndex;

                        GUI.backgroundColor = isCurrentClip ? GUIStyles.SelectedClipBackgroundColor : GUIStyles.DefaultBackgroundColor;
                        {
                            EditorGUILayout.BeginHorizontal();
                            {
                                EditorGUILayout.PrefixLabel("Clip: " + i);

                                if (GUILayout.Button(clip.name))
                                {
                                    SetClip(i);
                                    PlayClip();
                                }

                                if (GUILayout.Button(EditorGUIUtility.IconContent("AnimationClip Icon", "Open Clip in Project"), GUIStyles.ToolbarButtonStyle))
                                {
                                    OpenClip( i);
                                }

                            }
                            EditorGUILayout.EndHorizontal();
                        }
                        GUI.backgroundColor = GUIStyles.DefaultBackgroundColor;
                    }
                }

            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();
            {
                EditorGUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("Log Clips"))
                    {
                        LogClips();
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();

            // Apply changes to the serializedProperty - always do this in the end of OnInspectorGUI.
            editor.serializedObject.ApplyModifiedProperties();

            if (animatorChanged)
            {
                StopClip();

                // reset the 
                // set index to either -1 or the first index depending on the number of animations
                editorTarget.clipIndex = !HasAnimator() || !HasAnimatorController() || GetAnimatorController().animationClips.Length == 0 ? -1 : 0;

                UpdateClipName();

                EditorUtility.SetDirty(target);

            }
        }

        /// <summary>
        /// Check if an animator is available
        /// </summary>
        /// <returns></returns>
        public bool HasAnimator()
        {
            return GetAnimator() != null;
        }

        /// <summary>
        /// Check if an animator controller is available
        /// </summary>
        /// <returns></returns>
        public bool HasAnimatorController()
        {
            return GetAnimatorController() != null;
        }

        /// <summary>
        /// Get the animator
        /// </summary>
        /// <returns></returns>
        public Animator GetAnimator()
        {
            if (animator == null)
                return null;
               
            return animator.objectReferenceValue as Animator;
        }

        /// <summary>
        /// Wrapper method to get the controller from an animator without throwing an exception if the animator is null
        /// </summary>
        /// <returns></returns>
        private RuntimeAnimatorController GetAnimatorController()
        {
            Animator animatorReference = GetAnimator();

            if (animatorReference == null)
                return null;

            return animatorReference.runtimeAnimatorController;
        }

        #endregion Inspector

        #region Clip Navigation
        private void PreviousClip()
        {
            editorTarget.clipIndex--;
            editorTarget.clipIndex = GetValidClipIndex(editorTarget.clipIndex);

            ClipChanged();
        }

        private void NextClip()
        {
            editorTarget.clipIndex++;
            editorTarget.clipIndex = GetValidClipIndex(editorTarget.clipIndex);
            ClipChanged();
        }

        private void SetClip( int clipIndex)
        {
            editorTarget.clipIndex = GetValidClipIndex(clipIndex);
            ClipChanged();

        }

        /// <summary>
        /// Open the clip file in project view
        /// </summary>
        /// <param name="clipIndex"></param>
        private void OpenClip(int clipIndex)
        {

            AnimationClip clip = GetClip(clipIndex);

            if (!clip)
                return;

            Selection.activeObject = clip;

        }

        private void ClipChanged()
        {
            if (isPlaying)
                PlayClip();
            else
                ResetClip();

            UpdateClipName();
        }

        private void UpdateClipName()
        {
            AnimationClip clip = GetClipToPreview();

            editorTarget.clipName = clip == null ? "" : clip.name;

        }

        private int GetValidClipIndex( int clipIndex)
        {
            if (!HasAnimator())
                return -1;

            int clipCount = GetAnimatorController().animationClips.Length;

            // check if there are clips at all
            if (clipCount == 0)
            {
                return -1;
            }

            if (clipIndex < 0)
            {
                return clipCount - 1;
            }

            if( clipIndex >= clipCount)
            {
                return 0;
            }

            return clipIndex;

        }

        private AnimationClip GetClipToPreview()
        {
            int clipIndex = editorTarget.clipIndex;
            if (clipIndex == -1)
                return null;

            return GetClip(clipIndex);
        }

        private AnimationClip GetClip( int clipIndex)
        {
            if (!HasAnimatorController())
                return null;

            AnimationClip[] clips = GetAnimatorController().animationClips;

            if (clipIndex >= clips.Length)
                return null;

            AnimationClip clip = clips[clipIndex];

            return clip;
        }

        #endregion Clip Navigation

        #region Clip Control
        private void PlayClip()
        {
            isPlaying = true;

            previewClip = GetClipToPreview();
            ResetClip();

            EditorApplication.update -= DoPreview;
            EditorApplication.update += DoPreview;
        }



        void DoPreview()
        {
            if (!previewClip)
                return;

            if (!HasAnimator())
                return;

            if (!isPlaying)
                return;

            previewClip.SampleAnimation(editorTarget.gameObject, Time.deltaTime);
            
            GetAnimator().Update(Time.deltaTime);
        }

        private void ResetClip()
        {
            if (!previewClip)
                return;

            previewClip.SampleAnimation(editorTarget.gameObject, 0);

            Animator animator = GetAnimator();
            animator.Play(previewClip.name, 0, 0f);
            animator.Update(0);

        }

        private void StopClip()
        {
            isPlaying = false;

            EditorApplication.update -= DoPreview;

            ResetClip();
        }
        #endregion Clip Control

        #region Logging
        private void LogClips()
        {
            if (!HasAnimator() || !HasAnimatorController())
                return;

            AnimationClip[] clips = GetAnimatorController().animationClips;

            string text = "Clips of " + GetAnimator().name + ": " + clips.Length + "\n";

            for (int i = 0; i < clips.Length; i++)
            {
                AnimationClip clip = clips[i];

                text += string.Format("{0}: {1}\n", i, clip.name);
            }

            Debug.Log(text);

        }
        #endregion Logging
    }
}

