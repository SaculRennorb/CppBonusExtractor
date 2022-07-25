
using System.Diagnostics;
using System.IO.Compression;
using System.Text.RegularExpressions;

if(args.Length > 3) {
	Console.WriteLine("Too many arguments. expected [<exercise_name> [<zip_file> [<forced_files_dir>]]]");
	return;
}

Console.WriteLine("+=================================+");
Console.WriteLine("|   Rennorbs Exercise Extractor   |");
Console.WriteLine("+=================================+");
Console.WriteLine();
Console.WriteLine("This tool is designed to extract and prepare an archive obtained from a moodle exercise when selecting 'show all solutions' on the exercise and then selecting and downloading a set of solutions with the option 'place in separate folders' checked.");
Console.WriteLine();
Console.WriteLine();

string exercise_name;
if(args.Length >= 1) {
	exercise_name = args[0];
} else {
	int hightest_starting_number = 0;
	foreach(var subdirectory in Directory.EnumerateDirectories("."))
	{
		var match = Regex.Match(Path.GetFileName(subdirectory), @"^\d+");
		if(match.Success) {
			var number = int.Parse(match.Groups[0].Value);
			if(number > hightest_starting_number)
				hightest_starting_number = number;
		}
	}

	if(hightest_starting_number > 0)
	{
		Console.WriteLine($"Since there are subdirectories in here that start with a number im assuming the next number is the next exercise number, which would be {hightest_starting_number + 1}.");
		exercise_name = (hightest_starting_number + 1).ToString();
	}
	else
	{
		Console.Write("Exercise number wasn't supplied via command line, please tell me wich exercise i should extract (e.g. '5' or '1_basics'): ");
		exercise_name = Console.ReadLine()!;
	}
}

string main_zip = null!;
find_zip:
if(main_zip == null && args.Length >= 2) {
	main_zip = args[1];
} else {
	var files_in_dir = Directory.EnumerateFiles(".", "*.zip").ToArray();
	switch(files_in_dir.Length)
	{
		case 0:
			Console.Write("No zip file was supplied via the command line and there arent any in this directory. Please tell me the path to the downloaded archive form moodle: ");
			main_zip = Console.ReadLine()!;
			break;

		case 1:
			Console.WriteLine($"No zip file was provided via the command line, but there is a singular zip file in the  directory. Assuming you want to use this file ({Path.GetFileName(files_in_dir[0])}).");
			main_zip = files_in_dir[0];
			break;

		default:
			Console.WriteLine("No zip file was provided via the command line, and there are multiple zip files in the directory. Please select a file from the list below (by supplying hte number) or provide a path to a file: ");
			for(int i = 0; i < files_in_dir.Length; i++)
			{
				Console.WriteLine($"[{i}] {files_in_dir[i]}");
			}
			read_path:
			main_zip = Console.ReadLine()!;
			if(int.TryParse(main_zip, out var number))
			{
				if(number < 0 || number >= files_in_dir.Length)
				{
					Console.WriteLine($"unfortuantely {number} isnt a valid index. Try again with a value beteen 0 and {files_in_dir.Length - 1} inclusive, or provide a path: ");
					goto read_path;
				}
				main_zip = files_in_dir[number];
			}
			break;
	}
}

if(!File.Exists(main_zip)) {
	Console.WriteLine($"Zip file '{main_zip}' doesnt actually exist. Try again!");
	goto find_zip;
}

string forced_files_dir = "forced_"+exercise_name;
if(args.Length == 3) forced_files_dir = args[2];
bool forced_has_makefile = false;
bool has_forced_files;
if(has_forced_files = Directory.Exists(forced_files_dir)) {
	Console.WriteLine($"there is a directory called {forced_files_dir}. I will copy and overwrite those files into each groups directory.");
	foreach(var file in Directory.EnumerateFiles(forced_files_dir))
		if(is_makefile(file))
			{ forced_has_makefile = true; break; }
} else {
	Console.WriteLine($"There is no directory called {forced_files_dir}. no forced files for this exercise.");
}


if(int.TryParse(exercise_name, out _)) {
	var match = Regex.Match(Path.GetFileName(main_zip), @"\)-(.+)-\d+.zip");
	if(match.Success)
	{
		Console.WriteLine($"Since the exercice name is just a number I tried to parse the name from the name of the zip file, did i get it right :)?   name: '{match.Groups[1].Value}'   just rename the dir later if i was wrong.");
		exercise_name += $"_{match.Groups[1].Value.Replace(' ', '_')}";
	}
	else
	{
		Console.WriteLine("Since the exercice name is just a number I tried to parse the name from the name of the zip file, but couldnt figure it out. sory. continuing with a numeric name.");
	}
}

Directory.CreateDirectory(exercise_name);

var usual_extensions = new[] { ".c", ".h", ".cc", ".cpp", ".hpp" };

bool has_usual_ext(string filename)
{
	var usual = false;
	foreach(var ext in usual_extensions)
		if(filename.EndsWith(ext))
		{ usual = true; break; }
	return usual;
}

bool is_makefile(string filename) => filename.EndsWith("Makefile") || filename.EndsWith("CMakeLists.txt");

var single_file_groups_makefile_indicator = new Dictionary<string, bool>();

Console.WriteLine();
Console.WriteLine();

