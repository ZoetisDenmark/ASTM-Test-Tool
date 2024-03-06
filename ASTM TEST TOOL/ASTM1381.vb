Option Strict Off
Option Explicit On
Imports System.Net.Sockets
Imports System.Net
Imports System


Friend Class ASTM1381
	'**********************************************************************************
	
	'File:  ASTM1381.cls
	
	'Description:  This module contains the ASTM 1381 class.
	
	'Compiler:  This module is part of a project that is designed to be edited and compiled
	'in Visual Basic 6.0.  Choose "File->Make" from within the IDE to make the program.
	
	'$History: ASTM1381.cls $
	
	
	' constants
    'Private Const STX As Byte = &H2s
    'Private Const ETX As Byte = &H3s
    'Private Const EOT As Byte = &H4s
    '   Private Const ENQ As Byte = &H5
    'Private Const ACK As Byte = &H6s
    '   Private Const NAK As Byte = &H15S

    ' nonprintable ASCII characters
    Private Const NUL As Byte = &H0
    Private Const SOH As Byte = &H1
    Private Const STX As Byte = &H2
    Private Const ETX As Byte = &H3
    Private Const EOT As Byte = &H4
    Private Const ENQ As Byte = &H5
    Private Const ACK As Byte = &H6
    Private Const HT As Byte = &H9
    Private Const NAK As Byte = &H15

    Private Const RXTIMEOUT As Integer = 30000 '30sec
    Private Const TXTIMEOUT As Integer = 15000 '15 seconds
    Private Const TXCONTIMEOUT As Integer = 20000 '20sec not in use
    Private Const TXBUSYTIMEOUT As Integer = 10000 '10sec not in use
    Private Const TXMAXRETRY As Integer = 6

    'Mode of Communication
    Private Const SERIALCOMM As Byte = &H0 'Serial Communication using RS232 
    Private Const TCPIPCOMM As Byte = &H1 'TCP/IP Programming
    Private ReadContinue As Boolean
    Public commType As Byte


    ' ASTM states
    Protected Friend Enum enum_ASTMState
        abxIdle = 0
        abxRXWaiting = 1
        abxTXWaitingCxn = 2
        abxTXWaitingACK = 3
    End Enum

    ' member variable(s)
    Protected Friend intState_m As enum_ASTMState
    Private lngFrameNum_m As Integer
	Private astrFrame_m() As String
	Private lngTxFrameIndex_m As Integer
    Private lngTxMaxFrameIndex_m As Integer
    Private lngTxRetry_m As Integer
	Private WithEvents tmrTimeout_m As ccrpTimers6.ccrpCountdown
    Private WithEvents comSerial_m As MSCommLib.MSComm
    Private TcpIpListener As TcpListener
    Private HostSocket As Socket
    Private WithEvents tmrPollClientConnection As System.Timers.Timer
	
	' class events
	Public Event OnUpdate(ByVal update As String)
    Public Event OnMessage(ByVal message As String, ByRef messageError As Boolean)
    Public Event TransmitEnable(ByVal state As Boolean)
	Public Event OnError()

    '***********************************************************************

    'PROCEDURE:    comSerial_m_OnComm()

    'DESCRIPTION:  Event that occurs when the serial port has activity

    'PARAMETERS:   N/A

    'RETURNED:    N/A

    '***********************************************************************    

    Private Sub comSerial_m_OnComm() Handles comSerial_m.OnComm
        On Error GoTo ErrTrap
        If (comSerial_m.CommEvent = MSCommLib.OnCommConstants.comEvReceive) Then
            ProcessRxData(SERIALCOMM)
        End If
        Exit Sub
