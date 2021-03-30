using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DumbSolver
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.SetWindowSize(Console.LargestWindowWidth - 35, Console.LargestWindowHeight);
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            string[] allWords = System.IO.File.ReadAllLines(@"words.txt");
            IEnumerable<string> longWords = allWords.Where(word => word.Length > 2);

            Console.InputEncoding = Encoding.GetEncoding(1251);

            string gridAsWord =  Console.ReadLine();

            char[,] wordGrid = GetGrid(gridAsWord);


            Console.OutputEncoding = Encoding.GetEncoding(1251);

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    DFS(wordGrid, allWords, wordGrid[i,j].ToString() , new bool[4, 4], i, j);//start everywhere
                }
            }

            FoundWords.Sort(delegate (string x, string y)
            {
                if (x.Length == y.Length) return 0;
                return y.Length - x.Length;
            });
            foreach (var sortedWord in FoundWords)  Console.WriteLine(sortedWord); 

        }

        static char[,] GetGrid(string input)
        {
            char[,] grid = new char[4, 4];
            int index = 0;

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    grid[i, j] = input[index];
                    index++;
                }
            }

            return grid;
        }

        static void DFS(char[,] grid, string[] allWords, string currentWord, bool[,] visited, int currentI, int currentJ)
        {
            if (currentWord.Length > 10)//bottom
            {
                return;
            }

            if (currentWord.Length >= 3 && IsContained(currentWord, allWords) && !FoundWords.Contains(currentWord))
            {
                FoundWords.Add(currentWord);
                //Console.WriteLine(currentWord);
            }

            visited[currentI, currentJ] = true;

            //go left
            if (currentJ > 0 && !visited[currentI, currentJ - 1])
            {
                DFS(grid, allWords, currentWord + grid[currentI, currentJ - 1], visited, currentI, currentJ - 1);
            }

            //go right
            if (currentJ < 3 && !visited[currentI, currentJ + 1])
            {
                DFS(grid, allWords, currentWord + grid[currentI, currentJ + 1], visited, currentI, currentJ + 1);
            }

            //go up
            if (currentI > 0 && !visited[currentI - 1, currentJ])
            {
                DFS(grid, allWords, currentWord + grid[currentI - 1 , currentJ], visited, currentI - 1, currentJ);
            }

            //go down
            if (currentI < 3 && !visited[currentI + 1, currentJ])
            {
                DFS(grid, allWords, currentWord + grid[currentI + 1, currentJ], visited, currentI + 1, currentJ);
            }

            //go left up
            if (currentI > 0 && currentJ > 0 && !visited[currentI - 1, currentJ - 1])
            {
                DFS(grid, allWords, currentWord + grid[currentI - 1, currentJ - 1], visited, currentI - 1, currentJ - 1);
            }

            //go left down
            if (currentI < 3 && currentJ > 0 && !visited[currentI + 1, currentJ - 1])
            {
                DFS(grid, allWords, currentWord + grid[currentI + 1, currentJ - 1], visited, currentI + 1, currentJ - 1);
            }

            //right up
            if (currentI > 0 && currentJ < 3 && !visited[currentI - 1, currentJ + 1])
            {
                DFS(grid, allWords, currentWord + grid[currentI - 1, currentJ + 1], visited, currentI - 1, currentJ + 1);
            }

            //go right down
            if (currentI < 3 && currentJ < 3 && !visited[currentI + 1, currentJ + 1])
            {
                DFS(grid, allWords, currentWord + grid[currentI + 1, currentJ + 1], visited, currentI + 1, currentJ + 1);
            }

            visited[currentI, currentJ] = false;
        }

        static bool IsContained(string searchedWord, string[] allWords)
        {
            int leftIndex = 0;
            int rightIndex = allWords.Length - 1;
            int middleIndex, diff;

            if (allWords[leftIndex] == searchedWord) return true;
            if (allWords[rightIndex] == searchedWord) return true;

            while (rightIndex - leftIndex > 1)
            {
                middleIndex = (leftIndex + rightIndex) / 2;

                diff = string.Compare(allWords[middleIndex], searchedWord);
                if (diff == 0) return true;

                if ( diff > 0)
                {
                    rightIndex = middleIndex;
                }
                else
                {
                    leftIndex = middleIndex;
                }
            }

            return false;
        }

        static List<string> FoundWords = new List<string>();
    }
}
