#version 430 core
in vec3 aPosition;
in vec2 aTexCoord;
in vec3 aNormal;

out vec2 texCoord;
out vec3 normal;
out vec3 fragPos;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

void main()
{
	gl_Position = projection * view * model * vec4(aPosition, 1.0);
	texCoord = aTexCoord;
	normal = mat3(transpose(inverse(model))) * aNormal;
    fragPos = vec3(model * vec4(aPosition, 1.0));
}
