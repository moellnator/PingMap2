Imports System.Net
Imports System.Net.NetworkInformation

Public Class PingResult

    Public ReadOnly Property StartTTL As Integer
    Public ReadOnly Property TTL As Integer
    Public ReadOnly Property RTT As Double
    Public ReadOnly Property Target As IPAddress
    Public ReadOnly Property Replier As IPAddress
    Public ReadOnly Property State As IPStatus
    Public ReadOnly Property Success As Boolean
        Get
            Return Me.State = IPStatus.Success
        End Get
    End Property

    Public Sub New(state As IPStatus, target As IPAddress, replier As IPAddress, startTtl As Integer, ttl As Integer, rtt As Double)
        Me.State = state
        Me.Replier = replier
        Me.Target = target
        Me.TTL = ttl
        Me.StartTTL = startTtl
        Me.RTT = rtt
    End Sub

End Class
