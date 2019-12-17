
Public Delegate Sub PingEventHandler(sender As Object, e As PingEventArgs)
Public Class PingEventArgs : Inherits EventArgs

    Public ReadOnly Results As PingResult

    Public Sub New(results As PingResult)
        Me.Results = results
    End Sub

End Class
