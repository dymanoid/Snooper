// <copyright file="SnooperMod.cs" company="dymanoid">
// Copyright (c) dymanoid. All rights reserved.
// </copyright>

namespace Snooper
{
    using System.Collections.Generic;
    using ICities;
    using SkyTools.Patching;
    using SkyTools.Tools;
    using UnityEngine;

    /// <summary>
    /// The main class of the Snooper mod.
    /// </summary>
    public sealed class SnooperMod : LoadingExtensionBase, IUserMod
    {
        private const string HarmonyId = "com.cities_skylines.dymanoid.snooper";

        private readonly string modVersion = GitVersion.GetAssemblyVersion(typeof(SnooperMod).Assembly);
        private MethodPatcher patcher;

        /// <summary>
        /// Gets the name of this mod.
        /// </summary>
        public string Name => "Snooper";

        /// <summary>
        /// Gets the description string of this mod.
        /// </summary>
        public string Description => "Shows additional information about citizens and tourists. Version: " + modVersion;

        /// <summary>
        /// Called when a game level is loaded. If applicable, activates the Snooper mod
        /// for the loaded level.
        /// </summary>
        ///
        /// <param name="mode">The <see cref="LoadMode"/> a game level is loaded in.</param>
        public override void OnLevelLoaded(LoadMode mode)
        {
            switch (mode)
            {
                case LoadMode.LoadGame:
                case LoadMode.NewGame:
                case LoadMode.LoadScenario:
                case LoadMode.NewGameFromScenario:
                    break;

                default:
                    return;
            }

            IPatch[] patches =
            {
                WorldInfoPanelPatches.UpdateBindings,
                HumanAIPatches.StartMoving1,
                HumanAIPatches.StartMoving2,
                CargoTruckAIPatches.SetTarget
            };

            patcher = new MethodPatcher(HarmonyId, patches);

            HashSet<IPatch> patchedMethods = patcher.Apply();
            if (patchedMethods.Count != patches.Length)
            {
                Debug.LogError("The 'Snooper' mod failed to perform method redirections");
                patcher.Revert();
                return;
            }

            WorldInfoPanelPatches.CitizenInfoPanel = CustomCitizenInfoPanel.Enable();
            WorldInfoPanelPatches.TouristInfoPanel = CustomTouristInfoPanel.Enable();
            WorldInfoPanelPatches.CitizenVehicleInfoPanel = CustomCitizenVehicleInfoPanel.Enable();
            WorldInfoPanelPatches.ServiceVehicleInfoPanel = CustomCityServiceVehicleInfoPanel.Enable();
        }

        /// <summary>
        /// Called when a game level is about to be unloaded. If the Snooper mod was activated
        /// for this level, deactivates the mod for this level.
        /// </summary>
        public override void OnLevelUnloading()
        {
            patcher?.Revert();
            patcher = null;

            WorldInfoPanelPatches.CitizenInfoPanel?.Disable();
            WorldInfoPanelPatches.CitizenInfoPanel = null;

            WorldInfoPanelPatches.TouristInfoPanel?.Disable();
            WorldInfoPanelPatches.TouristInfoPanel = null;

            WorldInfoPanelPatches.CitizenVehicleInfoPanel?.Disable();
            WorldInfoPanelPatches.CitizenVehicleInfoPanel = null;

            WorldInfoPanelPatches.ServiceVehicleInfoPanel?.Disable();
            WorldInfoPanelPatches.ServiceVehicleInfoPanel = null;
        }
    }
}
