using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SourceIndexerNS
{
    public class SrcTool
    {
        static public string GetUnindexedList(SettingsBean Parameters)
        {
            return RunSrcTool(Parameters, "-u");
        }
        static public string GetSrcSrvResults(SettingsBean Parameters)
        {
            return RunSrcTool(Parameters, "-n");
        }
        static public List<SourceFile> GetFileListing(SettingsBean Parameters)
        {
            var output = RunSrcTool(Parameters, "-r");
            var lines = output.Split('\n');

            // Copy all of the lines out. The last line is a newline and the one before that is a summary hence the -2.
            var results = new List<SourceFile>();
            for (var i = 0; i < lines.Length - 2; ++i)
            {
                var file = new SourceFile() { PdbFilePath = lines[i].Trim() };
                results.Add(file);
            }
            return results;
        }
        static public string RunSrcTool(SettingsBean Parameters, string arguments)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = string.Format("\"{0}\\srctool.exe\"", Parameters.ToolsPath),
                Arguments = string.Format("\"{0}\" {1}", Parameters.PdbFile, arguments),
                RedirectStandardOutput = true,
                UseShellExecute = false
            };

            var process = Process.Start(startInfo);
            var output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            return output;
        }
    }
}
