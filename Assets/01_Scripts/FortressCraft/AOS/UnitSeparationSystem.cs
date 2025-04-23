using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Agit.FortressCraft
{
    [BurstCompile]
    public struct UnitSeparationJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<float2> Positions;
        public NativeArray<float2> SeparationOffsets;

        public float DesiredSeparation;
        public float SeparationStrength;

        public void Execute(int index)
        {
            float2 self = Positions[index];
            float2 offset = float2.zero;

            for (int i = 0; i < Positions.Length; i++)
            {
                if (i == index) continue;

                float2 other = Positions[i];
                float2 diff = self - other;
                float dist = math.length(diff);

                if (dist < DesiredSeparation && dist > 0.001f)
                {
                    offset += (math.normalize(diff) / dist) * SeparationStrength;
                }
            }

            SeparationOffsets[index] = offset;
        }
    }

    public class UnitSeparationSystem : MonoBehaviour
    {
        [SerializeField] private float desiredSeparation = 1.0f;
        [SerializeField] private float separationStrength = 0.1f;
        [SerializeField] public Transform[] unitTransforms;

        private NativeArray<float2> positions;
        private NativeArray<float2> offsets;

        void LateUpdate()
        {
            int count = unitTransforms.Length;
            if (count == 0) return;

            positions = new NativeArray<float2>(count, Allocator.TempJob);
            offsets = new NativeArray<float2>(count, Allocator.TempJob);

            for (int i = 0; i < count; i++)
                positions[i] = new float2(unitTransforms[i].position.x, unitTransforms[i].position.y);

            var job = new UnitSeparationJob
            {
                Positions = positions,
                SeparationOffsets = offsets,
                DesiredSeparation = desiredSeparation,
                SeparationStrength = separationStrength
            };

            JobHandle handle = job.Schedule(count, 32);
            handle.Complete();

            for (int i = 0; i < count; i++)
            {
                Vector3 offset3D = new Vector3(offsets[i].x, offsets[i].y, 0f);
                unitTransforms[i].position += offset3D * Time.deltaTime;
            }

            positions.Dispose();
            offsets.Dispose();
        }
    }
}
//