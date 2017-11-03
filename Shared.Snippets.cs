using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System.IO;

using System.Text.RegularExpressions;

namespace Shared
{

    static class Snippets
    {


        public static void GenerateSublimeCompletionsFromDirectory(string dir)
        {
            Dictionary<string, dynamic> dictionary = new Dictionary<string, dynamic>();

            const string scope = "source.c, source.c++";

            dictionary.Add("scope", scope);

            var dictionaryList = new List<Dictionary<string, string>>();

            var files = Directory.GetFiles(dir).Where(i => i.EndsWith(".sublime-snippet"));

            foreach (var item in files)
            {
                var dic = ParseSingleSublimeFile(item);

                dictionaryList.Add(dic);
            }

            dictionaryList = dictionaryList.OrderBy(i => i["tabTrigger"]).ToList();
            dictionary.Add("completions", dictionaryList);

            var targetFileName = @"C:\psycho\.RAR\Sublime Text Build 3143\Data\Packages\User\C.sublime-completions";


            targetFileName.WriteAllText(JsonConvert.SerializeObject(dictionary));

        }

        public static Dictionary<string, string> ParseSingleSublimeFile(string fileName)
        {


            var dic = new Dictionary<string, string>();

            var doc = new HtmlDocument();

            doc.LoadHtml(fileName.ReadAllText());

            var childs = doc.DocumentNode.Descendants().ToArray();

            var tabTrigger = childs.Where(i => i.Name == "tabtrigger").First();

            var content = childs.Where(i => i?.Name == "content").First();
            dic.Add("tabTrigger", tabTrigger.InnerText);

            const string h = "<![CDATA[";
            const string e = "]]>";
            var str = content.InnerText;
            str = str.Substring(h.Length);
            str = str.Substring(0, str.Length - e.Length);
            dic.Add("content", str);



            return dic;


        }

        public static void AddSublimeCompletions(string fileName, Dictionary<string, string> dictionary)
        {

            var obj = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(fileName.ReadAllText());

            List<Dictionary<string, string>> ls = obj["completions"].ToObject<List<Dictionary<string, string>>>();
            bool isUpdate = false;
            foreach (var item in ls)
            {
                if (item["trigger"] == dictionary["trigger"])
                {
                    item["contents"] = dictionary["contents"];
                    isUpdate = true;
                }
            }
            if (!isUpdate)
                ls.Add(dictionary);
            obj["completions"] = ls;
            fileName.WriteAllText(JsonConvert.SerializeObject(obj));

        }
        public static (string, string) FormatMSDNWin32Function(string value)
        {
            var v = value.FlatToLine();
            var fnName = Regex.Match(v, "([a-zA-Z_0-9]+) *\\(").Groups[1].Value;
            var argLs = v.Split(',').Where(i => i.IsReadable()).Select(i => i.Trim() + ",");

            var sb = new StringBuilder();
            var count = 1;
            var content = "";
            sb.Append(fnName).Append("(");
            foreach (var item in argLs)
            {

                var a = Regex.Match(item, "([a-zA-Z_0-9]+)(?=( *,)|( *\\)))");
                sb.Append($"${{{count}:{a}}}").Append(",");
                count++;
            }
            content = sb.ToString().TrimEnd(',') + ")";
            return (fnName, content);

        }
    }
}
