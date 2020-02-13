using System;
using System.Diagnostics;
using System.IO;

namespace SourceIndexerNS
{
    public class PdbStr
    {
        static public string ReadStream(SettingsBean Parameters, string streamName)
        {
            ProcessStartInfo info = new ProcessStartInfo
            {
                FileName = string.Format("\"{0}\\pdbstr.exe\"", Parameters.ToolsPath),
                Arguments = string.Format("-r -p:\"{0}\" -s:{1}", Parameters.PdbFile, streamName),
                UseShellExecute = false,
                RedirectStandardOutput = true
            };
            Process process = Process.Start(info);
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            return output;
        }

        static public string ReadStream(SettingsBean Parameters)
        {
            return ReadStream(Parameters, "srcsrv");
        }

        static public void WriteStream(SettingsBean Parameters, string streamContents)
        {
            WriteStream(Parameters, "srcsrv", streamContents);
        }

        static public void WriteStream(SettingsBean Parameters, string streamName, string streamContents)
        {
            var outFilePath = "SrcSrvOut.txt";
            File.WriteAllText(outFilePath, streamContents);

            ProcessStartInfo info = new ProcessStartInfo
            {
                FileName = string.Format("\"{0}\\pdbstr.exe\"", Parameters.ToolsPath),
                Arguments = string.Format("-w -p:\"{0}\" -s:{1} -i:{2}", Parameters.PdbFile, streamName, outFilePath),
                UseShellExecute = false
            };
            Process process = Process.Start(info);
            process.WaitForExit();
        }
    }
}
