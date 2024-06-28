using System.Diagnostics;
using UnityEngine;

public class PythonProcessor : MonoBehaviour
{
    // Path must be consistent per device
    //const string INTERPRETER_PATH = @"C:\Users\james\AppData\Local\Programs\Python\Python311\python.exe";
    const string INTERPRETER_PATH = @"C:\Users\ariya\AppData\Local\Programs\Python\Python311\python.exe";

    const string DIRECTORY_NAME = "App";
    const string SCRIPT_NAME = "app";

    Process process;

    public static PythonProcessor Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        StartProgram();
    }

    public void StartProgram()
    {
        UnityEngine.Debug.Log("Starting Program");

        // Innit start info
        ProcessStartInfo startInfo = new();
        startInfo.FileName = INTERPRETER_PATH;

        // Assign directory
        string location = $"{Application.dataPath}\\Python\\{DIRECTORY_NAME}";
        startInfo.Arguments = $"{location}\\{SCRIPT_NAME}.py";
        startInfo.WorkingDirectory = location;

        // Settings
        startInfo.UseShellExecute = false;
        startInfo.RedirectStandardOutput = true;
        startInfo.RedirectStandardError = true;
        startInfo.CreateNoWindow = true;

        // Innit process
        process = new();
        process.StartInfo = startInfo;

        // Log Python console
        process.OutputDataReceived +=
            (sender, args) => UnityEngine.Debug.Log("Python Output: " + args.Data);
        process.ErrorDataReceived +=
            (sender, args) => UnityEngine.Debug.LogError("Python Error: " + args.Data);

        // Start process
        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
    }

    // Terminate process and cleanup
    void EndProcess()
    {
        if (process != null && !process.HasExited)
        {
            process.Kill();
            process.Dispose();
        }
    }
    private void OnDisable() => EndProcess();
    private void OnDestroy() => EndProcess();
    private void OnApplicationQuit() => EndProcess();
}
