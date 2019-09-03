using System.IO;
using UnityEditor;
using UnityEngine;

namespace TrajectoryFollowing {

[CustomEditor(typeof(Trajectory))]
public class TrajectoryEditor : Editor
{
}

[InitializeOnLoad]
static class InstallGizmos {
    static InstallGizmos() {
        string srcDir = Application.dataPath + "/TrajectoryFollowing/Gizmos/TrajectoryFollowing";
        string destDir = Application.dataPath + "/Gizmos/TrajectoryFollowing";
        if (!Directory.Exists(destDir)) {
            Directory.CreateDirectory(destDir);
        }

        foreach (string gizmos in Directory.GetFiles(srcDir)) {
            string[] pathparts = gizmos.Split('/');
            string filename = pathparts[pathparts.Length - 1];
            if (filename.EndsWith(".png")) {
                File.Copy(gizmos, destDir + "/" + filename, true);
            }
        }
    }
}

}