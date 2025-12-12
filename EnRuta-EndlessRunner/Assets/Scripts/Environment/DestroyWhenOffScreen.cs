using UnityEngine;

public class DestroyWhenOffScreen : MonoBehaviour
{
    private void Update()
    {
        if (Camera.main == null) return;

        Vector3 viewportPos = Camera.main.WorldToViewportPoint(transform.position);
        if (viewportPos.z < 0 || viewportPos.y < -0.2f || viewportPos.y > 1.2f)
        {
            Destroy(gameObject);
        }
    }
}
