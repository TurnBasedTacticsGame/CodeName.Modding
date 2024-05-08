using System.Collections.Generic;
using CodeName.Modding.Localization;
using Cysharp.Threading.Tasks;
using Exanite.Core.Collections;
using Exanite.Core.Properties;
using UnityEngine;

namespace CodeName.Modding
{
    public static class GameResources
    {
        private static LoadedGameInfo GameInfo { get; set; }

        private static TwoWayDictionary<string, Object> Resources => GameInfo.Resources;

        public static IReadOnlyDictionary<string, Object> ResourcesByKey => GameInfo.Resources;
        public static Dictionary<string, LocalizationTable> LocalizationTables => GameInfo.LocalizationTables;

        public static Object GetResource(string key)
        {
            return Resources[key];
        }

        public static T GetResource<T>(PropertyDefinition<T> resourceDefinition) where T : Object
        {
            return (T)GetResource(resourceDefinition.Key);
        }

        public static bool TryGetResource(string key, out Object resource)
        {
            return Resources.TryGetValue(key, out resource);
        }

        public static string GetResourceKey(Object resource)
        {
            TryGetResourceKey(resource, out var key);

            return key;
        }

        public static bool TryGetResourceKey(Object resource, out string key)
        {
            return Resources.Inverse.TryGetValue(resource, out key);
        }

        public static void Initialize(LoadedGameInfo gameInfo)
        {
            GameInfo = gameInfo;
        }

        public static async UniTask Unload()
        {
            if (GameInfo == null)
            {
                return;
            }

            await GameInfo.OriginalInfo.Unload(GameInfo);
            GameInfo = null;
        }
    }
}
