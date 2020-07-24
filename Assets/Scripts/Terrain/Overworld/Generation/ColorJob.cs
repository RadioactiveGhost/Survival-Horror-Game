using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using Unity.Burst;
using Unity.Mathematics;

[BurstCompile]
public struct ColorJob : IJobParallelFor
{
    [NativeDisableParallelForRestriction]
    public NativeArray<float> heights;
    [NativeDisableParallelForRestriction]
    public NativeArray<Color> colors;
    [NativeDisableParallelForRestriction]
    public NativeArray<float> heightsFromBiome;
    [NativeDisableParallelForRestriction]
    public NativeArray<Color> colorsFromBiome;
    float y, height;

    public void Execute(int index)
    {
        //find height on certain point (works for points not in plane vertexes)
        y = heights[index];

        //Go through colors
        height = -1;
        for (int k = 0; k < heightsFromBiome.Length; k++)
        {
            if (y >= heightsFromBiome[k] && height < heightsFromBiome[k])
            {
                colors[index] = colorsFromBiome[k];
                height = heightsFromBiome[k];
            }
        }
    }
}

[BurstCompile]
public struct VertexHeightJob : IJobParallelFor
{
    [NativeDisableParallelForRestriction]
    public NativeArray<float3> originalPoints;
    public int sizeZtile, detailMultiplier;
    [NativeDisableParallelForRestriction]
    public NativeArray<float> results;
    public void Execute(int index)
    {
        Vector2 pointsToFindHeight = new Vector2(0, 0);
        int helper = index;
        while (helper > sizeZtile * detailMultiplier - 1) //starts on 0
        {
            helper -= sizeZtile * detailMultiplier;
            pointsToFindHeight.y++;
        }
        pointsToFindHeight.x = helper;

        pointsToFindHeight.x = ((1 / (float)detailMultiplier) * pointsToFindHeight.x) + originalPoints[0].x;
        pointsToFindHeight.y = ((1 / (float)detailMultiplier) * pointsToFindHeight.y) + originalPoints[0].z;

        float2 bottomLeft = new float2(Mathf.Floor(pointsToFindHeight.x), Mathf.Floor(pointsToFindHeight.y));
        float2 bottomRight = new float2(Mathf.Floor(pointsToFindHeight.x) + 1, Mathf.Floor(pointsToFindHeight.y));
        float2 topLeft = new float2(Mathf.Floor(pointsToFindHeight.x), Mathf.Floor(pointsToFindHeight.y) + 1);
        float2 topRight = new float2(Mathf.Floor(pointsToFindHeight.x) + 1, Mathf.Floor(pointsToFindHeight.y) + 1);

        float xDiff = pointsToFindHeight.x - bottomLeft.x;
        float yDiff = pointsToFindHeight.y - bottomLeft.y;

        float bottomY = FindPointHeight(bottomLeft) * (1 - xDiff) + FindPointHeight(bottomRight) * xDiff;
        float topY = FindPointHeight(topLeft) * (1 - xDiff) + FindPointHeight(topRight) * xDiff;

        results[index] = bottomY * (1 - yDiff) + topY * yDiff;
    }

    float FindPointHeight(float2 point)
    {
        for (int i = 0; i < originalPoints.Length; i++)
        {
            if (point.y == originalPoints[i].z && point.x == originalPoints[i].x)
            {
                return originalPoints[i].y;
            }
        }
        throw new System.Exception("Found no corresponding point for point " + point);
    }
}