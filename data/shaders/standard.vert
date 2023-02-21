#version 330 core
layout (location = 0) in vec3 pos;
layout (location = 1) in vec3 normal;
layout (location = 2) in vec3 color;
layout (location = 3) in vec2 uv;

/*world specific*/
uniform mat4 uVPmat;
uniform vec3 uLightPos;

/*Object specific */
uniform mat4 uMmat;
uniform vec3 uColor;

out vec3 vertColor;
out vec3 fragPos;
out vec3 oNormal;

void main()
{
    gl_Position = uVPmat * uMmat * vec4(pos, 1.0);

	fragPos = pos;
	vertColor = 0.4*color +  0.6*uColor;
	oNormal = normal;
} 