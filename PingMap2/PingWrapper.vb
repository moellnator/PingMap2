Imports System.Net
Imports System.Net.NetworkInformation

Public Class PingWrapper : Implements IDisposable

    Private Shared ReadOnly Property _PAYLOAD As Byte() = {}

    Public Event PingEvent As PingEventHandler

    Private _is_disposed As Boolean = False
    Private ReadOnly _ping As New Ping
    Private ReadOnly _thr As New Threading.Thread(AddressOf Me._worker_task)
    Private ReadOnly _target As IPAddress
    Private ReadOnly _ttl As Integer

    Public Property Timeout As Integer = 5000

    Private _is_running As Boolean = False
    Public ReadOnly Property IsRunning As Boolean
        Get
            Return Me._is_running
        End Get
    End Property

    Public Sub New(target As IPAddress, ttl As Integer)
        Me._target = target
        Me._ttl = ttl
    End Sub

    Public Sub StartPing()
        Me._thr.Start()
    End Sub

    Public Sub Abort()
        If Me._thr.ThreadState = Threading.ThreadState.Running Then Me._thr.Abort()
    End Sub

    Private Sub _worker_task()
        Dim reply As PingReply = Nothing
        Try
            Me._is_running = True
            Dim options As New PingOptions(Me._ttl, False)
            reply = Me._ping.Send(Me._target, Me.Timeout, _PAYLOAD, options)
        Catch tha_ex As Threading.ThreadAbortException
        Finally
            Me._is_running = False
            RaiseEvent PingEvent(
                Me,
                New PingEventArgs(
                    New PingResult(
                        reply.Status,
                        Me._target,
                        reply.Address,
                        Me._ttl,
                        If(reply.Options Is Nothing, 0, reply.Options.Ttl),
                        reply.RoundtripTime
                    )
                )
            )
        End Try
    End Sub

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not Me._is_disposed Then
            If disposing Then
                If Me.IsRunning Then Me.Abort()
                Me._ping.Dispose()
            End If
        End If
        Me._is_disposed = True
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        Dispose(True)
    End Sub

End Class