ErrTrap:
        Err.Source = Err.Source & " | ASTM1381.comSerial_m_OnComm"
        RaiseEvent OnError()
    End Sub

    '***********************************************************************

    'PROCEDURE:    ProcessRxData

    'DESCRIPTION:  Nalini

    'PARAMETERS:   N/A

    'RETURNED:    N/A

    '***********************************************************************

    Private Sub ProcessRxData(ByVal type As Byte)
        On Error GoTo ErrTrap

        Static buffer As String
        Dim parseBuffer As Boolean

        Dim numBytes As Int32

        ' for now, only check the receive events
        ' keep adding to the buffer until a complete message is found
        If (type = SERIALCOMM) Then
            buffer = buffer & comSerial_m.Input

        Else
            numBytes = HostSocket.Available
            Dim Data(numBytes) As Byte
            If (numBytes <> 0) Then 'read from the socket only if the available bytes are not equal to zero
                HostSocket.Receive(Data)
                buffer = buffer & System.Text.ASCIIEncoding.ASCII.GetString(Data)
            End If
        End If
            'Debug.Print buffer & vbCrLf

        Do
            Select Case intState_m
                Case enum_ASTMState.abxIdle
                    ' the only meaningful message when in idle mode is an 'ENQ'
                    If (InStr(buffer, Chr(ENQ)) <> 0) Then

                        RaiseEvent OnUpdate(DateTime.Now.ToString("yyyyMMddHHmmss") & ", " & My.Resources.ResourceManager.GetString("str" + CStr(RXENQTXT)))

                        ' acknowledge the 'ENQ'
                        'comSerial_m.Output = ACK.ToString

                        If (type = SERIALCOMM) Then
                            comSerial_m.Output = GetDataToTransmit(ACK)
                        Else
                            TransmitDataOverNetwork(GetDataToTransmit(ACK))
                            'HostSocket.Send(System.Text.Encoding.ASCII.GetBytes(GetDataToTransmit(ACK)))
                        End If

                        RaiseEvent OnUpdate(DateTime.Now.ToString("yyyyMMddHHmmss") & ", " & My.Resources.ResourceManager.GetString("str" + CStr(TXACKTXT)))

                        ' set a countdown timer for the first frame and wait for it
                        lngFrameNum_m = 1
                        tmrTimeout_m.Duration = RXTIMEOUT
                        tmrTimeout_m.Enabled = True
                        intState_m = enum_ASTMState.abxRXWaiting

                        RaiseEvent OnUpdate(My.Resources.ResourceManager.GetString("str" + CStr(WAITFRAMETXT)))
                    End If

                    ' clear the buffer
                    buffer = ""
                    parseBuffer = False

                Case enum_ASTMState.abxRXWaiting
                    ' look for the end of frame carriage return line feed pair
                    If (InStr(buffer, vbCrLf) <> 0) Then

                        ' in case a countdown timer was running
                        tmrTimeout_m.Enabled = False

                        RaiseEvent OnUpdate(My.Resources.ResourceManager.GetString("str" + CStr(RXFRAMETXT)) & buffer)
                        RaiseEvent OnUpdate(My.Resources.ResourceManager.GetString("str" + CStr(PARSEFRAMETXT)))

                        ' try to parse the frame
                        If (ParseFrame(buffer) = True) Then

                            ' acknowledge the good frame
                            'comSerial_m.Output = ACK.ToString

                            If (type = SERIALCOMM) Then
                                comSerial_m.Output = GetDataToTransmit(ACK)
                            Else
                                TransmitDataOverNetwork(GetDataToTransmit(ACK))
                                'HostSocket.Send(System.Text.Encoding.ASCII.GetBytes(GetDataToTransmit(ACK)))
                            End If

                            RaiseEvent OnUpdate(DateTime.Now.ToString("yyyyMMddHHmmss") & ", " & My.Resources.ResourceManager.GetString("str" + CStr(TXACKTXT)))

                            Call IncrementFrameNum()
                        Else

                            ' the frame was bad so 'NAK' it
                            'comSerial_m.Output = NAK.ToString

                            If (type = SERIALCOMM) Then
                                comSerial_m.Output = GetDataToTransmit(NAK)
                            Else
                                TransmitDataOverNetwork(GetDataToTransmit(NAK))
                                'HostSocket.Send(System.Text.Encoding.ASCII.GetBytes(GetDataToTransmit(NAK)))
                            End If
                            RaiseEvent OnUpdate(DateTime.Now.ToString("yyyyMMddHHmmss") & ", " & My.Resources.ResourceManager.GetString("str" + CStr(TXNAKTXT)))
                        End If

                        ' regardless of the frame's disposition, set a countdown timer
                        ' and wait for a new frame or an 'EOT'
                        tmrTimeout_m.Duration = RXTIMEOUT
                        tmrTimeout_m.Enabled = True

                        ' clear the buffer
                        buffer = ""
                        parseBuffer = False

                        RaiseEvent OnUpdate(My.Resources.ResourceManager.GetString("str" + CStr(WAITFRAMETXT)))

                        ' the communication is over
                    ElseIf (InStr(buffer, Chr(EOT)) <> 0) Then

                        ' in case a countdown timer was running
                        tmrTimeout_m.Enabled = False

                        RaiseEvent OnUpdate(My.Resources.ResourceManager.GetString("str" + CStr(RXEOTTXT)))

                        ' return the state machine to idle
                        intState_m = enum_ASTMState.abxIdle

                        ' there could be more to parse
                        parseBuffer = True

                        RaiseEvent OnUpdate(My.Resources.ResourceManager.GetString("str" + CStr(IDLETXT))) 'Idle

                        RaiseEvent TransmitEnable(True) 'enable Transmit button after this transmission ended
                    Else
                        ' there's nothing to parse yet
                        parseBuffer = False
                    End If

                    ' waiting for a connection
                Case enum_ASTMState.abxTXWaitingCxn
                    ' in case a countdown timer was running
                    tmrTimeout_m.Enabled = False

                    ' look for the 'ACK'
                    If (InStr(buffer, Chr(ACK)) <> 0) Then

                        RaiseEvent OnUpdate(DateTime.Now.ToString("yyyyMMddHHmmss") & ", " & My.Resources.ResourceManager.GetString("str" + CStr(RXACKTXT))) 'Received 'ACK'

                        ' prepare for sending frames
                        lngTxRetry_m = 0

                        Call TxFrame()

                        ' wait for the response
                        intState_m = enum_ASTMState.abxTXWaitingACK

                    Else
                        ' anything other than an 'ACK' means contention or analyzer is busy
                        ' for now, just go back to idle
                        intState_m = enum_ASTMState.abxIdle

                        RaiseEvent OnUpdate(My.Resources.ResourceManager.GetString("str" + CStr(TXBUSYTXT)))
                        RaiseEvent OnUpdate(My.Resources.ResourceManager.GetString("str" + CStr(IDLETXT))) 'Idle
                    End If

                    ' clear the buffer
                    buffer = ""
                    parseBuffer = False

                    ' waiting for response
                Case enum_ASTMState.abxTXWaitingACK
                    ' in case a countdown timer was running
                    tmrTimeout_m.Enabled = False

                    ' look for the 'ACK' or 'EOT'
                    ' for now treat 'EOT' as 'ACK'
                    If ((InStr(buffer, Chr(ACK)) <> 0) Or (InStr(buffer, Chr(EOT)) <> 0)) Then

                        RaiseEvent OnUpdate(DateTime.Now.ToString("yyyyMMddHHmmss") & ", " & My.Resources.ResourceManager.GetString("str" + CStr(RXACKTXT))) 'Received 'ACK'

                        ' reset the retry count
                        lngTxRetry_m = 0

                        ' go to the next frame index
                        lngTxFrameIndex_m = lngTxFrameIndex_m + 1

                        ' see if there are more frames to send
                        If (lngTxFrameIndex_m <= lngTxMaxFrameIndex_m) Then

                            Call TxFrame() 'Transmit frame
                        Else
                            ' no more frames so end transmission
                            'comSerial_m.Output = EOT.ToString

                            If (type = SERIALCOMM) Then
                                comSerial_m.Output = GetDataToTransmit(EOT)
                            Else
                                TransmitDataOverNetwork(GetDataToTransmit(EOT))
                                'HostSocket.Send(System.Text.Encoding.ASCII.GetBytes(GetDataToTransmit(EOT)))
                            End If
                            RaiseEvent OnUpdate(My.Resources.ResourceManager.GetString("str" + CStr(TXEOTTXT)))

                            intState_m = enum_ASTMState.abxIdle

                            RaiseEvent OnUpdate(My.Resources.ResourceManager.GetString("str" + CStr(IDLETXT))) 'Idle

                            RaiseEvent TransmitEnable(True) 'enable Transmit button after send Order to analyzer (Sending 'EOT' but not get response back from analyzer. -- Snow 3/11/2016
                        End If

                        ' there was a problem with the frame
                    Else
                            lngTxRetry_m = lngTxRetry_m + 1

                            ' see if a retry is possible
                            If (lngTxRetry_m < TXMAXRETRY) Then
                            RaiseEvent OnUpdate(My.Resources.ResourceManager.GetString("str" + CStr(TXFRAMEAGAINTXT)))

                            Call TxFrame()
                            Else
                                ' too many retries so end transmission
                                'comSerial_m.Output = EOT.ToString
                                If (type = SERIALCOMM) Then
                                    comSerial_m.Output = GetDataToTransmit(EOT)
                                Else
                                    TransmitDataOverNetwork(GetDataToTransmit(EOT))
                                    'HostSocket.Send(System.Text.Encoding.ASCII.GetBytes(GetDataToTransmit(EOT)))
                                End If

                            RaiseEvent OnUpdate(My.Resources.ResourceManager.GetString("str" + CStr(MAXRETRYTXT)))
                            RaiseEvent OnUpdate(My.Resources.ResourceManager.GetString("str" + CStr(TXEOTTXT)))

                            intState_m = enum_ASTMState.abxIdle

                            RaiseEvent OnUpdate(My.Resources.ResourceManager.GetString("str" + CStr(IDLETXT))) 'Idle

                            RaiseEvent TransmitEnable(True) 'enable Transmit button after this transmission finished. -- Snow 3/9/2016
                            End If
                        End If


                    ' clear the buffer
                    buffer = ""
                    parseBuffer = False

                    ' there should never be an unknown state, but just in case
                Case Else
                    ' in case a countdown timer was running
                    tmrTimeout_m.Enabled = False

                    ' return the state machine to idle
                    intState_m = enum_ASTMState.abxIdle

                    RaiseEvent OnUpdate(My.Resources.ResourceManager.GetString("str" + CStr(IDLETXT))) 'Idle

                    ' clear the buffer
                    buffer = ""
                    parseBuffer = False

            End Select

            ' keep looping while there's more to parse
        Loop While (parseBuffer = True)

            Exit Sub

