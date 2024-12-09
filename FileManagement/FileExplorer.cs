using UnityEngine;

namespace AndrewDowsett.FileManagement
{
    public class FileExplorer : MonoBehaviour
    {
        public static string path_GameFolder = $"D:/<folder path>/<project folder name>";

        //public static string path_TestFolder = $"{path_GameFolder}/_game/TestFolder";

#if !UNITY_EDITOR
    private void Start()
    {
        path_GameFolder             = Application.dataPath;
        path_GameFolder             = path_GameFolder.Substring(0, path_GameFolder.LastIndexOf('/'));
    }
#endif

        public static void ShowInExplorer(string itemPath)
        {
            itemPath = itemPath.Replace(@"/", @"\");   // explorer doesn't like front slashes
            System.Diagnostics.Process.Start("explorer.exe", "/select," + itemPath);
        }
    }
}