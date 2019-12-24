
Namespace Ping
    Public Delegate Sub PingEventHandler(sender As Object, e As PingEventArgs)
    Public Class PingEventArgs : Inherits EventArgs

        Public ReadOnly Results As Result

        Public Sub New(results As Result)
            Me.Results = results
        End Sub

    End Class

End Namespace