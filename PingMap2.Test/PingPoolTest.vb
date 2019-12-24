Imports System.Net

<TestClass()> Public Class PingPoolTest

    Private ReadOnly _ping_results As New List(Of Ping.Result)

    <TestMethod> Public Sub TestPingPool()
        Using pool As New Ping.Pool With {.MaxWorkerCount = 2}
            pool.EnqueuePing(IPAddress.Parse("127.0.0.1"), 1)
            pool.EnqueuePing(IPAddress.Parse("8.8.8.8"), 1)
            AddHandler pool.PingEvent, AddressOf Me._send_ping_callback
            pool.StartPool()
            Threading.SpinWait.SpinUntil(Function() pool.IsDone, 2000)
            pool.StopPool()
            RemoveHandler pool.PingEvent, AddressOf Me._send_ping_callback
        End Using
        Assert.IsTrue(_ping_results.Any(Function(r) r.Success))
        Assert.IsTrue(_ping_results.Any(Function(r) Not r.Success))
    End Sub

    Private Sub _send_ping_callback(sender As Object, e As Ping.PingEventArgs)
        Me._ping_results.Add(e.Results)
    End Sub

End Class
