namespace Snooper.Utils
{
    using System.Collections.Generic;
    using CitiesHarmony.API;
    using HarmonyLib;
    using SkyTools.Tools;

    public static class HarmonyPatcher
    {
        public static void PatchOnReady(System.Reflection.Assembly assembly, string harmonyID)
        {
            try
            {
                HarmonyHelper.EnsureHarmonyInstalled();
                HarmonyHelper.DoOnHarmonyReady(() => Patch(assembly, harmonyID));
            }
            catch (System.Exception e)
            {
                Log.Error("Snooper: " + e);
            }
        }

        public static void TryUnpatch(string harmonyID)
        {
            try
            {
                if (HarmonyHelper.IsHarmonyInstalled)
                {
                    Unpatch(harmonyID);
                }
            }
            catch (System.Exception e)
            {
                Log.Error("Snooper: " + e);
            }
        }

        public static void Patch(System.Reflection.Assembly assembly, string harmonyID)
        {
            if (PatchedAssemblies.Contains(harmonyID))
            {
                Log.Warning($"Snooper: <{harmonyID}> already patched");
                return;
            }

            Log.Info($"The 'Snooper' mod is patching <{harmonyID}>");
            try
            {
                var harmony = new Harmony(harmonyID);
                harmony.PatchAll(assembly);
                PatchedAssemblies.Add(harmonyID);
                Log.Info("Snooper: patched");
            }
            catch (System.Exception e)
            {
                Log.Error(" -- patching fails: " + e);
            }
        }

        public static void Unpatch(string harmonyID)
        {
            if (!PatchedAssemblies.Remove(harmonyID))
            {
                Log.Warning($"Snooper: <{harmonyID}> never been patched");
                return;
            }

            Log.Info($"Unpatching <{harmonyID}>");
            try
            {
                var harmony = new Harmony(harmonyID);
                harmony.UnpatchAll(harmonyID);
                Log.Info("Snooper: unpatched");
            }
            catch (System.Exception e)
            {
                Log.Error(" -- unpatching fails: " + e);
            }
        }

        public static bool HasPatched(string harmonyID) => PatchedAssemblies.Contains(harmonyID);

        private static readonly List<string> PatchedAssemblies = new List<string>();
    }
}
