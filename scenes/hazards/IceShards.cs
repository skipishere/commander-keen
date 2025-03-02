using Godot;

public partial class IceShards : GpuParticles2D
{
	private Vector2[] directions;
	public override void _Ready()
	{
        this.directions = [
            new Vector2(1, 1).Normalized(),
            new Vector2(-1, 1).Normalized(),
            new Vector2(-1, -1).Normalized(),
            new Vector2(1, -1).Normalized()
        ];
	}

	public void Shatter()
	{
		uint emitFlags = (uint)EmitFlags.Velocity;
        Emitting = true;

        for (int i = 0; i < directions.Length; i++)
        {
            EmitParticle(Transform2D.Identity, directions[i] * 400, new Color(1, 1, 1), new Color(), emitFlags);
        }
	}
}
