#if UNITY_EDITOR
using System;
using UnityEditor;

namespace CodeName.Modding.Editor
{
    public class ImportedAssetModificationProcessor : AssetModificationProcessor
    {
        private static string[] OnWillSaveAssets(string[] paths)
        {
            foreach (var asset in ImportedAssetUtility.DirtyImportedAssets)
            {
                var path = AssetDatabase.GetAssetPath(asset);
                if (string.IsNullOrEmpty(path))
                {
                    continue;
                }

                var importerType = AssetDatabase.GetImporterType(path);
                if (typeof(ISaveableImporter).IsAssignableFrom(importerType))
                {
                    var importer = (ISaveableImporter)Activator.CreateInstance(importerType);
                    importer.Save(asset, path);
                }
            }

            ImportedAssetUtility.ClearDirty();

            return paths;
        }
    }
}
#endif
