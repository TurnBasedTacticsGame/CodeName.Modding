#if UNITY_EDITOR
using CodeName.Modding.Localization;
using CodeName.Modding.Serialization;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace CodeName.Modding.Editor
{
    [ScriptedImporter(1, new[] { "langcsv" })]
    public class LocalizationTableCollectionImporter : ScriptedImporter, ISaveableImporter
    {
        private readonly LocalizationTableCollectionSerializer serializer = new();

        public override void OnImportAsset(AssetImportContext context)
        {
            var collection = serializer.Read(context.assetPath);
            context.AddObjectToAsset("Main", collection);
            context.SetMainObject(collection);
        }

        public void Save(Object asset, string path)
        {
            var serializer = new LocalizationTableCollectionSerializer();
            serializer.MergeWrite(path, (LocalizationTableCollection)asset);
        }
    }
}

#endif
