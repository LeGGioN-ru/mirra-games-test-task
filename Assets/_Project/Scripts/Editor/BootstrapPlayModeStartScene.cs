using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace ClockApp.Editor
{
    [InitializeOnLoad]
    internal static class BootstrapPlayModeStartScene
    {
        static BootstrapPlayModeStartScene()
        {
            EditorApplication.delayCall += Apply;
            EditorBuildSettings.sceneListChanged += Apply;
        }

        private static void Apply()
        {
            var firstScene = EditorBuildSettings.scenes.FirstOrDefault(scene => scene.enabled);

            EditorSceneManager.playModeStartScene = firstScene == null
                ? null
                : AssetDatabase.LoadAssetAtPath<SceneAsset>(firstScene.path);
        }
    }
}
