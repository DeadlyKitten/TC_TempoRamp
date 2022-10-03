using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System.IO;
using UnityEngine;

namespace TempoRamp
{
    [HarmonyPatch]
    [BepInPlugin("com.steven.trombone.temporamp", "Tempo Ramp", "1.0.2")]
    public class Plugin : BaseUnityPlugin
    {
        static bool modEnabled;
        //internal static bool pitchCorrection;
        internal static bool pitchTrombone;
        internal static float startRampTime;
        internal static float endRampTime;

        void Awake()
        {
            LoadConfig();

            if (modEnabled)
            {
                var harmony = new Harmony("com.steven.trombone.temporamp");
                harmony.PatchAll();
            }
        }

        void LoadConfig()
        {
            var config = new ConfigFile(Path.Combine(Paths.ConfigPath, "TempoRamp.cfg"), true);

            var cfgEnabled = config.Bind("General", "Enabled", true, "Whether or not the mod is enabled. Restart the game to take effect.");
            //var cfgPitchCorrection = config.Bind("General", "PitchCorrection", false, "Adjusting playback speed has an effect on the song's pitch.\nSet this option to true to try and correct for that (may cause some distortions).");
            var cfgPitchTrombone = config.Bind("General", "Pitch Trombone", true, "Whether or not the the trombone pitches up/down to match the background music.");
            var cfgStartRampTime = config.Bind("General", "Song Start Speed", 1.0f, "How fast the song should be at the start. 1.0 is default");
            var cfgEndRampTime = config.Bind("General", "Song End Speed", 1.5f, "How fast the song should be at the end. 1.0 is default");

            modEnabled = cfgEnabled.Value;
            //pitchCorrection = cfgPitchCorrection.Value;
            pitchTrombone = cfgPitchTrombone.Value;
            startRampTime = cfgStartRampTime.Value;
            endRampTime = cfgEndRampTime.Value;
        }

        [HarmonyPatch(typeof(GameController), "Update")]
        static void Postfix(GameController __instance, float ___levelendtime)
        {
            var percentDone = __instance.musictrack.time / ___levelendtime;
            var pitch = Mathf.Lerp(Plugin.startRampTime, Plugin.endRampTime, percentDone);
            __instance.musictrack.pitch = pitch;

            if (pitchTrombone) __instance.currentnotesound.pitch *= __instance.musictrack.pitch;
        }
    }
}
