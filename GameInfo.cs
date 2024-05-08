using System.Collections.Generic;
using CodeName.Modding.Loading;
using CodeName.Modding.Mods;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CodeName.Modding
{
    public class GameInfo : ScriptableObject
    {
        [SerializeField] private List<ModInfo> mods = new();

        private readonly List<IGameLoadStage> stages = new List<IGameLoadStage>()
        {
            new LoadMods(),
        };

        public List<ModInfo> Mods
        {
            get => mods;
            set => mods = value;
        }

        public async UniTask Load(GameLoadContext context)
        {
            await GameResources.Unload();
            var gameInfo = new LoadedGameInfo(this, context);
            GameResources.Initialize(gameInfo);

            foreach (var stage in stages)
            {
                await stage.Process(gameInfo);
            }
        }

        public async UniTask Unload(LoadedGameInfo gameInfo)
        {
            for (var i = gameInfo.OnUnloadTasks.Count - 1; i >= 0; i--)
            {
                var task = gameInfo.OnUnloadTasks[i];
                await task.Invoke();
            }

            await Resources.UnloadUnusedAssets();
        }
    }
}
