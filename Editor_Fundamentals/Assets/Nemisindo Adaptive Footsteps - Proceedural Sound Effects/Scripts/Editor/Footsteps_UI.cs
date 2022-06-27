using UnityEngine;
using UnityEditor;
using System;


namespace Nemisindo
{
    public static class Footsteps_params
    {
        public static float Footsteps_Pace = 90.0f;
        public static float Footsteps_Firmness = 0.5f;
        public static float Footsteps_Steadiness = 1;
        public static int Footsteps_ShoeType = 0;
        public static int Footsteps_SurfaceType = 0;
        public static int Footsteps_TerrainType = 0;
        public static float Footsteps_Volume = 0.8f;
        public static bool Footsteps_Start = false;
        public static bool Footsteps_Trigger = false;
        public static float Footsteps_HeelGain    = 0.8f;
        public static float Footsteps_HeelAttack  = 0.8f;
        public static float Footsteps_HeelDecay   = 0.8f;
        public static float Footsteps_HeelSustain = 0.8f;
        public static float Footsteps_HeelRelease = 0.8f;
        public static float Footsteps_BallGain    = 0.8f;
        public static float Footsteps_BallAttack  = 0.8f;
        public static float Footsteps_BallDecay   = 0.8f;
        public static float Footsteps_BallSustain = 0.8f;
        public static float Footsteps_BallRelease = 0.8f;
        public static float Footsteps_RollGain    = 0.8f;
        public static int Footsteps_Presets = 0;
        public static int Footsteps_prevPreset = 0;
        public static int channel = 0;

        private static GUIStyle Fontstyle = new GUIStyle();

        static GUIStyle style;

        static Texture banner = (Texture)AssetDatabase.LoadAssetAtPath("Assets/Nemisindo Adaptive Footsteps - Proceedural Sound Effects/nemisindo_square.png", typeof(Texture));

        public static void create_Footsteps_customSliders()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Box("Customise footwear", GUILayout.Width(300), GUILayout.Height(20));
            GUILayout.EndHorizontal();
            GUILayout.Space(20);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Heel Gain: ", GUILayout.Width(200));
            Footsteps_HeelGain = EditorGUILayout.Slider(Footsteps_HeelGain, 0, 1, GUILayout.Width(300));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Heel Attack: ", GUILayout.Width(200));
            Footsteps_HeelAttack = EditorGUILayout.Slider(Footsteps_HeelAttack, 0.0f, 50.0f, GUILayout.Width(300));
            GUILayout.EndHorizontal();


            GUILayout.BeginHorizontal();
            GUILayout.Label("Heel Decay: ", GUILayout.Width(200));
            Footsteps_HeelDecay = EditorGUILayout.Slider(Footsteps_HeelDecay, 0.0f, 50.0f, GUILayout.Width(300));
            GUILayout.EndHorizontal();


            GUILayout.BeginHorizontal();
            GUILayout.Label("Heel Sustain: ", GUILayout.Width(200));
            Footsteps_HeelSustain = EditorGUILayout.Slider(Footsteps_HeelSustain, 0.0f, 1.0f, GUILayout.Width(300));
            GUILayout.EndHorizontal();


            GUILayout.BeginHorizontal();
            GUILayout.Label("Heel Release: ", GUILayout.Width(200));
            Footsteps_HeelRelease = EditorGUILayout.Slider(Footsteps_HeelRelease, 0.0f, 50.0f, GUILayout.Width(300));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Ball Gain: ", GUILayout.Width(200));
            Footsteps_BallGain = EditorGUILayout.Slider(Footsteps_BallGain, 0.0f, 1.0f, GUILayout.Width(300));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Ball Attack: ", GUILayout.Width(200));
            Footsteps_BallAttack = EditorGUILayout.Slider(Footsteps_BallAttack, 0.0f, 50.0f, GUILayout.Width(300));
            GUILayout.EndHorizontal();


            GUILayout.BeginHorizontal();
            GUILayout.Label("Ball Decay: ", GUILayout.Width(200));
            Footsteps_BallDecay = EditorGUILayout.Slider(Footsteps_BallDecay, 0.0f, 50.0f, GUILayout.Width(300));
            GUILayout.EndHorizontal();


            GUILayout.BeginHorizontal();
            GUILayout.Label("Ball Sustain: ", GUILayout.Width(200));
            Footsteps_BallSustain = EditorGUILayout.Slider(Footsteps_BallSustain, 0.0f, 1.0f, GUILayout.Width(300));
            GUILayout.EndHorizontal();


