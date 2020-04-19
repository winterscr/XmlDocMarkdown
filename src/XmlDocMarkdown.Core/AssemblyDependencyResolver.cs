using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;

namespace XmlDocMarkdown.Core
{
	public class AssemblyDependencyResolver
	{
		private readonly FrameworkName _targetFramework;

		public AssemblyDependencyResolver(Assembly sourceAssembly)
		{
			var frameworkName = sourceAssembly.GetCustomAttribute<TargetFrameworkAttribute>()?.FrameworkName;

			if (frameworkName != null)
				_targetFramework = new FrameworkName(frameworkName);
		}

		public Assembly TryResolve(string name)
		{
			var profileFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
			var packagesFolder = Path.Combine(profileFolder, ".nuget", "packages");
			var requestedAssembly = ParseAssemblyName(name);
			var assemblyPackageFolder = Path.Combine(packagesFolder, requestedAssembly.Name);

			if (!Directory.Exists(assemblyPackageFolder))
			{
				return null;
			}

			var versionFolderNames = Directory.GetDirectories(assemblyPackageFolder);
			var versionFolderName = GetMatchingFolderVersion(versionFolderNames, requestedAssembly.Version);
			var packageLibsFolder = Path.Combine(assemblyPackageFolder, versionFolderName, "lib");

			var frameworkFolders = Directory.GetDirectories(packageLibsFolder);
			var frameworkFolder = GetMatchingFrameworkFolder(frameworkFolders, _targetFramework.Identifier);

			// TODO: check for different file extensions and handle case-sensitive file systems
			var requestedAssemblyFileName = requestedAssembly.Name + ".dll";
			var assemblyPath = Path.Combine(packageLibsFolder, frameworkFolder, requestedAssemblyFileName);

			if (File.Exists(assemblyPath))
			{
				return Assembly.LoadFile(assemblyPath);
			}

			return null;
		}

		private (string Name, Version Version) ParseAssemblyName(string name)
		{
			var sections = name.Split(',');

			if (sections.Length > 1)
			{
				var properties = sections.Skip(1).Select(i => i.Split('=')).ToDictionary(i => i[0], i => i[1], StringComparer.OrdinalIgnoreCase);

				if (properties.ContainsKey("version") && Version.TryParse(properties["version"], out var version))
					return (sections[0], version);
			}

			return (sections[0], null);
		}

		private string GetMatchingFrameworkFolder(string[] frameworkFolders, string targetFrameworkIdentifier)
		{
			// TODO: map this from the target framework identifier using fallback rules
			return "netstandard2.0";
		}

		private string GetMatchingFolderVersion(string[] versionFolderNames, Version targetFrameworkVersion)
		{
			// TODO: map this from the requested version using fallback rules
			return "2.4.7";
		}
	}
}
