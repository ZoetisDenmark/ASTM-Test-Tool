Option Strict Off
Option Explicit On

Imports System.Runtime.InteropServices 'DllImport

Friend Class ASTM1394
	'**********************************************************************************
	
	'File:  ASTM1394.cls
	
	'Description:  This module contains the ASTM 1394 class.
	
	'Compiler:  This module is part of a project that is designed to be edited and compiled
	'in Visual Basic 6.0.  Choose "File->Make" from within the IDE to make the program.
	
	'$History: ASTM1394.cls $
	
	
	'constants
	Private Const HEADER As String = "H"
    Private Const ASTM1394DB As String = "ASTM1394Order.MDB"

    ' fields in the ASTM 1394 database
    Private Const RECTYPEIDFLD As String = "Record Type ID"
	Private Const TYPEIDFLD As String = "Type ID"
	Private Const NAMEFLD As String = "Name"
	Private Const ORDERFLD As String = "Order"
	Private Const VALUEFLD As String = "Value"
	
	' Tables in ASTM 1394 database
	Private Const RECTBL As String = "Record"
	Private Const FIELDTBL As String = "Field"
	
	' private member variables
	Private strRxDelimiter_m As String
	Private strTxDelimiter_m As String
	Private strRepeat_m As String
	Private strComponent_m As String
	Private strEscape_m As String

    Private WithEvents objStateMachine_m As ASTM1381
    Private cxnDB_m As ADODB.Connection
	
	' class events
    Public Event OnUpdate(ByVal update As String)
    Public Event TransmitEnable(ByVal mode As Boolean)
    Public Event OnError()
    Public Event OnComm()




    '***********************************************************************

    'PROPERTY GET:   RecTypeIDList()

    'DESCRIPTION: Allows other objects to get the record type IDs list

    'PARAMETERS:  N/A

    'RETURNED:    array of record type IDs

    '*********************************************************************
    Public ReadOnly Property RecTypeIDList() As String()
		Get
			On Error GoTo ErrTrap
			
			RecTypeIDList = GetFieldValueList(RECTBL, TYPEIDFLD,  , "[" & NAMEFLD & "] ASC")
			
			Exit Property
ErrTrap: 
			Call Err.Raise(Err.Number, Err.Source & " | ASTM1394.PropertyGet.RecTypeIDList", Err.Description)
		End Get
	End Property
	
	'***********************************************************************
	
	'PROPERTY GET:   RecName()
	
	'DESCRIPTION: Allows other objects to get the record name for a record type ID
	
	'PARAMETERS:  inRecTypeID - the type ID of record
	
	'RETURNED:    record name
	
	'*********************************************************************
	Public ReadOnly Property RecName(ByVal inRecTypeID As String) As String
		Get
			On Error GoTo ErrTrap
			
			RecName = GetFieldValueList(RECTBL, NAMEFLD, "[" & TYPEIDFLD & "] LIKE '" & inRecTypeID & "'")(0)
			
			Exit Property
ErrTrap: 
			Call Err.Raise(Err.Number, Err.Source & " | ASTM1394.PropertyGet.RecName", Err.Description)
		End Get
	End Property
	
	'***********************************************************************
	
	'PROPERTY GET:   RecFieldList()
	
	'DESCRIPTION: Allows other objects to get the record field list
	
	'PARAMETERS:  inRecTypeID - the type ID of record
	
	'RETURNED:    array of record fields
	
	'*********************************************************************
	Public ReadOnly Property RecFieldList(ByVal inRecTypeID As String) As String()
		Get
			On Error GoTo ErrTrap
			
			RecFieldList = GetFieldValueList(FIELDTBL, NAMEFLD, "[" & RECTYPEIDFLD & "] LIKE '" & inRecTypeID & "'", "[" & ORDERFLD & "] ASC")
			
			Exit Property
ErrTrap: 
			Call Err.Raise(Err.Number, Err.Source & " | ASTM1394.PropertyGet.RecFieldList", Err.Description)
		End Get
	End Property
	
	'***********************************************************************
	
	'PROPERTY GET:   RecValueList()
	
	'DESCRIPTION: Allows other objects to get the record value list
	
	'PARAMETERS:  inRecTypeID - the type ID of record
	
	'RETURNED:    array of record values
	
	'*********************************************************************
	Public ReadOnly Property RecValueList(ByVal inRecTypeID As String) As String()
		Get
			On Error GoTo ErrTrap
			
			RecValueList = GetFieldValueList(FIELDTBL, VALUEFLD, "[" & RECTYPEIDFLD & "] LIKE '" & inRecTypeID & "'", "[" & ORDERFLD & "] ASC")
			
			Exit Property
ErrTrap: 
			Call Err.Raise(Err.Number, Err.Source & " | ASTM1394.PropertyGet.RecValueList", Err.Description)
		End Get
	End Property
	
	'***********************************************************************
	
	'PROPERTY LET:   RecFieldValue()
	
	'DESCRIPTION: Allows other objects to change the value for a record field
	
	'PARAMETERS:  inRecTypeID - the type ID of record
	'             inRecField - the field in the record
	'             inRecValue - the new value for the field
	
	'RETURNED:    N/A
	
	'*********************************************************************
	Public WriteOnly Property RecFieldValue(ByVal inRecTypeID As String, ByVal inRecField As String) As String
		Set(ByVal Value As String)
			On Error GoTo ErrTrap
			Dim rs As ADODB.Recordset
			Dim strSQL As String
			
			' prepare the recordset object
			rs = New ADODB.Recordset
			rs.CursorLocation = ADODB.CursorLocationEnum.adUseClient
			
			' the SQL string retrieves the record to update from the table
			strSQL = "SELECT * FROM " & FIELDTBL & " WHERE ([" & RECTYPEIDFLD & "] LIKE '" & inRecTypeID & "') AND ([" & NAMEFLD & "] LIKE '" & inRecField & "')"
			
			Call rs.Open(strSQL, cxnDB_m, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockOptimistic, ADODB.CommandTypeEnum.adCmdText)
			
			' make sure a record was found
			If (rs.EOF = True) Then
				Call Err.Raise(COMPERR, "ASTM1394", My.Resources.ResourceManager.GetString("str" + CStr(EOFERRTXT)))
			End If
			
			' update the value
			rs.Fields(VALUEFLD).Value = Value
			Call rs.update()
			
			' clean up
			Call rs.Close()
			'UPGRADE_NOTE: Object rs may not be destroyed until it is garbage collected. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
            rs = Nothing
            GC.Collect() 'nalini
			Exit Property
