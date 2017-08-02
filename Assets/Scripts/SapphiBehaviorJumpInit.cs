using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SapphiBehaviorJumpInit : StateMachineBehaviour
{
    //private bool m_bVelApplied;
    //// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    //override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    m_bVelApplied = false;  // Has velocity been applied yet?
    //}

    //// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // If jump_init animation has reached/passed the point that it's 
    //    if (!m_bVelApplied && stateInfo.normalizedTime >= 0.75f)
    //    {
    //        Rigidbody obj_body = animator.gameObject.GetComponent<Rigidbody>();
    //        if (obj_body)
    //        {
    //            Vector3 vel_body = obj_body.velocity;
    //            obj_body.velocity = vel_body + new Vector3(0.0f, 5.0f, 0.0f);
    //        }
    //        m_bVelApplied = true;
    //    }
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}
}
