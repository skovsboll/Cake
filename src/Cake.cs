using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using ConfigTransformationTool.Base;

namespace Cake {
	public class Cake {
		
		private readonly DirectoryInfo _root;
		private DirectoryInfo _bin;
		private DirectoryInfo _src;
		private DirectoryInfo _lib;

		private string _activeConfigurationName = "Debug";

		private List<ConfigurationGroup> _configurationGroups;
		private List<string> _configurations; 

		private readonly Dictionary<string, FileInfo[]> _sources = new Dictionary<string, FileInfo[]>();
		private readonly Dictionary<string, FileInfo[]> _resources = new Dictionary<string, FileInfo[]>();
		private readonly List<FileInfo> _content = new List<FileInfo>();

		private readonly List<FileInfo> _references = new List<FileInfo>();

		public Cake(string folder) {
			_root = FindNearestRoot(new DirectoryInfo(folder));
		}

		private DirectoryInfo FindNearestRoot(DirectoryInfo folder)
		{
			if (folder.Name == "src") return folder.Parent;
			if (folder.Parent == null)
			{
				throw new CakeException("You must be below a folder named 'src' to run Cake.");
			}

			return FindNearestRoot(folder.Parent);
		}

		private void EnsureSrcAndLibExists() {
			EnsureSubFolderExists("src");
			EnsureSubFolderExists("lib");
			EnsureSubFolderExists("bin");

			_bin = _root.GetDirectories("bin").First();
			_src = _root.GetDirectories("src").First();
			_lib = _root.GetDirectories("lib").First();
		}

		private void EnsureSubFolderExists(string subfolderName) {
			if (!_root.EnumerateDirectories(subfolderName).Any())
				_root.CreateSubdirectory(subfolderName);
		}

		public void Run(string configName)
		{
			_activeConfigurationName = configName;

			EnsureSrcAndLibExists();

			FindAndCakeSubProjects(_src);

			SortFiles(GatherFiles(_src));

			//GatherSourceFiles();
			//GatherResourceFiles();
			//GatherReferences();
			//GatherContentFiles();

			CleanOutput();
	
			EnumerateConfigurations();
			
			CompileCs();
			TransformConfigs();
			CopyContent();

			// TODO:
			//RunVbc();
			//RunFsc();
			//RunBooc();
			//MergeIl();

			// TODO  Support sub-projects
		}

		private void SortFiles(IEnumerable<FileInfo> files)
		{
			

		}

		private void FindAndCakeSubProjects(DirectoryInfo folder)
		{
			if (string.Equals(folder.Name, "src", StringComparison.CurrentCultureIgnoreCase))
			{
				new Cake(folder.FullName).Run(_activeConfigurationName);
			}
			else
			{
				folder.EnumerateDirectories("src").Each(FindAndCakeSubProjects);
			}
		}

		private IEnumerable<FileInfo> GatherFiles(DirectoryInfo folder)
		{
			if (string.Equals(folder.Name, "src", StringComparison.InvariantCultureIgnoreCase)) return new FileInfo[0];

			return folder.EnumerateFiles().Union(folder.EnumerateDirectories().SelectMany(GatherFiles));
		}

		private void EnumerateConfigurations()
		{
			_configurationGroups =
				(from c in _src.EnumerateFiles("*.config")
				 let parts = c.Name.Split('.')
				 let first = parts.First()
				 group c by first
				 into g
				 select new ConfigurationGroup(g)).ToList();

			_configurations = _configurationGroups.SelectMany(g => g.Configurations).Select(c => c.Name).Distinct().ToList();

			Console.Write("Available configurations: ");
			Console.WriteLine(string.Join(", ", _configurations.ToArray().DefaultIfEmpty("None")));
		}

		private class ConfigurationGroup
		{
			public readonly FileInfo BaseFile;
			public readonly IEnumerable<Configuration> Configurations;

			public ConfigurationGroup(IEnumerable<FileInfo> fileInfos)
			{
				var shortestFirst = 
					from f in fileInfos
					orderby f.Name.Length
					select f;

				BaseFile = shortestFirst.First();

				Configurations =
					from f in shortestFirst.Skip(1)
					select new Configuration(f);
			}

			internal class Configuration
			{
				public readonly FileInfo File;
				public readonly string Name;

				public Configuration(FileInfo fileInfo)
				{
					File = fileInfo;
					var parts = fileInfo.Name.Split('.');
					Name = string.Join(".", parts.Slice(1, -1).ToArray());
				}
			}
		}

