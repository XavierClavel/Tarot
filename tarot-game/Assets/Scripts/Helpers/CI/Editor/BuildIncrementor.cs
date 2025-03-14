using System;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class BuildIncrementor : IPreprocessBuildWithReport
{
    public int callbackOrder => 1;

    public void OnPreprocessBuild(BuildReport report)
    {
        BuildScriptableObject buildScriptableObject = ScriptableObject.CreateInstance<BuildScriptableObject>();

        switch(report.summary.platform)
        {
            case BuildTarget.StandaloneWindows64:
            case BuildTarget.StandaloneLinux64:
            case BuildTarget.StandaloneWindows:
            case BuildTarget.StandaloneOSX:
                PlayerSettings.macOS.buildNumber = IncrementBuildNumber(PlayerSettings.macOS.buildNumber);
                buildScriptableObject.buildNumber = PlayerSettings.macOS.buildNumber;
                break;
            case BuildTarget.iOS:
                PlayerSettings.iOS.buildNumber = IncrementBuildNumber(PlayerSettings.iOS.buildNumber);
                buildScriptableObject.buildNumber = PlayerSettings.iOS.buildNumber;
                break;
            case BuildTarget.Android:
                PlayerSettings.Android.bundleVersionCode++;
                buildScriptableObject.buildNumber = PlayerSettings.Android.bundleVersionCode.ToString();
                break;
            case BuildTarget.PS4:
                PlayerSettings.PS4.appVersion = IncrementBuildNumber(PlayerSettings.PS4.appVersion);
                buildScriptableObject.buildNumber = PlayerSettings.PS4.appVersion;
                break;
            case BuildTarget.XboxOne:
                PlayerSettings.XboxOne.Version = IncrementBuildNumber(PlayerSettings.XboxOne.Version);
                buildScriptableObject.buildNumber = PlayerSettings.XboxOne.Version;
                break;
            case BuildTarget.WSAPlayer:
                PlayerSettings.WSA.packageVersion = new System.Version(PlayerSettings.WSA.packageVersion.Major, PlayerSettings.WSA.packageVersion.Minor, PlayerSettings.WSA.packageVersion.Build + 1);
                buildScriptableObject.buildNumber = PlayerSettings.WSA.packageVersion.Build.ToString();
                break;
            case BuildTarget.tvOS:
                PlayerSettings.tvOS.buildNumber = IncrementBuildNumber(PlayerSettings.tvOS.buildNumber);
                buildScriptableObject.buildNumber = PlayerSettings.tvOS.buildNumber;
                break;
        }
        
        buildScriptableObject.gitCommit = GitCommitUtility.retrieveCurrentCommitShorthash();
        buildScriptableObject.date = $"{DateTime.Today:dd/MM/yyyy} {DateTime.Now:HH:mm}" ;

        AssetDatabase.DeleteAsset("Assets/Resources/Build.asset"); // delete any old build.asset
        AssetDatabase.CreateAsset(buildScriptableObject, "Assets/Resources/Build.asset"); // create the new one with correct build number before build starts
        AssetDatabase.SaveAssets();
    }

    private string IncrementBuildNumber(string buildNumber)
    {
        int.TryParse(buildNumber, out int outputBuildNumber);

        return (outputBuildNumber + 1).ToString();
    }
}