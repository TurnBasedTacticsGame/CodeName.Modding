using Cysharp.Threading.Tasks;
using Exanite.SceneManagement;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CodeName.Modding.Loading
{
    public class LoadGameResourcesStage : SceneLoadStage
    {
        [Header("Dependencies")]
        [Required] [SerializeField] private GameInfo gameInfo;

        public GameInfo Info
        {
            get => gameInfo;
            set => gameInfo = value;
        }

        public override async UniTask Load(Scene currentScene)
        {
            await gameInfo.Load(new GameLoadContext()
            {
                IsEditor = false,
            });
        }
    }
}
