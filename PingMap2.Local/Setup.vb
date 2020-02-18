Public Class Setup

    Private Shared _Current As Setup

    Public Shared ReadOnly Property Instance As Setup
        Get
            If _Current Is Nothing Then _Current = New Setup
            Return _Current
        End Get
    End Property

    Public ReadOnly Property SessionName As String
    Public ReadOnly Property ThreadCount As Integer
    Public ReadOnly Property LogLevel As Logger.LogLevel

    Private Sub New()
        Dim path As String = IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "setup.dat")
        If Not IO.File.Exists(path) Then Throw New IO.FileNotFoundException()
        Using f As New IO.StreamReader(path, Text.Encoding.UTF8)
            While Not f.EndOfStream
                Dim line As String = f.ReadLine
                If line.Equals(String.Empty) Then Continue While
                Dim args As String() = line.Split(":")
                Select Case args.First.ToLower
                    Case "sessionname"
                        Me.SessionName = args.Last
                    Case "threadcount"
                        Me.ThreadCount = Integer.Parse(args.Last)
                    Case "loglevel"
                        Me.LogLevel = [Enum].Parse(GetType(Logger.LogLevel), args.Last)
                    Case Else
                        Logger.Instance.Log(Logger.LogLevel.Warning, "Unkown setup property: " & args.First)
                End Select
            End While
        End Using
    End Sub

End Class
