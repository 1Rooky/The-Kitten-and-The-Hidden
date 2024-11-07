using UnityEngine;

// This class is responsible for checking the attack animation state of a mushroom enemy and triggering attack logic accordingly.
public class CheckMushroomEnemyAttackAnimation : StateMachineBehaviour
{
    private AttackRadius _attackRadius; // Reference to the AttackRadius component to call attack logic
    private bool hasCalled = false; // Flag to ensure the attack logic is called only once per animation

    // Awake is called when the script instance is being loaded.
    private void Awake() => _attackRadius = FindFirstObjectByType<AttackRadius>();

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    // It checks if the animation state is the attack state and if the animation has reached or passed the halfway point.
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Check if the current animation state is the attack state and if the animation time is at least 50% through
        if (stateInfo.IsName(GameConstant.EnemyAnimation.EnemyAnimationState.AttackState) && stateInfo.normalizedTime >= 0.5f && !hasCalled)
        {
            _attackRadius.OnAttackAnimationCompleted(); // Trigger the attack logic in the AttackRadius component
            hasCalled = true; // Set the flag to true to prevent multiple calls
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    // It resets the hasCalled flag to false, allowing the attack logic to be triggered again in the next animation cycle.
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) => hasCalled = false;
}