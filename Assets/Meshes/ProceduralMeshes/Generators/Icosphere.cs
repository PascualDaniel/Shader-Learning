using Unity.Mathematics;
using UnityEngine;

using static Unity.Mathematics.math;

namespace ProceduralMeshes.Generators
{

	public struct Icosphere  : IMeshGenerator
	{
		public int VertexCount => 5 * Resolution * Resolution + 2;

		public int IndexCount => 6 * 5 * Resolution * Resolution;

		public int JobLength => 5 * Resolution;

		public int Resolution { get; set; }

		struct Side
		{
			public int id;
			public float3 uvOrigin, uVector, vVector;
			public int seamStep;
			public bool TouchesMinimumPole => (id & 1) == 0;
		}

		struct Rhombus
		{
			public int id;
			public float3 leftCorner, rightCorner;
		}

		static Rhombus GetRhombus(int id) => id switch
		{
			0 => new Rhombus {
				id = id,
				leftCorner = GetCorner(0),
				rightCorner = GetCorner(1)
			},
			1 => new Rhombus {
				id = id,
				leftCorner = GetCorner(1),
				rightCorner = GetCorner(2)
			},
			2 => new Rhombus {
				id = id,
				leftCorner = GetCorner(2),
				rightCorner = GetCorner(3)
			},
			3 => new Rhombus {
				id = id,
				leftCorner = GetCorner(3),
				rightCorner = GetCorner(4)
			},
			_ => new Rhombus {
				id = id,
				leftCorner = GetCorner(4),
				rightCorner = GetCorner(0)
			}
		};

		static float3 CubeToSphere(float3 p) => p * sqrt(
			1f - ((p * p).yxx + (p * p).zzy) / 2f + (p * p).yxx * (p * p).zzy / 3f
		);

		public Bounds Bounds => new Bounds(Vector3.zero, new Vector3(2f, 2f, 2f));
		

		static float3 GetCorner (int id) => float3(
			sin(0.4f * PI * id),
			0f,
			-cos(0.4f * PI * id)
		);
	
		public void Execute<S>(int i, S streams) where S : struct, IMeshStreams
		{
			int u = i / 4;
			Rhombus rhombus = GetRhombus(i - 4 * u);
			int vi = Resolution * (Resolution * rhombus.id + u ) + 2;
			int ti = 2 * Resolution * (Resolution * rhombus.id + u);
			bool firstColumn = u == 0;

			int4 quad = int4(
				vi,
				firstColumn ? 0 : vi - Resolution,
				firstColumn ?
					rhombus.id == 0 ?
						3 * Resolution * Resolution + 2 :
						vi - Resolution * (Resolution + u) :
					vi - Resolution + 1,
				vi + 1
			);

			u += 1;

			float3 columnBottomDir = rhombus.rightCorner - down();
			float3 columnBottomStart = down() + columnBottomDir * u / Resolution;
			float3 columnBottomEnd =
				rhombus.leftCorner + columnBottomDir * u / Resolution;

			float3 columnTopDir = up() - rhombus.leftCorner;
			float3 columnTopStart =
				rhombus.rightCorner + columnTopDir * ((float)u / Resolution - 1f);
			float3 columnTopEnd = rhombus.leftCorner + columnTopDir * u / Resolution;

			var vertex = new Vertex();
			if (i == 0) {
				vertex.position = down();
				streams.SetVertex(0, vertex);
				vertex.position = up();
				streams.SetVertex(1, vertex);
			}
			vertex.position = columnBottomStart;
			streams.SetVertex(vi, vertex);
			vi += 1;

			for (int v = 1; v < Resolution; v++, vi++, ti += 2)
			{
				if (v <= Resolution - u)
				{
					vertex.position =
						lerp(columnBottomStart, columnBottomEnd, (float)v / Resolution);
				}
				else
				{
					vertex.position =
						lerp(columnTopStart, columnTopEnd, (float)v / Resolution);
				}
				
				
				streams.SetVertex(vi, vertex);
				streams.SetTriangle(ti + 0, quad.xyz);
				streams.SetTriangle(ti + 1, quad.xzw);

				quad.y = quad.z;
				quad += int4(1, 0, firstColumn  ? Resolution : 1, 1);
			}

			if (!firstColumn) {
				quad.z = Resolution * Resolution * (rhombus.id == 0 ? 4 : rhombus.id) -
					Resolution + u + 1;
			}
			quad.w = u < Resolution ? quad.z + 1 : 1;

			streams.SetTriangle(ti + 0, quad.xyz);
			streams.SetTriangle(ti + 1, quad.xzw);
		}
		
	}
}