using UnityEngine;
using UnityEngine.InputSystem;

public class Controls : MonoBehaviour
{
    public Vector2 mouseDelta;
    public bool jump, left, right, sprint, crouch, boost;
    public Vector2 movement;
    void OnJump(InputValue val)
    {
        if (!jump && val.Get<float>() > 0) gameObject.SendMessage("Jumped");
        if (jump && val.Get<float>() <= 0) gameObject.SendMessage("JumpReleased");
        jump = val.Get<float>() > 0;
    }
    void OnLeftMouse(InputValue val)
    {
        left = val.Get<float>() > 0;
    }
    void OnRightMouse(InputValue val)
    {
        right = val.Get<float>() > 0;
    }
    void OnSprint(InputValue val)
    {
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
    void OnBoost(InputValue val)
    {
        if (!boost && val.Get<float>() > 0) gameObject.SendMessage("Boosted");
        if (boost && val.Get<float>() <= 0) gameObject.SendMessage("BoostReleased");
        boost = val.Get<float>() > 0;
    }
}
