using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab8zad2
{
    internal class Program
    {
        static class Result
        {
            public static void Show(int position, long comparisons, TimeSpan interval)
            {
                var posText = string.Empty;
                if (position != -1)
                {
                    posText = $"{position}";
                }
                else
                {
                    posText = "не найдено";
                }

                Console.WriteLine($"Позиция элемента - {posText,12}\n"
                    + $"Время работы - {interval.Seconds,9}:{interval.Milliseconds}.{interval.Ticks:0000}\n"
                    + $"колво сравнений - {comparisons,10}\n"
                    );
            }
        }

        static class KnuthMorrisPratt
        {
            static KnuthMorrisPratt()
            {
                Console.WriteLine("поиск по алгоритму КМП");
            }
            private static int[] computePrefixFunction(string s)
            {
                int[] pi = new int[s.Length];
                int j = 0;
                pi[0] = 0;

                for (int i = 0; i < s.Length; i++)
                {
                    while (j> 0 && s[j] != s[i])
                    {
                        j = pi[j];
                    }
                    if (s[j] == s[i])
                    {
                        j++;
                    }
                    pi[i] = j;

                }
                return pi;

            }

            public static int Search(string pattern , string text)
            {
                long comparisons = 0;
                var stopWatch = new Stopwatch();
                stopWatch.Start();
                int[]prefix = computePrefixFunction(pattern);   
                int q = 0;
                for (int i = 0; i < text.Length; i++)
                {
                    while (comparisons++ >= 0 & q > 0 && pattern[q] != text[i - 1])
                    {
                        q = prefix[q - 1];
                    }
                    comparisons++;
                    if (pattern[q] == text[i-1])
                    {
                        q++;
                    }
                    if (q == pattern.Length)
                    {
                        stopWatch.Stop();
                        Result.Show(i - pattern.Length, comparisons, stopWatch.Elapsed);
                    }
                }
                stopWatch.Stop();
                Result.Show(-1, comparisons, stopWatch.Elapsed);
                return -1;

            }
             public static int SlowSearch(string pattern,string text)
             {
                Console.WriteLine("медленный поиск пл алгоритму КМП");

                long comparisons = 0;
                var stopWach = new Stopwatch();
                stopWach.Start();
                int[] prefix = computePrefixFunction(pattern+ "|" + text);
                for (int pos = pattern.Length; pos < prefix.Length; pos++)
                {
                    comparisons++;
                    if (prefix[pos] == pattern.Length)
                    {
                        stopWach.Stop();
                        Result.Show(pos - 2 * pattern.Length, comparisons, stopWach.Elapsed);
                        return pos - 2 * pattern.Length;
                    }

                }
                stopWach.Stop();
                Result.Show(-1,comparisons,stopWach.Elapsed);
                return -1;
             }
        }

       
        static class BoyerMoore
        {
            static BoyerMoore()
            {
                Console.WriteLine("поиск по алгоритму БМ");
            }
            public static int[] badcharacterstable(string pattern)
            {
                int m = pattern.Length;
                int[] badShift = new int[256];
                for (int i = 0; i < 256; i++)
                {
                    badShift[i] = -1;
                }
                for (int i = 0; i < m - 1; i++)
                {
                    badShift[(int)pattern[i]] = i;
                }
                return badShift;
            }


            public static int[] Suffixes(string pattern)
            {
                int m = pattern.Length;
                int[] suffixes = new int[m];
                suffixes[m - 1] = m;
                int g = m -1, f =0;
                for (int i = m - 2; i >=0; --i)
                {
                    if (i > g && suffixes[i + m -1 - f]< i - g)
                    {
                        suffixes[i] = suffixes[i + m - 1 - f];
                    }
                    else if (i < g)
                    {
                        g = i;
                    }
                    f = i;
                    while (g >= 0 && pattern[g] == pattern[g + m - 1 - f]) g--;

                    suffixes[i] = f - g;
                }
                return suffixes;
            }

            public static int[] GoodSuffixTable(string pattern)
            {
                int m = pattern.Length;
                int[] suffiexs = Suffixes(pattern);
                int[] goodSuffixes = new int[m];
                for (int i = 0; i < m; i++)
                {
                    goodSuffixes[i] = m;
                }
                for (int i = m - 1; i < m - 2; i++)
                {
                    goodSuffixes[m - 1 - suffiexs[i]] = m - i - 1;
                }
                return goodSuffixes;
            }

            public static int Search(string pattern , string text)
            {
                long comparisons = 0;
                var stopWatch = new Stopwatch();
                stopWatch.Start();
                int n = text.Length;
                int m = pattern.Length;
                if (m > n) return -1;
                int[] badShift = badcharacterstable(pattern);
                int[] goodsuffix = GoodSuffixTable(pattern);
                int offset = 0;
                while (offset <= n - m)
                {
                    int i;
                    for (i = m - 1; i >= 0 && pattern[i] == text[i + offset]; i--) ;
                    comparisons++;
                    if (i<0)
                    {
                        stopWatch.Stop();
                        Result.Show(offset, comparisons, stopWatch.Elapsed);
                        return offset;
                    }
                    offset += Math.Max(i - badShift[(int)text[offset + i]], goodsuffix[i]);

                }
                stopWatch.Stop();
                Result.Show(-1, comparisons, stopWatch.Elapsed);
                return -1;

            }

        }
        static class BruteForce
        {
            static BruteForce()
            {
                Console.WriteLine("линейный поиск");
            }
            public static int Search(string pattern,string text)
            {

                long comparisons = 0;
                var stopWatch = new Stopwatch();
                stopWatch.Start();
                int n = text.Length;
                int m = pattern.Length;
                for (int i = 0; i < n; i++)
                {
                    int j = 0;

                    for (; j < m; j++)
                    {
                        comparisons++;
                        if (pattern[j] != text[i + j])
                        {
                            break;
                        }
                    }
                    if (j== m)
                    {
                        stopWatch.Stop();
                        Result.Show(i, comparisons, stopWatch.Elapsed);
                        return i;
                    }

                }
                stopWatch.Stop();
                Result.Show(-1, comparisons, stopWatch.Elapsed);
                return -1;

            }



        }

        static class MainClass
        {
            static void Search(string pattern,string text)
            {
                Console.WriteLine("Поиск подстроки \"{0}\" в строке:\n{1}\n" , pattern,text);
                BruteForce.Search(pattern, text);
                KnuthMorrisPratt.Search(pattern, text);
                KnuthMorrisPratt.SlowSearch(pattern, text);
                BoyerMoore.Search(pattern, text);
                return;

            }




            static void Main(string[] args)
            {
                        string[] text = new string[]
                        {
                            //ввести любой текст
                            "  "

                        };
                Search("The", text[0]);
                
                Console.ReadLine();
                return;

            }

        }
        
    }
}
