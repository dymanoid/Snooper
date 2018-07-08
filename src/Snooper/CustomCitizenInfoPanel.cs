// <copyright file="CustomCitizenInfoPanel.cs" company="dymanoid">
// Copyright (c) dymanoid. All rights reserved.
// </copyright>

namespace Snooper
{
    /// <summary>
    /// A customized citizen info panel that additionally shows the origin building of the citizen.
    /// </summary>
    internal sealed class CustomCitizenInfoPanel : CustomInfoPanelBase<HumanWorldInfoPanel>
    {
        private const string GameInfoPanelName = "(Library) CitizenWorldInfoPanel";

        private CustomCitizenInfoPanel(string panelName)
            : base(panelName)
        {
        }

        /// <summary>Enables the citizen info panel customization. Can return null on failure.</summary>
        /// <returns>An instance of the <see cref="CustomCitizenInfoPanel"/> object that can be used for disabling
        /// the customization, or null when the customization fails.</returns>
        public static CustomCitizenInfoPanel Enable()
        {
            var result = new CustomCitizenInfoPanel(GameInfoPanelName);
            return result.IsValid ? result : null;
        }

        /// <summary>Updates the origin building display.</summary>
        /// <param name="citizenInstance">The game object instance to get the information from.</param>
        public override void UpdateOrigin(ref InstanceID citizenInstance)
        {
            if (citizenInstance.Type != InstanceType.Citizen)
            {
                UpdateOrigin(0);
            }
            else
            {
                UpdateOrigin(citizenInstance.Citizen);
            }
        }
    }
}
