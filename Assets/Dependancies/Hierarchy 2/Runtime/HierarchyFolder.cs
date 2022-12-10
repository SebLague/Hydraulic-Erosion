using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hierarchy2
{
    [AddComponentMenu("Hierarchy 2/Hierarchy Folder", 0)]
    public class HierarchyFolder : MonoBehaviour
    {
        public enum FlattenMode
        {
            None = 0,
            Editor = 1,
            Runtime = 2,
            All = 3
        }

        public enum FlattenSpace
        {
            Parent = 0,
            World = 1
        }

        public FlattenMode flattenMode = FlattenMode.None;

        public FlattenSpace flattenSpace = FlattenSpace.Parent;

        public bool destroyAfterFlatten = true;

        private void OnEnable()
        {
            Flatten();
        }

        public void Flatten()
        {
            if (flattenMode == FlattenMode.None)
                return;

            if (flattenMode != FlattenMode.All)
            {
                if (flattenMode == FlattenMode.Editor && !Application.isEditor)
                    return;

                if (flattenMode == FlattenMode.Runtime && Application.isEditor)
                    return;
            }

            var parent = flattenSpace == FlattenSpace.World ? null : transform.parent;
            var childCount = transform.childCount;
            var parentOrderIndex = flattenSpace == FlattenSpace.World ? transform.root.GetSiblingIndex() : transform.GetSiblingIndex();

            while (childCount-- > 0)
            {
                var child = transform.GetChild(0);
                child.SetParent(parent);
                child.SetSiblingIndex(++parentOrderIndex);
            }

            if (destroyAfterFlatten)
                Destroy(gameObject);
        }
    }
}