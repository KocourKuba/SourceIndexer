using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace SourceIndexerNS
{
    public class Git
    {
        public static string GetRevisionSha(string sourcePath)
        {
            string currentRevision = RunGitCommand(sourcePath, "-C \"" + sourcePath + "\" log -1 --pretty=%H");

            return currentRevision.Trim();
        }

        public static List<string> GetFileList(string sourcePath)
        {
            string output = RunGitCommand(sourcePath, "ls-files");
            output = output.Trim();
            var lines = output.Split('\n');
            return lines.ToList<string>();
        }

        public static List<string> FindSubModules(string sourcePath)
        {
            var results = new List<string>();
            results.Add(sourcePath);
            var subModulesStr = RunGitCommand(sourcePath, "submodule status");
            var regex = new Regex(@"[\d\w]+ (.*) \(.*\)");
            var matches = regex.Matches(subModulesStr);
            foreach (Match match in matches)
            {
                var relativePath = match.Groups[1].Value.ToString();
                results.Add(Path.Combine(sourcePath, relativePath));
            }

            return results;
        }

        public static string FindRemoteUrl(string sourcePath)
        {
            return RunGitCommand(sourcePath, "remote get-url --all origin");
        }

        static string RunGitCommand(string sourcePath, string args)
        {
            ProcessStartInfo info = new ProcessStartInfo
            {
                Arguments = args,
                FileName = "git.exe",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = sourcePath
            };
            Process process = Process.Start(info);
            string stdOut = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            return stdOut.Trim();
        }
    }
}
