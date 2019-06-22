#version 330 core
out vec4 FragColor;

in vec3 result;

uniform vec3 objectColor;

void main()
{

    FragColor = vec4(result * objectColor, 1.0);
}