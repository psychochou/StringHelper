using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using HtmlAgilityPack;
using SQLite;
using System.IO;

using System.Text.RegularExpressions;



namespace Shared
{
    public class S
    {
        public String chinese_medicine_name { get; set; }
        public String pinyin { get; set; }
        public String alias { get; set; }
        public String alias_for_deal { get; set; }
        public String functions { get; set; }
        public String indications { get; set; }
        public String dosage { get; set; }
        public String cautions { get; set; }
        public String toxicity { get; set; }
        public String clinical_reports { get; set; }
        public String pharmacodynamics { get; set; }
        public String commentary { get; set; }
        public String source { get; set; }

    }

    public class S1
    {
        public String alias_name { get; set; }
        public String common_name { get; set; }
        public String description { get; set; }
        public String dosage { get; set; }
        public String english_common_name { get; set; }
        public String geriatric_use { get; set; }
        public String indication { get; set; }
        public String interaction { get; set; }
        public String main_ingredient { get; set; }

        public String packages { get; set; }
        public String pediatric_use { get; set; }
        public String pharmacokinetics { get; set; }
        public String pharmacological_toxicology { get; set; }
        public String pinyin_common_name { get; set; }
        public String pregnant_lactating_use { get; set; }
        public String side_effect { get; set; }
        public String storage { get; set; }
        public String taboo { get; set; }
        public String overdose { get; set; }
    }

