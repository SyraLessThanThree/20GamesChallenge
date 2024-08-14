shader_type canvas_item;

uniform vec2 alphaRange = vec2(.55,.8);

float random( vec2 p )
{
   // e^pi (Gelfond's constant)
   // 2^sqrt(2) (Gelfond–Schneider constant)
     vec2 K1 = vec2( 23.14069263277926, 2.665144142690225 );

   //return fract( cos( mod( 12345678., 256. * dot(p,K1) ) ) ); // ver1
   //return fract(cos(dot(p,K1)) * 123456.); // ver2
     return fract(cos(dot(p,K1)) * 12345.6789); // ver3
}

void fragment() {
	float time = floor(TIME*10.)/3.;
	float time2 = floor(TIME*20.)/3.;
	vec2 moving_uv = vec2(UV.x+time,UV.y);
	vec4 tex = texture(TEXTURE,moving_uv);
	COLOR = tex;
	COLOR.a = (random(vec2(time2,0.)) * (alphaRange.x-alphaRange.y)+alphaRange.x) * tex.a;
}