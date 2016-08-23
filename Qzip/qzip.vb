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

    Sub Main(ByVal sArgs() As String)

        Try

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
                    Extract(_DirectoryPath, _OutputPath).Wait()
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

    Private Async Function Extract(ByVal _Directory As String, ByVal _Output As String) As Task

        Try

            'Check for valid file extension
            If Not _Directory.Contains(".zip") Then
                Console.WriteLine("Were sorry the file not a valid archive, Only [*.zip] is supported!") 'Just output sorry.
                Environment.ExitCode = 1
                Environment.Exit(1)
            End If

            If Not File.Exists(_Directory) Then
                Console.WriteLine("Were sorry, No file was found, Please check your path and try again!") 'Just output sorry.
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
                                           _Entry.ExtractToFile(_EntryFullName, True)
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

    Private Sub InvalidArguments(Optional _Message As String = Nothing)

        If IsNothing(_Message) Then
            _Message = ""
        End If

        Console.WriteLine(Environment.NewLine &
                "Designed and developed by 8pecxstudios 2012-2016" & Environment.NewLine & Environment.NewLine &
                _Message & Environment.NewLine & Environment.NewLine &
                "For more information on a specific commands, See below" & Environment.NewLine & Environment.NewLine &
                "--D *Path to the folder you want the archive from." & Environment.NewLine &
                "--O *Path to output the generated archive. (.zip automatically added)" & Environment.NewLine &
                "--X Extracts a archive when used with --D and --O" & Environment.NewLine &
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
                Environment.NewLine &
                "For items marked with * are required template parameters all parameters must be set." & Environment.NewLine & Environment.NewLine &
                "For more information on tools see the command-line reference in the online help.")

    End Sub

End Module
