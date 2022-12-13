using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.UIElements;
using UnityEngine;

namespace Hierarchy2
{
    public sealed class SelectionsRenamePopup : EditorWindow
    {
        static EditorWindow window;
        TextField textField;
        EnumField enumModeField;
        EditorHelpBox helpBox;

        enum Mode
        {
            None,
            Number,
            NumberReverse
        }


        new public static SelectionsRenamePopup ShowPopup()
        {
            if (window == null)
                window = ScriptableObject.CreateInstance<SelectionsRenamePopup>();

            Vector2 v2 = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
            window.position = new Rect(v2.x, v2.y, 200, 70);
            window.ShowPopup();
            window.Focus();

            SelectionsRenamePopup selectionsRenamePopup = window as SelectionsRenamePopup;
            selectionsRenamePopup.textField.Query("unity-text-input").First().Focus();

            return selectionsRenamePopup;
        }

        public void OnLostFocus() => Close();

        void OnEnable()
        {
            rootVisualElement.StyleBorderWidth(1);
            Color c = new Color32(58, 121, 187, 255);
            rootVisualElement.StyleBorderColor(c);
            rootVisualElement.StyleJustifyContent(Justify.Center);

            textField = new TextField();
            textField.value = "New Name...";
            rootVisualElement.Add(textField);
            textField.RegisterCallback<KeyUpEvent>((evt) =>
            {
                if (evt.keyCode == KeyCode.Return) Apply();
            });

            enumModeField = new EnumField(new Mode());
            enumModeField.label = "Mode";
            enumModeField.tooltip = "Rename with prefix.";
            enumModeField.labelElement.StyleMinWidth(64);
            enumModeField.labelElement.StyleMaxWidth(64);

            rootVisualElement.Add(enumModeField);

            helpBox = new EditorHelpBox("This mode require selections with the same parent.", MessageType.Info);
            helpBox.StyleDisplay(false);
            rootVisualElement.Add(helpBox);

            enumModeField.RegisterValueChangedCallback((evt) => { OnModeChanged(evt.newValue); });

            Button apply = new Button(Apply);
            apply.text = nameof(Apply);
            rootVisualElement.Add(apply);
        }

        void OnModeChanged(Enum mode)
        {
            Rect rect = window.position;
            rect.height = 70;

            if (!System.Enum.Equals(mode, Mode.None))
            {
                rect.height = 70;
                if (!IsSelectionsSameParent())
                {
                    helpBox.StyleDisplay(true);
                    rect.height += 44;
                }
                else
                {
                    helpBox.StyleDisplay(false);
                }
            }
            else
            {
                rect.height = 70;
                helpBox.StyleDisplay(false);
            }

            window.position = rect;
        }

        bool IsSelectionsSameParent()
        {
            var parent = Selection.activeGameObject.transform.parent;
            foreach (var gameObject in Selection.gameObjects)
            {
                if (parent != gameObject.transform.parent)
                    return false;
            }

            return true;
        }

        void Apply()
        {
            bool sameParent = IsSelectionsSameParent();

            List<GameObject> sortedSelections;

            int index = 0;

            if (System.Enum.Equals(enumModeField.value, Mode.NumberReverse))
            {
                sortedSelections = Selection.gameObjects.ToList()
                    .OrderByDescending(gameObject => gameObject.transform.GetSiblingIndex()).ToList();
            }
            else
            {
                sortedSelections = Selection.gameObjects.ToList()
                    .OrderBy(gameObject => gameObject.transform.GetSiblingIndex()).ToList();
            }

            foreach (GameObject gameObject in sortedSelections)
            {
                if (gameObject != null)
                {
                    Undo.RegisterCompleteObjectUndo(gameObject, "Selections Renaming...");

                    if (!System.Enum.Equals(enumModeField.value, Mode.None) && sameParent)
                        gameObject.name = string.Format("{0} ({1})", textField.value, index++);
                    else
                        gameObject.name = textField.value;
                }
            }

            rootVisualElement.StyleDisplay(DisplayStyle.None);

            Close();
        }
    }
}