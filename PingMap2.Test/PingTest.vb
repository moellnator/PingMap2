Imports System.Net
Imports PingMap2.Engine

<TestClass()> Public Class PingTest

    <TestMethod> Public Sub SourceAddress()
        Debug.Print(Network.Address.Source.ToString)
    End Sub

    <TestMethod> Public Sub PingRequest()
        Dim target As IPAddress = IPAddress.Parse("8.8.8.8")
        Dim reply As Network.Ping = Network.Ping.FromRequest(target, 255, 1000)
        Assert.AreEqual(target, reply.Replier)
        Assert.AreEqual(NetworkInformation.IPStatus.Success, reply.State)
    End Sub

    <TestMethod> Public Sub Tracert()
        Dim target As IPAddress = IPAddress.Parse("8.8.8.8")
        Dim route As Network.TraceRoute = Network.TraceRoute.FromRequest(target, 255, 1000)
        For Each node As IPAddress In route
            Debug.Print(node.ToString)
        Next
    End Sub

    <TestMethod> Public Sub RandomIp()
        Dim raddrd As IPAddress = Network.Address.Random
        Assert.IsFalse(Network.Address.IsReserved(raddrd))
    End Sub

    <TestMethod> Public Sub Runtime()
        Dim sink As New DummySink
        Using runtime As New Runtime("dummy", 16)
            runtime.AddReportSink(sink)
            runtime.StartRuntime()
            Threading.SpinWait.SpinUntil(Function() sink.Count > 0)
        End Using
        Debug.Print(sink.First.Ping.Replier.ToString)
    End Sub

    Private Class DummySink : Implements IReportSink, IReadOnlyList(Of Report)

        Private ReadOnly _reports As New List(Of Report)

        Public Sub Register(report As Report) Implements IReportSink.Register
            Me._reports.Add(report)
        End Sub

        Default Public ReadOnly Property Item(index As Integer) As Report Implements IReadOnlyList(Of Report).Item
            Get
                Return Me._reports.Item(index)
            End Get
        End Property

        Public ReadOnly Property Count As Integer Implements IReadOnlyCollection(Of Report).Count
            Get
                Return Me._reports.Count
            End Get
        End Property

        Public Function GetEnumerator() As IEnumerator(Of Report) Implements IEnumerable(Of Report).GetEnumerator
            Return Me._reports.GetEnumerator
        End Function

        Private Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
            Return Me.GetEnumerator
        End Function

    End Class

End Class