ErrTrap:
            Err.Source = Err.Source & " | ASTM1381.ProcessRxData"
            RaiseEvent OnError()
    End Sub

    '***********************************************************************

    'PROCEDURE:    tmrPollClientConnection

    'DESCRIPTION:  Event that occurs when the timer runs out. Every time
    'interval, the code checks if there is a client connected to this
    'server

    'PARAMETERS:   N/A

    'RETURNED:    N/A

    '***********************************************************************
    Private Sub tmrPollClientConnection_Elapsed(ByVal sender As Object, ByVal e As System.Timers.ElapsedEventArgs) Handles tmrPollClientConnection.Elapsed
        On Error GoTo ErrTrap
        'RaiseEvent OnUpdate(My.Resources.ResourceManager.GetString("str" + CStr(STARTTXT)))
        CheckTcpListener()
        Exit Sub
ErrTrap:
        Err.Source = Err.Source & " | ASTM1381.tmrPollClientConnection_Elapsed"
        RaiseEvent OnError()
    End Sub
    'Public Sub DoAcceptSocketCallback(ByVal ar As IAsyncResult)
    '    RaiseEvent OnUpdate("Got it...")
    'End Sub

    '***********************************************************************

    'PROCEDURE:   CheckTcpListener

    'DESCRIPTION:  this routine checks to see if there is any client on this
    'port. If yes, then the client is connected and the port is now polled
    'continuously for available data until the socked is disconnected

    'PARAMETERS:   N/A

    'RETURNED:    N/A

    '***********************************************************************
    Private Sub CheckTcpListener()
        'On Error GoTo ErrTrap
        Try
            ' This method is called every second when _Timer is running. 
            Dim portNumber As String
            Dim remotePort As System.Net.IPEndPoint
            Dim remoteIpAddr As System.Net.IPEndPoint
            Dim remoteIpAddress As String

            ' Check the TcpListner Pending Property 
            If ((TcpIpListener.Pending = True) And (IsNothing(TcpIpListener) = False)) Then
                ' TcpListner has pending connection; pass the connection to HostSocket 
                ' Call the AcceptSocket function on TcpIpListner; 
                ' assign the return value (a connection) to HostSocket. 
                HostSocket = TcpIpListener.AcceptSocket

                'now the connection is established between the client and the server, so enable the Transmit button
                RaiseEvent TransmitEnable(True)
                Me.tmrPollClientConnection.Stop() ' stop the timer since client connection is establised
                RaiseEvent OnUpdate("Client Connected...")

                'get the remote analyzer or client's Port Number
                remotePort = HostSocket.RemoteEndPoint
                portNumber = remotePort.Port.ToString()

                'get the remote analyzer or client's IP Address
                remoteIpAddr = HostSocket.RemoteEndPoint
                remoteIpAddress = remoteIpAddr.Address.ToString()

                RaiseEvent OnUpdate("Analyzer IP Address: " & remoteIpAddress)
                RaiseEvent OnUpdate("Analyzer Port: " & portNumber)

                ' Call the Socket Poll method to determine if the status of HostSocket 
                ' is SelectRead which means data is available to read. 
                While (HostSocket.Connected)
                    If (HostSocket.Poll(1, SelectMode.SelectRead)) Then 'returns true if bytes available or if remote client socked closed
                        If (HostSocket.Available > 0) Then ' bytes available to read
                            ProcessRxData(TCPIPCOMM)
                        Else
                            RaiseEvent TransmitEnable(False)
                            RaiseEvent OnUpdate("Client Disconnected...")
                            HostSocket.Disconnect(True)
                            Exit While
                        End If
                    End If
                End While

                RaiseEvent OnUpdate("Waiting for client connection again...")
                tmrPollClientConnection.Start() 'start the timer again and look for any connected clients 
            Else
                'RaiseEvent OnUpdate("Waiting for a client connection ....")
            End If
            Exit Sub

        Catch ex As ObjectDisposedException
            'do nothing
        End Try
        'ErrTrap:
        '        Err.Source = Err.Source & " | ASTM1381.CheckTcpListener"
        'RaiseEvent OnError()
    End Sub

    '***********************************************************************

    'PROCEDURE:    tmrTimeout_m_Timer()

    'DESCRIPTION:  Event that occurs when the timer runs out

    'PARAMETERS:   N/A

    'RETURNED:    N/A

    '***********************************************************************
    Private Sub tmrTimeout_m_Timer() Handles tmrTimeout_m.Timer
        On Error GoTo ErrTrap

        RaiseEvent OnUpdate(My.Resources.ResourceManager.GetString("str" + CStr(TIMEOUTTXT))) 'str201=Timeout waiting for Piccolo

        If ((intState_m = enum_ASTMState.abxTXWaitingACK) Or (intState_m = enum_ASTMState.abxTXWaitingCxn)) Then 'when waiting for ACK or Waiting for Transmit
            'comSerial_m.Output = EOT.ToString

            If (IsNothing(comSerial_m) = False) Then
                comSerial_m.Output = GetDataToTransmit(EOT)
            Else
                TransmitDataOverNetwork(GetDataToTransmit(EOT))
                'HostSocket.Send(System.Text.Encoding.ASCII.GetBytes(GetDataToTransmit(EOT)))
            End If


            RaiseEvent OnUpdate(My.Resources.ResourceManager.GetString("str" + CStr(TXEOTTXT))) 'str121=Sending EOT
        End If

        ' a timeout occurred so return the state machine back to idle
        intState_m = enum_ASTMState.abxIdle

        RaiseEvent OnUpdate(My.Resources.ResourceManager.GetString("str" + CStr(IDLETXT))) 'Idle

        RaiseEvent TransmitEnable(True) 'enable Transmit button after timeout from previous transmission so can Transmit next. -- Snow 3/9/2016
        Exit Sub
