Imports MaxMind.GeoIP2.Responses

Namespace Geo
    Public Class GeoLightEntry

        Public Shared ReadOnly Property Empty As New GeoLightEntry("Unknown", "Unknown", "Unknown", 0, 0, Double.NaN, Double.NaN)

        Public ReadOnly Property Confidence As Double
        Public ReadOnly Property ContinentName As String
        Public ReadOnly Property CityName As String
        Public ReadOnly Property CountryName As String
        Public ReadOnly Property Accuracy As Integer
        Public ReadOnly Property Longitude As Double
        Public ReadOnly Property Latitude As Double

        Private Sub New(continent As String, country As String, city As String, confidence As Double, accuracy As Integer, longitude As Double, latitude As Double)
            Me.Confidence = confidence
            Me.ContinentName = If(continent, String.Empty)
            Me.CountryName = If(country, String.Empty)
            Me.CityName = If(city, String.Empty)
            Me.Accuracy = accuracy
            Me.Longitude = longitude
            Me.Latitude = latitude
        End Sub

        Public Shared Function FromMaxMind(city As CityResponse) As GeoLightEntry
            Return New GeoLightEntry(
                city.Continent.Name,
                city.Country.Name,
                New String(city.City.Name),
                If(city.City.Confidence, 0) / 100,
                If(city.Location.AccuracyRadius, 0),
                city.Location.Longitude,
                city.Location.Latitude
            )
        End Function

        Public Sub ToBinStream(w As IO.BinaryWriter)
            With w
                .Write(Me.Confidence)
                .Write(Me.ContinentName)
                .Write(Me.CountryName)
                .Write(Me.CityName)
                .Write(Me.Accuracy)
                .Write(Me.Longitude)
                .Write(Me.Latitude)
            End With
        End Sub

        Public Shared Function FromBinStream(r As IO.BinaryReader) As GeoLightEntry
            Return New GeoLightEntry(
                r.ReadString,
                r.ReadString,
                r.ReadString,
                r.ReadDouble,
                r.ReadInt32,
                r.ReadDouble,
                r.ReadDouble
            )
        End Function

    End Class

End Namespace
