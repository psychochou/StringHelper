using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.SQLite;
using System.Text.RegularExpressions;

namespace Shared
{
    class SqliteTerms
    {
        private class Model
        {
            public string title;
            public string content;
            public int drug_id;

        }
        SQLiteConnection _sqliteConnection;
        public SqliteTerms(string fileName)
        {
            _sqliteConnection = new SQLiteConnection($"Data Source={fileName};Version=3;");
            _sqliteConnection.Open();
            //var cmd = new SQLiteCommand("drop table _search", _sqliteConnection);
            //cmd.ExecuteNonQuery();

            //cmd = new SQLiteCommand("CREATE VIRTUAL TABLE IF NOT EXISTS _search USING fts4(title,content1,content2,drug_id, drug_type, tokenize=simple)", _sqliteConnection);
            //cmd.ExecuteNonQuery();
            //cmd.Dispose();
        }

        public void Execute()
        {
            String columns = "id,name,alias,source,harvest_preparation,commercial_specification,properties,channels_tropism,functions,indications,dosage,cautions,commentary,chemical_compositions,pharmacological_effects,pharmacodynamics,clinical_reports,toxicity";
            //,`references`


            var cmd = new SQLiteCommand("select " + columns + " from t_chinese_medicine", _sqliteConnection);
            var cursor = cmd.ExecuteReader();
            var g = new JiebaNet.Segmenter.JiebaSegmenter();


            var models = new List<Model>();

            while (cursor.Read())
            {
                try
                {

                    var content = "";
                    if (cursor[2] != null)
                    {
                        content += cursor[2].ToString();
                    }
                    if (cursor[3] != null)
                    {
                        content += cursor[3].ToString();
                    }
                    if (cursor[4] != null)
                    {
                        content += cursor[4].ToString();
                    }
                    if (cursor[5] != null)
                    {
                        content += cursor[5].ToString();
                    }
                    if (cursor[6] != null)
                    {
                        content += cursor[6].ToString();
                    }
                    if (cursor[7] != null)
                    {
                        content += cursor[7].ToString();
                    }
                    if (cursor[8] != null)
                    {
                        content += cursor[8].ToString();
                    }
                    if (cursor[9] != null)
                    {
                        content += cursor[9].ToString();
                    }
                    if (cursor[10] != null)
                    {
                        content += cursor[10].ToString();
                    }
                    if (cursor[11] != null)
                    {
                        content += cursor[11].ToString();
                    }
                    if (cursor[12] != null)
                    {
                        content += cursor[12].ToString();
                    }
                    if (cursor[13] != null)
                    {
                        content += cursor[13].ToString();
                    }
                    if (cursor[14] != null)
                    {
                        content += cursor[14].ToString();
                    }
                    if (cursor[15] != null)
                    {
                        content += cursor[15].ToString();
                    }
                    if (cursor[16] != null)
                    {
                        content += cursor[16].ToString();
                    }
                    if (cursor[17] != null)
                    {
                        content += cursor[17].ToString();
                    }
                    //if (cursor.GetString(1) == "金银花")
                    //{
                    //    var s = cursor[16];
                    //    var i = 0;
                    //}
                    var ls = new List<string>();
                    if (Regex.IsMatch(content, "手足口"))
                    {
                        ls.Add("手足口");
                    }
                    content = content.RemoveNonChinese();

                    //if (cursor.GetString(1) == "金银花")
                    //{
                    //    ls.Add("剧毒");
                    //}
                    var sls = g.Cut(content);
                    ls.AddRange(sls);
                    models.Add(new Model
                    {
                        title = cursor.GetString(1),
                        content = string.Join(" ", ls),
                        drug_id = cursor.GetInt32(0),
                    });
                }

                catch (Exception ex)
                {



                    var i = 0;
                }

            }
            //cmd.Dispose();

            var t = _sqliteConnection.BeginTransaction();

            cmd = new SQLiteCommand("insert into _search (title,content1,content2, drug_id, drug_type) values (@title,@content1,@content2,@drug_id,4)", _sqliteConnection);
            cmd.Transaction = t;
            foreach (var item in models)
            {
                cmd.Parameters.AddWithValue("@title", item.title);
                cmd.Parameters.AddWithValue("@content1", string.Join(" ", item.title.ToCharArray()));
                cmd.Parameters.AddWithValue("@content2", item.content);

                cmd.Parameters.AddWithValue("@drug_id", item.drug_id);
                cmd.ExecuteNonQuery();
            }
            t.Commit();
            t.Dispose();
            // g.Tokenize("", JiebaNet.Segmenter.TokenizerMode.Search).Select(i => i.Word);


        }
        public void Execute1()
        {
            String columns = "id,common_name,main_ingredient,description,indication,dosage,side_effect,taboo,cautions,other,warning,pregnant_lactating_use,pediatric_use,geriatric_use,interaction,overdose,clinical_trials,pharmacological_toxicology,pharmacokinetics,storage";
            //,`references`


            var cmd = new SQLiteCommand("select " + columns + " from t_drug where type=1", _sqliteConnection);
            var cursor = cmd.ExecuteReader();
            var g = new JiebaNet.Segmenter.JiebaSegmenter();


            var models = new List<Model>();

            while (cursor.Read())
            {
                try
                {

                    var content = "";
                    if (cursor[2] != null)
                    {
                        content += cursor[2].ToString();
                    }
                    if (cursor[3] != null)
                    {
                        content += cursor[3].ToString();
                    }
                    if (cursor[4] != null)
                    {
                        content += cursor[4].ToString();
                    }
                    if (cursor[5] != null)
                    {
                        content += cursor[5].ToString();
                    }
                    if (cursor[6] != null)
                    {
                        content += cursor[6].ToString();
                    }
                    if (cursor[7] != null)
                    {
                        content += cursor[7].ToString();
                    }
                    if (cursor[8] != null)
                    {
                        content += cursor[8].ToString();
                    }
                    if (cursor[9] != null)
                    {
                        content += cursor[9].ToString();
                    }
                    if (cursor[10] != null)
                    {
                        content += cursor[10].ToString();
                    }
                    if (cursor[11] != null)
                    {
                        content += cursor[11].ToString();
                    }
                    if (cursor[12] != null)
                    {
                        content += cursor[12].ToString();
                    }
                    if (cursor[13] != null)
                    {
                        content += cursor[13].ToString();
                    }
                    if (cursor[14] != null)
                    {
                        content += cursor[14].ToString();
                    }
                    if (cursor[15] != null)
                    {
                        content += cursor[15].ToString();
                    }
                    if (cursor[16] != null)
                    {
                        content += cursor[16].ToString();
                    }
                    if (cursor[17] != null)
                    {
                        content += cursor[17].ToString();
                    }
                    if (cursor[18] != null)
                    {
                        content += cursor[18].ToString();
                    }
                    if (cursor[19] != null)
                    {
                        content += cursor[19].ToString();
                    }
                    //if (cursor.GetString(1) == "金银花")
                    //{
                    //    var s = cursor[16];
                    //    var i = 0;
                    //}
                    var ls = new List<string>();
                    if (Regex.IsMatch(content, "手足口"))
                    {
                        ls.Add("手足口");
                    }
                    content = content.RemoveNonChinese();

                    //if (cursor.GetString(1) == "金银花")
                    //{
                    //    ls.Add("剧毒");
                    //}
                    var sls = g.Cut(content);
                    ls.AddRange(sls);
                    models.Add(new Model
                    {
                        title = cursor.GetString(1),
                        content = string.Join(" ", ls),
                        drug_id = cursor.GetInt32(0),
                    });
                }

                catch (Exception ex)
                {



                    var i = 0;
                }

            }
            //cmd.Dispose();

            var t = _sqliteConnection.BeginTransaction();

            cmd = new SQLiteCommand("insert into _search (title,content1,content2, drug_id, drug_type) values (@title,@content1,@content2,@drug_id,1)", _sqliteConnection);
            cmd.Transaction = t;
            foreach (var item in models)
            {
                cmd.Parameters.AddWithValue("@title", item.title);
                cmd.Parameters.AddWithValue("@content1", string.Join(" ", item.title.ToCharArray()));
                cmd.Parameters.AddWithValue("@content2", item.content);

                cmd.Parameters.AddWithValue("@drug_id", item.drug_id);
                cmd.ExecuteNonQuery();
            }
            t.Commit();
            t.Dispose();
            // g.Tokenize("", JiebaNet.Segmenter.TokenizerMode.Search).Select(i => i.Word);


        }
        public void Execute2()
        {
            String columns = "id,common_name,main_ingredient,description,indication,dosage,side_effect,taboo,cautions,other,warning,pregnant_lactating_use,pediatric_use,geriatric_use,interaction,overdose,clinical_trials,pharmacological_toxicology,pharmacokinetics,storage";
            //,`references`


            var cmd = new SQLiteCommand("select " + columns + " from t_drug where type=2", _sqliteConnection);
            var cursor = cmd.ExecuteReader();
            var g = new JiebaNet.Segmenter.JiebaSegmenter();


            var models = new List<Model>();

            while (cursor.Read())
            {
                try
                {

                    var content = "";
                    if (cursor[2] != null)
                    {
                        content += cursor[2].ToString();
                    }
                    if (cursor[3] != null)
                    {
                        content += cursor[3].ToString();
                    }
                    if (cursor[4] != null)
                    {
                        content += cursor[4].ToString();
                    }
                    if (cursor[5] != null)
                    {
                        content += cursor[5].ToString();
                    }
                    if (cursor[6] != null)
                    {
                        content += cursor[6].ToString();
                    }
                    if (cursor[7] != null)
                    {
                        content += cursor[7].ToString();
                    }
                    if (cursor[8] != null)
                    {
                        content += cursor[8].ToString();
                    }
                    if (cursor[9] != null)
                    {
                        content += cursor[9].ToString();
                    }
                    if (cursor[10] != null)
                    {
                        content += cursor[10].ToString();
                    }
                    if (cursor[11] != null)
                    {
                        content += cursor[11].ToString();
                    }
                    if (cursor[12] != null)
                    {
                        content += cursor[12].ToString();
                    }
                    if (cursor[13] != null)
                    {
                        content += cursor[13].ToString();
                    }
                    if (cursor[14] != null)
                    {
                        content += cursor[14].ToString();
                    }
                    if (cursor[15] != null)
                    {
                        content += cursor[15].ToString();
                    }
                    if (cursor[16] != null)
                    {
                        content += cursor[16].ToString();
                    }
                    if (cursor[17] != null)
                    {
                        content += cursor[17].ToString();
                    }
                    if (cursor[18] != null)
                    {
                        content += cursor[18].ToString();
                    }
                    if (cursor[19] != null)
                    {
                        content += cursor[19].ToString();
                    }
                    //if (cursor.GetString(1) == "金银花")
                    //{
                    //    var s = cursor[16];
                    //    var i = 0;
                    //}
                    var ls = new List<string>();
                    if (Regex.IsMatch(content, "手足口"))
                    {
                        ls.Add("手足口");
                    }
                    content = content.RemoveNonChinese();

                    //if (cursor.GetString(1) == "金银花")
                    //{
                    //    ls.Add("剧毒");
                    //}
                    var sls = g.Cut(content);
                    ls.AddRange(sls);
                    models.Add(new Model
                    {
                        title = cursor.GetString(1),
                        content = string.Join(" ", ls),
                        drug_id = cursor.GetInt32(0),
                    });
                }

                catch (Exception ex)
                {



                    var i = 0;
                }

            }
            //cmd.Dispose();

            var t = _sqliteConnection.BeginTransaction();

            cmd = new SQLiteCommand("insert into _search (title,content1,content2, drug_id, drug_type) values (@title,@content1,@content2,@drug_id,2)", _sqliteConnection);
            cmd.Transaction = t;
            foreach (var item in models)
            {
                cmd.Parameters.AddWithValue("@title", item.title);
                cmd.Parameters.AddWithValue("@content1", string.Join(" ", item.title.ToCharArray()));
                cmd.Parameters.AddWithValue("@content2", item.content);

                cmd.Parameters.AddWithValue("@drug_id", item.drug_id);
                cmd.ExecuteNonQuery();
            }
            t.Commit();
            t.Dispose();
            // g.Tokenize("", JiebaNet.Segmenter.TokenizerMode.Search).Select(i => i.Word);


        }
        public void Execute3()
        {
            String columns = "id,name,ingredient,usage,function,indication,original_record,selected_record,elucldation,key_symptom,modification,modern_application,cautions,modern_research,verse";
            //,`references`


            var cmd = new SQLiteCommand("select " + columns + " from t_prescription", _sqliteConnection);
            var cursor = cmd.ExecuteReader();
            var g = new JiebaNet.Segmenter.JiebaSegmenter();


            var models = new List<Model>();

            while (cursor.Read())
            {
                try
                {

                    var content = "";
                    if (cursor[2] != null)
                    {
                        content += cursor[2].ToString();
                    }
                    if (cursor[3] != null)
                    {
                        content += cursor[3].ToString();
                    }
                    if (cursor[4] != null)
                    {
                        content += cursor[4].ToString();
                    }
                    if (cursor[5] != null)
                    {
                        content += cursor[5].ToString();
                    }
                    if (cursor[6] != null)
                    {
                        content += cursor[6].ToString();
                    }
                    if (cursor[7] != null)
                    {
                        content += cursor[7].ToString();
                    }
                    if (cursor[8] != null)
                    {
                        content += cursor[8].ToString();
                    }
                    if (cursor[9] != null)
                    {
                        content += cursor[9].ToString();
                    }
                    if (cursor[10] != null)
                    {
                        content += cursor[10].ToString();
                    }
                    if (cursor[11] != null)
                    {
                        content += cursor[11].ToString();
                    }
                    if (cursor[12] != null)
                    {
                        content += cursor[12].ToString();
                    }
                    if (cursor[13] != null)
                    {
                        content += cursor[13].ToString();
                    }
                    if (cursor[14] != null)
                    {
                        content += cursor[14].ToString();
                    }
                   
                    //if (cursor.GetString(1) == "金银花")
                    //{
                    //    var s = cursor[16];
                    //    var i = 0;
                    //}
                    var ls = new List<string>();
                    if (Regex.IsMatch(content, "手足口"))
                    {
                        ls.Add("手足口");
                    }
                    content = content.RemoveNonChinese();

                    //if (cursor.GetString(1) == "金银花")
                    //{
                    //    ls.Add("剧毒");
                    //}
                    var sls = g.Cut(content);
                    ls.AddRange(sls);
                    models.Add(new Model
                    {
                        title = cursor.GetString(1),
                        content = string.Join(" ", ls),
                        drug_id = cursor.GetInt32(0),
                    });
                }

                catch (Exception ex)
                {



                    var i = 0;
                }

            }
            //cmd.Dispose();

            var t = _sqliteConnection.BeginTransaction();

            cmd = new SQLiteCommand("insert into _search (title,content1,content2, drug_id, drug_type) values (@title,@content1,@content2,@drug_id,3)", _sqliteConnection);
            cmd.Transaction = t;
            foreach (var item in models)
            {
                cmd.Parameters.AddWithValue("@title", item.title);
                cmd.Parameters.AddWithValue("@content1", string.Join(" ", item.title.ToCharArray()));
                cmd.Parameters.AddWithValue("@content2", item.content);

                cmd.Parameters.AddWithValue("@drug_id", item.drug_id);
                cmd.ExecuteNonQuery();
            }
            t.Commit();
            t.Dispose();
            // g.Tokenize("", JiebaNet.Segmenter.TokenizerMode.Search).Select(i => i.Word);


        }
    }
}
