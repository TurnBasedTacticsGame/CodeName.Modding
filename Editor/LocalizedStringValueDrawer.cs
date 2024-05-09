#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using CodeName.Modding.Localization;
using CodeName.Modding.Mods;
using CodeName.Modding.Utility;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace CodeName.Modding.Editor
{
    public class LocalizedStringValueDrawer : OdinValueDrawer<LocalizedString>
    {
        public static string AssetNotPartOfModMessage { get; } = "Asset is not part of a mod";

        /// <summary>
        /// Tracks the selected locale code by ModInfo.
        /// </summary>
        private static string SelectedLocaleCode { get; set; }

        /// <summary>
        /// Tracks if the foldout is open by Property.Path.
        /// </summary>
        private static Dictionary<string, bool> IsFoldoutOpen { get; } = new();

        private void Initialize(out Object asset, out LocalizedString localizedString, out ModInfo mod, out LocalizationTableCollection collection)
        {
            base.Initialize();

            asset = (Object)Property.SerializationRoot.ValueEntry.WeakSmartValue;
            localizedString = (LocalizedString)Property.ValueEntry.WeakSmartValue;
            IsFoldoutOpen.TryAdd(Property.Path, false);

            if (!asset.TryGetExpectedMod(out mod))
            {
                collection = null;
                return;
            }

            collection = mod.MainLocalizationTableCollection;
        }

        protected override void DrawPropertyLayout(GUIContent label)
        {
            Initialize(out var asset, out var localizedString, out var mod, out var collection);

            using (new GUILayout.HorizontalScope())
            {
                if (localizedString.IsEmpty)
                {
                    EditorGUILayout.PrefixLabel(label);
                    IsFoldoutOpen[Property.Path] = false;
                }
                else
                {
                    IsFoldoutOpen[Property.Path] = EditorGUI.Foldout(GetPrefixLabelRect(), IsFoldoutOpen[Property.Path], label);
                }

                Property.RecordForUndo("Edit localization key");

                if (mod != null)
                {
                    localizedString.Key = EditorGUILayout.TextField(localizedString.Key);
                }
                else
                {
                    SirenixEditorGUI.WarningMessageBox(AssetNotPartOfModMessage);
                }

                DrawCreateEntryButtonIfNeeded(asset);
            }

            if (IsFoldoutOpen[Property.Path])
            {
                using (new EditorGUI.IndentLevelScope(2))
                {
                    if (collection != null)
                    {
                        using (new GUILayout.HorizontalScope())
                        {
                            var table = DrawLocaleCodeDropdown(mod, collection);
                            DrawLocalizationEntryTextField(collection, table, localizedString.Key);
                        }
                    }
                    else
                    {
                        SirenixEditorGUI.WarningMessageBox($"Mod does not have a {typeof(LocalizationTableCollection).Name}");
                    }
                }
            }
        }

        private void DrawCreateEntryButtonIfNeeded(Object asset)
        {
            var attribute = Property.GetAttribute<CreateLocalizationEntryButtonAttribute>();
            if (attribute == null)
            {
                return;
            }

            if (SirenixEditorGUI.IconButton(EditorIcons.Refresh, tooltip: "Generate Localization Entry"))
            {
                var localizedString = LocalizationUtility.CreateLocalizedString(asset, attribute.Name);
                if (!asset.TryGetResourceKey(out _, out var mod))
                {
                    return;
                }

                var collection = mod.MainLocalizationTableCollection;
                if (collection == null)
                {
                    collection = mod.CreateLocalizationTable();
                }

                foreach (var table in collection.Tables)
                {
                    table.RawEntries.TryAdd(localizedString.Key, string.Empty);
                }

                Property.RecordForUndo("Generate localization key");
                Property.ValueEntry.WeakSmartValue = localizedString;
            }
        }

        /// <summary>
        /// Draws the locale code dropdown and handles error cases by drawing a disabled dropdown.
        /// </summary>
        /// <param name="mod">The mod the property's asset is in. Should have been retrieved during <see cref="Initialize">Initialize()</see>.</param>
        /// <param name="collection">Can be null.</param>
        /// <returns>The <see cref="LocalizationTable"/> corresponding to the selected locale code.</returns>
        private LocalizationTable DrawLocaleCodeDropdown(ModInfo mod, LocalizationTableCollection collection)
        {
            LocalizationTable DrawDisabledDropdown()
            {
                var originalIsEnabled = GUI.enabled;
                GUI.enabled = false;

                SirenixEditorFields.Dropdown(GetPrefixLabelRect(), GUIContent.none, null, new List<string>());
                SelectedLocaleCode = null;

                GUI.enabled = originalIsEnabled;

                return null;
            }

            if (collection == null)
            {
                return DrawDisabledDropdown();
            }
            else
            {
                var possibleLocales = collection.Tables.Select(t => t.LocaleCode).ToList();
                if (SelectedLocaleCode == null || !possibleLocales.Contains(SelectedLocaleCode))
                {
                    SelectedLocaleCode = possibleLocales.FirstOrDefault();
                    if (SelectedLocaleCode == null)
                    {
                        return DrawDisabledDropdown();
                    }
                }

                var options = collection.Tables.Select(t => t.LocaleCode).ToList();
                SelectedLocaleCode = SirenixEditorFields.Dropdown(GetPrefixLabelRect(), GUIContent.none, SelectedLocaleCode, options);
                var table = collection.Tables.Find(t => t.LocaleCode == SelectedLocaleCode);

                return table;
            }
        }

        /// <summary>
        /// Draws the localization entry text field and handles error cases by drawing a disabled text field.
        /// </summary>
        /// <param name="collection">Can be null.</param>
        /// <param name="table">Can be null.</param>
        /// <param name="localizationKey">The localization key of the entry.</param>
        private void DrawLocalizationEntryTextField(LocalizationTableCollection collection, LocalizationTable table, string localizationKey)
        {
            var originalIndentLevel = EditorGUI.indentLevel;
            var originalIsEditable = GUI.enabled;

            var isEditable = collection != null && table != null;

            GUI.enabled = isEditable;
            EditorGUI.indentLevel = 0;
            {
                if (isEditable)
                {
                    table.TryGetLocalizedValue(localizationKey, out var currentValue);

                    var newValue = SirenixEditorFields.TextField(GUIContent.none, currentValue);
                    if (currentValue != newValue)
                    {
                        Undo.RecordObject(collection, "Edit localization entry");
                        table.RawEntries[localizationKey] = newValue;
                        ImportedAssetUtility.SetDirty(collection);
                    }
                }
                else
                {
                    SirenixEditorFields.TextField(GUIContent.none, "");
                }
            }
            EditorGUI.indentLevel = originalIndentLevel;
            GUI.enabled = originalIsEditable;
        }

        private Rect GetPrefixLabelRect()
        {
            // Adapted code from EditorGUILayout.PrefixLabel
            var buttonStyle = (GUIStyle)"Button";

            return GUILayoutUtility.GetRect(EditorGUIUtility.labelWidth - buttonStyle.margin.left, 18f, buttonStyle, GUILayout.ExpandWidth(false));
        }
    }
}
#endif
