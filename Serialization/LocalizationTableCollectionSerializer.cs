using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using CodeName.Modding.Localization;
using CsvHelper;
using UnityEngine;

namespace CodeName.Modding.Serialization
{
    public class LocalizationTableCollectionSerializer
    {
        private static string KeyHeaderName { get; } = "Key";
        private static Regex LocaleHeaderRegex { get; } = new(@"\((?<Code>.+)\)");

        public LocalizationTableCollection Read(string path)
        {
            var collection = ScriptableObject.CreateInstance<LocalizationTableCollection>();
            collection.Tables.Clear();

            using (var reader = new StreamReader(path))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Read();
                csv.ReadHeader();

                var header = csv.HeaderRecord;
                if (!IsValidHeader(header, out var error, out _, out var localeCodes))
                {
                    throw new FormatException($"Failed to read {path}: {error}");
                }

                var tables = localeCodes.Select(code =>
                    {
                        var table = new LocalizationTable(code);
                        collection.Tables.Add(table);

                        return table;
                    })
                    .ToList();

                while (csv.Read())
                {
                    var key = csv.GetField<string>(0);
                    if (string.IsNullOrWhiteSpace(key))
                    {
                        continue;
                    }

                    for (var i = 0; i < localeCodes.Length; i++)
                    {
                        var table = tables[i];
                        var value = csv.GetField<string>(i + 1);

                        table.RawEntries[key] = value;
                    }
                }
            }

            return collection;
        }

        /// <summary>
        /// Writes the <see cref="LocalizationTableCollection"/> to a path while preserving the order of existing entries.
        /// </summary>
        public void MergeWrite(string path, LocalizationTableCollection collection)
        {
            TableCsvMetadata metadata;
            try
            {
                metadata = ReadCsvMetadata(path);
            }
            catch
            {
                metadata = new TableCsvMetadata();
            }

            var keys = new HashSet<string>();
            foreach (var table in collection.Tables)
            {
                keys.UnionWith(table.RawEntries.Keys);
            }

            var localeCodes = new HashSet<string>();
            foreach (var table in collection.Tables)
            {
                localeCodes.Add(table.LocaleCode);
            }

            var tablesByLocaleCode = new Dictionary<string, LocalizationTable>();
            foreach (var table in collection.Tables)
            {
                tablesByLocaleCode[table.LocaleCode] = table;
            }

            // Use alphabetical order
            metadata.LocalizationKeys.Clear();
            metadata.LocalizationKeys.AddRange(keys);
            metadata.LocalizationKeys.Sort();

            // Keep existing in the same order, put new locales at the end
            metadata.LocaleHeaders.RemoveAll(header => !localeCodes.Remove(header.LocaleCode));
            metadata.LocaleHeaders.AddRange(localeCodes.Select(code => new TableCsvLocaleHeader($"Locale({code})", code)));

            using (var writer = new StreamWriter(path))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteField(KeyHeaderName);
                foreach (var header in metadata.LocaleHeaders)
                {
                    csv.WriteField(header.Name);
                }
                csv.NextRecord();

                foreach (var key in metadata.LocalizationKeys)
                {
                    csv.WriteField(key);
                    foreach (var header in metadata.LocaleHeaders)
                    {
                        var table = tablesByLocaleCode[header.LocaleCode];
                        if (table.RawEntries.TryGetValue(key, out var value))
                        {
                            csv.WriteField(value);
                        }
                        else
                        {
                            csv.WriteField(string.Empty);
                        }
                    }

                    csv.NextRecord();
                }
            }
        }

        private TableCsvMetadata ReadCsvMetadata(string path)
        {
            var metadata = new TableCsvMetadata();

            using (var reader = new StreamReader(path))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Read();
                csv.ReadHeader();

                var header = csv.HeaderRecord;
                if (!IsValidHeader(header, out var error, out var locales, out var localeCodes))
                {
                    throw new FormatException($"Failed to read {path}: {error}");
                }

                for (var i = 0; i < locales.Length; i++)
                {
                    metadata.LocaleHeaders.Add(new TableCsvLocaleHeader(locales[i], localeCodes[i]));
                }

                while (csv.Read())
                {
                    var key = csv.GetField<string>(0);
                    if (string.IsNullOrWhiteSpace(key))
                    {
                        continue;
                    }

                    metadata.LocalizationKeys.Add(key);
                }
            }

            return metadata;
        }

        private bool IsValidHeader(string[] header, out string error, out string[] locales, out string[] localeCodes)
        {
            error = null;
            locales = null;
            localeCodes = null;

            if (header.Length == 0)
            {
                error = "CSV header has too few entries";

                return false;
            }

            if (header[0].ToLowerInvariant() != KeyHeaderName.ToLowerInvariant())
            {
                error = $"First header must be named '{KeyHeaderName}'";

                return false;
            }

            localeCodes = new string[header.Length - 1];
            locales = new string[header.Length - 1];
            for (var i = 1; i < header.Length; i++)
            {
                if (!TryGetLocaleCodeFromHeader(header[i], out var localeCode))
                {
                    error = $"Failed to parse locale for header '{header[i]}'";

                    return false;
                }

                locales[i - 1] = header[i];
                localeCodes[i - 1] = localeCode;
            }

            return true;
        }

        private bool TryGetLocaleCodeFromHeader(string header, out string localeCode)
        {
            localeCode = null;

            var match = LocaleHeaderRegex.Match(header);
            if (!match.Success)
            {
                return false;
            }

            localeCode = match.Groups["Code"].Value;

            return true;
        }

        private class TableCsvLocaleHeader
        {
            public TableCsvLocaleHeader(string name, string localeCode)
            {
                Name = name;
                LocaleCode = localeCode;
            }

            public string Name { get; }
            public string LocaleCode { get; }
        }

        private class TableCsvMetadata
        {
            public List<TableCsvLocaleHeader> LocaleHeaders { get; } = new();
            public List<string> LocalizationKeys { get; } = new();
        }
    }
}
