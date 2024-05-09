using CodeName.Modding.Editor;
using CodeName.Modding.Localization;
using UnityEngine;

namespace CodeName.Modding.Utility
{
    public static class LocalizationUtility
    {
        public static string AssetNotPartOfModMessage { get; } = "Asset is not part of a mod";

        public static LocalizedString CreateLocalizedString(Object asset, string name)
        {
            if (!asset.TryGetResourceKey(out var key, out var mod))
            {
                return default;
            }

            // var collection = mod.MainLocalizationTableCollection;
            // if (collection == null)
            // {
            //     collection = mod.CreateLocalizationTable();
            // }

            var localizationKey = $"{new ResourceKey(key).ReplaceCsharpUnsafeCharacters()}_{name}";
            // foreach (var table in collection.Tables)
            // {
            //     table.RawEntries.TryAdd(localizationKey, string.Empty);
            // }

            return new LocalizedString(localizationKey);
        }
    }
}
