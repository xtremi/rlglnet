#version 330 core
layout (location = 0) in vec3 pos;
layout (location = 1) in vec3 normal;
layout (location = 2) in vec3 tangent;
layout (location = 3) in vec3 color;
layout (location = 4) in vec2 uv;

uniform vec3 uColor;
uniform mat4 uVPmat;
uniform vec3 uLightPos;

out vec3 vertColor;

const vec3 lightPos = vec3(150.0, 0.0, 150.0);

void main()
{
    gl_Position = uVPmat * vec4(pos.x, pos.y, pos.z, 1.0);

    float distanceToLight = distance(pos.xyz, uLightPos);
    float distanceFactor = clamp(1.0 - distanceToLight/250.0, 0.2, 1.0);
    
    vec3 lightDir = normalize(uLightPos - pos);
    float directionFactor = clamp(dot(normal, lightDir), 0.2, 1.0);
    
    float colorFactor = 0.2 + 0.4*directionFactor * 0.4*distanceFactor;

    vertColor = colorFactor * uColor;
}