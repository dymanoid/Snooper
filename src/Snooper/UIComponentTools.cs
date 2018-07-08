// <copyright file="UIComponentTools.cs" company="dymanoid">Copyright (c) dymanoid. All rights reserved.</copyright>

namespace Snooper
{
    using System;
    using ColossalFramework.UI;

    /// <summary>A helper class for the UI components.</summary>
    internal static class UIComponentTools
    {
        /// <summary>Creates a copy of the specified button and inserts it into the specified container item.</summary>
        /// <param name="template">The button to copy.</param>
        /// <param name="container">The container to insert the newly created button.</param>
        /// <param name="name">The new button's name.</param>
        /// <returns>
        /// A newly created <see cref="UIButton"/> whose properties are set to same values as in the <paramref name="template"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="template"/> or <paramref name="container"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="name"/> is null or an empty string.</exception>
        public static UIButton CreateCopy(UIButton template, UIComponent container, string name)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            if (template == null)
            {
                throw new ArgumentNullException(nameof(template));
            }

            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("The button name cannot be null or an empty string.", nameof(name));
            }

            UIButton result = container.AddUIComponent<UIButton>();
            result.name = name;
            result.height = template.height;
            result.width = template.width;
            result.autoSize = template.autoSize;
            result.textPadding = template.textPadding;
            result.textScale = template.textScale;
            result.textColor = template.textColor;
            result.textScaleMode = template.textScaleMode;
            result.font = template.font;
            result.disabledTextColor = template.disabledTextColor;
            result.hoveredTextColor = template.hoveredTextColor;
            result.focusedTextColor = template.focusedTextColor;
            result.pressedTextColor = template.pressedTextColor;
            result.useDropShadow = template.useDropShadow;
            result.dropShadowColor = template.dropShadowColor;
            result.dropShadowOffset = template.dropShadowOffset;
            result.maximumSize = template.maximumSize;
            return result;
        }

        /// <summary>Creates a copy of the specified label and inserts it into the specified container item.</summary>
        /// <param name="template">The label to copy.</param>
        /// <param name="container">The container to insert the newly created label.</param>
        /// <param name="name">The new label's name.</param>
        /// <returns>
        /// A newly created <see cref="UILabel"/> whose properties are set to same values as in the <paramref name="template"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="template"/> or <paramref name="container"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="name"/> is null or an empty string.</exception>
        public static UILabel CreateCopy(UILabel template, UIComponent container, string name)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            if (template == null)
            {
                throw new ArgumentNullException(nameof(template));
            }

            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("The label name cannot be null or an emtpy string.", nameof(name));
            }

            UILabel result = container.AddUIComponent<UILabel>();
            result.autoSize = true;
            result.name = name;
            result.padding = template.padding;
            result.textColor = template.textColor;
            result.textScale = template.textScale;
            result.textScaleMode = template.textScaleMode;
            result.font = template.font;
            return result;
        }

        /// <summary>Creates a copy of the specified panel and inserts it into the specified container item.</summary>
        /// <param name="template">The label to copy.</param>
        /// <param name="container">The container to insert the newly created label.</param>
        /// <returns>
        /// A newly created <see cref="UIPanel"/> whose properties are set to same values as in the <paramref name="template"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="template"/> or <paramref name="container"/> is null.
        /// </exception>
        public static UIPanel CreateCopy(UIPanel template, UIComponent container)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            if (template == null)
            {
                throw new ArgumentNullException(nameof(template));
            }

            UIPanel result = container.AddUIComponent<UIPanel>();
            result.zOrder = template.zOrder;
            result.autoLayoutDirection = template.autoLayoutDirection;
            result.autoLayoutStart = template.autoLayoutStart;
            result.autoLayoutPadding = template.autoLayoutPadding;
            result.padding = template.padding;
            result.autoLayout = template.autoLayout;
            result.maximumSize = template.maximumSize;
            result.width = template.width;
            result.height = template.height;
            return result;
        }

        /// <summary>Shortens the text of the specified <paramref name="component"/> to fit the component's parent width.</summary>
        /// <param name="component">The component to process.</param>
        /// <exception cref="ArgumentNullException">Thrown when the argument is null.</exception>
        /// <remarks>This method is copied from the original game and is slightly modified.</remarks>
        public static void ShortenTextToFitParent(UITextComponent component)
        {
            if (component == null)
            {
                throw new ArgumentNullException(nameof(component));
            }

            if (component.parent == null)
            {
                return;
            }

            float parentWidth = component.parent.width;
            float targetWidth = parentWidth - component.relativePosition.x;
            if (component.width > targetWidth && component.text != null)
            {
                component.tooltip = component.text;
                string text = component.text;
                while (component.width > targetWidth && text.Length > 5)
                {
                    text = text.Remove(text.Length - 4).Trim() + "...";
                    component.text = text;
                }
            }
            else
            {
                component.tooltip = string.Empty;
            }
        }
    }
}