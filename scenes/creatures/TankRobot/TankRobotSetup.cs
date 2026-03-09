using Godot;

[Tool]
public partial class TankRobotSetup : Path2D
{
    private float startPosition = 0;

    [Export(PropertyHint.Range, "0,1,")]
    public float StartPosition
    {
        get => startPosition;
        set
        {
            startPosition = value;
            var pathFollow2D = GetNode<PathFollow2D>("PathFollow2D");
            pathFollow2D.Progress = startPosition * this.Curve.GetBakedLength();
            this.QueueRedraw();
        }
    }

    public override void _EnterTree()
    {
        //var pathFollow2D = GetNode<PathFollow2D>("PathFollow2D");
        //pathFollow2D.Progress = startPosition * this.Curve.GetBakedLength();
   
        base._EnterTree();
    }
}
