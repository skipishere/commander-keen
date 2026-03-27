shader_type canvas_item;
render_mode blend_add;

uniform sampler2D GlowAtlas;      // glow mask sheet
uniform int FrameIndex;           // current frame
uniform vec4 GlowColor : source_color = vec4(1,0,0,1);
uniform float GlowIntensity : hint_range(0, 10) = 2.5;
uniform vec2 HVRatio;             // Hframes / Vframes ratio

void fragment() {
    // HVRatio.x = number of horizontal frames
    // HVRatio.y = number of vertical frames
    float cols = HVRatio.x;
    float rows = HVRatio.y;

    float row = floor(float(FrameIndex) / cols);
    float col = float(FrameIndex) - row * cols;

    // Compute UV offset for current frame
    vec2 frameUV = UV / vec2(cols, rows) + vec2(col/cols, row/rows);

    float mask = texture(GlowAtlas, frameUV).a;
    vec3 glow = GlowColor.rgb * mask * GlowIntensity;

    COLOR.rgb = glow;
    COLOR.a = mask;
}