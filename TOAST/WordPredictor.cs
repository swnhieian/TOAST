using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;
using System.IO;
using System.Diagnostics;

namespace TOAST
{
    class WordPredictor
    {
        private Keyboard keyboard;
        private Dictionary<string, int> freqDict;
        //map: word-length  to： tuple(word, word frequency, hand code)
        private Dictionary<int, List<Tuple<string, int, string>>> lenFreqDict;
        public WordPredictor(Keyboard kbd)
        {
            keyboard = kbd;
            loadCorpus();
        }
        //hand code for a - z, 0 for left, 1 for right
        private string[] handCode = { "0", "0", "0", "0", "0", "0", "0",
        "1", "1", "1", "1", "1", "1", "1",
        "1", "1", "0", "0", "0", "0", "1", "0", "0", "0", "1", "0"};
        
        private List<string> getWordHandCode(string word)
        {
            List<string> ret = new List<string>();
            List<string> suffixCodes = new List<string>();
            if (word.Length > 1)
            {
                suffixCodes = getWordHandCode(word.Substring(1));
            } else
            {
                suffixCodes.Add("");
            }
            char c = word[0];
            if (c == 't' || c == 'y' || c == 'g' || c == 'h' || c =='b' || c == 'n')
            {
                foreach(string suffix in suffixCodes)
                {
                    ret.Add("0" + suffix);
                    ret.Add("1" + suffix);
                }
            } else
            {
                foreach(string suffix in suffixCodes)
                {
                    ret.Add(handCode[c - 'a'] + suffix);
                }
            }
            return ret;
        }
        private void loadCorpus()
        {
            String corpusPath = "Resources/wordFreq.json";
            String lenFreqPath = "Resources/lenFreq.json";
            string json = File.ReadAllText(corpusPath);
            freqDict = JsonConvert.DeserializeObject<Dictionary<string, int>>(json);
            if (File.Exists(lenFreqPath))
            {
                json = File.ReadAllText(lenFreqPath);
                lenFreqDict = JsonConvert.DeserializeObject<Dictionary<int, List<Tuple<string, int, string>>>>(json);
            } else
            {
                lenFreqDict = new Dictionary<int, List<Tuple<string, int, string>>>();
                foreach (var item in freqDict)
                {
                    int length = item.Key.Length;
                    if (!lenFreqDict.ContainsKey(length)) lenFreqDict.Add(length, new List<Tuple<string, int, string>>());
                    List<string> codes = getWordHandCode(item.Key);
                    foreach (string code in codes)
                    {
                        lenFreqDict[length].Add(new Tuple<string, int, string>(item.Key, item.Value, code));
                    }
                }
                json = JsonConvert.SerializeObject(lenFreqDict, Formatting.Indented);
                File.WriteAllText(lenFreqPath, json);
            }
        }
        public List<Tuple<string, double, string>> predict(List<Point> pointList)
        {
            int length = pointList.Count;
            if (!lenFreqDict.ContainsKey(length))
            {
                return new List<Tuple<string, double, string>>();
            }
            List<Tuple<string, double, string>> ret = new List<Tuple<string, double, string>>();
            foreach (var cand in lenFreqDict[length]) // cand: item1 word item2 freq item3 handcode
            {
                double prob = Math.Log(cand.Item2);
                // 0 stands for left and 1 for right
                int[] last = new int[2] { -1, -1 };
                for (int i=0; i< length; i++)
                {
                    int lr = int.Parse(cand.Item3[i].ToString());
                    Point actualPos = keyboard.LeftRightPositionParams[lr].inverseTransform(pointList[i]);
                    if (last[lr] == -1) // the first char using absolute model
                    {
                        double muX = keyboard.letterPosX[cand.Item1[i] - 'a'];
                        double muY = keyboard.letterPosY[cand.Item1[i] - 'a'];
                        double sigmaX = 20;
                        double sigmaY = 15;
                        prob += Config.logGaussianDistribution(actualPos.X, muX, sigmaX);
                        prob += Config.logGaussianDistribution(actualPos.Y, muY, sigmaY);
                    } else // use relative model
                    {
                        Point lastActualPos = keyboard.LeftRightPositionParams[lr].inverseTransform(pointList[last[lr]]);
                        double kbdVecX = keyboard.letterPosX[cand.Item1[i] - 'a'] - keyboard.letterPosX[cand.Item1[last[lr]] - 'a'];
                        double kbdVecY = keyboard.letterPosY[cand.Item1[i] - 'a'] - keyboard.letterPosY[cand.Item1[last[lr]] - 'a'];
                        double vecX = actualPos.X - lastActualPos.X;
                        double vecY = actualPos.Y - lastActualPos.Y;
                        double sigmaX = 20;
                        double sigmaY = 15;
                        prob += Config.logGaussianDistribution(vecX, kbdVecX, sigmaX);
                        prob += Config.logGaussianDistribution(vecY, kbdVecY, sigmaY);
                    }
                    last[lr] = i;
                }
                ret.Add(new Tuple<string, double, string>(cand.Item1, prob, cand.Item3));
            }
            ret.Sort((x, y) => { return y.Item2.CompareTo(x.Item2); });
            return ret;
        }
    }
}
