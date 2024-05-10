#if UNITY_EDITOR
using System.IO;
using System.Linq;
using CodeName.Modding.Mods;
using UnityEditor;
using UnityEngine;

namespace CodeName.Modding.Editor
{
    public static class ModUtility
    {
        public static bool TryGetResourceKey(this Object asset, out string key, out ModInfo mod)
        {
            if (!TryGetExpectedMod(asset, out mod))
            {
                key = null;

                return false;
            }

            mod.RawResources.Inverse.TryGetValue(asset, out key);

            return key != null;
        }

        public static bool TryGetExpectedMod(this Object asset, out ModInfo mod)
        {
            var folderPath = Path.GetDirectoryName(AssetDatabase.GetAssetPath(asset));
            if (string.IsNullOrEmpty(folderPath))
            {
                mod = null;

                return false;
            }

            var projectFolder = new DirectoryInfo(".");
            var assetsFolder = new DirectoryInfo("Assets");

            var currentDirectory = new DirectoryInfo(folderPath);
            while (currentDirectory != null && currentDirectory.Exists && currentDirectory.FullName.StartsWith(assetsFolder.FullName))
            {
                var filePaths = currentDirectory.GetFiles()
                    .Where(file => file.Extension == ".asset")
                    .Select(file => file.FullName)
                    .Select(path => Path.GetRelativePath(projectFolder.FullName, path))
                    .Distinct();

                foreach (var filePath in filePaths)
                {
                    mod = AssetDatabase.LoadAssetAtPath<ModInfo>(filePath);

                    if (mod != null)
                    {
                        return true;
                    }
                }

                currentDirectory = currentDirectory.Parent;
            }

            mod = null;

            return false;
        }
    }
}

#endif
