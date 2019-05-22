// <copyright file="CustomCitizenInfoPanel.cs" company="dymanoid">
// Copyright (c) dymanoid. All rights reserved.
// </copyright>

namespace Snooper
{
    using ColossalFramework.Globalization;
    using ColossalFramework.UI;
    using SkyTools.UI;
    using UnityEngine;

    /// <summary>
    /// A customized citizen info panel that additionally shows the origin building of the citizen.
    /// </summary>
    internal sealed class CustomCitizenInfoPanel : CustomHumanInfoPanelBase<HumanWorldInfoPanel>
    {
        private const string GameInfoPanelName = "(Library) CitizenWorldInfoPanel";
        private const string WealthComponentId = "WealthLabelComponent";

        private UIPanel wealthPanel;
        private UILabel wealthLabel;

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
                UpdateWealth(0);
            }
            else
            {
                ushort instanceId = CitizenManager.instance.m_citizens.m_buffer[instance.Citizen].m_instance;
                UpdateOriginFromInstance(instanceId, instance.Index);
                UpdateCar(instance.Citizen);
                UpdateWealth(instance.Citizen);
            }
        }

        /// <summary>
        /// Builds up the custom UI objects for the info panel.
        /// </summary>
        /// <returns><c>true</c> on success; otherwise, <c>false</c>.</returns>
        protected override bool InitializeCore()
        {
            bool result = base.InitializeCore();
            if (!result)
            {
                return result;
            }

            wealthPanel = UIComponentTools.CreateCopy(OriginPanel, ItemsPanel);
            wealthLabel = UIComponentTools.CreateCopy(OriginLabel, wealthPanel, WealthComponentId);

            wealthLabel.text = string.Empty;
            wealthPanel.isVisible = false;

            return true;
        }

        /// <summary>Disables the custom citizen info panel, if it is enabled.</summary>
        protected override void DisableCore()
        {
            base.DisableCore();

            if (wealthPanel == null)
            {
                return;
            }

            if (wealthLabel != null)
            {
                wealthPanel.RemoveUIComponent(wealthLabel);
                Object.Destroy(wealthLabel.gameObject);
                wealthLabel = null;
            }

            wealthPanel.parent?.RemoveUIComponent(wealthPanel);

            Object.Destroy(wealthPanel.gameObject);
            wealthPanel = null;
        }

        private static string GetWealthAndAge(uint citizenId)
        {
            if (citizenId == 0)
            {
                return string.Empty;
            }

            ref var citizen = ref CitizenManager.instance.m_citizens.m_buffer[citizenId];
            var age = Citizen.GetAgeGroup(citizen.Age);
            var wealth = citizen.WealthLevel;
            return Locale.Get(LocaleID.TOURIST_AGEWEALTH, string.Concat(wealth, age));
        }

        private void UpdateWealth(uint citizenId)
        {
            if (citizenId == 0)
            {
                SetCustomPanelVisibility(wealthPanel, false);
                return;
            }

            SetCustomPanelVisibility(wealthPanel, true);
            wealthLabel.text = GetWealthAndAge(citizenId);
        }
    }
}
