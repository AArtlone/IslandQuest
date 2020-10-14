using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy1_search_behavior : StateMachineBehaviour
{
    private Animal _animalInterface;
    Vector2 heading;
    float time;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _animalInterface = animator.GetComponent<Animal>();
        _animalInterface.Speed = _animalInterface.SearchSpeed;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {       
        time += Time.deltaTime;

        if (time > 2.0f)
        {
            CalculateHeading();
            time = 0f;
        }
        Debug.Log(heading);
        animator.transform.position = Vector2.MoveTowards(animator.transform.position, heading, _animalInterface.Speed * Time.deltaTime);
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }

    public void CalculateHeading()
    {
        heading = new Vector2(_animalInterface.Target.position.x, _animalInterface.Target.position.y);
        heading += Random.insideUnitCircle * 6;
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
