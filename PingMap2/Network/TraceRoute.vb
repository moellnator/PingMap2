Imports System.Net

Namespace Network
    Public Class TraceRoute : Implements IReadOnlyList(Of IPAddress)

        Private ReadOnly _hops As IPAddress()

        Public Sub New(hops As IEnumerable(Of IPAddress))
            Me._hops = hops.ToArray
        End Sub

        Default Public ReadOnly Property Item(index As Integer) As IPAddress Implements IReadOnlyList(Of IPAddress).Item
            Get
                Return Me._hops(index)
            End Get
        End Property

        Public ReadOnly Property Count As Integer Implements IReadOnlyCollection(Of IPAddress).Count
            Get
                Return Me._hops.Count
            End Get
        End Property

        Public Shared Function FromRequest(target As IPAddress, maxTTL As Integer, timeout As Integer) As TraceRoute
            Dim retval As New List(Of Ping)
            For i = 1 To maxTTL
                Dim reply As Ping = Ping.FromRequest(target, i, timeout)
                retval.Add(reply)
                If reply.Replier IsNot Nothing AndAlso reply.Replier.Equals(target) Then Exit For
            Next
            Return New TraceRoute(retval.Select(Function(h) h.Replier))
        End Function

        Public Function GetEnumerator() As IEnumerator(Of IPAddress) Implements IEnumerable(Of IPAddress).GetEnumerator
            Return Me._hops.AsEnumerable.GetEnumerator
        End Function

        Private Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
            Return Me.GetEnumerator
        End Function

        Public Sub ToBinStream(w As IO.BinaryWriter)
            w.Write(Me._hops.Count)
            For Each h As IPAddress In Me._hops
                w.Write(If(h Is Nothing, {0, 0, 0, 0}, h.GetAddressBytes), 0, 4)
            Next
        End Sub

        Public Shared Function FromBinStream(r As IO.BinaryReader) As TraceRoute
            Dim count As Integer = r.ReadInt32
            Return New TraceRoute(
                Enumerable.Range(0, count).Select(
                    Function(i)
                        Dim retval As New IPAddress(r.ReadBytes(4))
                        Return If(retval.Equals(New IPAddress({0, 0, 0, 0})), Nothing, retval)
                    End Function
                )
            )
        End Function

    End Class

End Namespace
