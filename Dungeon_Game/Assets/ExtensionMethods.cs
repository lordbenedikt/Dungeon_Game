using System;
using System.Collections.Generic;
using UnityEngine;

namespace ExtensionMethods
{
    public static class LevelGenerator
    {
        static System.Random random = new System.Random();
        static int levelWidth;
        static int levelHeight;
        static int[,] level;

        public static int[,] Generate(int width, int height)
        {
            levelWidth = width;
            levelHeight = height;
            level = new int[levelWidth, levelHeight];

            resetCells(level);
            randomDivider(level);
            cleanMap(level);

            return (int[,])level.Clone();
        }

        public static void PrintLevel(int[,] level)
        {
            for (int i = 0; i < level.GetLength(1); i++)
            {
                for (int j = 0; j < level.GetLength(0); j++)
                {
                    Debug.Log((level[j, i] == 0 ? "E" : (level[j, i] == 1 ? "_" : " ")) + " ");
                }
                Console.Write("\n");
            }
        }

        static void Main(string[] args)
        {
            while (true)
            {
                String[] input = Console.ReadLine().Split(',');
                int width = Convert.ToInt32(input[0]);
                int height = Convert.ToInt32(input[1]);
                PrintLevel(Generate(width, height));
            }

        }

        static void cleanMap(int[,] level)
        {
            int w = level.GetLength(0);
            int h = level.GetLength(1);
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    int count = 0;
                    if (j != 0 && level[i, j - 1] == 1) count++;
                    if (i != 0 && level[i - 1, j] == 1) count++;
                    if (level[i, j] == 1) count++;
                    if (i != w - 1 && level[i + 1, j] == 1) count++;
                    if (j != h - 1 && level[i, j + 1] == 1) count++;
                    if (level[i, j] == 1 && count == 1 || count == 2)
                    {
                        level[i, j] = 0;
                    }
                }
            }
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    int count = 0;
                    if (j != 0 && level[i, j - 1] == 1) count++;
                    if (i != 0 && level[i - 1, j] == 1) count++;
                    if (level[i, j] == 1) count++;
                    if (i != w - 1 && level[i + 1, j] == 1) count++;
                    if (j != h - 1 && level[i, j + 1] == 1) count++;
                    if (i != 0 && j != 0 && level[i - 1, j - 1] == 1) count++;
                    if (i != w - 1 && j != 0 && level[i + 1, j - 1] == 1) count++;
                    if (i != 0 && j != h - 1 && level[i - 1, j + 1] == 1) count++;
                    if (i != w - 1 && j != h - 1 && level[i + 1, j + 1] == 1) count++;
                    if (count == 0)
                    {
                        level[i, j] = -1;
                    }
                }
            }
        }

        static void dfs(int[,] level, int pos, List<int> visited)
        {
            int w = level.GetLength(0);
            int h = level.GetLength(1);
            if (visited.Contains(pos))
            {
                return;
            }
            if (level[pos % w, pos / w] == 0)
            {
                return;
            }
            visited.Add(pos);
            if (pos % w != w - 1) dfs(level, pos + 1, visited);
            if (pos / w != h - 1) dfs(level, pos + w, visited);
            if (pos % w != 0) dfs(level, pos - 1, visited);
            if (pos / w != 0) dfs(level, pos - w, visited);
        }

        static Boolean isConnected(int[,] level)
        {
            int w = level.GetLength(0);
            int h = level.GetLength(1);
            List<int> visited = new List<int>();
            int pos = -1;
            for (int i = 0; i < w * h; i++)
            {
                if (level[i % w, i / w] == 1)
                {
                    pos = i;
                    break;
                }
            }
            if (pos != -1)
            {
                dfs(level, pos, visited);
            }
            int free = 0;
            for (int i = 0; i < w * h; i++)
            {
                if (level[i % w, i / w] == 1)
                {
                    free++;
                }
            }
            if (free == visited.Count)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        static void randomDivider(int[,] level)
        {
            int w = level.GetLength(0);
            int h = level.GetLength(1);
            divide(level, 5, 1, w - 2, 1, h - 2, 0, 3, true);
        }

        static void cutoutRect(int[,] level, int x1, int x2, int y1, int y2)
        {
            for (int i = x1; i <= x2; i++)
            {
                for (int j = y1; j <= y2; j++)
                {
                    level[i, j] = 0;
                }
            }
        }

        static void freeRect(int[,] level, int x1, int x2, int y1, int y2)
        {
            for (int i = x1; i <= x2; i++)
            {
                for (int j = y1; j <= y2; j++)
                {
                    level[i, j] = 1;
                }
            }
        }

        static void divide(int[,] level, int iter, int x1, int x2, int y1, int y2, int omit, int minRoomSize, Boolean horizontal)
        {
            Boolean cutout = (x2 - x1 > 15 || y2 - y1 > 15) ? false : random.NextDouble() < 0.7;
            iter -= random.Next(2);
            if (iter <= 0) return;
            int at = 0;
            if (horizontal)
            {
                if (y2 - y1 < minRoomSize * 2 + 2) return;
                do
                {
                    at = y1 + minRoomSize + random.Next(y2 - y1 - minRoomSize * 2);
                } while (at == omit);
                int nextOmit = random.Next(x2 - x1) + x1;
                for (int i = x1; i <= x2; i++)
                {
                    if (i == nextOmit) continue;
                    level[i, at] = 0;
                }
                if (at < omit && cutout)
                {
                    cutoutRect(level, x1, x2, y1, at - 1);
                    if (!isConnected(level))
                    {
                        freeRect(level, x1, x2, y1, at - 1);
                        divide(level, iter - 1, x1, x2, y1, at - 1, nextOmit, minRoomSize, false);
                    }
                }
                else
                {
                    divide(level, iter - 1, x1, x2, y1, at - 1, nextOmit, minRoomSize, false);

                }
                if (at > omit && cutout)
                {
                    cutoutRect(level, x1, x2, at + 1, y2);
                    if (!isConnected(level))
                    {
                        freeRect(level, x1, x2, at + 1, y2);
                        divide(level, iter - 1, x1, x2, at + 1, y2, nextOmit, minRoomSize, false);
                    }
                }
                else
                {
                    divide(level, iter - 1, x1, x2, at + 1, y2, nextOmit, minRoomSize, false);
                }
            }
            else
            {
                if (x2 - x1 < minRoomSize * 2 + 2) return;
                //print(random(3) + "\n");
                do
                {
                    at = x1 + minRoomSize + random.Next(x2 - x1 - minRoomSize * 2);
                } while (at == omit);
                int nextOmit = random.Next(y2 - y1) + y1;
                for (int i = y1; i <= y2; i++)
                {
                    if (i == nextOmit) continue;
                    level[at, i] = 0;
                }
                divide(level, iter - 1, x1, at - 1, y1, y2, nextOmit, minRoomSize, true);
                divide(level, iter - 1, at + 1, x2, y1, y2, nextOmit, minRoomSize, true);
            }
        }

        static void addRandomWalls(int[,] level)
        {
            int w = level.GetLength(0);
            int h = level.GetLength(1);
            int iterations = 5000;
            for (int i = 0; i < iterations; i++)
            {
                int cell = random.Next(levelWidth * levelHeight);
                int col = cell % w;
                int row = cell / w;
                if (col == 0 || col == w - 1 || row == 0 || row == h - 1)
                {
                    continue;
                }
                int beforeChange = level[col, row];
                level[col, row] = 0;
                int count = level[col - 1, row] + level[col + 1, row] +
                  level[col, row - 1] + level[col, row + 1];
                if (!isConnected(level) || count == 4 || count == 2 || count == 1 || count == 0)
                {
                    level[col, row] = beforeChange;
                }
            }
        }

        static void resetCells(int[,] level)
        {
            int w = level.GetLength(0);
            int h = level.GetLength(1);
            for (int j = 0; j < h; j++)
            {
                for (int i = 0; i < w; i++)
                {
                    level[i, j] = 0;
                }
            }
            for (int j = 1; j < h - 1; j++)
            {
                for (int i = 1; i < w - 1; i++)
                {
                    level[i, j] = 1;
                }
            }
        }

        static void randomAutomaton(int[,] level)
        {
            int w = level.GetLength(0);
            int h = level.GetLength(1);
            int[] cells = new int[(w - 2) * (h - 2)];
            for (int j = 1; j < h - 1; j++)
            {
                for (int i = 1; i < w - 1; i++)
                {
                    level[i, j] = random.NextDouble() < 0.5 ? 1 : 0;
                }
            }

            for (int i = 0; i < cells.Length; i++)
            {
                cells[i] = i;
            }
            shuffleArray(cells);

            for (int c = 0; c < cells.Length; c++)
            {
                int i = cells[c] % (w - 2) + 1;
                int j = cells[c] / (w - 2) + 1;
                int count = level[i - 1, j - 1] + level[i, j - 1] + level[i + 1, j - 1] +
                  level[i - 1, j] + level[i, j] + level[i + 1, j] +
                  level[i - 1, j + 1] + level[i, j + 1] + level[i + 1, j + 1];
                if (count >= 5)
                {
                    level[i, j] = 1;
                }
                else
                {
                    level[i, j] = 0;
                }
            }
        }
        static void shuffleArray(int[] ar)
        {
            for (int i = ar.Length - 1; i > 0; i--)
            {
                int index = random.Next(i + 1);
                // Simple swap
                int a = ar[index];
                ar[index] = ar[i];
                ar[i] = a;
            }
        }
    }
}