using UnityEngine;
						
namespace ProceduralMeshes {

	public interface IMeshGenerator {

		Bounds Bounds { get; }

		void Execute<S> (int i, S streams) where S : struct, IMeshStreams;
		
		int VertexCount { get; }
		
		int IndexCount { get; }
		int JobLength { get; }

		int Resolution { get; set; }

	}
}