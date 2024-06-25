using Godot;
using System;

[GlobalClass]
public partial class game_stats : Resource
{
    [Export]
    public int Score = 0;

    [Export]
    public static int Charges = 5;
}