		private void TransformConfigs()
		{
			foreach (var group in _configurationGroups)
			{
				ConfigurationGroup.Configuration targetConfiguration = @group.Configurations.FirstOrDefault(c => string.Compare(c.Name, _activeConfigurationName, StringComparison.CurrentCultureIgnoreCase) == 0);

				string destinationFilePath = Path.ChangeExtension(Path.Combine(_bin.FullName, _root.Name), ".exe.config");
				if (targetConfiguration != null)
				{
					var task = new TransformationTask(group.BaseFile.FullName, targetConfiguration.File.FullName);

					if (!task.Execute(destinationFilePath, false))
						throw new CakeException("Transformations failed: " + targetConfiguration.Name);					
				}
				else
				{
					group.BaseFile.CopyTo(destinationFilePath);
				}
			}
		}

		private void CopyContent()
		{
			foreach (var file in _content)
			{
				file.CopyTo(Path.Combine(_bin.FullName, file.Name));
			}
		}


		private void CleanOutput()
		{
			_bin.EnumerateFileSystemInfos().Each(fsi => fsi.Delete());
		}


		private void CompileCs() {
			FileInfo keyFile = _src.EnumerateFiles("*.snk").FirstOrDefault();

			string csFiles = string.Join(" ", _sources["*.cs"].Select(fi => "\"" + fi.FullName + "\"").ToArray());
			string resources = string.Join(" ", _resources.Values.SelectMany(fi => fi).Select(fi => "/resource:\"" + fi.FullName + "\"").ToArray());
			string output = "/out:\"" + _bin.FullName + "\\" + _root.Name + ".exe\"";
			string target = "/target:exe";
			string lib = "/lib:\"" + _lib.FullName + "\"";
			string references = string.Join(" ", _references.Select(r => "/reference:\"" + r.FullName + "\"").ToArray());

			var arguments = new List<string>(new[] { target, output, lib, references, resources, csFiles });

			if (string.Compare(_activeConfigurationName, "debug", StringComparison.CurrentCultureIgnoreCase) == 0)
			{
				arguments.Add("/debug");
				arguments.Add("/define:DEBUG");
				arguments.Add("/define:TRACE");
				arguments.Add("/define:CONTRACTS_FULL");
				arguments.Add("/pdb:\""+ Path.Combine(_bin.FullName, _root.Name) +".pdb\"");
			}
			else
			{
				arguments.Add("/optimize");
			}


			if (keyFile != null)
			{
				arguments.Add("/keyfile:\"" + keyFile.FullName + "\"");
			}

			const string command = @"C:\Windows\Microsoft.NET\Framework\v4.0.30319\csc.exe ";
			Console.WriteLine(string.Join(" ", arguments));
			var startInfo = new ProcessStartInfo(command, string.Join(" ", arguments)) { UseShellExecute = false, RedirectStandardError = true };
			Process.Start(startInfo).WaitForExit();
		}

		private void GatherReferences() {
			_references.AddRange(_lib.GetFiles("*.dll", SearchOption.AllDirectories));
			_references.AddRange(_lib.GetFiles("*.lnk", SearchOption.AllDirectories).Select(DeferenceLink));

			var gac = _lib.EnumerateFiles("global_references.txt").FirstOrDefault();
			if (gac != null)
			{
				_references.AddRange(
					File.ReadAllLines(gac.FullName)
					.Select(a => Assembly.ReflectionOnlyLoad(a).Location)
					.Select(p => new FileInfo(p)));
			}
		}

		private void GatherSourceFiles() {
			string[] extensions = new[] { "*.cs", "*.resx" };

			foreach (string extension in extensions)
				_sources.Add(extension, _src.GetFiles(extension, SearchOption.AllDirectories));
		}

		private void GatherResourceFiles()
		{
			string[] extensions = new[] { "*.xsd", "*.resx" };

			foreach (string extension in extensions)
				_resources.Add(extension, _src.GetFiles(extension, SearchOption.AllDirectories));
		}

		private void GatherContentFiles()
		{
			string[] extensions = new[] { "*.html" };

			foreach (var file in extensions.SelectMany(ext => _src.GetFiles(ext, SearchOption.AllDirectories)))
				_content.Add(file);
		}

		private FileInfo DeferenceLink(FileInfo lnkFile) {
			using(var link = new ShellLink(lnkFile.FullName)) {
				return new FileInfo(link.Target);
			}
		}

		public static void Main(string[] args) {
			try
			{
				new Cake(Path.GetFullPath(".")).Run(args.Take(1).DefaultIfEmpty("DEBUG").First());		

				Console.WriteLine("Done!");

			}
			catch (CakeException exception)
			{
				Console.WriteLine("Fatal error: " + exception.Message);
			}
			Console.ReadLine();
		}
	}

	internal class CakeException : Exception
	{
		public CakeException(string message) : base(message)
		{
		}
	}
}