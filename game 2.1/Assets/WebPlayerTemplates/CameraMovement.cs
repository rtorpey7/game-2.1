using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MilkShake;

public class CameraMovement : MonoBehaviour
{
    public Controls input;
    GroundChecker groundCheck;

    bool primed;
    private void Awake()
    {
        groundCheck = GetComponent<GroundChecker>();
    }
    void FixedUpdate()
    {
        if (!groundCheck.grounded) primed = true;
        if(groundCheck.grounded && primed)
        {
            //set up timer to see how long was in air
        }
        if (groundCheck.grounded) primed = false;
    }
}