ErrTrap:
        Err.Source = Err.Source & " | ASTM1381.tmrTimeout_m_Timer"
        RaiseEvent OnError()
        RaiseEvent TransmitEnable(True)
    End Sub

    '***********************************************************************

    'FUNCTION:    TransmitData()

    'DESCRIPTION:  Transmit the data over the network using TCPIP

    'PARAMETERS:   N/A

    'RETURNED:    N/A

    '***********************************************************************

    Private Sub TransmitDataOverNetwork(ByVal data As String)
        'On Error GoTo ErrTrap
        Try
            HostSocket.Send(System.Text.Encoding.ASCII.GetBytes(data))
        Catch e As SocketException
            RaiseEvent OnUpdate("Unable to Transmit. Error occured when attempting to access the socket.")
        Catch e1 As ObjectDisposedException
            RaiseEvent OnUpdate("Unable to Transmit. The socket has been closed.")
        Catch e2 As SystemException
            RaiseEvent OnUpdate("Not able to Transmit. Error occured.")
        End Try


        'Exit Sub
        'ErrTrap:
        ' Call Err.Raise(Err.Number, Err.Source & " | ASTM1381.TransmitDataOverNetwork", Err.Description)
    End Sub
    '***********************************************************************

    'FUNCTION:    ParseFrame()

    'DESCRIPTION:  Parses the frame from the analyzer

    'PARAMETERS:   inFrame - the frame from the analyzer

    'RETURNED:    whether the frame was okay

    '***********************************************************************
    Private Function ParseFrame(ByVal inFrame As String) As Boolean
        On Error GoTo ErrTrap
        Dim etxPos As Integer
        Dim checkSumPos As Integer
        Dim msgStartPos As Integer
        Dim msgEndPos As Integer
        Dim frmNumPos As Integer
        Dim calcCheckSum As Integer
        Dim repCheckSum As Integer
        Dim messageError As Boolean

        ' find the start of the frame
        frmNumPos = InStr(inFrame, Chr(STX)) + 1

        ' frmNumPos = 1 means the 'STX' was not found
        If (frmNumPos = 1) Then
            ParseFrame = False
            Exit Function
        End If

        ' the message starts right after the frame number
        msgStartPos = frmNumPos + 1

        ' find the end of the frame
        etxPos = InStr(msgStartPos, inFrame, Chr(ETX))

        ' etxPos = 0 means the 'ETX' was not found
        If (etxPos = 0) Then
            ParseFrame = False
            Exit Function
        End If

        ' the message ends right before the 'ETX'
        msgEndPos = etxPos - 1

        ' the checksum is right after the 'ETX'
        checkSumPos = etxPos + 1

        ' calculate the checksum for the message
        calcCheckSum = CalculateCheckSum(Mid(inFrame, frmNumPos, etxPos - frmNumPos + 1))

        RaiseEvent OnUpdate(My.Resources.ResourceManager.GetString("str" + CStr(CALCCHECKSUMTXT)) & Hex(calcCheckSum))

        ' the checksum reported by the analyzer is in the frame
        repCheckSum = CByte("&H" & Mid(inFrame, checkSumPos, 1) & Mid(inFrame, checkSumPos + 1, 1))

        RaiseEvent OnUpdate(My.Resources.ResourceManager.GetString("str" + CStr(REPCHECKSUMTXT)) & Hex(repCheckSum))

        ' see if the computed checksum matches the frame checksum
        If (calcCheckSum = repCheckSum) Then
            ParseFrame = True

            ' raise the message event
            RaiseEvent OnMessage((Mid(inFrame, msgStartPos, msgEndPos - msgStartPos + 1)), messageError)

            ' check for a message error
            If (messageError = True) Then
                ParseFrame = False
            End If
        Else
            RaiseEvent OnUpdate(My.Resources.ResourceManager.GetString("str" + CStr(FRAMEERRTXT)))

            ParseFrame = False
        End If

        Exit Function
