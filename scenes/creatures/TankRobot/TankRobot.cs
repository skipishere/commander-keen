using System.Diagnostics;
using Godot;

public partial class TankRobot : AnimatableBody2D
{
    public float Direction = 1f;
    private TankRobotStateMachine stateMachine = new();
    public float lastMovementX = Vector2.Left.X;

    public override void _Ready()
    {
        stateMachine = GetNode<TankRobotStateMachine>("StateMachine");
    }

    public override void _PhysicsProcess(double delta)
    {
        stateMachine.PhysicsProcess(delta, lastMovementX, true);
    }

    public void BodyEntered(Node2D body)
    {
        if (body is raygunShot shot)
        {
            Debug.Print($"TankRobot collided with {body.Name}");
            shot.TakeDamage();
        }
    }
}
