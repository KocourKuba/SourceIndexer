﻿using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace SourceIndexerNS
{
    public class GitHubBackEnd : IBackEnd
    {
        public override string ToString()
        {
            return "GitHub";
        }
        public override string BuildSrcSrvStream(List<RepositoryInfo> repositories)
        {
            var builder = new StringBuilder();
            builder.AppendLine("SRCSRV: ini ------------------------------------------------");
            builder.AppendLine("VERSION=1");
            builder.AppendLine("VERCTRL=http");
            builder.AppendLine("SRCSRV: variables------------------------------------------");
            builder.AppendLine("HTTP_ALIAS=https://raw.githubusercontent.com/");
            builder.AppendLine("HTTP_EXTRACT_TARGET=%HTTP_ALIAS%%var2%/%var3%/%var4%/%var5%");
            builder.AppendLine("SRCSRVTRG=%HTTP_EXTRACT_TARGET%");
            builder.AppendLine("SRCSRV: source files ---------------------------------------");
            foreach (var repo in repositories)
            {
                string organization = "";
                string project = "";
                ExtractRepoInfo(repo.Location, ref organization, ref project);
                foreach (var file in repo.SourceFiles)
                {
                    builder.AppendLine(string.Format("{0}*{1}*{2}*{3}*{4}", file.PdbFilePath, organization, project, repo.CurrentId, file.RelativePath));
                }
            }
            builder.AppendLine("SRCSRV: end------------------------------------------------");
            return builder.ToString();
        }

        bool ExtractRepoInfo(string remoteUrl, ref string organization, ref string project)
        {
            var regex = new Regex(@"github\.com\/([\d\w-_]+)\/([\d\w-_]+)");
            var match = regex.Match(remoteUrl);
            if (match.Success)
            {
                organization = match.Groups[1].Value.ToString();
                project = match.Groups[2].Value.ToString();
                LogWriter.Log(VerbosityLevel.Detailed, string.Format("Repo '{0}' found with organization '{1}' and project '{2}'", remoteUrl, organization, project));
                return true;
            }
            return false;
        }
    }
}