ErrTrap:
        Call Err.Raise(Err.Number, Err.Source & " | ASTM1381.ParseFrame", Err.Description)
    End Function

    '***********************************************************************

    'FUNCTION:    BuildFrame()

    'DESCRIPTION:  Builds a frame

    'PARAMETERS:    InFrameNum - the frame number to use
    '               inMessage - the message

    'RETURNED:    the frame

    '***********************************************************************
    Private Function BuildFrame(ByVal inFrameNum As Integer, ByVal inMessage As String) As String
        On Error GoTo ErrTrap
        Dim checkSum As Integer
        Dim hexCheckSum As String

        ' RaiseEvent OnUpdate(My.Resources.ResourceManager.GetString("str" + CStr(BUILDFRAMETXT))) 'do not display Build Frame Snow

        ' first build the parts of the frame included in the checksum
        BuildFrame = CStr(inFrameNum) & inMessage & Chr(ETX)

        checkSum = CalculateCheckSum(BuildFrame)

        ' get the hex version of the checksum
        hexCheckSum = Hex(checkSum)

        ' pad the checksum with a zero if necessary
        If (Len(hexCheckSum) = 1) Then
            hexCheckSum = "0" & hexCheckSum
        End If

        ' finish the frame
        BuildFrame = Chr(STX) & BuildFrame & Left(hexCheckSum, 1) & Right(hexCheckSum, 1) & vbCrLf

        Exit Function
