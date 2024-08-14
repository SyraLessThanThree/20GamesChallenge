shader_type canvas_item;
render_mode world_vertex_coords;

// --- Uniforms --- //
uniform vec2 speed = vec2(0.0, 0.0);
uniform sampler2D screen_texture : hint_screen_texture, filter_nearest;
uniform bool useScreen = false;

uniform vec2 node_pos;
uniform vec2 rect_size;

varying vec2 pixel_world_pos;

void vertex(){
	pixel_world_pos = (MODEL_MATRIX * vec4(VERTEX,0.0, 1.0)).xy;
}

// --- Functions --- //
void fragment() {
	if(useScreen){
		vec2 uv_ = mod(pixel_world_pos-node_pos,rect_size)/rect_size;
    	COLOR = textureLod(screen_texture, mod(uv_ + TIME * speed, 1.0),0.0);
	}else{
    	COLOR = texture(TEXTURE, mod(UV + TIME * speed, 1.0));
	}
}