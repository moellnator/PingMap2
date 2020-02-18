Imports PingMap2.Engine

Module Module1

    Sub Main()
        Try
            Console.WriteLine("PingMap version 2 build " & Reflection.Assembly.GetCallingAssembly.GetName.Version.Build)
            Console.WriteLine("Local client (c) Marc Oliver Herdrich 2020" & vbNewLine)
            Console.WriteLine("Press [F4] to exit..." & vbNewLine)
            Logger.Instance.Verbosity = Setup.Instance.LogLevel
            Using r As New Runtime(Setup.Instance.SessionName, Setup.Instance.ThreadCount), db As New ReportDatabase
                Logger.Instance.Log(Logger.LogLevel.Info, "Starting a new runtime...")
                Logger.Instance.Log(Logger.LogLevel.Debug, $"Session name: {Setup.Instance.SessionName}")
                Logger.Instance.Log(Logger.LogLevel.Debug, $"{r.MaxThreadCount.ToString} threads available.")
                r.AddReportSink(db)
                r.StartRuntime()
                _LoopUntilExit()
                Logger.Instance.Log(Logger.LogLevel.Info, "Exiting runtime.")
            End Using
        Catch ex As Exception
            Logger.Instance.Log(Logger.LogLevel.Fatal, "Exception: " & ex.Message & vbNewLine & ex.StackTrace)
        Finally
            Logger.Instance.Dispose()
            Console.WriteLine(vbNewLine & "Press [ANY] key to continue...")
            Console.ReadKey(True)
        End Try
    End Sub

    Private Sub _LoopUntilExit()
        Dim command As ConsoleKeyInfo
        Do
            command = Console.ReadKey(True)
        Loop Until command.Key = ConsoleKey.F4
    End Sub

End Module
