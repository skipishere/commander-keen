using Godot;

public partial class ScoreUI : HBoxContainer
{
    private SignalManager signalManager;
    private Label score;

    public override void _Ready()
    {
        score = GetNode<Label>("Value");
        signalManager = GetNode<SignalManager>("/root/SignalManager");
        signalManager.ScoreChanged += OnScoreChanged;
        signalManager.ResetUi += OnScoreChanged;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        signalManager.ScoreChanged -= OnScoreChanged;
        signalManager.ResetUi -= OnScoreChanged;
    }

    private void OnScoreChanged()
    {
        score.Text = game_stats.Score.ToString("#,0");
    }
}
