using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Trajectory))]
public class TrajectoryEditor : Editor
{
    protected virtual void OnSceneGUI()
    {
        Trajectory trajectoryComponent = (Trajectory)target;
        List<Vector3> trajectory = trajectoryComponent.trajectory;

        for (int i = 0; i < trajectory.Count; ++i) {
            EditorGUI.BeginChangeCheck();
            Vector3 target = Handles.PositionHandle(trajectory[i], Quaternion.identity);
            if (EditorGUI.EndChangeCheck()) {
                Undo.RecordObject(trajectoryComponent, "Change trajectory.");
                trajectory[i] = target;
                // trajectoryComponent.Update();
            }
        }

    }
}