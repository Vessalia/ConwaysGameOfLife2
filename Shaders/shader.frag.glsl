#version 430 core
in vec2 texCoord;

out vec4 fragColor;

uniform sampler2D tex;
uniform vec3 lightColour;

void main()
{
	fragColor = texture(tex, texCoord) * vec4(lightColour, 1.0);
}
