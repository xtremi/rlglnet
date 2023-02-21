#version 330 core
out vec4 result;

uniform vec3 uLightPos;

in vec3 vertColor;
in vec3 fragPos;
in vec3 oNormal;

void main()
{
	vec3 lightDir = normalize(uLightPos - fragPos);
	float directionFactor = clamp(dot(oNormal, lightDir), 0.2, 1.0);
    float colorFactor = 0.3 + 0.7*directionFactor;

	
	
    result = vec4(colorFactor*vertColor, 1.0);
} 