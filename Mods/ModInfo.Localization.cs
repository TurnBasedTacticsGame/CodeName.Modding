#if UNITY_EDITOR
using System.IO;
using CodeName.Modding.Editor;
using CodeName.Modding.Localization;
using Sirenix.OdinInspector;
using UnityEditor;

namespace CodeName.Modding.Mods
{
    public partial class ModInfo
    {
        private static string LocalizationTableFileName { get; } = "LocalizationTable";
        private static string LocalizationTableExtension { get; } = ".lang.csv";
        private static string LocalizationTableDefaultContent { get; } = "Key";

        [Button(DrawResult = false)]
        [BoxGroup(LocalizationGroup)]
        public LocalizationTableCollection CreateLocalizationTable()
        {
            // Create csv file
            var localizationTablePath = GenerateUniqueLocalizationTablePath();
            File.WriteAllText(localizationTablePath, LocalizationTableDefaultContent);

            // Import
            AssetDatabase.ImportAsset(localizationTablePath);
            AssetDatabase.SetImporterOverride<LocalizationTableCollectionImporter>(localizationTablePath);
            var collection = AssetDatabase.LoadAssetAtPath<LocalizationTableCollection>(localizationTablePath);

            // Update ModInfo
            localizationTableCollections.Add(collection);
            EditorUtility.SetDirty(this);

            return collection;
        }

        private string GenerateUniqueLocalizationTablePath()
        {
            var modPath = GetModPath();
            var tablePath = Path.Join(modPath, ContentFolderPath, $"{LocalizationTableFileName}{LocalizationTableExtension}");

            var suffix = 1;
            while (File.Exists(tablePath))
            {
                tablePath = Path.Join(modPath, ContentFolderPath, $"{LocalizationTableFileName}_{suffix}{LocalizationTableExtension}");
                suffix++;
            }

            return tablePath;
        }
    }
}
#endif
