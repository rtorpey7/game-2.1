using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    public Controls input;
    float yrot;
    public float yRotSpeed;
    public Transform cameraAnchor;
    public float xRotSpeed;
    float xrot;
    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    void Update()
    {
        yrot -= input.mouseDelta.y * Time.fixedDeltaTime * yRotSpeed;
        yrot = Mathf.Clamp(yrot, -70, 80);
        cameraAnchor.localRotation = Quaternion.Euler(yrot, 0, 0);
        xrot = input.mouseDelta.x * xRotSpeed * Time.fixedDeltaTime;
        transform.rotation *= Quaternion.Euler(0, xrot, 0);
    }
}
