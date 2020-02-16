using System.Collections.Generic;

namespace SourceIndexerNS
{
    public abstract class IBackEnd
    {
        public SettingsBean Parameters { get; set; }
        public Logger LogWriter = new Logger();
        public abstract string BuildSrcSrvStream(List<RepositoryInfo> repositories);
    }
}