ErrTrap: 
			'UPGRADE_NOTE: Object rs may not be destroyed until it is garbage collected. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
            rs = Nothing
            GC.Collect() 'nalini
			Call Err.Raise(Err.Number, Err.Source & " | ASTM1394.PropertyLet.RecFieldValue", Err.Description)
		End Set
	End Property
	
	'***********************************************************************
	
	'PROPERTY LET:   TxFieldDelimiter()
	
	'DESCRIPTION: Allows other objects to set the transmission field delimiter
	
	'PARAMETERS:  inTxFieldDelimiter - the transmission field delimiter
	
	'RETURNED:    N/A
	
	'*********************************************************************
	Public WriteOnly Property TxFieldDelimiter() As String
		Set(ByVal Value As String)
			On Error GoTo ErrTrap
			strTxDelimiter_m = Value
			Exit Property
ErrTrap: 
			Call Err.Raise(Err.Number, Err.Source & " | ASTM1394.PropertyLet.TxFieldDelimiter", Err.Description)
		End Set
	End Property
	
	
	'***********************************************************************
	
	'PROCEDURE:   Class_Initialize()
	
	'DESCRIPTION: Sets up the class upon instatiation
	
	'PARAMETERS:  N/A
	
	'RETURNED:    N/A
	
	'*********************************************************************
	'UPGRADE_NOTE: Class_Initialize was upgraded to Class_Initialize_Renamed. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
	Private Sub Class_Initialize_Renamed()
		On Error GoTo ErrTrap
		' setup connection and connect to the database
		cxnDB_m = New ADODB.Connection
		cxnDB_m.Provider = "Microsoft.Jet.OLEDB.4.0"
        cxnDB_m.Properties("Data Source").Value = My.Application.Info.DirectoryPath & "\" & ASTM1394DB '"\\fs1\Data\Departmental_Data\610 - Software Development\Members\Support Software\Databases\" & ASTM1394DB '"M:\Members\Support Software\Databases\" & ASTM1394DB 
        Call cxnDB_m.Open()
ErrTrap: 
		Err.Source = Err.Source & " | ASTM1394.Class_Initialize"
		RaiseEvent OnError()
	End Sub
	Public Sub New()
		MyBase.New()
		Class_Initialize_Renamed()
	End Sub
	
	'***********************************************************************
	
	'PROCEDURE:   Class_Terminate()
	
	'DESCRIPTION: Cleans up the class upon destruction
	
	'PARAMETERS:  N/A
	
	'RETURNED:    N/A
	
	'*********************************************************************
	'UPGRADE_NOTE: Class_Terminate was upgraded to Class_Terminate_Renamed. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
	Private Sub Class_Terminate_Renamed()
		On Error GoTo ErrTrap
		Call cxnDB_m.Close()
		'UPGRADE_NOTE: Object cxnDB_m may not be destroyed until it is garbage collected. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
        cxnDB_m = Nothing
        GC.Collect() 'nalini
ErrTrap: 
		Err.Source = Err.Source & " | ASTM1394.Class_Terminate"
		RaiseEvent OnError()
	End Sub
	Protected Overrides Sub Finalize()
		Class_Terminate_Renamed()
		MyBase.Finalize()
	End Sub
	
	'***********************************************************************
	
	'PROCEDURE:   objStateMachine_m_OnError()
	
	'DESCRIPTION: Event handler for when ASTM 1381 state machine has an unexpected error
	
	'PARAMETERS:  N/A
	
	'RETURNED:    N/A
	
	'*********************************************************************
	Private Sub objStateMachine_m_OnError() Handles objStateMachine_m.OnError
		Err.Source = Err.Source & " | ASTM1394.objStateMachine_m_OnError"
		RaiseEvent OnError()
	End Sub
	
	'***********************************************************************
	
	'PROCEDURE:   objStateMachine_m_OnMessage
	
	'DESCRIPTION: Event handler for when ASTM 1381 state machine has a message
	
	'PARAMETERS:  message - the message text
	'             messageError - whether there was an error in the message
	
	'RETURNED:    N/A
	
	'*********************************************************************
	Private Sub objStateMachine_m_OnMessage(ByVal message As String, ByRef messageError As Boolean) Handles objStateMachine_m.OnMessage
		On Error GoTo ErrTrap
		messageError = True
		
		' ParseRecord will return true if the record is okay
		messageError = Not ParseRecord(message)
		
		Exit Sub
ErrTrap: 
		Err.Source = Err.Source & " | ASTM1394.objStateMachine_m_OnMessage"
		RaiseEvent OnError()
	End Sub

    '***********************************************************************

    'PROCEDURE:   objStateMachine_m_OnUpdate

    'DESCRIPTION: Event handler for when ASTM 1381 state machine has a update

    'PARAMETERS:  update - the update text

    'RETURNED:    N/A

    '*********************************************************************
    Private Sub objStateMachine_m_TransmitEnable(ByVal mode As Boolean) Handles objStateMachine_m.TransmitEnable
        On Error GoTo ErrTrap

        RaiseEvent TransmitEnable(mode)

        Exit Sub
