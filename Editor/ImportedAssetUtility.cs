#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CodeName.Modding.Editor
{
    public static class ImportedAssetUtility
    {
        private static readonly HashSet<Object> InternalDirtyImportedAssets = new();
        public static IReadOnlyCollection<Object> DirtyImportedAssets => InternalDirtyImportedAssets;

        public static void SetDirty(Object importedAsset)
        {
            InternalDirtyImportedAssets.Add(importedAsset);
            EditorUtility.SetDirty(importedAsset);
        }

        public static void ClearDirty()
        {
            InternalDirtyImportedAssets.Clear();
        }
    }
}
#endif
