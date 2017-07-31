using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController_Sapphi : MonoBehaviour
{
    private static int m_idxLayerGround;

    // Components
    private Animator m_animator;                // Character Animation
    private Rigidbody m_body;                   // Rigidbody
    private AnimatorStateInfo m_animStatePrev;  // Animation State (Previous Frame)
    private AnimatorStateInfo m_animStateCurr;  // Animation State (Current Frame)

    // Animation Property Hashes
    private int m_hFloatVelY;
    private int m_hTriggerJump;
    private int m_hTriggerAttack;
    private int m_hFlagMove;
    private int m_hFlagSprint;
    private int m_hFlagAerial;

    // Animation State Hashes
    private static int m_hStateWalk = Animator.StringToHash("base.locomotion.walk");
    private static int m_hStateRun = Animator.StringToHash("base.locomotion.run");
    private static int m_hStateJumpInit = Animator.StringToHash("base.aerial.jump_init");
    private static int m_hStateFreefall = Animator.StringToHash("base.aerial.freefall");

    // Input Variables
    private bool m_bFlagMove;
    private bool m_bFlagSprint;
    private bool m_bFlagAirborne;
    private bool m_bTriggerJump;

    private float m_fRotation;

    // Velocity
    public float m_fVelocityJump    = 10.0f;
    public float m_fVelocityWalk    = 2.0f;
    public float m_fVelocitySprint  = 3.5f;
    private float m_fRotPerSec      = (Mathf.PI / 2.0f);

    // Use this for initialization
    void Start ()
    {
        // Initialize hashes
        m_hFloatVelY        = Animator.StringToHash("Float_VelY");
        m_hTriggerJump      = Animator.StringToHash("Trigger_Jump");
        m_hTriggerAttack    = Animator.StringToHash("Trigger_Attack");
        m_hFlagMove         = Animator.StringToHash("Flag_Move");
        m_hFlagSprint       = Animator.StringToHash("Flag_Sprint");
        m_hFlagAerial       = Animator.StringToHash("Flag_Airborne");

        m_idxLayerGround    = LayerMask.NameToLayer("Ground");

        // Retrieve Animator component
        m_animator  = gameObject.GetComponent<Animator>();
        m_body      = gameObject.GetComponent<Rigidbody>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        UpdateInput();

        // Collision-based stuff...?


        // UpdateAnimator()?
        m_animator.SetFloat(m_hFloatVelY, m_body.velocity.y);
        m_animator.SetBool(m_hFlagMove, m_bFlagMove);
        m_animator.SetBool(m_hFlagSprint, m_bFlagSprint);
        m_animator.SetBool(m_hFlagAerial, m_bFlagAirborne);
        if (m_bTriggerJump)
        {
            m_animator.SetTrigger(m_hTriggerJump);  // :NOTE: Triggers seems to stick around until consumed, so need logic to only fire directly before jumping 
        }

        m_animStatePrev = m_animStateCurr;
        m_animStateCurr = m_animator.GetCurrentAnimatorStateInfo(0);

        UpdateMobility();
	}

    private void UpdateInput()
    {
        m_bFlagMove     = (Input.GetAxis("Vertical") > 0.0f);
        m_bFlagSprint   = Input.GetButton("Fire3");

        m_fRotation     = Input.GetAxis("Horizontal");

        m_bTriggerJump = Input.GetButtonDown("Jump");
    }

    private void UpdateMobility()
    {
        // Check current animation state
        AnimatorStateInfo anim_curr = m_animStateCurr;

        // ROTATION
        if (m_fRotation != 0.0f)
        {
            // Calculate rotation for this frame based on DT
            float rot_frame = (m_fRotPerSec * Time.deltaTime) * (m_fRotation > 0.0f ? 1.0f : -1.0f);
            //float sin_rot = Mathf.Sin(rot_frame);
            //float cos_rot = Mathf.Cos(rot_frame);
            float sin_hrot = Mathf.Sin(rot_frame / 2.0f);
            float cos_hrot = Mathf.Cos(rot_frame / 2.0f);

            // Create quaternion
            Quaternion q_rot = new Quaternion(0.0f, sin_hrot, 0.0f, cos_hrot);

            // Normalize
            double q_length = Math.Sqrt(Convert.ToDouble(Quaternion.Dot(q_rot, q_rot)));
            float q_length_f = Convert.ToSingle(q_length);
            q_rot.x /= q_length_f;
            q_rot.y /= q_length_f;
            q_rot.z /= q_length_f;
            q_rot.w /= q_length_f;

            // Apply quaternion
            m_body.rotation = q_rot * m_body.rotation;  // :NOTE: I think we're only applying the left-hand rotations, 
                                                        // and Unity generates the conjugate automatically to apply to the RHS
        }

        // JUMP
        if (anim_curr.fullPathHash == m_hStateJumpInit)
        {
            // State: Enter
            if (anim_curr.fullPathHash != m_animStatePrev.fullPathHash)
            {
                //m_body.velocity = new Vector3(m_body.velocity.x, m_fVelocityJump, m_body.velocity.z);
            }
        }

        // MOVEMENT
        Vector3 v_forward = transform.forward;
        if (m_bFlagMove && (anim_curr.fullPathHash == m_hStateWalk))
        {
            m_body.velocity = m_fVelocityWalk * v_forward; // new Vector3(m_body.velocity.x, m_body.velocity.y, m_fVelocityWalk);
        }
        else
        if (m_bFlagMove && (anim_curr.fullPathHash == m_hStateRun))
        {
            m_body.velocity = m_fVelocitySprint * v_forward;//new Vector3(m_body.velocity.x, m_body.velocity.y, m_fVelocitySprint);
        }
        else
        {
            m_body.velocity = new Vector3(0.0f, m_body.velocity.y, 0.0f);// new Vector3(m_body.velocity.x, m_body.velocity.y, 0.0f);
        }


        //switch (anim_curr.fullPathHash)
        //{
        //    case m_hStateWalk:
        //    {
        //        break;
        //    }
        //    case m_hStateRun:
        //    {

        //        break;
        //    }
        //    default:
        //    {
        //        break;
        //    }
        //}
    }



    // COLLISION
    private void OnCollisionEnter(Collision collision)
    {
        Vector3 v_euler = collision.gameObject.transform.rotation.eulerAngles;
        if (collision.collider.gameObject.layer == m_idxLayerGround)
        {
            print("Grounded!");
            m_bFlagAirborne = false;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
    }

    private void OnCollisionExit(Collision collision)
    {
        Vector3 v_euler = collision.gameObject.transform.rotation.eulerAngles;
        if (collision.collider.gameObject.layer == m_idxLayerGround)
        {
            print("Airborne!");
            m_bFlagAirborne = true;
        }
    }
}
