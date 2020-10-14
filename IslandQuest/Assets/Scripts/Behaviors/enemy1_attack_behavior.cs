using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy1_attack_behavior : StateMachineBehaviour
{
    private Animal _animalInterface;
    private BoxCollider2D _attackTrigger;
    float time;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _animalInterface = animator.GetComponent<Animal>();
        _animalInterface.Speed = _animalInterface.AttackSpeed;
        _attackTrigger = animator.GetComponent<BoxCollider2D>();
        _attackTrigger.enabled = true;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        time += Time.deltaTime;

        if (time > 1.0f)
        {
            _animalInterface.Target.GetComponent<Player>().TakeDamage(_animalInterface.Damage);
            _animalInterface.Speed = 1;
            time = 0f;
        }
        animator.transform.position = Vector2.MoveTowards(animator.transform.position, _animalInterface.Target.transform.position, _animalInterface.Speed * Time.deltaTime);
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
