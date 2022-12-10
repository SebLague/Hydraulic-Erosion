using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEditor;

namespace Hierarchy2
{
    public class SceneRenamePopup : EditorWindow
    {
        static EditorWindow window;
        public Scene scene;
        TextField nameField;

        [System.Obsolete("Use ShowPopup(Scene) instead.")]
        new public static SceneRenamePopup ShowPopup() => null;

        public static SceneRenamePopup ShowPopup(Scene scene)
        {
            if (window == null)
                window = ScriptableObject.CreateInstance<SceneRenamePopup>();

            Vector2 v2 = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
            window.position = new Rect(v2.x, v2.y, 200, 50);
            window.ShowPopup();
            window.Focus();

            SceneRenamePopup sceneRenamePopup = window as SceneRenamePopup;
            sceneRenamePopup.scene = scene;
            sceneRenamePopup.nameField.value = scene.name;
            sceneRenamePopup.nameField.Query("unity-text-input").First().Focus();

            return sceneRenamePopup;
        }

        public void OnLostFocus() => Close();

        void OnEnable()
        {
            rootVisualElement.StyleBorderWidth(1);
            Color c = new Color32(58, 121, 187, 255);
            rootVisualElement.StyleBorderColor(c);
            rootVisualElement.StyleJustifyContent(Justify.Center);

            nameField = new TextField();
            nameField.RegisterCallback<KeyUpEvent>((evt) =>
            {
                if (evt.keyCode == KeyCode.Return) Apply();
            });
            rootVisualElement.Add(nameField);

            Button apply = new Button(() => { Apply(); });

            apply.text = "Apply";
            rootVisualElement.Add(apply);
        }

        void Apply()
        {
            AssetDatabase.RenameAsset(scene.path, nameField.value);
            rootVisualElement.StyleDisplay(DisplayStyle.None);
            nameField.value = "";
            Close();
        }
    }
}