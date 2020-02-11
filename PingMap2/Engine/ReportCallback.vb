Namespace Engine

    Public Delegate Sub ReportEventHandler(sender As Object, e As ReportEventArgs)

    Public Class ReportEventArgs : Inherits EventArgs

        Public ReadOnly Property Report As Report

        Public ReadOnly Property IsSuccessful As Boolean
            Get
                Return Me.Report IsNot Nothing
            End Get
        End Property

        Public Sub New(report As Report)
            Me.Report = report
        End Sub

    End Class

End Namespace
