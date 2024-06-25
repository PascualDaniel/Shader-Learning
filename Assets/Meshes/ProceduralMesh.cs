using ProceduralMeshes;
using ProceduralMeshes.Generators;
using ProceduralMeshes.Streams;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ProceduralMesh : MonoBehaviour {
	
	static MeshJobScheduleDelegate[] jobs = {
		MeshJob<SquareGrid, SingleStream>.ScheduleParallel,
		MeshJob<SharedSquareGrid, SingleStream>.ScheduleParallel,
		MeshJob<SharedTriangleGrid, SingleStream>.ScheduleParallel,
		MeshJob<PointyHexagonGrid, SingleStream>.ScheduleParallel
	};

	public enum MeshType {
		SquareGrid, SharedSquareGrid, SharedTriangleGrid, PointyHexagonGrid
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
		//GenerateMesh();
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
	}

}