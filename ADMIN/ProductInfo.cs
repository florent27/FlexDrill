// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProductInfo.cs" company="KUKA Roboter GmbH">
//   Copyright (c) KUKA Roboter GmbH 2006 - 2018
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
[assembly: System.Reflection.AssemblyCompany(BuildInformation.Company)]
[assembly: System.Reflection.AssemblyCopyright(BuildInformation.Copyright)]
#if !NOPRODUCTINFO

[assembly: System.Reflection.AssemblyProduct(BuildInformation.ProductName)]
[assembly: System.Reflection.AssemblyInformationalVersion(BuildInformation.ProductVersion)]
#endif

internal static class BuildInformation
{
    internal const string ProductName = "FlexDrill";

    internal const string Company = "KUKA Systems Aerospace France";

    internal const string Copyright = "Copyright (c) KUKA Systems Aerospace France 2019";

    //internal const string BuildNumber = "999";

    //internal const string BuildString = "999";

    ///internal const string ActualVersion = "999.999.999";

    //internal const string BuildVersion = "V999.999.999_Build999";

    //internal const string BuildPrefix = "";

    internal const string BuildTime = "2000-01-01 00:00:01Z";

    internal const string ProductVersion = "1.0";

    internal const string BuildLabel = "FlexDrill_999.999.999.999";

    internal const bool IsOfficialBuild = false;

#if DEBUG
    internal const bool DebugDefined = true;
#else
   internal const bool DebugDefined = false;
#endif
}