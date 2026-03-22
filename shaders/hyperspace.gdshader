shader_type canvas_item;

uniform float stretch = 0.0;
uniform float brightness = 1.0;

void vertex() {
    // Horizontal stretch only
    VERTEX.x *= (1.0 + stretch);
}

void fragment() {
    vec2 uv = UV - vec2(0.5);

    float dist = length(uv);

    // Soft circular falloff
    float alpha = smoothstep(0.5, 0.0, dist);

    vec3 col = vec3(1.0) * brightness;

    COLOR = vec4(col, alpha * brightness);
}
