// <copyright file="SnooperInfoPanelBase.cs" company="dymanoid">
// Copyright (c) dymanoid. All rights reserved.
// </copyright>

namespace Snooper
{
    using System.Linq;
    using ColossalFramework.UI;
    using SkyTools.GameTools;
    using SkyTools.UI;
    using UnityEngine;

    /// <summary>A base class for the customized world info panels.</summary>
    /// <typeparam name="T">The type of the game world info panel to customize.</typeparam>
    internal abstract class SnooperInfoPanelBase<T> : CustomInfoPanelBase<T>
        where T : WorldInfoPanel
    {
        /// <summary>The symbolic name of the info panel's target button.</summary>
        protected const string TargetButtonName = "Target";

        private const string OriginComponentId = "OriginBuildingInfo";

        /// <summary>Initializes a new instance of the <see cref="SnooperInfoPanelBase{T}"/> class.</summary>
        /// <param name="panelName">Name of the game's panel object.</param>
        /// <exception cref="System.ArgumentException">
        /// Thrown when <paramref name="panelName"/> is null or an empty string.
        /// </exception>
        protected SnooperInfoPanelBase(string panelName)
            : base(panelName)
        {
        }

        /// <summary>Gets the origin building panel. Valid only after initialization via <see cref="InitializeCore"/>.</summary>
        protected UIPanel OriginPanel { get; private set; }

        /// <summary>Gets the origin building label. Valid only after initialization via <see cref="InitializeCore"/>.</summary>
        protected UILabel OriginLabel { get; private set; }

        /// <summary>Gets the origin building button. Valid only after initialization via <see cref="InitializeCore"/>.</summary>
        protected UIButton OriginButton { get; private set; }

        /// <summary>Disables the custom citizen info panel, if it is enabled.</summary>
        protected override void DisableCore()
        {
            if (OriginPanel == null)
            {
                return;
            }

            if (OriginLabel != null)
            {
                OriginPanel.RemoveUIComponent(OriginLabel);
                Object.Destroy(OriginLabel.gameObject);
                OriginLabel = null;
            }

            if (OriginButton != null)
            {
                OriginButton.eventClick -= OriginButtonClick;
                OriginPanel.RemoveUIComponent(OriginButton);
                Object.Destroy(OriginButton.gameObject);
                OriginButton = null;
            }

            OriginPanel.parent?.RemoveUIComponent(OriginPanel);

            Object.Destroy(OriginPanel.gameObject);
            OriginPanel = null;
        }

        /// <summary>Updates the origin building display using the specified citizen instance ID. If 0 value is specified,
        /// hides the origin building panel.</summary>
        /// <param name="instanceId">The citizen instance ID to search the origin building for.</param>
        /// <param name="instanceIndex">The game instance object's index value.</param>
        protected void UpdateOriginFromInstance(ushort instanceId, uint instanceIndex)
        {
            if (instanceId == 0)
            {
                SetCustomPanelVisibility(OriginPanel, false);
                return;
            }

            ushort originBuildingId = GetSourceBuilding(instanceId);
            UpdateOriginFromBuilding(originBuildingId, instanceIndex);
        }

        /// <summary>Updates the origin building display using the specified building ID. If 0 value is specified,
        /// hides the origin building panel.</summary>
        /// <param name="buildingId">The ID of the building to show as origin.</param>
        /// <param name="instanceIndex">The game instance object's index value.</param>
        protected void UpdateOriginFromBuilding(ushort buildingId, uint instanceIndex)
        {
            if (buildingId == 0)
            {
                SetCustomPanelVisibility(OriginPanel, false);
                return;
            }

            SetCustomPanelVisibility(OriginPanel, true);
            OriginButton.text = GetBuildingName(buildingId, instanceIndex, out bool isObservable);
            OriginButton.isEnabled = isObservable;
            OriginButton.objectUserData = buildingId;
            UIComponentTools.ShortenTextToFitParent(OriginButton);
        }

        /// <summary>
        /// Builds up the custom UI objects for the info panel.
        /// </summary>
        /// <returns><c>true</c> on success; otherwise, <c>false</c>.</returns>
        protected override bool InitializeCore()
        {
            if (!GetUIObjects(InfoPanelName, ItemsPanel, out var targetPanel, out var targetLabel, out var targetButton))
            {
                return false;
            }

            OriginPanel = UIComponentTools.CreateCopy(targetPanel, ItemsPanel);
            OriginLabel = UIComponentTools.CreateCopy(targetLabel, OriginPanel, OriginComponentId + targetLabel.name);
            OriginButton = UIComponentTools.CreateCopy(targetButton, OriginPanel, OriginComponentId + targetButton.name);

            OriginButton.eventClick += OriginButtonClick;
            OriginLabel.text = "▣";
            OriginPanel.isVisible = false;

            return true;
        }

        private static bool GetUIObjects(string infoPanelName, UIPanel itemsPanel, out UIPanel targetPanel, out UILabel targetLabel, out UIButton targetButton)
        {
            targetPanel = null;
            targetLabel = null;

            targetButton = itemsPanel.Find<UIButton>(TargetButtonName);
            if (targetButton == null)
            {
                Debug.LogWarning($"The 'Snooper' mod failed to customize the info panel '{infoPanelName}'. Target button instance is null.");
                return false;
            }

            targetPanel = targetButton.parent as UIPanel;
            if (targetPanel == null)
            {
                Debug.LogWarning($"The 'Snooper' mod failed to customize the info panel '{infoPanelName}'. The target button's parent is null.");
                return false;
            }

            targetLabel = targetPanel.components.OfType<UILabel>().FirstOrDefault();
            if (targetLabel == null)
            {
                Debug.LogWarning($"The 'Snooper' mod failed to customize the info panel '{infoPanelName}'. No target label found.");
                return false;
            }

            return true;
        }

        private static ushort GetSourceBuilding(ushort instanceId) =>
            instanceId == 0
                ? (ushort)0
                : CitizenManager.instance.m_instances.m_buffer[instanceId].m_sourceBuilding;

        private static string GetBuildingName(ushort buildingId, uint instanceIndex, out bool isObservable)
        {
            if (buildingId == 0)
            {
                isObservable = false;
                return string.Empty;
            }

            var buildingInfo = BuildingManager.instance.m_buildings.m_buffer[buildingId].Info;
            isObservable = buildingInfo != null && !(buildingInfo.m_buildingAI is OutsideConnectionAI);
            return BuildingManager.instance.GetBuildingName(buildingId, new InstanceID { Building = buildingId, Index = instanceIndex });
        }

        private static void OriginButtonClick(UIComponent component, UIMouseEventParameter eventParam)
        {
            if (component.objectUserData is ushort)
            {
                CameraHelper.NavigateToBuilding((ushort)component.objectUserData, false);
            }
        }
    }
}
