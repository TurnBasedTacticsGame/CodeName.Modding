using System.IO;
using UnityEditor;

namespace CodeName.Modding.Mods
{
    public partial class ModInfo
    {
        private const string CodegenGroup = "Codegen";
        private const string LocalizationGroup = "Localization";

#if UNITY_EDITOR
        private string GetModPath()
        {
            var modInfoAssetPath = AssetDatabase.GetAssetPath(this);
            var modPath = Path.GetDirectoryName(modInfoAssetPath);

            return modPath;
        }
#endif
    }
}
