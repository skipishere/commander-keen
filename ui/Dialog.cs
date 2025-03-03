using Godot;
using System.Diagnostics;

public partial class Dialog : Control
{
	private SignalManager signalManager;

	public override void _Ready()
	{
		signalManager = GetNode<SignalManager>("/root/SignalManager");
		signalManager.ShowUi += ShowUi;
	}

    public override void _ExitTree()
    {
        base._ExitTree();
		signalManager.ShowUi -= ShowUi;
    }

    private void ShowUi(string title, string message)
    {
		Debug.WriteLine($"Showing message: {title} {message}");
		GetNode<Label>("PannelContainer/VBoxContainer/Title").Text = title;
		GetNode<Label>("PannelContainer/VBoxContainer/Message").Text = message;
        this.Visible = true;
		GetTree().Paused = true;
    }

    public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("show_ui") && this.Visible)
		{
			this.Visible = false;
			GetTree().Paused = false;
		}
	}
}
