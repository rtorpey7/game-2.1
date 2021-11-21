using UnityEngine;

public class PlayerRotation : MonoBehaviour
{
    public float speed;
    public Controls input;
    void Update()
    {
        float rot = input.mouseDelta.x * speed * Time.deltaTime;
        transform.rotation *= Quaternion.Euler(0, rot, 0);
    }
}
