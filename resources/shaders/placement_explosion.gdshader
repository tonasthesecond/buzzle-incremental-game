shader_type particles;

uniform float burst_speed_min = 20.0;
uniform float burst_speed_max = 80.0;
uniform float damping = 3.0;
uniform float particle_size = 16.0;
uniform float linger_start = 0.6; // normalized lifetime when particle stops and fades
uniform float gravity = 30;

float rand(float seed) {
    return fract(sin(seed * 127.1 + 311.7) * 43758.5453);
}

void start() {
    float seed = float(INDEX) * 1.73 + float(NUMBER) * 0.37;

    // burst in all directions
    float angle = (rand(seed) - 0.5) * TAU * 0.8 - PI / 2.0;
    float spd = mix(burst_speed_min, burst_speed_max, rand(seed * 3.17));
    VELOCITY = vec3(cos(angle) * spd, sin(angle) * spd, 0.0);

    // random rotation
    float rot = rand(seed * 13.7) * TAU;
    float s = floor(mix(1.0, 2.0, rand(seed * 7.13))) * particle_size;
    TRANSFORM[0][0] = cos(rot) * s;
    TRANSFORM[0][1] = -sin(rot) * s;
    TRANSFORM[1][0] = sin(rot) * s;
    TRANSFORM[1][1] = cos(rot) * s;
    TRANSFORM[3] = EMISSION_TRANSFORM * vec4(0.0, 0.0, 0.0, 1.0);

    COLOR = vec4(1.0);
	CUSTOM.x = mix(0.3, 1.0, rand(seed * 17.3));
    CUSTOM.y = 0.0;
}

void process() {
    CUSTOM.y += DELTA / (LIFETIME * CUSTOM.x);
	VELOCITY.y += gravity * DELTA;
	if (CUSTOM.y >= linger_start)
	    VELOCITY = vec3(0.0);

    // decelerate to stop
    float speed_scale = 1.0 - smoothstep(0.0, linger_start, CUSTOM.y);
    VELOCITY *= max(1.0 - damping * DELTA, 0.0) * step(CUSTOM.y, linger_start);

    // fade out after linger_start
    float alpha = 1.0 - smoothstep(linger_start, 1.0, CUSTOM.y);
    COLOR.a = alpha;
}