ErrTrap:
        Err.Source = Err.Source & " | ASTM1394.objStateMachine_m_TransmitEnable"
        RaiseEvent OnError()
    End Sub

	'***********************************************************************
	
	'PROCEDURE:   objStateMachine_m_OnUpdate
	
	'DESCRIPTION: Event handler for when ASTM 1381 state machine has a update
	
	'PARAMETERS:  update - the update text
	
	'RETURNED:    N/A
	
	'*********************************************************************
	Private Sub objStateMachine_m_OnUpdate(ByVal update As String) Handles objStateMachine_m.OnUpdate
		On Error GoTo ErrTrap
		
		RaiseEvent OnUpdate(update)
		
		Exit Sub
ErrTrap: 
		Err.Source = Err.Source & " | ASTM1394.objStateMachine_m_OnUpdate"
		RaiseEvent OnError()
	End Sub
	
	'***********************************************************************
	
	'FUNCTION:    ParseRecord()
	
	'DESCRIPTION:  Helper routine to parse an ASTM 1394 record
	
	'PARAMETERS:   inRecord - the record to parse
	
	'RETURNED:    whether the record could be parsed
	
	'***********************************************************************
	Private Function ParseRecord(ByVal inRecord As String) As Boolean
		On Error GoTo ErrTrap
		Dim recordType As String
		Dim startPos As Integer
		Dim fieldLen As Integer
		Dim curFieldValue As String
		Dim recEndPos As Integer
		Dim fieldList() As String
		Dim fieldIndex As Integer
		Dim maxFieldIndex As Integer
		Dim index As Integer
		
		ParseRecord = False
		
		RaiseEvent OnUpdate(My.Resources.ResourceManager.GetString("str" + CStr(PARSERECTXT)))
		
		index = 1
		
		' see what kind of record it is
		recordType = Mid(inRecord, index, 1)
		
		' the header record defines the delimiters, etc.
		If (recordType = HEADER) Then
			strRxDelimiter_m = Mid(inRecord, index + 1, 1)
			strRepeat_m = Mid(inRecord, index + 2, 1)
			strComponent_m = Mid(inRecord, index + 3, 1)
			strEscape_m = Mid(inRecord, index + 4, 1)
		End If
		
		' get the list of fields for the record type
		fieldList = GetFieldValueList(FIELDTBL, NAMEFLD, "[" & RECTYPEIDFLD & "] LIKE '" & recordType & "'", "[" & ORDERFLD & "] ASC")
		maxFieldIndex = UBound(fieldList)
		fieldIndex = 0
		
		' the end of the record is marked by a carriage return
		recEndPos = InStr(index, inRecord, vbCr)
		
		' parse the record according to the fields
		Do 
			' make sure there's another field for the record type
			If (fieldIndex > maxFieldIndex) Then
				RaiseEvent OnUpdate(My.Resources.ResourceManager.GetString("str" + CStr(RECFIELDERRTXT)))
				Exit Function
			End If
			
			' at this point index should be at the first character in the field
			' save this position for parsing
			startPos = index
			
			' look for the next field delimiter
			index = InStr(startPos, inRecord, strRxDelimiter_m)
			
			' There is no delimiter at the end of the record so index will be assigned zero
			' for the last field.
			If (index = 0) Then
				fieldLen = recEndPos - startPos
			Else
				fieldLen = index - startPos
				
				' advance past the delimiter to the first character in the next field
				index = index + 1
			End If
			
			' get the field text from the record
			curFieldValue = Mid(inRecord, startPos, fieldLen)
			
			' only print the field if it's not blank
			If (curFieldValue <> "") Then
				RaiseEvent OnUpdate(fieldList(fieldIndex) & ":  " & curFieldValue)
			End If
			
			' advance to the next field in the record
			fieldIndex = fieldIndex + 1
			
		Loop While (index <> 0)
		
		ParseRecord = True
		
		Exit Function
