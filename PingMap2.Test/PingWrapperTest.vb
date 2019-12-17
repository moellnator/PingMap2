Imports System.Net
Imports System.Text
Imports Microsoft.VisualStudio.TestTools.UnitTesting

<TestClass()> Public Class PingWrapperTest

    Private _ping_results As PingResult
    Private _ping_event_count As Integer = 0
    Private ReadOnly _lock As New Threading.AutoResetEvent(False)

    <TestMethod()> Public Sub SendPing()
        ExecutePing(IPAddress.Parse("127.0.0.1"), 1)
        Assert.AreEqual(1, Me._ping_event_count)
        Assert.IsTrue(Me._ping_results.Success)
    End Sub

    <TestMethod> Public Sub PingFail()
        ExecutePing(IPAddress.Parse("8.8.8.8"), 1)
        Assert.IsFalse(Me._ping_results.Success)
    End Sub

    Private Sub ExecutePing(address As IPAddress, ttl As Integer)
        Dim ping As New PingWrapper(address, ttl)
        AddHandler ping.PingEvent, AddressOf Me._send_ping_callback
        ping.Timeout = 1000
        ping.StartPing()
        Me._lock.WaitOne(1200)
        RemoveHandler ping.PingEvent, AddressOf Me._send_ping_callback
    End Sub

    Private Sub _send_ping_callback(sender As Object, e As PingEventArgs)
        Me._ping_results = e.Results
        Me._ping_event_count += 1
        Me._lock.Set()
    End Sub

End Class