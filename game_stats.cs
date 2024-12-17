using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

[GlobalClass]
public partial class game_stats : Resource
{
    [Flags]
    public enum KeyCards
    {
        Blue = 1,
        Red = 2,
        Green = 4,
        Yellow = 8
    }

    [Flags]
    public enum ShipParts
    {
        Battery = 1,
        JoyStick = 2,
        Vaccum = 4,
        Fuel = 8
    }

    public static ShipParts CollectedParts = 0;
    
    public static int Score = 0;

    public static int Charges = 0;

    public static bool ShowUI = true;

    public static string CurrentLevel = string.Empty;

    public static Vector2? KeenMapPosition;

    public static Dictionary<string, bool> Levels = new();

    public static bool UsedSecretExit = false;

    public static bool HasPogoStick = false;

    public static void Reset()
    {
        Charges = 0;
        CollectedParts = 0;
        KeenMapPosition = null;
        Score = 0;
        HasPogoStick = false;
        Levels.Clear();
    }

    public static void SetPart(ShipParts part)
    {
        CollectedParts |= part;
    }
}
