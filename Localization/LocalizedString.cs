using System;
using UnityEngine;

namespace CodeName.Modding.Localization
{
    [Serializable]
    public struct LocalizedString
    {
        public static string MissingKeyMessage { get; } = "MISSING_KEY";

        [SerializeField] private string key;

        public LocalizedString(string key)
        {
            this.key = key;
        }

        public string Key => key;

        public bool IsEmpty => string.IsNullOrEmpty(key);

        public string GetLocalizedValue()
        {
            if (IsEmpty)
            {
                return $"\\{{{MissingKeyMessage}\\}}";
            }

            if (GameResources.LocalizationTables.TryGetValue(GameResources.LocaleCode, out var tables))
            {
                for (var i = tables.Count - 1; i >= 0; i--)
                {
                    var table = tables[i];
                    if (table.TryGetLocalizedValue(key, out var localizedValue))
                    {
                        return localizedValue;
                    }
                }
            }

            return $"\\{{{key}\\}}";
        }
    }
}
