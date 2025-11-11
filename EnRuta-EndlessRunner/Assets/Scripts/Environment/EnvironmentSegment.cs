using UnityEngine;
using System.Collections.Generic;

public class EnvironmentSegment : MonoBehaviour
{
    [Header("Lane points")]
    public Transform[] lanePoints;

    [Header("Optional length helpers")]
    public Transform startPoint;
    public Transform endPoint;

    [Header("Object container (auto created if missing)")]
    public Transform objectsRoot;

    private float cachedLength = -1f;

    void Awake()
    {
        if (objectsRoot == null)
        {
            GameObject root = new GameObject("Objects");
            root.transform.SetParent(transform);
            root.transform.localPosition = Vector3.zero;
            objectsRoot = root.transform;
        }
    }

    public float GetSegmentLength()
    {
        if (cachedLength > 0f) return cachedLength;

        if (startPoint != null && endPoint != null)
        {
            cachedLength = Mathf.Abs(endPoint.position.z - startPoint.position.z);
            if (cachedLength > 0f) return cachedLength;
        }

        Renderer[] rs = GetComponentsInChildren<Renderer>();
        if (rs != null && rs.Length > 0)
        {
            Bounds b = rs[0].bounds;
            for (int i = 1; i < rs.Length; i++) b.Encapsulate(rs[i].bounds);
            cachedLength = b.size.z;
            if (cachedLength > 0f) return cachedLength;
        }

        cachedLength = 40f;
        return cachedLength;
    }

    public void ClearObjects()
    {
        if (objectsRoot == null) return;
        for (int i = objectsRoot.childCount - 1; i >= 0; i--)
            Destroy(objectsRoot.GetChild(i).gameObject);
    }
}
