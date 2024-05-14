using UnityEngine;
#if UNITY_EDITOR
using CodeName.Modding.Editor;
#endif

namespace CodeName.Modding.Localization
{
    public static class LocalizationUtility
    {
        public static LocalizedString CreateLocalizedString(Object asset, string name)
        {
#if UNITY_EDITOR
            if (EditorModUtility.TryGetExpectedResourceKey(asset, out var editorResourceKey, out _))
            {
                return CreateLocalizedString(new ResourceKey(editorResourceKey), name);
            }
#endif

            if (GameResources.TryGetResourceKey(asset, out var runtimeResourceKey))
            {
                return CreateLocalizedString(new ResourceKey(runtimeResourceKey), name);
            }

            return default;
        }

        public static bool IsValidLocalizedValue(string localizedValue)
        {
            return !string.IsNullOrEmpty(localizedValue);
        }

        private static LocalizedString CreateLocalizedString(ResourceKey resourceKey, string name)
        {
            return new LocalizedString($"{resourceKey.ReplaceCsharpUnsafeCharacters()}_{name}");
        }
    }
}
