using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FoundationToSoil
{
    
    [BepInPlugin("aki9284.DSP.plugin.FoundationToSoil", "FoundationToSoil Plug-In", "0.9.0.0")]
    public class FoundationToSoil : BaseUnityPlugin
    {
        private static ConfigEntry<int> configSoilThreshold;
        private static ConfigEntry<int> configConvertRate;

        void Awake()
        {
            configSoilThreshold = Config.Bind("General", "SoilThreshold", 30000, "If you build a foundation with less than this number of soil piles, the foundation in your inventory will be converted to soil.");
            configConvertRate = Config.Bind("General", "ConvertRate", 500, "Each foundation is converted into this number of soil piles.");
            Harmony.CreateAndPatchAll(typeof(FoundationToSoil));
        }


        [HarmonyPatch(typeof(Player), "SetSandCount")]
        [HarmonyPostfix]
        static void Postfix(Player __instance)
        {
            var inventory = __instance.package;

            //1131:Foundation
            int fNum = inventory.GetItemCount(1131);
            //if sandCount is less than the threshold, convert foundations
            int sandDemand = Math.Max(0, configSoilThreshold.Value - __instance.sandCount);
            int fNumToConvert = Math.Min(fNum, (int)Math.Ceiling((double)sandDemand / configConvertRate.Value));

            if ( fNumToConvert > 0)
            {
                //put foundations in the inventory before convert them
                if (__instance.inhandItemId == 1131)
                {
                    __instance.SetHandItems(0, 0);
                }
                inventory.TakeItem(1131, fNumToConvert);
                __instance.SetSandCount(__instance.sandCount + fNumToConvert * configConvertRate.Value);
            }
        }
    }
}
