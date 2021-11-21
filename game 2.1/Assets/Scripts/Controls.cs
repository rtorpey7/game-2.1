using UnityEngine;
using UnityEngine.InputSystem;

public class Controls : MonoBehaviour
{
    public Vector2 mouseDelta;
    public bool jump, left, right, sprint, crouch;
    public Vector2 movement;
    void OnJump(InputValue val) {
        if (!jump && val.Get<float>() > 0) gameObject.SendMessage("Jumped");
        if (jump && val.Get<float>() <= 0) gameObject.SendMessage("JumpReleased");
        jump = val.Get<float>() > 0;
    }
    void OnLeftMouse(InputValue val)
    {
        left = val.Get<bool>();
    }
    void OnRightMouse(InputValue val)
    {
        right = val.Get<bool>();
    }
    void OnSprint(InputValue val)
    {
        if (!sprint && val.Get<float>() > 0) gameObject.SendMessage("Sprint");
        if (sprint && val.Get<float>() <= 0) gameObject.SendMessage("SprintReleased");
        sprint = val.Get<float>() > 0;
    }
    void OnCrouch(InputValue val)
    {
        if (!crouch && val.Get<float>() > 0) gameObject.SendMessage("Crouched");
        if (crouch && val.Get<float>() <= 0) gameObject.SendMessage("CrouchReleased");
        crouch = val.Get<float>() > 0;
    }
    void OnMouse(InputValue val)
    {
        mouseDelta = val.Get<Vector2>();
    }
    void OnMovement(InputValue val)
    {
        movement = val.Get<Vector2>();
    }
}
