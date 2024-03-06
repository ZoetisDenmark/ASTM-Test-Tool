
Imports System.Runtime.InteropServices 'DllImport

Friend Class UnmanagedCalls
    <DllImport("Win32HMAC.dll", CallingConvention:=CallingConvention.Winapi)>
    Public Shared Function CalculateSha256Mac(messages As Byte(), hexStr As Byte(), ByVal hexStrLength As UInteger, ByVal keyIndex As UInteger) As Integer
    End Function

    'Public Static extern int CalculateSha256Mac(Byte[] messages, Byte[] hexStr, uint hexStrLength, uint keyIndex);       

End Class
