using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;


public class NoiseVisualization : Visualization {

    static int
		noiseId = Shader.PropertyToID("_Noise");


	NativeArray<float4> noise;


	[SerializeField]
	int seed;

	[SerializeField]
	SpaceTRS domain = new SpaceTRS {
		scale = 8f
	};

	ComputeBuffer noiseBuffer;

	[SerializeField]
	Shape shape;

	bool isDirty;
    protected override void EnableVisualization (int dataLength, MaterialPropertyBlock propertyBlock) {
		isDirty = true;

		
		noise = new NativeArray<float4>(dataLength, Allocator.Persistent);
		noiseBuffer = new ComputeBuffer(dataLength, 4);
	
		propertyBlock ??= new MaterialPropertyBlock();
		propertyBlock.SetBuffer(noiseId, noiseBuffer);

	}

    protected override void DisableVisualization () {
		noise.Dispose();
		noiseBuffer.Release();
		noiseBuffer = null;
	}


    protected override void UpdateVisualization (
		NativeArray<float3x4> positions, int resolution, JobHandle handle
	) {
		

		noiseBuffer.SetData(noise.Reinterpret<float>(4 * 4));

	}

    
}