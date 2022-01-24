using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace FoundationToSoil
{
    
    [BepInPlugin("aki9284.DSP.plugin.FoundationToSoil", "FoundationToSoil Plug-In", "0.9.1.0")]
    public class FoundationToSoil : BaseUnityPlugin
    {
        private static ConfigEntry<int> configSoilThreshold;
        private static ConfigEntry<int> configConvertRate;
        private static ConfigEntry<bool> configSurfaceFormationModeOnly;

        void Awake()
        {
            configSoilThreshold = Config.Bind("General", "SoilThreshold", 30000, "If you build a foundation with less than this number of soil piles, the foundation in your inventory will be converted to soil.");
            configConvertRate = Config.Bind("General", "ConvertRate", 500, "Each foundation is converted into this number of soil piles.");
            configSurfaceFormationModeOnly = Config.Bind("General", "SurfaceFormationModeOnly", false, "True: Convert foundations only when you have foundations in your hand.");
            Harmony.CreateAndPatchAll(typeof(FoundationToSoil));
        }


        [HarmonyPatch(typeof(Player), "SetSandCount")]
        [HarmonyPostfix]
        static void Postfix(Player __instance)
        {
            //1000000000 is the limit in DSP
            if (configSoilThreshold.Value > 1000000000)
            {
                configSoilThreshold.Value = 1000000000;
            }
            var inventory = __instance.package;

            if (configSurfaceFormationModeOnly.Value)
            {
                if (__instance.inhandItemId != 1131)
                {
                    return;
                }
            }

            //There is a UI problem with the amount of soil piles gained during construction mode(UI only shows the converted amount)
            //But the gained amount is appropriate
            //Debug.Log(string.Format("Sand Count:{0}", __instance.sandCount));

            //1131:Foundation
            int fNum = inventory.GetItemCount(1131);
            //if sandCount is less than the threshold, convert foundations
            int sandDemand = Math.Max(0, configSoilThreshold.Value - __instance.sandCount);
            int fNumToConvert = Math.Min(fNum, (int)Math.Ceiling((double)sandDemand / configConvertRate.Value));

            if ( fNumToConvert > 0)
            {
                int dummy;
                inventory.TakeItem(1131, fNumToConvert, out dummy);
                __instance.SetSandCount(__instance.sandCount + fNumToConvert * configConvertRate.Value);
            }
        }
    }
}