ErrTrap: 
		Call Err.Raise(Err.Number, Err.Source & " | ASTM1394.ParseRecord", Err.Description)
	End Function

    '***********************************************************************

    'FUNCTION:    BuildRecord()

    'DESCRIPTION:  Helper routine to build an ASTM 1394 record, then send to analyzer
    'MODIFICATIONS: ----------------------------------------------------------------------------------------------------------------
    '1. replace the Header Message Date&Time, Test Order Requested/Ordered Date&Time With right now time If they are today's date. 
    'Because the time In TextBox (auto filled When load that window) may be too earilier than hit transmit time.  
    '2. calculate Header password based on Key and Messages. KeyValue=Key[Index] from Key table, Index=randomly pick 0~99. Message=HPOL mandatory fields & optional fields
    'Header: Message Control ID=Key(random 0~99); message=HPOL mandatory fields + optional fields, Access Password=HMAC_sha256(message,password,bits,keyindex)
    'Snow 3/20/2016-----------------------------------------------------------------------------------------------------------------
    'PARAMETERS:   inRecordType - the record type to build
    'RETURNED:    the record in ASTM format

    '***********************************************************************
    ' <DllImport("Win32HMAC.dll", CallingConvention = CallingConvention.Cdecl)> SetLastError:=False)>

    '<DllImport("Win32HMAC.dll")>
    'Public Shared Function CalculateSha256Mac(messages As Byte(), hexStr As Byte(), ByVal hexStrLength As UInteger, ByVal keyIndex As UInteger) As Integer

    'End Function


    Private Function BuildRecord(ByVal inRecordType As String, ByVal boolSendingOrder As Boolean) As String
        On Error GoTo ErrTrap

        Dim fieldList() As String
        Dim fieldIndex As Integer
        Dim maxFieldIndex As Integer
        Dim endFieldIndex As Integer
        Dim keyIndex As UInteger 'random int between 0 and 99 for getting keyValue from KeyData.h

        BuildRecord = ""

        'RaiseEvent OnUpdate(My.Resources.ResourceManager.GetString("str" + CStr(BUILDRECTXT)))

        ' get the list of fields for the record type
        fieldList = GetFieldValueList(FIELDTBL, VALUEFLD, "[" & RECTYPEIDFLD & "] LIKE '" & inRecordType & "'", "[" & ORDERFLD & "] ASC")
        maxFieldIndex = UBound(fieldList)
        endFieldIndex = -1

        ' find the highest index field with a value
        For fieldIndex = 0 To maxFieldIndex

            If (fieldList(fieldIndex) <> "") Then
                endFieldIndex = fieldIndex
            End If

        Next fieldIndex

        ' load the fields, stopping at the last field
        For fieldIndex = 0 To endFieldIndex
            'replace time with current time if Head Date is empty. if not load Header type screen could still use old date&time -- Snow 3/28/2016
            If ((inRecordType = "H" And fieldIndex = 13) And fieldList(fieldIndex).Equals("")) Then
                BuildRecord = BuildRecord & DateTime.Now.ToString("yyyyMMddHHmmss") & strTxDelimiter_m
            ElseIf (((inRecordType = "O" And fieldIndex = 6)) And fieldList(fieldIndex).Equals("")) Then 'replace time with current time if Test Order "Requested/Ordered Date and Time" is empty. Do not auto fill date because when cancel an order auto filled date is wrong -- Snow 4/6/2016
                BuildRecord = BuildRecord & DateTime.Now.ToString("yyyyMMddHHmmss") & strTxDelimiter_m
            ElseIf (inRecordType = "H" And fieldIndex = 2 And boolSendingOrder = True) Then 'Index value fill to Header.Message Control ID
                Dim ran As New Random()
                keyIndex = ran.Next(0, 99)
                BuildRecord = BuildRecord & keyIndex & strTxDelimiter_m
            ElseIf (inRecordType = "H" And fieldIndex = 3 And boolSendingOrder = True) Then 'calculate MAC value fill to Header.Access Password

                'Dim message As Byte() = System.Text.Encoding.ASCII.GetBytes("The quick brown fox jumps over the lazy dog")

                'message=HOPL all Mandatory fields and optional fields
                Dim arrayValue1 As String()
                Dim arrayValue2 As String()
                Dim arrayValue3 As String()
                Dim arrayValue4 As String()
                arrayValue1 = GetFieldValueList(FIELDTBL, "IIF(([Value]='' or [Value] is Null) and [Order]=14, '" & DateTime.Now.ToString("yyyyMMddHHmmss") & "', [Value] ) AS Exp", "[" & RECTYPEIDFLD & "] LIKE 'H' AND ([Required] LIKE 'M' OR [Required] LIKE 'O')", "[" & ORDERFLD & "] ASC")
                arrayValue2 = GetFieldValueList(FIELDTBL, VALUEFLD, "[" & RECTYPEIDFLD & "] LIKE 'P' AND ([Required] LIKE 'M' OR [Required] LIKE 'O')", "[" & ORDERFLD & "] ASC")
                arrayValue3 = GetFieldValueList(FIELDTBL, VALUEFLD, "[" & RECTYPEIDFLD & "] LIKE 'O' AND ([Required] LIKE 'M' OR [Required] LIKE 'O')", "[" & ORDERFLD & "] ASC")
                'arrayValue4 = GetFieldValueList(FIELDTBL, VALUEFLD, "[" & RECTYPEIDFLD & "] LIKE 'L' AND ([Required] LIKE 'M' OR [Required] LIKE 'O')", "[" & ORDERFLD & "] ASC")

                Dim arrayValue(arrayValue1.Length + arrayValue2.Length + arrayValue3.Length - 1) As String

                Array.Copy(arrayValue1, arrayValue, arrayValue1.Length)
                Array.Copy(arrayValue2, 0, arrayValue, arrayValue1.Length, arrayValue2.Length)
                Array.Copy(arrayValue3, 0, arrayValue, arrayValue1.Length + arrayValue2.Length, arrayValue3.Length)
                'Array.Copy(arrayValue4, 0, arrayValue, arrayValue1.Length + arrayValue2.Length + arrayValue3.Length, arrayValue4.Length)

                Dim strMessage As String = String.Join("", arrayValue).Replace("^", "")

                Dim message As Byte() = System.Text.Encoding.ASCII.GetBytes(strMessage)


                Dim MAC_HEX_STRING_LENGTH As UInteger
                MAC_HEX_STRING_LENGTH = 2 * (256 / 8)
                Dim index As UInteger

                index = 6
                Dim passwordByte As Byte() = New Byte(MAC_HEX_STRING_LENGTH) {}

                Dim index1 As Integer = 1

                Dim password As String = ""

                index1 = UnmanagedCalls.CalculateSha256Mac(message, passwordByte, MAC_HEX_STRING_LENGTH, keyIndex)
                password = System.Text.Encoding.ASCII.GetString(passwordByte)

                BuildRecord = BuildRecord & Left(password, MAC_HEX_STRING_LENGTH) & strTxDelimiter_m 'BuildRecord & password & strTxDelimiter_m
            Else
                BuildRecord = BuildRecord & fieldList(fieldIndex) & strTxDelimiter_m
            End If

        Next fieldIndex

        ' if fields were added to the message then cut off the last delimiter
        If (Len(BuildRecord) <> 0) Then
            BuildRecord = Left(BuildRecord, Len(BuildRecord) - 1)
        End If

        ' the end of the record is marked by a carriage return
        BuildRecord = BuildRecord & vbCr

        Exit Function
