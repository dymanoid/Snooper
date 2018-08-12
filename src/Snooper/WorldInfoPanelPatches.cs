// <copyright file="WorldInfoPanelPatches.cs" company="dymanoid">
// Copyright (c) dymanoid. All rights reserved.
// </copyright>

namespace Snooper
{
    using System;
    using System.Reflection;
    using SkyTools.Patching;

    /// <summary>
    /// A static class that provides the patch objects for the world info panel game methods.
    /// </summary>
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

        /// <summary>Gets the patch for the update bindings method.</summary>
        public static IPatch UpdateBindings { get; } = new WorldInfoPanel_UpdateBindings();

        private sealed class WorldInfoPanel_UpdateBindings : PatchBase
        {
            protected override MethodInfo GetMethod()
            {
                return typeof(WorldInfoPanel).GetMethod(
                    "UpdateBindings",
                    BindingFlags.Instance | BindingFlags.NonPublic,
                    null,
                    new Type[0],
                    new ParameterModifier[0]);
            }

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Redundancy", "RCS1213", Justification = "Harmony patch")]
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming Rules", "SA1313", Justification = "Harmony patch")]
            private static void Postfix(WorldInfoPanel __instance, ref InstanceID ___m_InstanceID)
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
}
