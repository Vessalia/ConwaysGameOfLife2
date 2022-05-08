#version 430 core
in vec2 texCoord;
in vec3 normal;
in vec3 fragPos;

out vec4 fragColor;

struct Material 
{
    sampler2D diffuse;
    sampler2D specular;
    float shininess;
};
struct Light 
{
    vec3 position;
  
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};


uniform Light light;  
uniform Material material;
uniform sampler2D tex;
uniform vec3 viewPos;

void main()
{
	vec3 norm = normalize(normal);
	vec3 lightDir = normalize(light.position - fragPos);  
	float proj = max(dot(norm, lightDir), 0.0);
	vec3 diffuse = light.diffuse * proj * vec3(texture(material.diffuse, texCoord));
	
	vec3 viewDir = normalize(viewPos - fragPos);
	vec3 reflectDir = reflect(-lightDir, norm);
	float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
	vec3 specular = light.specular * spec * vec3(texture(material.specular, texCoord));

	vec3 ambientLighting = light.ambient * vec3(texture(material.diffuse, texCoord));

	fragColor = texture(tex, texCoord) * vec4(ambientLighting + diffuse + specular, 1.0);
}
