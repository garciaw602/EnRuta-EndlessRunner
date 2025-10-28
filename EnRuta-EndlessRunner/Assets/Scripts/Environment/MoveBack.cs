using UnityEngine;

public class Moveback : MonoBehaviour
{
    public float speed = 10f;

    void Update()
    {
        transform.Translate(Vector3.back * speed * Time.deltaTime, Space.World);
    }

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }
}
