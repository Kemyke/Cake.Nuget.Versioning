# Cake.Nuget.Versioning

## Summary

Contains functionality for creating Nuget compatible version numbers.
Create nuget compatible version from the parameters. If branch names and other settings are provided it created a valid suffix for the version.
You can't just use the current branch name because it can be too long or starts with numbers. You can trim string from the branch name to create more readable versions.

## Usage

```
#addin "nuget:?package=Cake.Nuget.Versioning"


var fullVersion = BuildNugetVersionFromBranch(major, minor, patch, new BuildNugetVersionFromBranchSettings { BranchName = gitBranch, TrimPatterns = new[] { "feature/" }});
```