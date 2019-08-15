﻿using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace AlchemyEngine
{
    public class Tags
    {
        private Random rand;
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
            rand = new Random();
            types = new HashSet<string>();
            strongAgainst = new Dictionary<string, HashSet<string>>();
            synthRules = new Dictionary<string, Dictionary<string, string>>();
            kinds = new Dictionary<string, string>();
        }

        public Tags(string dataFilePath)
        {
            rand = new Random();

            types = new HashSet<string>();
            strongAgainst = new Dictionary<string, HashSet<string>>();
            synthRules = new Dictionary<string, Dictionary<string, string>>();
            kinds = new Dictionary<string, string>();

            var root = JObject.Parse(System.IO.File.ReadAllText(dataFilePath));
            var tags = (JObject)root["tags"];

            // Set the keys
            foreach (var row in tags)
            {
                types.Add(row.Key);
                strongAgainst[row.Key] = new HashSet<string>();
                synthRules[row.Key] = new Dictionary<string, string>();
            }

            // Load the properties of each pairing
            foreach (var row in tags)
            {
                var data = (JObject)row.Value;

                // Note what things are strong against this
                foreach (string stronger in data["weakness"])
                {
                    if (!types.Contains(stronger))
                        throw new Exception(stronger + " is not a tag type.");
                    strongAgainst[stronger].Add(row.Key);
                }

                // Note what things this synthesises whith 
                JToken synths;
                if (data.TryGetValue("synth", StringComparison.Ordinal, out synths))
                {
                    foreach (var product in (JObject)synths)
                    {
                        if (!types.Contains(product.Key))
                            throw new Exception(product.Key + " is not a tag type.");
                        if (!types.Contains((string)product.Value))
                            throw new Exception((string)product.Value + " is not a tag type.");
                        synthRules[row.Key][product.Key] = (string)product.Value;
                    }
                }

                // Check if there is a kind
                JToken kind;
                if (data.TryGetValue("kind", StringComparison.Ordinal, out kind))
                {
                    if (!types.Contains((string)kind))
                        throw new Exception((string)kind + " is not a tag type.");
                    kinds[row.Key] = (string)kind;
                }
            }
        }

        public List<string> CombineTags(List<string> initLeft, List<string> initRight)
        {
            List<string> left = new List<string>(initLeft);
            List<string> right = new List<string>(initRight);

            List<string> newTags = new List<string>();

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

            foreach (Tuple<string, string, int, ruleTypes> tuple in rules)
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
            double prob = rand.NextDouble();

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
            double prob = rand.NextDouble();

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
            double prob = rand.NextDouble();

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
