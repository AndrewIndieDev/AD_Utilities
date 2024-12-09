using UnityEngine;
using System.IO;

namespace AndrewDowsett.FileManagement
{
    public class FileManager : MonoBehaviour
    {
        public static FileManager Instance;
        private void Awake()
        {
            Instance = this;
        }

        //public IniFile testInfo;

        private void Start()
        {
            //if (!Directory.Exists(FileExplorer.path_TestFolder))
            //Directory.CreateDirectory(FileExplorer.path_TestFolder);

            //testInfo = new IniFile(FileExplorer.path_TestFolder, "TestInfo");
        }
    }
}