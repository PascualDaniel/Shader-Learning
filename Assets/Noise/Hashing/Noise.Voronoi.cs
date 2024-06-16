using Unity.Mathematics;

using static Unity.Mathematics.math;

public static partial class Noise {


    static float4 UpdateVoronoiMinima (float4 minima, float4 distances) {
		return select(minima, distances, distances < minima);
	}

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

			return 0f;
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