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
            Dim timestamp As Date = Now
            Dim retval As New Ping(timestamp, target, Nothing, 0, IPStatus.Unknown)
            Try
                Dim ping As New NetworkInformation.Ping
                Dim reply As PingReply = Ping.Send(
                    target,
                    timeout,
                    target.GetAddressBytes,
                    New PingOptions(ttl, True)
                )
                retval = New Ping(
                    timestamp,
                    target,
                    reply.Address,
                    reply.RoundtripTime,
                    reply.Status
                )
            Catch ex As PingException
            End Try
            Return retval
        End Function

        Public Sub ToBinStream(w As IO.BinaryWriter)
            With w
                .Write(Me.Timestamp.ToBinary)
                .Write(Me.Target.GetAddressBytes, 0, 4)
                .Write(If(Me.Replier.GetAddressBytes, {0, 0, 0, 0}), 0, 4)
                .Write(Me.RTT)
                .Write(Me.State)
            End With
        End Sub

        Public Shared Function FromBinStream(r As IO.BinaryReader) As Ping
            Return New Ping(
                Date.FromBinary(r.ReadInt64),
                New IPAddress(r.ReadBytes(4)),
                (Function(b As IPAddress) If(b.Equals(New IPAddress({0, 0, 0, 0})), Nothing, b))(New IPAddress(r.ReadBytes(4))),
                r.ReadInt32,
                r.ReadInt32
            )
        End Function

    End Class

End Namespace
