Imports System.Net

Namespace Engine
    Public Class Session : Implements IEquatable(Of Session), IDisposable

        Public Event Closed As EventHandler

        Public ReadOnly Property ClientName As String
        Public ReadOnly Property Source As IPAddress
        Public ReadOnly Property Start As Date

        Private ReadOnly _session_thread As Threading.Thread
        Private ReadOnly _session_token As New Threading.CancellationTokenSource
        Private _is_disposed As Boolean = False
        Private _is_valid As Boolean = True

        Private Sub New(clientName As String, source As IPAddress, start As Date)
            Me.ClientName = clientName
            Me.Source = source
            Me.Start = start
            Me._session_thread = New Threading.Thread(AddressOf Me._session_worker)
            Me._session_thread.Start(Me._session_token.Token)
        End Sub

        Public ReadOnly Property IsValid As Boolean
            Get
                Return Me._is_valid
            End Get
        End Property

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

        Private Sub _session_worker(token As Threading.CancellationToken)
            Me._is_valid = True
            Dim watch As New Stopwatch
            While Not token.IsCancellationRequested
                If watch.ElapsedMilliseconds >= 5000 Then
                    If Not Me._validate Then Exit While
                    watch.Restart()
                End If
                Threading.Thread.Sleep(10)
            End While
            Me._is_valid = False
            RaiseEvent Closed(Me, EventArgs.Empty)
        End Sub

        Private Function _validate() As Boolean
            Dim current As IPAddress = Network.Address.Source
            Return current IsNot Nothing AndAlso current.Equals(Me.Source)
        End Function

        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me._is_disposed Then
                If disposing Then
                    Me._session_token.Cancel()
                    Threading.SpinWait.SpinUntil(Function() Not Me.IsValid)
                End If
            End If
            Me._is_disposed = True
        End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
            Dispose(True)
        End Sub

        Public Sub ToBinStream(w As IO.BinaryWriter)
            w.Write(Me.ClientName)
            w.Write(Me.Source.GetAddressBytes, 0, 4)
            w.Write(Me.Start.GetIsoString)
        End Sub

        Public Shared Function FromBinStream(r As IO.BinaryReader) As Session
            Return New Session(r.ReadString, New IPAddress(r.ReadBytes(4)), Date.Parse(r.ReadString))
        End Function

    End Class

End Namespace