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
        /// Default value: new[] { "^master$" }
        /// </summary>
        public string[] PreReleaseFilters { get; set; } = new[] { "^master$" };
        /// <summary>
        /// If true "refs/heads/", "refs/tags/" and "refs/remotes/" are trimmed from the beginning of the branch name.
        /// Default value: true
        /// </summary>
        public bool FilterGitReferences { get; set; } = true;
        /// <summary>
        /// Regexp patterns which are trimmed off from the branch name.
        /// Default value: null
        /// </summary>
        public string[] TrimPatterns { get; set; } = null;
        /// <summary>
        /// Branch prefix what is applied as a prefix before the branch name
        /// Default value: b-
        /// </summary>
        public string BranchPrefix { get; set; } = "b-";
        /// <summary>
        /// By default branch prefix is only applied when the branch name is not complatible as a version suffix. Eg. starts with numbers.
        /// If true branch prefix is always applied.
        /// Default value: false
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
        /// <summary>
        /// Ordered commit number
        /// </summary>
        public int? BranchChangeNumber { get; set; } = null;
    }
}
