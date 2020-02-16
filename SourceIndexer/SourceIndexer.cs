using System;
using System.Collections.Generic;

namespace SourceIndexerNS
{
    public class SourceFile
    {
        public string PdbFilePath = "";
        public string FullPath = "";
        public string RelativePath = "";
    }
    public class RepositoryInfo
    {
        public List<SourceFile> SourceFiles = new List<SourceFile>();
        public string RepositoryName = "";
        public string RepositoryPath = "";
        public string RepositoryType = "";
        public string CurrentId = "";
        public string Location = "";
    }

    public class SourceIndexer
    {
        private Logger LogWriter = new Logger();
        private SettingsBean Parameters;
        private IFrontEnd FrontEnd = null;
        private IBackEnd BackEnd = null;
        public SourceIndexer(SettingsBean Params)
        {
            Parameters = Params;
            LogWriter = new ConsoleLogger();
            FrontEnd = new GitFrontEnd();
            if (Parameters.BackEndType == "GIT")
            {
                BackEnd = new GitBackEnd();
            }
            else if (Parameters.BackEndType == "GITHUB")
            {
                BackEnd = new GitHubBackEnd();
            }
            else
            {
                // Default backend
                BackEnd = new CmdBackEnd();
            }

            BackEnd.Parameters = Params;
        }
        public virtual void SetParameters(SettingsBean Params)
        {
            Parameters = Params;
        }
        public virtual string BuildSrcSrvStream(List<SourceFile> files) { return ""; }
        public virtual void RunSourceIndexing()
        {
            try
            {
                // Get the list of files from the source tool
                var files = GetFileListing();
                if (LogWriter.IsVerboseEnough(VerbosityLevel.Detailed))
                {
                    foreach (var file in files)
                    {
                        LogWriter.Log(VerbosityLevel.Detailed, string.Format(" {0}", file.PdbFilePath));
                    }
                }

                // Setup the front end
                FrontEnd.SetParameters(Parameters);
                // Evaluate the pdb source files given our front end
                FrontEnd.EvaluateFiles(files);
                // Pull out the repositories with all of their files
                var repositories = FrontEnd.GetRepositoryInfo();
                // Build the text data for the srcsrv stream from the backend
                var srcSrvStream = BackEnd.BuildSrcSrvStream(repositories);

                LogWriter.Log(VerbosityLevel.Detailed, string.Format("Writing stream\n{0}", srcSrvStream));

                // Write the stream out to the pdb file
                PdbStr.WriteStream(Parameters, srcSrvStream);
                int total = 0;
                foreach (var repo in repositories)
                {
                    total += repo.SourceFiles.Count;
                }

                LogWriter.Log(VerbosityLevel.Basic, string.Format("Pdb contains {0} files, {1} files indexed", files.Count, total));
            }
            catch (Exception ex)
            {
                LogWriter.Log(VerbosityLevel.Error, string.Format("Exception: {0}", ex.ToString()));
            }
        }
        public string GetSourceIndexingResults()
        {
            return PdbStr.ReadStream(Parameters);
        }

        public string EvaluateSourceIndexing()
        {
            return SrcTool.GetSrcSrvResults(Parameters);
        }

        public string GetUnindexedList()
        {
            return SrcTool.GetUnindexedList(Parameters);
        }

        // Returns the list of files in that need source indexing
        private List<SourceFile> GetFileListing()
        {
            return SrcTool.GetFileListing(Parameters);
        }
    }
}
