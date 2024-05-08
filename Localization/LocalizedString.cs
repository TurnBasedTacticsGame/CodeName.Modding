using System;
using UnityEngine;

namespace CodeName.Modding.Localization
{
    [Serializable]
    public class LocalizedString
    {
        public static string MissingKeyMessage { get; } = "MISSING_KEY";

        [SerializeField] private string key;

        public LocalizedString(string key)
        {
            this.key = key;
        }

        public string Key
        {
            get => key;
            set => key = value;
        }

        public bool IsEmpty => string.IsNullOrEmpty(key);

        public string GetLocalizedValue()
        {
            if (IsEmpty)
            {
                return $"\\{{{MissingKeyMessage}\\}}";
            }

            // Todo Allow selection of different locales
            if (GameResources.LocalizationTables.TryGetValue(Constants.DefaultLocaleCode, out var table) && table.TryGetLocalizedValue(key, out var localizedValue))
            {
                return localizedValue;
            }

            return $"\\{{{key}\\}}";
        }
    }
}
