#if UNITY_EDITOR
using CodeName.Modding.Localization;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace CodeName.Modding.Editor
{
    [CustomEditor(typeof(LocalizationTableCollection))]
    public class LocalizationTableCollectionEditor : OdinEditor
    {
        public override void OnInspectorGUI()
        {
            var assetPath = AssetDatabase.GetAssetPath(target);
            if (string.IsNullOrEmpty(assetPath) || AssetDatabase.GetImporterType(assetPath) != typeof(LocalizationTableCollectionImporter))
            {
                Draw();

                return;
            }

            GUI.enabled = true;
            ImportedAssetUtility.SetDirty(target);

            Draw();
        }

        private void Draw()
        {
            Tree.Draw();
        }
    }
}
#endif
