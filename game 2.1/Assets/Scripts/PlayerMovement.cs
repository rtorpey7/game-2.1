using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    #region variables
    [Header("References")]
	//object contains variables for all inputs
	public Controls input;
	//used to determine if the player is touching the ground, or a wall
    public GroundChecker groundCheck;
	//the rigidbody of the pkayer, uised to do all movement
    public Rigidbody physics;
	//object used to know the position of the head
    public Transform eyes;
	//component used to simulate gravity for player.
	public ConstantForce gravity;
	[Header("Movement")]
	//velocity of the players walk
	public float walkSpeed;
	//% change in walk speed when sprinting
    public float sprintFactor;
	//amount of time it takes to get to 100% speed while walking
	public float accTime;
	//acceleration value for walking
	private float walkAcc;
	//acceleration value for sprinting
	private float sprintAcc;
	//max speed attained while sprinting, = walkSpeed*sprintFactor;
	private float sprintSpeed;
    //constant to multiply sprint speed by to keep momentum
    private float speedMultiplier;

	[Header("Jump Values")]
	//height attained when jumping
	public float jumpHeight;
	//time it takes to reach peak
    public float upTime;
	//total time of jump
    public float jumpTime;
	//amount of time to go back down after reaching peak.
	private float downTime;
	//gravity constant used in first part of jump
	private float upGrav;
	//gravity constant used during second falling part of jump
	private float downGrav;
	//inital velocity of jump
	private float jumpVelocity;
	//% of down gravity to use for the standard falling gravity
	public float standardGravPercent = 2f;
	//gravity constant to use when not presing spacebar.
	private float standardGrav;
	[Header("Crouch")]
	//height of character when fully crouched
	public float crouchHeight;
	//time it takes to reach a fully crouched state
	public float crouchTime;
	//total distance a slide will last on the ground
	public float slideDistance;
	//total time it will take for a slide to take
	public float slideTime;
	//a graph representing the amount of friction the slie will have during its course.
	//i recommend a graph starting at 0 and staying at 0 for 90% of the time, before finally ramping up to 1 in the end. this will amke the slide feel really smooth at teh start, and then slow down rapidly at the end.
	public AnimationCurve slideFriction;
	//amount of time needed before a second slide can be used
	public float slideCooldown;
	//minimum amount of time before a slide can be used again
	public float minSlideSpeed;
	//factor of standard gravity to use when crouching, usually greater than 1
	public float crouchGravityRatio;
	//factor of walk speed to use when crouching
	public float crouchSpeedFactor = 0.5f;
	//gravity constant to use when crouching
	private float crouchGrav;
	//amount to shrink each second when crouching. = (normalheight- crouchheight) / crouchtime
	private float crouchStep;
	//velocity to use when crouching. = walkspeed*crouchSpeedFactor
	private float crouchSpeed;
	//value to represent the players normal height
	private float originalHeight;
	[Header("Vault")]
	//velocity to use when vaulting an object
	public float vaultVelocity;
	//boolean representing whether or not we are currently crouching.
	private bool crouching = false, sprinting = false;
	//integers used to determine what state we are in.
	private int crouchState, slideState, vaultState, jumpState;
	//local boolean repesenting the grounded state of the player
	private bool grounded = true;
    #endregion
    /*Different State Values:
	 * 
	 * crouch:
	 * 0 - not crouching
	 * 1 - shrinking, and on the ground
	 * 2 - currently crouched, idle
	 * 3 - growing, and on ground
	 * 4 - shrinking and in air
	 * 5 - currently crouched, idle
	 * 6 - growing, and in air
	 * the difference between 1 and 4, and 3 and 6 is that when crouchng on the groud we lower our head and keep the feet in the same spot, while in the air we do the opposite, we raise our feet and keep our head int he same spot
	 * 
	 * jump:
	 * 0-not jumping
	 * 1 - just jumped, adding velocity and changing gravity to upGrav
	 * 2 - in stage 1 of the jump, waiting for velocity to go to zero before going to state 3
	 * 3 - reached the second stage of the jump, already set the gravity to downGrav, waiting to touch the ground. when we do, we sert the gravity to standardGrav, and go to state 0
	 * if at anypoint duriong these states, the spacebar is released, gravity is set to standard, and state is set to 0
	 */
    private void Awake()
    {
		//jump math
		//this math is really complicated to derive but its right, so just leave it
		downTime = jumpTime - upTime;
		upGrav = (-2f * jumpHeight) / (upTime*upTime);
		downGrav = (-2f * jumpHeight) / (downTime * downTime);
		jumpVelocity = -upGrav*upTime;
		standardGrav = downGrav * standardGravPercent;
		gravity.force = new Vector3(0, standardGrav, 0);

		//walk math
		walkAcc = walkSpeed / accTime;
		sprintAcc = sprintFactor * walkSpeed / accTime;
		sprintSpeed = sprintFactor * walkSpeed;
		
		//crouch math
		crouchGrav = downGrav * crouchGravityRatio;
		crouchStep = (transform.localScale.y - crouchHeight) / crouchTime;
		crouchSpeed = walkSpeed * crouchSpeedFactor;
		originalHeight = transform.localScale.y;
	}
    private void Move()
    {
        // using our player rotate right and forward, as well as our input values, create a movement vector called velGoal
        Vector3 xMove = input.movement.x * transform.right * walkSpeed;
		Vector3 zMove = input.movement.y * transform.forward * walkSpeed;

        if (crouching) { xMove *= crouchSpeedFactor; zMove *= crouchSpeedFactor; }
        else if (sprinting && input.movement.y > 0) { xMove *= sprintFactor * speedMultiplier; zMove *= sprintFactor * speedMultiplier;}

        Vector2 velGoal = new Vector2(xMove.x + zMove.x, xMove.z + zMove.z);
		Vector2 curVel = new Vector2(physics.velocity.x, physics.velocity.z);

        //changes acceleration based on speed
        float moveAcc = walkAcc;
        if (curVel.magnitude > walkSpeed) { moveAcc *= (curVel.magnitude / walkSpeed) * (curVel.magnitude / walkSpeed); }

        //if the change between our current velocity and the velocity we want to go is smaller than the update we are about to do to it
        if ((velGoal - curVel).magnitude <= walkAcc * Time.fixedDeltaTime)
		{
			//make our velocity directly equal to the goal velcoity. this makes it so that we dont overshoot the velocity
			physics.velocity = new Vector3(velGoal.x, physics.velocity.y, velGoal.y);
		}
		else
		{
			//change our velocity based on our walk acceleration value
			Vector2 step = (velGoal - curVel).normalized * walkAcc * Time.fixedDeltaTime;
			physics.velocity = new Vector3(physics.velocity.x + step.x, physics.velocity.y, physics.velocity.z + step.y);
		}
	}
	private void Jump() 
	{
		switch (jumpState)
		{
			case 1:
				physics.velocity += transform.up * jumpVelocity;
				gravity.force = new Vector3(0, upGrav, 0);
				jumpState = 2;
				break;
			case 2:
				if (physics.velocity.y <= 0)
				{
					gravity.force = new Vector3(0, downGrav, 0);
					jumpState = 3;
				}
				break;
			case 3:
				if (grounded)
				{
					gravity.force = new Vector3(0, standardGrav, 0);
					jumpState = 0;
				}
				break;
		}
	}
	private void Crouch() 
	{
		Vector3 scale = transform.localScale;
		Vector3 position = transform.localPosition;
		float step = crouchStep * Time.fixedDeltaTime;
		switch (crouchState)
		{
			case 1:
				//decrease the scale based on crouchstep
				scale = new Vector3(scale.x, scale.y - step, scale.z);
				//decrease the position by 1/2 of what we decreased thje scale by to make sure that the bottom of our character stays at the same spot
				position = new Vector3(position.x, position.y - step / 2f, position.z);
				//if we have gone lower than our crouch height
				if (scale.y < crouchHeight)
				{
					//add 1/2 of what we overshot by to out position 
					position = new Vector3(position.x, position.y + (crouchHeight - scale.y) / 2, position.z);
					//set our scale corectly
					scale = new Vector3(scale.x, crouchHeight, scale.z);
					//go to state 2 as we have now fully crouched
					crouchState = 2;
				}
				//if we ever leave the ground, then go to state 4, which shrinks our character in the air
				if (grounded) crouchState = 4;
				break;
			case 3:
				//this state is the same as state 1, except we increase our scale and position rather than decrease it
				scale = new Vector3(scale.x, scale.y + step, scale.z);
				position = new Vector3(position.x, position.y + step / 2f, position.z);
				if (scale.y > originalHeight)
				{
					position = new Vector3(position.x, position.y - (scale.y - originalHeight) / 2, position.z);
					scale = new Vector3(scale.x, originalHeight, scale.z);
					crouchState = 0;
				}
				if (!grounded) crouchState = 6;
				break;
			case 4:
				//decrease scale by crouchstep
				scale = new Vector3(scale.x, scale.y - step, scale.z);
				//increase our position by 1/2 of what we shrank ourselves by to make sure our head remains in the same spot
				position = new Vector3(position.x, position.y + step / 2f, position.z);
				//if we have overshot
				if (scale.y < crouchHeight)
				{
					//set position to the correct value
					position = new Vector3(position.x, position.y - (crouchHeight - scale.y) / 2, position.z);
					//set scale to the correct value
					scale = new Vector3(scale.x, crouchHeight, scale.z);
					//go to idle state
					crouchState = 5;
				}
				//if we ever touch the ground make sure to switch to the ground shrink state
				if (grounded) crouchState = 1;
				break;
			case 6:
				//same as state 4 except we increase our scale, and decrease our position.
				scale = new Vector3(scale.x, scale.y + step, scale.z);
				position = new Vector3(position.x, position.y - step / 2f, position.z);
				if (scale.y > originalHeight)
				{
					position = new Vector3(position.x, position.y + (scale.y - originalHeight) / 2, position.z);
					scale = new Vector3(scale.x, originalHeight, scale.z);
					crouchState = 0;
				}
				if (grounded) crouchState = 3;
				break;
		}
		transform.localScale = scale;
		transform.localPosition = position;
	}
    private void FixedUpdate()
    {
		grounded = groundCheck.grounded;
        CalcSpeed();
		Move();
		Jump();
		Crouch();
		
    }
	void Jumped() 
	{
		if (groundCheck.grounded && jumpState == 0) jumpState = 1;
	}
	void JumpReleased() 
	{
		gravity.force = new Vector3(0, standardGrav, 0);
		jumpState = 0;
	}
	void Crouched() 
	{
		if (groundCheck.grounded)
		{
			crouchState = 1;
			crouching = true;
			if ((physics.velocity.x * physics.velocity.x + physics.velocity.z * physics.velocity.z) >= minSlideSpeed * minSlideSpeed)
			{
				slideState = 1;
			}
		}
		else 
		{
			crouching = true;
			crouchState = 4;
		}
	}
    void CrouchReleased()
    {
		if (groundCheck.grounded)
		{
			crouchState = 3;
			crouching = false;
		}
		else 
		{
			crouchState = 6;
			crouching = false;
		}
    }

    void Sprint()
    {
        sprinting = true;
    }
    void SprintReleased()
    {
        sprinting = false;
    }

    void CalcSpeed()
    {
        //allows for speed multipliers to preserve momentum
            float speedMag = new Vector2(physics.velocity.x, physics.velocity.z).magnitude;
            if(speedMag > sprintSpeed)
            {
                speedMultiplier = speedMag / sprintSpeed;
                return;
            }
        speedMultiplier = 1;
    }
}
