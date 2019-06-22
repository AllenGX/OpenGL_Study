#version 330 core
out vec4 FragColor;

in vec2 TexCoords;
in vec3 Normal;
in vec3 Position;

struct Material {
    float     shininess;
}; 


//点光源
struct PointLight {
    vec3 position;

    float constant;
    float linear;
    float quadratic;

    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};  
#define NR_POINT_LIGHTS 4
uniform PointLight pointLights[NR_POINT_LIGHTS];
uniform Material material;
uniform sampler2D texture_diffuse1;
uniform sampler2D texture_reflection1;
uniform vec3 objectColor;
uniform vec3 lightColor;
uniform vec3 cameraPos;
uniform samplerCube skybox;

vec3 CalcPointLight(PointLight light, vec3 normal, vec3 fragPos, vec3 viewDir)
{
    vec3 lightDir = normalize(light.position - fragPos);
    // 漫反射着色
    float diff = max(dot(normal, lightDir), 0.0);
    // 镜面光着色
    vec3 reflectDir = reflect(-lightDir, normal);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
    // 衰减
    float distance    = length(light.position - fragPos);
    float attenuation = 1.0 / (light.constant + light.linear * distance + 
                 light.quadratic * (distance * distance));    
    // 合并结果
    vec3 ambient = light.ambient * texture(texture_diffuse1, TexCoords).rgb;
    vec3 diffuse  = light.diffuse  * diff * vec3(texture(texture_diffuse1, TexCoords));
    vec3 specular = light.specular * spec * texture(texture_diffuse1, TexCoords).rgb; 
    ambient  *= attenuation;
    diffuse  *= attenuation;
	specular *= attenuation;  
    return (ambient + diffuse+specular);
}

void main()
{    
	
	vec3 norm = normalize(Normal);
    vec3 viewDir = normalize(cameraPos - Position);
	vec3 result;
	for(int i = 0; i < NR_POINT_LIGHTS; i++)
        result += CalcPointLight(pointLights[i], norm, Position, viewDir); 
	//镜面反射
	//float ratio = 1.00 / 1.52;
	vec4 diffuse_color =texture(texture_diffuse1, TexCoords);
    vec3 I = normalize(Position - cameraPos);
	//vec3 R = refract(I, normalize(Normal), ratio);
    vec3 R = reflect(I, normalize(Normal));
	float reflect_intensity = texture(texture_reflection1, TexCoords).r;
	vec4 reflect_color;
	if(reflect_intensity > 0.1) // Only sample reflections when above a certain treshold
		reflect_color = texture(skybox, R) * reflect_intensity;
	FragColor =  diffuse_color + reflect_color+vec4(result,1.0);
}

