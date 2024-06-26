using System.Collections.Generic;
using System.IO;
using System.Linq;
using CodegenCS;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine.Assertions;
using Object = UnityEngine.Object;
using PropertyDefinition = Exanite.Core.Properties.PropertyDefinition;

namespace CodeName.Modding.Mods
{
    public partial class ModInfo
    {
#if UNITY_EDITOR
        [BoxGroup(CodegenGroup)]
        [Button]
        public void UpdateResourceList()
        {
            var searchPaths = new List<string>();
            var contentFolderPath = Path.Join(GetModPath(), ContentFolderName);
            var overridesFolderPath = Path.Join(GetModPath(), OverridesFolderName);
            if (Directory.Exists(contentFolderPath))
            {
                searchPaths.Add(contentFolderPath);
            }

            if (Directory.Exists(overridesFolderPath))
            {
                searchPaths.Add(overridesFolderPath);
            }

            var assetGuids = AssetDatabase.FindAssets("", searchPaths.ToArray());
            var assetInfos = new List<AssetInfo>();
            foreach (var assetGuid in assetGuids)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(assetGuid);
                var asset = AssetDatabase.LoadMainAssetAtPath(assetPath);
                assetInfos.Add(new AssetInfo(asset, assetPath));

                foreach (var subAsset in AssetDatabase.LoadAllAssetRepresentationsAtPath(assetPath))
                {
                    assetInfos.Add(new AssetInfo(subAsset, assetPath, subAsset.name));
                }
            }

            RawResources.Clear();
            foreach (var assetInfo in assetInfos)
            {
                switch (assetInfo.Asset)
                {
                    case DefaultAsset:
                    case MonoScript:
                    {
                        break;
                    }
                    default:
                    {
                        RawResources.Add(ConvertToModAssetKey(assetInfo), assetInfo.Asset);

                        break;
                    }
                }
            }

            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        [BoxGroup(CodegenGroup)]
        [Button]
        public void UpdateResourceListAndGenerateCode()
        {
            UpdateResourceList();
            GenerateClass();
        }

        [BoxGroup(CodegenGroup)]
        private void GenerateClass()
        {
            using (var writer = new CodegenTextWriter(Path.Combine(GetModPath(), $"{Id}.Generated.cs")))
            {
                var overrides = new Dictionary<string, List<ModResource>>();

                writer.WriteLine("// <auto-generated/>");
                writer.WriteLine($"// Auto-generated by {GetType().FullName}");

                using (writer.WithCBlock($"namespace {GeneratedNamespace}"))
                using (writer.WithCBlock($"public static partial class {Id}"))
                {
                    writer.WriteLine($"public static string ModId {{ get; }} = \"{Id}\";");
                    writer.WriteLine();

                    using (writer.WithCBlock("public static partial class Content"))
                    {
                        foreach (var resource in RawResources.Select(kvp => new ModResource(kvp.Key, kvp.Value)).OrderBy(res => res).ToList())
                        {
                            var modId = new ResourceKey(resource.Key).GetModId();
                            if (modId != Id)
                            {
                                if (!overrides.TryGetValue(modId, out var overridenResources))
                                {
                                    overridenResources = new List<ModResource>();

                                    overrides.Add(modId, overridenResources);
                                }

                                overridenResources.Add(resource);

                                continue;
                            }

                            WriteResourceKeyProperty(writer, resource, modId);
                        }
                    }

                    writer.WriteLine();
                    writer.WriteLine();
                    using (writer.WithCBlock("public static partial class Overrides"))
                    {
                        var isFirst = true;

                        foreach (var (modId, overridenResources) in overrides)
                        {
                            if (!isFirst)
                            {
                                writer.WriteLine();
                                writer.WriteLine();
                            }

                            isFirst = false;

                            using (writer.WithCBlock($"public static partial class {modId}"))
                            {
                                foreach (var overridenResource in overridenResources)
                                {
                                    WriteResourceKeyProperty(writer, overridenResource, modId);
                                }
                            }
                        }
                    }
                }

                writer.WriteLine();
            }

            AssetDatabase.Refresh();
        }

        private void WriteResourceKeyProperty(CodegenTextWriter writer, ModResource resource, string modId)
        {
            var propertyName = resource.Key.Substring(modId.Length);
            propertyName = ResourceKey.Regexes.ReplaceCsharpUnsafeCharacters.Replace(propertyName, "_").Trim('_');

            writer.WriteLine($"public static global::{typeof(PropertyDefinition)}<global::{resource.Asset.GetType()}> {propertyName} {{ get; }} = new(\"{resource.Key}\");");
        }

        private ResourceKey ConvertToModAssetKey(AssetInfo assetInfo)
        {
            var assetPath = Path.GetRelativePath(GetModPath(), assetInfo.Path);

            var segments = assetPath.Split(Path.DirectorySeparatorChar);
            Assert.IsTrue(segments.Length >= 2, $"{assetPath} must be in the {ContentFolderName} folder or {OverridesFolderName} folder");

            // Remove entire file extension, including cases where a file has "multiple" extensions.
            // Eg: "file.ext1.ext2"'" should become "file"
            var assetName = segments[segments.Length - 1];
            var firstPeriodIndex = assetName.IndexOf('.');
            if (firstPeriodIndex >= 0)
            {
                segments[segments.Length - 1] = assetName.Substring(0, firstPeriodIndex);
            }

            var modId = Id;
            var assetPathStartIndex = 1;
            if (segments[0] == OverridesFolderName)
            {
                modId = segments[1];
                assetPathStartIndex++;

                Assert.IsTrue(segments.Length >= 3, $"{assetPath} is an override. It must be in a subfolder of the {OverridesFolderName} folder. The name of the subfolder will be used as the Mod ID.");
            }

            var assetResourcePath = string.Empty;
            for (var i = assetPathStartIndex; i < segments.Length; i++)
            {
                if (i != assetPathStartIndex)
                {
                    assetResourcePath += "/";
                }

                assetResourcePath += segments[i];
            }

            var resourceKey = $"{modId}:{assetResourcePath}";
            if (assetInfo.SubAssetName != null)
            {
                resourceKey += $"[{assetInfo.SubAssetName}]";
            }

            return new ResourceKey(resourceKey);
        }

        private class AssetInfo
        {
            public Object Asset { get; }
            public string Path { get; }
            public string SubAssetName { get; }

            public AssetInfo(Object asset, string path, string subAssetName = null)
            {
                Asset = asset;
                Path = path;
                SubAssetName = subAssetName;
            }
        }
#endif
    }
}
