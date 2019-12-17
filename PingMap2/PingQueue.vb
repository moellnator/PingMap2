Public Class PingQueue

    Public Event QueueEmpty As EventHandler

    Private _is_done As Boolean = False
    Private ReadOnly _pool As New PingPool
    Private ReadOnly _results As New List(Of PingResult)

    Public ReadOnly Property IsDone As Boolean
        Get
            Return Me._is_done
        End Get
    End Property

    Public ReadOnly Property Results As IEnumerable(Of PingResult)
        Get
            Return Me._results
        End Get
    End Property

    Public Sub New(requests As IEnumerable(Of PingWrapper))
        Me._pool.EnqueuePings(requests)
        AddHandler Me._pool.PingEvent, AddressOf Me._pool_callback
    End Sub

    Public Sub RunQueue()
        If Me._is_done Then Throw New InvalidOperationException("Cannot rerun ping queue.")
        Me._pool.StartPool()
    End Sub

    Private Sub _pool_callback(sender As Object, e As PingEventArgs)
        Me._results.Add(e.Results)
        If Me._pool.IsDone Then
            Me._is_done = True
            Me._pool.StopPool()
            RemoveHandler Me._pool.PingEvent, AddressOf Me._pool_callback
            Me._pool.Dispose()
            RaiseEvent QueueEmpty(Me, EventArgs.Empty)
        End If
    End Sub

End Class
