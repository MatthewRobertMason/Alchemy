using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace AlchemyEngine
{
    public class Tags
    {
        HashSet<string> types;
        Dictionary<string, HashSet<string>> strongAgainst;
        Dictionary<string, Dictionary<string, string>> synthRules;
        Dictionary<string, string> kinds;

        public Tags(string dataFilePath)
        {
            types = new HashSet<string>();
            strongAgainst = new Dictionary<string, HashSet<string>>();
            synthRules = new Dictionary<string, Dictionary<string, string>>();
            kinds = new Dictionary<string, string>();

            var root = JObject.Parse(System.IO.File.ReadAllText(dataFilePath));
            var tags = (JObject)root["tags"];

            // Set the keys
            foreach(var row in tags){
                types.Add(row.Key);
                strongAgainst[row.Key] = new HashSet<string>();
                synthRules[row.Key] = new Dictionary<string, string>();
            }

            // Load the properties of each pairing
            foreach(var row in tags){
                var data = (JObject)row.Value;

                // Note what things are strong against this
                foreach(string stronger in data["weakness"]){
                    if(!types.Contains(stronger)) 
                        throw new Exception(stronger + " is not a tag type.");
                    strongAgainst[stronger].Add(row.Key);
                }

                // Note what things this synthesises whith 
                JToken synths;
                if(data.TryGetValue("synth", StringComparison.Ordinal, out synths)){
                    foreach(var product in (JObject)synths){
                        if(!types.Contains(product.Key)) 
                            throw new Exception(product.Key + " is not a tag type.");
                        if(!types.Contains((string)product.Value)) 
                            throw new Exception((string)product.Value + " is not a tag type.");
                        synthRules[row.Key][product.Key] = (string)product.Value;
                    }
                }

                // Check if there is a kind
                JToken kind;
                if(data.TryGetValue("kind", StringComparison.Ordinal, out kind)){
                    if(!types.Contains((string)kind))
                        throw new Exception((string)kind + " is not a tag type.");
                    kinds[row.Key] = (string)kind;
                }
            }
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
