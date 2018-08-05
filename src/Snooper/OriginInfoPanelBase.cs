// <copyright file="OriginInfoPanelBase.cs" company="dymanoid">
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
    internal abstract class OriginInfoPanelBase<T> : CustomInfoPanelBase<T>
        where T : WorldInfoPanel
    {
        private const string ComponentId = "OriginBuildingInfo";
        private const string TargetButtonName = "Target";

        private UIPanel originPanel;
        private UILabel originLabel;
        private UIButton originButton;

        /// <summary>Initializes a new instance of the <see cref="OriginInfoPanelBase{T}"/> class.</summary>
        /// <param name="panelName">Name of the game's panel object.</param>
        /// <exception cref="System.ArgumentException">
        /// Thrown when <paramref name="panelName"/> is null or an empty string.
        /// </exception>
        protected OriginInfoPanelBase(string panelName)
            : base(panelName)
        {
        }

        /// <summary>Disables the custom citizen info panel, if it is enabled.</summary>
        protected override sealed void DisableCore()
        {
            if (originPanel == null)
            {
                return;
            }

            if (originLabel != null)
            {
                originPanel.RemoveUIComponent(originLabel);
                Object.Destroy(originLabel.gameObject);
                originLabel = null;
            }

            if (originButton != null)
            {
                originButton.eventClick -= OriginButtonClick;
                originPanel.RemoveUIComponent(originButton);
                Object.Destroy(originButton.gameObject);
                originButton = null;
            }

            if (originPanel.parent != null)
            {
                originPanel.parent.RemoveUIComponent(originPanel);
            }

            Object.Destroy(originPanel.gameObject);
            originPanel = null;
        }

        /// <summary>Updates the origin building display using the specified citizen instance ID. If 0 value is specified,
        /// hides the origin building panel.</summary>
        /// <param name="instanceId">The citizen instance ID to search the origin building for.</param>
        protected void UpdateOrigin(ushort instanceId)
        {
            if (instanceId == 0)
            {
                SetCustomPanelVisibility(originPanel, false);
                return;
            }

            ushort originBuildingId = GetSourceBuilding(instanceId);
            if (originBuildingId == 0)
            {
                SetCustomPanelVisibility(originPanel, false);
            }
            else
            {
                SetCustomPanelVisibility(originPanel, true);
                originButton.text = GetBuildingName(originBuildingId);
                originButton.objectUserData = originBuildingId;
                UIComponentTools.ShortenTextToFitParent(originButton);
            }
        }

        /// <summary>
        /// Builds up the custom UI objects for the info panel.
        /// </summary>
        /// <returns><c>true</c> on success; otherwise, <c>false</c>.</returns>
        protected override sealed bool InitializeCore()
        {
            if (!GetUIObjects(InfoPanelName, ItemsPanel, out UIPanel targetPanel, out UILabel targetLabel, out UIButton targetButton))
            {
                return false;
            }

            originPanel = UIComponentTools.CreateCopy(targetPanel, ItemsPanel);
            originLabel = UIComponentTools.CreateCopy(targetLabel, originPanel, ComponentId + targetLabel.name);
            originButton = UIComponentTools.CreateCopy(targetButton, originPanel, ComponentId + targetButton.name);

            originButton.eventClick += OriginButtonClick;
            originLabel.text = "▣";
            originPanel.isVisible = false;

            return true;
        }

        private static bool GetUIObjects(string infoPanelName, UIPanel itemsPanel, out UIPanel targetPanel, out UILabel targetLabel, out UIButton targetButton)
        {
            targetPanel = null;
            targetLabel = null;
            targetButton = null;

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

        private static ushort GetSourceBuilding(ushort instanceId)
        {
            return instanceId == 0
                ? (ushort)0
                : CitizenManager.instance.m_instances.m_buffer[instanceId].m_sourceBuilding;
        }

        private static string GetBuildingName(ushort buildingId)
        {
            return buildingId == 0
                ? string.Empty
                : BuildingManager.instance.GetBuildingName(buildingId, InstanceID.Empty);
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