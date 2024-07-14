using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class game_stats : Resource
{
    [Export]
    public int Score = 0;

    [Export]
    public static int Charges = 5;

    public static Vector2? KeenMapPosition;

    public static Dictionary<string, bool> Levels = new();
}
