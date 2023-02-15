#version 330 core
layout (location = 0) in vec2 posXY;
layout (location = 1) in vec2 uv;

layout (location = 2) in float posZ;
layout (location = 3) in vec3 color;
layout (location = 4) in vec3 normal;
layout (location = 5) in vec3 tangent;


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
    vec3 pos = vec3(posXY.x, posXY.y, posZ);
    gl_Position = uVPmat * uMmat * vec4(pos, 1.0);

	fragPos = pos;
	vertColor = 0.7*color +  0.3*uColor;
	oNormal = normal;
		
    /*float distanceToLight = distance(pos.xyz, uLightPos);
    float distanceFactor = clamp(1.0 - distanceToLight/250.0, 0.2, 1.0);
    
    vec3 lightDir = normalize(uLightPos - pos);
    float directionFactor = clamp(dot(normal, lightDir), 0.2, 1.0); 
    float colorFactor = 0.3 + 0.7*directionFactor;// * 0.25*distanceFactor;*/

    //vertColor = colorFactor * color;
    
} 