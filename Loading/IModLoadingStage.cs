using CodeName.Modding.Mods;
using Cysharp.Threading.Tasks;

namespace CodeName.Modding.Loading
{
    public interface IModLoadingStage
    {
        /// <summary>
        /// Does the <see cref="Process"/> method complete asynchronously?
        /// <para/>
        /// Asynchronous means that the <see cref="Process"/> does not block until completion.
        /// This distinction is important in environments that do not support async loading.
        /// </summary>
        public bool IsAsync { get; }

        public UniTask Process(LoadedGameInfo gameInfo, LoadedModInfo modInfo);
    }
}
