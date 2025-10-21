using System.Diagnostics;
using Godot;

namespace CommanderKeen.Scenes.Creatures.Vorticon;
public partial class JumpState : VorticonBaseState
{
    private const float JumpMultiple = -74;
    public override VorticonStateMachine.VorticonStates StateType => VorticonStateMachine.VorticonStates.Jump;
    private bool aboutToJump = false;

    public override void _Ready()
    {
    }

    private void OnTimerTimeout()
    {
        this.NextState = VorticonStateMachine.VorticonStates.Jump;
    }

    public override void StateInput(InputEvent inputEvent)
    {
    }

    public override void PhysicsProcess(double delta, float lastMovementX)
    {
        if (Character.IsOnWall())
        {
            lastMovementX = -lastMovementX;
        }
        else
        {
            lastMovementX = Character.Velocity.X > 0 ? Vector2.Right.X : Vector2.Left.X;
        }

        if (Character.IsOnFloor() && !aboutToJump)
        {
            this.NextState = VorticonStateMachine.VorticonStates.Thinking;
        }

        AnimationTree.Set("parameters/Jump/blend_position", lastMovementX);
        Character.Velocity = new Vector2(lastMovementX * Speed, Character.Velocity.Y + gravity * (float)delta);
        aboutToJump = false;
    }

    public override void Enter()
    {
        aboutToJump = true;
        // Can jump a half tile or 1 to 6 tiles high
        var jump = random.RandfRange(1, 6) * JumpMultiple;

        Debug.Print("Jump power: " + jump);
        var direction = Character.Velocity.X > 0 ? Vector2.Right.X : Vector2.Left.X;
        AnimationTree.Set("parameters/Jump/blend_position", direction);
        Character.Velocity = new Vector2 { X = direction, Y = jump };
    }
}
