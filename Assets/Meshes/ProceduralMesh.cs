using ProceduralMeshes;
using ProceduralMeshes.Generators;
using ProceduralMeshes.Streams;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ProceduralMesh : MonoBehaviour {

	Vector3[] vertices, normals;
	Vector4[] tangents;
	
	static MeshJobScheduleDelegate[] jobs = {
		MeshJob<SquareGrid, SingleStream>.ScheduleParallel,
		MeshJob<SharedSquareGrid, SingleStream>.ScheduleParallel,
		MeshJob<SharedTriangleGrid, SingleStream>.ScheduleParallel,
		MeshJob<PointyHexagonGrid, SingleStream>.ScheduleParallel,
		MeshJob<FlatHexagonGrid, SingleStream>.ScheduleParallel
	};

	public enum MeshType {
		SquareGrid, SharedSquareGrid, SharedTriangleGrid, PointyHexagonGrid,FlatHexagonGrid
	};

	[SerializeField]
	MeshType meshType;

	[SerializeField, Range(1, 50)]
	int resolution = 1;

    Mesh mesh;
	

	void Awake () {
		mesh = new Mesh {
			name = "Procedural Mesh"
		};
		
		GetComponent<MeshFilter>().mesh = mesh;
	}
	
	void GenerateMesh () {
        Mesh.MeshDataArray meshDataArray = Mesh.AllocateWritableMeshData(1);
		Mesh.MeshData meshData = meshDataArray[0];

		jobs[(int)meshType](mesh, meshData, resolution, default).Complete();

		Mesh.ApplyAndDisposeWritableMeshData(meshDataArray, mesh);
    }

	void OnValidate () => enabled = true;

	void Update () {
		GenerateMesh();
		enabled = false;

		vertices = mesh.vertices;
		normals = mesh.normals;
		tangents = mesh.tangents;
	}

	void OnDrawGizmos () {
		if (mesh == null) {
			return;
		}
		for (int i = 0; i < vertices.Length; i++) {
			Vector3 position = vertices[i];
			Gizmos.color = Color.cyan;
			Gizmos.DrawSphere(position, 0.02f);
			Gizmos.color = Color.green;
			Gizmos.DrawRay(position, normals[i] * 0.2f);
			Gizmos.color = Color.red;
			Gizmos.DrawRay(position, tangents[i] * 0.2f);
		}
	}

}