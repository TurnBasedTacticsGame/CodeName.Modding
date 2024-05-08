using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace CodeName.Modding.Localization
{
    [Serializable]
    public class LocalizationTable : ISerializationCallbackReceiver
    {
        [SerializeField] private string localeCode;

        [HideInInspector]
        [SerializeField] private List<LocalizationEntry> serializedEntries = new();

        public LocalizationTable() {}

        public LocalizationTable(string localeCode)
        {
            this.localeCode = localeCode;
        }

        public string LocaleCode
        {
            get => localeCode;
            set => localeCode = value;
        }

        [LabelText("Entries")]
        [Searchable]
        [ShowInInspector]
        public Dictionary<string, string> RawEntries { get; private set; } = new();

        public bool TryGetLocalizedValue(string key, out string localizedValue)
        {
            if (RawEntries.TryGetValue(key, out localizedValue) && IsValidLocalizedValue(localizedValue))
            {
                return true;
            }

            localizedValue = string.Empty;

            return false;
        }

        public bool IsValidLocalizedValue(string localizedValue)
        {
            return !string.IsNullOrEmpty(localizedValue);
        }

        public void OnBeforeSerialize()
        {
            serializedEntries.Clear();
            foreach (var (key, value) in RawEntries)
            {
                serializedEntries.Add(new LocalizationEntry(key, value));
            }
        }

        public void OnAfterDeserialize()
        {
            RawEntries.Clear();
            foreach (var entry in serializedEntries)
            {
                RawEntries[entry.Key] = entry.Value;
            }
        }
    }
}
