
namespace Shared
{

    using System.Diagnostics;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Text;
    using System;
    using System.IO;

    public static class Linqs
    {
        public static IEnumerable<T> Distinct<T, U>(
    this IEnumerable<T> seq, Func<T, U> getKey)
        {
            return
                from item in seq
                group item by getKey(item) into gp
                select gp.First();
        }
    }
    public static class Files
    {
        private static readonly char[] InvalidFileNameChars = { '\"', '<', '>', '|', '\0', ':', '*', '?', '\\', '/' };



        public static string GetDirectoryFileName(this string v)
        {
            return Path.GetFileName(Path.GetDirectoryName(v));
        }

        public static string GetApplicationPath(this string v)
        {
            return Path.Combine(Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]), v);
        }
        public static string GetDesktopPath(this string fileName) => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), fileName);
        public static string GetUniqueFileName(this String v)
        {
            int i = 1;
            Regex regex = new Regex(" \\- [0-9]+");
            String t = Path.Combine(Path.GetDirectoryName(v),
                regex.Split(Path.GetFileNameWithoutExtension(v), 2).First() + " - " + i.ToString().PadLeft(3, '0') +
                Path.GetExtension(v));

            while (File.Exists(t))
            {
                i++;
                t = Path.Combine(Path.GetDirectoryName(v),
                    regex.Split(Path.GetFileNameWithoutExtension(v), 2).First() + " - " + i.ToString().PadLeft(3, '0') +
                    Path.GetExtension(v));
            }
            return t;
        }

        public static string GetValidFileName(this String v)
        {
            if (v == null) return null;
            // (Char -> Int) 1-31 Invalid;
            List<char> chars = new List<char>(v.Length);

            for (int i = 0; i < v.Length; i++)
            {
                if (InvalidFileNameChars.Contains(v[i]))
                {
                    chars.Add(' ');
                }
                else
                {
                    chars.Add(v[i]);
                }
            }

            return new String(chars.ToArray());
        }
        public static string GetFileSha1(this string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Open))
            using (BufferedStream bs = new BufferedStream(fs))
            using (var reader = new StreamReader(bs))
            {
                using (System.Security.Cryptography.SHA1Managed sha1 = new System.Security.Cryptography.SHA1Managed())
                {
                    byte[] hash = sha1.ComputeHash(bs);
                    StringBuilder formatted = new StringBuilder(2 * hash.Length);
                    foreach (byte b in hash)
                    {
                        formatted.AppendFormat("{0:X2}", b);
                    }
                }
                return reader.ReadToEnd();
            }
        }
        public static void FileCopy(this string path, string dstPath)
        {
            File.Copy(path, dstPath, false);
        }
        public static string GetUniqueImageRandomFileName(this string path)
        {
            var files = Directory.GetFiles(path, "*", SearchOption.AllDirectories).Select(i => i.GetFileNameWithoutExtension());
            var fileName = 8.GetRandomString();
            while (files.Contains(fileName))
                fileName = 8.GetRandomString();

            return fileName;
        }
        public static String GetCommandPath(this string fileName)
        {
            return Environment.GetCommandLineArgs()[0].GetDirectoryName().Combine(fileName);
        }
        public static UTF8Encoding sUTF8Encoding = new UTF8Encoding(false);

        public static IEnumerable<string> GetFiles(this String path, string pattern = "*")
        {
            foreach (var item in Directory.GetFiles(path, pattern))
            {
                yield return item;
            }
        }
        public static DirectoryInfo GetParent(this String path) => Directory.GetParent(path);
        public static void CreateDirectoryIfNotExists(this String path)
        {
            if (Directory.Exists(path)) return;
            Directory.CreateDirectory(path);
        }
        public static bool DirectoryExists(this String path) => Directory.Exists(path);
        public static string ChangeFileName(this string path, string fileNameWithoutExtension)
        {
            return Path.Combine(Path.GetDirectoryName(path), fileNameWithoutExtension + Path.GetExtension(path));
        }
        public static void DirectoryMove(this String sourceDirName, String destDirName) => Directory.Move(sourceDirName, destDirName);
        public static void DirectoryDelete(this String path, bool recursive = false)
        {
            if (recursive)
            {
                Directory.Delete(path, true);
            }
            else
            {
                Directory.Delete(path);
            }
        }
        public static void WriteAllText(this String path, String contents) => File.WriteAllText(path, contents, sUTF8Encoding);
        public static void WriteAllBytes(this String path, byte[] bytes) => File.WriteAllBytes(path, bytes);
        public static void WriteAllLines(this String path, String[] contents) => File.WriteAllLines(path, contents, sUTF8Encoding);

        public static void AppendAllText(this String path, String contents) => File.AppendAllText(path, contents, sUTF8Encoding);
        public static void AppendAllLines(this String path, IEnumerable<String> contents, Encoding encoding) => File.AppendAllLines(path, contents, sUTF8Encoding);
        public static void FileMove(this String sourceFileName, String destFileName) => File.Move(sourceFileName, destFileName);
        public static void FileReplace(this String sourceFileName, String destinationFileName, String destinationBackupFileName) => File.Replace(sourceFileName, destinationFileName, destinationBackupFileName);

        public static StreamReader OpenText(this String path) => File.OpenText(path);
        public static StreamWriter CreateText(this String path) => File.CreateText(path);
        public static StreamWriter AppendText(this String path) => File.AppendText(path);
        public static FileStream Create(this String path) => File.Create(path);
        public static FileStream Open(this String path, FileMode mode, FileAccess access) => File.Open(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        public static DateTime FileGetCreationTimeUtc(this String path) => File.GetCreationTimeUtc(path);
        public static DateTime FileGetLastAccessTimeUtc(this String path) => File.GetLastAccessTimeUtc(path);
        public static DateTime FileGetLastWriteTimeUtc(this String path) => File.GetLastWriteTimeUtc(path);
        public static FileStream OpenRead(this String path) => File.OpenRead(path);
        public static FileStream OpenWrite(this String path) => File.OpenWrite(path);
        public static String ReadAllText(this String path) => File.ReadAllText(path, new UTF8Encoding(false));
        public static byte[] ReadAllBytes(this String path) => File.ReadAllBytes(path);

        public static String ChangeExtension(this String path, String extension) => Path.ChangeExtension(path, extension);
        public static string GetDirectoryName(this string path) => Path.GetDirectoryName(path);
        public static String GetExtension(this String path) => Path.GetExtension(path);
        public static String GetFullPath(this String path) => Path.GetFullPath(path);
        public static String GetFileName(this String path) => Path.GetFileName(path);
        public static String GetFileNameWithoutExtension(this String path) => Path.GetFileNameWithoutExtension(path);
        public static String GetPathRoot(this String path) => Path.GetPathRoot(path);
        public static String Combine(this String path1, String path2) => Path.Combine(path1, path2);
        public static string ToLine(this IEnumerable<string> value, string separator = "\r\n")
        {
            return string.Join(separator, value);
        }
        public static bool FileExists(this String path) => File.Exists(path);
        public static string GetValidFileName(this string value, char c)
        {

            var chars = Path.GetInvalidFileNameChars();

            return new string(value.Select<char, char>((i) =>
            {
                if (chars.Contains(i)) return c;
                return i;
            }).Take(125).ToArray());
        }
    }
    public static class Systems
    {

        public static IEnumerable<Process> FilterProcess(string regexPattern)
        {
            return Process.GetProcesses().Where(i => Regex.IsMatch(i.ProcessName, regexPattern));
        }

    }
    public static class Strings
    {


        public static string FormatCode(this string v)
        {

            v = v.Replace("\r", "");
            v = v.Replace("\\", "\\\\");
            v = v.Replace("\n", "\\n");
            v = v.Replace("\"", "\\\"");

            return v;
        }
        public static string Repeat(this char c, int count)
        {
            return new String(c, count);
        }
        public static string Repeat(this string value, int count)
        {
            return new StringBuilder(value.Length * count).Insert(0, value, count).ToString();
        }

        public static string RemoveHtmlTag(this string v)
        {
            return Regex.Replace(v, "<[^>]*?>", "");
        }
        public static string RemoveNonChinese(this string v)
        {
            return Regex.Replace(v, "[^\u4e00-\u9fa5]+", "");
        }


        public static bool IsWhiteSpace(this String value)
        {
            if (value == null) return true;

            for (int i = 0; i < value.Length; i++)
            {
                if (!Char.IsWhiteSpace(value[i])) return false;
            }

            return true;
        }
        public static IEnumerable<string> Matches(this string value, string pattern)
        {
            var match = Regex.Match(value, pattern);

            while (match.Success)
            {

                yield return match.Value;
                match = match.NextMatch();
            }
        }
        public static IEnumerable<string> MatchesMultiline(this string value, string pattern)
        {
            var match = Regex.Match(value, pattern, RegexOptions.Multiline);

            while (match.Success)
            {

                yield return match.Value;
                match = match.NextMatch();
            }
        }
        public static IEnumerable<string> MatchesByGroup(this string value, string pattern)
        {
            var match = Regex.Match(value, pattern);

            while (match.Success)
            {

                yield return match.Groups[1].Value;
                match = match.NextMatch();
            }
        }
        public static (string, string) SplitTwo(this string value, char c)
        {
            var array = value.Split(new char[] { c }, 2);
            return (array[0], array[1]);
        }
        public static int CountStart(this string value, char c)
        {
            var count = 0;

            foreach (var item in value)
            {
                if (item == c) count++;
                else break;
            }
            return count;
        }
        public static int ConvertToInt(this string value, int defaultValue = 0)
        {
            var match = Regex.Match(value, "[0-9]+");
            if (match.Success)
            {
                return int.Parse(match.Value);
            }
            return defaultValue;
        }
        public static string GetRandomString(this int length)
        {
            Random s_nameRand = new Random((int)(new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds()));

            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789_";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[s_nameRand.Next(s.Length)]).ToArray());
        }
        //public static string GetRandomStringAlpha(this int length)
        //{

        //    StringBuilder builder = new StringBuilder();
        //    char ch;
        //    for (int i = 0; i < length; i++)
        //    {
        //        ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * s_nameRand.NextDouble() + 65)));
        //        builder.Append(ch);
        //    }
        //    return builder.ToString();
        //}
        public static StringBuilder Append(this String value) => new StringBuilder().Append(value);

        public static StringBuilder AppendLine(this String value) => new StringBuilder().AppendLine(value);
        public static string GetFirstReadable(this string value) => value.TrimStart().Split(new char[] { '\n' }, 2).First().Trim();
        public static IEnumerable<string> ToLines(this string value) => value.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(i => i.Trim());
        public static string FlatToLine(this string value) => Regex.Replace(Regex.Replace(value, "[\r\n]+", " "), "\\s{2,}", " ");
        public static string Flat(this IEnumerable<string> collection, string separator = "\r\n") => string.Join(separator, collection);
        public static bool IsVacuum(this string value) => string.IsNullOrWhiteSpace(value);
        public static bool IsReadable(this string value) => !string.IsNullOrWhiteSpace(value);

        public static string RemoveEmptyLines(this string value)
        {

            return string.Join(Environment.NewLine, value.Split('\n').Where(i => i.IsReadable()).Select(i => i.TrimEnd()));
        }
    }
    public static class Formatters
    {
        public static string FormatNginxConf(string value)
        {
            var sb = new StringBuilder();
            var count = 0;
            foreach (var item in value)
            {
                if (item == '{')
                {
                    sb.AppendLine("{");
                    count++;
                }
                else if (item == '}')
                {
                    sb.AppendLine('\t'.Repeat(count) + "}");

                    count--;
                }
                else
               if (item == ';')
                {
                    sb.AppendLine(";");
                    sb.Append('\t'.Repeat(count));
                }
                else if (item == '\r' || item == '\n' || item == '\t')
                {

                    continue;
                }
                else
                {
                    sb.Append(item);
                }

            }
            return sb.ToString();
        }
        public static string FormatBlockComment(this string value)
        {
            var sb = new StringBuilder();
            var cacheSb = new StringBuilder();

            sb.Append("/*\r\n\r\n");
            foreach (var item in value.Split(new char[] { '\n' }))
            {
                if (item.IsReadable())
                {
                    foreach (var l in item.Split(' '))
                    {
                        if (l.IsReadable())
                        {

                            cacheSb.Append(l.Trim()).Append(' ');
                            if (cacheSb.Length > 50)
                            {
                                sb.Append(cacheSb).AppendLine();
                                cacheSb.Clear();
                            }
                        }
                    }
                    if (cacheSb.Length > 0)
                    {
                        sb.Append(cacheSb).AppendLine().AppendLine();
                        cacheSb.Clear();
                    }

                }
            }
            sb.Append("*/\r\n");
            return sb.ToString();
        }

        public static IEnumerable<string> FormatMethodList(this string value)
        {
            var count = 0;
            var sb = new StringBuilder();
            var ls = new List<string>();
            for (int i = 0; i < value.Length; i++)
            {
                sb.Append(value[i]);

                if (value[i] == '{')
                {
                    count++;
                }
                else if (value[i] == '}')
                {
                    count--;
                    if (count == 0)
                    {
                        ls.Add(sb.ToString());
                        sb.Clear();
                    }
                }

            }
            //if (ls.Any())
            //{
            //    var firstLine = ls[0];
            //    ls.RemoveAt(0);
            //    ls.Add(firstLine.)

            //}
            return ls.Select(i => i.Split(new char[] { '{' }, 2).First().Trim() + ";").OrderBy(i => i.Trim());

        }
    }


}