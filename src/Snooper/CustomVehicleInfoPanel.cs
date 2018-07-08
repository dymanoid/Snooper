// <copyright file="CustomVehicleInfoPanel.cs" company="dymanoid">
// Copyright (c) dymanoid. All rights reserved.
// </copyright>

namespace Snooper
{
    /// <summary>
    /// A customized vehicle info panel that additionally shows the origin building of the owner citizen.
    /// </summary>
    internal sealed class CustomVehicleInfoPanel : CustomInfoPanelBase<VehicleWorldInfoPanel>
    {
        private const string GameInfoPanelName = "(Library) CitizenVehicleWorldInfoPanel";

        private CustomVehicleInfoPanel(string panelName)
            : base(panelName)
        {
        }

        /// <summary>Enables the vehicle info panel customization. Can return null on failure.</summary>
        /// <returns>An instance of the <see cref="CustomVehicleInfoPanel"/> object that can be used for disabling
        /// the customization, or null when the customization fails.</returns>
        public static CustomVehicleInfoPanel Enable()
        {
            var result = new CustomVehicleInfoPanel(GameInfoPanelName);
            return result.IsValid ? result : null;
        }

        /// <summary>Updates the origin building display.</summary>
        /// <param name="citizenInstance">The game object instance to get the information from.</param>
        public override void UpdateOrigin(ref InstanceID citizenInstance)
        {
            if (citizenInstance.Type != InstanceType.Vehicle)
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
