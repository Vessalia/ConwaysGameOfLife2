#version 430 core
in vec2 texCoord;
in vec3 normal;
in vec3 fragPos;

out vec4 fragColor;

uniform sampler2D tex;
uniform vec3 lightColour;
uniform vec3 lightPos;

void main()
{
	vec3 norm = normalize(normal);
	vec3 lightDir = normalize(lightPos - fragPos);  
	float proj = max(dot(norm, lightDir), 0.0);
	vec3 diffuse = proj * lightColour;

	float ambientStrength = 0.3;
	vec3 ambientLighting = ambientStrength * lightColour;

	fragColor = texture(tex, texCoord) * vec4(ambientLighting + diffuse, 1.0);
}
