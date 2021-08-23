using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CellAuto
{
    public static bool[,] CellMap(Vector2Int startLoc, int mapSize, int seed, WorldGenSettings genSettings)
    {
        bool[,] cells = new bool[mapSize, mapSize];

        for (int x = 0; x < mapSize; ++x)
        {
            for (int y = 0; y < mapSize; ++y)
            {
                cells[x, y] = RanGen.PullNumber(x + startLoc.x, y + startLoc.y, seed) % 100 >= 50;
            }
        }

        for(int smoothing = 0; smoothing < genSettings.smoothing; ++smoothing)
        {
            cells = SmoothCells(cells, mapSize, genSettings);
        }

        return cells;
    }

    public static bool[,] SmoothCells(bool[,] cells, int mapSize, WorldGenSettings cellSettings)
    {
        bool[,] tempMap = new bool[mapSize, mapSize];

        for(int x = 0; x < mapSize; ++x)
        {
            for(int y = 0; y < mapSize; ++y)
            {
                int countCells = 0;

                for(int cX = -1; cX < 2; ++cX)
                {
                    for(int cY = -1; cY < 2; ++cY)
                    {
                        if (cX + x < 0 || cX + x >= mapSize) countCells++;
                        else if (cY + y < 0 || cY + y >= mapSize) countCells++;
                        else if (cells[cX + x, cY + y] && !(cX == 0 && cY == 0)) countCells++;
                    }
                }

                tempMap[x, y] = cells[x, y] || countCells >= cellSettings.rebirth;
                
                if (countCells < cellSettings.underPopulation) tempMap[x, y] = false;
            }
        }

        return tempMap;
    }    
}
