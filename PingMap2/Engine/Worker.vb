Imports System.Net

Namespace Engine
    Public Class Worker : Implements IDisposable

        Public Event ReportEvent As ReportEventHandler

        Private _is_disposed As Boolean = False
        Private ReadOnly _thr As New Threading.Thread(AddressOf Me._worker_task)
        Private ReadOnly _target As IPAddress
        Private ReadOnly _time_out As Integer
        Private ReadOnly _max_ttl As Integer
        Private ReadOnly _session As Session

        Private _is_running As Boolean = False
        Public ReadOnly Property IsRunning As Boolean
            Get
                Return Me._is_running
            End Get
        End Property

        Public Sub New(session As Session, target As IPAddress, ttl As Integer, timeout As Integer)
            Me._session = session
            Me._target = target
            Me._max_ttl = ttl
            Me._time_out = timeout
        End Sub

        Public Sub StartReport()
            Me._thr.Start()
        End Sub

        Public Sub Abort()
            If Me._thr.ThreadState = Threading.ThreadState.Running Then Me._thr.Abort()
        End Sub

        Private Sub _worker_task()
            Dim report As Report = Nothing
            Try
                Me._is_running = True
                If Me._session.IsValid Then report = Report.FromRequest(Me._session, Me._target, Me._max_ttl, Me._time_out)
            Catch tha_ex As Threading.ThreadAbortException
            Finally
                Me._is_running = False
                RaiseEvent ReportEvent(Me, New ReportEventArgs(report))
            End Try
        End Sub

        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me._is_disposed Then
                If disposing Then
                    If Me.IsRunning Then Me.Abort()
                End If
            End If
            Me._is_disposed = True
        End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
            Dispose(True)
        End Sub

    End Class

End Namespace
