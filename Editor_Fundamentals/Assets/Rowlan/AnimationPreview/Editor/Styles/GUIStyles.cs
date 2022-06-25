using UnityEditor;
using UnityEngine;

namespace Rowlan.AnimationPreview
{
    /// <summary>
    /// Editor styles
    /// </summary>
    public class GUIStyles
    {
        private static GUIStyle _boxTitleStyle;
        public static GUIStyle BoxTitleStyle
        {
            get
            {
                if (_boxTitleStyle == null)
                {
                    _boxTitleStyle = new GUIStyle("Label");
                    _boxTitleStyle.fontStyle = FontStyle.BoldAndItalic;
                }
                return _boxTitleStyle;
            }
        }

        private static GUIStyle _groupTitleStyle;
        public static GUIStyle GroupTitleStyle
        {
            get
            {
                if (_groupTitleStyle == null)
                {
                    _groupTitleStyle = new GUIStyle("Label");
                    _groupTitleStyle.fontStyle = FontStyle.Bold;
                }
                return _groupTitleStyle;
            }
        }

        private static GUIStyle _toolbarButtonStyle;
        public static GUIStyle ToolbarButtonStyle
        {
            get
            {
                if (_toolbarButtonStyle == null)
                {
                    _toolbarButtonStyle = new GUIStyle("Button");
                    _toolbarButtonStyle.fixedWidth = 32f;
                    _toolbarButtonStyle.fixedHeight = EditorGUIUtility.singleLineHeight + 1;

                }
                return _toolbarButtonStyle;
            }
        }

        public static Color DefaultBackgroundColor = GUI.backgroundColor;
        public static Color ErrorBackgroundColor = new Color(1f, 0f, 0f, 1f); // red
        public static Color PlayBackgroundColor = new Color(0f, 1f, 0f, 1f); // green
        public static Color SelectedClipBackgroundColor = new Color(1f, 1f, 0f, 1f); // yellow

    }
}