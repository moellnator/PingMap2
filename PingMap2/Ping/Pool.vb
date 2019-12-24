Imports System.Net

Namespace Ping
    Public Class Pool : Implements IDisposable

        Public Event PingEvent As PingEventHandler

        Public Property MaxWorkerCount As Integer = 16

        Private ReadOnly _available_pings As New Stack(Of Wrapper)
        Private ReadOnly _lock As New Threading.AutoResetEvent(False)
        Private ReadOnly _active_pings As New List(Of Wrapper)
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
                Return Me._active_pings.Count = 0 And Me._available_pings.Count = 0
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

        Public Sub EnqueuePing(address As IPAddress, ttl As Integer)
            SyncLock Me._update_lock
                Me._available_pings.Push(New Wrapper(address, ttl))
            End SyncLock
        End Sub

        Public Sub EnqueuePings(pings As IEnumerable(Of Wrapper))
            SyncLock Me._update_lock
                For Each p As Wrapper In pings
                    Me._available_pings.Push(p)
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
                If Me._active_pings.Count < Me.MaxWorkerCount Then
                    If Me._available_pings.Count > 0 Then
                        Dim new_ping As Wrapper = Me._available_pings.Pop
                        AddHandler new_ping.PingEvent, AddressOf Me._ping_call_back
                        new_ping.StartPing()
                    End If
                End If
            End SyncLock
        End Sub

        Private Sub _ping_call_back(sender As Object, e As PingEventArgs)
            Dim wrapper As Wrapper = sender
            SyncLock Me._update_lock
                Me._active_pings.Remove(sender)
                RemoveHandler wrapper.PingEvent, AddressOf Me._ping_call_back
                wrapper.Dispose()
            End SyncLock
            RaiseEvent PingEvent(Me, e)
        End Sub

        Private Sub _abort_pool()
            SyncLock Me._update_lock
                For Each p As Wrapper In Me._active_pings
                    p.Abort()
                    p.Dispose()
                Next
                Me._active_pings.Clear()
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