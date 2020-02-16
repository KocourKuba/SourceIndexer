namespace SourceIndexerNS
{
    public class SettingsBean
    {
        public string SourcePath { get; set;}
        public string PdbFile { get; set; }
        public string ToolsPath { get; set; } = "External";
        public string SrcSrvIniPath { get; set; } = "External";
        public string BackEndType { get; set; } = "CMD";
        public string CustomCommand { get; set; }
        public string LogLevel { get; set; }
    }
}
