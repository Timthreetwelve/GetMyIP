// Copyright(c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.
//
// This file is generated during the pre-build event by GenBuildInfo.ps1.
// Any edits to this file will be overwritten during the next build!

namespace DFWatch
{
    /// <summary>Class for information about the current build.</summary>
    public static class BuildInfo
    {
        public const string CommitIDString = "e9a90d6";

        public const string CommitIDFullString = "e9a90d6cd7d3b103b88218d302b0360635bc67ed";

        public const string VersionString = "0.4.5.0";

        public const string BuildDateString = "12/14/2022 18:03:12";

        public static readonly DateTime BuildDateUtc = DateTime.SpecifyKind(DateTime.Parse(BuildDateString), DateTimeKind.Utc);

        public static readonly DateTime BuildDateLocal = BuildDateUtc.ToLocalTime();
    }
}
