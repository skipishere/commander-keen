shader_type canvas_item;
render_mode blend_mix;

// Per-instance uniforms
instance uniform vec4 MaskColour : source_color;  // Color to trigger glow
instance uniform vec4 GlowColour : source_color;  // Glow color
instance uniform float Threshold : hint_range(0.001, 0.05) = 0.003; // Channel tolerance
instance uniform float GlowIntensity : hint_range(0.0, 5.0) = 1.5; // Glow strength

void fragment() {
    vec3 sprite_col = COLOR.rgb;

    // Compute per-channel absolute difference from mask color
    vec3 diff = abs(sprite_col - MaskColour.rgb);

    // Glow mask: 1.0 if all channels within threshold, 0.0 otherwise
    float glow_mask = float(all(lessThan(diff, vec3(Threshold))));

    // Multiply by alpha so transparent pixels don't glow
    vec3 glow = GlowColour.rgb * glow_mask * GlowIntensity * COLOR.a;

    // Additive glow
    COLOR.rgb = sprite_col + glow;
    COLOR.a = COLOR.a;
}