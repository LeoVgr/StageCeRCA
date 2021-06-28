#ifndef MYPERLIN_TILE
#define MYPERLIN_TILE


#define OCTAVES 4


//to 1d functions

//get a scalar random value from a 3d value
float rand3dTo1d(float3 value, float3 dotDir = float3(12.9898, 78.233, 37.719)) {
	//make value smaller to avoid artefacts
	float3 smallValue = sin(value);
	//get scalar value from 3d vector
	float random = dot(smallValue, dotDir);
	//make value more random by making it bigger and then taking the factional part
	random = frac(sin(random) * 143758.5453);
	return random;
}

float rand2dTo1d(float2 value, float2 dotDir = float2(12.9898, 78.233)) {
	float2 smallValue = sin(value);
	float random = dot(smallValue, dotDir);
	random = frac(sin(random) * 143758.5453);
	return random;
}

float rand1dTo1d(float3 value, float mutator = 0.546) {
	float random = frac(sin(value + mutator) * 143758.5453);
	return random;
}

//to 2d functions

float2 rand3dTo2d(float3 value) {
	return float2(
		rand3dTo1d(value, float3(12.989, 78.233, 37.719)),
		rand3dTo1d(value, float3(39.346, 11.135, 83.155))
		);
}

float2 rand2dTo2d(float2 value) {
	return float2(
		rand2dTo1d(value, float2(12.989, 78.233)),
		rand2dTo1d(value, float2(39.346, 11.135))
		);
}

float2 rand1dTo2d(float value) {
	return float2(
		rand2dTo1d(value, 3.9812),
		rand2dTo1d(value, 7.1536)
		);
}

//to 3d functions

float3 rand3dTo3d(float3 value) {
	return float3(
		rand3dTo1d(value, float3(12.989, 78.233, 37.719)),
		rand3dTo1d(value, float3(39.346, 11.135, 83.155)),
		rand3dTo1d(value, float3(73.156, 52.235, 09.151))
		);
}

float3 rand2dTo3d(float2 value) {
	return float3(
		rand2dTo1d(value, float2(12.989, 78.233)),
		rand2dTo1d(value, float2(39.346, 11.135)),
		rand2dTo1d(value, float2(73.156, 52.235))
		);
}

float3 rand1dTo3d(float value) {
	return float3(
		rand1dTo1d(value, 3.9812),
		rand1dTo1d(value, 7.1536),
		rand1dTo1d(value, 5.7241)
		);
}

float easeIn(float interpolator) {
	return interpolator * interpolator;
}

float easeOut(float interpolator) {
	return 1 - easeIn(1 - interpolator);
}

float easeInOut(float interpolator) {
	float easeInValue = easeIn(interpolator);
	float easeOutValue = easeOut(interpolator);
	return lerp(easeInValue, easeOutValue, interpolator);
}

float2 modulo(float2 divident, float2 divisor) {
	float2 positiveDivident = divident % divisor + divisor;
	return positiveDivident % divisor;
}

float perlinNoise(float2 value, float2 period) {
	float2 cellsMimimum = floor(value);
	float2 cellsMaximum = ceil(value);

	cellsMimimum = modulo(cellsMimimum, period);
	cellsMaximum = modulo(cellsMaximum, period);

	//generate random directions
	float2 lowerLeftDirection = rand2dTo2d(float2(cellsMimimum.x, cellsMimimum.y)) * 2 - 1;
	float2 lowerRightDirection = rand2dTo2d(float2(cellsMaximum.x, cellsMimimum.y)) * 2 - 1;
	float2 upperLeftDirection = rand2dTo2d(float2(cellsMimimum.x, cellsMaximum.y)) * 2 - 1;
	float2 upperRightDirection = rand2dTo2d(float2(cellsMaximum.x, cellsMaximum.y)) * 2 - 1;

	float2 fraction = frac(value);

	//get values of cells based on fraction and cell directions
	float lowerLeftFunctionValue = dot(lowerLeftDirection, fraction - float2(0, 0));
	float lowerRightFunctionValue = dot(lowerRightDirection, fraction - float2(1, 0));
	float upperLeftFunctionValue = dot(upperLeftDirection, fraction - float2(0, 1));
	float upperRightFunctionValue = dot(upperRightDirection, fraction - float2(1, 1));

	float interpolatorX = easeInOut(fraction.x);
	float interpolatorY = easeInOut(fraction.y);

	//interpolate between values
	float lowerCells = lerp(lowerLeftFunctionValue, lowerRightFunctionValue, interpolatorX);
	float upperCells = lerp(upperLeftFunctionValue, upperRightFunctionValue, interpolatorX);

	float noise = lerp(lowerCells, upperCells, interpolatorY);
	return noise;
}


void PerlinTile_float(float2 uv, int cellAmount, float2 period, float persistance, float roughness, out float output) 
{

	float noise = 0;
	float frequency = 1;
	float factor = 1;

	float2 value = uv * cellAmount;

	for (int i = 0; i < OCTAVES; i++) {
		noise = noise + perlinNoise(value * frequency + i * 0.72354, period * frequency) * factor;
		factor *= persistance;
		frequency *= roughness;
	}

	output = noise + 0.5;
}

#endif