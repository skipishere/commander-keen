shader_type canvas_item;
uniform float desaturate : hint_range(0, 1) = 0.0;
uniform float black_fade : hint_range(0.0, 1.0) = 0.0;

void fragment() {
    vec4 tex_color = texture(TEXTURE, UV);
    
    // Rec. 709 weights (Human eye perceives Green as brightest)
    float luminance = dot(tex_color.rgb, vec3(0.2126, 0.7152, 0.0722));
    vec3 mixed_color = mix(tex_color.rgb, vec3(luminance), desaturate);
    
    // We mix our current color with pure black (vec3(0.0))
    COLOR.rgb = mix(mixed_color, vec3(0.0), black_fade);
    COLOR.a = tex_color.a;
}