ErrTrap:
        Call Err.Raise(Err.Number, Err.Source & " | ASTM1381.BuildFrame", Err.Description)
    End Function


    '***********************************************************************

    'FUNCTION:    CalculateCheckSum()

    'DESCRIPTION:  Calculates a check sum

    'PARAMETERS:   inString - the string to calculate the checksum on

    'RETURNED:    the checksum

    '***********************************************************************
    Private Function CalculateCheckSum(ByVal inString As String) As Integer
        On Error GoTo ErrTrap
        Dim index As Integer
        Dim maxIndex As Integer

        ' see how many characters are in the string
        maxIndex = Len(inString)

        ' initialize the checksum
        CalculateCheckSum = 0

        ' cycle through each character and calculate the checksum
        For index = 1 To maxIndex
            CalculateCheckSum = CalculateCheckSum + Asc(Mid(inString, index, 1))
        Next index

        ' the calculated checksum is modulo 256
        CalculateCheckSum = CalculateCheckSum Mod 256

        Exit Function
ErrTrap:
        Call Err.Raise(Err.Number, Err.Source & " | ASTM1381.CalculateCheckSum", Err.Description)
    End Function

    '***********************************************************************

    'PROCEDURE:    IncrementFrameNum()

    'DESCRIPTION:  Increments the frame number

    'PARAMETERS:   N/A

    'RETURNED:    N/A

    '***********************************************************************
    Private Sub IncrementFrameNum()
        On Error GoTo ErrTrap

        lngFrameNum_m = (lngFrameNum_m + 1) Mod 8

        Exit Sub
ErrTrap:
        Call Err.Raise(Err.Number, Err.Source & " | ASTM1381.IncrementFrameNum", Err.Description)
    End Sub

    '***********************************************************************

    'PROCEDURE:    TxFrame()

    'DESCRIPTION:  Transmits a frame

    'PARAMETERS:   N/A

    'RETURNED:    N/A

    '***********************************************************************
    Private Sub TxFrame()
        On Error GoTo ErrTrap

        RaiseEvent OnUpdate(My.Resources.ResourceManager.GetString("str" + CStr(TXFRAMETXT)) & astrFrame_m(lngTxFrameIndex_m)) 'Sending frame ??? Snow

        If (IsNothing(comSerial_m) = False) Then
            comSerial_m.Output = astrFrame_m(lngTxFrameIndex_m)
        Else
            TransmitDataOverNetwork(astrFrame_m(lngTxFrameIndex_m))
            'HostSocket.Send(System.Text.Encoding.ASCII.GetBytes(astrFrame_m(lngTxFrameIndex_m)))
        End If

        ' set a countdown timer for the response and wait for it
        tmrTimeout_m.Duration = TXTIMEOUT
        tmrTimeout_m.Enabled = True

        RaiseEvent OnUpdate(My.Resources.ResourceManager.GetString("str" + CStr(WAITRESPONSETXT)))

        Exit Sub
