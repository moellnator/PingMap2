Imports System.Net
Imports PingMap2.Tracking

<TestClass> Public Class TrackingTest

    <TestMethod> Public Sub TestGeoLight()
        Dim geo As GeoLight = GeoLight.Current
        Dim result As GeoResult = geo.Resolve(IPAddress.Parse("8.8.8.8"))
        Assert.AreEqual("North America", result.ContinentName)
    End Sub

    <TestMethod> Public Sub TestDnsLookup()
        Debug.Print(RDNS.LookupIP(IPAddress.Parse("8.8.8.8")))
    End Sub

End Class
