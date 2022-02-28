#version 430 core
out vec4 fragColour;

uniform sampler2D state;
uniform vec2 size;

float get(int x, int y) 
{
    vec2 colour = gl_FragCoord.xy;
    colour += vec2(x, y);
    vec2 uv = colour / size;
    return texture2D(state, uv).r;
}

void main() 
{
    int sum = int(get(-1, -1) + get(-1,  0) + get(-1,  1) +
                  get( 0, -1) +               get( 0,  1) +
                  get( 1, -1) + get( 1,  0) + get( 1,  1));
    if (sum == 3) 
    {
        fragColour = vec4(1.0, 0.0, 0.0, 1.0);
    }
    else if (sum == 2)
    {
        float current = float(get(0, 0));
        fragColour = vec4(current, 0.0, 0.0, 1.0);
    }
    else
    {
        fragColour = vec4(0.0, 0.0, 0.0, 1.0);
    }
}
