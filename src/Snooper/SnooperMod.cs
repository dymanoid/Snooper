// <copyright file="SnooperMod.cs" company="dymanoid">
// Copyright (c) dymanoid. All rights reserved.
// </copyright>

namespace Snooper
{
    using System;
    using ICities;
    using UnityEngine;

    /// <summary>
    /// The main class of the Snooper mod.
    /// </summary>
    public sealed class SnooperMod : LoadingExtensionBase, IUserMod
    {
        private readonly string modVersion = GitVersion.GetAssemblyVersion(typeof(SnooperMod).Assembly);
        private readonly MethodPatcher patcher;

        /// <summary>Initializes a new instance of the <see cref="SnooperMod"/> class.</summary>
        public SnooperMod()
        {
            patcher = new MethodPatcher(
                WorldInfoPanelPatches.UpdateBindings,
                HumanAIPatches.StartMoving1,
                HumanAIPatches.StartMoving2);
        }

        /// <summary>
        /// Gets the name of this mod.
        /// </summary>
        public string Name => "Snooper";

        /// <summary>
        /// Gets the description string of this mod.
        /// </summary>
        public string Description => "Shows the origin buildings where the citizens are moving from. Version: " + modVersion;

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

            try
            {
                patcher.Apply();
            }
            catch (Exception ex)
            {
                Debug.LogError("The 'Snooper' mod failed to perform method redirections: " + ex);
                SafeRevertPatches();
                return;
            }

            WorldInfoPanelPatches.CitizenInfoPanel = CustomCitizenInfoPanel.Enable();
            WorldInfoPanelPatches.VehicleInfoPanel = CustomVehicleInfoPanel.Enable();
        }

        /// <summary>
        /// Called when a game level is about to be unloaded. If the Snooper mod was activated
        /// for this level, deactivates the mod for this level.
        /// </summary>
        public override void OnLevelUnloading()
        {
            SafeRevertPatches();

            WorldInfoPanelPatches.CitizenInfoPanel?.Disable();
            WorldInfoPanelPatches.CitizenInfoPanel = null;

            WorldInfoPanelPatches.VehicleInfoPanel?.Disable();
            WorldInfoPanelPatches.VehicleInfoPanel = null;
        }

        private void SafeRevertPatches()
        {
            try
            {
                patcher.Revert();
            }
            catch (Exception ex)
            {
                Debug.LogWarning("The 'Snooper' mod failed to revert method redirections: " + ex);
            }
        }
    }
}
