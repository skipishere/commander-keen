using Godot;

public abstract partial class State : Node, IState<StateMachine.KeenStates>
{
    public const float Speed = 180.0f;
    private const float TileSize = 16f;

    public CharacterBody2D Character { get; set; }
    
    private Vector2 shoveVelocity = Vector2.Zero;
    private float shoveStartX = 0f;

    public AnimationTree AnimationTree { get; set; }
	protected bool IsBeingShoved => shoveVelocity != Vector2.Zero;
	protected bool WasRecentlyShoved => (Character as Keen)?.WasRecentlyShoved ?? false;

    public abstract StateMachine.KeenStates StateType { get; }

    public virtual bool CanMove => true;
    public StateMachine.KeenStates? NextState { get; set; }

    internal AnimationNodeStateMachinePlayback playback { get => (AnimationNodeStateMachinePlayback)AnimationTree.Get("parameters/playback"); }

    internal float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

    public virtual void StateInput(InputEvent inputEvent)
    {
    }

    public abstract void PhysicsProcess(double delta, float lastMovementX);

    public virtual void Enter()
    {
    }

    public void Exit()
    {
        NextState = null;
    }
    
    public void Shove(float direction)
    {
        if (!IsBeingShoved)
        {
            shoveVelocity = new Vector2(Mathf.Sign(direction) * Speed, 0);
            shoveStartX = Character.GlobalPosition.X;
            if (Character is Keen keen) keen.WasRecentlyShoved = true;
        }
    }
    
    public void ExtendShove()
    {
        // Restart the shove with the current direction for 1 more tile
        float direction = Mathf.Sign(Character.Velocity.X);
        if (direction == 0) direction = 1; // Fallback if velocity is zero
        shoveVelocity = new Vector2(direction * Speed, 0);
        shoveStartX = Character.GlobalPosition.X;
        if (Character is Keen keen) keen.WasRecentlyShoved = false;
    }
    
    protected void ProcessShove()
    {
        if (!IsBeingShoved) return;
        
        float distanceTraveled = Mathf.Abs(Character.GlobalPosition.X - shoveStartX);
        float targetDistance = (StateType == StateMachine.KeenStates.Air) ? (TileSize * 2) : TileSize;
        
        if (Character.IsOnWall() || distanceTraveled >= targetDistance)
        {
            shoveVelocity = Vector2.Zero;
            Character.Velocity = Character.Velocity with { X = 0 };
            return;
        }
        
        Character.Velocity = Character.Velocity with { X = shoveVelocity.X };
    }

}