using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RanGen
{
    int seed;
    int position;

    public RanGen(int seedling)
    {
        seed = seedling;
        position = 0;
    }

    public RanGen(int seedling, int index)
    {
        seed = seedling;
        position = index;

        if (position <= 0) position = 1;
    }

    public float Percentage(int decimals = 2)
    {
        position++;

        return PullNumber(position, seed) % (MathFun.Power(10, decimals) + 1f) / (float)MathFun.Power(10, decimals);
    }

    public int GetSeed { get { return seed; } }
    public int GetIndex { get { return position; } }

    public int Roll(int min, int max)
    {
        position++;

        return PullNumber(position, seed) % (max - min + 1) + min;
    }

    public int RandomIndex(int length)
    {
        if (length == 0) return 0;

        position++;

        return PullNumber(position, seed) % length;
    }

    public static int PullNumber(int a, int inputSeed)
    {
        int mangle = a;

        mangle *= PrimeIndex[0];
        mangle += inputSeed;
        mangle ^= (mangle >> 8);
        mangle *= PrimeIndex[1];
        mangle ^= (mangle << 8);
        mangle *= PrimeIndex[2];
        mangle ^= (mangle >> 8);

        return mangle;
    }

    public static int PullNumber(int a, int b, int inputSeed)
    {
        int mangle = a + b * PrimeIndex[3];

        mangle *= PrimeIndex[0];
        mangle += inputSeed;
        mangle ^= (mangle >> 8);
        mangle *= PrimeIndex[1];
        mangle ^= (mangle << 8);
        mangle *= PrimeIndex[2];
        mangle ^= (mangle >> 8);

        return mangle;
    }

    public static int PullNumber(int a, int b, int c, int inputSeed)
    {
        int mangle = a + b * PrimeIndex[3] + c * PrimeIndex[4];

        mangle *= PrimeIndex[0];
        mangle += inputSeed;
        mangle ^= (mangle >> 8);
        mangle *= PrimeIndex[1];
        mangle ^= (mangle << 8);
        mangle *= PrimeIndex[2];
        mangle ^= (mangle >> 8);

        return mangle;
    }

    private static readonly int[] PrimeIndex = new int[]
    {
        16769023,
        479001599,
        433494437,
        370248451,
        218198423,
        479001599,
        777767777,
        715827883,
        126122527,
        53471161
    };
}
