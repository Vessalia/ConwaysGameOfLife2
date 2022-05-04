#version 430 core
in vec2 texCoord;
in vec3 normal;
in vec3 fragPos;

out vec4 fragColor;

uniform sampler2D tex;
uniform vec3 lightColour;
uniform vec3 lightPos;
uniform vec3 viewPos;

void main()
{
	vec3 norm = normalize(normal);
	vec3 lightDir = normalize(lightPos - fragPos);  
	float proj = max(dot(norm, lightDir), 0.0);
	vec3 diffuse = proj * lightColour;
	
	float specularStrength = 0.5;
	vec3 viewDir = normalize(viewPos - fragPos);
	vec3 reflectDir = reflect(-lightDir, norm);
	float spec = pow(max(dot(viewDir, reflectDir), 0.0), 64);
	vec3 specular = specularStrength * spec * lightColour;

	float ambientStrength = 0.1;
	vec3 ambientLighting = ambientStrength * lightColour;

	fragColor = texture(tex, texCoord) * vec4(ambientLighting + diffuse + specular, 1.0);
}
