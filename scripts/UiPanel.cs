using Godot;

public partial class UiPanel : PanelContainer
{
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("show_ui"))
		{
			game_stats.ShowUI = !game_stats.ShowUI;
			this.Visible = game_stats.ShowUI;
		}
	}
}
