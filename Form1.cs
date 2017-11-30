using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


using DynamicExpresso;

using System.IO;
using System.Text.RegularExpressions;

using Ionic.Zip;
using System.Diagnostics;
using Shared;

using Newtonsoft.Json.Linq;



namespace StringHelper
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        #region Methods
        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }
        private static Image cropImage(Image img, Rectangle cropArea)
        {
            Bitmap bmpImage = new Bitmap(img);
            return bmpImage.Clone(cropArea, bmpImage.PixelFormat);
        }
        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }
        public static void ClipboardTextAction(Func<String, String> action)
        {
            try
            {
                var v = Clipboard.GetText().Trim();
                if (v.IsVacuum()) return;

                var r = action(v);
                if (r.IsReadable())
                    Clipboard.SetText(r);
            }
            catch (
            Exception exception)
            {

                MessageBox.Show(exception.Message);
            }
        }
        public static void ClipboardFileAction(Action<String> action)
        {
            try
            {
                var v = Clipboard.GetText().Trim();
                if (!v.FileExists() && Clipboard.GetFileDropList().Count > 0)
                {
                    v = Clipboard.GetFileDropList()[0];
                }
                if (v.FileExists())
                {
                    action(v);
                }


            }
            catch (
            Exception exception)
            {

                MessageBox.Show(exception.Message);
            }
        }
        public static void ClipboardFilesAction(Action<List<string>> action)
        {
            try
            {
                var ls = new List<string>();
                if (Clipboard.GetFileDropList().Count > 0)
                {
                    foreach (var item in Clipboard.GetFileDropList())
                    {
                        if (item.FileExists())
                            ls.Add(item);
                    }
                }
                action(ls);


            }
            catch (
            Exception exception)
            {

                MessageBox.Show(exception.Message);
            }
        }

        public static void ClipboardDirectoryAction(Action<String> action)
        {
            try
            {
                var v = Clipboard.GetText().Trim();
                if (!v.DirectoryExists() && Clipboard.GetFileDropList().Count > 0)
                {
                    v = Clipboard.GetFileDropList()[0];
                }
                if (v.DirectoryExists())
                {
                    action(v);
                }


            }
            catch (
            Exception exception)
            {

                MessageBox.Show(exception.Message);
            }
        }


        #region ZIP
        public static void CompressCSharpDirectoryEncrypt(string path, string dstDir)
        {
            var dirList = Directory.GetDirectories(path, "*", System.IO.SearchOption.AllDirectories).Where(i => !Regex.IsMatch(i, "\\\\(?:\\.|bin|obj|packages)")).OrderBy(i => i.Length).ToList();
            dirList.Add(path);
            using (var zip = new ZipFile(Encoding.GetEncoding("gb2312")))
            {
                //zip.Encryption = EncryptionAlgorithm.WinZipAes256;
                //zip.Password = "gamahuched64";
                foreach (var item in dirList)
                {
                    zip.AddFiles(Directory.GetFiles(item, "*"), item.Substring(path.Length));
                }
                var fileName = dstDir.Combine(path.GetFileName() + ".zip");
                if (fileName.FileExists())
                {

                    fileName = dstDir.Combine($"{path.GetFileName()}-{new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds()}.zip");
                }
                zip.Save(fileName);
            }
        }
        public static void CompressAndroidIntellijDirectoryEncrypt(string path, string dstDir)
        {
            using (var zip = new ZipFile(Encoding.GetEncoding("gb2312")))
            {

                zip.AddFiles(Directory.GetFiles(path), "");
                zip.AddFiles(Directory.GetFiles(path.Combine("app")), "app");
                zip.AddDirectory(path.Combine("app").Combine("src"), "app/src");

                var fileName = dstDir.Combine(path.GetFileName() + ".zip");
                if (fileName.FileExists())
                {

                    fileName = dstDir.Combine($"{path.GetFileName()}-{new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds()}.zip");
                }
                zip.Save(fileName);
            }
        }
        public static void CompressNodeJsDirectory(string path, string dstDir)
        {
            using (var zip = new ZipFile(Encoding.GetEncoding("gb2312")))
            {

                zip.AddFiles(Directory.GetFiles(path), "");

                var directories = Directory.GetDirectories(path).Where(i => i.GetFileName() != "node_modules" && i.GetFileName() != "databases");
                foreach (var item in directories)
                {
                    zip.AddDirectory(item, item.GetFileName());
                }
                var fileName = dstDir.Combine(path.GetFileName() + ".zip");
                if (fileName.FileExists())
                {

                    fileName = dstDir.Combine($"{path.GetFileName()}-{new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds()}.zip");
                }
                zip.Save(fileName);
            }
        }
        public static void CompressDirectoriesEncrypt(string path, string dstDir)
        {
            foreach (var item in Directory.GetDirectories(path))
            {
                using (var zip = new ZipFile(Encoding.GetEncoding("gb2312")))
                {
                    zip.Encryption = EncryptionAlgorithm.WinZipAes256;
                    zip.Password = "gamahuched64";
                    zip.AddDirectory(item);
                    var fileName = dstDir.Combine(item.GetFileName() + ".zip");
                    if (fileName.FileExists())
                    {

                        fileName = dstDir.Combine($"{item.GetFileName()}-{new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds()}.zip");
                    }
                    zip.Save(fileName);
                }
            }

        }
        public static void CompressFile(string path, string dstDir)
        {

            using (var zip = new ZipFile(Encoding.GetEncoding("gb2312")))
            {
                zip.AddFile(path, "");
                zip.CompressionLevel = Ionic.Zlib.CompressionLevel.BestCompression;
                var fileName = dstDir.Combine(path.GetFileName() + ".zip");
                if (fileName.FileExists())
                {

                    fileName = dstDir.Combine($"{path.GetFileName()}-{new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds()}.zip");
                }
                zip.Save(fileName);
            }


        }
        public static void CompressDirectories(string path, string dstDir)
        {
            foreach (var item in Directory.GetDirectories(path))
            {
                using (var zip = new ZipFile(Encoding.GetEncoding("gb2312")))
                {

                    zip.AddDirectory(item);
                    var fileName = dstDir.Combine(item.GetFileName() + ".zip");
                    if (fileName.FileExists())
                    {

                        fileName = dstDir.Combine($"{item.GetFileName()}-{new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds()}.zip");
                    }
                    zip.Save(fileName);
                }
            }

        }
        #endregion

        #endregion



        private void formatBlockCommentButton_Click(object sender, EventArgs e)
        {
            try
            {

                var value = Clipboard.GetText();
                Clipboard.SetText(Formatters.FormatBlockComment(value));
            }
            catch (Exception exception)
            {

                MessageBox.Show(exception.Message);
            }
        }

        private void extractMethodListButton_Click(object sender, EventArgs e)
        {
            try
            {

                var value = Clipboard.GetText();
                Clipboard.SetText(value.FormatMethodList().Flat());
            }
            catch (Exception exception)
            {

                MessageBox.Show(exception.Message);
            }
        }

        private void 创建C文件ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                var count = 0;
                var dir = Clipboard.GetText();
                if (System.IO.Directory.Exists(dir))
                {
                    for (int i = 1; i < 12; i++)
                    {
                        var targetFile = Path.Combine(dir, i.ToString().PadLeft(3, '0') + ".c");
                        if (File.Exists(targetFile)) continue;
                        if (count == 3) return;
                        count++;

                        File.Create(targetFile).Close();
                        File.Create(Path.ChangeExtension(targetFile, ".h")).Close();

                    }
                }
                else
                {
                    var targetDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                    dir = Path.GetFileNameWithoutExtension(dir);
                    File.Create(Path.Combine(targetDirectory, dir + ".c")).Close();
                    File.Create(Path.Combine(targetDirectory, dir + ".h")).Close();


                }

            }
            catch (Exception exception)
            {

                MessageBox.Show(exception.Message);
            }
        }

        private void formatFileNameButton_Click(object sender, EventArgs e)
        {
            try
            {
                var v = Clipboard.GetText().Trim();

                Clipboard.SetText(v.Replace("\\", "\\\\"));
            }
            catch (Exception exception)
            {

                MessageBox.Show(exception.Message);
            }
        }

        private void formatOrderStringButton_Click(object sender, EventArgs e)
        {
            try
            {
                var v = Clipboard.GetText().Trim();

                Clipboard.SetText(string.Join(Environment.NewLine, v.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(i => i.Trim()).OrderBy(i => i).Distinct()));
            }
            catch (Exception exception)
            {

                MessageBox.Show(exception.Message);
            }
        }

        private void generateSublimeCSnippets_Click(object sender, EventArgs e)
        {

            try
            {
                var v = Clipboard.GetText().Trim();

                var template = @"<snippet>
	<content><![CDATA[{0}]]></content>
	
	<tabTrigger>{1}</tabTrigger>
	
	<scope>source.c, source.c++</scope>
    <description>{2}</description>
</snippet>";
                var fnName = Regex.Match(v, "([a-zA-Z_0-9]+) *\\(").Groups[1].Value;

                var content = Regex.Match(v, "[a-zA-Z_0-9]+ *\\([^\\)]*?\\)").Value;

                var c1 = content.Split(new char[] { '(' }, 2).First() + "(";
                var c2 = content.Split(new char[] { '(' }, 2).Last().Trim();
                var count = 1;
                foreach (var item in c2.Split(','))
                {
                    c1 += $"${{{count}:{Regex.Replace(Regex.Split(item.Trim(), "\\s+(?!\\))").Last(), "(^[\\s+\\(\\*]+)|([\\s+\\)]+$)", "")}}},";
                    count++;
                }
                c1 = c1.TrimEnd(',') + ");";

                var output = string.Format(template, c1, fnName, v);

                var targetFileName = @"C:\psycho\.RAR\Sublime Text Build 3143\Data\Packages\User\c-" + fnName + ".sublime-snippet";

                File.WriteAllText(targetFileName, output, new UTF8Encoding(false));

            }
            catch (Exception exception)
            {

                MessageBox.Show(exception.Message);
            }
        }

        private void generateSublimeCWinSnippets_Click(object sender, EventArgs e)
        {
            //  AdjustSnippets();

            // GenerateWin32SublimeSnippet();


            try
            {
                var v = Clipboard.GetText().Trim();
                //if (Directory.Exists(v))
                //{
                //    Shared.Snippets.GenerateSublimeCompletionsFromDirectory(v);
                //}
                var targetFileName = @"C:\psycho\.RAR\Sublime Text Build 3143\Data\Packages\User\C.sublime-completions";

                var dictionary = new Dictionary<string, string>();
                var (a, b) = Snippets.FormatMSDNWin32Function(v);
                dictionary.Add("trigger", a);
                dictionary.Add("contents", b);
                Shared.Snippets.AddSublimeCompletions(targetFileName, dictionary);
            }
            catch (Exception exception)
            {

                MessageBox.Show(exception.Message);
            }
        }

        private void GenerateWin32SublimeSnippet()
        {
            try
            {
                var v = Regex.Replace(Clipboard.GetText().Trim(), "[\r\n]+", "");

                var template = @"<snippet>
	<content><![CDATA[{0}]]></content>
	
	<tabTrigger>{1}</tabTrigger>
	
	<scope>source.c, source.c++</scope>
    <description>{2}</description>
</snippet>";
                var fnName = Regex.Match(v, "([a-zA-Z_0-9]+) *\\(").Groups[1].Value;

                var content = Regex.Match(v, "[a-zA-Z_0-9]+ *\\([^\\)]*?\\)").Value;

                var c1 = content.Split(new char[] { '(' }, 2).First() + "(";
                var c2 = content.Split(new char[] { '(' }, 2).Last().Trim();
                var count = 1;
                foreach (var item in c2.Split(','))
                {
                    c1 += $"${{{count}:{Regex.Replace(Regex.Split(item.Trim(), "\\s+(?!\\))").Last(), "(^[\\s+\\(\\*]+)|([\\s+\\)]+$)", "")}}},";
                    count++;
                }
                c1 = c1.TrimEnd(',') + ")";

                var output = string.Format(template, c1, fnName, v);

                var targetFileName = @"C:\psycho\.RAR\Sublime Text Build 3143\Data\Packages\User\win-" + fnName + ".sublime-snippet";

                File.WriteAllText(targetFileName, output, new UTF8Encoding(false));

            }
            catch (Exception exception)
            {

                MessageBox.Show(exception.Message);
            }
        }
        private void AdjustSnippets()
        {

            try
            {
                var v = Clipboard.GetText().Trim();
                if (Directory.Exists(v))
                {
                    var encoding = new UTF8Encoding(false);

                    var files = Directory.GetFiles(v).Where(i => i.EndsWith(".sublime-snippet"));

                    foreach (var item in files)
                    {
                        var value = File.ReadAllText(item, encoding);


                        var content = Regex.Replace(value, Regex.Escape("![CDATA[") + "[\\s]+", "![CDATA[");
                        //content = Regex.Replace(value, "[\\s]+" + Regex.Escape("]]>"), "![CDATA[");
                        content = content.Replace("![CDATA[<", "]]><");
                        //var block = Regex.Match(value, @"/\*(.*?)\*/").Groups[1].Value;

                        //var content = Regex.Replace(value, @"/\*(.*?)\*/", "");

                        //content = content.Replace("</snippet>", $"<description>{block}</description>\n</snippet>");
                        File.WriteAllText(item, content, encoding);
                        // File.WriteAllText(item, value.Replace("<scope>source.c - source.c++</scope>", "<scope>source.c, source.c++</scope>"), encoding);
                    }
                }


            }
            catch (Exception exception)
            {

                MessageBox.Show(exception.Message);
            }

        }

        private void compressDirectoryButton_Click(object sender, EventArgs e)
        {
            try
            {
                var v = Clipboard.GetText().Trim();
                if (Directory.Exists(v))
                {


                }


            }
            catch (Exception exception)
            {

                MessageBox.Show(exception.Message);
            }
        }

        private void formatTableButton_Click(object sender, EventArgs e)
        {
            try
            {
                var v = Clipboard.GetText().Trim();

                var doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(v);

                var table = doc.DocumentNode.SelectSingleNode("//table");

                if (table == null) return;

                var sb = new StringBuilder();

                var tr = table.SelectNodes("//tr");
                if (!tr.Any()) return;
                foreach (var item in tr)
                {
                    var td = item.ChildNodes.Where(i => i.Name == "td");

                    foreach (var tdItem in td)
                    {
                        sb.Append(Regex.Replace(tdItem.InnerText.FlatToLine(), "\\<![^\\>]*?\\>", "")).Append("     ");
                    }
                    sb.AppendLine();
                }

                Clipboard.SetText(sb.ToString());



            }
            catch (Exception exception)
            {

                MessageBox.Show(exception.Message);
            }
        }

        private void formatCodeButton_Click(object sender, EventArgs e)
        {
            try
            {
                var v = Clipboard.GetText().Trim();


                v = v.Replace("\r", "");
                v = v.Replace("\\", "\\\\");
                v = v.Replace("\n", "\\n");
                v = v.Replace("\"", "\\\"");


                Clipboard.SetText(v);
            }
            catch (
            Exception exception)
            {

                MessageBox.Show(exception.Message);
            }
        }

        private void formatEpub_Click(object sender, EventArgs e)
        {
            try
            {
                var v = Clipboard.GetText().Trim();

                if (!Directory.Exists(v)) return;

                var epubFiles = System.IO.Directory.GetFiles(v, "*.epub", System.IO.SearchOption.AllDirectories).Where(i => !i.Contains(".EPUB"));
                var targetDirectory = "C:\\BOOKS\\EPUBS";
                targetDirectory.CreateDirectoryIfNotExists();
                foreach (var item in epubFiles)
                {
                    try
                    {
                        HelperEpubRename.ReNameEpub(item, targetDirectory);
                    }
                    catch
                    {

                    }
                }
            }
            catch (
            Exception exception)
            {

                MessageBox.Show(exception.Message);
            }


        }

        private void formatEbooksByAuthorNameButton_Click(object sender, EventArgs e)
        {
            try
            {
                var v = Clipboard.GetText().Trim();

                if (!Directory.Exists(v)) return;

                var epubFiles = System.IO.Directory.GetFiles(v, "*.*", System.IO.SearchOption.AllDirectories).Where(i => !i.Contains(".EPUB"));
                var targetDirectory = v;
                foreach (var item in epubFiles)
                {
                    var dir = Path.Combine(v, item.LastIndexOf('-') > -1 ? Path.GetFileNameWithoutExtension(item.Substring(item.LastIndexOf('-') + 1).Trim()) : "");
                    dir.CreateDirectoryIfNotExists();

                    File.Move(item, Path.Combine(dir, Path.GetFileName(item)));
                }
            }
            catch (
            Exception exception)
            {

                MessageBox.Show(exception.Message);
            }
        }
        #region Methods

        void FormatSourceCodeWithClangFormat()
        {
            try
            {
                var v = Clipboard.GetText().Trim();

                if (v.FileExists())
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = "clang-format",
                        Arguments = $"-i -style=Google \"{v}\"",
                        WindowStyle = ProcessWindowStyle.Hidden,
                    });
                }
            }
            catch (
            Exception exception)
            {

                MessageBox.Show(exception.Message);
            }
        }
        #endregion
        private void clangFormatButton_Click(object sender, EventArgs e)
        {
            FormatSourceCodeWithClangFormat();
        }

        private void formatMSDNTableButton_Click(object sender, EventArgs e)
        {
            try
            {
                var v = Clipboard.GetText().Trim();

                var doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(v);

                var table = doc.DocumentNode.SelectSingleNode("//table");

                if (table == null) return;

                var sb = new StringBuilder();

                var tr = table.SelectNodes("//tr//dl");
                if (tr == null) return;
                foreach (var item in tr)
                {
                    var dts = item.ChildNodes.Where(i => i.Name == "dt");

                    sb.Append($"public const int {dts.ElementAt(0).InnerText.Trim()}={dts.ElementAt(1).InnerText.Trim()};");
                    sb.AppendLine();
                }

                Clipboard.SetText(sb.ToString());



            }
            catch (Exception exception)
            {

                MessageBox.Show(exception.Message);
            }
        }

        private void GenerateVirtualCodeButton_Click(object sender, EventArgs e)
        {
            try
            {

                var sb = new StringBuilder();


                for (int i = 0; i < 10; i++)
                {
                    sb.AppendLine($"public const int VK_{i} =0x3{i};");
                }
                var count = 40;
                foreach (var item in Enumerable.Range('A', 'I' - 'A' + 1))
                {

                    count++;

                    sb.AppendLine($"public const int VK_{(char)item}= 0x{count};");

                }
                var hex = Enumerable.Range('A', 'F' - 'A' + 1).ToArray();
                var offset = 0;
                foreach (var item in Enumerable.Range('J', 'O' - 'J' + 1))
                {



                    sb.AppendLine($"public const int VK_{(char)item}=0x4{(char)hex[offset++]};");

                }
                count = 49;
                foreach (var item in Enumerable.Range('P', 'Y' - 'P' + 1))
                {

                    count++;

                    sb.AppendLine($"public const int VK_{(char)item}= 0x{count};");

                }
                sb.AppendLine($"public const int VK_Z= 0x5A;");

                Clipboard.SetText(sb.ToString());



            }
            catch (Exception exception)
            {

                MessageBox.Show(exception.Message);
            }
        }

        private void cHeaderDefineToCConstantsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {


                var filePath = Clipboard.GetText().Trim();
                if (!File.Exists(filePath)) return;

                var ls = new List<string>();

                var matches = Regex.Matches(filePath.ReadAllText(), "^#define WM_[ _0-9A-Za-z]+$", RegexOptions.Multiline);

                foreach (Match item in matches)
                {
                    ls.Add(item.Value);
                }
                var sb = new StringBuilder();

                foreach (var item in ls)
                {
                    var s = item.Split(' ').Where(i => i.IsReadable());
                    sb.AppendLine($"public const int {s.ElementAt(s.Count() - 2).Trim()} ={s.Last().Trim()};");

                }


                Clipboard.SetText(sb.ToString());



            }
            catch (Exception exception)
            {

                MessageBox.Show(exception.Message);
            }
        }
        private string Crypted(string content)
        {
            CustomEncrypted _crypt = new CustomEncrypted();

            //String iv = CustomEncrypted.GenerateRandomIV(16); //16 bytes = 128 bits
            // string key = CustomEncrypted.getHashSha256("cccb601a7314472686dc40acc27933b", 31); //32 bytes = 256 bits
            String cypherText = _crypt.encrypt(content, "a3106b1138cf947f69b7038942976a3", "EEGTuhyHRaFrQMhl");
            return cypherText;

        }
        private void 加密文本文字ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                var v = Clipboard.GetText().Trim();

                Clipboard.SetText(Crypted(v));



            }
            catch (Exception exception)
            {

                MessageBox.Show(exception.Message);
            }
        }


        private void lRCToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                var v = Clipboard.GetText().Trim();

                if (!Directory.Exists(v)) return;

                var files = Directory.GetFiles(v, "*.srt");

                foreach (var item in files)
                {
                    Videos.ConvertSrtToLrc(item, 0);
                }
            }
            catch (
            Exception exception)
            {

                MessageBox.Show(exception.Message);
            }
        }

        private void lRC3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                var v = Clipboard.GetText().Trim();

                if (!Directory.Exists(v)) return;

                var files = Directory.GetFiles(v, "*.srt");

                foreach (var item in files)
                {
                    Videos.ConvertSrtToLrc(item);
                }
            }
            catch (
            Exception exception)
            {

                MessageBox.Show(exception.Message);
            }
        }

        private void rgbtohexButton_Click(object sender, EventArgs e)
        {
            try
            {
                var v = Clipboard.GetText().Trim();

                Clipboard.SetText(Colors.ConvertRGBStringToHex(v));
            }
            catch (
            Exception exception)
            {

                MessageBox.Show(exception.Message);
            }
        }
        KeyboardHook _keyboardHotKeyF1 = new KeyboardHook();

        private void 获取屏幕颜色ToolStripMenuItem_Click(object sender, EventArgs e)
        {


            _keyboardHotKeyF1.RegisterHotKey(Shared.ModifierKeys.None, Keys.F1);
            _keyboardHotKeyF1.KeyPressed += (o, k) =>
            {
                try
                {
                    var (r, g, b) = Colors.GetColorAt();

                    Clipboard.SetText($"{r.ToString("X2")}{g.ToString("X2")}{b.ToString("X2")}");
                }
                catch (
                Exception exception)
                {

                    MessageBox.Show(exception.Message);
                }
            };

           
        }

        private void converttoicoButton_Click(object sender, EventArgs e)
        {
            try
            {


                var filePath = Clipboard.GetText().Trim();
                if (!File.Exists(filePath)) return;

                Images.ConvertToIco(filePath);


            }
            catch (Exception exception)
            {

                MessageBox.Show(exception.Message);
            }
        }

        private void 生成文字图标ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {


                var s = Clipboard.GetText().Trim().First().ToString().ToUpper();
                Images.Generate(s, Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\01.ico");



            }
            catch (Exception exception)
            {

                MessageBox.Show(exception.Message);
            }
        }

        private void 计算表达式ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                var v = Clipboard.GetText().Trim();
                var interpreter = new Interpreter();
                var result = interpreter.Eval(v);
                MessageBox.Show(v + "=" + result);
            }
            catch (
            Exception exception)
            {

                MessageBox.Show(exception.Message);
            }
        }

        private void calculateButton_ButtonClick(object sender, EventArgs e)
        {
            try
            {

                var interpreter = new Interpreter();
                var result = interpreter.Eval(valueTextBox.Text);
                valueTextBox.Text = valueTextBox.Text + "=" + result;
            }
            catch (
            Exception exception)
            {

                MessageBox.Show(exception.Message);
            }
        }

        private void textBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            textBox1.SelectAll();

            textBox1.Paste();
        }

        private void 生成表格ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // ：

            try
            {
                var v = Clipboard.GetText().Trim();
                if (v.IsVacuum()) return;

                var separator = '：';

                var sb = new StringBuilder();

                const string tdS = "<td>";
                const string tdE = "</td>";

                var lines = v.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                sb.AppendLine("<table>");

                foreach (var item in lines)
                {
                    var splited = item.Split(separator);

                    sb.Append("<tr>");
                    sb.Append($"{tdS}{splited.First().Trim()}{tdE}")
                        .Append($"{tdS}{splited.Last().Trim()}{tdE}");
                    sb.AppendLine("</tr>");

                }
                sb.AppendLine("</table>");
                Clipboard.SetText(sb.ToString());
            }
            catch (
            Exception exception)
            {

                MessageBox.Show(exception.Message);
            }
        }

        private void 百度网盘JSON重复项ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClipboardTextAction((v) =>
            {
                Newtonsoft.Json.Linq.JObject obj = Newtonsoft.Json.JsonConvert.DeserializeObject<Newtonsoft.Json.Linq.JObject>(v);

                JArray j = obj["list"].ToObject<JArray>();
                var sb = new StringBuilder();

                foreach (JObject item in j)
                {
                    var p = item["path"];
                    if (Regex.IsMatch(p.Value<string>(), "\\([0-9]\\)"))
                    {
                        sb.AppendLine($"pcs remove \"{p}\"");

                    }
                }
                return sb.ToString();
            });
        }

        private void 压缩目录ZIPToolStripMenuItem_Click(object sender, EventArgs e)
        {

            ClipboardDirectoryAction(v =>
            {

                string dir = @"C:\psycho\.RAR";

                var targetFileName = Path.Combine(dir, Path.GetFileName(v) + ".zip");

                using (var zip = new ZipFile(Encoding.GetEncoding("gb2312")))
                {
                    zip.AddDirectory(v);
                    if (File.Exists(targetFileName))
                    {
                        targetFileName = Path.Combine(dir, Path.GetFileName(v) + new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds()
                        + ".zip");
                    }
                    zip.Save(targetFileName);
                }
            });
        }

        private void 压缩CSharp目录ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClipboardDirectoryAction(v =>
            {
                var dstDir = @"c:\psycho\.RAR";
                dstDir.CreateDirectoryIfNotExists();
                CompressCSharpDirectoryEncrypt(v, dstDir);
            });
        }

        private void 压缩IntellijAndroidToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClipboardDirectoryAction(v =>
            {
                var dstDir = @"c:\psycho\.RAR";
                dstDir.CreateDirectoryIfNotExists();
                CompressAndroidIntellijDirectoryEncrypt(v, dstDir);
            });
        }

        private void 压缩子目录ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClipboardDirectoryAction(v =>
            {
                var dstDir = @"c:\psycho\.RAR";
                dstDir.CreateDirectoryIfNotExists();
                CompressDirectories(v, dstDir);
            });

        }




        private void 压缩单个文件ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClipboardFileAction(v =>
            {
                CompressFile(v, @"C:\psycho\.RAR");
            });
        }

        private void 解码二维码ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClipboardFileAction(v =>
            {
                ZXing.IBarcodeReader reader = new ZXing.BarcodeReader();
                // load a bitmap
                var barcodeBitmap = (Bitmap)Bitmap.FromFile(v);
                // detect and decode the barcode inside the bitmap
                var result = reader.Decode(barcodeBitmap);
                // do something with the result
                if (result != null)
                {
                    textBox1.Text = result.BarcodeFormat.ToString() + Environment.NewLine + result.Text;

                }
            });
        }

        private void 创建文件夹ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClipboardTextAction(v =>
            {
                HelpersSafari.CreateDirectory(v);
                return "";
            });
        }

        private void 创建目录ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            HelpersSafari.CreateTableContents();
        }

        private void 离线文档ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var dlg = new OpenFileDialog();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                var dir = dlg.FileName.GetDirectoryName();

                var ls = System.IO.Directory.GetDirectories(dir);
                foreach (var item in ls)
                {
                    HelpersSafari.ProcessForOffline(item);
                }
            }
        }

        private void 转换成HTM文档ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var dlg = new OpenFileDialog();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                var dir = dlg.FileName.GetDirectoryName();

                var ls = System.IO.Directory.GetDirectories(dir);
                foreach (var item in ls)
                {
                    HelpersSafari.FormatHTML(item);
                }
            }
        }

        private void 下载图片ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var dlg = new OpenFileDialog();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                var dir = dlg.FileName.GetDirectoryName();

                var ls = System.IO.Directory.GetDirectories(dir);
                foreach (var item in ls)
                {
                    HelpersSafari.DoExtractImages(item);
                }
            }
        }

        private void 生成未下载文件列表linksToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClipboardDirectoryAction((path) =>
             {
                 var ls = System.IO.Directory.GetDirectories(path);

                 foreach (var item in ls)
                 {


                     HelpersSafari.GenerateUnDownloadFileListFile(item, item.Combine("links.txt"));

                 }
             });
        }

        private void 生成未下载文件列表imagesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClipboardDirectoryAction((path) =>
            {
                var ls = System.IO.Directory.GetDirectories(path);

                foreach (var item in ls)
                {
                    var dir = item.Combine("images");

                    HelpersSafari.GenerateUnDownloadFileListFile(dir, dir.Combine("img-links.txt"));

                }
            });
        }

        private void 创建EpubToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClipboardDirectoryAction((dir) =>
            {
                var ls = System.IO.Directory.GetDirectories(dir);

                var targetDirectory = @"C:\Users\Administrator\Desktop\Safari\EPUBS";
                targetDirectory.CreateDirectoryIfNotExists();
                foreach (var item in ls)
                {
                    targetDirectory.Combine(item.GetFileName()).CreateDirectoryIfNotExists();
                    HelperEpubCreator.CreateEpub(item, targetDirectory.Combine(item.GetFileName()));
                }
            });


        }

        private void f1ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            var keyboardHotKey = new KeyboardHook();

            keyboardHotKey.RegisterHotKey(Shared.ModifierKeys.None, Keys.F1);
            keyboardHotKey.KeyPressed += (o, k) =>
            {

            };
        }

        private void 导入ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClipboardDirectoryAction((dir) =>
            {

                new Sqlites("datas.db".GetApplicationPath()).ImportOne(dir, "zl");

            });


        }

        private void 导入2ToolStripMenuItem_Click(object sender, EventArgs e)
        {


            new Sqlites("datas.db".GetApplicationPath()).ImportTwo();
            //   new Sqlites("datas.db".GetApplicationPath()).InsertOne(new System.Net.Http.HttpClient(), "http://ypk.familydoctor.com.cn/202442/");


        }

        private void Form1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Process.Start(Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]));
        }

        private void 导入3ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            ClipboardFileAction(v =>
            {


                new Sqlites("datas.db".GetApplicationPath()).ImportThree(v);
            });
        }

        private void sQLCLASSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClipboardTextAction(v =>
            {


                var splited = v.Split(',').Where(i => i.IsReadable()).Select(i => i.Trim());
                var ls = new List<string>();

                foreach (var item in splited)
                {
                    ls.Add($"public String {item.Split('.').Last()}{{get;set;}}");
                    ls.Add($"if (item.{item.Split('.').Last()}.IsReadable()){{\r\n}}");
                }
                return string.Join(Environment.NewLine, ls.OrderBy(i => i));
            });
        }

        private void sQLCLASSToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ClipboardTextAction(v =>
            {


                var splited = new List<String>();
                var matches = Regex.Matches(v, "\\[([^\\]]*?)\\]");

                foreach (Match item in matches)
                {
                    splited.Add(item.Groups[1].Value);
                }
                var ls = new List<string>();

                foreach (var item in splited)
                {
                    ls.Add($"public String {item.Split('.').Last()}{{get;set;}}\r\n");

                    ls.Add($"{splited[0]}.{item}");
                }
                return string.Join(Environment.NewLine, ls.OrderBy(i => i));
            });
        }

        private void 导入5ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            ClipboardFileAction(v =>
            {

                new Sqlites("datas.db".GetApplicationPath()).ImportFive(v);
            });
        }

        private void 导入1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClipboardFileAction(v =>
            {

                new Sqlites("datas.db".GetApplicationPath()).ImportSix(v);
            });
        }

        private void 按文件名整理文件ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClipboardDirectoryAction(v =>
            {

                var files = Directory.GetFiles(v);
                var dir = Path.Combine(v, "COLLECTION");
                dir.CreateDirectoryIfNotExists();
                foreach (var item in files)
                {
                    var td = dir.Combine(item.GetExtension());

                    td.CreateDirectoryIfNotExists();
                    var tf = td.Combine(item.GetFileName());
                    if (item == tf) continue;

                    try
                    {
                        File.Move(item, tf);

                    }
                    catch { }
                }
            });
        }

        private void magnetButton_ButtonClick(object sender, EventArgs e)
        {
            try
            {

                var dir = "C:\\psycho\\aria2c";
                dir.CreateDirectoryIfNotExists();
                Process.Start(new ProcessStartInfo
                {
                    WorkingDirectory = dir,
                    FileName = "aria2c",
                    Arguments = "\"" + Clipboard.GetText() + "\""
                });

            }
            catch (Exception exception)
            {

                MessageBox.Show(exception.Message);
            }
        }

        private void magnetTorrentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {

                var magnet = Clipboard.GetText().Trim();

                if (!magnet.StartsWith("magnet:"))
                {
                    return;

                }
                var dir = "C:\\psycho\\aria2c";
                dir.CreateDirectoryIfNotExists();

                Process.Start(new ProcessStartInfo
                {
                    WorkingDirectory = dir,
                    FileName = "aria2c",
                    Arguments = "--bt-metadata-only=true --bt-save-metadata=true \"" + magnet + "\""
                });

            }
            catch (Exception exception)
            {

                MessageBox.Show(exception.Message);
            }
        }

        private void 移动所有文件目录ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClipboardDirectoryAction(v =>
            {

                var files = Directory.GetFiles(v, "*", SearchOption.AllDirectories);

                var t = v.GetDirectoryName().Combine("COLLECTION");
                t.CreateDirectoryIfNotExists();

                foreach (var item in files)
                {

                    var tf = t.Combine(item.GetFileName());

                    if (tf == item) continue;
                    var count = 0;
                    while (tf.FileExists())
                    {
                        tf = tf.ChangeFileName($"{tf.GetFileNameWithoutExtension()}{(++count).ToString().PadLeft(3, '0')}");
                    }

                    File.Move(item, tf);
                }

            });
        }

        private void 格式化PDF转TXTToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClipboardFileAction(v =>
            {
                var lines = File.ReadAllLines(v, new UTF8Encoding(false));

                var sb = new StringBuilder();

                foreach (var item in lines)
                {
                    var l = item.Trim();
                    if (l.IsReadable())
                    {
                        if (l.EndsWith("-") && (!l.EndsWith("--")))
                        {
                            sb.Append(l.TrimEnd('-'));
                        }
                        else if (l.EndsWith(".") || l.EndsWith("?") || l.EndsWith("!") || l.EndsWith("-") || l.EndsWith("...") || l.EndsWith("\""))
                        {
                            sb.AppendLine(l);
                        }
                    }
                };
                v.WriteAllText(sb.ToString());
            });
        }

        private void s8_Click(object sender, EventArgs e)
        {
            ClipboardDirectoryAction((path) =>
            {
                var ls = System.IO.Directory.GetDirectories(path);

                foreach (var item in ls)
                {
                    var dir = item.Combine("images");

                    HelpersSafari.GenerateUnDownloadFileListFile(dir, dir.Combine("img-links.txt"));

                }
            });
        }

        private void 排序CSSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClipboardTextAction(v =>
            {

                var ls = new List<String>();

                var sb = new StringBuilder();
                var count = 0;
                foreach (var item in v)
                {
                    sb.Append(item);
                    if (item == '{')
                    {
                        count++;
                    }
                    else if (item == '}')
                    {
                        count--;
                        if (count == 0)
                        {
                            ls.Add(sb.ToString());
                            sb.Clear();
                        }
                    }
                }
                var s = "{".ToArray();
                return string.Join(Environment.NewLine, ls.OrderBy(i =>
                {
                    var c = i.Split(s, 2).First().Trim();
                    if (!Regex.IsMatch(c, "^[a-zA-Z]"))
                    {
                        return "z" + c;
                    }
                    return c;

                })).RemoveEmptyLines();
            });
        }

        private void 压缩NodeJSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClipboardDirectoryAction(v =>
            {
                var dstDir = @"c:\psycho\.RAR";
                dstDir.CreateDirectoryIfNotExists();
                CompressNodeJsDirectory(v, dstDir);
            });
        }

        private void 数组ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClipboardTextAction(v =>
            {
                var r = string.Empty;
                if (textBox1.Text.IsReadable())
                {
                    var sb = new StringBuilder();
                    var ls = v.Split('\n').Where(i => i.IsReadable()).Select(i => i.Trim()).OrderBy(i => i).Distinct();

                    foreach (var item in ls)
                    {
                        sb.AppendLine(textBox1.Text.Trim().Replace("{0}", item)).AppendLine();

                    }
                    v = sb.ToString();
                }
                return v;
            });
        }

        private void 数组ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ClipboardTextAction(v =>
            {
                var r = string.Empty;

                var sb = new StringBuilder();
                var matches = Regex.Matches(v, "(【[^\\]]*?】)");
                var ls = new List<string>();
                foreach (Match m in matches)
                {
                    ls.Add(m.Groups[1].Value);
                }

                v = string.Join(",", ls);
                return v;
            });
        }

        private void 生成JavaScriptSublimeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                var v = Clipboard.GetText().Trim();

                var template = @"<snippet>
	<content><![CDATA[{0}]]></content>
	
	<tabTrigger>{0}</tabTrigger>
	
	<scope>source.js</scope>
