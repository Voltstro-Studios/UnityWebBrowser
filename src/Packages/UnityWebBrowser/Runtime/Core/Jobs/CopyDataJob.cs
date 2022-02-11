using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

namespace UnityWebBrowser.Core.Jobs
{
    [BurstCompile(CompileSynchronously = true)]
    public struct CopyDataJob : IJobParallelFor
    {
        [ReadOnly]
        [NativeDisableContainerSafetyRestriction]
        public NativeArray<byte> fromArray;

        [WriteOnly]
        [NativeDisableContainerSafetyRestrictionAttribute]
        public NativeArray<byte> toArray;

        public void Execute(int index)
        {
            toArray[index] = fromArray[index];
        }
    }
}