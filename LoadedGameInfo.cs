using System;
using System.Collections.Generic;
using CodeName.Modding.Loading;
using CodeName.Modding.Localization;
using CodeName.Modding.Mods;
using Cysharp.Threading.Tasks;
using Exanite.Core.Collections;
using Object = UnityEngine.Object;

namespace CodeName.Modding
{
    public class LoadedGameInfo
    {
        public LoadedGameInfo(GameInfo originalInfo, GameLoadContext loadContext)
        {
            OriginalInfo = originalInfo;
            LoadContext = loadContext;
        }

        public GameInfo OriginalInfo { get; }
        public GameLoadContext LoadContext { get; }

        public List<LoadedModInfo> Mods { get; } = new();

        public TwoWayDictionary<string, Object> Resources { get; } = new();
        public Dictionary<string, LocalizationTable> LocalizationTables { get; } = new();

        public List<Func<UniTask>> OnUnloadTasks { get; } = new();
    }
}
