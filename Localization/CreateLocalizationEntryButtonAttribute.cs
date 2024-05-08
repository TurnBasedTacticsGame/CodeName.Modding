using System;

namespace CodeName.Modding.Localization
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class CreateLocalizationEntryButtonAttribute : Attribute
    {
        public CreateLocalizationEntryButtonAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}