ErrTrap:
        Call Err.Raise(Err.Number, Err.Source & " | ASTM1381.TxFrame", Err.Description)
    End Sub

    '***********************************************************************

    'PROCEDURE:    StartStateMachine()

    'DESCRIPTION:  Allows other objects to engage the ASTM state machine

    'PARAMETERS:   inPort - the serial port to use

    'RETURNED:    N/A

    '***********************************************************************
    Public Sub StartStateMachine(ByVal inPort As Short) 'As Byte
        On Error GoTo ErrTrap
        comSerial_m = New MSCommLib.MSComm
        tmrTimeout_m = New ccrpTimers6.ccrpCountdown

        ' set up the serial port
        comSerial_m._CommPort = inPort '??? why not take COM20??? COM# only allow 1 to 16, default is 1. --Snow 3/10/2016
        comSerial_m.Settings = "9600,N,8,1" 'baud rate, parity, data bits, and stop bits as a string 
        comSerial_m.RThreshold = 1

        ' the state machine starts at idle
        intState_m = enum_ASTMState.abxIdle
        comSerial_m.PortOpen = True

        ' assign property value for btn Stop Transmit use
        commType = SERIALCOMM

        RaiseEvent OnUpdate(My.Resources.ResourceManager.GetString("str" + CStr(STARTTXT)))
        RaiseEvent OnUpdate(My.Resources.ResourceManager.GetString("str" + CStr(IDLETXT))) 'Idle
        Exit Sub
ErrTrap:
        If (comSerial_m.PortOpen = False) Then
            Call Err.Raise(Err.Number, "", "Com Port not found in ASTM1381") 'if inPort is not correct/active display message "Com Port not found"
        Else

            Call Err.Raise(Err.Number, Err.Source & " | ASTM1381.StartStateMachine", Err.Description) 'if inPort is not correct/active display message "Com Port not found"
        End If
    End Sub

    '***********************************************************************

    'PROCEDURE:    StartStateMachine()

    'DESCRIPTION:  Allows other objects to engage the ASTM state machine

    'PARAMETERS:   inAddress - the IPAddress 

    'RETURNED:    inPort - Port Number to use

    '***********************************************************************
    Public Sub StartStateMachine(ByVal inAddress As String, ByVal inPort As String)
        On Error GoTo ErrTrap

        Dim hostIPAddress As IPAddress = System.Net.IPAddress.Parse(inAddress)
        TcpIpListener = New TcpListener(hostIPAddress, Int32.Parse(inPort))

        tmrTimeout_m = New ccrpTimers6.ccrpCountdown
        tmrPollClientConnection = New System.Timers.Timer
        TcpIpListener.Start() 'start the timer to poll for any client connections

        'TcpIpListener.BeginAcceptSocket(New AsyncCallback(AddressOf DoAcceptSocketCallback), TcpIpListener)

        tmrPollClientConnection.Interval = 1000 'time interval to poll the network for any clients connected to this port 1sec
        tmrPollClientConnection.Start() 'start the time to keep polling the net every 1sec

        ' the state machine starts at idle
        intState_m = enum_ASTMState.abxIdle
        ' assign property value for btn Stop Transmit use
        commType = TCPIPCOMM

        RaiseEvent OnUpdate(My.Resources.ResourceManager.GetString("str" + CStr(STARTTXT)))
        RaiseEvent OnUpdate(My.Resources.ResourceManager.GetString("str" + CStr(IDLETXT)))
        RaiseEvent OnUpdate("Starting Server in Listening mode ....")
        RaiseEvent OnUpdate("Waiting for a client connection ....")
        Exit Sub
ErrTrap:
        Call Err.Raise(Err.Number, Err.Source & " | ASTM1381.StartTCPIPStateMachine", Err.Description)
    End Sub

    '***********************************************************************

    'PROCEDURE:    StopStateMachine()

    'DESCRIPTION:  Allows other objects to stop the ASTM state machine

    'PARAMETERS:   N/A Nalini

    'RETURNED:    N/A

    '***********************************************************************
    Public Sub StopStateMachine(ByVal type As Byte)
        On Error GoTo ErrTrap

        ' return the state machine to an idle state
        If (type = SERIALCOMM) Then
            comSerial_m.PortOpen = False
        ElseIf (type = TCPIPCOMM) Then
            'ReadContinue = False
            Me.tmrPollClientConnection.Stop()

            If (IsNothing(HostSocket) = False) Then
                HostSocket.Close()
            End If
            TcpIpListener.Stop()
        End If

        intState_m = enum_ASTMState.abxIdle
        tmrTimeout_m.Enabled = False

        RaiseEvent OnUpdate(My.Resources.ResourceManager.GetString("str" + CStr(STOPTXT)))

        'UPGRADE_NOTE: Object comSerial_m may not be destroyed until it is garbage collected. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
        comSerial_m = Nothing
        TcpIpListener = Nothing

        'UPGRADE_NOTE: Object tmrTimeout_m may not be destroyed until it is garbage collected. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
        tmrTimeout_m = Nothing
        GC.Collect() 'nalini
        Exit Sub
