using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GradNoise
{
    private static readonly int a = 1001;
    private static readonly float b = 1000f;

    public static float[,] GradMap(Vector2 startPoint, World worldSettings, WorldGenSettings genSettings)
    {
        float[,] gMap = new float[worldSettings.chunkSize, worldSettings.chunkSize];

        for(int x = 0; x < worldSettings.chunkSize; ++x)
        {
            for(int y = 0; y < worldSettings.chunkSize; ++y)
            {
                float sampleX = (x + startPoint.x) / genSettings.noiseScale;
                float sampleY = (y + startPoint.y) / genSettings.noiseScale;

                gMap[x, y] = Noise2D(sampleX, sampleY, worldSettings.worldSeed);
            }
        }

        return gMap;
    }

    public static bool[,] ClutterMap(Vector2Int startPoint, World worldSettings, Clutter decor)
    {
        bool[,] cMap = new bool[worldSettings.chunkSize, worldSettings.chunkSize];

        for (int x = 0; x < worldSettings.chunkSize; ++x)
        {
            for (int y = 0; y < worldSettings.chunkSize; ++y)
            {
                float sampleX = (x + startPoint.x) / decor.clutterScale;
                float sampleY = (y + startPoint.y) / decor.clutterScale;

                cMap[x, y] = ((Noise2D(sampleX, sampleY, decor.zLoc, worldSettings.worldSeed) + 1) * 0.5f) <= decor.threshold;
            }
        }

        return cMap;
    }

    public static float Noise1D(float x, int seed)
    {
        int iX = MathFun.Floor(x);

        float cX = MathFun.Curve(x - iX);

        float x1 = RanGen.PullNumber(iX, seed) % a / b * 2 - 1;
        float x2 = RanGen.PullNumber(iX + 1, seed) % a / b * 2 - 1;

        return MathFun.Lerp(x1, x2, cX);
    }

    public static float Noise2D(float x, float y, int seed)
    {
        int iX = MathFun.Floor(x);
        int iY = MathFun.Floor(y);

        float cX = MathFun.Curve(x - iX);
        float cY = MathFun.Curve(y - iY);

        Vector2[] corner = new Vector2[]
        {
            new Vector2(iX - x, iY - y),
            new Vector2(iX + 1 - x, iY - y),
            new Vector2(iX - x, iY + 1 - y),
            new Vector2(iX + 1 - x, iY + 1 - y),
        };

        float x1y1 = MathFun.Dot(corner[0], RandomDirection(iX, iY, seed));
        float x2y1 = MathFun.Dot(corner[1], RandomDirection(iX + 1, iY, seed));

        float a = MathFun.Lerp(x1y1, x2y1, cX);

        float x1y2 = MathFun.Dot(corner[2], RandomDirection(iX, iY + 1, seed));
        float x2y2 = MathFun.Dot(corner[3], RandomDirection(iX + 1, iY + 1, seed));

        float b = MathFun.Lerp(x1y2, x2y2, cX);

        return MathFun.Lerp(a, b, cY);
    }

    public static float Noise2D(float x, float y, int z, int seed)
    {
        int iX = MathFun.Floor(x);
        int iY = MathFun.Floor(y);

        float cX = MathFun.Curve(x - iX);
        float cY = MathFun.Curve(y - iY);

        Vector2[] corner = new Vector2[]
        {
            new Vector2(iX - x, iY - y),
            new Vector2(iX + 1 - x, iY - y),
            new Vector2(iX - x, iY + 1 - y),
            new Vector2(iX + 1 - x, iY + 1 - y),
        };

        float x1y1 = MathFun.Dot(corner[0], RandomDirection(iX, iY, z, seed));
        float x2y1 = MathFun.Dot(corner[1], RandomDirection(iX + 1, iY, z, seed));

        float a = MathFun.Lerp(x1y1, x2y1, cX);

        float x1y2 = MathFun.Dot(corner[2], RandomDirection(iX, iY + 1, z, seed));
        float x2y2 = MathFun.Dot(corner[3], RandomDirection(iX + 1, iY + 1, z, seed));

        float b = MathFun.Lerp(x1y2, x2y2, cX);

        return MathFun.Lerp(a, b, cY);
    }

    public static Vector2 RandomDirection(int x, int y, int z, int seed)
    {
        int quad = RanGen.PullNumber(x, y, z, seed) % 4 + 1;

        float xP = RanGen.PullNumber(x, y, z, seed) % a / b;
        float yP = MathFun.PointOnCircle(xP);

        switch (quad)
        {
            case 2:
                yP *= -1;
                break;
            case 3:
                yP *= -1;
                xP *= -1;
                break;
            case 4:
                xP *= -1;
                break;
        }

        return new Vector2(xP, yP);
    }

    public static Vector2 RandomDirection(int x, int y, int seed, bool addToOrigin = false)
    {
        int quad = RanGen.PullNumber(x, y, 1, seed) % 4 + 1;

        float xP = RanGen.PullNumber(x, y, seed) % a / b;
        float yP = MathFun.PointOnCircle(xP);

        switch(quad)
        {
            case 2:
                yP *= -1;
                break;
            case 3:
                yP *= -1;
                xP *= -1;
                break;
            case 4:
                xP *= -1;
                break;
        }

        return new Vector2(addToOrigin ? x : 0 + xP, addToOrigin ? y : 0 + yP);
    }
}
