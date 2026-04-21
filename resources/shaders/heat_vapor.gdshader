shader_type particles;

uniform float emit_radius = 50.0;
uniform float speed_min = 10.0;
uniform float speed_max = 30.0;
uniform float scale_min = 1.0;
uniform float scale_max = 3.0;
uniform float particle_size = 16.0;

float rand(float seed) {
    return fract(sin(seed * 127.1 + 311.7) * 43758.5453);
}

void start() {
    float seed = float(INDEX) * 1.73 + float(NUMBER) * 0.37;

    float angle = rand(seed) * TAU;
    float r = sqrt(rand(seed * 2.39)) * emit_radius;
    TRANSFORM[3].x = cos(angle) * r;
    TRANSFORM[3].y = sin(angle) * r;
    TRANSFORM[3].z = 0.0;
	
    float spd = mix(speed_min, speed_max, rand(seed * 3.17));
    VELOCITY = vec3(
        (rand(seed * 5.31) * 2.0 - 1.0) * spd * 0.3,
        -spd,
        0.0
    );

    float s = floor(mix(scale_min, scale_max, rand(seed * 7.13))) * particle_size;
	float rot = rand(seed * 13.7) * TAU;
	TRANSFORM[0][0] = cos(rot) * s;
	TRANSFORM[0][1] = -sin(rot) * s;
	TRANSFORM[1][0] = sin(rot) * s;
	TRANSFORM[1][1] = cos(rot) * s;

    COLOR = vec4(1.0);
    CUSTOM.y = 0.0;
	
	vec3 offset = vec3(cos(angle) * r, sin(angle) * r, 0.0);
	TRANSFORM[3] = EMISSION_TRANSFORM * vec4(offset, 1.0);
}

void process() {
    CUSTOM.y += DELTA / LIFETIME;
    float alpha = smoothstep(0.0, 0.15, CUSTOM.y) * (1.0 - smoothstep(0.7, 1.0, CUSTOM.y));
    COLOR.a = alpha;
    VELOCITY.x += sin(CUSTOM.y * TAU * 2.0 + float(INDEX)) * DELTA * 4.0;
}