            GUILayout.BeginHorizontal();
            GUILayout.Label("Ball Release: ", GUILayout.Width(200));
            Footsteps_BallRelease = EditorGUILayout.Slider(Footsteps_BallRelease, 0.0f, 50.0f, GUILayout.Width(300));
            GUILayout.EndHorizontal();


            GUILayout.BeginHorizontal();
            GUILayout.Label("Roll Gain: ", GUILayout.Width(200));
            Footsteps_RollGain = EditorGUILayout.Slider(Footsteps_RollGain, 0.0f, 1.0f, GUILayout.Width(300));
            GUILayout.EndHorizontal();

        }
        public static void create_Footsteps_Sliders()
        {

            Fontstyle.fontSize = 20;
            Fontstyle.normal.textColor = Color.white;
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            style = new GUIStyle(GUI.skin.button);
            style.onActive.textColor = Color.red;
            style.onNormal.textColor = Color.green;
            style.fontSize = 10;

            GUILayout.BeginHorizontal();
            Footsteps_Start = GUILayout.Toggle(Footsteps_Start, "Automate", style, GUILayout.Width(70), GUILayout.Height(30));
            Footsteps_Trigger = GUILayout.Toggle(Footsteps_Start, "Trigger", style, GUILayout.Width(70), GUILayout.Height(30));
            GUILayout.EndHorizontal();

            GUILayout.Space(140);
            GUILayout.Box(banner, GUILayout.Width(50), GUILayout.Height(30));
            Rect lastRect = GUILayoutUtility.GetLastRect();
            Rect LabelRect = new Rect(lastRect.x + 80, lastRect.y + 4, 100, 30);
            GUILayout.FlexibleSpace();
            GUI.Label(LabelRect, "Footsteps", Fontstyle);
            GUILayout.EndHorizontal();
            GUILayout.Space(20);


            GUILayout.BeginHorizontal();
                string[] options1 = new string[]
                {
                "Wood", "Concrete", "Gravel", "Dirt" , "Grass" , "Hollow Wood", "Metal"
                };
                Footsteps_SurfaceType = EditorGUILayout.Popup("Surface Type: ", Footsteps_SurfaceType, options1, GUILayout.Width(450));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Pace: ", GUILayout.Width(200));
                Footsteps_Pace = EditorGUILayout.Slider(Footsteps_Pace, 60.0f, 300.0f, GUILayout.Width(300));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Firmness: ", GUILayout.Width(200));
                Footsteps_Firmness = EditorGUILayout.Slider(Footsteps_Firmness, 0, 1, GUILayout.Width(300));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Steadiness: ", GUILayout.Width(200));
                Footsteps_Steadiness = EditorGUILayout.Slider(Footsteps_Steadiness, 0.5f, 1, GUILayout.Width(300));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                string[] options2 = new string[]
                {
                "Trainers", "High Heels", "Oxford Boots", "Work Boots", "Custom"
                };
                Footsteps_ShoeType = EditorGUILayout.Popup("Shoe Type: ", Footsteps_ShoeType, options2, GUILayout.Width(450));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Volume: ", GUILayout.Width(200));
                Footsteps_Volume = EditorGUILayout.Slider(Footsteps_Volume, 0.0f, 1.0f, GUILayout.Width(300));
                GUILayout.EndHorizontal();

                 GUILayout.BeginHorizontal();
                string[] options3 = new string[]
                {
                "Flat", "Stairs"
                };
                Footsteps_TerrainType = EditorGUILayout.Popup("Terrain Type: ", Footsteps_TerrainType, options3, GUILayout.Width(450));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Pugin Instance ", GUILayout.Width(200));
                channel = (int)EditorGUILayout.Slider(channel, 0, 32, GUILayout.Width(300));
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();

            string[] Footsteps_parameter_options = new string[]
            {
            "Use Slider Values","Walking Upstairs","Running Outside","Sprint","High Heels","Smart Men's Shoes","Boots On Dirt","Running On Grass","Walking On Gravel","Military Marching","Walking On Metal","Zombie Walk","Drunk High Heels"
            };
            Footsteps_Presets = EditorGUILayout.Popup("Presets", Footsteps_Presets, Footsteps_parameter_options, GUILayout.Width(400));
            if (Footsteps_prevPreset != Footsteps_Presets)
            {
                if (Footsteps_Presets == 1)
                {
                    Footsteps_Firmness = 0.68f;
                    Footsteps_ShoeType = 2;
                    Footsteps_SurfaceType = 0;
                    Footsteps_TerrainType = 1;
                    Footsteps_Pace = 82.0f;
                    Footsteps_Steadiness = 0.9f;
                    Footsteps_Volume = 0.8f;
                }
                if (Footsteps_Presets == 2)
                {
                    Footsteps_Firmness = 0.3f;
                    Footsteps_ShoeType = 0;
                    Footsteps_SurfaceType = 3;
                    Footsteps_TerrainType = 0;
                    Footsteps_Pace = 150.0f;
                    Footsteps_Steadiness = 0.95f;
                    Footsteps_Volume = 0.8f;
                }
                if (Footsteps_Presets == 3)
                {
                    Footsteps_Firmness = 0.3f;
                    Footsteps_ShoeType = 0;
                    Footsteps_SurfaceType = 1;
                    Footsteps_TerrainType = 0;
                    Footsteps_Pace = 180.0f;
                    Footsteps_Steadiness = 0.9f;
                    Footsteps_Volume = 0.8f;
                }
                if (Footsteps_Presets == 4)
                {
                    Footsteps_Firmness = 0.3f;
                    Footsteps_ShoeType = 1;
                    Footsteps_SurfaceType = 1;
                    Footsteps_TerrainType = 0;
                    Footsteps_Pace = 82.0f;
                    Footsteps_Steadiness = 0.95f;
                    Footsteps_Volume = 0.8f;
                }
                if (Footsteps_Presets == 5)
                {
                    Footsteps_Firmness = 0.3f;
                    Footsteps_ShoeType = 2;
                    Footsteps_SurfaceType = 0;
                    Footsteps_TerrainType = 0;
                    Footsteps_Pace = 82.0f;
                    Footsteps_Steadiness = 0.9f;
                    Footsteps_Volume = 0.8f;
                }
                if (Footsteps_Presets == 6)
                {
                    Footsteps_Firmness = 0.3f;
                    Footsteps_ShoeType = 3;
                    Footsteps_SurfaceType = 3;
                    Footsteps_TerrainType = 0;
                    Footsteps_Pace = 82.0f;
                    Footsteps_Steadiness = 0.95f;
                    Footsteps_Volume = 0.8f;
                }
                if (Footsteps_Presets == 7)
                {
                    Footsteps_Firmness = 0.3f;
                    Footsteps_ShoeType = 0;
                    Footsteps_SurfaceType = 4;
                    Footsteps_TerrainType = 0;
                    Footsteps_Pace = 130.0f;
                    Footsteps_Steadiness = 0.95f;
                    Footsteps_Volume = 0.8f;
                }
                if (Footsteps_Presets == 8)
                {
                    Footsteps_Firmness = 0.1f;
                    Footsteps_ShoeType = 3;
                    Footsteps_SurfaceType = 2;
                    Footsteps_TerrainType = 0;
                    Footsteps_Pace = 82.0f;
                    Footsteps_Steadiness = 0.9f;
                    Footsteps_Volume = 0.8f;
                }
                if (Footsteps_Presets == 9)
                {
                    Footsteps_Firmness = 1;
                    Footsteps_ShoeType = 2;
                    Footsteps_SurfaceType = 1;
                    Footsteps_TerrainType = 0;
                    Footsteps_Pace = 116.0f;
                    Footsteps_Steadiness = 1;
                    Footsteps_Volume = 0.8f;
                }
                if (Footsteps_Presets == 10)
                {
                    Footsteps_Firmness = 0.16f;
                    Footsteps_ShoeType = 0;
                    Footsteps_SurfaceType = 6;
                    Footsteps_TerrainType = 0;
                    Footsteps_Pace = 82.0f;
                    Footsteps_Steadiness = 0.95f;
                    Footsteps_Volume = 0.8f;
                }
                if (Footsteps_Presets == 11)
                {
                    Footsteps_Firmness = 1.0f;
                    Footsteps_ShoeType = 0;
                    Footsteps_SurfaceType = 1;
                    Footsteps_TerrainType = 0;
                    Footsteps_Pace = 60.0f;
                    Footsteps_Steadiness = 0;
                    Footsteps_Volume = 0.8f;
                }
                if (Footsteps_Presets == 12)
                {
                    Footsteps_Firmness = 0.34f;
                    Footsteps_ShoeType = 1;
                    Footsteps_SurfaceType = 1;
                    Footsteps_TerrainType = 0;
                    Footsteps_Pace = 76.0f;
                    Footsteps_Steadiness = 0.2f;
                    Footsteps_Volume = 0.8f;
                }
            }
            Footsteps_prevPreset = Footsteps_Presets;
            GUILayout.EndHorizontal();
            GUILayout.BeginVertical();
            GUILayout.Space(10);
            GUILayout.EndVertical();
        }

    }
}