    public class S2
    {
        public String alias_name { get; set; }
        public String cautions { get; set; }
        public String elucldation { get; set; }
        public String english_name { get; set; }
        public String function { get; set; }
        public String indication { get; set; }
        public String ingredient { get; set; }
        public String key_symptom { get; set; }
        public String modern_application { get; set; }
        public String modern_research { get; set; }
        public String modification { get; set; }
        public String name { get; set; }
        public String pinyin { get; set; }
        public String selected_record { get; set; }
        public String source { get; set; }
        public String usage { get; set; }
        public String verse { get; set; }
    }
    public class SqliteModel
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }


        public string Title { get; set; }
        [Indexed]

        public string Tag { get; set; }
        public string Content { get; set; }
        public DateTime UpdateTime { get; set; }
        public DateTime CreateTime { get; set; }

    }
    public class Sqlites
    {
        private SQLiteConnection _connection;
        public Sqlites(String fileName)
        {
            Initialize(fileName);
        }

        private void Initialize(String fileName)
        {
            _connection = new SQLiteConnection(fileName);
            _connection.CreateTable<SqliteModel>();
        }

        public void Insert(SqliteModel model)
        {
            _connection.Insert(model);
        }
        public void Insert(List<SqliteModel> models)
        {
            _connection.InsertAll(models);
        }
        public void ImportOne(string dir, string tag)
        {
            var ls = new List<SqliteModel>();

            var files = Directory.GetFiles(dir, "*.html");

            foreach (var item in files)
            {
                var content = item.ReadAllText();
                var hd = new HtmlDocument();
                hd.LoadHtml(content);

                var keyTitle = hd.DocumentNode.SelectSingleNode("//*[@class='x3']");

                var node = hd.DocumentNode.SelectSingleNode("//*[@class='frame']");
                if (node != null)
                {
                    var sb = new StringBuilder();
                    var children = node.ChildNodes.ToArray();
                    if (children.Where(i => i.NodeType == HtmlNodeType.Element).Count() == 1)
                    {
                        node = children.Where(i => i.NodeType == HtmlNodeType.Element).First();
                        children = node.ChildNodes.ToArray();
                    }

                    foreach (var child in children)
                    {
                        if (child.NodeType == HtmlNodeType.Element)
                        {
                            var className = child.GetAttributeValue("class", "");
                            if (className == "frame2" || className == "frame") break;
                        }
                        var htmlContent = child.InnerHtml;


                        htmlContent = Regex.Replace(htmlContent, "<img[^>]*?>", new MatchEvaluator(m =>
                        {
                            //toggle = !toggle;
                            //if (toggle)
                            //{
                            //    return "【";

                            //}

                            return "|";
                        }));
                        child.InnerHtml = htmlContent;
                        sb.Append(child.InnerText);

                    }

                    var key = keyTitle.InnerText;
                    var textContent = sb.ToString();

                    var model = new SqliteModel
                    {
                        Title = key.Trim(),
                        Tag = tag,
                        Content = string.Join(Environment.NewLine + Environment.NewLine, textContent.Split(Environment.NewLine.ToCharArray()).Where(i => i.IsReadable()).Select(i => i.Trim())),
                        UpdateTime = DateTime.UtcNow,
                        CreateTime = DateTime.UtcNow,

                    };
                    if (ls.Where(i => i.Title == model.Title).Any())
                    {
                        model.GetHashCode();
                    }
                    else
                        ls.Add(model);
                }


            }
            Insert(ls);
        }

        public async void ImportTwo()
        {
            var client = new HttpClient(new HttpClientHandler
            {
                UseProxy = false
            });

            var ls = await GetList(client);
            foreach (var item in ls)
            {
                try
                {
                    var subList = await GetSingleList(client, item);
                    if (subList == null) continue;
                    foreach (var itemSub in subList)
                    {

                        InsertOne(client, itemSub);
                    }
                }
                catch { }
            }
        }

        public void ImportThree(String fileName)
        {

            var connection = new SQLiteConnection(fileName);
            var obj = connection.CreateCommand(@"select  t_chinese_medicine_category_relation.chinese_medicine_name,
t_chinese_medicine.pinyin,
t_chinese_medicine.alias,
t_chinese_medicine.alias_for_deal,
t_chinese_medicine.functions,
t_chinese_medicine.indications,
t_chinese_medicine.dosage,
t_chinese_medicine.cautions,
t_chinese_medicine.toxicity,
t_chinese_medicine.clinical_reports,
t_chinese_medicine.pharmacodynamics,
t_chinese_medicine.commentary,
t_chinese_medicine.source
 from t_chinese_medicine join t_chinese_medicine_category_relation on t_chinese_medicine.id=chinese_medicine_id", "").ExecuteQuery<S>();

            var hd = new HtmlDocument();

            var ls = new List<SqliteModel>();

            foreach (var item in obj)
            {
                var sb = new StringBuilder();


                sb.AppendLine(item.chinese_medicine_name).AppendLine();
                if (item.pinyin.IsReadable())
                    sb.AppendLine($"【拼音】{item.pinyin}").AppendLine();

                if (item.alias_for_deal.IsReadable())
                    sb.AppendLine($"【别名】{item.alias_for_deal}\r\n{item.alias}").AppendLine();

                if (item.functions.IsReadable())

                    sb.AppendLine($"【功能】{item.functions}").AppendLine();

                if (item.indications.IsReadable())

                    sb.AppendLine($"【适用症】{item.indications}").AppendLine();

                if (item.dosage.IsReadable())

                    sb.AppendLine($"【剂量】{item.dosage}").AppendLine();

                if (item.cautions.IsReadable())

                    sb.AppendLine($"【注意事项】{item.cautions}").AppendLine();

                if (item.toxicity.IsReadable())

                    sb.AppendLine($"【毒性】{item.toxicity}").AppendLine();

                if (item.pharmacodynamics.IsReadable())

                    sb.AppendLine($"【药效学】{item.pharmacodynamics}").AppendLine();

                if (item.clinical_reports.IsReadable())

                    sb.AppendLine($"【临床报告】{item.clinical_reports}").AppendLine();

                if (item.commentary.IsReadable())

                    sb.AppendLine($"【评注】{item.commentary}").AppendLine();

                if (item.source.IsReadable())

                    sb.AppendLine($"【来源】{item.source}").AppendLine();

                hd.LoadHtml(sb.ToString());

                var model = new SqliteModel
                {
                    Title = item.chinese_medicine_name,
                    Content = hd.DocumentNode.InnerText,
                    Tag = "zy",
                    UpdateTime = DateTime.UtcNow,
                    CreateTime = DateTime.UtcNow
                };
                //if (ls.Where(i => i.Title == model.Title).Any()) continue;
                ls.Add(model);

            }
            _connection.InsertAll(ls);
        }

        public void ImportFive(String fileName)
        {

            var connection = new SQLiteConnection(fileName);
            var obj = connection.CreateCommand(@"select 
t_drug.common_name,
t_drug.pinyin_common_name,
t_drug.alias_name,
t_drug.english_common_name,
t_drug.description,

t_drug.indication,
t_drug.dosage,
t_drug.side_effect,
t_drug.pharmacological_toxicology,
t_drug.main_ingredient,
t_drug.packages,
t_drug.storage,
t_drug.pharmacokinetics,
t_drug.interaction,
t_drug.pregnant_lactating_use,
t_drug.pediatric_use,
t_drug.geriatric_use,
t_drug.taboo,
t_drug.overdose

from t_drug", "").ExecuteQuery<S1>();

            var hd = new HtmlDocument();

            var ls = new List<SqliteModel>();

            foreach (var item in obj)
            {
                var sb = new StringBuilder();


                sb.AppendLine(item.common_name).AppendLine();
                if (item.pinyin_common_name.IsReadable())
                    sb.AppendLine($"【拼音】{item.pinyin_common_name}").AppendLine();

                if (item.alias_name.IsReadable())
                    sb.AppendLine($"【别名】{item.english_common_name}\r\n{item.alias_name}").AppendLine();

                if (item.description.IsReadable())

                    sb.AppendLine($"【描述】{item.description}").AppendLine();

                if (item.indication.IsReadable())

                    sb.AppendLine($"【适用症】{item.indication}").AppendLine();

                if (item.dosage.IsReadable())

                    sb.AppendLine($"【剂量】{item.dosage}").AppendLine();

                if (item.side_effect.IsReadable())

                    sb.AppendLine($"【副作用】{item.side_effect}").AppendLine();

                if (item.pharmacological_toxicology.IsReadable())

                    sb.AppendLine($"【药理毒理学】{item.pharmacological_toxicology}").AppendLine();

                if (item.main_ingredient.IsReadable())

                    sb.AppendLine($"【主要成分】{item.main_ingredient}").AppendLine();

                if (item.packages.IsReadable())

                    sb.AppendLine($"【包装】{item.packages}").AppendLine();

                if (item.storage.IsReadable())

                    sb.AppendLine($"【贮藏】{item.storage}").AppendLine();

                if (item.pharmacokinetics.IsReadable())

                    sb.AppendLine($"【药代动力学】{item.pharmacokinetics}").AppendLine();
                if (item.interaction.IsReadable())

                    sb.AppendLine($"【混用】{item.interaction}").AppendLine();

                if (item.pregnant_lactating_use.IsReadable())

                    sb.AppendLine($"【哺乳期】{item.pregnant_lactating_use}").AppendLine();
                if (item.pediatric_use.IsReadable())

                    sb.AppendLine($"【儿童】{item.pediatric_use}").AppendLine();

                if (item.geriatric_use.IsReadable())

                    sb.AppendLine($"【老人】{item.geriatric_use}").AppendLine();

                if (item.taboo.IsReadable())

                    sb.AppendLine($"【禁忌】{item.taboo}").AppendLine();

                if (item.overdose.IsReadable())

                    sb.AppendLine($"【过量】{item.overdose}").AppendLine();


                hd.LoadHtml(sb.ToString());

                var model = new SqliteModel
                {
                    Title = item.common_name,
                    Content = hd.DocumentNode.InnerText,
                    Tag = "xy",
                    UpdateTime = DateTime.UtcNow,
                    CreateTime = DateTime.UtcNow
                };
                if (ls.Where(i => i.Title == model.Title).Any()) continue;
                ls.Add(model);

            }
            _connection.InsertAll(ls);
        }

        public void ImportSix(String fileName)
        {

            var connection = new SQLiteConnection(fileName);
            var obj = connection.CreateCommand(@"
select

t_prescription.name,
t_prescription.pinyin,
t_prescription.english_name,

t_prescription.alias_name,
t_prescription.function,
t_prescription.key_symptom,

t_prescription.indication,
t_prescription.ingredient,
t_prescription.usage,

t_prescription.cautions,
t_prescription.elucldation,
t_prescription.source,

t_prescription.verse,
t_prescription.modification,
t_prescription.selected_record,

t_prescription.modern_application,
t_prescription.modern_research

from t_prescription", "").ExecuteQuery<S2>();

            var hd = new HtmlDocument();

            var ls = new List<SqliteModel>();

            foreach (var item in obj)
            {
                var sb = new StringBuilder();


                sb.AppendLine(item.name).AppendLine();
                if (item.pinyin.IsReadable())
                    sb.AppendLine($"【拼音】{item.pinyin}").AppendLine();

                if (item.alias_name.IsReadable())
                    sb.AppendLine($"【别名】{item.english_name}\r\n{item.alias_name}").AppendLine();


                if (item.function.IsReadable())
                    sb.AppendLine($"【功能】{item.function}").AppendLine();

                if (item.key_symptom.IsReadable())
                    sb.AppendLine($"【主要症状】{item.key_symptom}").AppendLine();



                if (item.indication.IsReadable())

                    sb.AppendLine($"【适用症】{item.indication}").AppendLine();

                if (item.ingredient.IsReadable())

                    sb.AppendLine($"【成分】{item.ingredient}").AppendLine();
                if (item.modification.IsReadable())

                    sb.AppendLine($"【修正】{item.modification}").AppendLine();

                if (item.usage.IsReadable())

                    sb.AppendLine($"【用法】{item.usage}").AppendLine();

                if (item.cautions.IsReadable())

                    sb.AppendLine($"【注意事项】{item.cautions}").AppendLine();

                if (item.elucldation.IsReadable())

                    sb.AppendLine($"【说明】{item.elucldation}").AppendLine();

                if (item.source.IsReadable())

                    sb.AppendLine($"【来源】{item.source}").AppendLine();

                if (item.verse.IsReadable())

                    sb.AppendLine($"【引用】{item.verse}").AppendLine();

                   if (item.selected_record.IsReadable())

                    sb.AppendLine($"【案例】{item.selected_record}").AppendLine();

                if (item.modern_application.IsReadable())

                    sb.AppendLine($"【现代应用】{item.modern_application}").AppendLine();
                if (item.modern_research.IsReadable())

                    sb.AppendLine($"【现代研究】{item.modern_research}").AppendLine();
                 

                hd.LoadHtml(sb.ToString());

                var model = new SqliteModel
                {
                    Title = item.name,
                    Content = hd.DocumentNode.InnerText,
                    Tag = "fj",
                    UpdateTime = DateTime.UtcNow,
                    CreateTime = DateTime.UtcNow
                };
                if (ls.Where(i => i.Title == model.Title).Any()) continue;
                ls.Add(model);

            }
            _connection.InsertAll(ls);
        }
        private async Task<List<String>> GetSingleList(HttpClient client, string url)

        {
            var res = await client.GetAsync(url);
            var content = await res.Content.ReadAsStringAsync();


            var hd = new HtmlDocument();
            hd.LoadHtml(content);

            var nodes = hd.DocumentNode.SelectNodes("//*[contains(@class,'content')]//a");

            if (nodes.Any())
            {
                var ls = new List<string>();
                foreach (var item in nodes)
                {
                    var h = item.GetAttributeValue("href", "");
                    if (!ls.Contains(h))
                        ls.Add("http://ypk.39.net" + h + "manual");

                }
                return ls.Distinct().ToList();
            }


            return null;
        }

        public async void InsertOne(HttpClient client, string url)

        {
            var res = await client.GetAsync(url);
            var content = await res.Content.ReadAsStringAsync();


            var hd = new HtmlDocument();
            hd.LoadHtml(content);

            var nodes = hd.DocumentNode.SelectSingleNode("//*[@class='tab_box']");

            if (nodes != null)
            {
                //var sb = new StringBuilder();

                //var children = nodes.SelectNodes("//tr");

                //foreach (var item in children)
                //{
                //    var v = item.InnerText;
                //    v = Regex.Replace(v, "\\s+", "");
                //    if (v.IsReadable() && !v.StartsWith("【生产企业】"))
                //        sb.AppendLine(v.Replace("&nbsp;", ""));
                //}
                var model = new SqliteModel
                {
                    Title = hd.DocumentNode.SelectSingleNode("//*[@class='t1']/h1").InnerText.Trim(),
                    Tag = "xy",
                    Content = String.Join(Environment.NewLine, nodes.InnerText.Trim().Split("\n".ToCharArray()).Where(i => i.IsReadable()).Select(i => i.Trim())),
                    UpdateTime = DateTime.UtcNow,
                    CreateTime = DateTime.UtcNow,
                };
                try
                {
                    _connection.Insert(model);
                }
                catch (Exception e)
                {
                    var i = 0;
                }
            }



        }
        private async Task<List<String>> GetList(HttpClient client)
        {
            var res = await client.GetAsync("http://ypk.39.net/AllCategory");
            var content = await res.Content.ReadAsStringAsync();


            var hd = new HtmlDocument();
            hd.LoadHtml(content);

            var nodes = hd.DocumentNode.SelectNodes("//*[contains(@class,'classification')]//dd/a");

            if (nodes.Any())
            {
                var ls = new List<string>();
                foreach (var item in nodes)
                {
                    var h = item.GetAttributeValue("href", "");
                    if (h.IsVacuum()) continue;
                    ls.Add("http://ypk.39.net" + h);

                }
                return ls.Distinct().ToList();
            }


            return null;


        }
    }
}