</snippet>";
                var fnName = v;


                var output = string.Format(template, fnName);

                var targetFileName = @"C:\psycho\.RAR\Sublime Text Build 3143\Data\Packages\User\JavaScript." + fnName + ".sublime-snippet";

                File.WriteAllText(targetFileName, output, new UTF8Encoding(false));

            }
            catch (Exception exception)
            {

                MessageBox.Show(exception.Message);
            }
        }

        private void 保留正则表达式ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (patternBox.Text.IsVacuum()) return;

            ClipboardTextAction(v =>
            {

                var r = string.Empty;

                var sb = new StringBuilder();
                var matches = Regex.Matches(v, patternBox.Text.Trim());
                var ls = new List<string>();
                foreach (Match m in matches)
                {
                    ls.Add(m.Groups[1].Value);
                }
                ls = ls.Distinct().ToList();

                v = string.Join(",", ls) + Environment.NewLine + Environment.NewLine + string.Join(Environment.NewLine, ls);
                return v;
            });
        }

        private void 序列数字ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClipboardTextAction(v =>
            {

                var r = string.Empty;

                var ls = new List<string>();

                for (int i = 0; i < 51; i++)
                {
                    ls.Add(v.Replace("{0}", i.ToString()));
                }
                return string.Join(Environment.NewLine, ls) + Environment.NewLine + Environment.NewLine + string.Join("", ls);
            });
        }

        private void sQLite分词ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClipboardFileAction(f =>
            {
                new SqliteTerms(f).Execute3();
            });
        }

        private void 压缩CSSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClipboardFileAction(f =>
            {
                var css = new WebMarkupMin.Core.KristensenCssMinifier();
                var r = css.Minify(f.ReadAllText(), false);
                f.ChangeFileName(f.GetFileName() + ".min").WriteAllText(r.MinifiedContent);
            });

        }

        private void 解压CSSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClipboardFileAction(f =>
            {
                var min = new Microsoft.Ajax.Utilities.Minifier();
                var r = min.MinifyStyleSheet(f.ReadAllText(), new Microsoft.Ajax.Utilities.CssSettings
                {
                    OutputMode = Microsoft.Ajax.Utilities.OutputMode.MultipleLines
                });
                f.WriteAllText(r);
            });
        }

        private void 上传nginxconfToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClipboardFileAction(f =>
            {
                var host = "180.76.145.233";
                var username = "root";
                var password = "yzlA9Q82itbN#";
                using (var sf = new Renci.SshNet.SftpClient(host, username, password))
                {
                    sf.Connect();
                    using (var stream = File.Open(f, FileMode.OpenOrCreate))
                    {
                        sf.UploadFile(stream, "/etc/nginx/" + f.GetFileName());
                    }

                }
            });
        }

        private void 重启NginxToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var host = "180.76.145.233";
            var username = "root";
            var password = "yzlA9Q82itbN#";
            using (var sf = new Renci.SshNet.SshClient(host, username, password))
            {
                sf.Connect();
                sf.RunCommand("sudo systemctl reload nginx");

            }
        }

        private void 格式化NginxToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClipboardTextAction(v =>
            {

                return Formatters.FormatNginxConf(v);
            });
        }

        private void 映射上传文件ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            ClipboardFilesAction(fs =>
            {
                var host = "180.76.145.233";
                var username = "root";
                var password = "yzlA9Q82itbN#";

                var prefix = @"C:\Users\Administrator\Desktop\ZZZ";

                using (var sf = new Renci.SshNet.SftpClient(host, username, password))
                {
                    sf.Connect();

                    foreach (var f in fs)
                    {
                        if (f.StartsWith(prefix))
                        {
                            var suffix = f.Substring(prefix.Length + 1);
                            var tf = "/usr/share/nginx/html/" + suffix.Replace("\\", "/");




                            using (var stream = File.Open(f, FileMode.OpenOrCreate))
                            {
                                sf.UploadFile(stream, tf);
                            }

                        }
                    }
                }

            });
        }

        private void 重启ForeverToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var host = "180.76.145.233";
            var username = "root";
            var password = "yzlA9Q82itbN#";
            using (var sf = new Renci.SshNet.SshClient(host, username, password))
            {
                sf.Connect();
                var r = sf.RunCommand("cd /usr/share/nginx/html && NODE_ENV=production forever restart app.js");
                MessageBox.Show(r.Result);
            }
        }

        private void 启动ForeverToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var host = "180.76.145.233";
            var username = "root";
            var password = "yzlA9Q82itbN#";
            using (var sf = new Renci.SshNet.SshClient(host, username, password))
            {
                sf.Connect();
                var r = sf.RunCommand("cd /usr/share/nginx/html && NODE_ENV=production forever start app.js");
                MessageBox.Show(r.Result);

            }
        }



        private void 上传到imagesToolStripMenuItem_Click_1(object sender, EventArgs e)
        {

            ClipboardFilesAction(fs =>
            {
                var host = "180.76.145.233";
                var username = "root";
                var password = "yzlA9Q82itbN#";


                using (var sf = new Renci.SshNet.SftpClient(host, username, password))
                {
                    sf.Connect();

                    foreach (var f in fs)
                    {


                        var tf = "/usr/share/nginx/html/public/images/" + f.GetFileName();




                        using (var stream = File.Open(f, FileMode.OpenOrCreate))
                        {
                            sf.UploadFile(stream, tf);
                        }

                    }
                }

            });
        }

        private void 缩放到320X180ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClipboardFileAction(v =>
            {

                using (var bitmap = Bitmap.FromFile(v))
                {
                    var width = bitmap.Width;
                    var height = bitmap.Height;
                    var targetWidth = 320;

                    var targetHeight = (targetWidth * 1.0f / width) * height;
                    var i = ResizeImage(bitmap, targetWidth, (int)targetHeight);
                    width = i.Width;
                    height = i.Height;

                    var h = 180;
                    var n = cropImage(i, new Rectangle(0, (height - 180) / 2, targetWidth, h));
                    i.Dispose();

                    ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);

                    // Create an Encoder object based on the GUID  
                    // for the Quality parameter category.  
                    System.Drawing.Imaging.Encoder myEncoder =
                        System.Drawing.Imaging.Encoder.Quality;

                    EncoderParameters myEncoderParameters = new EncoderParameters(1);

                    EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 100L);

                    myEncoderParameters.Param[0] = myEncoderParameter;

                    n.Save(v.ChangeExtension("jpg").ChangeFileName(v.GetFileName() + "-320X180"), jpgEncoder, myEncoderParameters);
                }
            });
        }

        private void 整理MOBIToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClipboardDirectoryAction(v =>
            {
                HelperMobi.ReName(v);
            });
        }

        private void 删除重复的MOBIToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClipboardDirectoryAction(v =>
            {
                var mfs = Directory.GetFiles(v, "*.mobi");
                var afs = Directory.GetFiles(v, "*.azw3").Select(i => i.GetFileNameWithoutExtension());
                var t = v.Combine("重复");
                t.CreateDirectoryIfNotExists();
                foreach (var item in mfs)
                {
                    if (afs.Contains(item.GetFileNameWithoutExtension()))
                    {
                        File.Move(item, t.Combine(item.GetFileName()));
                    }
                }
            });
        }
        KeyboardHook _keyboardHotKeyF6 = new KeyboardHook();

        private void 编译SCSSToolStripMenuItem_Click(object sender, EventArgs e)
        {

            _keyboardHotKeyF6.RegisterHotKey(Shared.ModifierKeys.None, Keys.F6);

            _keyboardHotKeyF6.KeyPressed += (o, k) =>
            {



                var fileName = @"C:\Program Files\libsass\sassc.exe";
                var targetFileName = @"C:\Users\Administrator\Desktop\ZZZ\.frontend\app.scss";
                var outFileName = @"C:\Users\Administrator\Desktop\ZZZ\.frontend\app.css";
                Process.Start(fileName, $"--style compact \"{targetFileName}\" \"{outFileName}\"");
            };
        }

        private void 下载nginxconfToolStripMenuItem_Click(object sender, EventArgs e)
        {

            var host = "180.76.145.233";
            var username = "root";
            var password = "yzlA9Q82itbN#";
            using (var sf = new Renci.SshNet.SftpClient(host, username, password))
            {
                sf.Connect();
                var f = "nginx.conf".GetDesktopPath();
                using (var stream = File.Open(f, FileMode.OpenOrCreate))
                {
                    sf.DownloadFile("/etc/nginx/" + f.GetFileName(), stream);
                }

            }

        }

        private void 上传databasesToolStripMenuItem_Click(object sender, EventArgs e)
        {

            ClipboardFilesAction(fs =>
            {
                var host = "180.76.145.233";
                var username = "root";
                var password = "yzlA9Q82itbN#";


                using (var sf = new Renci.SshNet.SftpClient(host, username, password))
                {
                    sf.Connect();

                    foreach (var f in fs)
                    {


                        var tf = "/usr/share/nginx/html/databases/" + f.GetFileName();




                        using (var stream = File.Open(f, FileMode.OpenOrCreate))
                        {
                            sf.UploadFile(stream, tf);
                        }

                    }
                }

            });
        }



        private void 下载databasesToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            ClipboardFilesAction(fs =>
            {
                var host = "180.76.145.233";
                var username = "root";
                var password = "yzlA9Q82itbN#";


                using (var sf = new Renci.SshNet.SftpClient(host, username, password))
                {
                    sf.Connect();



                    var tf = "/usr/share/nginx/html/databases/articles.db";



                    var stream = new FileStream("articles.db".GetDesktopPath(), FileMode.OpenOrCreate);

                    sf.DownloadFile(tf, stream);

                    stream.Close();

                }
            }

            );
        }

        private void 排序JSONToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClipboardTextAction(v =>
            {
                var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, List<Dictionary<string, string>>>>(v);

                var items = obj["items"].OrderBy(i => i["name"]).ToList();

                var outputObj = new Dictionary<string, List<Dictionary<string, string>>>();
                outputObj.Add("items", items);

                return Newtonsoft.Json.JsonConvert.SerializeObject(outputObj);
            });
        }

        private void 移除空行ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClipboardTextAction(v =>
            {

                return string.Join("\n", v.Split('\n').Where(i => i.IsReadable()).Select(i => i.TrimEnd()));
            });
        }

        private void 提取PREToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClipboardDirectoryAction(v =>
            {
                var files = Directory.GetFiles(v, "*.html");
                var hd = new HtmlAgilityPack.HtmlDocument();
                var sb = new StringBuilder();

                foreach (var itemv in files)
                {
                    hd.LoadHtml(itemv.ReadAllText());

                    var children = hd.DocumentNode.SelectNodes("//h1|//h2|//pre");
                    if (children == null) return;

                    foreach (var item in children)
                    {
                        //var items = item.ChildNodes;

                        //foreach (var its in items)
                        //{
                        //    if (its.GetAttributeValue("class", "") != "p")
                        //        sb.Append(HtmlAgilityPack.HtmlEntity.DeEntitize(its.InnerText));
                        //    else

                        //        sb.AppendLine(HtmlAgilityPack.HtmlEntity.DeEntitize(its.InnerText));
                        //}
                        if (item.Name == "h1")
                        {
                            sb.AppendLine("# " + HtmlAgilityPack.HtmlEntity.DeEntitize(item.InnerText).Trim()).AppendLine();
                            continue;
                        }
                        if (item.Name == "h2")
                        {
                            sb.AppendLine("## " + HtmlAgilityPack.HtmlEntity.DeEntitize(item.InnerText).Trim()).AppendLine();
                            continue;
                        }
                        sb.AppendLine().AppendLine().Append("```").AppendLine();
                        sb.AppendLine(Regex.Replace(HtmlAgilityPack.HtmlEntity.DeEntitize(item.InnerText), "[\r\n]+", "\r\n"));
                        sb.Append("```").AppendLine().AppendLine();

                    }
                }


                Clipboard.SetText(sb.ToString());
            });
        }

        private void 格式化代码为段落ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClipboardTextAction(v =>
            {

                var sb = new StringBuilder();

                var splited = v.Split('\n').Select(i => i.TrimEnd());

                foreach (var item in splited)
                {
                    sb.AppendLine($"\"{item.FormatCode()}\",");
                }
                return sb.ToString();
            });
        }

        private void 生成CSSLinksToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClipboardDirectoryAction(v =>
            {
                var files = Directory.GetFiles(v, "*.css");

                var sb = new StringBuilder();

                foreach (var item in files)
                {
                    sb.AppendLine($"<link href=\"{item.GetFileName()}\" rel=\"stylesheet\">");

                }

                Clipboard.SetText(sb.ToString());
            });

        }

        private void 合并CSSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClipboardDirectoryAction(v =>
            {
                var files = Directory.GetFiles(v, "*.css");

                var sb = new StringBuilder();

                foreach (var item in files)
                {
                    sb.Append(item.ReadAllText()).AppendLine();

                }

                Clipboard.SetText(sb.ToString());
            });
        }
    }
}
