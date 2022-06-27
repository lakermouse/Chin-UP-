#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

using System.Collections.Generic;
using System.Runtime.InteropServices;

public class audioplugin_FootstepsGUI : IAudioEffectPluginGUI
{
    public override string Name           { get { return "Nemisindo Footsteps"; } }
    public override string Description    { get { return "Footsteps"; } }
    public override string Vendor         { get { return "Nemisindo"; } }

    //==============================================================================

#if UNITY_IOS
      [DllImport("__Internal")]
#else
    [DllImport("audioplugin_Footsteps")] static extern void setFloatValue(float channel, int index, float value);

#endif

#if UNITY_IOS
      [DllImport("__Internal")]
#else
    [DllImport("audioplugin_Footsteps")] static extern float getFloatValue(float channel, int index);

#endif

    //==============================================================================

    //==============================================================================
    public override bool OnGUI (IAudioEffectPlugin plugin)
    {
        float val0;
        plugin.GetFloatParameter("Channel", out val0);
        float param0 = val0;
        float param1 = getFloatValue(param0, 1);
        float param2 = getFloatValue(param0, 2);
        float param3 = getFloatValue(param0, 3);
        float param4 = getFloatValue(param0, 4);
        float param5 = getFloatValue(param0, 5);
        float param6 = getFloatValue(param0, 6);
        float param7 = getFloatValue(param0, 7);
        float param8 = getFloatValue(param0, 8);
        float param9 = getFloatValue(param0, 9);
        float param10 = getFloatValue(param0, 10);
        float param11 = getFloatValue(param0, 11);
        float param12 = getFloatValue(param0, 12);
        float param13 = getFloatValue(param0, 13);
        float param14 = getFloatValue(param0, 14);
        float param15 = getFloatValue(param0, 15);
        float param16 = getFloatValue(param0, 16);
        float param17 = getFloatValue(param0, 17);
        float param18 = getFloatValue(param0, 18);
        float param19 = getFloatValue(param0, 19);
        float param20 = getFloatValue(param0, 20);
        int param21 = (int)getFloatValue(param0, 21);

        Nemisindo.Footsteps_params.channel = (int)param0;

        Nemisindo.Footsteps_params.Footsteps_Pace = param1;

        Nemisindo.Footsteps_params.Footsteps_Firmness = param2;

        Nemisindo.Footsteps_params.Footsteps_Steadiness = param3;

        Nemisindo.Footsteps_params.Footsteps_ShoeType = (int)param4;

        Nemisindo.Footsteps_params.Footsteps_SurfaceType = (int)param5;

        Nemisindo.Footsteps_params.Footsteps_TerrainType = (int) param6;

        Nemisindo.Footsteps_params.Footsteps_Volume = param7;

        if (param8 == 1)
        {
            Nemisindo.Footsteps_params.Footsteps_Start = true;
        }
        else
        {
            Nemisindo.Footsteps_params.Footsteps_Start = false;
        }

        if (param9 == 1)
        {
            Nemisindo.Footsteps_params.Footsteps_Trigger = true;
        }
        else
        {
            Nemisindo.Footsteps_params.Footsteps_Trigger = false;
        }
        if (Nemisindo.Footsteps_params.Footsteps_Trigger)
        {
            Nemisindo.Footsteps_params.Footsteps_Trigger = false;
        }

        Nemisindo.Footsteps_params.Footsteps_HeelGain    = param10;
        Nemisindo.Footsteps_params.Footsteps_HeelAttack  = param11;
        Nemisindo.Footsteps_params.Footsteps_HeelDecay   = param12;
        Nemisindo.Footsteps_params.Footsteps_HeelSustain = param13;
        Nemisindo.Footsteps_params.Footsteps_HeelRelease = param14;
        Nemisindo.Footsteps_params.Footsteps_BallGain    = param15;
        Nemisindo.Footsteps_params.Footsteps_BallAttack  = param16;
        Nemisindo.Footsteps_params.Footsteps_BallDecay   = param17;
        Nemisindo.Footsteps_params.Footsteps_BallSustain = param18;
        Nemisindo.Footsteps_params.Footsteps_BallRelease = param19;
        Nemisindo.Footsteps_params.Footsteps_RollGain    = param20;
        Nemisindo.Footsteps_params.Footsteps_Presets = param21;


        Nemisindo.Footsteps_params.create_Footsteps_Sliders();

        if(Nemisindo.Footsteps_params.Footsteps_ShoeType==4)
        {
                Nemisindo.Footsteps_params.create_Footsteps_customSliders();
        }
        
        if (param0 != Nemisindo.Footsteps_params.channel)
        {
            plugin.SetFloatParameter("Channel", Nemisindo.Footsteps_params.channel);
        }

        if (param1 != Nemisindo.Footsteps_params.Footsteps_Pace)
        {
            if (Nemisindo.Footsteps_params.channel == val0)
            {
                setFloatValue(Nemisindo.Footsteps_params.channel, 1, Nemisindo.Footsteps_params.Footsteps_Pace);
                plugin.SetFloatParameter("Pace", Nemisindo.Footsteps_params.Footsteps_Pace);
            }
        }


           if (param2 != Nemisindo.Footsteps_params.Footsteps_Firmness)
         {
             if (Nemisindo.Footsteps_params.channel == val0)
             {
                 setFloatValue(Nemisindo.Footsteps_params.channel, 2, Nemisindo.Footsteps_params.Footsteps_Firmness);
                 plugin.SetFloatParameter("Firmness", Nemisindo.Footsteps_params.Footsteps_Firmness);
             }
         }


        if (param3 != Nemisindo.Footsteps_params.Footsteps_Steadiness)     
        {
            if (Nemisindo.Footsteps_params.channel == val0)
            {
                setFloatValue(Nemisindo.Footsteps_params.channel, 3, Nemisindo.Footsteps_params.Footsteps_Steadiness);
                 plugin.SetFloatParameter("Steadiness", Nemisindo.Footsteps_params.Footsteps_Steadiness);
            }
        }



         if (param4 != Nemisindo.Footsteps_params.Footsteps_ShoeType)
         {
             if (Nemisindo.Footsteps_params.channel == val0)
             {
                 setFloatValue(Nemisindo.Footsteps_params.channel, 4, Nemisindo.Footsteps_params.Footsteps_ShoeType);
                 plugin.SetFloatParameter("ShoeType", Nemisindo.Footsteps_params.Footsteps_ShoeType);
             }

         }

         if (param5 != Nemisindo.Footsteps_params.Footsteps_SurfaceType)
         {
             if (Nemisindo.Footsteps_params.channel == val0)
             {
                 setFloatValue(Nemisindo.Footsteps_params.channel, 5, Nemisindo.Footsteps_params.Footsteps_SurfaceType);
                 plugin.SetFloatParameter("SurfaceType", Nemisindo.Footsteps_params.Footsteps_SurfaceType);
             }

         }

         if (param6 != Nemisindo.Footsteps_params.Footsteps_TerrainType)
         {
             if (Nemisindo.Footsteps_params.channel == val0)
             {
                 setFloatValue(Nemisindo.Footsteps_params.channel, 6, Nemisindo.Footsteps_params.Footsteps_TerrainType);
                 plugin.SetFloatParameter("TerrainType", Nemisindo.Footsteps_params.Footsteps_TerrainType);
             }

         }

 if (param7 != Nemisindo.Footsteps_params.Footsteps_Volume)
 {
     if (Nemisindo.Footsteps_params.channel == val0)
     {
         setFloatValue(Nemisindo.Footsteps_params.channel, 7, Nemisindo.Footsteps_params.Footsteps_Volume);
         plugin.SetFloatParameter("Volume", Nemisindo.Footsteps_params.Footsteps_Volume);
     }

 }



 float strt = Nemisindo.Footsteps_params.Footsteps_Start ? 1.0f : 0.0f;
 if (param8 != strt)
 {
     if (Nemisindo.Footsteps_params.channel == val0)
     {
         setFloatValue(Nemisindo.Footsteps_params.channel, 8, strt);
         plugin.SetFloatParameter("Start", strt);
     }

 }

 float trigger = Nemisindo.Footsteps_params.Footsteps_Trigger ? 1.0f : 0.0f;
 if (param9 != trigger)
 {
     if (Nemisindo.Footsteps_params.channel == val0)
     {
                 plugin.SetFloatParameter("Trigger", trigger);
                 setFloatValue(param0, 9, trigger);
                 plugin.SetFloatParameter("Trigger", 0);
     }

 }



        if (param10 != Nemisindo.Footsteps_params.Footsteps_HeelGain)
 {
     if (Nemisindo.Footsteps_params.channel == val0)
     {
         setFloatValue(Nemisindo.Footsteps_params.channel, 10, Nemisindo.Footsteps_params.Footsteps_HeelGain);
         plugin.SetFloatParameter("HeelGain", Nemisindo.Footsteps_params.Footsteps_HeelGain);
     }
 }

       if (param11 != Nemisindo.Footsteps_params.Footsteps_HeelAttack)
         {
             if (Nemisindo.Footsteps_params.channel == val0)
             {
                 setFloatValue(Nemisindo.Footsteps_params.channel, 11, Nemisindo.Footsteps_params.Footsteps_HeelAttack);
                 plugin.SetFloatParameter("HeelAttack", Nemisindo.Footsteps_params.Footsteps_HeelAttack);
             }
         }

         if (param12 != Nemisindo.Footsteps_params.Footsteps_HeelDecay)
         {
             if (Nemisindo.Footsteps_params.channel == val0)
             {
                 setFloatValue(Nemisindo.Footsteps_params.channel, 12, Nemisindo.Footsteps_params.Footsteps_HeelDecay);
                 plugin.SetFloatParameter("HeelDecay", Nemisindo.Footsteps_params.Footsteps_HeelDecay);
             }
         }

         if (param13 != Nemisindo.Footsteps_params.Footsteps_HeelSustain)
         {
             if (Nemisindo.Footsteps_params.channel == val0)
             {
                 setFloatValue(Nemisindo.Footsteps_params.channel, 13, Nemisindo.Footsteps_params.Footsteps_HeelSustain);
                 plugin.SetFloatParameter("HeelSustain", Nemisindo.Footsteps_params.Footsteps_HeelSustain);
             }
         }

         if (param14 != Nemisindo.Footsteps_params.Footsteps_HeelRelease)
         {
             if (Nemisindo.Footsteps_params.channel == val0)
             {
                 setFloatValue(Nemisindo.Footsteps_params.channel, 14, Nemisindo.Footsteps_params.Footsteps_HeelRelease);
                 plugin.SetFloatParameter("HeelRelease", Nemisindo.Footsteps_params.Footsteps_HeelRelease);
             }
         }

         if (param15 != Nemisindo.Footsteps_params.Footsteps_BallGain)
         {
             if (Nemisindo.Footsteps_params.channel == val0)
             {
                 setFloatValue(Nemisindo.Footsteps_params.channel, 15, Nemisindo.Footsteps_params.Footsteps_BallGain);
                 plugin.SetFloatParameter("BallGain", Nemisindo.Footsteps_params.Footsteps_BallGain);
             }
         }

         if (param16 != Nemisindo.Footsteps_params.Footsteps_BallAttack)
         {
             if (Nemisindo.Footsteps_params.channel == val0)
             {
                 setFloatValue(Nemisindo.Footsteps_params.channel, 16, Nemisindo.Footsteps_params.Footsteps_BallAttack);
                 plugin.SetFloatParameter("BallAttack", Nemisindo.Footsteps_params.Footsteps_BallAttack);
             }
         }

        if (param17 != Nemisindo.Footsteps_params.Footsteps_BallDecay)
        {
            if (Nemisindo.Footsteps_params.channel == val0)
            {
                setFloatValue(Nemisindo.Footsteps_params.channel, 17, Nemisindo.Footsteps_params.Footsteps_BallDecay);
                plugin.SetFloatParameter("BallDecay", Nemisindo.Footsteps_params.Footsteps_BallDecay);
            }
        }

        if (param18 != Nemisindo.Footsteps_params.Footsteps_BallSustain)
         {
             if (Nemisindo.Footsteps_params.channel == val0)
             {
                 setFloatValue(Nemisindo.Footsteps_params.channel, 18, Nemisindo.Footsteps_params.Footsteps_BallSustain);
                 plugin.SetFloatParameter("BallSustain", Nemisindo.Footsteps_params.Footsteps_BallSustain);
             }
         }

         if (param19 != Nemisindo.Footsteps_params.Footsteps_BallRelease)
         {
             if (Nemisindo.Footsteps_params.channel == val0)
             {
                 setFloatValue(Nemisindo.Footsteps_params.channel, 19, Nemisindo.Footsteps_params.Footsteps_BallRelease);
                 plugin.SetFloatParameter("BallRelease", Nemisindo.Footsteps_params.Footsteps_BallRelease);
             }
         }

         if (param20 != Nemisindo.Footsteps_params.Footsteps_RollGain)
         {
             if (Nemisindo.Footsteps_params.channel == val0)
             {
                 setFloatValue(Nemisindo.Footsteps_params.channel, 20, Nemisindo.Footsteps_params.Footsteps_RollGain);
                 plugin.SetFloatParameter("RollGain", Nemisindo.Footsteps_params.Footsteps_RollGain);
             }
         }

         if (param21 != Nemisindo.Footsteps_params.Footsteps_Presets)
         {
             if (Nemisindo.Footsteps_params.channel == val0)
             {
                 setFloatValue(param0, 21, Nemisindo.Footsteps_params.Footsteps_Presets);
                 plugin.SetFloatParameter("Presets", Nemisindo.Footsteps_params.Footsteps_Presets);
             }
         }
   
        return false;
    }
}

#endif
