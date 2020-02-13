using System.Collections.Generic;

namespace SourceIndexerNS
{
    public abstract class IBackEnd
    {
        public Logger Logger = new Logger();
        public abstract string BuildSrcSrvStream(List<RepositoryInfo> repositories);
    }
}
