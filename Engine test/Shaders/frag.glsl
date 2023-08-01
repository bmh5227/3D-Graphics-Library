#version 330
out vec4 outputColor;

in vec2 texPos;
uniform sampler2D texture0;

void main()
{
    outputColor = texture(texture0, texPos);
    //outputColor = vec4(vertColor, 1.0);
}