using Unity.Mathematics;

using static Unity.Mathematics.math;

public static partial class Noise {

	public struct Simplex1D<G> : INoise where G : struct, IGradient {

		public float4 GetNoise4 (float4x3 positions, SmallXXHash4 hash, int frequency) {
			positions *= frequency;
			int4 x0 = (int4)floor(positions.c0), x1 = x0 + 1;

			return default(G).EvaluateCombined (
				Kernel(hash.Eat(x0), x0, positions) + Kernel(hash.Eat(x1), x1, positions)
			);
		}
        static float4 Kernel (SmallXXHash4 hash, float4 lx, float4x3 positions) {
			float4 x = positions.c0 - lx;
			float4 f = 1f - x * x;
            f = f * f * f;
			return f * default(G).Evaluate(hash, x);
		}
	}

	public struct Simplex2D<G> : INoise where G : struct, IGradient {

		public float4 GetNoise4 (float4x3 positions, SmallXXHash4 hash, int frequency) {
			return default(G).EvaluateCombined (0f);
		}
	}

	public struct Simplex3D<G> : INoise where G : struct, IGradient {

		public float4 GetNoise4 (float4x3 positions, SmallXXHash4 hash, int frequency) {
			return default(G).EvaluateCombined (0f);
		}
	}
}