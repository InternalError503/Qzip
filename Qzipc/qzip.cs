using System;
using System.IO;
using System.IO.Compression;

namespace Qzip
{
    class Program
    {

        private static string DirectoryPathArgument = "--D=";
        private static string _DirectoryPath = null;

        private static string OutputPathArgument = "--O=";
        private static string _OutputPath = null;

        private static string ExtractArgument = "--X";
        private static bool _IsExtractArchive = false;

        private static string IncludeBaseDirectoryArgument = "--B";
        private static bool _IncludeBaseDirectory = false;

        private static string CompressionLevelArgumentOptimal = "--Best";
        private static string CompressionLevelArgumentFastest = "--Fast";
        private static string CompressionLevelArgumentNoCompression = "--Store";
        private static bool _UseCompression = false;
        private static int _CompressionLevel = 2;

        static void Main(string[] args)
        {
            try
            {
                // Set window title
                Console.Title = "QuickZip";

                // If no arguments passed shutdown.
                int getArgsLength = args.Length;
                if (getArgsLength == 0)
                {
                    InvalidArguments("Were sorry <-no arguments passed-> this application requires valid arguments!"); // Just output sorry.
                    Environment.ExitCode = 1;
                    Environment.Exit(1);
                }
                else // We have some arguments
                {
                    foreach (string mArgs in args)
                    {

                        if (mArgs.ToLower().StartsWith(DirectoryPathArgument.ToLower()))
                        {
                            _DirectoryPath = mArgs.Remove(0, DirectoryPathArgument.Length);
                        }

                        if (mArgs.ToLower().StartsWith(OutputPathArgument.ToLower()))
                        {
                            _OutputPath = mArgs.Remove(0, OutputPathArgument.Length);
                        }

                        if (mArgs.ToLower().StartsWith(ExtractArgument.ToLower()))
                        {
                            _IsExtractArchive = true;
                        }

                        if (!_IsExtractArchive)
                        {

                            if (mArgs.ToLower().StartsWith(IncludeBaseDirectoryArgument.ToLower()))
                            {
                                _IncludeBaseDirectory = true;
                            }

                            if (mArgs.ToLower().StartsWith(CompressionLevelArgumentOptimal.ToLower()))
                            {
                                _UseCompression = true;
                                _CompressionLevel = 0;
                            }

                            if (mArgs.ToLower().StartsWith(CompressionLevelArgumentFastest.ToLower()))
                            {
                                _UseCompression = true;
                                _CompressionLevel = 1;
                            }

                            if (mArgs.ToLower().StartsWith(CompressionLevelArgumentNoCompression.ToLower()))
                            {
                                _UseCompression = true;
                                _CompressionLevel = 2;
                            }

                        }

                    }

                    // Check directory paths given
                    if (!Directory.Exists(_DirectoryPath) && string.IsNullOrEmpty(_DirectoryPath) &&
                        !Directory.Exists(_OutputPath) && string.IsNullOrEmpty(_OutputPath))
                    {
                        InvalidArguments("Were sorry <-invalid arguments passed-> | path to folder or path to file not valid!"); // Just output sorry.
                        Environment.ExitCode = 1;
                        Environment.Exit(1);
                    }

                    // Skip these if extracting.
                    if (!_IsExtractArchive)
                    {

                        if (_UseCompression)
                        {
                            switch (_CompressionLevel)
                            {
                                case 0:
                                    Compress(_DirectoryPath, _OutputPath, CompressionLevel.Optimal);
                                    break;

                                case 1:
                                    Compress(_DirectoryPath, _OutputPath, CompressionLevel.Fastest);
                                    break;

                                case 2:
                                    Compress(_DirectoryPath, _OutputPath, CompressionLevel.NoCompression);
                                    break;

                            }
                        }

                    }

                    if (_IsExtractArchive)
                    {
                        Extract(_DirectoryPath, _OutputPath);
                    }

                }


        }
            catch (Exception This)
            {
                Console.WriteLine(This.StackTrace);
                Environment.ExitCode = 1;
                Environment.Exit(1);
            }

}

