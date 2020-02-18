Imports PingMap2.Engine

Module Module1

    Sub Main()
        Try
            Console.WriteLine("PingMap version 2 build " & Reflection.Assembly.GetCallingAssembly.GetName.Version.Build)
            Console.WriteLine("Local client (c) Marc Oliver Herdrich 2020" & vbNewLine)
            Logger.Instance.Verbosity = Logger.LogLevel.Debug
            Dim session As String = IO.File.ReadAllText(IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "session.name"))
            If session = "" Then Throw New Exception("No session name give.")
            Logger.Instance.Log(Logger.LogLevel.Debug, "Session name set: " & session)
            Using r As New Runtime(session, Environment.ProcessorCount * 2), db As New ReportDatabase
                Logger.Instance.Log(Logger.LogLevel.Info, "Starting a new runtime...")
                Logger.Instance.Log(Logger.LogLevel.Debug, $"{r.MaxThreadCount.ToString} threads available.")
                r.AddReportSink(db)
                r.StartRuntime()
                _LoopUntilExit()
                Logger.Instance.Log(Logger.LogLevel.Info, "Exiting runtime: " & session)
            End Using
        Catch ex As Exception
            Logger.Instance.Log(Logger.LogLevel.Fatal, "Exception: " & ex.Message & vbNewLine & ex.StackTrace)
        Finally
            Logger.Instance.Dispose()
            Console.WriteLine(vbNewLine & "Press [ANY] key to continue...")
            Console.ReadKey(False)
        End Try
    End Sub

    Private Sub _LoopUntilExit()
        Dim command As String
        Do
            command = Console.ReadLine
        Loop Until command = "exit"
    End Sub

End Module
