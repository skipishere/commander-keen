using Godot;

public partial class GameOver : Node2D
{
    public Image FinalScreenshot { get; set; }

    public override void _Ready()
    {
        var textureRect = GetNode<TextureRect>("TextureRect");
        textureRect.Texture = ImageTexture.CreateFromImage(FinalScreenshot);
    }

    public void ReturnToMainMenu()
    {
        GetTree().ChangeSceneToFile("res://scenes/levels/ck1-title.tscn");
    }
}
