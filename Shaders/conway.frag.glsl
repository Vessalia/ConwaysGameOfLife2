#version 430 core
in vec2 texCoord;

out vec4 fragColour;

uniform sampler2D state;
uniform vec2 size;

vec4 getColour(int x, int y) 
{
    vec2 uv = texCoord;
    vec2 step = vec2(x, y) / size;
    uv += step;
    clamp(uv.x, 0.0, 1.0);
    clamp(uv.y, 0.0, 1.0);
    return texture2D(state, uv);
}

float get(int x, int y)
{
    vec4 colour = getColour(x, y);
    return round(colour.r / 3 + colour.g / 3 + colour.b / 3);
}

void main() 
{
    int sum = int(get(-1,  1) + get( 0,  1) + get( 1,  1) +
                  get(-1,  0) +               get( 1,  0) +
                  get(-1, -1) + get( 0, -1) + get( 1, -1));
    if (sum == 3) 
    {
        vec4 neighbours[3];
        int index = 0;
        for(int i = -1; i <= 1; i++)
        {
            for(int j = -1; j <= 1; j++)
            {
                if(i == 0 && j == 0) 
                {
                    continue;
                }
                else if(get(i, j) == 1)
                {
                    neighbours[index++] = getColour(i, j);
                }
            }
        }

        vec4 dominantColour;
        vec4 sumColour;
        bool dominantColourFound = false;
        for(int i = 0; i < neighbours.length; i++)
        {
            sumColour += neighbours[i];

            if(dominantColour == neighbours[i] && !dominantColourFound)
            {
                dominantColourFound = true;
            }
            else if (!dominantColourFound)
            {
                dominantColour = neighbours[i];
            }
        }

        sumColour /= 3;

        if(!dominantColourFound)
        {
            dominantColour = sumColour;
        }

        fragColour = dominantColour;
    }
    else if (sum == 2)
    {
        fragColour = vec4(getColour(0, 0).rgb, 1.0);
    }
    else
    {
        fragColour = vec4(0.0, 0.0, 0.0, 1.0);
    }
}