ErrTrap:
        Call MsgBox("Number:  " & Err.Number & vbCrLf & vbCrLf & "Audit Trail:  " & Err.Source & vbCrLf & vbCrLf & "Description:  " & Err.Description, MsgBoxStyle.Exclamation Or MsgBoxStyle.OkOnly, "Error")
        'Call Err.Raise(Err.Number, Err.Source & " | ASTM1394.BuildRecord", Err.Description)
    End Function

    '***********************************************************************

    'FUNCTION:    BuildRecord()

    'DESCRIPTION:  Helper routine to build an ASTM 1394 record, then send to analyzer
    'MODIFICATIONS: ----------------------------------------------------------------------------------------------------------------
    '1. replace the Header Message Date&Time, Test Order Requested/Ordered Date&Time With right now time If they are today's date. 
    'Because the time In TextBox (auto filled When load that window) may be too earilier than hit transmit time.  
    '2. calculate Header password based on Key and Messages. KeyValue=Key[Index] from Key table, Index=randomly pick 0~99. Message=HPOL mandatory fields & optional fields
    'Header: Message Control ID=Key(random 0~99); message=HPOL mandatory fields + optional fields, Access Password=HMAC_sha256(message,password,bits,keyindex)
    'Snow 3/20/2016-----------------------------------------------------------------------------------------------------------------
    'PARAMETERS:   inRecordType - the record type to build
    'RETURNED:    the record in ASTM format

    '***********************************************************************
    ' <DllImport("Win32HMAC.dll", CallingConvention = CallingConvention.Cdecl)> SetLastError:=False)>

    '<DllImport("Win32HMAC.dll")>
    'Public Shared Function CalculateSha256Mac(messages As Byte(), hexStr As Byte(), ByVal hexStrLength As UInteger, ByVal keyIndex As UInteger) As Integer

    'End Function


    Private Function CalculatePasswordForXMLFile(ByRef arrHeader As String(), ByRef arrPatient As String(), ByRef arrOrder As String()) As String()
        On Error GoTo ErrTrap

        Dim fieldList() As String
        Dim fieldIndex As Integer
        Dim maxFieldIndex As Integer
        Dim endFieldIndex As Integer
        Dim keyIndex As UInteger 'random int between 0 and 99 for getting keyValue from KeyData.h
        Dim strMessage As String = ""


        Dim ran As New Random()
        keyIndex = ran.Next(0, 99)
        arrHeader(2) = keyIndex

        strMessage = arrHeader(9) + arrHeader(13) + arrPatient(2) + arrPatient(3) + arrPatient(4) + arrPatient(7) + arrPatient(8) + arrPatient(9) + arrPatient(12) + arrPatient(14).Replace("^", "") + arrPatient(15) + arrPatient(25) + arrOrder(4).Replace("^", "") + arrOrder(6) + arrOrder(11) + arrOrder(16) + arrOrder(20) + arrOrder(25)
        Dim message As Byte() = System.Text.Encoding.ASCII.GetBytes(strMessage.Replace("^", ""))

        Dim MAC_HEX_STRING_LENGTH As UInteger
        MAC_HEX_STRING_LENGTH = 2 * (256 / 8)
        Dim index As UInteger

        index = 6
        Dim passwordByte As Byte() = New Byte(MAC_HEX_STRING_LENGTH) {}

        Dim index1 As Integer = 1

        Dim password As String = ""

        index1 = UnmanagedCalls.CalculateSha256Mac(message, passwordByte, MAC_HEX_STRING_LENGTH, keyIndex)
        password = System.Text.Encoding.ASCII.GetString(passwordByte)

        arrHeader(3) = Left(password, MAC_HEX_STRING_LENGTH)
        CalculatePasswordForXMLFile = arrHeader

        Exit Function
ErrTrap:
        Call MsgBox("Number:  " & Err.Number & vbCrLf & vbCrLf & "Audit Trail:  " & Err.Source & vbCrLf & vbCrLf & "Description:  " & Err.Description, MsgBoxStyle.Exclamation Or MsgBoxStyle.OkOnly, "Error")
        'Call Err.Raise(Err.Number, Err.Source & " | ASTM1394.BuildRecord", Err.Description)
    End Function

    '***********************************************************************

    'PROCEDURE:   GetFieldValueList()

    'DESCRIPTION: Helper routine that returns the values for a field in a table

    'PARAMETERS:  inTable - the table with the values
    '             inField - the field with the values
    '             inCriteria - the specific records to retrieve
    '             inSort - the order to sort by

    'RETURNED:    the list of values in the field
    '*********************************************************************
    Private Function GetFieldValueList(ByVal inTable As String, ByVal inField As String, Optional ByVal inCriteria As Object = Nothing, Optional ByVal inSort As Object = Nothing) As String()
        On Error GoTo ErrTrap
        Dim rs As ADODB.Recordset
        Dim maxIndex As Integer
        Dim index As Integer
        Dim list() As String
        Dim strSQL As String
        'ByVal inField As String
        ' prepare the recordset object
        rs = New ADODB.Recordset
        rs.CursorLocation = ADODB.CursorLocationEnum.adUseClient

        ' the SQL string starts by retrieving all the records from the table
        If (inField.StartsWith("IIF")) Then
            strSQL = "SELECT " & inField & " FROM " & "[" & inTable & "]" 'change SELECT * to SELECT inField, only get one wanted field -- Snow 4/11/2016
        Else
            strSQL = "SELECT * FROM " & "[" & inTable & "]"
        End If


        ' the caller may want to add criteria
        'UPGRADE_NOTE: IsMissing() was changed to IsNothing(). Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="8AE1CB93-37AB-439A-A4FF-BE3B6760BB23"'
        If (IsNothing(inCriteria) = False) Then
            'UPGRADE_WARNING: Couldn't resolve default property of object inCriteria. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            strSQL = strSQL & " WHERE " & inCriteria
        End If

        ' the caller may want to add sorting
        'UPGRADE_NOTE: IsMissing() was changed to IsNothing(). Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="8AE1CB93-37AB-439A-A4FF-BE3B6760BB23"'
        If (IsNothing(inSort) = False) Then
            'UPGRADE_WARNING: Couldn't resolve default property of object inSort. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            strSQL = strSQL & " ORDER BY " & inSort
        End If

        Call rs.Open(strSQL, cxnDB_m, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockOptimistic, ADODB.CommandTypeEnum.adCmdText)

        ' make sure fields were found for the record type
        If (rs.EOF = True) Then
            Call Err.Raise(COMPERR, "ASTM1394", My.Resources.ResourceManager.GetString("str" + CStr(EOFERRTXT)))
        End If

        Call rs.MoveFirst()

        ' prepare the return array
        maxIndex = rs.RecordCount - 1
        ReDim list(maxIndex)

        ' add the field values to the return array
        If (inField.StartsWith("IIF")) Then
            inField = "Exp"
        End If
        For index = 0 To maxIndex

            'UPGRADE_WARNING: Use of Null/IsNull() detected. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="2EED02CB-5C0E-4DC1-AE94-4FAA3A30F51A"'
            If (IsDBNull(rs.Fields(inField).Value)) Then
                list(index) = ""
            Else
                list(index) = rs.Fields(inField).Value
            End If

            Call rs.MoveNext()

        Next index

        GetFieldValueList = list.Clone() 'VB6.CopyArray(list). CopyArray() is obsolete. change to .Clone() Snow 3/24/2016

        ' clean up
        Call rs.Close()
        'UPGRADE_NOTE: Object rs may not be destroyed until it is garbage collected. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
        rs = Nothing
        GC.Collect() 'nalini

        Exit Function
