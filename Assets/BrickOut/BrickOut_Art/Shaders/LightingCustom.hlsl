half3 CaculateLighting(half3 mainColor, half4 shadowColor, half ambient, half3 worldNormal, half3 Directional, half shadow)
{
	half lighting = dot(normalize(Directional),normalize(worldNormal));
	lighting = smoothstep((1-shadowColor.a)*0.5,shadowColor.a*0.5+0.5,lighting);
	lighting*=shadow;
	lighting -=(1-ambient);
	return lerp(shadowColor.rgb,mainColor,lighting);
}

half AngleBetweenVectors(float3 a, float3 b)
{
    float3 na = normalize(a);
    float3 nb = normalize(b);
    
    half dotProduct = dot(na, nb);
    dotProduct = clamp(dotProduct, -1.0, 1.0);
    
    half angleRad = acos(dotProduct);
    return degrees(angleRad);
}

half Specular(half3 Directional, half3 worldNormal)
{
	float4x4 viewMat = GetWorldToViewMatrix();
	half3 viewDir = viewMat[2].xyz;
	half3 refVec = reflect(normalize(-Directional), normalize(worldNormal));
	half t = saturate(dot(normalize(refVec), normalize(viewDir)));
	t= pow(t,15);
	half3 direct = mul((float3x3)unity_ObjectToWorld, float3(0,1,0));
	half angle = AngleBetweenVectors(Directional,direct);
	half strength = 1;
	if (angle<45)
	{
		strength = angle/45;
	}

	return smoothstep(0.2,0.8,t)*strength;
}