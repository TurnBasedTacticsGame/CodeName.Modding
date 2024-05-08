#if UNITY_EDITOR
using System.Linq;
using CodeName.Modding.Loading;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine.Assertions;
using Progress = UnityEditor.Progress;

namespace CodeName.Modding.Editor
{
    internal class UnityEntrypoints : AssetPostprocessor
    {
        private static string LoadingGameResources { get; } = "Loading game resources";
        private static string UnloadingGameResources { get; } = "Unloading game resources";

        public static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths, bool didDomainReload)
        {
            if (!didDomainReload)
            {
                return;
            }

            InitializeGameResources();

            EditorApplication.playModeStateChanged += static state =>
            {
                switch (state)
                {
                    case PlayModeStateChange.EnteredEditMode:
                    {
                        InitializeGameResources();

                        break;
                    }
                    case PlayModeStateChange.ExitingEditMode:
                    {
                        ClearGameResources();

                        break;
                    }
                }
            };
        }

        /// <summary>
        /// Used to initialize GameResources at Editor time so Editor tooling can access it.
        /// </summary>
        private static void InitializeGameResources()
        {
            var progressId = Progress.Start(LoadingGameResources);
            FindGameInfo()
                .Load(new GameLoadContext()
                {
                    IsEditor = true,
                })
                .ContinueWith(() =>
                {
                    Progress.Finish(progressId);
                })
                .Forget();
        }

        /// <summary>
        /// Used to clear GameResources before entering play mode to ensure consistent behaviour.
        /// </summary>
        private static void ClearGameResources()
        {
            var progressId = Progress.Start(UnloadingGameResources);
            GameResources.Unload()
                .ContinueWith(() =>
                {
                    Progress.Finish(progressId);
                })
                .Forget();
        }

        private static GameInfo FindGameInfo()
        {
            var gameInfoGuids = AssetDatabase.FindAssets($"t:{nameof(GameInfo)}");
            var gameInfos = gameInfoGuids.Select(guid => AssetDatabase.GUIDToAssetPath(guid)).Select(path => AssetDatabase.LoadAssetAtPath<GameInfo>(path)).ToList();

            Assert.AreEqual(1, gameInfos.Count);

            var gameInfo = gameInfos[0];

            return gameInfo;
        }
    }
}

#endif
