Imports System.Runtime.CompilerServices

Public Module Extensions

    <Extension> Public Function GetIsoString(t As Date) As String
        Return t.ToString("yyyy-MM-ddTHH:mm:ssK")
    End Function

End Module
