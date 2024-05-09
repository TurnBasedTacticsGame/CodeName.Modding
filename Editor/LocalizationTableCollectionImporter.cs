#if UNITY_EDITOR
using UnityEditor.AssetImporters;

namespace CodeName.Modding.Editor
{
    [ScriptedImporter(1, new[] { "langcsv" })]
    public class LocalizationTableCollectionImporter : ScriptedImporter
    {
        private readonly LocalizationTableCollectionSerializer serializer = new();

        public override void OnImportAsset(AssetImportContext context)
        {
            var collection = serializer.Read(context.assetPath);
            context.AddObjectToAsset("Main", collection);
            context.SetMainObject(collection);
        }
    }
}

#endif
