using System;
using System.IO;
using System.IO.Compression;

namespace Qzip
{
    class Program
    {

        private static string DirectoryPathArgument = "-D=";
        private static string _DirectoryPath = null;

        private static string OutputPathArgument = "-O=";
        private static string _OutputPath = null;

        private static string ExtractArgument = "-X";
        private static bool _IsExtractArchive = false;

        private static string IncludeBaseDirectoryArgument = "-B";
        private static bool _IncludeBaseDirectory = false;

        private static string CompressionLevelArgument = "-C";
        private static bool _UseCompression = false;
        private static int _CompressionLevel = 2;

        private static string OverwriteModeArgument = "-M";
        private static string ForceOverwriteModeArgument = "-F";
        private static int _OverwriteMode = 2;
        private static bool _ForceOverwriteMode = false;

        // Create an overwrite method we can enumerate through
        public enum OverwriteMethod
        {
            Never,      // 0
            IfNewer,    // 1
            Always      // 2
        }

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

                        if (_IsExtractArchive)
                        {

                            if (mArgs.ToLower().StartsWith(OverwriteModeArgument.ToLower()))
                            {
                                _OverwriteMode = Convert.ToInt32(mArgs.Remove(0, OverwriteModeArgument.Length));
                                // If argument given but without value just use always
                                if (string.IsNullOrEmpty(Convert.ToString(_OverwriteMode)))
                                {
                                    _OverwriteMode = 2;
                                }
                            }

                            if (mArgs.ToLower().StartsWith(ForceOverwriteModeArgument.ToLower()))
                            {
                                _ForceOverwriteMode = true;
                            }

                        }

                        if (!_IsExtractArchive)
                        {

                            if (mArgs.ToLower().StartsWith(IncludeBaseDirectoryArgument.ToLower()))
                            {
                                _IncludeBaseDirectory = true;
                            }

                            if (mArgs.ToLower().StartsWith(CompressionLevelArgument.ToLower()))
                            {
                                _CompressionLevel = Convert.ToInt32(mArgs.Remove(0, CompressionLevelArgument.Length));
                                // If argument given but without value just use always
                                if (string.IsNullOrEmpty(Convert.ToString(_CompressionLevel)))
                                {
                                    _CompressionLevel = 2;
                                }
                                _UseCompression = true;
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

                        switch (_OverwriteMode)
                        {

                            case 0:
                                Extract(_DirectoryPath, _OutputPath, OverwriteMethod.Never);
                                break;

                            case 1:
                                Extract(_DirectoryPath, _OutputPath, OverwriteMethod.IfNewer);
                                break;

                            case 2:
                                Extract(_DirectoryPath, _OutputPath, OverwriteMethod.Always);
                                break;

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

        static void Extract(string _Directory, string _Output, OverwriteMethod _OverwriteMode)
        {
            try
            {

                // Add file extension if not added
                if (!_Directory.Contains(".zip"))
                {
                    SendMessageToConsole("Were sorry the file not a valid archive, Only [*.zip] is supported!"); // Just output sorry.
                    Environment.ExitCode = 1;
                    Environment.Exit(1);
                }

                if (!File.Exists(_Directory))
                {
                    SendMessageToConsole("Were sorry, No file was found, Please check your path and try again!"); // Just output sorry.
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

                            bool _ShowConsoleOutput = true;

                            switch (_OverwriteMode)
                            {

                                case OverwriteMethod.Never:

                                    if (!File.Exists(_EntryFullName))
                                    {
                                        _Entry.ExtractToFile(_EntryFullName);
                                    }

                                    break;

                                case OverwriteMethod.IfNewer:

                                    if (!File.Exists(_EntryFullName) || File.GetLastWriteTime(_EntryFullName) < _Entry.LastWriteTime)
                                    {
                                        _Entry.ExtractToFile(_EntryFullName, true);
                                    }

                                    break;

                                case OverwriteMethod.Always:

                                    if (_ForceOverwriteMode)
                                    {
                                        _Entry.ExtractToFile(_EntryFullName, true);
                                    }
                                    else
                                    {
                                        bool _StillRunning = true;

                                        while (_StillRunning)
                                        {

                                            SendMessageToConsole(string.Format("Do you want to overwrite {0}: Yes (Y) | No (N) | All (A)", Path.GetFileName(_EntryFullName)));
                                            string _UserInput = Console.ReadLine().ToLower();

                                            switch (_UserInput)
                                            {
                                                case "y":
                                                    _Entry.ExtractToFile(_EntryFullName, true);
                                                    _StillRunning = false;
                                                    break;

                                                case "n": // Skip file
                                                    _ShowConsoleOutput = false;
                                                    _StillRunning = false;
                                                    break;

                                                case "a":
                                                    _ForceOverwriteMode = true;
                                                    _Entry.ExtractToFile(_EntryFullName, true);
                                                    _StillRunning = false;
                                                    break;

                                                case "exit": // Just exit
                                                    _ForceOverwriteMode = true;
                                                    Environment.ExitCode = 2;
                                                    Environment.Exit(2);
                                                    break;

                                                default:
                                                    _ShowConsoleOutput = false;
                                                    SendMessageToConsole("[Invalid Option] -- Valid options are: Yes (Y) | No (N)| All (A) --");
                                                    break;

                                            }

                                        }

                                    }

                                    break;

                            }
                            if (_ShowConsoleOutput)
                            {
                                SendMessageToConsole(string.Format("Extracting: {0}", _EntryFullName)); // Show feedback
                            }
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

        static void SendMessageToConsole(string _Message)
        {
            Console.WriteLine(_Message.Replace("\\", "/"));
        }

        static void InvalidArguments(string _Message = "")
        {

            string _SetMessage =
            "\nDesigned and developed by 8pecxstudios 2012-2016\n\n" +
            _Message + "\n\n" +
            "For more information on a specific commands, See below\n\n"  +
            "-D= *Path of the folder you want the archive from, `*`Path of archive you want to extract from.\n" +
            "-O= *Path to output generated archive (*.zip automatically added), `*`Path to output archive contents.\n" +
            "-X Extracts a archive when used with -D and -O\n" +
            "-M(N) (`0` = Never overwrite, `1` = Overwrite only if newer, `2` = Always overwrite [Default])\n" +
            "-F Force overwrite mode 2 (Always Overrite)\n" +
            "-B Include base folder directory.\n" +
            "-C(N) (`0` = Optimal possible compression, `1` = Fastest possible compression, `2` = No compression [Default])\n" +
            "-D and -O must be specified.\n" +
            "-B [Optional]\n" +
            "-C(N) [Optional] -C0, -C1, -C2\n" +
            "-M(N) [Optional] -M0, -M1, -M2\n" +
            "-F [Optional]\n\n" +
            "For items marked with * are required template parameters all parameters must be set.\n\n" +
            "For more information on tools see the command-line reference in the online help.";

            SendMessageToConsole(_SetMessage.Replace("\n", Environment.NewLine));

        }

    }
}
