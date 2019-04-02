using System.Collections.Generic;
using System;
using UnityEngine;

[ExecuteInEditMode]
public class Trajectory : MonoBehaviour
{
    [Tooltip("The trajectory points. Always includes this GameObject's transform and optionally a goal transform.")]
    public List<Vector3> trajectory = new List<Vector3>(new Vector3[] {
        new Vector3(1f, 0f, 0f),
        new Vector3(2f, 0f, 0f),
        new Vector3(3f, 0f, 0f),
        new Vector3(4f, 0f, 0f),
    });

    private List<Vector3> m_trajectory;

    [Tooltip("The trajectory goal - if empty, the last point of the trajectory is the goal.")]
    public Transform goal;

    public void Awake() {
        m_trajectory = new List<Vector3>(trajectory);
    }

    public void Start() {
        if (goal != null) {
            m_trajectory.Add(goal.position);
        }
    }

    public void Update() {
        m_trajectory = new List<Vector3>(trajectory);
        if (goal != null) {
            m_trajectory.Add(goal.position);
        }
    }

    public Vector3 GetAt(float offset) {
        int section = (int) Math.Truncate((decimal) offset);
        return this.GetAt(section, offset - section);
    }

    public Vector3 GetAt(int section, float offset) {
        if (section < 0) {
            return m_trajectory[0];
        } else if (section > m_trajectory.Count - 2) {
            return m_trajectory[m_trajectory.Count - 1];
        }
        return Vector3.Lerp(m_trajectory[section], m_trajectory[section + 1], offset);
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.magenta;
        foreach(Vector3 p in trajectory) {
            Gizmos.DrawSphere(p, 0.2f);
        }
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        foreach(Vector3 p in trajectory) {
            Gizmos.DrawSphere(p, 0.2f);
        }
    }
}
