using Godot;
using System;
using System.Diagnostics;

public partial class Ck1Lv04 : Node2D
{
    [Export]
    public AnimationPlayer Player;

    [Export]
    public Area2D Area;

    private int triggerCount = 0;

    private void BodyShapeEntered(Rid bodyRid, Node2D body, int bodyShapeIndex, int areaShapeIndex)
    {
        if (body is Keen)
        {
            Area.GetChild(areaShapeIndex).QueueFree();

            triggerCount++;
            Player.Play($"Trigger{triggerCount}");
        }
    }
}
