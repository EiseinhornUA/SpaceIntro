using UnityEditor;
using UnityEditor.iOS.Xcode;
using System.IO;
using UnityEditor.Callbacks;

public class PostProcess
{
    [PostProcessBuild]
    public static void OnPostProcessBuild(BuildTarget target, string pathToBuiltProject)
    {
#if UNITY_IOS
        // Path to Info.plist
        var plistPath = Path.Combine(pathToBuiltProject, "Info.plist");

        // Read the existing plist
        var plist = new PlistDocument();
        plist.ReadFromFile(plistPath);

        // Update the value for ITSAppUsesNonExemptEncryption
        PlistElementDict rootDict = plist.root;
        rootDict.SetBoolean("ITSAppUsesNonExemptEncryption", false);

        // Write the updated plist back
        plist.WriteToFile(plistPath);
#endif //UNITY_IOS
    }
}
