using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

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

    public static int NextKeenScore = 20000;

    public static int Charges = 0;

    public static bool ShowUI = true;

    public static string CurrentLevel = string.Empty;

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
        Score = 0;
        HasPogoStick = PogoStickState.No;
        Levels.Clear();
        Lives = 5;
    }

    public static void SetPart(ShipParts part)
    {
        CollectedParts |= part;
    }

    public static Godot.Collections.Dictionary<string, Variant> Save()
    {
        var dict = new Godot.Collections.Dictionary<string, Variant>
        {
            { "score", Score },
            { "next_keen_score", NextKeenScore },
            { "charges", Charges },
            { "show_ui", ShowUI },
            { "current_level", CurrentLevel },
            { "keen_map_position_x", OverworldKeen.mapPosition?.X.ToString()  },
            { "keen_map_position_y", OverworldKeen.mapPosition?.Y.ToString() },
            { "levels", Levels.Select(x => $"{x.Key.Replace("res://scenes/", "")}={x.Value}").Aggregate((x, y) => $"{x},{y}") },
            { "lives", Lives },
            { "collected_parts", (int)CollectedParts },
            { "has_pogo_stick", (int)HasPogoStick }
        };

        return dict;
    }

    public static void Load(Godot.Collections.Dictionary<string, Variant> dict)
    {
        Score = (int)dict["score"];
        NextKeenScore = (int)dict["next_keen_score"];
        Charges = (int)dict["charges"];
        ShowUI = (bool)dict["show_ui"];
        CurrentLevel = (string)dict["current_level"];
        OverworldKeen.mapPosition = new Vector2(float.Parse((string)dict["keen_map_position_x"]), float.Parse((string)dict["keen_map_position_y"]));
        Levels = dict["levels"].ToString().Split(',').ToDictionary(x => "res://scenes/" + x.Split('=')[0], x => bool.Parse(x.Split('=')[1]));
        Lives = (int)dict["lives"];
        CollectedParts = (ShipParts)(int)dict["collected_parts"];
        HasPogoStick = (PogoStickState)(int)dict["has_pogo_stick"];
    }
}
