// Skin recolouring for sprites and UI images (Progress/Skins.cs): no extra art per skin.
// _Fur multiplies the pale, unsaturated pixels (the pup's white fur and its grey shading); the hat, scarf, cheeks and
// outlines keep their own colour. _Hsv (hue shift 0..1, saturation x, value x) recolours the rest: with _Gate 1 only
// the bright, saturated red and teal pixels (the pup's beanie and scarf; not its navy outline, pink cheeks, gold goggles
// or a prop drawn into a frame), with _Gate 0 every pixel (cart, rope).
// The project renders in Linear space: the masks are measured on gamma values, as the art was painted.
Shader "SealCatapult/Recolor"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite", 2D) = "white" {}
        _Fur ("Fur tint", Color) = (1,1,1,1)
        _Hsv ("Hue shift, saturation, value", Vector) = (0,1,1,0)
        _Gate ("Only bright saturated pixels", Float) = 0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" "PreviewType"="Plane" "CanUseSpriteAtlas"="True" }
        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata { float4 vertex : POSITION; fixed4 color : COLOR; float2 uv : TEXCOORD0; };
            struct v2f { float4 pos : SV_POSITION; fixed4 color : COLOR; float2 uv : TEXCOORD0; };

            sampler2D _MainTex;
            fixed4 _Fur;
            float4 _Hsv;
            float _Gate;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            float3 rgb2hsv(float3 c)
            {
                float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
                float4 p = lerp(float4(c.bg, K.wz), float4(c.gb, K.xy), step(c.b, c.g));
                float4 q = lerp(float4(p.xyw, c.r), float4(c.r, p.yzx), step(p.x, c.r));
                float d = q.x - min(q.w, q.y);
                return float3(abs(q.z + (q.w - q.y) / (6.0 * d + 1e-5)), d / (q.x + 1e-5), q.x);
            }

            float3 hsv2rgb(float3 c)
            {
                float3 p = abs(frac(c.xxx + float3(0.0, 2.0 / 3.0, 1.0 / 3.0)) * 6.0 - 3.0);
                return c.z * lerp(float3(1, 1, 1), saturate(p - 1.0), c.y);
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 c = tex2D(_MainTex, i.uv);
                #ifdef UNITY_COLORSPACE_GAMMA
                float3 g = c.rgb;
                #else
                float3 g = LinearToGammaSpace(c.rgb);
                #endif
                float3 hsv = rgb2hsv(g);
                float pale = 1.0 - smoothstep(0.16, 0.34, hsv.y);
                float3 shifted = hsv2rgb(float3(hsv.x + _Hsv.x, saturate(hsv.y * _Hsv.y), saturate(hsv.z * _Hsv.z)));
                // beanie red sits round hue 0 (wrapping), scarf teal round 0.48
                float red = 1.0 - smoothstep(0.035, 0.07, min(hsv.x, 1.0 - hsv.x));
                float teal = 1.0 - smoothstep(0.07, 0.11, abs(hsv.x - 0.48));
                float cloth = max(red, teal) * smoothstep(0.38, 0.52, hsv.z) * smoothstep(0.5, 0.65, hsv.y);
                float3 rgb = lerp(g, shifted, lerp(1.0, cloth, _Gate));
                #ifndef UNITY_COLORSPACE_GAMMA
                rgb = GammaToLinearSpace(rgb);
                #endif
                rgb = lerp(rgb, rgb * _Fur.rgb, pale);
                c.rgb = rgb;
                c *= i.color;
                c.rgb *= c.a;
                return c;
            }
            ENDCG
        }
    }
}
