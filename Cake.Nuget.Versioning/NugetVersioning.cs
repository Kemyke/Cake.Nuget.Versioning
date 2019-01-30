using Cake.Core;
using Cake.Core.Annotations;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Cake.Nuget.Versioning
{
    /// <summary>
    /// Contains functionality for creating Nuget compatible version numbers
    /// </summary>
    public static class NugetVersioningAliases
    {
        /// <summary>
        /// Create nuget compatible version from the parameters. 
        /// If branch names and other settings are provided it created a valid suffix for the version.
        /// You can't just use the current branch name because it can be too long or starts with numbers. 
        /// You can trim string from the branch name to create more readable versions.
        /// </summary>
        [CompilerGenerated]
        internal class NamespaceDoc
        {
        }

        private static string GetSuffixSeparator(bool isPrerelease = true)
        {
            if(isPrerelease)
            {
                return "-";
            }
            else
            {
                return "+";
            }
        }

        private static string ComposeVersion(int major, int minor, int patch, int? build = null)
        {
            if (build == null)
            {
                return $"{major}.{minor}.{patch}";
            }
            else
            {
                return $"{major}.{minor}.{patch}.{build}";
            }
        }

        private static string TrimBranch(string branch)
        {
            return branch.Replace("refs/heads/", "").Replace("refs/tags/", "").Replace("refs/remotes/", "");
        }

        private static bool IsBranchPrefixNecessary(string branch)
        {
            return !Regex.IsMatch(branch, "[A-Za-z]");
        }

        private static string ComposeSuffix(BuildNugetVersionFromBranchSettings settings)
        {
            string branch = GetTrimmedBranchName(settings);
            string suffix = string.Empty;

            if (settings.PreReleaseFilters != null && settings.PreReleaseFilters.All(f => !Regex.IsMatch(branch, f)))
            {
                if(IsBranchPrefixNecessary(branch) || settings.AlwaysApplyBranchPrefix)
                {
                    branch = $"{settings.BranchPrefix}{branch}";
                }

                suffix = $"{GetSuffixSeparator(true)}{branch}";
            }

            return NormalizeSuffix(suffix, 20);
        }

        private static int GetPatch(int patch, string trimmedBranch, BuildNugetVersionFromBranchSemVer200Settings settings)
        {
            if (settings.PreReleaseFilters != null && settings.PreReleaseFilters.Any(f => Regex.IsMatch(trimmedBranch, f)))
            {
                return patch + (settings.BranchChangeNumber ?? 0);
            }
            else
            {
                return patch;
            }
        }

        private static string GetTrimmedBranchName(BuildNugetVersionFromBranchSettings settings)
        {
            string branch = settings.BranchName;
            string suffix = string.Empty;

            if (settings.FilterGitReferences)
            {
                branch = TrimBranch(branch);
            }

            if (settings.TrimPatterns != null)
            {
                foreach (string trim in settings.TrimPatterns)
                {
                    branch = branch.Replace(trim, string.Empty);
                }
            }

            return branch;
        }

        private static string ComposeSuffixSemVer2(BuildNugetVersionFromBranchSemVer200Settings settings)
        {
            string branch = GetTrimmedBranchName(settings);
            string hash = settings.Hash;
            int? branchChangeNumber = settings.BranchChangeNumber;
            string suffix = string.Empty;

            branch = branch.Substring(0, Math.Min(branch.Length, 200));

            var separator = GetSuffixSeparator(settings.PreReleaseFilters != null && settings.PreReleaseFilters.All(f => !Regex.IsMatch(branch, f)));

            if (settings.PreReleaseFilters != null && settings.PreReleaseFilters.All(f => !Regex.IsMatch(branch, f)))
            {
                if (IsBranchPrefixNecessary(branch) || settings.AlwaysApplyBranchPrefix)
                {
                    branch = $"{settings.BranchPrefix}{branch}";
                }

                suffix = branch;
                
                if(branchChangeNumber != null)
                {
                    suffix += $".{branchChangeNumber}";
                }
                if (hash != null)
                {
                    suffix += $".{hash}";
                }
            }
            else
            {
                if (hash != null)
                {
                    suffix = hash;
                }
            }

            return string.IsNullOrEmpty(suffix) ? suffix : separator + NormalizeSuffixSemVer2(suffix, 255);
        }

        private static string NormalizeSuffix(string suffix, int maxLength)
        {
            var normalizedSuffix = Regex.Replace(suffix, "[^A-Za-z0-9]", "-");
            return normalizedSuffix.Substring(0, Math.Min(normalizedSuffix.Length, maxLength)).TrimEnd('-');
        }

        private static string NormalizeSuffixSemVer2(string suffix, int maxLength)
        {
            var normalizedSuffix = Regex.Replace(suffix, "[^A-Za-z0-9.]", "-");
            return normalizedSuffix.Substring(0, Math.Min(normalizedSuffix.Length, maxLength)).TrimEnd('-');
        }

        /// <summary>
        /// Build a nuget compatible version from a branch.
        /// </summary>
        /// <example>
        /// <code>
        /// var version = BuildNugetVersionFromBranch(1, 0, 0, 0, settings);
        /// </code>
        /// </example>
        /// <param name="context">The context.</param>
        /// <param name="major">The major part of the version (1st)</param>
        /// <param name="minor">The minor part of the version (2nd)</param>
        /// <param name="patch">The patch part of the version (3rd)</param>
        /// <param name="build">The build part of the version (4th)</param>
        /// <param name="settings">Settings for building the version suffix. At least BranchName should be filled. Otherwise ArgumentNullException is thrown.</param>
        /// <returns>The nuget compatible version</returns>
        [CakeMethodAlias]
        public static string BuildNugetVersionFromBranch(this ICakeContext context, int major, int minor, int patch, int build, BuildNugetVersionFromBranchSettings settings)
        {
            if(settings.BranchName == null)
            {
                throw new ArgumentNullException("BranchName cannot be null!");
            }

            string version = ComposeVersion(major, minor, patch, build);
            var suffix = ComposeSuffix(settings);
            return $"{version}{suffix}";
        }

        /// <summary>
        /// Build a nuget compatible version from a branch.
        /// </summary>
        /// <example>
        /// <code>
        /// var version = BuildNugetVersionFromBranch(1, 0, 0, 0, "feature/newfunction");
        /// </code>
        /// </example>
        /// <param name="context">The context.</param>
        /// <param name="major">The major part of the version (1st)</param>
        /// <param name="minor">The minor part of the version (2nd)</param>
        /// <param name="patch">The patch part of the version (3rd)</param>
        /// <param name="build">The build part of the version (4th)</param>
        /// <param name="branch">Branch name for creating the version suffix</param>
        /// <returns>The nuget compatible version</returns>
        [CakeMethodAlias]
        public static string BuildNugetVersionFromBranch(this ICakeContext context, int major, int minor, int patch, int build, string branch)
        {
            var settings = new BuildNugetVersionFromBranchSettings { BranchName = branch };
            return BuildNugetVersionFromBranch(context, major, minor, patch, build, settings);
        }

        /// <summary>
        /// Build a nuget compatible version from a branch.
        /// </summary>
        /// <example>
        /// <code>
        /// var version = BuildNugetVersionFromBranch(1, 0, 0, settings);
        /// </code>
        /// </example>
        /// <param name="context">The context.</param>
        /// <param name="major">The major part of the version (1st)</param>
        /// <param name="minor">The minor part of the version (2nd)</param>
        /// <param name="patch">The patch part of the version (3rd)</param>
        /// <param name="settings">Settings for building the version suffix. At least BranchName should be filled. Otherwise ArgumentNullException is thrown.</param>
        /// <returns>The nuget compatible version</returns>
        [CakeMethodAlias]
        public static string BuildNugetVersionFromBranch(this ICakeContext context, int major, int minor, int patch, BuildNugetVersionFromBranchSettings settings)
        {
            if (settings.BranchName == null)
            {
                throw new ArgumentNullException("BranchName cannot be null!");
            }

            string version = ComposeVersion(major, minor, patch);
            var suffix = ComposeSuffix(settings);
            return $"{version}{suffix}";
        }

        /// <summary>
        /// Build a nuget compatible version from a branch.
        /// </summary>
        /// <example>
        /// <code>
        /// var version = BuildNugetVersionFromBranch(1, 0, 0, "feature/newfunction");
        /// </code>
        /// </example>
        /// <param name="context">The context.</param>
        /// <param name="major">The major part of the version (1st)</param>
        /// <param name="minor">The minor part of the version (2nd)</param>
        /// <param name="patch">The patch part of the version (3rd)</param>
        /// <param name="branch">Branch name for creating the version suffix</param>
        /// <returns>The nuget compatible version</returns>
        [CakeMethodAlias]
        public static string BuildNugetVersionFromBranch(this ICakeContext context, int major, int minor, int patch, string branch)
        {
            var settings = new BuildNugetVersionFromBranchSettings { BranchName = branch };
            return BuildNugetVersionFromBranch(context, major, minor, patch, settings);
        }

        /// <summary>
        /// Build a Nuget 3.0 compatible version from a branch. (SemVer 2.0.0)
        /// </summary>
        /// <example>
        /// <code>
        /// var version = BuildNugetVersionFromBranchSemVer200(1, 0, 0, settings);
        /// </code>
        /// </example>
        /// <param name="context">The context.</param>
        /// <param name="major">The major part of the version (1st)</param>
        /// <param name="minor">The minor part of the version (2nd)</param>
        /// <param name="patch">The patch part of the version (3rd)</param>
        /// <param name="settings">Settings for building the version suffix. At least BranchName should be filled. Otherwise ArgumentNullException is thrown.</param>
        /// <returns>The nuget compatible version</returns>
        [CakeMethodAlias]
        public static string BuildNugetVersionFromBranchSemVer200(this ICakeContext context, int major, int minor, int patch, BuildNugetVersionFromBranchSemVer200Settings settings)
        {
            if (settings.BranchName == null)
            {
                throw new ArgumentNullException("BranchName cannot be null!");
            }

            patch = GetPatch(patch, GetTrimmedBranchName(settings), settings);
            string version = ComposeVersion(major, minor, patch);
            var suffix = ComposeSuffixSemVer2(settings);
            return $"{version}{suffix}";

        }

        /// <summary>
        /// Build a Nuget 3.0 compatible version from a branch. (SemVer 2.0.0)
        /// </summary>
        /// <example>
        /// <code>
        /// var version = BuildNugetVersionFromBranchSemVer200(1, 0, 0, "feature/newfunction");
        /// </code>
        /// </example>
        /// <param name="context">The context.</param>
        /// <param name="major">The major part of the version (1st)</param>
        /// <param name="minor">The minor part of the version (2nd)</param>
        /// <param name="patch">The patch part of the version (3rd)</param>
        /// <param name="branch">Branch name for creating the version suffix</param>
        /// <param name="hash">Hash of the last commit</param>
        /// <returns>The nuget compatible version</returns>
        [CakeMethodAlias]
        public static string BuildNugetVersionFromBranchSemVer200(this ICakeContext context, int major, int minor, int patch, string branch, string hash = null)
        {
            var settings = new BuildNugetVersionFromBranchSemVer200Settings { BranchName = branch, Hash = hash };
            return BuildNugetVersionFromBranchSemVer200(context, major, minor, patch, settings);
        }

        /// <summary>
        /// Build a nuget compatible version from a branch.
        /// </summary>
        /// <example>
        /// <code>
        /// var version = BuildNugetVersion(1, 0, 0, 0, "newfunction");
        /// </code>
        /// </example>
        /// <param name="context">The context.</param>
        /// <param name="major">The major part of the version (1st)</param>
        /// <param name="minor">The minor part of the version (2nd)</param>
        /// <param name="patch">The patch part of the version (3rd)</param>
        /// <param name="build">The build part of the version (4th)</param>
        /// <param name="suffix">The version suffix</param>
        /// <returns>The nuget compatible version</returns>
        [CakeMethodAlias]
        public static string BuildNugetVersion(this ICakeContext context, int major, int minor, int patch, int build, string suffix)
        {
            string version = ComposeVersion(major, minor, patch, build);
            string normalizedSuffix = NormalizeSuffix(suffix, 20);
            return $"{version}{GetSuffixSeparator()}{normalizedSuffix}";
        }

        /// <summary>
        /// Build a nuget compatible version from a branch.
        /// </summary>
        /// <example>
        /// <code>
        /// var version = BuildNugetVersion(1, 0, 0, "newfunction");
        /// </code>
        /// </example>
        /// <param name="context">The context.</param>
        /// <param name="major">The major part of the version (1st)</param>
        /// <param name="minor">The minor part of the version (2nd)</param>
        /// <param name="patch">The patch part of the version (3rd)</param>
        /// <param name="suffix">The version suffix</param>
        /// <returns>The nuget compatible version</returns>
        [CakeMethodAlias]
        public static string BuildNugetVersion(this ICakeContext context, int major, int minor, int patch, string suffix)
        {
            string version = ComposeVersion(major, minor, patch);
            string normalizedSuffix = NormalizeSuffix(suffix, 20);
            return $"{version}{GetSuffixSeparator()}{normalizedSuffix}";
        }

        /// <summary>
        /// Build a nuget compatible version from a branch.
        /// </summary>
        /// <example>
        /// <code>
        /// var version = BuildNugetSemVer200(1, 0, 0, "newfunction");
        /// </code>
        /// </example>
        /// <param name="context">The context.</param>
        /// <param name="major">The major part of the version (1st)</param>
        /// <param name="minor">The minor part of the version (2nd)</param>
        /// <param name="patch">The patch part of the version (3rd)</param>
        /// <param name="suffix">The version suffix</param>
        /// <param name="prerelease">The suffix indicates a pre-release or a final release version</param>
        /// <returns>The nuget compatible version</returns>
        [CakeMethodAlias]
        public static string BuildNugetSemVer200(this ICakeContext context, int major, int minor, int patch, string suffix, bool prerelease)
        {
            string version = ComposeVersion(major, minor, patch);
            string normalizedSuffix = NormalizeSuffix(suffix, 255);
            return $"{version}{GetSuffixSeparator(prerelease)}{normalizedSuffix}";
        }

    }
}