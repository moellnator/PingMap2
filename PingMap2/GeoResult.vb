﻿Imports MaxMind.GeoIP2.Responses

Public Class GeoResult

    Public ReadOnly Property Confidence As Double
    Public ReadOnly Property ContinentName As String
    Public ReadOnly Property CityName As String
    Public ReadOnly Property CountryName As String
    Public ReadOnly Property Accuracy As Integer
    Public ReadOnly Property Longitude As Double
    Public ReadOnly Property Latitude As Double

    Private Sub New(continent As String, country As String, city As String, confidence As Double, accuracy As Integer, longitude As Double, latitude As Double)
        Me.Confidence = confidence
        Me.ContinentName = continent
        Me.CountryName = country
        Me.CityName = city
        Me.Accuracy = accuracy
        Me.Longitude = longitude
        Me.Latitude = latitude
    End Sub

    Public Shared Function FromMaxMind(city As CityResponse) As GeoResult
        Return New GeoResult(
            city.Continent.Name,
            city.Country.Name,
            New String(city.City.Name),
            If(city.City.Confidence IsNot Nothing, city.City.Confidence / 100, 0),
            city.Location.AccuracyRadius,
            city.Location.Longitude,
            city.Location.Latitude
        )
    End Function

End Class
