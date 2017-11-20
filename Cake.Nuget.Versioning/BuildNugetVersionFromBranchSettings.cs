using System;
using System.Collections.Generic;
using System.Text;

namespace Cake.Nuget.Versioning
{
    public class BuildNugetVersionFromBranchSettings
    {
        public string BranchName { get; set; } = null;
        public string[] PreReleaseFilters { get; set; } = new[] { "master", "release" };
        public bool FilterGitReferences { get; set; } = true;
        public string[] TrimPatterns { get; set; } = null;
        public string BranchPrefix { get; set; } = "b-";
        public bool AlwaysApplyBranchPrefix{ get; set; } = false;
    }

    public class BuildNugetVersionFromBranchSemVer200Settings : BuildNugetVersionFromBranchSettings
    {
        public string Hash { get; set; }
    }
}
