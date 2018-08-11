// <copyright file="CustomCityServiceVehicleInfoPanel.cs" company="dymanoid">
// Copyright (c) dymanoid. All rights reserved.
// </copyright>

namespace Snooper
{
    /// <summary>
    /// A customized service vehicle info panel that additionally shows the origin building of the service vehicles.
    /// </summary>
    internal sealed class CustomCityServiceVehicleInfoPanel : SnooperInfoPanelBase<CityServiceVehicleWorldInfoPanel>
    {
        private const string GameInfoPanelName = "(Library) CityServiceVehicleWorldInfoPanel";

        private CustomCityServiceVehicleInfoPanel(string panelName)
            : base(panelName)
        {
        }

        /// <summary>Enables the vehicle info panel customization. Can return null on failure.</summary>
        /// <returns>An instance of the <see cref="CustomCityServiceVehicleInfoPanel"/> class that can be used for disabling
        /// the customization, or null when the customization fails.</returns>
        public static CustomCityServiceVehicleInfoPanel Enable()
        {
            var result = new CustomCityServiceVehicleInfoPanel(GameInfoPanelName);
            return result.Initialize() ? result : null;
        }

        /// <summary>Updates the origin building information in this panel.</summary>
        /// <param name="instance">The game object instance to get the information from.</param>
        public override void UpdateCustomInfo(ref InstanceID instance)
        {
            if (instance.Type != InstanceType.Vehicle || instance.Vehicle == 0)
            {
                UpdateOriginFromBuilding(0, 0);
                return;
            }

            ushort leadingVehicle = VehicleManager.instance.m_vehicles.m_buffer[instance.Vehicle].GetFirstVehicle(instance.Vehicle);
            ref Vehicle vehicle = ref VehicleManager.instance.m_vehicles.m_buffer[leadingVehicle];
            ushort originBuilding;
            if ((vehicle.m_flags & Vehicle.Flags.GoingBack) == 0)
            {
                originBuilding = 0;
            }
            else if (vehicle.m_targetBuilding != 0)
            {
                originBuilding = vehicle.m_targetBuilding;
            }
            else
            {
                // This is intentional, see comments in the CargoTruckAIPatches class
                originBuilding = vehicle.m_touristCount;
            }

            UpdateOriginFromBuilding(originBuilding, instance.Index);
        }
    }
}
