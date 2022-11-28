#version 330
layout(location = 0) in vec2 i_pos;
uniform mat4 u_model;
uniform mat4 u_projection;
void main()
{
gl_Position = u_projection * u_model * vec4(i_pos, -1.0f, 1.0f);
    
}

