using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{
    Controls im;
    GroundChecker groundCheck;
    float x, z;
    bool grounded;
    public Animator anim;
    private void Awake()
    {
        im = GetComponent <Controls>();
        groundCheck = GetComponent<GroundChecker>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        x = im.movement.x;
        z = im.movement.y;
        grounded = groundCheck.grounded;
        if (z > .05 || Mathf.Abs(x) > .05 && Mathf.Abs(z) < .05)
        {
            anim.SetInteger("walk", 1);
        }
        else if (Mathf.Abs(x) < .05 && Mathf.Abs(z) < .05)
        {
            anim.SetInteger("walk", 0);
        }
        else
        {
            anim.SetInteger("walk", -1);
        }
        if (grounded) anim.SetBool("grounded", true);
        else anim.SetBool("grounded", false);
    }
}
