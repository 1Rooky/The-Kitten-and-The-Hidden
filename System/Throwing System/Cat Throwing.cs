using FMODUnity;
using UnityEngine;

// The CatThrowing class is responsible for handling the throwing mechanism specific to the cat character.
public class CatThrowing : ThrowingMechanism
{
    [Header("Cat Specific")]
    [SerializeField] private Animator _animator; // Animator component for controlling cat animations

    private int AttackAnimationId; // Hash ID for the attack animation, used for performance optimization
    private float animationPlayTransition = 0.001f; // Transition time for starting the attack animation, set to a very short duration for a quick transition

    [SerializeField] private EventReference _catShoot;

    // Called when the object becomes enabled and active
    protected override void OnEnable()
    {
        base.OnEnable(); // Call the base class OnEnable method

        // Convert the animation name to a hash ID for efficient animation parameter handling
        AttackAnimationId = Animator.StringToHash(GameConstant.CatAnimation.Throwing);

        // Subscribe to the Throw action's started and canceled events in the InputManager
        InputManager.CatActions.Throw.started += StartThrowAction;
        InputManager.CatActions.Throw.canceled += EndThrowAction;
    }

    // Called when the object becomes disabled or inactive
    protected override void OnDisable()
    {
        base.OnDisable(); // Call the base class OnDisable method

        // Unsubscribe from the Throw action's started and canceled events to prevent memory leaks
        InputManager.CatActions.Throw.started -= StartThrowAction;
        InputManager.CatActions.Throw.canceled -= EndThrowAction;
    }

    // Method to handle the throwing action
    protected override void Throw()
    {
        // Determine the initial velocity based on whether the player is inputting a direction
        if (CheckingPlayerInput)
            InitialVelocity = transform.forward * CurrentVelocity; // Forward throw
        else
            InitialVelocity = transform.up * CurrentVelocity + transform.forward * CurrentVelocity; // Upward and forward throw

        base.Throw(); // Call the base class Throw method to perform the throw

        // Play the shooting sound using the audio system
        //AudioSystem.PlayerShooting(AudioSystem.FmodSystem.CatShoot, this.gameObject.transform.position);
        AudioSystem.PlayerShooting(_catShoot, this.gameObject.transform.position);

        // Play the attack animation with a quick transition
        _animator.CrossFade(AttackAnimationId, animationPlayTransition);
    }

    // Method to draw the trajectory of the throw
    protected override void DrawTrajectory(int numP)
    {
        Vector3[] points = new Vector3[numP]; // Array to store the trajectory points
        Vector3 startingPosition = transform.position; // Starting position of the throw
        Vector3 startingVelocity = transform.up * CurrentVelocity + transform.forward * CurrentVelocity; // Initial velocity for the trajectory calculation

        // Calculate each point in the trajectory
        for (int i = 0; i < numP; i++)
        {
            float time = i * TimeBetweenPoints; // Time at each point
            // Calculate the position of the point using the formula for projectile motion
            points[i] = startingPosition + startingVelocity * time + time * time * Physics.gravity / 2f;
        }

        // Set the calculated points to the line renderer to draw the trajectory
        TrajectoryLineRenderer.positionCount = numP;
        TrajectoryLineRenderer.SetPositions(points);
    }
}