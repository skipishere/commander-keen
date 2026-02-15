shader_type canvas_item;
render_mode blend_mix;

uniform sampler2D GlowMask;              // emission mask
uniform vec4 GlowColour : source_color;  // glow colour
uniform float GlowIntensity : hint_range(0.0, 10.0) = 2.5;

void fragment() {
    // This is already: texture * modulate * scene modulation
    vec4 base = COLOR;

    // Sample glow mask (use alpha or red channel)
    float mask = texture(GlowMask, UV).a;

    // Additive emission (NOT modulated again)
    vec3 glow = GlowColour.rgb * mask * GlowIntensity;

    COLOR.rgb = base.rgb + glow;
    COLOR.a   = base.a;
}