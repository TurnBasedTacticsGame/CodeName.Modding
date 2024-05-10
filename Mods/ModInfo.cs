using System;
using System.Collections.Generic;
using CodeName.Modding.Localization;
using Exanite.Core.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

namespace CodeName.Modding.Mods
{
    public partial class ModInfo : ScriptableObject, ISerializationCallbackReceiver
    {
        public static string ContentFolderName = "Content";
        public static string OverridesFolderName = "Overrides";

        [SerializeField] private string displayName = "Mod Name";
        [PropertySpace(SpaceBefore = 0, SpaceAfter = 16)]
        [SerializeField] private string id = "ModName";

        [SerializeField] private ModEntrypoint entrypoint;

        [BoxGroup(LocalizationGroup)]
        [SerializeField] private List<LocalizationTableCollection> localizationTableCollections = new();

        [BoxGroup(CodegenGroup)]
        [SerializeField] private string generatedNamespace = "ModName";

        [FormerlySerializedAs("resources")]
        [BoxGroup(CodegenGroup)]
        [Searchable]
        [ListDrawerSettings(ShowFoldout = false, DraggableItems = false, HideAddButton = true, HideRemoveButton = true)]
        [SerializeField] private List<ModResource> serializedResources = new();

        public string DisplayName => displayName;
        public string Id => id;

        public string GeneratedNamespace => generatedNamespace;

        public TwoWayDictionary<string, Object> RawResources { get; } = new();

        public LocalizationTableCollection MainLocalizationTableCollection => localizationTableCollections.Count > 0 ? localizationTableCollections[0] : null;
        public List<LocalizationTableCollection> LocalizationTableCollections => localizationTableCollections;

        public ModEntrypoint Entrypoint
        {
            get => entrypoint;
            set => entrypoint = value;
        }

        public void OnBeforeSerialize()
        {
            serializedResources.Clear();
            foreach (var (key, value) in RawResources)
            {
                serializedResources.Add(new ModResource(key, value));
            }
            serializedResources.Sort();
        }

        public void OnAfterDeserialize()
        {
            RawResources.Clear();
            foreach (var entry in serializedResources)
            {
                RawResources[entry.Key] = entry.Asset;
            }
        }

        [Serializable]
        private struct ModResource : IComparable<ModResource>, IEquatable<ModResource>
        {
            [ReadOnly]
            [SerializeField] private string key;

            [ReadOnly]
            [SerializeField] private Object asset;

            public ModResource(string key, Object asset)
            {
                this.key = key;
                this.asset = asset;
            }

            public string Key => key;
            public Object Asset => asset;

            public int CompareTo(ModResource other)
            {
                return string.Compare(key, other.key, StringComparison.Ordinal);
            }

            public bool Equals(ModResource other)
            {
                return key == other.key && Equals(asset, other.asset);
            }

            public override bool Equals(object obj)
            {
                return obj is ModResource other && Equals(other);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(key, asset);
            }

            public static bool operator ==(ModResource left, ModResource right)
            {
                return left.Equals(right);
            }

            public static bool operator !=(ModResource left, ModResource right)
            {
                return !left.Equals(right);
            }
        }
    }
}
