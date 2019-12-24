Imports System.Net
Imports MaxMind.GeoIP2
Imports MaxMind.GeoIP2.Responses

Public Class GeoLight

    'Database and Contents Copyright (c) 2019 MaxMind, Inc.

    'This product includes GeoLite2 data created by MaxMind, available from
    ' <a href = "https://www.maxmind.com" > https : //www.maxmind.com</a>.

    Private Shared _Current_Instance As GeoLight

    Public Shared ReadOnly Property Current As GeoLight
        Get
            If _Current_Instance Is Nothing Then _Current_Instance = New GeoLight
            Return _Current_Instance
        End Get
    End Property

    Private ReadOnly _geo_db_reader As DatabaseReader

    Private Sub New()
        Using m As New IO.MemoryStream(My.Resources.GeoIP2_City)
            _geo_db_reader = New DatabaseReader(m)
        End Using
    End Sub

    Protected Overrides Sub Finalize()
        Me._geo_db_reader.Dispose()
    End Sub

    Public Function Resolve(address As IPAddress) As GeoResult
        Return GeoResult.FromMaxMind(Me._geo_db_reader.City(address))
    End Function

End Class
