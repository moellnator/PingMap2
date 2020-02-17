Imports PingMap2.Engine

Module Module1

    Sub Main()
        Console.Write("Enter session name: ")
        Dim session As String = Console.ReadLine
        Dim db As New ReportDatabase
        Logger.Instance.Log(Logger.LogLevel.Info, "Starting a new runtime: " & session)
        Using r As New Runtime(session, Environment.ProcessorCount * 2)
            r.AddReportSink(db)
            r.StartRuntime()
            Dim command As String
            Do
                command = Console.ReadLine
            Loop Until command = "exit"
            Logger.Instance.Log(Logger.LogLevel.Info, "Exiting runtime: " & session)
        End Using
        Logger.Instance.Dispose()
    End Sub

End Module
