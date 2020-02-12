Imports System.Net

Namespace Engine
    Public Class Pool : Implements IDisposable

        Public Event ReportEvent As ReportEventHandler
        Public Event ReportQueueEmptyEvent As EventHandler

        Public Property MaxWorkerCount As Integer = 16

        Private ReadOnly _available_workers As New Stack(Of Worker)
        Private ReadOnly _lock As New Threading.AutoResetEvent(False)
        Private ReadOnly _active_workers As New List(Of Worker)
        Private ReadOnly _update_lock As New Object
        Private _token_source As Threading.CancellationTokenSource
        Private _current_pool_thread As Threading.Thread
        Private _is_running As Boolean = False
        Private _is_disposed As Boolean = False

        Public Property IsRunning As Boolean
            Get
                Return Me._is_running
            End Get
            Set(value As Boolean)
                If Me._is_running <> value Then
                    If value Then
                        Me.StartPool()
                    Else
                        Me.StopPool()
                    End If
                End If
            End Set
        End Property

        Public ReadOnly Property IsDone As Boolean
            Get
                Return Me._active_workers.Count = 0 And Me._available_workers.Count = 0
            End Get
        End Property

        Public Sub StartPool()
            If Me._is_running Then Exit Sub
            Me._token_source = New Threading.CancellationTokenSource
            Me._current_pool_thread = New Threading.Thread(AddressOf Me._pool_worker)
            Me._current_pool_thread.Start(Me._token_source.Token)
            Me._lock.WaitOne()
        End Sub

        Public Sub StopPool()
            If Not Me._is_running Then Exit Sub
            Me._abort_pool()
            Me._token_source.Cancel()
            Me._token_source.Token.WaitHandle.WaitOne()
            Me._lock.WaitOne(1000)
            If Not Me._is_running = False Then Me._current_pool_thread.Abort()
            Me._token_source.Dispose()
            Me._current_pool_thread = Nothing
        End Sub

        Public Sub EnqueueRequests(workers As IEnumerable(Of Worker))
            SyncLock Me._update_lock
                For Each p As Worker In workers
                    Me._available_workers.Push(p)
                Next
            End SyncLock
        End Sub

        Private Sub _pool_worker(token As Threading.CancellationToken)
            Try
                Me._is_running = True
                Me._lock.Set()
                While Not token.IsCancellationRequested
                    Me._update_pool()
                    Threading.Thread.Sleep(100)
                End While
            Catch tha_ex As Threading.ThreadAbortException
            Finally
                Me._is_running = False
                Me._lock.Set()
            End Try
        End Sub

        Private Sub _update_pool()
            SyncLock Me._update_lock
                If Me._available_workers.Count = 0 Then
                    RaiseEvent ReportQueueEmptyEvent(Me, EventArgs.Empty)
                Else
                    If Me._active_workers.Count < Me.MaxWorkerCount Then
                        Dim new_ping As Worker = Me._available_workers.Pop
                        AddHandler new_ping.ReportEvent, AddressOf Me._worker_call_back
                        new_ping.StartReport()
                    End If
                End If
            End SyncLock
        End Sub

        Private Sub _worker_call_back(sender As Object, e As ReportEventArgs)
            Dim worker As Worker = sender
            SyncLock Me._update_lock
                Me._active_workers.Remove(sender)
                RemoveHandler worker.ReportEvent, AddressOf Me._worker_call_back
                worker.Dispose()
            End SyncLock
            RaiseEvent ReportEvent(Me, e)
        End Sub

        Private Sub _abort_pool()
            SyncLock Me._update_lock
                For Each p As Worker In Me._active_workers
                    p.Abort()
                    p.Dispose()
                Next
                Me._active_workers.Clear()
            End SyncLock
        End Sub

        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me._is_disposed Then
                If disposing Then
                    If Me.IsRunning Then Me.StopPool()
                End If
            End If
            Me._is_disposed = True
        End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
            Dispose(True)
        End Sub

    End Class

End Namespace
