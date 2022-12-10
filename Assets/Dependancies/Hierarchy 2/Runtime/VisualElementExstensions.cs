using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace Hierarchy2
{
    public static class UIElementsExstensions
    {
        public static void StyleDisplay(this VisualElement ui, DisplayStyle displayStyle) =>
            ui.style.display = displayStyle;

        public static void StyleDisplay(this VisualElement ui, bool value) =>
            ui.StyleDisplay(value ? DisplayStyle.Flex : DisplayStyle.None);

        public static bool IsDisplaying(this VisualElement ui) => ui.style.display == DisplayStyle.Flex;

        public static void StyleVisibility(this VisualElement ui, Visibility visibility) =>
            ui.style.visibility = visibility;

        public static void StyleVisibility(this VisualElement ui, bool value) =>
            ui.StyleVisibility(value ? Visibility.Visible : Visibility.Hidden);

        public static Vector2 StylePosition(this VisualElement ui)
        {
            return new Vector2(ui.style.left.value.value, ui.style.top.value.value);
        }

        public static Vector2 StyleSize(this VisualElement ui)
        {
            return new Vector2(ui.style.width.value.value, ui.style.height.value.value);
        }

        public static Vector2 StyleMinSize(this VisualElement ui)
        {
            return new Vector2(ui.style.minWidth.value.value, ui.style.minHeight.value.value);
        }

        public static Vector2 StyleMaxSize(this VisualElement ui)
        {
            return new Vector2(ui.style.maxWidth.value.value, ui.style.maxHeight.value.value);
        }

        public static void StylePosition(this VisualElement ui, Vector2 position)
        {
            ui.StyleLeft(position.x);
            ui.StyleTop(position.y);
        }

        public static void StylePosition(this VisualElement ui, StyleLength x, StyleLength y)
        {
            ui.StyleLeft(x);
            ui.StyleTop(y);
        }

        public static void StyleTop(this VisualElement ui, StyleLength value) => ui.style.top = value;

        public static void StyleBottom(this VisualElement ui, StyleLength value) => ui.style.bottom = value;

        public static void StyleLeft(this VisualElement ui, StyleLength value) => ui.style.left = value;

        public static void StyleRight(this VisualElement ui, StyleLength value) => ui.style.right = value;

        public static float StyleTop(this VisualElement ui) => ui.style.top.value.value;

        public static float StyleBottom(this VisualElement ui) => ui.style.bottom.value.value;

        public static float StyleLeft(this VisualElement ui) => ui.style.left.value.value;

        public static float StyleRight(this VisualElement ui) => ui.style.right.value.value;

        public static void StylePosition(this VisualElement ui, Position type) => ui.style.position = type;

        public static void StyleSize(this VisualElement ui, StyleLength width, StyleLength height)
        {
            ui.StyleWidth(width);
            ui.StyleHeight(height);
        }

        public static void StyleSize(this VisualElement ui, Vector2 size) => StyleSize(ui, size.x, size.y);

        public static void StyleMinSize(this VisualElement ui, StyleLength width, StyleLength height)
        {
            ui.StyleMinWidth(width);
            ui.StyleMinHeight(height);
        }

        public static void StyleMaxSize(this VisualElement ui, StyleLength width, StyleLength height)
        {
            ui.StyleMaxWidth(width);
            ui.StyleMaxHeight(height);
        }


        public static void StyleWidth(this VisualElement ui, StyleLength width) => ui.style.width = width;

        public static void StyleMinWidth(this VisualElement ui, StyleLength width) => ui.style.minWidth = width;

        public static void StyleMaxWidth(this VisualElement ui, StyleLength width) => ui.style.maxWidth = width;

        public static void StyleHeight(this VisualElement ui, StyleLength height) => ui.style.height = height;

        public static void StyleMinHeight(this VisualElement ui, StyleLength height) => ui.style.minHeight = height;

        public static void StyleMaxHeight(this VisualElement ui, StyleLength height) => ui.style.maxHeight = height;

        public static void StyleFont(this VisualElement ui, FontStyle fontStyle) =>
            ui.style.unityFontStyleAndWeight = fontStyle;

        public static void StyleFontSize(this VisualElement ui, StyleLength size) => ui.style.fontSize = size;

        public static void StyleTextAlign(this VisualElement ui, TextAnchor textAnchor) =>
            ui.style.unityTextAlign = textAnchor;

        public static void StyleAlignSelf(this VisualElement ui, Align align) => ui.style.alignSelf = align;

        public static void StyleAlignItem(this VisualElement ui, Align align) => ui.style.alignItems = align;

        public static void StyleJustifyContent(this VisualElement ui, Justify justify) =>
            ui.style.justifyContent = justify;

        public static void StyleFlexDirection(this VisualElement ui, FlexDirection flexDirection) =>
            ui.style.flexDirection = flexDirection;

        public static void StyleMargin(this VisualElement ui, StyleLength value) =>
            ui.StyleMargin(value, value, value, value);

        public static void StyleMargin(this VisualElement ui, StyleLength left, StyleLength right, StyleLength top,
            StyleLength bottom)
        {
            ui.style.marginLeft = left;
            ui.style.marginRight = right;
            ui.style.marginTop = top;
            ui.style.marginBottom = bottom;
        }

        public static void StyleMarginLeft(this VisualElement ui, StyleLength value) => ui.style.marginLeft = value;

        public static void StyleMarginRight(this VisualElement ui, StyleLength value) => ui.style.marginRight = value;

        public static void StyleMarginTop(this VisualElement ui, StyleLength value) => ui.style.marginTop = value;

        public static void StyleMarginBottom(this VisualElement ui, StyleLength value) => ui.style.marginBottom = value;

        public static void StylePadding(this VisualElement ui, StyleLength value) =>
            ui.StylePadding(value, value, value, value);

        public static void StylePadding(this VisualElement ui, StyleLength left, StyleLength right, StyleLength top,
            StyleLength bottom)
        {
            ui.style.paddingLeft = left;
            ui.style.paddingRight = right;
            ui.style.paddingTop = top;
            ui.style.paddingBottom = bottom;
        }

        public static void StylePaddingLeft(this VisualElement ui, StyleLength value) => ui.style.paddingLeft = value;

        public static void StylePaddingRight(this VisualElement ui, StyleLength value) => ui.style.paddingRight = value;

        public static void StylePaddingTop(this VisualElement ui, StyleLength value) => ui.style.paddingTop = value;

        public static void StylePaddingBottom(this VisualElement ui, StyleLength value) =>
            ui.style.paddingBottom = value;

        public static void StyleBorderRadius(this VisualElement ui, StyleLength radius) =>
            ui.StyleBorderRadius(radius, radius, radius, radius);

        public static void StyleBorderRadius(this VisualElement ui, StyleLength topLeft, StyleLength topRight,
            StyleLength bottomLeft, StyleLength bottomRight)
        {
            ui.style.borderTopLeftRadius = topLeft;
            ui.style.borderTopRightRadius = topRight;
            ui.style.borderBottomLeftRadius = bottomLeft;
            ui.style.borderBottomRightRadius = bottomRight;
        }

        public static void StyleBorderWidth(this VisualElement ui, StyleFloat width) =>
            ui.StyleBorderWidth(width, width, width, width);

        public static void StyleBorderWidth(this VisualElement ui, StyleFloat left, StyleFloat right, StyleFloat top,
            StyleFloat bottom)
        {
            ui.style.borderLeftWidth = left;
            ui.style.borderRightWidth = right;
            ui.style.borderTopWidth = top;
            ui.style.borderBottomWidth = bottom;
        }

        public static void StyleBorderColor(this VisualElement ui, StyleColor color) =>
            ui.StyleBorderColor(color, color, color, color);

        public static void StyleBorderColor(this VisualElement ui, StyleColor left, StyleColor right, StyleColor top,
            StyleColor bottom)
        {
            ui.style.borderLeftColor = left;
            ui.style.borderRightColor = right;
            ui.style.borderTopColor = top;
            ui.style.borderBottomColor = bottom;
        }

        public static void StyleFlexBasisAsPercent(this VisualElement ui, StyleLength basis) =>
            ui.style.flexBasis = basis;

        public static void StyleFlexGrow(this VisualElement ui, StyleFloat grow) => ui.style.flexGrow = grow;

        public static void StyleBackgroundColor(this VisualElement ui, StyleColor color) =>
            ui.style.backgroundColor = color;

        public static void StyleTextColor(this VisualElement ui, StyleColor color) => ui.style.color = color;

        public static VisualElement FindChildren(this VisualElement ui, string name)
        {
            return ui.Children().ToList().Find(childElement => childElement.name == name);
        }

        public static T FindChildren<T>(this VisualElement ui, string name) where T : VisualElement
        {
            return ui.FindChildren(name) as T;
        }

        public static VisualElement FindChildrenPhysicalHierarchy(this VisualElement ui, string name)
        {
            return ui.hierarchy.Children().ToList().Find(childElement => childElement.name == name);
        }

        public static VisualElement FindChildrenPhysicalHierarchy<T>(this VisualElement ui, string name)
            where T : VisualElement
        {
            return ui.FindChildrenPhysicalHierarchy(name) as T;
        }
    }
}