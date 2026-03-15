using Godot;

public partial class UiPanel : PanelContainer
{
    private bool overrideIsActive = false;

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("show_ui") && !overrideIsActive)
        {
            game_stats.ShowUI = !game_stats.ShowUI;
            this.Visible = game_stats.ShowUI;
        }
    }

    public void OverrideUiVisibility(bool visible)
    {
        overrideIsActive = true;
        this.Visible = visible;
    }

    public bool RestoreUiVisibility()
    {
        overrideIsActive = false;
        this.Visible = game_stats.ShowUI;
        return game_stats.ShowUI;
    }
}