ErrTrap:
        'UPGRADE_NOTE: Object rs may not be destroyed until it is garbage collected. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
        rs = Nothing
        GC.Collect() 'nalini
        Call Err.Raise(Err.Number, Err.Source & " | ASTM1394.GetFieldValueList", Err.Description)
    End Function

    '***********************************************************************

    'PROCEDURE:    OpenComm()

    'DESCRIPTION:  Allows other objects to engage the ASTM 1381 state machine

    'PARAMETERS:   inPort - the serial port to use

    'RETURNED:    N/A

    '***********************************************************************
    Public Sub OpenComm(ByVal inPort As Short) 'As Byte
        On Error GoTo ErrTrap

        objStateMachine_m = New ASTM1381

        ' start the state machine
        Call objStateMachine_m.StartStateMachine(inPort)

        Exit Sub
ErrTrap:
        If (Err.Number = "8002") Then
            Call Err.Raise(Err.Number, "", "Com Port not found in ASTM1394") 'if inPort is not correct/active display message "Com Port not found". Snow 3/4/2016
        Else
            Call Err.Raise(Err.Number, Err.Source & " | ASTM1394.OpenComm", Err.Description) 'if inPort is not correct/active display message "Com Port not found"
        End If
    End Sub

    '***********************************************************************

    'PROCEDURE:    OpenComm()

    'DESCRIPTION:  Allows other objects to engage the ASTM 1381 state machine

    'PARAMETERS:   inAddress - the IP address for communicating

    'RETURNED:    inPort -  port number to use

    '***********************************************************************
    Public Sub OpenComm(ByVal inAddress As String, ByVal inPort As String)
        On Error GoTo ErrTrap

        objStateMachine_m = New ASTM1381

        ' start the state machine
        Call objStateMachine_m.StartStateMachine(inAddress, inPort)

        Exit Sub
ErrTrap:
        Call Err.Raise(Err.Number, Err.Source & " | ASTM1394.OpenTCPIPComm", Err.Description)
    End Sub
	'***********************************************************************
	
	'PROCEDURE:    CloseComm()
	
	'DESCRIPTION:  Allows other objects to stop the ASTM 1381 state machine
	
    'PARAMETERS:   N/A   Nalini
	
	'RETURNED:    N/A
	
	'***********************************************************************
    Public Sub CloseComm(ByVal type As Byte)
        On Error GoTo ErrTrap

        ' stop the state machine
        Call objStateMachine_m.StopStateMachine(type)

        'UPGRADE_NOTE: Object objStateMachine_m may not be destroyed until it is garbage collected. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
        objStateMachine_m = Nothing
        GC.Collect() 'nalini
        Exit Sub
ErrTrap:
        Call Err.Raise(Err.Number, Err.Source & " | ASTM1394.CloseComm", Err.Description)
    End Sub

    '***********************************************************************

    'PROCEDURE:    TransmitRecords()

    'DESCRIPTION:  Allows other objects to transmit a sequence of records. Transmit filled data to analyzer 

    'PARAMETERS:   inRecordSequence - the record sequence to transmit

    'RETURNED:    N/A

    '***********************************************************************
    Public Sub TransmitRecords(ByVal inRecordSequence As String)
        On Error GoTo ErrTrap

        Dim recordType As String
        Dim index As Integer
        Dim maxIndex As Integer
        Dim recordList() As String
        Dim delimiters As String
        Dim dateValue As String
        'add boolean boolSendingOrder to pass to ASTM1394.BuildOrder(recordType, boolSendingOrder) so when inRecordSequence=HPOL then collect all fields form message 
        'calculate password based on KeyValue[Index] & message, put value in Header.[Access Password] send to analyzer. Snow 4/8/2016
        Dim boolSendingOrder As Boolean
        boolSendingOrder = False

        'check if Header.[Message date and time]='' then set=Now()
        UpdateDateValue("H", 14)

        If (inRecordSequence.Equals("HPOL")) Then
            boolSendingOrder = True

            'check if Order.[Requested/Ordered date and time]='' set=Now()
            UpdateDateValue("O", 7)
        End If

        ' see how many records to send
        maxIndex = Len(inRecordSequence)

        If (maxIndex = 0) Then
            Exit Sub
        End If

        'RaiseEvent OnUpdate(My.Resources.ResourceManager.GetString("str" + CStr(TXSEQUENCETXT)) & inRecordSequence) 'Snow do not show sequence

        ' prepare the records array
        ReDim recordList(maxIndex - 1)

        ' build each record
        For index = 1 To maxIndex

            ' see what kind of record it is
            recordType = Mid(inRecordSequence, index, 1)

            recordList(index - 1) = BuildRecord(recordType, boolSendingOrder)

        Next index

        Call objStateMachine_m.TransmitMessages(recordList) 'recordList=String(){"1H...", "2P...", "3O...", "4L..."}


        Exit Sub
