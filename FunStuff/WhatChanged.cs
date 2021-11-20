using System.Collections.Generic;

namespace FunStuff
{
    /// <summary>
    /// Given an original list and a changed list, produce a list explaining the changes that happened.
    /// Must be in order 
    /// </summary>
    public static class WhatChanged
    {
        public static List<(char id, char change)> GetChanges(char[] original, char[] changed)
        {
            List<(char id, char change)> changes = new List<(char id, char change)>();

            //m is original length n is changed length
            //Put in dictionary O(n)
            Dictionary<char, int> originalDict = new Dictionary<char, int>();
            for (int x = 0; x < original.Length; x++)
            {
                originalDict.Add(original[x], x);
            }

            int originalIndex = 0;

            //loop through changed list and check if in dictionary every loop
            //O(n + m)
            
            for (int x = 0; x < changed.Length; x++)
            {
                //O(1)
                //if dict does not contain key add to changed list
                if (!originalDict.ContainsKey(changed[x]))
                {
                    changes.Add((changed[x], '+'));
                    continue;
                }

                var index = originalDict[changed[x]];

                //Make sure that if we skip an index in the original list to add the ones we skipped to 
                //the changed list with a subtraction
                //Through all of the loops this will execute O(m) times
                for (int y = originalIndex; y < index; y++)
                {
                    changes.Add((original[y], '-'));
                }

                originalIndex = index + 1;


                changes.Add((changed[x], '.'));

            }
            //make sure we add the rest of the original
            for (int y = originalIndex; y < original.Length; y++)
            {
                changes.Add((original[y], '-'));
            }





            return changes;
        }
    }
}
