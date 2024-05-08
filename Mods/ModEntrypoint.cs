using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CodeName.Modding.Mods
{
    public abstract class ModEntrypoint : ScriptableObject
    {
        public abstract UniTask OnLoad(LoadedGameInfo gameInfo, LoadedModInfo modInfo);
    }
}