ErrTrap:
        Call Err.Raise(Err.Number, Err.Source & " | ASTM1394.TransmitRecords", Err.Description)
    End Sub

    '***********************************************************************

    'PROCEDURE:    TransmitRecords(Sequence, HeaderString, PatientString, OrderString, TerminatorString) //strings from file

    'DESCRIPTION:  Transmit Order HPOL data from file to analyzer. Could be multiple patients, multiple rotors in one xml file. 
    'Store all data[] temperally In array For Loop them To transmit. Not get from database. 

    'PARAMETERS:   inRecordSequence - the record sequence to transmit=HPOL, numOfOrder - how many orders from .xml file

    'RETURNED:    N/A
    ' Snow 4/28/2016
    '***********************************************************************
    Public Sub TransmitRecords(ByVal inRecordSequence As String, ByRef arrHeader As String(), ByVal arrPatient As String(), ByVal arrOrder As String(), ByVal arrTerminator As String(), ByVal orderNum As String)
        On Error GoTo ErrTrap

        Dim recordType As String
        Dim totalOrderNumber As Integer
        Dim index As Integer

        Dim recordList() As String

        'add boolean boolSendingOrder to pass to ASTM1394.BuildOrder(recordType, boolSendingOrder) so when inRecordSequence=HPOL then collect all fields form message 
        'calculate password based on KeyValue[Index] & message, put value in Header.[Access Password] send to analyzer. Snow 4/8/2016
        Dim boolSendingOrder As Boolean
        boolSendingOrder = True

        'loop each patient, each rotor Array Order[], each Order[i]={H, P, O, L}

        'If (objStateMachine_m.intState_m = ASTM1381.enum_ASTMState.abxIdle Or objStateMachine_m.intState_m = ASTM1381.enum_ASTMState.abxTXWaitingCxn Or objStateMachine_m.intState_m = ASTM1381.enum_ASTMState.abxTXWaitingACK) Then
        'RaiseEvent OnUpdate(My.Resources.ResourceManager.GetString("str" + CStr(TXSEQUENCETXT)) & inRecordSequence)

        RaiseEvent OnUpdate("Order #" & orderNum)

        ' prepare the records array
        ReDim recordList(3) 'upper bound=3

        'calculate password by HMAC based on required fields and optional fields in HPO
        arrHeader = CalculatePasswordForXMLFile(arrHeader, arrPatient, arrOrder) 'fill H[2]=key, H[3]=password
        Threading.Thread.Sleep(300)

        recordList(0) = String.Join("|", arrHeader) + vbCr 'get array recordList(0,1,2,3) from received parameter from Main.vb
        recordList(1) = String.Join("|", arrPatient) + vbCr
        recordList(2) = String.Join("|", arrOrder) + vbCr
        recordList(3) = String.Join("|", arrTerminator) + vbCr
        'If (objStateMachine_m.intState_m = ASTM1381.enum_ASTMState.abxRXWaiting) Then
        Select Case objStateMachine_m.intState_m
            Case ASTM1381.enum_ASTMState.abxIdle
                Call objStateMachine_m.TransmitMessages(recordList) 'recordList=String(){"1H...", "2P...", "3O...", "4L..."}
                Exit Select
            Case Else
                Threading.Thread.Sleep(500)
        End Select

        'End If
        Exit Sub
ErrTrap:
        Call Err.Raise(Err.Number, Err.Source & " | ASTM1394.TransmitRecords", Err.Description)
    End Sub


    Public Sub UpdateDateValue(ByVal inRecTypeID As String, ByVal inOrderID As Integer)

        On Error GoTo ErrTrap
        Dim rs As ADODB.Recordset
        Dim strSQL As String

        ' prepare the recordset object
        rs = New ADODB.Recordset
        rs.CursorLocation = ADODB.CursorLocationEnum.adUseClient

        ' the SQL string retrieves the record to update from the table
        strSQL = "SELECT * FROM " & FIELDTBL & " WHERE ([" & RECTYPEIDFLD & "] LIKE '" & inRecTypeID & "') AND ([Order] = " & inOrderID & ")"

        Call rs.Open(strSQL, cxnDB_m, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockOptimistic, ADODB.CommandTypeEnum.adCmdText)

        ' make sure a record was found
        If (rs.EOF = True) Then
            Call Err.Raise(COMPERR, "ASTM1394", My.Resources.ResourceManager.GetString("str" + CStr(EOFERRTXT)))
        End If

        If (IsDBNull(rs.Fields("Value").Value) Or rs.Fields("Value").Value.Equals("")) Then
            ' update the value
            rs.Fields(VALUEFLD).Value = DateTime.Now.ToString("yyyyMMddHHmmss")
            Call rs.Update()
        End If

        ' clean up
        Call rs.Close()
        'UPGRADE_NOTE: Object rs may not be destroyed until it is garbage collected. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
        rs = Nothing
        GC.Collect() 'nalini
        Exit Sub
