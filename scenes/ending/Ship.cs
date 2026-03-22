using Godot;
using System;

public partial class Ship : Sprite2D
{
    private GpuParticles2D exhaustParticles;
    public override void _Ready()
    {
        exhaustParticles = GetNode<GpuParticles2D>("ExhaustParticles");
    }

    public void HyperJump()
    {
        exhaustParticles.TrailEnabled = true;
    }
}
