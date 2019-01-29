using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace FileRenamer
{

    class Program
    {
        static string currentDirectory = "";
        static string filepath = "";
        static DirectoryInfo directoryInfo = null;

        static void RenameFiles(DirectoryInfo directory)
        {
            foreach (FileInfo file in directory.GetFiles())
            {
                if (file.Name != "FileRenamer.exe")
                {
                    Console.WriteLine("\tProcessing file: {0}", file.Name);
                    List<string> components = file.Name.Split('.').ToList<string>();           

                    string episodeNumber = "";
                    string seasonNumber = "";
                    string identifier = "";
                    string name = "";
                    string extension = "";

                    extension = components[components.Count - 1];
                    if (extension == "config" || extension == "pdb")
                    {
                        continue;
                    }

                    seasonNumber = ParseEpisodeNumber(components[5]).Item1;
                    episodeNumber = ParseEpisodeNumber(components[5]).Item2;

                    identifier = seasonNumber + "x" + episodeNumber;

                    for (int i = 6; i < components.Count - 1; i++)
                    {
                        name += components[i];
                        if (i != components.Count - 2)
                        {
                            name += " ";
                        }
                    }

                    string newFilename = identifier + " - " + name + "." + extension;
                    file.MoveTo(newFilename);
                }
            }
        }

        static Tuple<string, string> ParseEpisodeNumber(string combined)
        {
            string episode = "";
            string season = "";

            int seasonSubstrModifier = 0;

            season = Regex.Match(combined, "(S).*(E)").ToString();
            episode = Regex.Match(combined, "(E).*").ToString();

            if (season[1] == '0')
            {
                seasonSubstrModifier = 1;
            }

            return new Tuple<string, string>(season.Substring(1 + seasonSubstrModifier, season.Length - (2 + seasonSubstrModifier)), episode.Substring(1));
        }

        static void Main(string[] args)
        {
            currentDirectory = Directory.GetCurrentDirectory();
            Console.WriteLine("Current directory is:\n{0}\n", currentDirectory);

            directoryInfo = new DirectoryInfo(currentDirectory);

            if (directoryInfo.GetDirectories().Count<DirectoryInfo>() > 0)
            {
                foreach (DirectoryInfo directory in directoryInfo.GetDirectories())
                {
                    Console.WriteLine("Processing directory:\t{0}", directory.Name);
                    RenameFiles(directory);
                    Console.WriteLine();
                }
            }

            Console.WriteLine("Processing directory:\t{0}", currentDirectory);
            RenameFiles(directoryInfo);

            Console.WriteLine("Done.");
            Console.ReadKey();
        }
    }
}