ErrTrap:
        Call Err.Raise(Err.Number, Err.Source & " | ASTM1381.StopStateMachine", Err.Description)
        TcpIpListener.Stop()
    End Sub

    '***********************************************************************

    'PROCEDURE:    GetDataToTransmit()

    'DESCRIPTION:  Gets the data to be transmitted and returns the data after conversion to string

    'PARAMETERS:   data

    'RETURNED:    N/A

    '***********************************************************************
    Private Function GetDataToTransmit(ByVal data As Byte) As String
        On Error GoTo ErrTrap
        GetDataToTransmit = Chr(data)
        Exit Function
ErrTrap:
        Call Err.Raise(Err.Number, Err.Source & " | ASTM1381.GetDataToTransmit", Err.Description)
    End Function
    '***********************************************************************

    'PROCEDURE:    TransmitMessages()

    'DESCRIPTION:  Allows other objects to transmit messages

    'PARAMETERS:   inMessageList - the messages to transmit

    'RETURNED:    N/A

    '***********************************************************************
    Public Sub TransmitMessages(ByVal inMessageList() As String)
        On Error GoTo ErrTrap


        Dim index As Integer

        ' see how many messages to send
        lngTxMaxFrameIndex_m = UBound(inMessageList)

        ' prepare the frames array
        ReDim astrFrame_m(lngTxMaxFrameIndex_m)
        lngFrameNum_m = 1

        ' build each frame
        For index = 0 To lngTxMaxFrameIndex_m

            astrFrame_m(index) = BuildFrame(lngFrameNum_m, inMessageList(index))
            Call IncrementFrameNum()

        Next index

        ' prepare to send frames
        lngTxFrameIndex_m = 0

        'send out the 'ENQ'

        'check if the mode of communication is serial or TCPIP, before transmitting
        If (IsNothing(comSerial_m) = False) Then
            comSerial_m.Output = GetDataToTransmit(ENQ) '??? Output = exception Snow ??? send ENQ
        Else
            'ReadContinue = False nalini
            TransmitDataOverNetwork(GetDataToTransmit(ENQ))
            'HostSocket.Send(System.Text.Encoding.ASCII.GetBytes(GetDataToTransmit(ENQ)))
        End If

        'RaiseEvent OnUpdate(My.Resources.ResourceManager.GetString("str" + CStr(TXENQTXT)))
        RaiseEvent OnUpdate(DateTime.Now.ToString("yyyyMMddHHmmss") & ", " & My.Resources.ResourceManager.GetString("str" + CStr(TXENQTXT))) 'My.Resources.ResourceManager.GetString("str" + CStr(TXENQTXT))

        ' set a countdown timer for the connection and wait for it
        tmrTimeout_m.Duration = TXTIMEOUT
        tmrTimeout_m.Enabled = True
        intState_m = enum_ASTMState.abxTXWaitingCxn

        RaiseEvent OnUpdate(My.Resources.ResourceManager.GetString("str" + CStr(WAITRESPONSETXT)))


        Exit Sub
ErrTrap:
        Call Err.Raise(Err.Number, Err.Source & " | ASTM1381.TransmitMessages", Err.Description)
    End Sub

    '***********************************************************************

    'PROCEDURE:    StopTransmit()

    'DESCRIPTION:  To stop transmit in the middle of process

    'RETURNED:    N/A

    'Snow 4/5/2016
    '***********************************************************************
    Public Sub StopTransmit()
        On Error GoTo ErrTrap

        Static Buffer As String
        Dim parseBuffer As Boolean
        Dim numBytes As Int32

        ' for now, only check the receive events
        ' keep adding to the buffer until a complete message is found
        If (commType = SERIALCOMM) Then
            Buffer = Buffer & comSerial_m.Input
        Else
            numBytes = HostSocket.Available
            Dim Data(numBytes) As Byte
            If (numBytes <> 0) Then 'read from the socket only if the available bytes are not equal to zero
                HostSocket.Receive(Data)
                Buffer = Buffer & System.Text.ASCIIEncoding.ASCII.GetString(Data)
            End If
        End If

        ' in case a countdown timer was running
        tmrTimeout_m.Enabled = False
        ' return the state machine to idle
        intState_m = enum_ASTMState.abxIdle

        ' there could be more to parse
        parseBuffer = False

        RaiseEvent OnUpdate(My.Resources.ResourceManager.GetString("str" + CStr(IDLETXT))) 'Idle
        RaiseEvent TransmitEnable(True) 'enable Transmit button after this transmission ended

        ' clear the buffer
        Buffer = ""
        Exit Sub
ErrTrap:
        Call Err.Raise(Err.Number, Err.Source & " | ASTM1381.StopTransmit", Err.Description)
    End Sub
End Class