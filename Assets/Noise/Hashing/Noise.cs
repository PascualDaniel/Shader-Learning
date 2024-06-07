using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using static Unity.Mathematics.math;

public static partial class Noise {

	[Serializable]
	public struct Settings {

		public int seed;
		[Min(1)]
		public int frequency;
		[Range(1, 6)]
		public int octaves;
		public static Settings Default => new Settings {
			frequency = 4,
			octaves = 1
		};
	}


	public interface INoise
	{
		float4 GetNoise4(float4x3 positions, SmallXXHash4 hash);
	}

	[BurstCompile(FloatPrecision.Standard, FloatMode.Fast, CompileSynchronously = true)]
	public struct Job<N> : IJobFor where N : struct, INoise
	{

		[ReadOnly]
		public NativeArray<float3x4> positions;

		[WriteOnly]
		public NativeArray<float4> noise;

		public Settings settings;

		public float3x4 domainTRS;

		

		public void Execute(int i)
		{
			float4x3 position = domainTRS.TransformVectors(transpose(positions[i]));
			var hash = SmallXXHash4.Seed(settings.seed);
			int frequency = settings.frequency;
			float4 sum = 0f;

			for (int o = 0; o < settings.octaves; o++) {
				sum += default(N).GetNoise4(frequency * position, hash);
				frequency *= 2;
			}
			noise[i] = sum;
		}
		public static JobHandle ScheduleParallel(
			NativeArray<float3x4> positions, NativeArray<float4> noise,
			Settings settings, SpaceTRS domainTRS, int resolution, JobHandle dependency
			) => new Job<N>
			{
				positions = positions,
				noise = noise,
				//hash = SmallXXHash.Seed(seed),
				settings = settings,
				domainTRS = domainTRS.Matrix,
			}.ScheduleParallel(positions.Length, resolution, dependency);
	}

	public delegate JobHandle ScheduleDelegate (
		NativeArray<float3x4> positions, NativeArray<float4> noise, //int seed,
		Settings settings, SpaceTRS trs, int resolution, JobHandle dependency
	);
}
