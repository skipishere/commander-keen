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

    public static PogoStickState HasPogoStick = PogoStickState.No;

    public static int Lives = 5;

    public enum PogoStickState
    {
        /// <summary>
        /// Keen does not have the pogo stick yet.
        /// </summary>
        No,
        /// <summary>
        /// Keen got the pogo stick in the current level so will lose it if they die.
        /// </summary>
        Gained,
        /// <summary>
        /// Keen got the pogo stick in a previous level and still has it.
        /// </summary>
        Keep
    }

    public static void Reset()
    {
        Charges = 0;
        CollectedParts = 0;
        KeenMapPosition = null;
        Score = 0;
        HasPogoStick = PogoStickState.No;
        Levels.Clear();
        Lives = 5;
    }

    public static void SetPart(ShipParts part)
    {
        CollectedParts |= part;
    }
}
