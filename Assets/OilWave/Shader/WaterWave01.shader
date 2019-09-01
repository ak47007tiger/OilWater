Shader "DC/ShaderDemo/WaterWave01"
{
  Properties
  {
    [HideInInspector]
    _ClickPoint("ClickPoint", Vector)=(0.5,0.5,0,0)
    [KeywordEnum(Toy, Classic)] _Type("Type",float)=0
    _OutDepth("OutDepth", Range(0,100))=4
    _CenterDepth("CenterDepth", Range(0,100))=1
    [HideInInspector]
    _Base("Base", float)=100
    _SizeFactor("SizeFactor", float)=1
    _Atten("Atten",Range(0,1))=0.97
  }

  CGINCLUDE
  
  #include "UnityCG.cginc"
  #include "UnityCustomRenderTexture.cginc"

  #pragma multi_compile _TYPE_TOY _TYPE_CLASSIC

  #define velPropagation 1.0

  float4 _ClickPoint;
  float _OutDepth;
  float _Base;
  float _SizeFactor;
  float _Atten;
  float _CenterDepth;

  float ComputeD(v2f_customrendertexture i, float dInit){
    float2 q = i.globalTexcoord;

    float du = velPropagation / _CustomRenderTextureWidth;
    float dv = velPropagation / _CustomRenderTextureHeight;

    float3 e = float3(du, dv, 0);

    float4 c = tex2D(_SelfTexture2D, q);
    
    float p11 = c.y;
    
    //getting adjacent pixels
    float p10 = tex2D(_SelfTexture2D, q-e.zy).r;
    float p01 = tex2D(_SelfTexture2D, q-e.xz).r;
    float p21 = tex2D(_SelfTexture2D, q+e.xz).r;
    float p12 = tex2D(_SelfTexture2D, q+e.zy).r;

    p11 *= _Base;

    p10 *= _Base;
    p01 *= _Base;
    p21 *= _Base;
    p12 *= _Base;

    float d = dInit;
    #if _TYPE_TOY
      // d +=  ((p10 + p01 + p21 + p12 - 2.) - (p11-.5)*2.);
      d +=  ((p10 + p01 + p21 + p12) - (p11 - 3)*2.);
    #else
      //X0'=（X1+X2+X3+X4）/ 2- X0
      d += (p10 + p01 + p21 + p12) * 0.5 - p11;
      // d += (p10 + p01 + p21 + p12) * 0.25 - p11;
      // d += (p10 + p01 + p21 + p12) * 0.2 + p11*0.25;
      // d = (2 * c.r - c.g + 0.2 * (p10 + p01 + p21 + p12 - 4 * c.r));
    #endif
    // d *= .99; // dampening
    d *= _Atten; // dampening
    // d *= float(iFrame>=2); // clear the buffer at iFrame < 2
    #if _TYPE_TOY
      d = d*.5 + .5;
    #endif
    d /= _Base;
    return d;
  }

  float4 frag (v2f_customrendertexture i) : SV_Target
  {
    float2 q = i.globalTexcoord;

    float d = ComputeD(i,0);
    float4 c = tex2D(_SelfTexture2D, q);

    return float4(d, c.r, 0, 0);
  }

  float4 frag_left_click(v2f_customrendertexture i) : SV_Target
  {
    float2 q = i.globalTexcoord;
    float4 c = tex2D(_SelfTexture2D, q);
    
    float len = length(_ClickPoint.xy - q) * _SizeFactor;
    float d = smoothstep(_CenterDepth / _Base, _OutDepth / _Base, len);
    d = ComputeD(i,d);
    return float4(d,c.r,0,0);
  }
  ENDCG

  SubShader
  {
    Tags { "RenderType"="Opaque" }
    Cull Off ZWrite Off ZTest Always
    LOD 100

    Pass
    {
      Name "Update"
      CGPROGRAM
      #pragma vertex CustomRenderTextureVertexShader
      #pragma fragment frag
      ENDCG
    }

    Pass
    {
      Name "Click"
      CGPROGRAM
      #pragma vertex CustomRenderTextureVertexShader
      #pragma fragment frag_left_click
      ENDCG
    }
  }
}
