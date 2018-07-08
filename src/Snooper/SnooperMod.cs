// <copyright file="SnooperMod.cs" company="dymanoid">
// Copyright (c) dymanoid. All rights reserved.
// </copyright>

namespace Snooper
{
    using System;
    using System.Linq;
    using ColossalFramework.Plugins;
    using ICities;
    using UnityEngine;

    /// <summary>
    /// The main class of the Snooper mod.
    /// </summary>
    public sealed class SnooperMod : IUserMod
    {
        private const ulong WorkshopId = 0;

        private readonly string modVersion = GitVersion.GetAssemblyVersion(typeof(SnooperMod).Assembly);
        private readonly string modPath = GetModPath();
        private readonly MethodPatcher patcher;

        public SnooperMod()
        {
            patcher = new MethodPatcher(
                CitizenWorldInfoPanelPatch.UpdateBindings,
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
        /// Called when this mod is enabled.
        /// </summary>
        public void OnEnabled()
        {
            Debug.Log("The 'Snooper' mod has been enabled, version: " + modVersion);

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

            CitizenWorldInfoPanelPatch.CitizenInfoPanel = CustomCitizenInfoPanel.Enable();
        }

        /// <summary>
        /// Called when this mod is disabled.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Must be instance method due to C:S API")]
        public void OnDisabled()
        {
            SafeRevertPatches();
            CitizenWorldInfoPanelPatch.CitizenInfoPanel?.Disable();
            CitizenWorldInfoPanelPatch.CitizenInfoPanel = null;
            Debug.Log("The 'Snooper' mod has been disabled.");
        }

        private static string GetModPath()
        {
            string assemblyName = typeof(SnooperMod).Assembly.GetName().Name;

            PluginManager.PluginInfo pluginInfo = PluginManager.instance.GetPluginsInfo()
                .FirstOrDefault(pi => pi.name == assemblyName || pi.publishedFileID.AsUInt64 == WorkshopId);

            return pluginInfo == null
                ? Environment.CurrentDirectory
                : pluginInfo.modPath;
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
