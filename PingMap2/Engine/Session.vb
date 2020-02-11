Imports System.Net

Namespace Engine
    Public Class Session : Implements IEquatable(Of Session)

        Public ReadOnly Property ClientName As String
        Public ReadOnly Property Source As IPAddress
        Public ReadOnly Property Start As Date

        Private Sub New(clientName As String, source As IPAddress, start As Date)
            Me.ClientName = clientName
            Me.Source = source
            Me.Start = start
        End Sub

        Public Shared Function CreateNew(clientName As String) As Session
            Return New Session(clientName, Network.Address.Source, Now)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return Me.ClientName.GetHashCode Xor Me.Source.GetHashCode Xor Me.Start.GetHashCode
        End Function

        Public Overrides Function Equals(obj As Object) As Boolean
            Dim retval As Boolean = False
            If TypeOf obj Is Session Then
                retval = Me.IEquatable_Equals(obj)
            End If
            Return retval
        End Function

        Private Function IEquatable_Equals(other As Session) As Boolean Implements IEquatable(Of Session).Equals
            Return other.ClientName.Equals(Me.ClientName) And other.Source.Equals(Me.Source) And other.Start.Equals(Me.Start)
        End Function

    End Class

End Namespace