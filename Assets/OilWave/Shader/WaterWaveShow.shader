Shader "DC/ShaderDemo/WaterWaveShow"
{
  Properties
  {
    _MainTex ("Texture", 2D) = "white" {}
    _WaveTex("WaveTex",2D)= "black" {}
    _NoiseTex("Noise",2D) = "black" {}
    _NormalTex("Normal",2D) = "black" {}
    _Base("Base",Float)=100

    //亮度调节
    _SpecularK("SpecularK", Float) = 1
    //控制发光区域大小，越小发光区域越大
    _Shininess("Shininess", Float) = 1
  }

  CGINCLUDE
  #include "UnityCG.cginc"

  struct appdata
  {
    float4 vertex : POSITION;
    float2 uv : TEXCOORD0;
    float2 normalUv : TEXCOORD1;
  };

  struct v2f
  {
    float2 uv : TEXCOORD0;
    float2 uv1 : TEXCOORD1;
    float2 normalUv : TEXCOORD2;
    float4 vertex : SV_POSITION;
  };

  sampler2D _MainTex;
  float4 _MainTex_ST;

  float _Base;

  sampler2D _WaveTex;
  float4 _WaveTex_ST;
  float4 _WaveTex_TexelSize;

  sampler2D _NoiseTex;
  sampler2D _NormalTex;
  float4 _NormalTex_ST;

  float _SpecularK;
  float _Shininess;

  v2f vert (appdata v)
  {
    v2f o;
    o.vertex = UnityObjectToClipPos(v.vertex);
    o.uv = TRANSFORM_TEX(v.uv, _MainTex);
    o.uv1 = TRANSFORM_TEX(v.uv, _WaveTex);
    o.normalUv = TRANSFORM_TEX(v.uv, _NormalTex);
    return o;
  }

  fixed4 frag (v2f i) : SV_Target
  {
    float2 q = i.uv;
    float3 e = float3(_WaveTex_TexelSize.xy,0);
    // e.x = 1.0/11520.0;

    float p10 = tex2D(_WaveTex, q-e.zy).x;
    float p01 = tex2D(_WaveTex, q-e.xz).x;
    float p21 = tex2D(_WaveTex, q+e.xz).x;
    float p12 = tex2D(_WaveTex, q+e.zy).x;

    p10 *= _Base;
    p01 *= _Base;
    p21 *= _Base;
    p12 *= _Base;

    float3 grad = normalize(float3(p21 - p01, p12 - p10, 1.));
    fixed4 normal = tex2D(_NormalTex, i.normalUv);
    fixed4 noise = tex2D(_NoiseTex, i.uv);
    float3 viewDir = normalize(_WorldSpaceCameraPos);
    // return fixed4(viewDir,1);
    float3 lightDir = normalize(_WorldSpaceLightPos0);
    // return fixed4(lightDir, 1);
    fixed3 specularCol = _SpecularK * pow(max(dot(normal.xyz, normalize(lightDir + viewDir)), 0), _Shininess);
    // return fixed4(specularCol,1);
    fixed4 c = tex2D(_MainTex, q + grad.xy*.35);
    return fixed4(c.rgb + specularCol, c.a);
    // return c;
  }
  ENDCG

  SubShader
  {
    Tags{"Queue"="Transparent+1"}
    Cull Off ZWrite Off ZTest Always

    Pass
    {
      CGPROGRAM
      #pragma vertex vert
      #pragma fragment frag

      ENDCG
    }
  }
}
