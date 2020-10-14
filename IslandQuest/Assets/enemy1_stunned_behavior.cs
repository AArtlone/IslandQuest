using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy1_stunned_behavior : StateMachineBehaviour
{
    private Animal _animalInterface;
    float time = 0;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _animalInterface = animator.GetComponent<Animal>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log(_animalInterface.CurrentStunTime);
        time += Time.deltaTime;
        animator.transform.position = Vector2.MoveTowards(animator.transform.position, animator.transform.position,
            _animalInterface.Speed * Time.deltaTime);
        if (_animalInterface.CurrentStunTime == 0f) return;
        if (time > _animalInterface.CurrentStunTime)
        {
            _animalInterface.CurrentStunTime = 0f;
            _animalInterface.isStunned = false;
            animator.SetBool("isStunned", false);
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
