using Godot;

public partial class TankRobotSetup : Path2D
{
    [Export]
    private float startPosition = 0;

    public override void _Ready()
    {
    }

    public override void _EnterTree()
    {
        var pathFollow2D = GetNode<PathFollow2D>("PathFollow2D");
        pathFollow2D.Progress = startPosition;
        
        base._EnterTree();
    }

}
