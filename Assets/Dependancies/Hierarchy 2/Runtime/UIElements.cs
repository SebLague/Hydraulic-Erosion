using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;

#endif

namespace Hierarchy2
{
#if UNITY_EDITOR
    public class Foldout : VisualElement
    {
        public Image imageElement;
        public HorizontalLayout headerElement;
        public Label labelElement;
        public VerticalLayout contentElement;

        Image foloutImage;

        Texture onIcon = EditorGUIUtility.IconContent("IN foldout on@2x").image;
        Texture offIcon = EditorGUIUtility.IconContent("IN foldout@2x").image;

        bool value;

        public bool Value
        {
            get { return value; }

            set
            {
                this.value = value;
                contentElement.StyleDisplay(this.value);
                foloutImage.image = this.value ? onIcon : offIcon;
            }
        }

        public string Title
        {
            get { return labelElement.text; }
            set { labelElement.text = value; }
        }

        public Foldout() => Init("");

        public Foldout(string title) => Init(title);

        private void Init(string title)
        {
            this.StyleFont(FontStyle.Normal);
            this.StyleMinHeight(20);
            this.StyleBorderWidth(0, 0, 1, 0);
            Color borderColor = EditorGUIUtility.isProSkin
                ? new Color32(35, 35, 35, 255)
                : new Color32(153, 153, 153, 255);
            this.StyleBorderColor(borderColor);

            headerElement = new HorizontalLayout();
            headerElement.StyleHeight(21);
            headerElement.StyleMaxHeight(21);
            headerElement.StyleMinHeight(21);
            headerElement.StylePadding(4, 0, 0, 0);
            headerElement.StyleAlignItem(Align.Center);
            Color backgroundColor = EditorGUIUtility.isProSkin
                ? new Color32(80, 80, 80, 255)
                : new Color32(222, 222, 222, 255);
            headerElement.StyleBackgroundColor(backgroundColor);
            Color hoverBorderColor = new Color32(58, 121, 187, 255);
            headerElement.RegisterCallback<MouseEnterEvent>((evt) =>
            {
                headerElement.StyleBorderWidth(1);
                headerElement.StyleBorderColor(hoverBorderColor);
            });
            headerElement.RegisterCallback<MouseLeaveEvent>((evt) =>
            {
                headerElement.StyleBorderWidth(0);
                headerElement.StyleBorderColor(Color.clear);
            });
            base.Add(headerElement);

            contentElement = new VerticalLayout();
            contentElement.StyleDisplay(value);
            base.Add(contentElement);

            labelElement = new Label();
            labelElement.text = title;
            headerElement.Add(labelElement);

            imageElement = new Image();
            imageElement.name = nameof(imageElement);
            imageElement.StyleMargin(0, 4, 0, 0);
            imageElement.StyleSize(16, 16);
            headerElement.Add(imageElement);
            imageElement.SendToBack();
            imageElement.RegisterCallback<GeometryChangedEvent>((evt) => { imageElement.StyleDisplay(imageElement.image == null ? DisplayStyle.None : DisplayStyle.Flex); });

            foloutImage = new Image();
            foloutImage.StyleWidth(13);
            foloutImage.StyleMargin(0, 2, 0, 0);
            foloutImage.scaleMode = ScaleMode.ScaleToFit;
            foloutImage.image = value ? onIcon : offIcon;
            if (!EditorGUIUtility.isProSkin)
                foloutImage.tintColor = Color.grey;
            headerElement.Add(foloutImage);
            foloutImage.SendToBack();


            headerElement.RegisterCallback<MouseUpEvent>((evt) =>
            {
                if (evt.button == 0)
                {
                    Value = !Value;
                    evt.StopPropagation();
                }
            });
        }

        new public void Add(VisualElement visualElement)
        {
            contentElement.Add(visualElement);
        }
    }

    public class EditorHelpBox : VisualElement
    {
        public string Label
        {
            get { return label; }
            set { label = value; }
        }

        private string label = "";

        public EditorHelpBox(string text, MessageType messageType, bool wide = true)
        {
            style.marginLeft = style.marginRight = style.marginTop = style.marginBottom = 4;
            Label = text;

            IMGUIContainer iMGUIContainer = new IMGUIContainer(() => { EditorGUILayout.HelpBox(label, messageType, wide); });

            iMGUIContainer.name = nameof(IMGUIContainer);
            Add(iMGUIContainer);
        }
    }

#endif

    public class HorizontalLayout : VisualElement
    {
        public HorizontalLayout()
        {
            name = nameof(HorizontalLayout);
            this.StyleFlexDirection(FlexDirection.Row);
            this.StyleFlexGrow(1);
        }
    }

    public class VerticalLayout : VisualElement
    {
        public VerticalLayout()
        {
            name = nameof(VerticalLayout);
            this.StyleFlexDirection(FlexDirection.Column);
            this.StyleFlexGrow(1);
        }
    }
}