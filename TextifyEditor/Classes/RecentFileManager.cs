using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Textify_Editor
{
    public static class RecentFileManager
    {
        private static readonly string FilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Textify Editor", "recent_files.txt");

        static RecentFileManager()
        {
            var dir = Path.GetDirectoryName(FilePath);
            if (!Directory.Exists(dir)) 
                Directory.CreateDirectory(dir);
        }

        public static List<string> GetRecentFiles()
        {
            if (!File.Exists(FilePath)) return new List<string>();
            return File.ReadAllLines(FilePath)
                .Where(File.Exists)
                .ToList();
        }

        public static void Clear()
        {
            if (File.Exists(FilePath)) 
                File.Delete(FilePath);
        }

        public static void Remove(string filePath)
        {
            var list = GetRecentFiles();
            list.Remove(filePath);
            File.WriteAllLines(FilePath, list);
        }

        public static void Add(string file)
        {
            var list = GetRecentFiles();

            list.Remove(file);
            list.Insert(0, file);

            if (list.Count > 10)
                list.Take(10).ToList();

            File.WriteAllLines(FilePath, list);
        }
    }

    public class RecentFileItem
    {
        public string FileName { get; set; }
        public string FullPath { get; set; }

        public override string ToString()
        {
            return Path.GetFileName(FullPath);
        }
    }
}
