Shader "Spine/SkeletonGraphic/SinglePassOutline"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        [Header(Outline Settings)]
        // Bỏ HideInInspector để bạn dễ dàng chỉnh sửa trực tiếp trên Material
        _OutlineWidth("Outline Width", Range(0, 10)) = 2.0
        _OutlineColor("Outline Color", Color) = (1,1,0,1)
        _ThresholdEnd("Outline Threshold", Range(0,1)) = 0.1

        [Header(UI Mask Settings)]
        [Enum(UnityEngine.Rendering.CompareFunction)] _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        [Enum(UnityEngine.Rendering.StencilOp)] _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15
        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Fog { Mode Off }
        
        // Chế độ Blend chuẩn cho Spine Premultiplied Alpha (PMA)
        Blend One OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            Name "SinglePassOutline"
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            fixed4 _Color;
            float4 _ClipRect;

            float _OutlineWidth;
            fixed4 _OutlineColor;
            float _ThresholdEnd;

            v2f vert(appdata_t v)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                OUT.worldPosition = v.vertex;
                OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);
                OUT.texcoord = v.texcoord;
                OUT.color = v.color * _Color;
                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                // 1. Lấy màu gốc của con chim
                half4 texColor = tex2D(_MainTex, IN.texcoord);
                half4 mainColor = texColor * IN.color;

                // 2. Tính toán độ lệch pixel cho viền (Scale theo Texel Size)
                float2 texD = _MainTex_TexelSize.xy * _OutlineWidth;

                // 3. Lấy mẫu 8 hướng xung quanh để tìm mép viền ngoài của ảnh
                half alphaU  = tex2D(_MainTex, IN.texcoord + float2(0, texD.y)).a;
                half alphaD  = tex2D(_MainTex, IN.texcoord + float2(0, -texD.y)).a;
                half alphaR  = tex2D(_MainTex, IN.texcoord + float2(texD.x, 0)).a;
                half alphaL  = tex2D(_MainTex, IN.texcoord + float2(-texD.x, 0)).a;
                
                half alphaUR = tex2D(_MainTex, IN.texcoord + float2(texD.x, texD.y)).a;
                half alphaUL = tex2D(_MainTex, IN.texcoord + float2(-texD.x, texD.y)).a;
                half alphaDR = tex2D(_MainTex, IN.texcoord + float2(texD.x, -texD.y)).a;
                half alphaDL = tex2D(_MainTex, IN.texcoord + float2(-texD.x, -texD.y)).a;

                // Tổng hợp Alpha của vùng viền
                half outlineAlpha = max(max(max(alphaU, alphaD), max(alphaR, alphaL)),
                                        max(max(alphaUR, alphaUL), max(alphaDR, alphaDL)));

                // Định dạng màu viền theo chuẩn Premultiplied Alpha (PMA) của Spine
                half4 outlineCol = _OutlineColor;
                outlineCol.a *= outlineAlpha * IN.color.a;
                outlineCol.rgb *= outlineCol.a; // Ép chuẩn PMA để viền không bị lỗi màu đen

                // 4. KIỂM TRA: Nếu điểm ảnh gốc trong suốt (<= Threshold) NHƯNG xung quanh có màu -> Đây là viền
                half isOutline = step(texColor.a, _ThresholdEnd) * step(0.01, outlineAlpha);

                // Trộn màu: Chỗ nào là viền thì tô màu vàng, ngược lại giữ nguyên màu con chim gốc
                half4 finalColor = lerp(mainColor, outlineCol, isOutline);

                // --- Xử lý mặt nạ (Mask/ScrollRect) cho giao diện UI ---
                #ifdef UNITY_UI_CLIP_RECT
                finalColor.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
                #endif

                #ifdef UNITY_UI_ALPHACLIP
                clip (finalColor.a - 0.001);
                #endif

                return finalColor;
            }
            ENDCG
        }
    }
}