using(var zip = ZipFile.OpenRead(main_zip))
	foreach(var entry in zip.Entries)
	{
		var og_group_name = Path.GetDirectoryName(entry.FullName);
		var group_name = og_group_name;
		Debug.Assert(group_name != null);
		var match = Regex.Match(group_name, @"Gruppe (\d+)");
		if(match.Success) group_name = "Gruppe "+match.Groups[1].Value.PadLeft(3, '0');

		Directory.CreateDirectory(Path.Combine(exercise_name, group_name));

		if(entry.Name.EndsWith(".zip"))
		{
			Console.WriteLine($"'{group_name}' supplied zip, i will extract that for you...");
			using(var group_zip = new ZipArchive(entry.Open(), ZipArchiveMode.Read))
			{
				//expect to have the structure
				// - src/...
				// - Makefile

				var ignored_dirs = new HashSet<string>(2);
				bool slash_build_notice = false;
				bool has_makefile = false;

				foreach(var inner_entry in group_zip.Entries)
				{
					if(string.IsNullOrWhiteSpace(inner_entry.Name)) continue;

					if(ignored_dirs.Contains(Path.GetDirectoryName(inner_entry.FullName)!)) continue;
					if(inner_entry.FullName.Contains("build/") || inner_entry.FullName.Contains("__MACOSX/"))
					{
						if(!slash_build_notice)
							Console.WriteLine($"\tThey included a build directory, PUNISH!");
						slash_build_notice = true;
						continue;
					}

					if(is_makefile(inner_entry.Name)) {
						inner_entry.ExtractToFile(Path.Combine(exercise_name, group_name, inner_entry.Name));
						has_makefile = true;
						continue;
					}
					
					var src_index = inner_entry.FullName.IndexOf("src");
					if(src_index < 0) {
						
						var dir_name = Path.GetDirectoryName(inner_entry.FullName);

						if((dir_name == "." || dir_name == "") && has_usual_ext(inner_entry.Name))
						{
							Console.WriteLine($"\tFound presumed source file '{inner_entry.Name}' in root dir, guessing it should be in src/");
							Directory.CreateDirectory(Path.Combine(exercise_name, group_name, "src"));
							inner_entry.ExtractToFile(Path.Combine(exercise_name, group_name, "src", inner_entry.Name));
						}
						else if(!(dir_name == "." || dir_name == ""))
						{
							Console.WriteLine($"\tZip entry '{inner_entry.FullName}' is not inside src directory, ignoring files directly in '{Path.GetDirectoryName(inner_entry.FullName)}' from now on.");
							ignored_dirs.Add(dir_name!);
						}
						continue;
					}

					if(!has_usual_ext(inner_entry.Name))
					{
						Console.WriteLine($"\tThey supplied an unusual file '{inner_entry.FullName}', ill just not extract that. ");
						continue;
					}

					var prepared_slug = inner_entry.FullName[src_index..];
					var target_path = Path.Combine(exercise_name, group_name, prepared_slug);
					Directory.CreateDirectory(Path.GetDirectoryName(target_path)!);

					inner_entry.ExtractToFile(target_path);
				}

				if(has_forced_files) {
					CopyDirectoryRecursive(forced_files_dir, Path.Combine(exercise_name, group_name));
					has_makefile |= forced_has_makefile;
				}

				if(!has_makefile){
					Console.WriteLine($"\tThey did not provide a makefile, you need to provide one yourself. may i suggest you provide one in {forced_files_dir} next time?");
				}
			}
		}
		else
		{
			if(string.IsNullOrWhiteSpace(entry.Name)) continue;

			if(!single_file_groups_makefile_indicator.ContainsKey(group_name))
				single_file_groups_makefile_indicator.Add(group_name, false);

			if(is_makefile(entry.Name))
			{
				entry.ExtractToFile(Path.Combine(exercise_name, group_name, entry.Name));
				single_file_groups_makefile_indicator[group_name] = true;
				continue;
			}

			if(!has_usual_ext(entry.Name))
			{
				Console.WriteLine($"'{group_name}' supplied an unusual file '{entry.Name}', ill just not extract that. ");
				continue;
			}

			entry.ExtractToFile(Path.Combine(exercise_name, group_name, "src", entry.Name));
		}
	}

foreach(var (group_name, had_makefile) in single_file_groups_makefile_indicator)
{
	Console.WriteLine($"'{group_name}' supplied single files...");

	var has_makefile = had_makefile;
	if(has_forced_files) {
		CopyDirectoryRecursive(forced_files_dir, Path.Combine(exercise_name, group_name));
		has_makefile |= forced_has_makefile;
	}

	if(!has_makefile) {
		Console.WriteLine($"\t They did not provide a makefile, you need to provide one yourself. may i suggest you provide one in {forced_files_dir} next time?");
	}
}


void CopyDirectoryRecursive(string src, string dst)
{
	foreach(var entry in new DirectoryInfo(src).GetFileSystemInfos())
	{
		if(entry.Attributes.HasFlag(FileAttributes.Directory))
		{
			var new_dst = Path.Combine(src, entry.Name);
			Directory.CreateDirectory(new_dst);
			CopyDirectoryRecursive(Path.Combine(src, entry.Name), new_dst);
		}
		else
		{
			var dst_name = Path.Combine(dst, entry.Name);
			if(is_makefile(entry.Name) && File.Exists(dst_name)) {
				Console.WriteLine($"\tI'm not gonna overwrite dst file '{entry.Name}' since its a makefile and we allow groups to supply custom ones.");
				continue;
			}
			File.Copy(entry.FullName, dst_name, true);
		}
	}
}