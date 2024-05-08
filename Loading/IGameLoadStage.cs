using Cysharp.Threading.Tasks;

namespace CodeName.Modding.Loading
{
    public interface IGameLoadStage
    {
        public UniTask Process(LoadedGameInfo gameInfo);
    }
}
