using UnityEngine;

// TurretThrowing class inherits from ThrowingMechanism
public class TurretThrowing : ThrowingMechanism
{
    // Called when the object is enabled
    protected override void OnEnable()
    {
        base.OnEnable();

        // Set up the input actions
        InputManager.PossessTurretActions.Fire.started += StartThrowAction;
        InputManager.PossessTurretActions.Fire.canceled += EndThrowAction;
    }

    // Called when the object is disabled
    protected override void OnDisable()
    {
        base.OnDisable();

        // Remove the input actions
        InputManager.PossessTurretActions.Fire.started -= StartThrowAction;
        InputManager.PossessTurretActions.Fire.canceled -= EndThrowAction;
    }

    // Throw the ball
    protected override void Throw()
    {
        // Set the initial velocity for the throw
        InitialVelocity = CurrentVelocity * 4 * transform.forward;

        // Play the shooting sound
        AudioSystem.PlayerShooting(AudioSystem.FmodSystem.TurretShoot, this.gameObject.transform.position);

        base.Throw();
    }

    // Draw the trajectory
    protected override void DrawTrajectory(int numP)
    {
        Vector3[] points = new Vector3[numP];
        Vector3 startingPosition = transform.position;
        Vector3 startingVelocity = transform.forward * CurrentVelocity;

        // Calculate the points for the trajectory
        for (int i = 0; i < numP; i++)
        {
            float time = i * TimeBetweenPoints;
            points[i] = startingPosition + startingVelocity * time;
        }

        // Set the points for the line renderer
        TrajectoryLineRenderer.positionCount = numP;
        TrajectoryLineRenderer.SetPositions(points);
    }
}