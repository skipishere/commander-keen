using Godot;
using System.Diagnostics;

namespace CommanderKeen.Scenes.Creatures.Vorticon;

public partial class vorticon_guard : CharacterBody2D, ITakeDamage
{
    [Export]
    internal int Health = 3;
    public const float Speed = 180.0f;
    public const float JumpVelocity = -315.0f;
    private bool isActivated = false;
    private Sprite2D sprite;
    private Timer hitTimer;
    private SignalManager signalManager;
    private VorticonStateMachine stateMachine = new VorticonStateMachine();
    public float lastMovementX = Vector2.Left.X;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        sprite = GetNode<Sprite2D>("Sprite2D");
        hitTimer = GetNode<Timer>("HitTimer");
        signalManager = GetNode<SignalManager>("/root/SignalManager");
        stateMachine = GetNode<VorticonStateMachine>("StateMachine");
    }

    private void HitTimeout()
    {
        (sprite.Material as ShaderMaterial).SetShaderParameter("line_thickness", 0);
    }

    public override void _PhysicsProcess(double delta)
    {
        if (this.Velocity.Normalized().X != 0)
        {
            lastMovementX = this.Velocity.Normalized().X;
        }

        stateMachine.PhysicsProcess(delta, lastMovementX, isActivated);
        MoveAndSlide();
    }

    public override void _Process(double delta)
    {
        // var velocity = Velocity;

        // if (!IsOnFloor())
        // {
        // 	velocity.Y += gravity * (float)delta;
        // }
        // else
        // {
        // 	velocity.Y = JumpVelocity;
        // }
    }

    public void BodyEntered(Node2D body)
    {
        if (body is Keen)
        {
            signalManager.EmitSignal(nameof(SignalManager.KeenDead));
        }
    }

    public void TakeDamage()
    {
        (sprite.Material as ShaderMaterial).SetShaderParameter("line_thickness", 1);
        hitTimer.Start();
        stateMachine.TakeDamage();

        Debug.WriteLine("Vorticon Guard took damage! Health: " + Health);
    }

    public void Kill()
    {
        stateMachine.Kill();
    }

    public void _on_screen_entered()
    {
        Debug.WriteLine("Vorticon Guard on screen");
        isActivated = true;
    }

    public void OnScreenExited()
    {
        Debug.WriteLine("Vorticon Guard off screen");
        isActivated = false;

        if (!Camera.CameraRect.HasPoint(this.GlobalPosition))
        {
            Debug.Print($"Vorticon Guard is out of camera bounds: {this.GlobalPosition}");
            QueueFree();
        }
    }
}
