Imports System.Net

Namespace Network
    Public Class Address

        Public Shared ReadOnly Reserved As IPRange() = {
            IPRange.Parse("0.0.0.0/8"),
            IPRange.Parse("10.0.0.0/8"),
            IPRange.Parse("100.64.0.0/10"),
            IPRange.Parse("127.0.0.0/8"),
            IPRange.Parse("169.254.0.0/16"),
            IPRange.Parse("172.16.0.0/12"),
            IPRange.Parse("192.0.0.0/24"),
            IPRange.Parse("192.0.2.0/24"),
            IPRange.Parse("192.88.99.0/24"),
            IPRange.Parse("192.168.0.0/16"),
            IPRange.Parse("198.18.0.0/15"),
            IPRange.Parse("198.51.100.0/24"),
            IPRange.Parse("203.0.113.0/24"),
            IPRange.Parse("224.0.0.0/4"),
            IPRange.Parse("240.0.0.0/4"),
            IPRange.Parse("255.255.255.255/32")
        }

        Private Shared _Source As IPAddress
        Private Shared ReadOnly _RNG As New Random

        Public Shared ReadOnly Property Source As IPAddress
            Get
                Static _obj As New Object
                SyncLock _obj
                    If _Source Is Nothing Then
                        Dim addr As String = New WebClient().DownloadString("http://icanhazip.com").Trim(vbCr, vbLf, vbTab, " ")
                        _Source = IPAddress.Parse(addr)
                    End If
                End SyncLock
                Return _Source
            End Get
        End Property

        Public Shared ReadOnly Property Random As IPAddress
            Get
                Dim retval As IPAddress
                Do
                    Dim bytes As Byte() = New Byte(3) {}
                    _RNG.NextBytes(bytes)
                    retval = New IPAddress(bytes)
                Loop While IsReserved(retval)
                Return retval
            End Get
        End Property

        Public Shared Function IsReserved(address As IPAddress) As Boolean
            Return Reserved.Any(Function(r) r.Contains(address))
        End Function

    End Class

End Namespace
