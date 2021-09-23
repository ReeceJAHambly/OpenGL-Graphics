#version 330

uniform sampler2D uTextureSampler;
uniform sampler2D uTextureSampler2;

uniform float uThreshold;

in vec2 oTexCoords;

out vec4 FragColour;

void main()
{
	if(texture(uTextureSampler2, oTexCoords).r < uThreshold)
	{
		discard;
	}
	else
	{
		FragColour = texture(uTextureSampler,oTexCoords);
	}
}