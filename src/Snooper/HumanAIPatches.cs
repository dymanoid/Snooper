// <copyright file="HumanAIPatches.cs" company="dymanoid">
// Copyright (c) dymanoid. All rights reserved.
// </copyright>

namespace Snooper
{
    using HarmonyLib;

    /// <summary>
    /// A static class that provides the patch objects for the human AI game methods.
    /// </summary>
    [HarmonyPatch]
    internal static class HumanAIPatches
    {
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

            ref var instance = ref CitizenManager.instance.m_instances.m_buffer[instanceId];
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

        [HarmonyPatch(typeof(HumanAI), nameof(HumanAI.StartMoving), new[] { typeof(uint), typeof(Citizen), typeof(ushort), typeof(TransferManager.TransferOffer), }, new ArgumentType[] { ArgumentType.Normal, ArgumentType.Ref, ArgumentType.Normal, ArgumentType.Normal, })]
        [HarmonyPostfix]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming Rules", "SA1313", Justification = "Harmony patch")]
        private static void StartMovingPatch1(bool __result, uint citizenID, ushort sourceBuilding)
        {
            if (__result && sourceBuilding != 0)
            {
                SetSourceBuilding(citizenID, sourceBuilding);
            }
        }

        [HarmonyPatch(typeof(HumanAI), nameof(HumanAI.StartMoving), new[] { typeof(uint), typeof(Citizen), typeof(ushort), typeof(ushort), }, new ArgumentType[] { ArgumentType.Normal, ArgumentType.Ref, ArgumentType.Normal, ArgumentType.Normal, })]
        [HarmonyPostfix]

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming Rules", "SA1313", Justification = "Harmony patch")]
        private static void StartMovingPatch2(bool __result, uint citizenID, ushort sourceBuilding)
        {
            if (__result && sourceBuilding != 0)
            {
                SetSourceBuilding(citizenID, sourceBuilding);
            }
        }
    }
}
