Imports PingMap2.Engine

Module Module1

    Sub Main()
        Dim report_path As String = "D:\Nutzer\Documents\Visual Studio 2017\Projects\PingMap2\PingMap2.Local\bin\Debug"
        Dim reports As Report() = ReportDatabaseReader.GetReports(report_path).ToArray

        For Each t In (
                From r As Report In reports
                Where r.Location.CountryName <> "" Or r.Location.CountryName = "Unknown"
                Group By c = r.Location.CountryName Into g = Group
                Select o = New With {.Name = c, g.Count, .Average = g.Sum(Function(r) r.Ping.RTT) / g.Count}
                Order By o.Average
            )
            Console.WriteLine(
                t.Name.PadRight(24, " "c) &
                t.Count.ToString.PadRight(4, " "c) &
                (t.Average.ToString("0.0").Replace(",", ".") & " ms").PadRight(7, " "c)
            )
        Next


        Console.WriteLine("Press [ANY] key to continue...")
        Console.ReadKey(True)

    End Sub

End Module
