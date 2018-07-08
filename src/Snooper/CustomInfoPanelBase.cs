// <copyright file="CustomInfoPanelBase.cs" company="dymanoid">Copyright (c) dymanoid. All rights reserved.</copyright>

namespace Snooper
{
    using System;
    using System.Linq;
    using System.Reflection;
    using ColossalFramework.UI;
    using UnityEngine;

    /// <summary>A base class for the customized world info panels.</summary>
    /// <typeparam name="T">The type of the game world info panel to customize.</typeparam>
    internal abstract class CustomInfoPanelBase<T>
        where T : WorldInfoPanel
    {
        private const string ButtonId = "OriginBuildingInfoLabel";
        private const string TargetButtonFieldName = "m_Target";

        private UIPanel originPanel;
        private UILabel originLabel;
        private UIButton originButton;
        private float newHeight;
        private float defaultHeight;

        /// <summary>Initializes a new instance of the <see cref="CustomInfoPanelBase{T}"/> class.</summary>
        /// <param name="panelName">Name of the game's panel object.</param>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="panelName"/> is null or an empty string.
        /// </exception>
        protected CustomInfoPanelBase(string panelName)
        {
            if (string.IsNullOrEmpty(panelName))
            {
                throw new ArgumentException("The panel name cannot be null or an empty string.", nameof(panelName));
            }

            Initialize(panelName);
        }

        /// <summary>Gets a value indicating whether this instance is valid.</summary>
        protected bool IsValid => originPanel != null && originLabel != null && originButton != null;

        /// <summary>Disables the custom citizen info panel, if it is enabled.</summary>
        public void Disable()
        {
            if (originPanel == null)
            {
                return;
            }

            if (originLabel != null)
            {
                originPanel.RemoveUIComponent(originLabel);
                UnityEngine.Object.Destroy(originLabel.gameObject);
                originLabel = null;
            }

            if (originButton != null)
            {
                originButton.eventClick -= OriginButtonClick;
                originPanel.RemoveUIComponent(originButton);
                UnityEngine.Object.Destroy(originButton.gameObject);
                originButton = null;
            }

            if (originPanel.parent != null)
            {
                originPanel.parent.RemoveUIComponent(originPanel);
            }

            UnityEngine.Object.Destroy(originPanel.gameObject);
            originPanel = null;
        }

        /// <summary>When implemented in derived classes, updates the origin building display.</summary>
        /// <param name="citizenInstance">The game object instance to get the information from.</param>
        public abstract void UpdateOrigin(ref InstanceID citizenInstance);

        /// <summary>Updates the origin building display using the specified citizen instance ID. If 0 value is specified,
        /// hides the origin building panel.</summary>
        /// <param name="instanceId">The citizen instance ID to search the origin building for.</param>
        protected void UpdateOrigin(ushort instanceId)
        {
            if (instanceId == 0)
            {
                UpdateVisibility(false);
                return;
            }

            ushort originBuildingId = GetSourceBuilding(instanceId);
            if (originBuildingId == 0)
            {
                UpdateVisibility(false);
            }
            else
            {
                UpdateVisibility(true);
                originButton.text = GetBuildingName(originBuildingId);
                originButton.objectUserData = originBuildingId;
                UIComponentTools.ShortenTextToFitParent(originButton);
            }
        }

        private static void GetUIObjects(string panelName, out UIComponent itemsPanel, out UIPanel targetPanel, out UILabel targetLabel, out UIButton targetButton)
        {
            itemsPanel = null;
            targetPanel = null;
            targetLabel = null;
            targetButton = null;

            var panelGameObject = GameObject.Find(panelName);
            if (panelGameObject == null)
            {
                Debug.LogWarning($"The 'Snooper' mod failed to customize the info panel '{panelName}'. No game object '{panelName}' found.");
                return;
            }

            T infoPanel = panelGameObject.GetComponent<T>();
            if (infoPanel == null)
            {
                Debug.LogWarning($"The 'Snooper' mod failed to customize the info panel '{panelName}'. No game object's component of type '{typeof(T).Name}' found.");
                return;
            }

            try
            {
                FieldInfo targetField = typeof(T).GetField(TargetButtonFieldName, BindingFlags.Instance | BindingFlags.NonPublic);
                if (targetField == null)
                {
                    Debug.LogWarning($"The 'Snooper' mod failed to customize the info panel '{panelName}'. No target button field found.");
                    return;
                }

                targetButton = targetField.GetValue(infoPanel) as UIButton;
                if (targetButton == null)
                {
                    Debug.LogWarning($"The 'Snooper' mod failed to customize the info panel '{panelName}'. Target button instance is null.");
                    return;
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"The 'Snooper' mod failed to customize the info panel '{panelName}'. Error message: " + ex);
                return;
            }

            targetPanel = targetButton.parent as UIPanel;
            if (targetPanel == null)
            {
                Debug.LogWarning($"The 'Snooper' mod failed to customize the info panel '{panelName}'. The target button's parent is null.");
                return;
            }

            targetLabel = targetPanel.components.OfType<UILabel>().FirstOrDefault();
            if (targetLabel == null)
            {
                Debug.LogWarning($"The 'Snooper' mod failed to customize the info panel '{panelName}'. No target label found.");
                return;
            }

            itemsPanel = targetPanel.parent;
            if (itemsPanel == null)
            {
                Debug.LogWarning($"The 'Snooper' mod failed to customize the info panel '{panelName}'. The info panel's items panel is null.");
            }
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

        private void Initialize(string panelName)
        {
            GetUIObjects(panelName, out UIComponent itemsPanel, out UIPanel targetPanel, out UILabel targetLabel, out UIButton targetButton);

            if (itemsPanel == null || targetPanel == null || targetLabel == null || targetButton == null)
            {
                return;
            }

            defaultHeight = itemsPanel.parent?.height ?? 0;

            string buttonId = ButtonId + GetType().Name;
            originPanel = UIComponentTools.CreateCopy(targetPanel, itemsPanel);
            originLabel = UIComponentTools.CreateCopy(targetLabel, originPanel, buttonId);
            originButton = UIComponentTools.CreateCopy(targetButton, originPanel, buttonId);

            newHeight = defaultHeight + originPanel.height + originPanel.padding.bottom + originPanel.padding.top;

            originButton.eventClick += OriginButtonClick;
            originLabel.text = "⚑➜";
            originPanel.isVisible = false;
        }

        private void UpdateVisibility(bool visible)
        {
            if (originPanel.isVisible == visible)
            {
                return;
            }

            originPanel.isVisible = visible;

            UIComponent parent = originPanel.parent?.parent;
            if (parent != null)
            {
                parent.height = visible ? newHeight : defaultHeight;
            }
        }
    }
}