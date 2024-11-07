using UnityEngine;

public class CheckCatAttackAnimation : StateMachineBehaviour
{
    private CatAttack _catAttack;
    private bool hasCalled = false;

    private void Awake() => _catAttack = FindFirstObjectByType<CatAttack>();

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.IsName(GameConstant.CatAnimation.StateNames.Attack) && stateInfo.normalizedTime >= 0.5f && !hasCalled)
        {
            _catAttack.OnAttackAnimationCompleted();
            hasCalled = true;
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) => hasCalled = false;
}