using Godot;
using System.Diagnostics;

public partial class Pause : PanelContainer
{
    [Export]
    private Button saveButton;

    [Export]
    public Control DefaultButton;

    [Export]
    private VBoxContainer mainMenuContainer;

    [Export]
    private Settings settingsMenu;
    private bool disablePauseMenu;

    private SignalManager signalManager;

    public override void _Ready()
    {
        signalManager = GetNode<SignalManager>("/root/SignalManager");
        signalManager.PauseMenu += OnPauseMenu;
        signalManager.TeleportStart += () =>
        {
            disablePauseMenu = true;
        };
        signalManager.TeleportComplete += (position, finished) =>
        {
            if (finished)
            {
                disablePauseMenu = false;
            }
        };
        signalManager.KeenDead += () =>
        {
            disablePauseMenu = true;
        };
        signalManager.ExitingLevel += () =>
        {
            if (!game_stats.GameOver)
            {
                disablePauseMenu = false;
            }
        };
        signalManager.GameOver += () =>
        {
            disablePauseMenu = true;
        };

        settingsMenu.Visible = false;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        signalManager.PauseMenu -= OnPauseMenu;
    }

    private void OnPauseMenu(bool paused, bool showSaveButton)
    {
        if (disablePauseMenu)
        {
            return;
        }

        saveButton.Visible = showSaveButton;
        GetTree().Paused = paused;
        Visible = paused;
        Input.MouseMode = paused ? Input.MouseModeEnum.Visible : Input.MouseModeEnum.Hidden;

        if (paused)
        {
            ShowMainMenu();
        }
    }

    public void ShowMainMenu()
    {
        mainMenuContainer.Visible = true;
        settingsMenu.Visible = false;
        DefaultButton.GrabFocus();
    }

    public void ShowSettings()
    {
        mainMenuContainer.Visible = false;

        settingsMenu.Visible = true;
        settingsMenu.OnVisibilityChanged();
    }

    public void SaveFile()
    {
        var file = FileAccess.Open("user://savegame.save", FileAccess.ModeFlags.WriteRead);

        var savedate = Json.Stringify(game_stats.Save());
        Debug.WriteLine(savedate);
        file.StoreLine(savedate);
        file.Close();
    }

    public void LoadFile()
    {
        if (!FileAccess.FileExists("user://savegame.save"))
        {
            return;
        }

        Debug.WriteLine("File found - attempting to load");

        var file = FileAccess.Open("user://savegame.save", FileAccess.ModeFlags.Read);
        var savedata = file.GetAsText();
        var json = new Json();
        var parseResult = json.Parse(savedata);
        if (parseResult != Error.Ok)
        {
            Debug.WriteLine("Error parsing save file");
            return;
        }
        var nodeData = new Godot.Collections.Dictionary<string, Variant>((Godot.Collections.Dictionary)json.Data);
        game_stats.Load(nodeData);

        // TODO sort out the UI reset
        signalManager.EmitSignal(nameof(SignalManager.ResetUi));

        if (!saveButton.Visible)
        {
            signalManager.EmitSignal(nameof(SignalManager.ExitingLevel));
        }

        Resume();
    }

    public void Quit()
    {
        //TODO add a confirmation dialog
        OnPauseMenu(false, false);
        GetTree().ChangeSceneToFile("res://scenes/levels/ck1-title.tscn");
    }

    public void Resume()
    {
        OnPauseMenu(false, false);
    }
}
