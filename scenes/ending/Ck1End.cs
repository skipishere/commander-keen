using Godot;

public partial class Ck1End : Node2D
{
    [Export]
    private AnimationPlayer actionPlayer;

    public override void _Ready()
    {
        actionPlayer.Queue("Keen rushes back");
        actionPlayer.Queue("Shh honey");
        actionPlayer.Queue("Billy are you");
        actionPlayer.Queue("Can I keep");
        actionPlayer.Queue("Talk in morning");
        actionPlayer.Queue("Goodnight");
        actionPlayer.Queue("No sleep");
        actionPlayer.Queue("Finish");
    }

    public override void _Process(double delta)
    {
        //Debug.WriteLine(string.Join(" ,",actionPlayer.GetQueue()));
        // This scene is just a cutscene, so we don't need to do anything here.
    }
}
