using UnityEngine;

namespace DebugStuff
{
    public class ConsoleToGUI : MonoBehaviour
    {
        private static string myLog = ""; // Stores the log messages
        private string output;            // Stores the latest log message
        private string stack;             // Stores the latest stack trace

        private Vector2 scrollPosition;   // Tracks the scroll position

        void OnEnable()
        {
            Application.logMessageReceived += Log; // Subscribe to the log event
        }

        void OnDisable()
        {
            Application.logMessageReceived -= Log; // Unsubscribe to avoid memory leaks
        }

        // Logs the message, stack trace, and type
        public void Log(string logString, string stackTrace, LogType type)
        {
            output = logString;
            stack = stackTrace;
            myLog = $"{output}\n{myLog}";

            // Limit the log size to prevent excessive memory usage
            if (myLog.Length > 5000)
            {
                myLog = myLog.Substring(0, 4000);
            }
        }

        void OnGUI()
        {
            // Create a scrollable area
            scrollPosition = GUI.BeginScrollView(
                new Rect(10, 10, 520, 300), // Scroll view position and size
                scrollPosition,            // Current scroll position
                new Rect(0, 0, 500, Mathf.Max(300, myLog.Length * 10)) // Viewable area
            );

            // Display the log messages inside the scrollable area
            GUI.TextArea(new Rect(0, 0, 500, Mathf.Max(300, myLog.Length * 10)), myLog);

            // End the scroll view
            GUI.EndScrollView();
        }
    }
}