ErrTrap:
        'UPGRADE_NOTE: Object rs may not be destroyed until it is garbage collected. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
        rs = Nothing
        GC.Collect() 'nalini
        Call Err.Raise(Err.Number, Err.Source & " | ASTM1394.PropertyLet.UpdateDateValue", Err.Description)

    End Sub

    Private Function BuildRecordForFile(ByVal inRecordType As String, ByVal boolSendingOrder As Boolean) As String
        On Error GoTo ErrTrap

        Dim fieldList() As String
        Dim fieldIndex As Integer
        Dim maxFieldIndex As Integer
        Dim endFieldIndex As Integer
        Dim keyIndex As UInteger 'random int between 0 and 99 for getting keyValue from KeyData.h

        BuildRecordForFile = ""
        boolSendingOrder = True
        'RaiseEvent OnUpdate(My.Resources.ResourceManager.GetString("str" + CStr(BUILDRECTXT)))

        ' get the list of fields for the record type
        fieldList = GetFieldValueList(FIELDTBL, VALUEFLD, "[" & RECTYPEIDFLD & "] LIKE '" & inRecordType & "'", "[" & ORDERFLD & "] ASC")
        maxFieldIndex = UBound(fieldList)
        endFieldIndex = -1

        ' find the highest index field with a value
        For fieldIndex = 0 To maxFieldIndex

            If (fieldList(fieldIndex) <> "") Then
                endFieldIndex = fieldIndex
            End If

        Next fieldIndex

        ' load the fields, stopping at the last field
        For fieldIndex = 0 To endFieldIndex
            'replace time with current time if Head Date is empty. if not load Header type screen could still use old date&time -- Snow 3/28/2016
            If ((inRecordType = "H" And fieldIndex = 13) And fieldList(fieldIndex).Equals("")) Then
                BuildRecordForFile = BuildRecordForFile & DateTime.Now.ToString("yyyyMMddHHmmss") & strTxDelimiter_m
            ElseIf (((inRecordType = "O" And fieldIndex = 6)) And fieldList(fieldIndex).Equals("")) Then 'replace time with current time if Test Order "Requested/Ordered Date and Time" is empty. Do not auto fill date because when cancel an order auto filled date is wrong -- Snow 4/6/2016
                BuildRecordForFile = BuildRecordForFile & DateTime.Now.ToString("yyyyMMddHHmmss") & strTxDelimiter_m
            ElseIf (inRecordType = "H" And fieldIndex = 2 And boolSendingOrder = True) Then 'Index value fill to Header.Message Control ID
                Dim ran As New Random()
                keyIndex = ran.Next(0, 99)
                BuildRecordForFile = BuildRecordForFile & keyIndex & strTxDelimiter_m
            ElseIf (inRecordType = "H" And fieldIndex = 3 And boolSendingOrder = True) Then 'calculate MAC value fill to Header.Access Password

                'Dim message As Byte() = System.Text.Encoding.ASCII.GetBytes("The quick brown fox jumps over the lazy dog")

                'message=HOPL all Mandatory fields and optional fields
                Dim arrayValue1 As String()
                Dim arrayValue2 As String()
                Dim arrayValue3 As String()
                Dim arrayValue4 As String()
                arrayValue1 = GetFieldValueList(FIELDTBL, "IIF(([Value]='' or [Value] is Null) and [Order]=14, '" & DateTime.Now.ToString("yyyyMMddHHmmss") & "', [Value] ) AS Exp", "[" & RECTYPEIDFLD & "] LIKE 'H' AND ([Required] LIKE 'M' OR [Required] LIKE 'O')", "[" & ORDERFLD & "] ASC")
                arrayValue2 = GetFieldValueList(FIELDTBL, VALUEFLD, "[" & RECTYPEIDFLD & "] LIKE 'P' AND ([Required] LIKE 'M' OR [Required] LIKE 'O')", "[" & ORDERFLD & "] ASC")
                arrayValue3 = GetFieldValueList(FIELDTBL, VALUEFLD, "[" & RECTYPEIDFLD & "] LIKE 'O' AND ([Required] LIKE 'M' OR [Required] LIKE 'O')", "[" & ORDERFLD & "] ASC")
                'arrayValue4 = GetFieldValueList(FIELDTBL, VALUEFLD, "[" & RECTYPEIDFLD & "] LIKE 'L' AND ([Required] LIKE 'M' OR [Required] LIKE 'O')", "[" & ORDERFLD & "] ASC")

                Dim arrayValue(arrayValue1.Length + arrayValue2.Length + arrayValue3.Length - 1) As String

                Array.Copy(arrayValue1, arrayValue, arrayValue1.Length)
                Array.Copy(arrayValue2, 0, arrayValue, arrayValue1.Length, arrayValue2.Length)
                Array.Copy(arrayValue3, 0, arrayValue, arrayValue1.Length + arrayValue2.Length, arrayValue3.Length)
                'Array.Copy(arrayValue4, 0, arrayValue, arrayValue1.Length + arrayValue2.Length + arrayValue3.Length, arrayValue4.Length)

                Dim strMessage As String = String.Join("", arrayValue).Replace("^", "")

                Dim message As Byte() = System.Text.Encoding.ASCII.GetBytes(strMessage)


                Dim MAC_HEX_STRING_LENGTH As UInteger
                MAC_HEX_STRING_LENGTH = 2 * (256 / 8)
                Dim index As UInteger

                index = 6
                Dim passwordByte As Byte() = New Byte(MAC_HEX_STRING_LENGTH) {}

                Dim index1 As Integer = 1

                Dim password As String = ""

                index1 = UnmanagedCalls.CalculateSha256Mac(message, passwordByte, MAC_HEX_STRING_LENGTH, keyIndex)
                password = System.Text.Encoding.ASCII.GetString(passwordByte)

                BuildRecordForFile = BuildRecordForFile & Left(password, MAC_HEX_STRING_LENGTH) & strTxDelimiter_m 'BuildRecord & password & strTxDelimiter_m
            Else
                BuildRecordForFile = BuildRecordForFile & fieldList(fieldIndex) & strTxDelimiter_m
            End If

        Next fieldIndex

        ' if fields were added to the message then cut off the last delimiter
        If (Len(BuildRecordForFile) <> 0) Then
            BuildRecordForFile = Left(BuildRecordForFile, Len(BuildRecordForFile) - 1)
        End If

        ' the end of the record is marked by a carriage return
        BuildRecordForFile = BuildRecordForFile & vbCr

        Exit Function
ErrTrap:
        'Call MsgBox("Number:  " & Err.Number & vbCrLf & vbCrLf & "Audit Trail:  " & Err.Source & vbCrLf & vbCrLf & "Description:  " & Err.Description, MsgBoxStyle.Exclamation Or MsgBoxStyle.OkOnly, "Error")
        Call Err.Raise(Err.Number, Err.Source & " | ASTM1394.BuildRecordForFile", Err.Description)
    End Function

    '***********************************************************************

    'PROCEDURE:    StopTransmit()

    'DESCRIPTION:  Stop transmission in the middle of process

    'RETURNED:    N/A

    '***********************************************************************
    Public Sub StopTransmit()
        On Error GoTo ErrTrap

        Call objStateMachine_m.StopTransmit()

        Exit Sub
ErrTrap:
        Call Err.Raise(Err.Number, Err.Source & " | ASTM1394.StopTransmit", Err.Description)
    End Sub

End Class