using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementOld : MonoBehaviour
{
    Controls im;
    GroundChecker groundCheck;
    Rigidbody rb;
    public Transform scalePreserver;
    public Transform eyes;

    [Header("Basic Movement")]
    public float walkSpeed;
    public float sprintSpeed;
    private float savedSprintSpeed;
    float scaledAcceleration;
    //between 1 and 1/fixedTimeStep
    public float accelerationFactor;
    public float airAcceleration;
    public float extraGrav;
    float moveSpeed;

    [Header("Jumping")]
    public float jumpForce;
    bool readyToJump = true;
    public float jumpCooldown;
    bool jumping;

    [Header("Crouching")]
    public float crouchForce;
    public float airCrouchForce;
    public float crouchFriction;
    public float crouchExtraGravity;
    public float crouchSpeed;
    public float minimumSlideSpeed;
    public float slideCooldown;
    public Vector3 armsOffset;
    bool needsToCrouch, canSlide = true;
    public bool crouching, alsoSliding;
    Vector3 playerScale;
    public Transform cam;

    [Header("Vault")]
    public float vaultForce;
    bool vaultable;


    private void Awake()
    {
        im = GetComponent<Controls>();
        groundCheck = GetComponent<GroundChecker>();
        rb = GetComponent<Rigidbody>();
        playerScale = transform.localScale;
        savedSprintSpeed = sprintSpeed;
    }
    // Update is called once per frame
    void Update()
    {
        scalePreserver.position = eyes.position + armsOffset;
        scalePreserver.rotation = eyes.rotation;
        //arms.localRotation = cam.localRotation;
        jumping = im.jump;
        if (im.crouch && !crouching) needsToCrouch = true;
        if (!im.crouch) needsToCrouch = false;
    }

    private void FixedUpdate()
    {
        Movement();

    }

    private void Movement()
    {
        float playerSpeed = new Vector3(rb.velocity.x, 0, rb.velocity.z).magnitude / Time.fixedDeltaTime;
        if (playerSpeed > savedSprintSpeed)
        {
            sprintSpeed = playerSpeed;
        }
        else
        {
            sprintSpeed = savedSprintSpeed;
        }
        if (needsToCrouch && !crouching)
        {
            StartCrouch();
        }
        else if(!needsToCrouch && crouching)
        {
            StopCrouch();
        }
        else if (alsoSliding)
        {
            OnSlide();
        }

        if (crouching && !alsoSliding) moveSpeed = crouchSpeed;
        else if (im.sprint && im.movement.y > .1f) moveSpeed = sprintSpeed;
        else moveSpeed = walkSpeed;

        if (jumping)
        {
            Jump();
            needsToCrouch = false;
        }
        if (groundCheck.grounded)
        {
            scaledAcceleration = accelerationFactor;
            rb.AddForce(Vector3.down * extraGrav * Time.fixedDeltaTime);
        }
        else
        {
            scaledAcceleration = airAcceleration;
        }
        if (!alsoSliding)
        {
            Vector2 inputVector = new Vector2(im.movement.x, im.movement.y).normalized;
            Vector3 xMove = inputVector.x * Time.fixedDeltaTime * transform.right * moveSpeed;
            Vector3 zMove = inputVector.y * Time.fixedDeltaTime * transform.forward * moveSpeed;
            float xValue = Mathf.Lerp(rb.velocity.x, xMove.x + zMove.x, scaledAcceleration * Time.fixedDeltaTime);
            float zValue = Mathf.Lerp(rb.velocity.z, xMove.z + zMove.z, scaledAcceleration * Time.fixedDeltaTime);
            rb.velocity = new Vector3(xValue, rb.velocity.y, zValue);
        }
        
        if(!groundCheck.grounded && groundCheck.walled && im.movement.y > .1f && vaultable && rb.velocity.y <= 0)
        {
            Vault();
        }
    }

    void Vault()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        rb.AddForce(Vector2.up * vaultForce, ForceMode.Impulse);
    }

    void StartCrouch()
    {
        crouching = true;
        Vector3 scale = new Vector3(playerScale.x, playerScale.y / 2f, playerScale.z);
        transform.localScale = scale;
        transform.position = new Vector3(transform.position.x, transform.position.y - .5f, transform.position.z);

        ////keep scale of child objects
        //scalePreserver.localScale = new Vector3(preserverScale.x, preserverScale.y * 2, preserverScale.z);
        //foreach(Transform child in scaledChildren)
        //{
        //    child.localPosition = new Vector3(child.localPosition.x, child.localPosition.y -.5f, child.localPosition.z);
        //}
        if (moveSpeed >= savedSprintSpeed && canSlide)//is sprinting, slide

        {
            if (groundCheck.grounded)
            {
                rb.AddForce(transform.forward * Time.fixedDeltaTime * crouchForce, ForceMode.Impulse);
            }
            else
            {
                rb.AddForce(transform.forward * Time.fixedDeltaTime * airCrouchForce, ForceMode.Impulse);
            }
            alsoSliding = true;
            canSlide = false;
            Invoke(nameof(ResetSlide), slideCooldown);
        }
    }

    void OnSlide()
    {
        Vector3 frictionVector = -new Vector3(rb.velocity.x, 0, rb.velocity.z).normalized;
        if (groundCheck.grounded)
        {
            rb.AddForce(frictionVector * Time.fixedDeltaTime * crouchFriction);
        }
        rb.AddForce(Vector3.down * Time.fixedDeltaTime * crouchExtraGravity);

        if(new Vector3(rb.velocity.x, 0, rb.velocity.z).magnitude < minimumSlideSpeed)
        {
            alsoSliding = false;
        }
    }

    void StopCrouch()
    {
        alsoSliding = false;
        crouching = false;
        transform.localScale = playerScale;
        transform.position = new Vector3(transform.position.x, transform.position.y + .5f, transform.position.z);

        ////keep scale of child objects
        //scalePreserver.localScale = preserverScale;
        //foreach (Transform child in scaledChildren)
        //{
        //    child.localPosition = new Vector3(child.localPosition.x, child.localPosition.y + .5f, child.localPosition.z);
        //}
    }


    private void Jump()
    {
        if (groundCheck.grounded && readyToJump)
        {
            readyToJump = false;
            //Add jump forces
            rb.AddForce(Vector2.up * jumpForce * 1.5f, ForceMode.Impulse);
            rb.AddForce(groundCheck.normalVector * jumpForce * 0.5f, ForceMode.Impulse);

            //If jumping while falling, reset y velocity.
            Vector3 vel = rb.velocity;
            if (rb.velocity.y < 0.5f)
                rb.velocity = new Vector3(vel.x, 0, vel.z);
            else if (rb.velocity.y > 0)
                rb.velocity = new Vector3(vel.x, vel.y / 2, vel.z);

            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void ResetSlide()
    {
        canSlide = true;
    }
    private void ResetJump()
    {
        readyToJump = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        vaultable = false;
    }
    private void OnTriggerExit(Collider other)
    {
        vaultable = true;
    }
}
