using Unity.Mathematics;
using UnityEngine;

using static Unity.Mathematics.math;

namespace ProceduralMeshes.Generators
{

	public struct UVSphere : IMeshGenerator
	{
		public int VertexCount => (Resolution + 1) * (Resolution + 1);
		public int IndexCount => 6 * Resolution * Resolution;

		public int JobLength => Resolution+ 1;

		public int Resolution { get; set; }

		public Bounds Bounds => new Bounds(Vector3.zero, new Vector3(2f, 2f, 2f));

		public void Execute<S> (int u, S streams) where S : struct, IMeshStreams {
			int vi = (Resolution + 1) * u, ti = 2 * Resolution * (u - 1);

			var vertex = new Vertex();	
			vertex.position.y = vertex.normal.y = -1f;
			vertex.tangent.w = -1f;
			
			float2 circle;
			circle.x = sin(2f * PI * u / Resolution);
			circle.y = cos(2f * PI * u / Resolution);
			vertex.tangent.xz = circle.yx;
			circle.y = -circle.y;
			vertex.texCoord0.x = (float)u / Resolution;
			streams.SetVertex(vi, vertex);
			vi += 1;

			for (int v = 1; v <= Resolution; v++, vi++, ti += 2) {
				float circleRadius = sin(PI * v / Resolution);
				vertex.position.xz = circle * circleRadius;
				vertex.position.y = -cos(PI * v / Resolution);
				vertex.normal = vertex.position;
				vertex.texCoord0.y = (float)v / Resolution;
				streams.SetVertex(vi, vertex);

				if (u > 0) {
					streams.SetTriangle(
						ti + 0, vi + int3(-Resolution - 2, -Resolution - 1, -1)
					);
					streams.SetTriangle(
						ti + 1, vi + int3(-1, -Resolution - 1, 0)
					);
				}
			}
		}
	}
}