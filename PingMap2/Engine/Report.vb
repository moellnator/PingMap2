Imports System.Net

Namespace Engine
    Public Class Report

        Public ReadOnly Property Session As Session
        Public ReadOnly Property Target As IPAddress
        Public ReadOnly Property Ping As Network.Ping
        Public ReadOnly Property Route As Network.TraceRoute
        Public ReadOnly Property Location As Geo.GeoLightEntry

        Public Sub New(session As Session, target As IPAddress, ping As Network.Ping, route As Network.TraceRoute, location As Geo.GeoLightEntry)
            Me.Session = session
            Me.Target = target
            Me.Ping = ping
            Me.Route = route
            Me.Location = location
        End Sub

        Public Shared Function FromRequest(session As Session, target As IPAddress, maxTTL As Integer, timeout As Integer) As Report
            Dim retval As Report = Nothing
            Dim ping As Network.Ping = Network.Ping.FromRequest(target, maxTTL, timeout)
            If ping.IsSuccessful Then
                Dim location As Geo.GeoLightEntry = Geo.GeoLightDB.Current.Resolve(target)
                Dim route As Network.TraceRoute = Network.TraceRoute.FromRequest(target, maxTTL, timeout)
                retval = New Report(session, target, ping, route, location)
            End If
            Return retval
        End Function

    End Class

End Namespace
