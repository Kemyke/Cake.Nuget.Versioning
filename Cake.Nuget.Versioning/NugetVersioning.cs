using Cake.Core;
using Cake.Core.Annotations;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Cake.Nuget.Versioning
{
    public static class NugetVersioning
    {
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

        private static string ComposeSuffixSemVer2(BuildNugetVersionFromBranchSemVer200Settings settings)
        {
            string branch = settings.BranchName;
            string hash = settings.Hash;
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

            if (settings.PreReleaseFilters != null && settings.PreReleaseFilters.All(f => !Regex.IsMatch(branch, f)))
            {
                if (IsBranchPrefixNecessary(branch) || settings.AlwaysApplyBranchPrefix)
                {
                    branch = $"{settings.BranchPrefix}{branch}";
                }

                if (hash == null)
                {
                    suffix = $"{GetSuffixSeparator(true)}{branch}";
                }
                else
                {
                    suffix = $"{GetSuffixSeparator(true)}{branch}.{hash}";
                }
            }
            else
            {
                if (hash != null)
                {
                    suffix = $"{GetSuffixSeparator(false)}{hash}";
                }
            }

            return NormalizeSuffixSemVer2(suffix, 255);
        }

        private static string NormalizeSuffix(string suffix, int maxLength)
        {
            var normalizedSuffix = Regex.Replace(suffix, "[^A-Za-z0-9]", "-");
            return normalizedSuffix.Substring(0, Math.Min(normalizedSuffix.Length, maxLength));
        }

        private static string NormalizeSuffixSemVer2(string suffix, int maxLength)
        {
            var normalizedSuffix = Regex.Replace(suffix, "[^A-Za-z0-9+.]", "-");
            return normalizedSuffix.Substring(0, Math.Min(normalizedSuffix.Length, maxLength));
        }


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

        [CakeMethodAlias]
        public static string BuildNugetVersionFromBranch(this ICakeContext context, int major, int minor, int patch, int build, string branch)
        {
            var settings = new BuildNugetVersionFromBranchSettings { BranchName = branch };
            return BuildNugetVersionFromBranch(context, major, minor, patch, build, settings);
        }

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

        [CakeMethodAlias]
        public static string BuildNugetVersionFromBranch(this ICakeContext context, int major, int minor, int patch, string branch)
        {
            var settings = new BuildNugetVersionFromBranchSettings { BranchName = branch };
            return BuildNugetVersionFromBranch(context, major, minor, patch, settings);
        }

        [CakeMethodAlias]
        public static string BuildNugetVersionFromBranchSemVer200(this ICakeContext context, int major, int minor, int patch, BuildNugetVersionFromBranchSemVer200Settings settings)
        {
            if (settings.BranchName == null)
            {
                throw new ArgumentNullException("BranchName cannot be null!");
            }

            string version = ComposeVersion(major, minor, patch);
            var suffix = ComposeSuffixSemVer2(settings);
            return $"{version}{suffix}";

        }

        [CakeMethodAlias]
        public static string BuildNugetVersionFromBranchSemVer200(this ICakeContext context, int major, int minor, int patch, string branch, string hash = null)
        {
            var settings = new BuildNugetVersionFromBranchSemVer200Settings { BranchName = branch, Hash = hash };
            return BuildNugetVersionFromBranchSemVer200(context, major, minor, patch, settings);
        }


        [CakeMethodAlias]
        public static string BuildNugetVersion(this ICakeContext context, int major, int minor, int patch, int build, string suffix)
        {
            string version = ComposeVersion(major, minor, patch, build);
            string normalizedSuffix = NormalizeSuffix(suffix, 20);
            return $"{version}{GetSuffixSeparator()}{normalizedSuffix}";
        }

        [CakeMethodAlias]
        public static string BuildNugetVersion(this ICakeContext context, int major, int minor, int patch, string suffix)
        {
            string version = ComposeVersion(major, minor, patch);
            string normalizedSuffix = NormalizeSuffix(suffix, 20);
            return $"{version}{GetSuffixSeparator()}{normalizedSuffix}";
        }

        [CakeMethodAlias]
        public static string BuildNugetSemVer200(this ICakeContext context, int major, int minor, int patch, string suffix, bool prerelease)
        {
            string version = ComposeVersion(major, minor, patch);
            string normalizedSuffix = NormalizeSuffix(suffix, 255);
            return $"{version}{GetSuffixSeparator(prerelease)}{normalizedSuffix}";
        }

    }
}