// <copyright file="CameraHelper.cs" company="dymanoid">
// Copyright (c) dymanoid. All rights reserved.
// </copyright>

namespace Snooper
{
    using UnityEngine;

    /// <summary>
    /// A helper class for interacting with the game's main camera.
    /// </summary>
    internal static class CameraHelper
    {
        /// <summary>Moves the camera to the building with specified ID and zooms in to show the building closely.
        /// Does nothing if the <paramref name="buildingId"/>is 0.</summary>
        /// <param name="buildingId">The ID of the building to navigate to.</param>
        /// <param name="zoomIn"><c>true</c> to zoom in to the building, <c>false</c> to not change the zoom.</param>
        public static void NavigateToBuilding(ushort buildingId, bool zoomIn = true)
        {
            if (buildingId == 0)
            {
                return;
            }

            InstanceID instance = default;
            instance.Building = buildingId;

            Vector3 buildingPosition = BuildingManager.instance.m_buildings.m_buffer[buildingId].m_position;
            ToolsModifierControl.cameraController.SetTarget(instance, buildingPosition, zoomIn);
        }
    }
}
