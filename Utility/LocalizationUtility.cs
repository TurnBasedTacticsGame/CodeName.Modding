using CodeName.Modding.Editor;
using CodeName.Modding.Localization;
using UnityEngine;

namespace CodeName.Modding.Utility
{
    public static class LocalizationUtility
    {
        public static LocalizedString CreateLocalizedString(Object asset, string name)
        {
            if (!ModUtility.TryGetResourceKey(asset, out var key, out _))
            {
                return default;
            }

            return new LocalizedString($"{new ResourceKey(key).ReplaceCsharpUnsafeCharacters()}_{name}");
        }

        public static bool IsValidLocalizedValue(string localizedValue)
        {
            return !string.IsNullOrEmpty(localizedValue);
        }
    }
}
