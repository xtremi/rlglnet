#version 330 core
out vec4 result;

in vec3 vertColor;


void main()
{
    result = vec4(vertColor, 1.0);
} 