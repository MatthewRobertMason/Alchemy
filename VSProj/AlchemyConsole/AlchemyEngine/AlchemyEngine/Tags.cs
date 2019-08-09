using System;
using System.Collections;
using System.Collections.Generic;

namespace AlchemyEngine
{
    public class Tags
    {
        private HashSet<string> types;
        private Dictionary<string, HashSet<string>> strongAgainst;
        private Dictionary<string, Dictionary<string, string>> synthRules;
        private Dictionary<string, string> kinds;

        public HashSet<string> Types { get => types; set => types = value; }
        public Dictionary<string, HashSet<string>> StrongAgainst { get => strongAgainst; set => strongAgainst = value; }
        public Dictionary<string, Dictionary<string, string>> SynthRules { get => synthRules; set => synthRules = value; }
        public Dictionary<string, string> Kinds { get => kinds; set => kinds = value; }

        public Tags()
        {
            types = new HashSet<string>();
            strongAgainst = new Dictionary<string, HashSet<string>>();
            synthRules = new Dictionary<string, Dictionary<string, string>>();
        }

        public List<string> CombineTags (List<string> initLeft, List<string> initRight)
        {
            List<string> left = new List<string>(initLeft);
            List<string> right= new List<string>(initRight);

            List<string> newTags = new List<string>();
            Random rand = new Random();
            int leftIndex = 0;
            int rightIndex = 0;
            while (left.Count > 0 && right.Count > 0)
            {
                leftIndex = rand.Next(left.Count);
                rightIndex = rand.Next(right.Count);
                newTags.AddRange(Combine(left[leftIndex], right[rightIndex]));

                left.RemoveAt(leftIndex);
                right.RemoveAt(rightIndex);
            }

            newTags.AddRange(left);
            newTags.AddRange(right);

            return newTags;
        }

        enum ruleTypes
        {
            Synth,
            Strong,
            Basic,
            Combine
        }

        public List<string> Combine(string left, string right)
        {
            Random rand = new Random();

            int maxWeight = 0;
            // Left, Right, weight, rule
            List<Tuple<string, string, int, ruleTypes>> rules = new List<Tuple<string, string, int, ruleTypes>>();

            if (SynthRules.ContainsKey(left) && SynthRules[left].ContainsKey(right))
            {
                rules.Add(new Tuple<string, string, int, ruleTypes>(left, right, 5, ruleTypes.Synth));
                maxWeight += 5;
            }
            else if (SynthRules.ContainsKey(right) && SynthRules[right].ContainsKey(left))
            {
                rules.Add(new Tuple<string, string, int, ruleTypes>(right, left, 5, ruleTypes.Synth));
                maxWeight += 5;
            }
            if (StrongAgainst.ContainsKey(left) && StrongAgainst[left].Contains(right))
            {
                rules.Add(new Tuple<string, string, int, ruleTypes>(left, right, 6, ruleTypes.Strong));
                maxWeight += 6;
            }
            else if (StrongAgainst.ContainsKey(right) && StrongAgainst[right].Contains(left))
            {
                rules.Add(new Tuple<string, string, int, ruleTypes>(right, left, 6, ruleTypes.Strong));
                maxWeight += 6;
            }

            rules.Add(new Tuple<string, string, int, ruleTypes>(left, right, 1, ruleTypes.Basic));
            maxWeight += 1;

            if (left == right)
            { 
                rules.Add(new Tuple<string, string, int, ruleTypes>(left, right, 1, ruleTypes.Combine));
                maxWeight += 1;
            }

            int randomweight = rand.Next() % maxWeight;
            
            foreach(Tuple<string, string, int, ruleTypes> tuple in rules)
            {
                randomweight -= tuple.Item3;

                if (randomweight <= 0)
                {
                    switch (tuple.Item4)
                    {
                        case ruleTypes.Basic:
                            return BasicRule(tuple.Item1, tuple.Item2);
                            
                        case ruleTypes.Combine:
                            return CombineRule(tuple.Item1);
                            
                        case ruleTypes.Strong:
                            return StrongRule(tuple.Item1, tuple.Item2);

                        case ruleTypes.Synth:
                            return SynthRule(tuple.Item1, tuple.Item2);
                    }
                }
            }
            throw new Exception("Error with rules");
        }

        // Synth Rules
        public List<string> SynthRule(string left, string right)
        {
            List<string> newTags = new List<string>();
            newTags.Add(SynthRules[left][right]);

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

            if (StrongAgainst.ContainsKey(left) && StrongAgainst[left].Contains(right))
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
            if (StrongAgainst.ContainsKey(right) && StrongAgainst[left].Contains(left))
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
