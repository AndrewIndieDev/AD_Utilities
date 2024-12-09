using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace AndrewDowsett.FileManagement
{
    public class IniFile
    {
        public string FullPath { get; set; }
        public string FolderPath { get; set; }
        public string FileName { get; set; }

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        static extern long WritePrivateProfileString(string Section, string Key, string Value, string FilePath);

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        static extern int GetPrivateProfileString(string Section, string Key, string Default, StringBuilder RetVal, int Size, string FilePath);

        public IniFile(string IniPath, string IniName)
        {
            string fullPath = $"{IniPath}/{IniName}.ini";

            if (!Directory.Exists(IniPath))
                Directory.CreateDirectory(IniPath);
            if (!File.Exists(fullPath))
                File.Create(fullPath);

            FolderPath = IniPath;
            FileName = IniName;
            FullPath = fullPath;
        }

        public string Read(string Key, string Section, string def = null)
        {
            var RetVal = new StringBuilder(255);
            GetPrivateProfileString(Section, Key, "", RetVal, 255, FolderPath);
            string temp = RetVal.ToString();
            return (temp.Length > 0) ? temp : ((def == null) ? "" : def);
        }

        public void Write(string Key, string Value, string Section)
        {
            WritePrivateProfileString(Section, Key, Value, FolderPath);
        }

        public void DeleteKey(string Key, string Section = null)
        {
            Write(Key, null, Section);
        }

        public void DeleteSection(string Section)
        {
            Write(null, null, Section);
        }

        public bool KeyExists(string Key, string Section = null)
        {
            return Read(Key, Section).Length > 0;
        }

        public List<string> SectionToList(string folderPath, string fullPath)
        {
            if (!Directory.Exists(folderPath))
            {
                return null;
            }

            List<string> sectionList = new List<string>();
            StreamReader sr = new StreamReader(fullPath);

            string data = sr.ReadToEnd();
            string sec;

            if (data.Length == 0)
                return null;

            data = data.Remove(0, 1);
            while (data.Contains("]"))
            {
                sec = data.Substring(0, data.IndexOf(']'));
                sec = sec.Trim();
                int temp = (data.IndexOf('[') + 1 != 0) ? data.IndexOf('[') + 1 : -1;
                data = (temp == -1) ? data.Remove(0) : data.Remove(0, data.IndexOf('[') + 1);
                sectionList.Add(sec);
            }

            sr.Close();

            return sectionList;
        }

        public void RemoveSection(string section, string path)
        {
            StreamReader sr = new StreamReader(path);
            string data = sr.ReadToEnd();
            string dataremoved = data.Substring(data.IndexOf(section) - 1);
            dataremoved = dataremoved.Remove(0, 1);
            int temp = ((dataremoved.IndexOf('[') + 1) != 0) ? dataremoved.IndexOf('[') + 1 : -1;
            string toRemove = (temp == -1) ? data.Substring(data.IndexOf(section) - 1) : data.Substring(data.IndexOf(section) - 1, temp);
            data = data.Replace(toRemove, "");
            sr.Close();

            StreamWriter sw = new StreamWriter(path);
            sw.Write(data);
            sw.Close();
        }
    }
}