using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace Hierarchy2
{
    internal class HierarchyResources : ScriptableObject
    {
        Dictionary<string, Texture2D> dicIcons = new Dictionary<string, Texture2D>();
        public List<Texture2D> listIcons = new List<Texture2D>();

        public void GenerateKeyForAssets()
        {
            dicIcons.Clear();
            dicIcons = listIcons.ToDictionary(texture2D => texture2D.name);
        }

        public Texture2D GetIcon(string key)
        {
            Texture2D texture2D = null;
            var getResult = dicIcons.TryGetValue(key, out texture2D);
            if (getResult == false)
                Debug.Log(string.Format("Icon with {0} not found, return null.", key));
            return texture2D;
        }

        internal static HierarchyResources GetAssets()
        {
            var guids = AssetDatabase.FindAssets(string.Format("t:{0}", typeof(HierarchyResources).Name));

            if (guids.Length > 0)
            {
                var asset = AssetDatabase.LoadAssetAtPath<HierarchyResources>(AssetDatabase.GUIDToAssetPath(guids[0]));
                if (asset != null)
                    return asset;
            }

            return null;
        }

        internal static HierarchyResources CreateAssets()
        {
            String path = EditorUtility.SaveFilePanelInProject("Save as...", "Resources", "asset", "");
            if (path.Length > 0)
            {
                HierarchyResources settings = ScriptableObject.CreateInstance<HierarchyResources>();
                AssetDatabase.CreateAsset(settings, path);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                EditorUtility.FocusProjectWindow();
                Selection.activeObject = settings;
                return settings;
            }

            return null;
        }
    }

    [CustomEditor(typeof(HierarchyResources))]
    internal class ResourcesInspector : Editor
    {
        HierarchyResources resources;

        void OnEnable() => resources = target as HierarchyResources;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            if (GUILayout.Button("Generate Key For Assets"))
                resources.GenerateKeyForAssets();
        }
    }
}