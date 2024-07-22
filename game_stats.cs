using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class game_stats : Resource
{
    [Export]
    public static int Score = 0;

    [Export]
    public static int Charges = 0;

    public static Vector2? KeenMapPosition;

    public static Dictionary<string, bool> Levels = new();

    public static bool UsedSecretExit = false;

    public static void Reset()
    {
        Charges = 0;
        KeenMapPosition = null;
        Levels.Clear();
    }

    [Flags]
    public enum KeyCards
    {
        Blue = 1,
        Red = 2,
        Green = 4,
        Yellow = 8
    }
}
