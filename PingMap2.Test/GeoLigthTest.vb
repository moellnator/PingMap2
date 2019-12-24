Imports System.Net

<TestClass> Public Class GeoLigthTest

    <TestMethod> Public Sub TestGeoLight()
        Dim geo As GeoLight = GeoLight.Current
        Dim result As GeoResult = geo.Resolve(IPAddress.Parse("8.8.8.8"))
        Assert.AreEqual("North America", result.ContinentName)
    End Sub

End Class
