using System.Text;
using System.Text.RegularExpressions;

namespace CodeName.Modding
{
    /// <summary>
    /// Currently used to centralize code related to resource key parsing.
    /// </summary>
    public readonly struct ResourceKey
    {
        private readonly string internalKey;

        public ResourceKey(string key)
        {
            internalKey = key;
        }

        public string GetModId()
        {
            var firstColonIndex = internalKey.IndexOf(':');
            return internalKey.Substring(0, firstColonIndex);
        }

        public string GetResourcePath()
        {
            var firstColonIndex = internalKey.IndexOf(':');
            return internalKey.Substring(firstColonIndex + 1);
        }

        public ResourceKey ReplaceCsharpUnsafeCharacters(char replacement = '_')
        {
            return new ResourceKey(Regexes.ReplaceCsharpUnsafeCharacters.Replace(internalKey, replacement.ToString()).Trim(replacement));
        }

        public override string ToString()
        {
            return internalKey;
        }

        public static implicit operator string(ResourceKey value)
        {
            return value.internalKey;
        }

        /// <summary>
        /// Creates a key in the format: "ModId:ResourceName".
        /// <para/>
        /// Note: It is recommended to use PascalCase and only alphanumeric characters.
        /// </summary>
        public static ResourceKey Create(string modId, string resourceName)
        {
            return new ResourceKey($"{modId}:{resourceName}");
        }

        /// <summary>
        /// Creates a key in the format: "ModId:Path/To/Resource".
        /// <para/>
        /// Note: It is recommended to use PascalCase and only alphanumeric characters.
        /// Path should not have a leading or trailing slash.
        /// </summary>
        public static ResourceKey Create(string modId, params string[] pathSegments)
        {
            var builder = new StringBuilder();
            builder.Append(modId);
            builder.Append(":");

            for (var i = 0; i < pathSegments.Length; i++)
            {
                if (i != 0)
                {
                    builder.Append("/");
                }

                builder.Append(pathSegments[i]);
            }

            return new ResourceKey(builder.ToString());
        }

        public static class Regexes
        {
            public static Regex ReplaceCsharpUnsafeCharacters { get; } = new(@"[^A-Za-z0-9_]+");
        }
    }
}
