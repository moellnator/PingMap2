Imports System.Net

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

End Class
