Namespace Engine
    Public Class Runtime : Implements IDisposable

        Private _is_disposed As Boolean = False
        Private _is_disposing As Boolean = False

        Private WithEvents _session As Session
        Private ReadOnly _pool As Pool
        Private ReadOnly _thread_count As Integer
        Private ReadOnly _sinks As New List(Of IReportSink)

        Public Property MaxTTL As Integer = 255
        Public Property Timeout As Integer = 1000

        Public Sub New(clientName As String, threads As Integer)
            Me._session = Session.CreateNew(clientName)
            Me._pool = New Pool With {.MaxWorkerCount = threads}
            AddHandler Me._pool.ReportQueueEmptyEvent, AddressOf Me._pool_empty
            AddHandler Me._pool.ReportEvent, AddressOf Me._pool_report
            Me._thread_count = threads
            Me._pool.EnqueueRequests(_CreateRandomWorkers())
        End Sub

        Public Sub AddReportSink(sink As IReportSink)
            Me._sinks.Add(sink)
        End Sub

        Public Sub StartRuntime()
            Me._pool.StartPool()
        End Sub

        Private Sub _pool_empty(sender As Object, e As EventArgs)
            SyncLock Me._session
                Me._pool.EnqueueRequests(_CreateRandomWorkers())
            End SyncLock
        End Sub

        Private Sub _pool_report(sender As Object, e As ReportEventArgs)
            If e.IsSuccessful Then
                For Each sink As IReportSink In Me._sinks
                    sink.Register(e.Report)
                Next
            End If
        End Sub

        Private Function _CreateRandomWorkers() As Worker()
            Return Enumerable.Range(0, Me._thread_count).Select(
                Function(i) New Worker(
                    Me._session,
                    Network.Address.Random,
                    Me.MaxTTL,
                    Me.Timeout
                )
            ).ToArray
        End Function

        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me._is_disposed Then
                Me._is_disposing = True
                If disposing Then
                    RemoveHandler Me._pool.ReportQueueEmptyEvent, AddressOf Me._pool_empty
                    RemoveHandler Me._pool.ReportEvent, AddressOf Me._pool_report
                    Me._session.Dispose()
                    Me._pool.Dispose()
                End If
            End If
            Me._is_disposed = True
        End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
            Dispose(True)
        End Sub

        Private Sub _session_Closed(sender As Object, e As EventArgs) Handles _session.Closed
            If Not Me._is_disposing Then
                SyncLock Me._session
                    Me._session = Session.CreateNew(Me._session.ClientName)
                End SyncLock
            End If
        End Sub

    End Class

End Namespace
