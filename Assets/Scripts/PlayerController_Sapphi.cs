using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController_Sapphi : MonoBehaviour
{
    // Components
    private Animator m_animator;                //Character Animation
    private Rigidbody m_body;                   // Rigidbody
    private string m_animationNext = null;      //Character Last Animation
    internal string m_animationCurr = null;     //Character Animation Name

    // Animation Property Hashes
    private int m_hTriggerJump;
    private int m_hTriggerAttack;
    private int m_hFlagMove;
    private int m_hFlagSprint;
    private int m_hFlagAerial;

    // Animation State Hashes
    private static int m_hStateWalk = Animator.StringToHash("base.locomotion.walk");
    private static int m_hStateRun = Animator.StringToHash("base.locomotion.run");

    // Input Variables
    private bool m_bFlagMove;
    private bool m_bFlagSprint;

    private float m_fRotation;

    // Velocity
    public float m_fVelocityWalk   = 2.0f;
    public float m_fVelocitySprint = 3.5f;
    private float m_fRotPerSec      = (Mathf.PI / 2.0f);

    // Use this for initialization
    void Start ()
    {
        // Initialize hashes
        m_hTriggerJump      = Animator.StringToHash("Trigger_Jump");
        m_hTriggerAttack    = Animator.StringToHash("Trigger_Attack");
        m_hFlagMove         = Animator.StringToHash("Flag_Move");
        m_hFlagSprint       = Animator.StringToHash("Flag_Sprint");
        m_hFlagAerial       = Animator.StringToHash("Flag_Aerial");

        // Retrieve Animator component
        m_animator  = gameObject.GetComponent<Animator>();
        m_body      = gameObject.GetComponent<Rigidbody>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        UpdateInput();

        m_animator.SetBool(m_hFlagMove, m_bFlagMove);
        m_animator.SetBool(m_hFlagSprint, m_bFlagSprint);

        UpdateMobility();
	}

    private void UpdateInput()
    {
        m_bFlagMove     = (Input.GetAxis("Vertical") > 0.0f);
        m_bFlagSprint   = Input.GetButton("Fire3");

        m_fRotation     = Input.GetAxis("Horizontal");
    }

    private void UpdateMobility()
    {
        // Check current animation state
        AnimatorStateInfo anim_curr = m_animator.GetCurrentAnimatorStateInfo(0);

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
            m_body.velocity = Vector3.zero;// new Vector3(m_body.velocity.x, m_body.velocity.y, 0.0f);
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
}
