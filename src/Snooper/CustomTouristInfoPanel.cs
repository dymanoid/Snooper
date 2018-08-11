// <copyright file="CustomTouristInfoPanel.cs" company="dymanoid">
// Copyright (c) dymanoid. All rights reserved.
// </copyright>

namespace Snooper
{
    /// <summary>
    /// A customized citizen info panel that additionally shows the origin building of the toursit.
    /// </summary>
    internal sealed class CustomTouristInfoPanel : CustomHumanInfoPanelBase<HumanWorldInfoPanel>
    {
        private const string GameInfoPanelName = "(Library) TouristWorldInfoPanel";

        private CustomTouristInfoPanel(string panelName)
            : base(panelName)
        {
        }

        /// <summary>Enables the tourist info panel customization. Can return null on failure.</summary>
        /// <returns>An instance of the <see cref="CustomTouristInfoPanel"/> class that can be used for disabling
        /// the customization, or null when the customization fails.</returns>
        public static CustomTouristInfoPanel Enable()
        {
            var result = new CustomTouristInfoPanel(GameInfoPanelName);
            return result.Initialize() ? result : null;
        }

        /// <summary>Updates the origin building display.</summary>
        /// <param name="instance">The game object instance to get the information from.</param>
        public override void UpdateCustomInfo(ref InstanceID instance)
        {
            if (instance.Type != InstanceType.Citizen)
            {
                UpdateOriginFromInstance(0);
                UpdateCar(0);
            }
            else
            {
                ushort instanceId = CitizenManager.instance.m_citizens.m_buffer[instance.Citizen].m_instance;
                UpdateOriginFromInstance(instanceId);
                UpdateCar(instance.Citizen);
            }
        }
    }
}
