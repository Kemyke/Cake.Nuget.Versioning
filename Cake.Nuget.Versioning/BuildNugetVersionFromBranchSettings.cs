using System;
using System.Collections.Generic;
using System.Text;

namespace Cake.Nuget.Versioning
{
    /// <summary>
    /// Settings how to convert branch name to version suffix
    /// </summary>
    public class BuildNugetVersionFromBranchSettings
    {
        /// <summary>
        /// The name of the branch
        /// </summary>
        public string BranchName { get; set; } = null;
        /// <summary>
        /// If the branch name match with one of the regexp in this array, the branch is
        /// identified as a final branch. The version won't contain pre-release suffix.
        /// </summary>
        public string[] PreReleaseFilters { get; set; } = new[] { "^master$", "^release/" };
        /// <summary>
        /// If true "refs/heads/", "refs/tags/" and "refs/remotes/" are trimmed from the beginning of the branch name.
        /// </summary>
        public bool FilterGitReferences { get; set; } = true;
        /// <summary>
        /// Regexp patterns which are trimmed off from the branch name.
        /// </summary>
        public string[] TrimPatterns { get; set; } = null;
        /// <summary>
        /// Branch prefix what is applied as a prefix before the branch name
        /// </summary>
        public string BranchPrefix { get; set; } = "b-";
        /// <summary>
        /// By default branch prefix is only applied when the branch name is not complatible as a version suffix. Eg. starts with numbers.
        /// If true branch prefix is always applied.
        /// </summary>
        public bool AlwaysApplyBranchPrefix{ get; set; } = false;
    }

    /// <summary>
    /// Settings how to convert branch name to SemVer 2.0.0 suffix
    /// </summary>
    public class BuildNugetVersionFromBranchSemVer200Settings : BuildNugetVersionFromBranchSettings
    {
        /// <summary>
        /// Hash of the last commit
        /// </summary>
        public string Hash { get; set; }
    }
}
