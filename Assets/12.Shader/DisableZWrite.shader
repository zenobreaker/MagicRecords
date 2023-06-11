Shader "Custom/DisableZWriteTUT"
{
	SubShader{
		Tags{
			"RednerType" = "Opaqu"
		}

		Pass{
			Zwrite On
		}
	}
}