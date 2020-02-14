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
        private readonly char[] charsToTrim = { '\\', '/' };

        class CommonOptions
        {
            [Option('p', "pdbFile", Required = true, HelpText = "Path to the pdb file.")]
            public string PdbFile { get; set; }
            [Option('t', "tools", Required = false, HelpText = "The path where Source Server files are located. If omitted used \"External\" folder near the app.")]
            public string ToolsPath { get; set; }
        }

        [Verb("index", HelpText = "Perform indexing source files.")]
        class IndexOptions : CommonOptions
        {
            [Option('s', "source", Required = true, HelpText = "Path to the source files.")]
            public string SourcePath { get; set; }

            [Option('i', "ini", Required = false, HelpText = "The path where the srcsrv.ini file is located. If not specified used file in the Source Server path.")]
            public string IniPath { get; set; }

            [Option('b', "backend", Required = false, HelpText = "Backend used for indexing. Allowed options: CMD, GIT, GITHUB. Default: CMD")]
            public string BackEnd { get; set; }
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

            var tools = (opts.ToolsPath != null) ? opts.ToolsPath.TrimEnd(charsToTrim) : "External";
            var ini = (opts.IniPath != null) ? opts.IniPath.TrimEnd(charsToTrim) : tools;
            SettingsBean Params = new SettingsBean
            {
                PdbFile = Path.GetFullPath(opts.PdbFile),
                SourcePath = Path.GetFullPath(opts.SourcePath.TrimEnd(charsToTrim)),
                ToolsPath = Path.GetFullPath(tools),
                SrcSrvIniPath = Path.GetFullPath(ini),
                BackEndType = (opts.BackEnd != null) ? opts.BackEnd.ToUpper() : "CMD",
            };

            new SourceIndexer(Params).RunSourceIndexing();
            return 0;
        }
        private object EvalAndReturnExitCode(EvalOptions opts)
        {
            SettingsBean Params = new SettingsBean
            {
                PdbFile = Path.GetFullPath(opts.PdbFile),
                ToolsPath = Path.GetFullPath((opts.ToolsPath != null) ? opts.ToolsPath.TrimEnd(charsToTrim) : "External"),
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
