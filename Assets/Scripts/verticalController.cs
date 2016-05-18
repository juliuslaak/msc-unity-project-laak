using UnityEngine;
using System.Collections;

public class verticalController : StateMachineBehaviour {

    private Transform transform;
    private Vector3 enterPosition;
    private int horizontalSpeedHash = Animator.StringToHash("HorizontalSpeed");

    private float startTime;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        transform = animator.GetComponent<Transform>();
        enterPosition = transform.position;

        animator.SetFloat(horizontalSpeedHash, 0.0F);

        //animator.transform.gameObject.GetComponent<Renderer>().material.color = new Color(255, 0, 0);

        startTime = Time.time;
    }

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	//override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {

        BallStatisticsController.control.AddToTimesPositions(Time.time - startTime, transform.position.y - enterPosition.y);

        // TODO is this what we want?
        //transform.position = enterPosition;
        animator.SetFloat(horizontalSpeedHash, 1.0F);

        //animator.transform.gameObject.GetComponent<Renderer>().material.color = new Color(255, 255, 255);
        
    }

	// OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
	//override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	// OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
	//override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}
}
