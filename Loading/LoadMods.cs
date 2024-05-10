using CodeName.Modding.Localization;
using CodeName.Modding.Mods;
using Cysharp.Threading.Tasks;

namespace CodeName.Modding.Loading
{
    public class LoadMods : IGameLoadStage
    {
        public UniTask Process(LoadedGameInfo gameInfo)
        {
            foreach (var mod in gameInfo.OriginalInfo.Mods)
            {
                var loadedMod = RegisterMod(gameInfo, mod);

                AddModResources(gameInfo, loadedMod);
                AddModLocalizationTables(gameInfo, loadedMod);
            }

            return UniTask.CompletedTask;
        }

        private LoadedModInfo RegisterMod(LoadedGameInfo gameInfo, ModInfo mod)
        {
            var loadedMod = new LoadedModInfo(mod);
            gameInfo.Mods.Add(loadedMod);

            return loadedMod;
        }

        private void AddModResources(LoadedGameInfo gameInfo, LoadedModInfo mod)
        {
            foreach (var (resourceKey, asset) in mod.OriginalInfo.RawResources)
            {
                gameInfo.Resources[resourceKey] = asset;
            }
        }

        private void AddModLocalizationTables(LoadedGameInfo gameInfo, LoadedModInfo mod)
        {
            foreach (var tableCollection in mod.OriginalInfo.LocalizationTableCollections)
            {
                foreach (var table in tableCollection.Tables)
                {
                    if (!gameInfo.LocalizationTables.TryGetValue(table.LocaleCode, out var loadedTable))
                    {
                        loadedTable = new LocalizationTable(table.LocaleCode);
                        gameInfo.LocalizationTables[table.LocaleCode] = loadedTable;
                    }

                    foreach (var (key, value) in table.RawEntries)
                    {
                        if (!loadedTable.IsValidLocalizedValue(value))
                        {
                            continue;
                        }

                        loadedTable.RawEntries[key] = value;
                    }
                }
            }
        }
    }
}
