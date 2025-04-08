using System.Diagnostics;
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Solid.Identity.Protocols.Saml2p;

public static class Tracing
{
    private static readonly string AssemblyVersion = GenerateAssemblyVersion();
    // ReSharper disable once InconsistentNaming
    public static ActivitySource Saml2p { get; } = new (Names.Saml2p, AssemblyVersion);
    public static ActivitySource Cache { get; } = new (Names.Cache, AssemblyVersion);
    public static ActivitySource Factories { get; } = new (Names.Factories, AssemblyVersion);
    public static ActivitySource Providers { get; } = new (Names.Providers, AssemblyVersion);
    public static ActivitySource Validation { get; } = new (Names.Validation, AssemblyVersion);
    public static class Names
    {
        public static readonly string[] All = new[]
        {
            Saml2p,
            Cache,
            Factories,
            Providers,
            Validation
        };
        
        public const string Saml2p = "Solid.Identity.Protocols.Saml2p";
        public const string Cache = Saml2p + ".Cache";
        public const string Factories = Saml2p + ".Factories";
        public const string Providers = Saml2p + ".Providers";
        public const string Validation = Saml2p + ".Validation";
    }

    private static string GenerateAssemblyVersion()
    {
        var version = typeof(Saml2pConstants).Assembly.GetName().Version;
        return version == null ? "0.0.0" : $"{version.Major}.{version.Minor}.{version.Build}";
    }
}