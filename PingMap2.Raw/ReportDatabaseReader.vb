Imports PingMap2.Engine

Public Class ReportDatabaseReader

    Private Shared ReadOnly _MAGIC As Byte() = Text.Encoding.ASCII.GetBytes("PING")
    Private Shared ReadOnly _VERSION As Byte() = {1, 0}

    Public Shared Iterator Function GetReports(folder As String) As IEnumerable(Of Report)
        For Each db_file As String In IO.Directory.GetFiles(folder, "ping_db_*.bin")
            Using f As New IO.FileStream(db_file, IO.FileMode.Open, IO.FileAccess.Read, IO.FileShare.Read)
                Using r As New IO.BinaryReader(f, Text.Encoding.ASCII, True)
                    If Not r.ReadBytes(4).SequenceEqual(_MAGIC) Then Throw New Exception("Invalid file format.")
                    If Not (r.ReadByte() = _VERSION(0) AndAlso r.ReadByte() <= _VERSION(1)) Then Throw New Exception("Reader version too old.")
                    r.ReadInt32()
                End Using
                Using r As New IO.BinaryReader(f, Text.Encoding.UTF8, True)
                    While Not f.Position >= f.Length
                        If Not r.ReadInt32.Equals(&HFF00) Then Throw New Exception("File stream corrupted.")
                        Yield Report.FromBinStream(r)
                    End While
                End Using
            End Using
        Next
    End Function

End Class
