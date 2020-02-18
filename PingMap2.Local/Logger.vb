Public Class Logger : Implements IDisposable

    Public Enum LogLevel
        Fatal = 0
        Critical
        Warning
        Info
        Debug
    End Enum

    Private Shared ReadOnly _COLORS As ConsoleColor() = {
        ConsoleColor.Red,
        ConsoleColor.Red,
        ConsoleColor.Yellow,
        ConsoleColor.Green,
        ConsoleColor.White
    }

    Private Shared ReadOnly _SYMBOLS As String() = {
        "ERR",
        "CRT",
        "WRN",
        "INF",
        "DBG"
    }

    Private Class _LogEntry
        Public ReadOnly Property Timestamp As Date
        Public ReadOnly Property Message As String
        Public ReadOnly Property Level As LogLevel

        Public Sub New(timeStamp As Date, message As String, level As LogLevel)
            Me.Timestamp = timeStamp
            Me.Message = message
            Me.Level = level
        End Sub

        Public Sub Print()
            Me._Print(ConsoleColor.White, Me.Timestamp.GetIsoString & " ")
            Me._Print(_COLORS(Me.Level), _SYMBOLS(Me.Level) & " ")
            Me._Print(ConsoleColor.Gray, Me.Message & vbNewLine)
        End Sub

        Private Sub _Print(color As ConsoleColor, message As String)
            Dim c As ConsoleColor = Console.ForegroundColor
            Console.ForegroundColor = color
            Console.Write(message)
            Console.ForegroundColor = c
        End Sub

    End Class

    Private _is_disposed As Boolean
    Private _is_running As Boolean = False
    Private ReadOnly _thr_worker As New Threading.Thread(AddressOf Me._logging_worker)
    Private ReadOnly _thr_token_source As New Threading.CancellationTokenSource
    Private ReadOnly _queue As New List(Of _LogEntry)

    Public Property Verbosity As LogLevel = LogLevel.Info

    Private Shared _Current As Logger
    Public Shared ReadOnly Property Instance As Logger
        Get
            If _Current Is Nothing Then _Current = New Logger
            Return _Current
        End Get
    End Property

    Private Sub New()
        Me._thr_worker.Start(Me._thr_token_source.Token)
        Threading.SpinWait.SpinUntil(Function() Me._is_running)
        Me.Log(LogLevel.Info, "Beginning of log.")
    End Sub

    Private Sub _logging_worker(token As Threading.CancellationToken)
        Try
            Me._is_running = True
            While Not token.IsCancellationRequested
                Me._logging_step()
                Threading.Thread.Sleep(1)
            End While
        Catch thex As Threading.ThreadAbortException
        Finally
            Me._logging_step()
            Me._is_running = False
        End Try
    End Sub

    Private Sub _logging_step()
        SyncLock Me._queue
            For Each entry As _LogEntry In Me._queue.OrderBy(Function(e) e.Timestamp)
                If Me.Verbosity >= entry.Level Then entry.Print()
            Next
            Me._queue.Clear()
        End SyncLock
    End Sub

    Public Sub Log(level As LogLevel, message As String)
        If Not Me._is_running Then Throw New Exception("Logging has been disabled.")
        SyncLock Me._queue
            Me._queue.Add(New _LogEntry(Now, message, level))
        End SyncLock
    End Sub

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not Me._is_disposed Then
            If disposing Then
                Me.Log(LogLevel.Info, "Ending of log.")
                Me._thr_token_source.Cancel()
                Threading.SpinWait.SpinUntil(Function() Not Me._is_running)
            End If
        End If
        Me._is_disposed = True
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        Dispose(True)
    End Sub

End Class
