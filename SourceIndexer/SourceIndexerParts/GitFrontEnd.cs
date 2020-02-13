using System.Collections.Generic;
using System.IO;

namespace SourceIndexerNS
{
    public class GitFrontEnd : IFrontEnd
    {
        protected List<RepositoryInfo> Repositories = new List<RepositoryInfo>();
        public override void SetParameters(SettingsBean Params)
        {
            base.SetParameters(Params);
            FindRepositories(Params.SourcePath);
        }

        public override void EvaluateFiles(List<SourceFile> pdbFiles)
        {
            foreach (var repo in Repositories)
            {
                var gitFiles = Git.GetFileList(repo.RepositoryPath);

                if (LogWriter.IsVerboseEnough(VerbosityLevel.Detailed))
                {
                    LogWriter.Log(VerbosityLevel.Detailed, string.Format("Git repo {0} at '{1}' has file list:", repo.Location, repo.RepositoryPath));
                    foreach (var file in gitFiles)
                    {
                        LogWriter.Log(VerbosityLevel.Detailed, string.Format(" {0}", file));
                    }
                }

                List<SourceFile> remainingPdbFiles = new List<SourceFile>();
                Utilities.FindSourceFileIntersection(pdbFiles, repo.RepositoryPath, gitFiles, ref repo.SourceFiles, ref remainingPdbFiles, LogWriter);
                pdbFiles = remainingPdbFiles;

                LogWriter.Log(VerbosityLevel.Basic, string.Format("Repository {0} found with {1} matching files:", repo.Location, repo.SourceFiles.Count));
                if (LogWriter.IsVerboseEnough(VerbosityLevel.Detailed))
                {
                    foreach (var file in repo.SourceFiles)
                    {
                        LogWriter.Log(VerbosityLevel.Detailed, string.Format(" {0}", file.FullPath));
                    }
                }
            }
        }
        public override List<RepositoryInfo> GetRepositoryInfo()
        {
            return Repositories;
        }
        protected void FindRepositories(string sourceRoot)
        {
            var repos = Git.FindSubModules(sourceRoot);
            foreach (var repo in repos)
            {
                RepositoryInfo repoInfo = new RepositoryInfo
                {
                    RepositoryName = Path.GetFileName(repo),
                    RepositoryType = "git",
                    RepositoryPath = repo,
                    CurrentId = Git.GetRevisionSha(repo),
                    Location = Git.FindRemoteUrl(repo)
                };
                Repositories.Add(repoInfo);

                LogWriter.Log(VerbosityLevel.Basic, string.Format("Found Git module {0} at sha {1} with remote {2}", repoInfo.RepositoryPath, repoInfo.CurrentId, repoInfo.Location));
            }
        }
    }
}
