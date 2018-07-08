// <copyright file="WorldInfoPanelPatches.cs" company="dymanoid">
// Copyright (c) dymanoid. All rights reserved.
// </copyright>

namespace Snooper
{
    using System;
    using System.Reflection;

    internal static class WorldInfoPanelPatches
    {
        public static CustomCitizenInfoPanel CitizenInfoPanel { get; set; }

        public static CustomVehicleInfoPanel VehicleInfoPanel { get; set; }

        public static IPatch UpdateBindings { get; } = new WorldInfoPanel_UpdateBindings();

        internal sealed class WorldInfoPanel_UpdateBindings : PatchBase
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

#pragma warning disable SA1313 // Parameter names must begin with lower-case letter
            private static void Postfix(ref InstanceID ___m_InstanceID)
            {
                CitizenInfoPanel?.UpdateOrigin(ref ___m_InstanceID);
                VehicleInfoPanel?.UpdateOrigin(ref ___m_InstanceID);
            }
#pragma warning restore SA1313 // Parameter names must begin with lower-case letter
        }
    }
}
