using UnityEngine;

public class GroundChecker : MonoBehaviour
{
    public LayerMask whatIsGround;
    public float maxIncline;
    public bool grounded;
    public bool walled;
    public Rigidbody physics;
    public Vector3 normalVector;
    private bool resetGround = false;
    private bool resetWall = false;
    void FixedUpdate()
    {
        if (physics.IsSleeping())
        {
            physics.WakeUp();
        }
        resetGround = true;
        resetWall = true;
    }
    //returns whether a surface is recognized as a floor. Input the normal vector of surface
    private bool IsFloor(Vector3 v)
    {
        return Vector3.Angle(transform.up, v) < maxIncline;
    }
    //returns whether a surface is recognized as a wall. Input normal vector of surface
    private bool IsWall(Vector3 v)
    {
        return Vector3.Angle(transform.up, v) >= maxIncline;
    }
    private void OnCollisionStay(Collision other)
    {
        //Make sure we are only checking for walkable layers
        int layer = other.gameObject.layer;
        if (whatIsGround.value != (whatIsGround.value | (1 << layer))) return;
        Vector3 Normal;
        foreach (ContactPoint contact in other.contacts)
        {
            Normal = contact.normal;
            if (IsFloor(Normal)) { grounded = true; resetGround = false; }
            if (IsWall(Normal)) { walled = true; resetWall = false; }
            Debug.DrawRay(contact.point, other.impulse, Color.white);
        }
    }
    private void LateUpdate()
    {
        if (resetGround) grounded = false;
        if (resetWall) walled = false;
    }
}
