using System.Collections.Generic;

namespace SourceIndexerNS
{
    public abstract class IFrontEnd
    {
        SettingsBean Parameters;
        public Logger LogWriter = new Logger();

        public virtual void SetParameters(SettingsBean Params)
        {
            Parameters = Params;
        }

        public abstract void EvaluateFiles(List<SourceFile> pdbFiles);
        public abstract List<RepositoryInfo> GetRepositoryInfo();
    }
}
