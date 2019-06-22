#version 330 core
out vec4 FragColor;

struct Material {
    sampler2D diffuse;
    sampler2D specular;
    float     shininess;
}; 

uniform Material material;

//struct Light {
    // vec3 position; // ʹ�ö����Ͳ�����Ҫ��
    //vec3 direction;

    //vec3 ambient;
    //vec3 diffuse;
    //vec3 specular;
//};

//struct Light {
    //vec3 position;  

    //vec3 ambient;
    //vec3 diffuse;
    //vec3 specular;


//};

struct Light {
    vec3  position;
    vec3  direction;
    float cutOff;
	float outerCutOff;

	vec3 ambient;
    vec3 diffuse;
    vec3 specular;

	float constant;
    float linear;
    float quadratic;
};

uniform Light light;



in vec3 FragPos;
in vec3 Normal;
in vec2 TexCoords;

uniform vec3 objectColor;
uniform vec3 lightColor;
uniform vec3 viewPos;

void main()
{    
	//ambient  *= attenuation; 
	//diffuse  *= attenuation;
	//specular *= attenuation;

	vec3 lightDir = normalize(light.position - FragPos);
	float theta = dot(lightDir, normalize(-light.direction));

	if(theta > light.outerCutOff) 
	{       
		// ִ�й��ռ���
	    // ������
   		vec3 ambient  = light.ambient  * vec3(texture(material.diffuse, TexCoords));

		// ������ 
		vec3 norm = normalize(Normal);
		//vec3 lightDir = normalize(-light.direction);
		float diff = max(dot(norm, lightDir), 0.0);
		vec3 diffuse  = light.diffuse  * diff * vec3(texture(material.diffuse, TexCoords));  

		// �����
		vec3 viewDir = normalize(viewPos - FragPos);
		vec3 reflectDir = reflect(-lightDir, norm);  
		float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
		vec3 specular = light.specular * spec * vec3(texture(material.specular, TexCoords));
		
		float theta     = dot(lightDir, normalize(-light.direction));
		float epsilon   = light.cutOff - light.outerCutOff;
		float intensity = clamp((theta - light.outerCutOff) / epsilon, 0.0, 1.0);    
		// �����Ի���������Ӱ�죬������������һ���
		diffuse  *= intensity;
		specular *= intensity;

		float distance    = length(light.position - FragPos);
		float attenuation = 1.0 / (light.constant + light.linear * distance + light.quadratic * (distance * distance));
		//�����ⲻ��Ҫ˥��
		//ambient  *= attenuation; 
		diffuse   *= attenuation;
        specular *= attenuation; 


		FragColor = vec4(ambient + diffuse + specular, 1.0);
	}
	else {
		// ����ʹ�û����⣬�ó����ھ۹�֮��ʱ��������ȫ�ڰ�
		FragColor = vec4(light.ambient * vec3(texture(material.diffuse, TexCoords)), 1.0);
	} 
		
}
