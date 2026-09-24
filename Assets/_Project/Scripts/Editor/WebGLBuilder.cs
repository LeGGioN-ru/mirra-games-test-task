using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.WebGL;
using UnityEngine;

namespace ClockApp.Editor
{
    public static class WebGLBuilder
    {
        private const string OutputPath = "Builds/WebGL";
        private const string TemplateName = "PROJECT:Clock";

        [MenuItem("Build/WebGL")]
        public static void Build()
        {
            PlayerSettings.WebGL.template = TemplateName;
            UserBuildSettings.codeOptimization = WasmCodeOptimization.DiskSizeLTO;

            var options = new BuildPlayerOptions
            {
                scenes = EditorBuildSettings.scenes.Where(scene => scene.enabled).Select(scene => scene.path).ToArray(),
                locationPathName = OutputPath,
                target = BuildTarget.WebGL,
                targetGroup = BuildTargetGroup.WebGL,
                options = BuildOptions.None
            };

            var summary = BuildPipeline.BuildPlayer(options).summary;
            var message = $"WebGL build {summary.result}: {summary.totalSize / (1024f * 1024f):F1} MB in {summary.totalTime:mm\\:ss}.";

            if (summary.result == BuildResult.Succeeded)
            {
                Debug.Log(message);
            }
            else
            {
                Debug.LogError(message);
            }
        }
    }
}
