shader_type canvas_item;
uniform bool enable = true;

void fragment() {
	vec4 tex_color = texture(TEXTURE, UV);
    if (enable)
	{
		float grey = tex_color.r * 0.299 + tex_color.g * 0.587 + tex_color.b * 0.114;
		COLOR = vec4(vec3(grey), tex_color.a*.2);
	}
	else
	{
		COLOR = tex_color;
	}
}