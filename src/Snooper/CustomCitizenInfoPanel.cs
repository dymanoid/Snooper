// <copyright file="CustomCitizenInfoPanel.cs" company="dymanoid">
// Copyright (c) dymanoid. All rights reserved.
// </copyright>

namespace Snooper
{
    /// <summary>
    /// A customized citizen info panel that additionally shows the origin building of the citizen.
    /// </summary>
    internal sealed class CustomCitizenInfoPanel : CustomHumanInfoPanelBase<HumanWorldInfoPanel>
    {
        private const string GameInfoPanelName = "(Library) CitizenWorldInfoPanel";

        private CustomCitizenInfoPanel(string panelName)
            : base(panelName)
        {
        }

        /// <summary>Enables the citizen info panel customization. Can return null on failure.</summary>
        /// <returns>An instance of the <see cref="CustomCitizenInfoPanel"/> class that can be used for disabling
        /// the customization, or null when the customization fails.</returns>
        public static CustomCitizenInfoPanel Enable()
        {
            var result = new CustomCitizenInfoPanel(GameInfoPanelName);
            return result.Initialize() ? result : null;
        }

        /// <summary>Updates the origin building display.</summary>
        /// <param name="instance">The game object instance to get the information from.</param>
        public override void UpdateCustomInfo(ref InstanceID instance)
        {
            if (instance.Type != InstanceType.Citizen)
            {
                UpdateOriginFromInstance(0, 0);
                UpdateCar(0);
            }
            else
            {
                ushort instanceId = CitizenManager.instance.m_citizens.m_buffer[instance.Citizen].m_instance;
                UpdateOriginFromInstance(instanceId, instance.Index);
                UpdateCar(instance.Citizen);
            }
        }
    }
}
