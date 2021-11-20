using System.Collections.Generic;

namespace FunStuff
{
    public static class WhatChanged
    {
        public static List<(char id, char change)> GetChanges(char[] original, char[] changed)
        {
            List<(char id, char change)> changes = new List<(char id, char change)>();

            Dictionary<char, int> originalDict = new Dictionary<char, int>();
            for (int x = 0; x < original.Length; x++)
            {
                originalDict.Add(original[x], x);
            }

            int originalIndex = 0;

            for (int x = 0; x < changed.Length; x++)
            {
                if (!originalDict.ContainsKey(changed[x]))
                {
                    changes.Add((changed[x], '+'));
                    continue;
                }

                var index = originalDict[changed[x]];


                for (int y = originalIndex; y < index; y++)
                {
                    changes.Add((original[y], '-'));
                }

                originalIndex = index + 1;


                changes.Add((changed[x], '.'));

            }

            for (int y = originalIndex; y < original.Length; y++)
            {
                changes.Add((original[y], '-'));
            }





            return changes;
        }
    }
}
