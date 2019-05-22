// <copyright file="CustomHumanInfoPanelBase.cs" company="dymanoid">
// Copyright (c) dymanoid. All rights reserved.
// </copyright>

namespace Snooper
{
    using ColossalFramework.UI;
    using SkyTools.UI;
    using UnityEngine;

    /// <summary>A base class for the customized world info panels for people.</summary>
    /// <typeparam name="T">The type of the game world info panel to customize.</typeparam>
    internal abstract class CustomHumanInfoPanelBase<T> : SnooperInfoPanelBase<T>
        where T : WorldInfoPanel
    {
        private const string CarComponentId = "OwnedCarInfo";

        private UIPanel carPanel;
        private UILabel carLabel;
        private UIButton carButton;

        /// <summary>Initializes a new instance of the <see cref="CustomHumanInfoPanelBase{T}"/> class.</summary>
        /// <param name="panelName">Name of the game's panel object.</param>
        /// <exception cref="System.ArgumentException">
        /// Thrown when <paramref name="panelName"/> is null or an empty string.
        /// </exception>
        protected CustomHumanInfoPanelBase(string panelName)
            : base(panelName)
        {
        }

        /// <summary>Disables the custom citizen info panel, if it is enabled.</summary>
        protected sealed override void DisableCore()
        {
            base.DisableCore();

            if (carPanel == null)
            {
                return;
            }

            if (carLabel != null)
            {
                carPanel.RemoveUIComponent(carLabel);
                Object.Destroy(carLabel.gameObject);
                carLabel = null;
            }

            if (carButton != null)
            {
                carButton.eventClick -= CarButtonClick;
                carPanel.RemoveUIComponent(carButton);
                Object.Destroy(carButton.gameObject);
                carButton = null;
            }

            carPanel.parent?.RemoveUIComponent(carPanel);

            Object.Destroy(carPanel.gameObject);
            carPanel = null;
        }

        /// <summary>Updates the parked car display using the specified citizen instance ID. If 0 value is specified,
        /// hides the parked car panel.</summary>
        /// <param name="citizenId">The citizen ID to search the parked car for.</param>
        protected void UpdateCar(uint citizenId)
        {
            if (citizenId == 0)
            {
                SetCustomPanelVisibility(carPanel, false);
                return;
            }

            ushort parkedCarId = GetParkedVehicle(citizenId);
            if (parkedCarId == 0)
            {
                SetCustomPanelVisibility(carPanel, false);
            }
            else
            {
                SetCustomPanelVisibility(carPanel, true);
                carButton.text = GetVehicleName(parkedCarId);
                carButton.objectUserData = parkedCarId;
                UIComponentTools.ShortenTextToFitParent(carButton);
            }
        }

        /// <summary>
        /// Builds up the custom UI objects for the info panel.
        /// </summary>
        /// <returns><c>true</c> on success; otherwise, <c>false</c>.</returns>
        protected sealed override bool InitializeCore()
        {
            bool result = base.InitializeCore();
            if (!result)
            {
                return result;
            }

            carPanel = UIComponentTools.CreateCopy(OriginPanel, ItemsPanel);
            carLabel = UIComponentTools.CreateCopy(OriginLabel, carPanel, CarComponentId + "Label");
            carButton = UIComponentTools.CreateCopy(OriginButton, carPanel, CarComponentId + "Button");

            carButton.eventClick += CarButtonClick;
            carLabel.text = "| P| ";
            carPanel.isVisible = false;

            return true;
        }

        private static ushort GetParkedVehicle(uint citizenId)
        {
            return citizenId == 0
                ? (ushort)0
                : CitizenManager.instance.m_citizens.m_buffer[citizenId].m_parkedVehicle;
        }

        private static string GetVehicleName(ushort carId)
        {
            return carId == 0
                ? string.Empty
                : VehicleManager.instance.GetParkedVehicleName(carId);
        }

        private void CarButtonClick(UIComponent component, UIMouseEventParameter eventParam)
        {
            if (!(component.objectUserData is ushort))
            {
                return;
            }

            ushort vehicleId = (ushort)component.objectUserData;

            InstanceID instance = default;
            instance.ParkedVehicle = vehicleId;
            Vector3 position = VehicleManager.instance.m_parkedVehicles.m_buffer[vehicleId].m_position;

            ToolsModifierControl.cameraController.SetTarget(instance, position, true);
        }
    }
}