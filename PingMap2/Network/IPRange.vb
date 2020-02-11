Imports System.Net

Namespace Network
    Public Class IPRange

        Public ReadOnly Property NetMask As IPAddress
        Public ReadOnly Property LowerAddress As IPAddress
        Public ReadOnly Property UpperAddress As IPAddress
        Public ReadOnly Property Bits As Integer

        Private Sub New(netmask As IPAddress, lowerAddress As IPAddress, upperAddress As IPAddress, bits As Integer)
            Me.NetMask = netmask
            Me.LowerAddress = lowerAddress
            Me.UpperAddress = upperAddress
            Me.Bits = bits
        End Sub

        Public Shared Function FromCIDR(baseAddress As IPAddress, bits As Integer) As IPRange
            Dim netmask_data As UInteger = ReverseBytes((CULng(UInteger.MaxValue) << (32 - bits)) And UInteger.MaxValue)
            Dim lower_data As UInteger = GetAddress(baseAddress) And netmask_data
            Dim upper_data As UInteger = lower_data + (netmask_data Xor UInteger.MaxValue)
            Return New IPRange(New IPAddress(netmask_data), New IPAddress(lower_data), New IPAddress(upper_data), bits)
        End Function

        Public Shared Function Parse(text As String) As IPRange
            Dim parts As String() = text.Split("/")
            Return FromCIDR(IPAddress.Parse(parts.First), Integer.Parse(parts.Last))
        End Function

        Private Shared Function ReverseBytes(value As UInteger) As UInteger
            Return (value And &HFFUI) << 24 Or (value And &HFF00UI) << 8 Or (value And &HFF0000UI) >> 8 Or (value And &HFF000000UI) >> 24
        End Function

        Private Shared Function GetAddress(ip As IPAddress) As UInteger
            Return BitConverter.ToUInt32(ip.GetAddressBytes(), 0)
        End Function

        Public Function Contains(address As IPAddress) As Boolean
            Return GetAddress(Me.LowerAddress).Equals(GetAddress(Me.NetMask) And GetAddress(address))
        End Function

    End Class

End Namespace
