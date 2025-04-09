using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Solid.Testing.AspNetCore.Abstractions.Providers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Solid.Testing.AspNetCore.Abstractions.Factories
{
    /// <summary>
    /// Abstract asp net core web host factory
    /// </summary>
    public abstract class WebHostFactory : IWebHostFactory
    {
        /// <summary>
        /// The web host options provider
        /// </summary>
        protected IWebHostOptionsProvider Provider { get; }

        protected WebHostFactory(IWebHostOptionsProvider provider)
        {
            Provider = provider;
        }

        /// <summary>
        /// Create an asp net core web host using a startup class and a hostname
        /// </summary>
        /// <param name="startup">The startup class type</param>
        /// <param name="hostname">The host name</param>
        /// <returns>An asp net core web host</returns>
        public virtual IWebHost CreateWebHost(Type startup, string hostname)
        {
            var builder = InitializeWebHostBuilder(startup, hostname);
            builder.UseConfiguration(GenerateApplicationConfiguration(new ConfigurationBuilder()));
            Provider.Configure(builder);
            return builder.Start();
        }
        
        /// <summary>
        /// Initialized an asp net core web host builder for a startup class and a hostname
        /// </summary>
        /// <param name="startup">The startup class type</param>
        /// <param name="hostname">The host name</param>
        /// <returns>An asp net core web host</returns>
        protected abstract IWebHostBuilder InitializeWebHostBuilder(Type startup, string hostname);

        /// <summary>
        /// Generates the application configuration
        /// </summary>
        /// <param name="builder">A configuration builder</param>
        /// <returns>The application configuration for the web host</returns>
        protected virtual IConfiguration GenerateApplicationConfiguration(IConfigurationBuilder builder)
        {
            foreach (var path in GetAppSettingsPaths())
                builder.AddJsonFile(path);
            return builder.Build();
        }

        /// <summary>
        /// Gets the path to all relative appsettings.json files 
        /// </summary>
        /// <returns>An enumerable of appsettings.json paths</returns>
        protected virtual IEnumerable<string> GetAppSettingsPaths()
        {
            var current = AppDomain.CurrentDomain.BaseDirectory;
            var settings = new[]
            {
                Path.Combine(current, "appsettings.json")
            }
            .Concat(GetProjectFilePaths()
                .Distinct()
                .Select(p => Path.GetDirectoryName(p))
                .Select(p => Path.Combine(p, "appsettings.json"))
            )
            .Where(p => File.Exists(p))
            .ToArray();

            return settings;
        }

        /// <summary>
        /// Gets the path to all relative csproj files
        /// </summary>
        /// <returns>An enumerable of csproj paths</returns>
        protected virtual IEnumerable<string> GetProjectFilePaths()
        {
            var path = AppDomain.CurrentDomain.BaseDirectory;
            var directory = new DirectoryInfo(path);
            while (!ContainsProject(directory.FullName))
            {
                if (directory.Parent == directory.Root) break;
                directory = directory.Parent;
            }

            var projects = Directory.EnumerateFiles(directory.FullName, "*.csproj");
            foreach (var project in projects)
            {
                foreach (var reference in GetProjectReferences(project))
                    yield return reference;
                yield return project;
            }
        }

        /// <summary>
        /// Reads references from a project file
        /// <para>This only supports the new project types (vs2017+)</para>
        /// </summary>
        /// <param name="project">The path to a csproj project file</param>
        /// <returns>Gets an enumerable of the project references from the csproj file</returns>
        protected virtual IEnumerable<string> GetProjectReferences(string project)
        {
            using (var file = File.OpenRead(project))
            {
                var doc = XDocument.Load(file);
                var references = doc.Descendants("ProjectReference");
                foreach (var reference in references)
                {
                    var path = reference.Attribute("Include").Value;
                    path = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(project), path));
                    yield return path;
                }
            }

            yield break;
        }

        /// <summary>
        /// Recursively checks whether a path contains an csproj files
        /// </summary>
        /// <param name="path">The path to check</param>
        /// <returns>A boolean value indicating whether a csproj file was found</returns>
        protected virtual bool ContainsProject(string path) =>
            Directory.EnumerateFiles(path, "*.csproj").Any();
    }
}
