using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AlchemyEngine;

namespace AlchemyConsole
{
    class Program
    {
        private static Random rand;

        static void Main(string[] args)
        {
            rand = new Random();

            Tags tags = new Tags(rand.Next());

            AddTypes(tags);
            AddStrongAgainst(tags);
            AddSynthRules(tags);

            List<string> leftTags = new List<string>() { "Water", "Air"};
            List<string> rightTags = new List<string>() { "Air"};
            
            List<string> Combined = new List<string>();

            for (int i = 0; i < 10; i++)
            {
                Combined = tags.CombineTags(leftTags, rightTags);
                DisplayList(Combined);
            }

            Console.Read();
        }

        public static void DisplayList(List<string> list)
        {
            StringBuilder sbuild = new StringBuilder();
            sbuild.Append("[");
            foreach (string s in list)
            {
                sbuild.Append(s);
                sbuild.Append(",");
            }
            sbuild.Remove(sbuild.Length - 1, 1);
            sbuild.AppendLine("]");
            Console.WriteLine(sbuild);
        }

        public static void AddTypes(Tags tags)
        {
            tags.Types.Add("Fire");
            tags.Types.Add("Earth");
            tags.Types.Add("Air");
            tags.Types.Add("Water");
            tags.Types.Add("Dark");
        }

        public static void AddStrongAgainst(Tags tags)
        {
            HashSet<string> fireSet = new HashSet<string>();
            fireSet.Add("Dark");
            tags.StrongAgainst.Add("Fire", fireSet);

            HashSet<string> darkSet = new HashSet<string>();
            darkSet.Add("Earth");
            tags.StrongAgainst.Add("Dark", darkSet);

            HashSet<string> earthSet = new HashSet<string>();
            earthSet.Add("Air");
            tags.StrongAgainst.Add("Earth", earthSet);

            HashSet<string> airSet = new HashSet<string>();
            airSet.Add("Water");
            tags.StrongAgainst.Add("Air", airSet);

            HashSet<string> waterSet = new HashSet<string>();
            waterSet.Add("Fire");
            tags.StrongAgainst.Add("Water", waterSet);
        }

        public static void AddSynthRules(Tags tags)
        {
            Dictionary<string, string> fireSynth = new Dictionary<string, string>();
            fireSynth.Add("Earth", "Dark");
            tags.SynthRules.Add("Fire", fireSynth);

            Dictionary<string, string> earthSynth = new Dictionary<string, string>();
            earthSynth.Add("Water", "Air");
            tags.SynthRules.Add("Earth", earthSynth);

            Dictionary<string, string> airSynth = new Dictionary<string, string>();
            airSynth.Add("Fire", "Fire+");
            tags.SynthRules.Add("Air", airSynth);

            Dictionary<string, string> waterSynth = new Dictionary<string, string>();
            waterSynth.Add("Dark", "Dark+");
            tags.SynthRules.Add("Water", waterSynth);

            Dictionary<string, string> darkSynth = new Dictionary<string, string>();
            darkSynth.Add("Air", "Water");
            tags.SynthRules.Add("Dark", darkSynth);
        }
    }
}
