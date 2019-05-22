// <copyright file="CargoTruckAIPatches.cs" company="dymanoid">
// Copyright (c) dymanoid. All rights reserved.
// </copyright>

namespace Snooper
{
    using System.Reflection;
    using SkyTools.Patching;

    /// <summary>
    /// A static class that provides the patch objects for the car AI game methods.
    /// </summary>
    internal static class CargoTruckAIPatches
    {
        /// <summary>Gets the patch for the set target method.</summary>
        public static IPatch SetTarget { get; } = new CargoTruckAI_SetTarget();

        private sealed class CargoTruckAI_SetTarget : PatchBase
        {
            protected override MethodInfo GetMethod()
            {
                return typeof(CargoTruckAI).GetMethod(
                    "SetTarget",
                    BindingFlags.Public | BindingFlags.Instance,
                    null,
                    new[] { typeof(ushort), typeof(Vehicle).MakeByRefType(), typeof(ushort) },
                    new ParameterModifier[0]);
            }

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Redundancy", "RCS1213", Justification = "Harmony patch")]
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming Rules", "SA1313", Justification = "Harmony patch")]
            private static void Prefix(ref Vehicle data, ref ushort __state) => __state = data.m_targetBuilding;

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Redundancy", "RCS1213", Justification = "Harmony patch")]
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming Rules", "SA1313", Justification = "Harmony patch")]
            private static void Postfix(ref Vehicle data, ushort targetBuilding, ref ushort __state)
            {
                if (__state != 0 && targetBuilding == 0 && data.m_touristCount == 0 && (data.m_flags & Vehicle.Flags.GoingBack) != 0)
                {
                    // Storing the original target building ID in the tourist count field.
                    // It won't be used by the cargo truck AI, so no side effects.
                    // Storing it back into the target building field might lead to side effects though.
                    data.m_touristCount = __state;
                }
            }
        }
    }
}
