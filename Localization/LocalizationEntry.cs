using System;
using UnityEngine;

namespace CodeName.Modding.Localization
{
    [Serializable]
    public class LocalizationEntry
    {
        [SerializeField] private string key;
        [SerializeField] private string value;

        public LocalizationEntry(string key, string value)
        {
            this.key = key;
            this.value = value;
        }

        public string Key
        {
            get => key;
            set => key = value;
        }

        public string Value
        {
            get => value;
            set => this.value = value;
        }
    }
}
