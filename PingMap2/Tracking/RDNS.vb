Imports System.Net

Namespace Tracking
    Public Class RDNS

        Public Shared Function LookupIP(address As IPAddress) As String
            Dim retval As String = ""
            Dim entry As IPHostEntry = Dns.GetHostEntry(address)
            Return entry.HostName
        End Function

    End Class

End Namespace
