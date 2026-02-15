shader_type canvas_item;
render_mode blend_add, unshaded;

uniform vec4 GlowColor : source_color = vec4(1,0,0,1);
uniform float GlowIntensity : hint_range(0.0,5.0) = 2.0;

void fragment() {
    vec4 mask_col = texture(TEXTURE, UV);
    float mask = mask_col.a; // alpha = 1 where glow, 0 elsewhere
    vec3 glow = GlowColor.rgb * mask * GlowIntensity;
    COLOR = vec4(glow, mask);
}