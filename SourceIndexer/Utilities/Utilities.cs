﻿using System.Collections.Generic;
using System.IO;

namespace SourceIndexerNS
{
    class Utilities
    {
        // Unfortunately, the pdb changes the case of the path. Mercurial is case
        // sensitive with some commands so this poses problems. Luckily windows can
        // find the file case-insenstive and then reconstruct the original path.
        public static string GetExactPathName(string pathName)
        {
            if (!(File.Exists(pathName) || Directory.Exists(pathName)))
                return pathName;

            var di = new DirectoryInfo(pathName);

            if (di.Parent != null)
            {
                return Path.Combine(GetExactPathName(di.Parent.FullName), di.Parent.GetFileSystemInfos(di.Name)[0].Name);
            }
            else
            {
                return di.Name.ToUpper();
            }
        }

        public static void FindSourceFileIntersection(List<SourceFile> pdbFiles, string repositoryRoot, List<string> repositoryFiles,
          ref List<SourceFile> intersectionList, ref List<SourceFile> remainingList, Logger logger)
        {
            var map = new Dictionary<string, SourceFile>();
            foreach (var relativePath in repositoryFiles)
            {
                var fullPath = Path.Combine(repositoryRoot, relativePath);
                // Fix the path to be correct for our OS (deals with slashes)
                fullPath = Path.GetFullPath(fullPath);

                var sourceFile = new SourceFile();
                sourceFile.FullPath = fullPath;
                sourceFile.RelativePath = relativePath;
                map[fullPath.ToLower()] = sourceFile;

                logger.Log(VerbosityLevel.Detailed, string.Format("Repo File {0} relative {1}", sourceFile.FullPath, sourceFile.RelativePath));
            }

            foreach (var file in pdbFiles)
            {
                var fullPathName = GetExactPathName(file.PdbFilePath);
                logger.Log(VerbosityLevel.Detailed, string.Format("Pdb File {0} full {1}", file.PdbFilePath, fullPathName));
                if (map.ContainsKey(fullPathName.ToLower()))
                {
                    var resultFile = map[fullPathName.ToLower()];
                    resultFile.PdbFilePath = file.PdbFilePath;
                    intersectionList.Add(resultFile);
                    logger.Log(VerbosityLevel.Detailed, "  found");
                }
                else
                {
                    remainingList.Add(file);
                    logger.Log(VerbosityLevel.Detailed, "  missing");
                }
            }
        }
    }
}
