#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CodeName.Modding.Editor
{
    public static class ImportedAssetUtility
    {
        private static readonly HashSet<Object> BackingDirtyImportedAssets = new();
        public static IReadOnlyCollection<Object> DirtyImportedAssets => BackingDirtyImportedAssets;

        public static void SetDirty(Object importedAsset)
        {
            BackingDirtyImportedAssets.Add(importedAsset);
            EditorUtility.SetDirty(importedAsset);
        }

        public static void ClearDirty()
        {
            BackingDirtyImportedAssets.Clear();
        }
    }
}
#endif
