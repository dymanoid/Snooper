// <copyright file="CitizenWorldInfoPanelPatch.cs" company="dymanoid">
// Copyright (c) dymanoid. All rights reserved.
// </copyright>

namespace Snooper
{
    using System;
    using System.Reflection;

    internal static class CitizenWorldInfoPanelPatch
    {
        public static CustomCitizenInfoPanel CitizenInfoPanel { get; set; }

        public static IPatch UpdateBindings { get; } = new CitizenWorldInfoPanel_UpdateBindings();

        internal sealed class CitizenWorldInfoPanel_UpdateBindings : PatchBase
        {
            protected override MethodInfo GetMethod()
            {
                return typeof(CitizenWorldInfoPanel).GetMethod(
                    "UpdateBindings",
                    BindingFlags.Instance | BindingFlags.NonPublic,
                    null,
                    new Type[0],
                    new ParameterModifier[0]);
            }

#pragma warning disable SA1313 // Parameter names must begin with lower-case letter
            private static void Postfix(ref InstanceID ___m_InstanceID)
            {
                CitizenInfoPanel?.UpdateOrigin(ref ___m_InstanceID);
            }
#pragma warning restore SA1313 // Parameter names must begin with lower-case letter
        }
    }
}
