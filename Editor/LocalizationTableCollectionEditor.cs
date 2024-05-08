#if UNITY_EDITOR
using CodeName.Modding.Localization;
using Exanite.Core.Utilities;
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
            if (target.GetOverrideImporter() != typeof(LocalizationTableCollectionImporter))
            {
                Draw();

                return;
            }

            var assetPath = AssetDatabase.GetAssetPath(target);
            if (string.IsNullOrEmpty(assetPath))
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
