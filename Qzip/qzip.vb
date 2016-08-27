Imports System.IO
Imports System.IO.Compression

Module qzip

    'Setup input/output
    Dim DirectoryPathArgument As String = "--D="
    Dim _DirectoryPath As String = Nothing

    Dim OutputPathArgument As String = "--O="
    Dim _OutputPath As String = Nothing

    Dim ExtractArgument As String = "--X"
    Dim _IsExtractArchive As Boolean = False

    Dim IncludeBaseDirectoryArgument As String = "--B"
    Dim _IncludeBaseDirectory As Boolean = False

    Dim CompressionLevelArgumentOptimal As String = "--Best"
    Dim CompressionLevelArgumentFastest As String = "--Fast"
    Dim CompressionLevelArgumentNoCompression As String = "--Store"
    Dim _UseCompression As Boolean = False
    Dim _CompressionLevel As Integer = 2

    Dim OverwriteModeArgument As String = "--M"
    Dim ForceOverwriteModeArgument As String = "--F"
    Dim _OverwriteMode As Integer = 2
    Dim _ForceOverwriteMode As Boolean = False

    'Create an overwrite method we can enumerate through
    Public Enum OverwriteMethod
        Never      ' 0
        IfNewer    ' 1
        Always     ' 2
    End Enum

    Sub Main(ByVal sArgs() As String)

        Try

            'Set window title
            Console.Title = "QuickZip"

            'If no arguments passed shutdown.
            If sArgs.Length = 0 Then
                InvalidArguments("Were sorry <-no arguments passed-> this application requires valid arguments!") 'Just output sorry.
                Environment.ExitCode = 1
                Environment.Exit(1)
            Else  'We have some arguments 

                For Each mArgs As String In sArgs 'Proccess arguments

                    If mArgs.ToLower.StartsWith(DirectoryPathArgument.ToLower) Then
                        _DirectoryPath = mArgs.Remove(0, DirectoryPathArgument.Length)
                    End If

                    If mArgs.ToLower.StartsWith(OutputPathArgument.ToLower) Then
                        _OutputPath = mArgs.Remove(0, OutputPathArgument.Length)
                    End If

                    If mArgs.ToLower.StartsWith(ExtractArgument.ToLower) Then
                        _IsExtractArchive = True
                    End If

                    If _IsExtractArchive Then

                        If mArgs.ToLower().StartsWith(OverwriteModeArgument.ToLower()) Then
                            _OverwriteMode = Convert.ToInt32(mArgs.Remove(0, OverwriteModeArgument.Length))
                            'If argument given but without value just use always
                            If String.IsNullOrEmpty(Convert.ToString(_OverwriteMode)) Then
                                _OverwriteMode = 2
                            End If
                        End If

                        If mArgs.ToLower().StartsWith(ForceOverwriteModeArgument.ToLower()) Then
                            _ForceOverwriteMode = True
                        End If

                    End If

                        'Skip these if extracting
                        If Not _IsExtractArchive Then

                        If mArgs.ToLower.StartsWith(IncludeBaseDirectoryArgument.ToLower) Then
                            _IncludeBaseDirectory = True
                        End If

                        If mArgs.ToLower.StartsWith(CompressionLevelArgumentOptimal.ToLower) Then
                            _UseCompression = True
                            _CompressionLevel = 0
                        End If

                        If mArgs.ToLower.StartsWith(CompressionLevelArgumentFastest.ToLower) Then
                            _UseCompression = True
                            _CompressionLevel = 1
                        End If

                        If mArgs.ToLower.StartsWith(CompressionLevelArgumentNoCompression.ToLower) Then
                            _UseCompression = True
                            _CompressionLevel = 2
                        End If

                    End If
                Next

                'Check directory paths given
                If Not Directory.Exists(_DirectoryPath) And
                    Not Directory.Exists(_OutputPath) And
                    IsNothing(_DirectoryPath) Or _DirectoryPath = String.Empty And
                    IsNothing(_OutputPath) Or _OutputPath = String.Empty Then
                    InvalidArguments("Were sorry <-invalid arguments passed-> | path to folder or path to file not valid!") 'Just output sorry.
                    Environment.ExitCode = 1
                    Environment.Exit(1)
                End If

                'Skip these if extracting
                If Not _IsExtractArchive Then

                    If _UseCompression Then

                        Select Case _CompressionLevel
                            Case 0
                                Compress(_DirectoryPath, _OutputPath, CompressionLevel.Optimal).Wait()
                            Case 1
                                Compress(_DirectoryPath, _OutputPath, CompressionLevel.Fastest).Wait()
                            Case 2
                                Compress(_DirectoryPath, _OutputPath, CompressionLevel.NoCompression).Wait()
                        End Select

                    Else
                        Compress(_DirectoryPath, _OutputPath).Wait()
                    End If

                End If

                If _IsExtractArchive Then

                    Select Case _OverwriteMode
                        Case 0
                            Extract(_DirectoryPath, _OutputPath, OverwriteMethod.Never).Wait()
                        Case 1
                            Extract(_DirectoryPath, _OutputPath, OverwriteMethod.IfNewer).Wait()
                        Case 2
                            Extract(_DirectoryPath, _OutputPath, OverwriteMethod.Always).Wait()
                    End Select

                End If

            End If

        Catch This As Exception
            Console.WriteLine(This.StackTrace)
            Environment.ExitCode = 1
            Environment.Exit(1)
        End Try

    End Sub

    Private Async Function Compress(ByVal _Directory As String, ByVal _Output As String, Optional _Compression As CompressionLevel = 2) As Task

        Try

            'Check directory again.
            If Not Directory.Exists(_DirectoryPath) Then
                InvalidArguments("Were sorry <-invalid arguments passed-> | path to folder or path to file not valid!") 'Just output sorry.
                Environment.ExitCode = 1
                Environment.Exit(1)
            End If

            'Add file extension if not added.
            If Not _Output.Contains(".zip") Then
                _Output = _Output & ".zip"
            End If

            'Delete existing file.
            If File.Exists(_Output) Then
                Kill(_Output)
            End If

            Await Task.Run(Sub()
                               ZipFile.CreateFromDirectory(_Directory, _Output, _Compression, _IncludeBaseDirectory)
                           End Sub)

        Catch This As Exception
            Console.WriteLine(This.StackTrace)
            Environment.ExitCode = 1
            Environment.Exit(1)
        End Try

    End Function

    Private Async Function Extract(ByVal _Directory As String, ByVal _Output As String, ByVal _OverwriteMode As OverwriteMethod) As Task

        Try

            'Check for valid file extension
            If Not _Directory.Contains(".zip") Then
                SendMessageToConsole("Were sorry the file not a valid archive, Only [*.zip] is supported!") 'Just output sorry.
                Environment.ExitCode = 1
                Environment.Exit(1)
            End If

            If Not File.Exists(_Directory) Then
                SendMessageToConsole("Were sorry, No file was found, Please check your path and try again!") 'Just output sorry.
                Environment.ExitCode = 1
                Environment.Exit(1)
                Exit Function
            End If

            Using _Archive As ZipArchive = ZipFile.OpenRead(_Directory)

                For Each _Entry As ZipArchiveEntry In _Archive.Entries

                    Await Task.Run(Sub()
                                       Dim _EntryFullName = Path.Combine(_Output, _Entry.FullName)
                                       Dim _EntryPath = Path.GetDirectoryName(_EntryFullName)

                                       If (Not (Directory.Exists(_EntryPath))) Then
                                           Directory.CreateDirectory(_EntryPath)
                                       End If

                                       Dim _EntryFileName = Path.GetFileName(_EntryFullName)

                                       If (Not String.IsNullOrEmpty(_EntryFileName)) Then

                                           Dim _ShowConsoleOutput As Boolean = True

                                           Select Case _OverwriteMode
                                               Case OverwriteMethod.Never
                                                   If Not File.Exists(_EntryFullName) Then
                                                       _Entry.ExtractToFile(_EntryFullName)
                                                   End If
                                               Case OverwriteMethod.IfNewer
                                                   If Not File.Exists(_EntryFullName) Or File.GetLastWriteTime(_EntryFullName) < _Entry.LastWriteTime Then
                                                       _Entry.ExtractToFile(_EntryFullName, True)
                                                   End If
                                               Case OverwriteMethod.Always

                                                   If _ForceOverwriteMode Then
                                                       _Entry.ExtractToFile(_EntryFullName, True)
                                                   Else

                                                       Dim _StillRunning As Boolean = True

                                                       While _StillRunning

                                                           SendMessageToConsole(String.Format("Do you want to overwrite {0}: Yes (Y) | No (N) | All (A)", Path.GetFileName(_EntryFullName)))
                                                           Dim _UserInput As String = Console.ReadLine().ToLower

                                                           Select Case _UserInput
                                                               Case "y"
                                                                   _Entry.ExtractToFile(_EntryFullName, True)
                                                                   _StillRunning = False
                                                               Case "n" 'Skip file
                                                                   _ShowConsoleOutput = False
                                                                   _StillRunning = False
                                                               Case "a"
                                                                   _ForceOverwriteMode = True
                                                                   _Entry.ExtractToFile(_EntryFullName, True)
                                                                   _StillRunning = False
                                                               Case "exit" 'Just exit
                                                                   _ForceOverwriteMode = True
                                                                   Environment.ExitCode = 2
                                                                   Environment.Exit(2)
                                                               Case Else
                                                                   _ShowConsoleOutput = False
                                                                   SendMessageToConsole("[Invalid Option] -- Valid options are: Yes (Y) | No (N)| All (A) --")
                                                           End Select
                                                       End While

                                                   End If

                                           End Select
                                           If _ShowConsoleOutput Then
                                               SendMessageToConsole(String.Format("Extracting: {0}", _EntryFullName)) 'Show feedback
                                           End If
                                       End If
                                   End Sub)

                Next

            End Using

        Catch This As Exception
            Console.WriteLine(This.StackTrace)
            Environment.ExitCode = 1
            Environment.Exit(1)
        End Try

    End Function

    Private Sub SendMessageToConsole(ByVal _Message As String)
        Console.WriteLine(_Message.Replace("\", "/"))
    End Sub

    Private Sub InvalidArguments(Optional _Message As String = "")

        dim _SetMessage As string = Environment.NewLine &
                "Designed and developed by 8pecxstudios 2012-2016" & Environment.NewLine & Environment.NewLine &
                _Message & Environment.NewLine & Environment.NewLine &
                "For more information on a specific commands, See below" & Environment.NewLine & Environment.NewLine &
                "--D *Path to the folder you want the archive from." & Environment.NewLine &
                "--O *Path to output the generated archive. (.zip automatically added)" & Environment.NewLine &
                "--X Extracts a archive when used with --D and --O" & Environment.NewLine &
                "--M (0 = Never overwrite, 1 = Overwrite only if newer, 2 = Always overwrite [Default])" & Environment.NewLine &
                "--F Force overwrite mode 2 (Always overrite)" & Environment.NewLine &
                "--B Include base folder directory." & Environment.NewLine &
                "--Best Optimal possible compression level." & Environment.NewLine &
                "--Fast Fastest possible compression level." & Environment.NewLine &
                "--Store No compression." & Environment.NewLine &
                Environment.NewLine &
                "--D and --O must be specified." & Environment.NewLine &
                "--B [Optional]" & Environment.NewLine &
                "--Best [Optional]" & Environment.NewLine &
                "--Fast [Optional]" & Environment.NewLine &
                "--Store [Optional]" & Environment.NewLine &
                "--M(N) [Optional] --M0, --M1 or --M2" & Environment.NewLine &
                "--F [Optional]" & Environment.NewLine &
                Environment.NewLine &
                "For items marked with * are required template parameters all parameters must be set." & Environment.NewLine & Environment.NewLine &
                "For more information on tools see the command-line reference in the online help."

        SendMessageToConsole(_SetMessage)

    End Sub

End Module
