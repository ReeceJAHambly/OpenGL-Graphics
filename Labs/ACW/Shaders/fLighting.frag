﻿#version 330

struct LightProperties 
{
vec4 Position;
vec3 AmbientLight;
vec3 DiffuseLight;
vec3 SpecularLight;
float CutoffAngle;
vec3 SpotDirection;
int LightType;
};

uniform LightProperties uLight[3];

struct MaterialProperties 
{
vec3 AmbientReflectivity;
vec3 DiffuseReflectivity;
vec3 SpecularReflectivity;
float Shininess;
};

uniform MaterialProperties uMaterial;

uniform vec4 uEyePosition;
in vec4 oNormal;
in vec4 oSurfacePosition;
out vec4 FragColour;
void main()
{
	//vec4 lightDir = normalize(uLight.Position - oSurfacePosition);

	vec4 eyeDirection = normalize(uEyePosition - oSurfacePosition);

	FragColour = vec4(0,0,0,1);

	for(int i = 0; i < 3; ++i)
	{
		bool inCone = false;
		vec4 lightDir = normalize(uLight[i].Position - oSurfacePosition);
		vec4 reflectedVector = reflect(-lightDir, oNormal);

		float diffuseFactor = max(dot(oNormal, lightDir), 0);
		float specularFactor = pow(max(dot( reflectedVector, eyeDirection), 0.0), uMaterial.Shininess * 128);
		

		if(uLight[i].LightType ==1 && degrees(acos(dot(lightDir.xyz, uLight[i].SpotDirection))) < uLight[i].CutoffAngle)
		{
			inCone = true;
		}

		FragColour = FragColour + vec4(uLight[i].AmbientLight * uMaterial.AmbientReflectivity, 1);

		if(uLight[i].LightType != 1 || inCone)
		{
			FragColour = FragColour + vec4(uLight[i].DiffuseLight * uMaterial.DiffuseReflectivity * diffuseFactor, 1);
		}

		if(uLight[i].LightType != 1 || inCone)
		{
			FragColour = FragColour + vec4(uLight[i].SpecularLight * uMaterial.SpecularReflectivity * specularFactor, 1);
		}

		
		//FragColour = FragColour + vec4(uLight[i].AmbientLight * uMaterial.AmbientReflectivity + uLight[i].DiffuseLight * uMaterial.DiffuseReflectivity * diffuseFactor + uLight[i].SpecularLight * uMaterial.SpecularReflectivity * specularFactor, 0);
	}

	
}


