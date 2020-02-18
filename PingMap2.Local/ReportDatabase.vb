Imports PingMap2.Engine

Public Class ReportDatabase : Implements IReportSink, IDisposable

    Private ReadOnly _path As String
    Private _current_file As String
    Private _current_stream As IO.FileStream
    Private _is_disposed As Boolean

    Private Shared ReadOnly _MAGIC As Byte() = Text.Encoding.ASCII.GetBytes("PING")
    Private Shared ReadOnly _VERSION As Byte() = {1, 0}
    Private Shared ReadOnly _BUILD As Integer = Reflection.Assembly.GetCallingAssembly.GetName.Version.Build
    Private Shared ReadOnly _MAX_FILE_SIZE As Integer = 2 ^ 20

    Public Sub New()
        Me.New(AppDomain.CurrentDomain.BaseDirectory)
    End Sub

    Public Sub New(basePath As String)
        Me._path = basePath
        Me._MakeNewBinaryFile()
    End Sub

    Private Shared Function _GetNewFileName() As String
        Return String.Concat("ping_db_", Date.Now.ToString("yyyyMMddThhmmss"), ".bin")
    End Function

    Private Shared Function _CreateFile(fileName As String) As IO.FileStream
        If IO.File.Exists(fileName) Then Throw New IO.IOException("File does already exist.")
        Dim retval As New IO.FileStream(fileName, IO.FileMode.CreateNew, IO.FileAccess.Write, IO.FileShare.None)
        Using w As New IO.BinaryWriter(retval, Text.Encoding.ASCII, True)
            w.Write(_MAGIC, 0, 4)
            w.Write(_VERSION, 0, 2)
            w.Write(_BUILD)
        End Using
        Return retval
    End Function

    Private Sub _MakeNewBinaryFile()
        Me._current_file = IO.Path.Combine(Me._path, _GetNewFileName)
        Logger.Instance.Log(Logger.LogLevel.Info, "New binary data base: " & Me._current_file)
        Me._current_stream = _CreateFile(Me._current_file)
    End Sub

    Public Sub Register(report As Report) Implements IReportSink.Register
        SyncLock Me._current_stream
            Try
                Using w As New IO.BinaryWriter(Me._current_stream, Text.Encoding.UTF8, True)
                    w.Write(&HFF00)
                    report.ToBinStream(w)
                End Using
                Logger.Instance.Log(
                    Logger.LogLevel.Info,
                    "Report: " & Hex(report.Session.GetHashCode).PadLeft(8, "0"c) & " > " &
                    report.Ping.Replier.ToString.PadRight(15, " "c) & " > " &
                    report.Ping.RTT.ToString.PadLeft(4, "0"c) & " > " &
                    report.Location.CountryName
                )
            Catch ex As Exception
                Logger.Instance.Log(Logger.LogLevel.Critical, "Exception: " & ex.Message & vbNewLine & ex.StackTrace)
            End Try
            If Me._current_stream.Length >= _MAX_FILE_SIZE Then
                Logger.Instance.Log(Logger.LogLevel.Info, "Closing data base: " & Me._current_file)
                Me._current_stream.Close()
                Me._current_stream.Dispose()
                Me._MakeNewBinaryFile()
            End If
        End SyncLock
    End Sub

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not Me._is_disposed Then
            If disposing Then
                Me._current_stream.Close()
                Me._current_stream.Dispose()
            End If
        End If
        Me._is_disposed = True
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        Dispose(True)
    End Sub

End Class
