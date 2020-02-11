Imports System.Net
Imports System.Net.NetworkInformation

Namespace Network
    Public Class Ping

        Public ReadOnly Property Timestamp As Date
        Public ReadOnly Property Target As IPAddress
        Public ReadOnly Property Replier As IPAddress
        Public ReadOnly Property RTT As Integer
        Public ReadOnly Property State As IPStatus

        Public ReadOnly Property IsSuccessful As Boolean
            Get
                Return Me.State = IPStatus.Success
            End Get
        End Property

        Public Sub New(timeStamp As Date, target As IPAddress, replier As IPAddress, rtt As Integer, state As IPStatus)
            Me.Timestamp = timeStamp
            Me.Target = target
            Me.Replier = replier
            Me.RTT = rtt
            Me.State = state
        End Sub

        Public Shared Function FromRequest(target As IPAddress, ttl As Integer, timeout As Integer) As Ping
            Dim ping As New NetworkInformation.Ping
            Dim timestamp As Date = Now
            Dim reply As PingReply = ping.Send(
                target,
                timeout,
                target.GetAddressBytes,
                New PingOptions(ttl, True)
            )
            Return New Ping(
                timestamp,
                target,
                reply.Address,
                reply.RoundtripTime,
                reply.Status
            )
        End Function

    End Class

End Namespace
