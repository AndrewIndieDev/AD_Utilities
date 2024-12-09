using UnityEngine;

public static class TextForLoadingScreens
{
    public static string[] loadingLines = new string[] {
        "I hear you like loading screens.",
        "I like loading screens.",
        "I like loading screens too.",
        "Loading is fun.",
        "Wait I just found a loading screen.",
        "I found a loading screen.",
        "I found a loading screen. I'm so happy.",
        "A wild loading screen appears.",
        "The loading screen is a wild one."
    };

    public static string GetRandomLoadingText()
    {
        return loadingLines[Random.Range(0, loadingLines.Length)];
    }
}
