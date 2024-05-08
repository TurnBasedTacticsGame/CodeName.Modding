#if UNITY_EDITOR
using CodeName.Modding.ImportedAssets;
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

                if (AssetDatabase.GetImporterOverride(path) == null)
                {
                    continue;
                }

                if (asset is IImportedAsset importedAsset)
                {
                    importedAsset.Save(path);
                }
            }

            ImportedAssetUtility.ClearDirty();

            return paths;
        }
    }
}
#endif
