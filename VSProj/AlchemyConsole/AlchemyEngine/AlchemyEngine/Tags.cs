using System;
using System.Collections;
using System.Collections.Generic;

namespace AlchemyEngine
{
    public class Tags
    {
        HashSet<string> types;
        Dictionary<string, HashSet<string>> strongAgainst;
        Dictionary<string, Dictionary<string, string>> synthRules;
        Dictionary<string, string> kinds;

        public Tags()
        {

        }

        // Synth Rules
        public List<string> SynthRule(string left, string right)
        {
            List<string> newTags = new List<string>();
            newTags.Add(synthRules[left][right]);

            return newTags;
        }

        public List<string> BasicRule(string left, string right)
        {
            Random r = new Random();
            double prob = r.NextDouble();

            List<string> newTags = new List<string>();

            if (prob < 0.05)
            {
                return newTags;
            }
            else if (prob < 0.15)
            {
                newTags.Add(left);
                return newTags;
            }
            else if (prob < 0.25)
            {
                newTags.Add(right);
                return newTags;
            }
            else
            {
                newTags.Add(left);
                newTags.Add(right);
                return newTags;
            }
        }

        // Strong Rules
        public List<string> StrongRule(string left, string right)
        {
            List<string> newTags = new List<string>();
            Random r = new Random();
            double prob = r.NextDouble();

            if (strongAgainst.ContainsKey(left) && strongAgainst[left].Contains(right))
            {
                if (prob < 0.1)
                {
                    newTags.Add(right);
                    newTags.Add(left + "+");
                }
                else
                {
                    newTags.Add(left);
                }
            }
            if (strongAgainst.ContainsKey(right) && strongAgainst[left].Contains(left))
            {
                if (prob < 0.1)
                {
                    newTags.Add(left);
                    newTags.Add(right + "+");
                }
                else
                {
                    newTags.Add(right);
                }
            }

            return newTags;
        }

        // General Rules
        public List<string> CombineRule(string left)
        {
            Random r = new Random();
            double prob = r.NextDouble();

            List<string> newTags = new List<string>();

            if (prob < 0.8)
            {
                newTags.Add(left);
            }
            else
            {
                newTags.Add(left + "+");
            }
            return newTags;
        }
    }
}
