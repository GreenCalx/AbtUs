
using UnityEngine;
using UnityEngine.Events;
using System;
using System.IO;
using System.Collections.Generic;

public static class EventLog
{
    public const string filePath = "/event_log.txt";
    public const string prevLogFilePath = "/prev_event_log.txt";
    public static List<string> pendingLogs = new List<string>();
    private static StreamWriter sw;

    public static void OK(string iLog)
    { ELog("[OK] " + iLog); }

    public static void FAIL(string iLog)
    { ELog("[FAIL] " + iLog); }

    public static void INFO(string iLog)
    { ELog("[INFO] " + iLog); }

    public static void ELog(string iLog)
    {
        string s = System.TimeSpan.FromSeconds(Time.realtimeSinceStartup).ToString("hh':'mm':'ss");
        s += " : ";
        s += iLog;
        pendingLogs.Add(s);
    }

    public static void Write()
    {
        if (sw == null)
        { Init(); }

        foreach (string log in pendingLogs)
        {
            sw.WriteLine(log);
        }
        pendingLogs.Clear();
    }

    public static void Init()
    {
        if (File.Exists(Application.dataPath + filePath))
        {
            File.Delete(Application.dataPath + prevLogFilePath);
            File.Copy(Application.dataPath + filePath, Application.dataPath + prevLogFilePath);
            File.Delete(Application.dataPath + filePath);
        }
        sw = File.CreateText(Application.dataPath + filePath);
    }

    public static void Close()
    {
        sw.Close();
    }
}
