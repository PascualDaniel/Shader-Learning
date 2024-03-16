using Unity.Mathematics;

using static Unity.Mathematics.math;

public static partial class Noise {

	public struct Lattice1D : INoise {

		public float4 GetNoise4(float4x3 positions, SmallXXHash4 hash) {
			int4 p = (int4)floor(positions.c0);
			float4 v = (uint4)hash.Eat(p) & 255;
			return v * (2f / 255f) - 1f;
		}
	}
}