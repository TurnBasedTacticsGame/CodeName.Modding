using System.Collections.Generic;
using Exanite.Core.OdinInspector;
using UnityEngine;

namespace CodeName.Modding.Localization
{
    public class LocalizationTableCollection : ScriptableObject
    {
        [Inline]
        [SerializeField] private List<LocalizationTable> tables = new()
        {
            // Add one table by default
            new LocalizationTable(Constants.DefaultLocaleCode),
        };

        public List<LocalizationTable> Tables => tables;
    }
}