        static void Compress(string _Directory, string _Output, CompressionLevel _Compression)
        {
            try
            {
                // Check directory again.
                if (!Directory.Exists(_Directory))
                {
                    InvalidArguments("Were sorry <-invalid arguments passed-> | path to folder or path to file not valid!"); // Just output sorry.
                    Environment.ExitCode = 1;
                    Environment.Exit(1);
                }

                // Add file extension if not added
                if (!_Output.Contains(".zip"))
                {
                    _Output = _Output + ".zip";
                }

                // Delete existing file
                if (File.Exists(_Output))
                {
                    File.Delete(_Output);
                }

                ZipFile.CreateFromDirectory(_Directory, _Output, _Compression, _IncludeBaseDirectory);
  
            }
            catch (Exception This)
            {
                Console.WriteLine(This.StackTrace);
                Environment.ExitCode = 1;
                Environment.Exit(1);
            }
        }

        static void Extract(string _Directory, string _Output)
        {
            try
            {

                // Add file extension if not added
                if (!_Directory.Contains(".zip"))
                {
                    Console.WriteLine("Were sorry the file not a valid archive, Only [*.zip] is supported!"); // Just output sorry.
                    Environment.ExitCode = 1;
                    Environment.Exit(1);
                }

                if (!File.Exists(_Directory))
                {
                    Console.WriteLine("Were sorry, No file was found, Please check your path and try again!"); // Just output sorry.
                    Environment.ExitCode = 1;
                    Environment.Exit(1);
                }

                using (ZipArchive _Archive = ZipFile.OpenRead(_Directory))
                {

                    foreach (ZipArchiveEntry _Entry in _Archive.Entries)
                    {

                        string _EntryFullName = Path.Combine(_Output, _Entry.FullName);
                        string _EntryPath = Path.GetDirectoryName(_EntryFullName);

                        if (!Directory.Exists(_EntryPath))
                        {
                            Directory.CreateDirectory(_EntryPath);
                        }

                        string _EntryFileName = Path.GetFileName(_EntryFullName);

                        if (!string.IsNullOrEmpty(_EntryFileName))
                        {
                            _Entry.ExtractToFile(_EntryFullName, true);
                            Console.WriteLine(string.Format("Extracting: {0}", _EntryFullName)); // Show feedback
                        }

                    }

                }

        }


            catch (Exception This)
            {
                Console.WriteLine(This.StackTrace);
                Environment.ExitCode = 1;
                Environment.Exit(1);
            }
}

        static void InvalidArguments(string _Message = "")
        {

            string _SetMessage = Environment.NewLine +
            "Designed and developed by 8pecxstudios 2012-2016" + Environment.NewLine + Environment.NewLine +
            _Message + Environment.NewLine + Environment.NewLine +
            "For more information on a specific commands, See below" + Environment.NewLine + Environment.NewLine +
            "--D *Path to the folder you want the archive from." + Environment.NewLine +
            "--O *Path to output the generated archive. (.zip automatically added)" + Environment.NewLine +
            "--X Extracts a archive when used with --D and --O" + Environment.NewLine +
            "--B Include base folder directory." + Environment.NewLine +
            "--Best Optimal possible compression level." + Environment.NewLine +
            "--Fast Fastest possible compression level." + Environment.NewLine +
            "--Store No compression." + Environment.NewLine +
            Environment.NewLine +
            "--D and --O must be specified." + Environment.NewLine +
            "--B [Optional]" + Environment.NewLine +
            "--Best [Optional]" + Environment.NewLine +
            "--Fast [Optional]" + Environment.NewLine +
            "--Store [Optional]" + Environment.NewLine +
            Environment.NewLine +
            "For items marked with * are required template parameters all parameters must be set." + Environment.NewLine + Environment.NewLine +
            "For more information on tools see the command-line reference in the online help.";

            Console.WriteLine(_SetMessage);

        }

    }
}
