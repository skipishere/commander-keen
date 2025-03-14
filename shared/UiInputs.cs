using Godot;
using System;
using System.Diagnostics;
using System.Linq;

public partial class UiInputs : CanvasLayer
{

    [Export]
    public Label HintLabel;

    private SignalManager signalManager;
    private AnimationPlayer animationPlayer;
    public override void _Ready()
    {
        signalManager = GetNode<SignalManager>("/root/SignalManager");
        animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        signalManager.ShowUiHint += OnShowUiHint;
        // var up = GetKeycode("map_up");
        // var down = GetKeycode("map_down");
        // var left = GetKeycode("move_left");
        // var right = GetKeycode("move_right");
        // var enterLevel = GetKeycode("move_jump");

        //var message = $"Move {GetUiSymbol(up)}{GetUiSymbol(down)}{GetUiSymbol(left)}{GetUiSymbol(right)}\nEnter level {GetUiSymbol(enterLevel)}";
        //var message = $"Move {GetUiSymbol("arrow_keys")}\nEnter level {GetUiSymbol(enterLevel)}\nShow/Hide status bar {GetUiSymbol("Enter")}";
        //HintLabel.Visible = false;
        //HintLabel.Text = InputHelper.GetKeyboardInputForAction("move_jump").AsText();
    }

    private void OnShowUiHint(string message)
    {
        animationPlayer.Stop();
        HintLabel.Text = message;
        animationPlayer.Play("Show");
    }


    public override void _ExitTree()
    {
        base._ExitTree();
        signalManager.ShowUiHint -= OnShowUiHint;
    }
}
