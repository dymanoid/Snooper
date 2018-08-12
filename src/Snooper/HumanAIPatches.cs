// <copyright file="HumanAIPatches.cs" company="dymanoid">
// Copyright (c) dymanoid. All rights reserved.
// </copyright>

namespace Snooper
{
    using System.Reflection;
    using SkyTools.Patching;

    /// <summary>
    /// A static class that provides the patch objects for the human AI game methods.
    /// </summary>
    internal static class HumanAIPatches
    {
        /// <summary>Gets the patch for the start moving method (1st overload).</summary>
        public static IPatch StartMoving1 { get; } = new HumanAI_StartMoving1();

        /// <summary>Gets the patch for the start moving method (2nd overload).</summary>
        public static IPatch StartMoving2 { get; } = new HumanAI_StartMoving2();

        private static void SetSourceBuilding(uint citizenId, ushort buildingId)
        {
            if (citizenId == 0)
            {
                return;
            }

            ushort instanceId = CitizenManager.instance.m_citizens.m_buffer[citizenId].m_instance;
            if (instanceId == 0)
            {
                return;
            }

            ref CitizenInstance instance = ref CitizenManager.instance.m_instances.m_buffer[instanceId];
            if (instance.m_sourceBuilding == buildingId)
            {
                return;
            }

            if (instance.m_sourceBuilding != 0)
            {
                BuildingManager.instance.m_buildings.m_buffer[instance.m_sourceBuilding].RemoveSourceCitizen(instanceId, ref instance);
            }

            instance.m_sourceBuilding = buildingId;

            if (buildingId != 0)
            {
                BuildingManager.instance.m_buildings.m_buffer[buildingId].AddSourceCitizen(instanceId, ref instance);
            }
        }

        private sealed class HumanAI_StartMoving1 : PatchBase
        {
            protected override MethodInfo GetMethod()
            {
                return typeof(HumanAI).GetMethod(
                    "StartMoving",
                    BindingFlags.Instance | BindingFlags.Public,
                    null,
                    new[] { typeof(uint), typeof(Citizen).MakeByRefType(), typeof(ushort), typeof(TransferManager.TransferOffer) },
                    new ParameterModifier[0]);
            }

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Redundancy", "RCS1213", Justification = "Harmony patch")]
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming Rules", "SA1313", Justification = "Harmony patch")]
            private static void Postfix(bool __result, uint citizenID, ushort sourceBuilding)
            {
                if (__result && sourceBuilding != 0)
                {
                    SetSourceBuilding(citizenID, sourceBuilding);
                }
            }
        }

        private sealed class HumanAI_StartMoving2 : PatchBase
        {
            protected override MethodInfo GetMethod()
            {
                return typeof(HumanAI).GetMethod(
                    "StartMoving",
                    BindingFlags.Instance | BindingFlags.Public,
                    null,
                    new[] { typeof(uint), typeof(Citizen).MakeByRefType(), typeof(ushort), typeof(ushort) },
                    new ParameterModifier[0]);
            }

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Redundancy", "RCS1213", Justification = "Harmony patch")]
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming Rules", "SA1313", Justification = "Harmony patch")]
            private static void Postfix(bool __result, uint citizenID, ushort sourceBuilding)
            {
                if (__result && sourceBuilding != 0)
                {
                    SetSourceBuilding(citizenID, sourceBuilding);
                }
            }
        }
    }
}
