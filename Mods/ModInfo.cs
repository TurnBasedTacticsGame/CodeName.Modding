using System.Collections.Generic;
using CodeName.Modding.Localization;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace CodeName.Modding.Mods
{
    public partial class ModInfo : ScriptableObject
    {
        public static string ContentFolderPath = "Content";
        public static string OverridesFolderPath = "Overrides";

        [SerializeField] private string displayName = "Mod Name";
        [PropertySpace(SpaceBefore = 0, SpaceAfter = 16)]
        [SerializeField] private string id = "ModName";

        [SerializeField] private ModEntrypoint entrypoint;

        [BoxGroup(LocalizationGroup)]
        [FormerlySerializedAs("localizationTables")]
        [SerializeField] private List<LocalizationTableCollection> localizationTableCollections = new();

        [BoxGroup(CodegenGroup)]
        [SerializeField] private string contentFolderPath = "Content";

        [BoxGroup(CodegenGroup)]
        [SerializeField] private string generatedNamespace = "ModName";

        [BoxGroup(CodegenGroup)]
        [Searchable]
        [ListDrawerSettings(ShowFoldout = false, DraggableItems = false, HideAddButton = true, HideRemoveButton = true)]
        [SerializeField] private List<ModResource> resources = new();

        public string DisplayName => displayName;
        public string Id => id;

        public string GeneratedNamespace => generatedNamespace;

        public List<ModResource> Resources => resources;

        public LocalizationTableCollection MainLocalizationTableCollection => localizationTableCollections.Count > 0 ? localizationTableCollections[0] : null;
        public List<LocalizationTableCollection> LocalizationTableCollections => localizationTableCollections;

        public ModEntrypoint Entrypoint
        {
            get => entrypoint;
            set => entrypoint = value;
        }
    }
}
