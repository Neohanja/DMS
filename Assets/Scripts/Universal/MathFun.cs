using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathFun
{
    public static float Abs(float value)
    {
        if (value < 0) return -value;
        return value;
    }

    public static int Abs(int value)
    {
        if (value < 0) return -value;
        return value;
    }

    public static float Power(float value, float power)
    {
        if (power == 0) return 1;

        float a = value;

        for (int i = 1; i < Abs(power); ++i)
        {
            a *= value;
        }

        if (power < 0)
        {
            a = 1f / a;
        }

        return a;
    }

    public static int Floor(float value)
    {
        int t = (int)value;
        if (value < 0) t--;
        return t;
    }

    public static int Round(float value)
    {
        int t = Floor(value);
        if (Abs(value) - Abs(t) >= 0.5f) t++;
        return t;
    }

    public static float Curve(float value)
    {
        return value * value * value * (value * (value * 6f - 15f) + 10f);
    }

    public static float Lerp(float a, float b, float value)
    {
        return a + value * (b - a);
    }

    public static float InverseLerp(float a, float b, float value)
    {
        return (a - value) / (b - a);
    }

    public static float Dot(Vector2 a, Vector2 b)
    {
        return a.x * b.x + a.y * b.y;
    }

    public static float PointOnCircle(float value)
    {
        return Mathf.Sqrt(-(value * value - 1));
    }

    public static int EpochTime
    {
        get
        {
            System.DateTime epochTime = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);

            return (int)(System.DateTime.UtcNow - epochTime).TotalSeconds;
        }
    }
}

[System.Serializable]
public class MinMax
{
    public int min;
    public int max;

    public MinMax(int a, int b)
    {
        min = (a <= b) ? a : b;
        max = (a <= b) ? b : a;
    }
}