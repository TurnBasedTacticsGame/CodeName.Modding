using System.Collections.Generic;
using CodeName.Modding.ImportedAssets;
using Exanite.Core.OdinInspector;
using UnityEngine;
#if UNITY_EDITOR
using CodeName.Modding.Editor;
#endif

namespace CodeName.Modding.Localization
{
    public class LocalizationTableCollection : ScriptableObject, IImportedAsset
    {
        [Inline]
        [SerializeField] private List<LocalizationTable> tables = new()
        {
            // Add one table by default
            new LocalizationTable(Constants.DefaultLocaleCode),
        };

        public List<LocalizationTable> Tables => tables;

#if UNITY_EDITOR
        public void Save(string path)
        {
            var serializer = new LocalizationTableCollectionSerializer();
            serializer.MergeWrite(path, this);
        }
#endif
    }
}
