using UnityEngine;
using System.Collections.Generic;
using System;

namespace TriangularBattle
{
    public class CsvUtil : MonoBehaviour
    {
        private static List<string[]> ListData = new List<string[]>();
        private static List<string> listKeys = new List<string>();
        private static string currentFileName;

        public static bool parse(string filename)
        {
            if(ListData.Count!=0&&currentFileName==filename) return true;
            try
            {
                var textAsset = Resources.Load<TextAsset>(filename);
                if(textAsset==null) return false;
                parseText(textAsset.text);
                currentFileName=filename;
                return true;
            }
            catch(InvalidCastException e)
            {
                return false;
            }
        }

        public static void parseText(string text)
        {
            ListData.Clear();
            listKeys.Clear();

            string[] lines = text.Replace(Environment.NewLine, "\n").Split('\n');
            foreach(string line in lines)
            {
                if(string.IsNullOrWhiteSpace(line)) continue;
                var data = new List<string>(line.Split(','));
                string key = data[0];
                data.RemoveAt(0);
                listKeys.Add(key);
                ListData.Add(data.ToArray());
            }
        }

        public static string[] find(string key)
        {
            if(ListData.Count==0||!listKeys.Contains(key)) return null;
            return ListData[listKeys.IndexOf(key)];
        }

        public static int getLevelCount()
        {
            return ListData.Count-1;
        }
    }
}
