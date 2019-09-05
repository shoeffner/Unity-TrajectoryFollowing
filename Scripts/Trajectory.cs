using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

namespace TrajectoryFollowing {

[System.Serializable, ExecuteAlways]
public class Trajectory : MonoBehaviour
{
    public enum InterpolationMethod {
        Linear,
        HermiteSpline
    }

    [Tooltip("The trajectory points. Always includes this GameObject's transform and optionally a goal transform.")]
    public List<GameObject> trajectory = new List<GameObject>();

    private List<Vector3> m_trajectory;

    [Tooltip("The trajectory goal - if empty, the last point of the trajectory is the goal.")]
    public GameObject goal;

    [Tooltip("The interpolation method to use.")]
    public InterpolationMethod interpolationMethod = InterpolationMethod.HermiteSpline;

    public void Awake() {
        m_trajectory = new List<Vector3>();
    }

    public void Start() {
        if (trajectory.Count == 0) {
            Transform trajectoryTransform = gameObject.transform.Find($"{gameObject.name}Trajectory");
            if (trajectoryTransform == null) {
                GameObject trajectoryGameObject = new GameObject($"{gameObject.name}Trajectory");
                SceneManager.MoveGameObjectToScene(trajectoryGameObject, gameObject.scene);
                trajectoryTransform = trajectoryGameObject.transform;
                trajectoryTransform.SetPositionAndRotation(gameObject.transform.position, Quaternion.identity);
            }

            for (int i = 1; i <= 4; ++i) {
                GameObject prefab = (GameObject) PrefabUtility.InstantiatePrefab(Resources.Load<GameObject>("Prefabs/TrajectoryPoint"), trajectoryTransform);
                prefab.name = $"Trajectory {i}";
                prefab.transform.SetPositionAndRotation(trajectoryTransform.position + new Vector3(0, 0, i), Quaternion.identity);
                trajectory.Add(prefab);
            }
        }
        UpdatePoints();
    }

    public bool IsBehind(float offset) {
        return (int) Math.Truncate((decimal) offset) > m_trajectory.Count - 2;
    }

    public bool IsBefore(float offset) {
        return offset < 0;
    }

    public Vector3 GetStart() {
        return GetAt(0);
    }

    public Vector3 GetEnd() {
        return GetAt(m_trajectory.Count - 1);
    }

    public Vector3 GetAt(float offset) {
        int section = (int) Math.Truncate((decimal) offset);
        return GetAt(section, offset - section);
    }

    public Vector3 GetAt(int section, float offset) {
        if (m_trajectory.Count == 0) {
            return transform.position;
        }

        if (section < 0) {
            return m_trajectory[0];
        } else if (section > m_trajectory.Count - 2) {
            return m_trajectory[m_trajectory.Count - 1];
        }

        Vector3 startPos = m_trajectory[section];
        Vector3 endPos = m_trajectory[section + 1];
        switch (interpolationMethod) {
            case InterpolationMethod.HermiteSpline:
                return Trajectory.HermiteSplineInterpolation(
                    m_trajectory[Math.Max(section - 1, 0)],
                    m_trajectory[section],
                    m_trajectory[section + 1],
                    m_trajectory[Math.Min(section + 2, m_trajectory.Count - 1)],
                    offset);

            case InterpolationMethod.Linear:
            default:
                return Vector3.Lerp(startPos, endPos, offset);
        }
    }

    private static Vector3 HermiteSplineInterpolation(Vector3 before, Vector3 start, Vector3 end, Vector3 after, float offset) {
        Vector3 D1 = (end - before) / 2;
        Vector3 D2 = (after - start) / 2;

        Matrix4x4 conditions = new Matrix4x4(
            new Vector4(start.x, start.y, start.z),
            new Vector4(D1.x, D1.y, D1.z),
            new Vector4(end.x, end.y, end.z),
            new Vector4(D2.x, D2.y, D2.z)
        );

        float offset2 = offset * offset;
		float offset3 = offset2 * offset;
        Vector4 blendingFunction = new Vector4(
            2 * offset3 - 3 * offset2 + 1,
            offset3 - 2 * offset2 + offset,
            -2 * offset3 + 3 * offset2,
            offset3 - offset2
        );
        Vector4 result = conditions * blendingFunction;

        return new Vector3(result.x, result.y, result.z);
    }

    public void UpdatePoints() {
        m_trajectory = new List<Vector3>(trajectory.Capacity);
        if (gameObject != null) {
            m_trajectory.Add(gameObject.transform.position);
        }
        if (trajectory != null) {
            foreach (GameObject point in trajectory) {
                if (point != null) {
                    m_trajectory.Add(point.transform.position);
                }
            }
        }
        if (goal != null) {
            m_trajectory.Add(goal.transform.position);
        }
    }

    void OnDrawGizmos() {
        if (!Application.isPlaying) {
            UpdatePoints();
        }

        Gizmos.color = new Color(0.5f, 0.2f, 0.4f, 0.8f);
        float stepSize = 0.05f;
        for (int section = 0; section < m_trajectory.Count; ++section) {
            for (float offset = 0; offset <= 1; offset += stepSize) {
                Gizmos.DrawLine(GetAt(section, offset), GetAt(section, offset + stepSize));
            }
        }
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = new Color(0.5f, 0.6f, 0.8f, 0.8f);
        float stepSize = 0.05f;
        for (int section = 0; section < m_trajectory.Count; ++section) {
            for (float offset = 0; offset <= 1; offset += stepSize) {
                Gizmos.DrawLine(GetAt(section, offset), GetAt(section, offset + stepSize));
            }
        }
    }
}

}