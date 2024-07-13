// <copyright file="WorldInfoPanelPatches.cs" company="dymanoid">
// Copyright (c) dymanoid. All rights reserved.
// </copyright>

namespace Snooper
{
    using HarmonyLib;

    /// <summary>
    /// A static class that provides the patch objects for the world info panel game methods.
    /// </summary>
    [HarmonyPatch]
    internal static class WorldInfoPanelPatches
    {
        /// <summary>Gets or sets the customized citizen information panel.</summary>
        public static CustomCitizenInfoPanel CitizenInfoPanel { get; set; }

        /// <summary>Gets or sets the customized tourist information panel.</summary>
        public static CustomTouristInfoPanel TouristInfoPanel { get; set; }

        /// <summary>Gets or sets the customized citizen vehicle information panel.</summary>
        public static CustomCitizenVehicleInfoPanel CitizenVehicleInfoPanel { get; set; }

        /// <summary>Gets or sets the customized service vehicle information panel.</summary>
        public static CustomCityServiceVehicleInfoPanel ServiceVehicleInfoPanel { get; set; }

        [HarmonyPatch(typeof(WorldInfoPanel), "UpdateBindings")]
        [HarmonyPostfix]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Redundancy", "RCS1213", Justification = "Harmony patch")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming Rules", "SA1313", Justification = "Harmony patch")]
        private static void WorldInfoPanelPatch(WorldInfoPanel __instance, ref InstanceID ___m_InstanceID)
        {
            switch (__instance)
            {
                case CitizenWorldInfoPanel _:
                    CitizenInfoPanel?.UpdateCustomInfo(ref ___m_InstanceID);
                    break;

                case TouristWorldInfoPanel _:
                    TouristInfoPanel?.UpdateCustomInfo(ref ___m_InstanceID);
                    break;

                case CitizenVehicleWorldInfoPanel _:
                    CitizenVehicleInfoPanel?.UpdateCustomInfo(ref ___m_InstanceID);
                    break;

                case CityServiceVehicleWorldInfoPanel _:
                    ServiceVehicleInfoPanel?.UpdateCustomInfo(ref ___m_InstanceID);
                    break;
            }
        }
    }
}
