using Unity.Mathematics;

using static Unity.Mathematics.math;

public static partial class Noise {


    static float4 UpdateVoronoiMinima (float4 minima, float4 distances) {
		return select(minima, distances, distances < minima);
	}

    static float4 GetDistance (float4 x, float4 y) => sqrt(x * x + y * y);

	public struct Voronoi1D<L> : INoise where L : struct, ILattice {

		public float4 GetNoise4 (float4x3 positions, SmallXXHash4 hash, int frequency) {
			var l = default(L);
			LatticeSpan4 x = l.GetLatticeSpan4(positions.c0, frequency);

			float4 minima = 2f;
			for (int u = -1; u <= 1; u++) {
				SmallXXHash4 h = hash.Eat(l.ValidateSingleStep(x.p0 + u, frequency));
				minima = UpdateVoronoiMinima(minima, abs(h.Floats01A + u - x.g0));
			}
			return minima;
		}
	}

	public struct Voronoi2D<L> : INoise where L : struct, ILattice {

		public float4 GetNoise4 (float4x3 positions, SmallXXHash4 hash, int frequency) {
			var l = default(L);
			LatticeSpan4
				x = l.GetLatticeSpan4(positions.c0, frequency),
				z = l.GetLatticeSpan4(positions.c2, frequency);

			float4 minima = 2f;
			for (int u = -1; u <= 1; u++) {
				SmallXXHash4 hx = hash.Eat(l.ValidateSingleStep(x.p0 + u, frequency));
				float4 xOffset = u - x.g0;
				for (int v = -1; v <= 1; v++) {
					SmallXXHash4 h = hx.Eat(l.ValidateSingleStep(z.p0 + v, frequency));
					float4 zOffset = v - z.g0;
					minima = UpdateVoronoiMinima(minima, GetDistance(
						h.Floats01A + xOffset, h.Floats01B + zOffset
					));
					minima = UpdateVoronoiMinima(minima, GetDistance(
						h.Floats01C + xOffset, h.Floats01D + zOffset
					));
				}
			}
			return min(minima, 1f);
		}
	}

	public struct Voronoi3D<L> : INoise where L : struct, ILattice {

		public float4 GetNoise4 (float4x3 positions, SmallXXHash4 hash, int frequency) {
			var l = default(L);
			LatticeSpan4
				x = l.GetLatticeSpan4(positions.c0, frequency),
				y = l.GetLatticeSpan4(positions.c1, frequency),
				z = l.GetLatticeSpan4(positions.c2, frequency);

			return 0f;
		}
	}
}