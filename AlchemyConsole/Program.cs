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

            Tags tags = new Tags("../../data/five_elements.yaml");

            List<string> leftTags = new List<string>() { "metal", "metal" };
            List<string> rightTags = new List<string>() { "water", "fire" };

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
            sbuild.Append("]");
            Console.WriteLine(sbuild);
        }
    }
}
