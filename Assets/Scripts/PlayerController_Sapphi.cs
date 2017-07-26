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

    // Animation Variables
    private bool m_bFlagMove;
    private bool m_bFlagSprint;

    // Velocity
    private float m_fVelocityWalk   = 2.0f;
    private float m_fVelocitySprint = 3.0f;

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
    }

    private void UpdateMobility()
    {
        // Check current animation state
        AnimatorStateInfo anim_curr = m_animator.GetCurrentAnimatorStateInfo(0);

        if (anim_curr.fullPathHash == m_hStateWalk)
        {
            m_body.velocity = new Vector3(m_body.velocity.x, m_body.velocity.y, m_fVelocityWalk);
        }
        else
        if (anim_curr.fullPathHash == m_hStateRun)
        {
            m_body.velocity = new Vector3(m_body.velocity.x, m_body.velocity.y, m_fVelocitySprint);
        }
        else
        {
            m_body.velocity = new Vector3(m_body.velocity.x, m_body.velocity.y, 0.0f);
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
