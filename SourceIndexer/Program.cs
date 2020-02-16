using System;
using System.IO;
using System.Reflection;
using System.Linq;
using CommandLine;

namespace SourceIndexerNS
{
    class Program
    {
        /// <summary>
        /// The main application entry point.
        /// </summary>
        /// <param name="args">The command line arguments.</param>
        /// <remarks>
        /// SourceIndexer <index> <-p|--pdb <filename.pdb>> [-s|--source <path>] [-t|--tools <path>] [-i|--ini <path>]
        ///               [-s|--source <path>] [-t|--tools <path>] [-i|--ini <path>] [<eval> <filename.pdb>]
        ///               [-?|-h|--help]
        /// </remarks>

        class CommonOptions
        {
            [Option('p', "pdbFile", Required = true, HelpText = "Path to the pdb file.")]
            public string PdbFile
            {
                get { return _pdbFile; }
                set { _pdbFile = Path.GetFullPath(value); }
            }
            [Option('t', "tools", Required = false, HelpText = "The path where Source Server files are located. If omitted used \"External\" folder near the app.")]
            public string ToolsPath
            {
                get { return _toolsPath; }
                set { _toolsPath = Path.GetFullPath(value ?? "External"); }
            }
            [Option('l', "log", Required = false, HelpText = "Log level: None, Error, Warning, Basic, Detailed")]
            public string LogLevel { get; set; } = "Basic";

            private string _pdbFile;
            private string _toolsPath;
        }

        [Verb("index", HelpText = "Perform indexing source files.")]
        class IndexOptions : CommonOptions
        {
            private readonly char[] charsToTrim = { '\\', '/' };

            [Option('s', "source", Required = true, HelpText = "Path to the source files.")]
            public string SourcePath
            {
                get { return _sourcePath; }
                set { _sourcePath = Path.GetFullPath(value.TrimEnd(charsToTrim)); }
            }

            [Option('i', "ini", Required = false, HelpText = "The path where the srcsrv.ini file is located. If not specified used file in the Source Server path.")]
            public string IniPath
            {
                get { return _iniPath; }
                set { _iniPath = Path.GetFullPath(value.TrimEnd(charsToTrim)); }
            }

            [Option('b', "backend", Required = false, HelpText = "Backend used for indexing. Allowed options: CMD, GIT, GITHUB. Default: CMD")]
            public string BackEnd
            {
                get { return _backEnd; }
                set { _backEnd = value ?? "cmd"; }
            }

            private string _sourcePath;
            private string _iniPath;
            private string _backEnd;
        }

        [Verb("eval", HelpText = "Record changes to the repository.")]
        class EvalOptions : CommonOptions
        {
        }

        static void Main(string[] args)
        {
            new Program().ParseCommandLine(args);
        }
        private void ParseCommandLine(string[] args)
        {
            var res = CommandLine.Parser.Default.ParseArguments<IndexOptions, EvalOptions>(args)
                    .MapResult(
                      (IndexOptions opts) => IndexAndReturnExitCode(opts),
                      (EvalOptions opts) => EvalAndReturnExitCode(opts),
                      errs => 1);
        }

        private object IndexAndReturnExitCode(IndexOptions opts)
        {
            if (opts.SourcePath.Length == 0 || opts.PdbFile.Length == 0)
                return 1;

            var ini = opts.IniPath ?? opts.ToolsPath;
            var arr = opts.BackEnd.Split(':');
            var backEndType = arr[0].ToUpper();
            var customCmd = arr.Length > 1 ? arr[1] : "git";

            SettingsBean Params = new SettingsBean
            {
                PdbFile = opts.PdbFile,
                SourcePath = opts.SourcePath,
                ToolsPath = opts.ToolsPath,
                SrcSrvIniPath = ini,
                BackEndType = backEndType,
                CustomCommand = customCmd,
                LogLevel = opts.LogLevel,
            };

            new SourceIndexer(Params).RunSourceIndexing();
            return 0;
        }
        private object EvalAndReturnExitCode(EvalOptions opts)
        {
            SettingsBean Params = new SettingsBean
            {
                PdbFile = opts.PdbFile,
                ToolsPath = opts.ToolsPath,
            };

            var indexer = new SourceIndexer(Params);
            string result = indexer.EvaluateSourceIndexing();
            Console.Write(result);
            return 0;
        }

        private string GetFileVersion(Assembly assembly, int components)
        {
            var fileVersion = (string)assembly.CustomAttributes.Single(c => c.AttributeType == typeof(AssemblyFileVersionAttribute)).ConstructorArguments[0].Value;
            var versionComponents = fileVersion.Split('.');
            return string.Join(".", versionComponents.Take(components).ToArray());
        }
    }
}
