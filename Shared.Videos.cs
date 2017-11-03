using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace Shared
{
    public class Videos
    {

        private static string GenerateTimeSpan(string value, int addMout)
        {
            var splited = value.Split(':').Select(i => int.Parse(i)).ToArray();
            var t = splited[1] * 60 + splited[2];
            t = t + addMout;
            var r = $"[{(t / 60).ToString().PadLeft(2, '0')}:{(t % 60).ToString().PadLeft(2, '0')}.00]";
            return r;
        }
        public static void ConvertSrtToLrc(string fileName, int addMout = -3)
        {

            var lines = File.ReadAllLines(fileName, new UTF8Encoding(false));

            var sb = new StringBuilder();

            var matchNumber = new Regex("^[0-9]+$", RegexOptions.Multiline);
            var matchTime = new Regex("^[0-9]{2}:[0-9]{2}:[0-9]{2}", RegexOptions.Multiline);

            foreach (var item in lines)
            {
                if (item.IsVacuum() || matchNumber.IsMatch(item)) continue;

                if (matchTime.IsMatch(item))
                {
                    sb.Append($"\r\n{GenerateTimeSpan(matchTime.Match(item).Value, addMout)}");
                }
                else
                {
                    sb.Append(item.Trim() + " ");
                }
            }

            fileName.ChangeExtension("lrc").WriteAllText(sb.ToString().Trim());
        }
    }
}
