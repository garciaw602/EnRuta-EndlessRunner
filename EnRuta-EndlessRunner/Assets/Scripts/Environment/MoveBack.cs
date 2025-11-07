using UnityEngine;

public class MoveBack : MonoBehaviour
{
    public static float GlobalSpeed = 12f;
    public bool useGlobalSpeed = true;
    public float localSpeed = 12f;

    private void Update()
    {
        float speed = useGlobalSpeed ? GlobalSpeed : localSpeed;
        transform.Translate(Vector3.back * speed * Time.deltaTime, Space.World);
    }

    public static void SetGlobalSpeed(float newSpeed)
    {
        GlobalSpeed = newSpeed;
    }
}
