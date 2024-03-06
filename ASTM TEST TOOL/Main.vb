Option Strict Off
Option Explicit On
Imports Microsoft.Win32
Imports System.Net.Sockets
Imports System.IO
Imports System.Xml
Imports System.ComponentModel
Imports System.Threading.Tasks
Imports System.Collections.Concurrent
Imports System.Collections.Generic


'UPGRADE_NOTE: Main was upgraded to Main_Renamed. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
Friend Class Main_Renamed
    Inherits System.Windows.Forms.Form
    '**********************************************************************************

    'FILE:  Main.frm

    'DESCRIPTION:  This module contains the form for the main user interface.

    'COMPILER:  This module is part of a project that is designed to be edited and compiled
    'in Visual Basic 6.0.  Choose "File->Make" from within the IDE to make the program.

    '$History: Main.frm $
    ' *****************  Version 2.1.0 B1  *****************
    ' Snow     Date: 5/27/2016    
    ' added new features: generate .xml file to send multiple orders in once. max 100 orders (analyzer can take only 100 in buffer)
    ' Visual Basic.NET compiled with .NET 4.0
    '
    ' *****************  Version 2.0.0 B1  *****************
    ' Snow     Date: 4/19/2016    
    ' fixed old bugs. added new features: create password in Header and send to analyzer
    ' only when analyzer' side calculated password matches ASTM password, then send response back to system. 
    ' Visual Basic.NET compiled with .NET 4.0
    '
    ' 
    ' *****************  Version 2  *****************
    ' User: Ballard      Date: 5/18/04    Time: 3:05p
    ' Updated in $/ASTM/Source
    ' The TX and RX blocks are now separated with a carriage return.
    ' The communications window now scrolls down with the new text.
    '
    ' *****************  Version 1  *****************
    ' User: Ballard      Date: 4/09/04    Time: 9:34a
    ' Created in $/ASTM/Source
    ' Added to SourceSafe.

    Delegate Sub UpdateButtonEnabledHandler(ByVal enabled As Boolean)
    Delegate Sub UpdateTextHandler(ByVal message As String)

    ' registry entries
    Private Const SECTIONKEY As String = "Settings"
    Private Const COMPORTKEY As String = "COMPort"
    Private Const LASTPATHKEY As String = "LastPath"
    Private Const TXSEQUENCEKEY As String = "TxSequence"
    Private Const TXFIELDDELIMITERKEY As String = "TxFieldDelimiter"
    Private Const VERBOSEKEY As String = "Verbose"
    Private Const IPADDRESSKEY As String = "IPAddress"
    Private Const PORTKEY As String = "Port"
    Private Const APPLICATIONTITLE As String = "ASTM Test Tool"

    ' registry keys
    Private Const REGKEYSETTINGS As String = "Software\VB and VBA Program Settings\ASTM Test Tool\Settings"

    Private Const BS As Integer = &H8S

    ' columns in the list view
    Private Const VALUECOL As Integer = 1

    'Mode of communication
    Private Const SERIALCOMM As Byte = &H0 'Serial Communication using RS232 
    Private Const TCPIPCOMM As Byte = &H1 'TCP/IP Programming

    ' member variables
    Private WithEvents objAnalyzer_m As ASTM1394
    Private astrRecTypeID_m() As String
    'Private WithEvents BackgroundWorker1 As BackgroundWorker

    ' selected ListView lvwRecord data for update database when change cboRecordType.SelectedIndexChanged. Value changed in a TextBox get saved only when click another TextBox field
    ' Save the last click TextBox.Text when mouse move to cboRecordType or cmdTx (Transmit button). Snow Shi 3/1/2016
    Private selectedRecordTypeID As String
    Private selectedItemName As String
    Private selectedItemValue As String

    'Defining the delegate 
    'Public Delegate Sub EnableTransmitDelegate()
    'Instantiating the delegate
    'Dim TransmitEnable As EnableTransmitDelegate

    '***********************************************************************

    'PROCEDURE:   Form_Load()

    'DESCRIPTION: Event handler for when the form loads

    'PARAMETERS:  N/A

    'RETURNED:    N/A

    '*********************************************************************
    Private Sub Main_Load(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles MyBase.Load
        On Error GoTo ErrTrap
        Dim list() As String
        Dim maxIndex As Integer
        Dim index As Integer
        Dim ipaddr As String
        Dim delim() As Char = New Char() {"."}
        Dim addr() As String

        ' initialize the controls from the registry
        ' get the last session information saved from the registry
        Dim reg As RegistryKey = Registry.CurrentUser.CreateSubKey(REGKEYSETTINGS)
        nudPortNumber.Value = reg.GetValue(COMPORTKEY, 9)
        txtSequence.Text = reg.GetValue(TXSEQUENCEKEY, "HQL")
        txtFieldDelimiter.Text = reg.GetValue(TXFIELDDELIMITERKEY, "|") 'ColumnWidthChangedEventArgs default Delimiter from "l" to "|" pipe. --Snow 2/19/2016
        chkVerbose.Checked = reg.GetValue(VERBOSEKEY, True)
        txtPort.Text = reg.GetValue(PORTKEY, "13000")
        ipaddr = reg.GetValue(IPADDRESSKEY, "192.1.2.8")
        addr = ipaddr.Split(delim)
        txtIP1.Text = addr(0)
        txtIP2.Text = addr(1)
        txtIP3.Text = addr(2)
        txtIP4.Text = addr(3)

        reg.Close()

        ' set up the save file dialog
        dlgSave.RestoreDirectory = False
        dlgSave.OverwritePrompt = True
        dlgSave.Filter = "Text (*.txt)|*.txt"

        ' initialize the Analyzer
        objAnalyzer_m = New ASTM1394
        objAnalyzer_m.TxFieldDelimiter = txtFieldDelimiter.Text

        ' get the list of record types from the analyzer object
        astrRecTypeID_m = objAnalyzer_m.RecTypeIDList.Clone() 'VB6.CopyArray(objAnalyzer_m.RecTypeIDList) CopyArray() is obsolete. change to .Clone() Snow 3/24/2016
        maxIndex = UBound(astrRecTypeID_m)

        ' load all the record types into the combo box
        For index = 0 To maxIndex
            Call cboRecordType.Items.Add(objAnalyzer_m.RecName(astrRecTypeID_m(index)))
        Next index

        ' set up the list view columns
        With lvwRecord
            Call .ColumnHeaders.Add(1, "Field", "Field", 3500, Nothing, Nothing)
            Call .ColumnHeaders.Add(2, "Value", "Value", 7100, Nothing, Nothing)
        End With

        Exit Sub
ErrTrap:
        ' can't raise an error here because there's nothing up the call stack to trap it
        Call HandleError(Err.Number, Err.Source & " | Main.Form_Load", Err.Description)
    End Sub

    '***********************************************************************

    'PROCEDURE:   Form_Unload()

    'DESCRIPTION: Event handler for when the form unloads

    'PARAMETERS:  cancel - whether to cancel the unload

    'RETURNED:    N/A

    '*********************************************************************
    Private Sub Main_FormClosed(ByVal eventSender As System.Object, ByVal eventArgs As System.Windows.Forms.FormClosedEventArgs) Handles Me.FormClosed
        On Error GoTo ErrTrap
        'UPGRADE_NOTE: Object objAnalyzer_m may not be destroyed until it is garbage collected. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
        objAnalyzer_m = Nothing
        GC.Collect() 'nalini
        ' save the control settings to the registry
        Call SaveSetting(APPLICATIONTITLE, SECTIONKEY, COMPORTKEY, CStr(nudPortNumber.Value))
        Call SaveSetting(APPLICATIONTITLE, SECTIONKEY, TXSEQUENCEKEY, txtSequence.Text)
        Call SaveSetting(APPLICATIONTITLE, SECTIONKEY, TXFIELDDELIMITERKEY, txtFieldDelimiter.Text)
        Call SaveSetting(APPLICATIONTITLE, SECTIONKEY, VERBOSEKEY, CStr(chkVerbose.CheckState))
        Call SaveSetting(APPLICATIONTITLE, SECTIONKEY, PORTKEY, txtPort.Text)
        Dim addr As String
        addr = txtIP1.Text & "." & txtIP2.Text & "." & txtIP3.Text & "." & txtIP4.Text
        Call SaveSetting(APPLICATIONTITLE, SECTIONKEY, IPADDRESSKEY, addr)

        Exit Sub
ErrTrap:
        ' can't raise an error here because there's nothing up the call stack to trap it
        Call HandleError(Err.Number, Err.Source & " | Main.Form_Unload", Err.Description)
    End Sub

    '***********************************************************************

    'PROCEDURE:   Form_Resize()

    'DESCRIPTION: Event handler for when the form resizes

    'PARAMETERS:  N/A

    'RETURNED:    N/A

    '*********************************************************************
    'UPGRADE_WARNING: Event Main.Resize may fire when form is initialized. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="88B12AE1-6DE0-48A0-86F1-60C0686C026A"'
    Private Sub Main_Resize(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles MyBase.Resize
        On Error Resume Next
        'fraRecord.Width = VB6.TwipsToPixelsX(VB6.PixelsToTwipsX(Me.Width) - 300)
        'lvwRecord.Width = VB6.TwipsToPixelsX(VB6.PixelsToTwipsX(Me.Width) - 570)
        'fraDisplay.Width = VB6.TwipsToPixelsX(VB6.PixelsToTwipsX(Me.Width) - 300)
        'txtDisplay.Width = VB6.TwipsToPixelsX(VB6.PixelsToTwipsX(Me.Width) - 570)
        'fraDisplay.Height = VB6.TwipsToPixelsY(VB6.PixelsToTwipsY(Me.Height) - 4530)
        'txtDisplay.Height = VB6.TwipsToPixelsY(VB6.PixelsToTwipsY(Me.Height) - 5550)
        'cmdSave.Top = VB6.TwipsToPixelsY(VB6.PixelsToTwipsY(Me.Height) - 5000)
        'cmdClear.Top = VB6.TwipsToPixelsY(VB6.PixelsToTwipsY(Me.Height) - 5000)
    End Sub

    '***********************************************************************

    'PROCEDURE:   TransmitEnable()

    'DESCRIPTION: Event handler to enable or disable the transmit button

    'PARAMETERS:  False means Disable the Transmit button and True means
    'Enable te button

    'RETURNED:    N/A

    '*********************************************************************
    Private Sub TransmitEnable(ByVal state As Boolean) Handles objAnalyzer_m.TransmitEnable
        'added nwr 2/19/2014
        If (cmdTx.InvokeRequired) Then
            If (state) Then
                cmdTx.Invoke(New UpdateButtonEnabledHandler(AddressOf UpdateBtnEnabled), New Object() {True})
            Else
                cmdTx.Invoke(New UpdateButtonEnabledHandler(AddressOf UpdateBtnEnabled), New Object() {False})
            End If
        Else
            If (state) Then
                cmdTx.Enabled = True
            Else
                cmdTx.Enabled = False
            End If
        End If
    End Sub
    '***********************************************************************

    'PROCEDURE:   cmdStart_Click()

    'DESCRIPTION: Event handler for when the user clicks 'Start' Serial Communication button

    'PARAMETERS:  N/A

    'RETURNED:    N/A

    '*********************************************************************
    Private Sub cmdStart_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles cmdStart.Click
        On Error GoTo ErrTrap

        ' start the Analyzer
        Call objAnalyzer_m.OpenComm(nudPortNumber.Value)
        ' change the UI to a "communicating state"
        nudPortNumber.Enabled = False
        cmdStart.Enabled = False
        cmdStop.Enabled = True
        cmdTx.Enabled = True

        txtIP1.Enabled = False
        txtIP2.Enabled = False
        txtIP3.Enabled = False
        txtIP4.Enabled = False
        txtPort.Enabled = False
        btnStart.Enabled = False
        btnStop.Enabled = False
        btnStopTransmit.Enabled = True
        btnBrowseFile.Enabled = True
        btnFileTransmit.Enabled = True
        Exit Sub
ErrTrap:
        ' can't raise an error here because there's nothing up the call stack to trap it
        If (Err.Number = "8002") Then
            Call HandleError(Err.Number, "", "Com Port not found") 'if inPort is not correct/active display message "Com Port not found"
        Else
            Call HandleError(Err.Number, Err.Source & " | Main.cmdStart_Click", Err.Description)
        End If
    End Sub

    '***********************************************************************

    'PROCEDURE:   btnStart_Click()

    'DESCRIPTION: Event Handler called when the user clicks 'Start' TCP/IP Communication

    'PARAMETERS:  N/A

    'RETURNED:    N/A

    '*********************************************************************
    Private Sub btnStart_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnStart.Click
        On Error GoTo ErrTrap

        Dim ipAddress As String
        ipAddress = txtIP1.Text & "." & txtIP2.Text & "." & txtIP3.Text & "." & txtIP4.Text
        ' start the Analyzer
        Call objAnalyzer_m.OpenComm(ipAddress, txtPort.Text)
        ' change the UI to a "communicating state"
        nudPortNumber.Enabled = False
        cmdStart.Enabled = False
        cmdStop.Enabled = False
        'cmdTx.Enabled = True

        txtIP1.Enabled = False
        txtIP2.Enabled = False
        txtIP3.Enabled = False
        txtIP4.Enabled = False
        txtPort.Enabled = False
        btnStart.Enabled = False
        btnStop.Enabled = True
        btnBrowseFile.Enabled = True
        btnFileTransmit.Enabled = True

        Exit Sub
ErrTrap:
        ' can't raise an error here because there's nothing up the call stack to trap it
        Call HandleError(Err.Number, Err.Source & " | Main.btnStart_Click", Err.Description)
    End Sub
    '***********************************************************************

    'PROCEDURE:   cmdStop_Click()

    'DESCRIPTION: Event handler for when the user clicks stop

    'PARAMETERS:  N/A

    'RETURNED:    N/A

    '*********************************************************************
    Private Sub cmdStop_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles cmdStop.Click
        On Error GoTo ErrTrap

        ' stop the state machine
        Call objAnalyzer_m.CloseComm(SERIALCOMM)

        ' change the UI to "idle"
        nudPortNumber.Enabled = True
        cmdStart.Enabled = True
        cmdStop.Enabled = False
        cmdTx.Enabled = False
        btnStopTransmit.Enabled = False

        txtIP1.Enabled = True
        txtIP2.Enabled = True
        txtIP3.Enabled = True
        txtIP4.Enabled = True
        txtPort.Enabled = True
        btnStart.Enabled = True
        btnStop.Enabled = False
        btnBrowseFile.Enabled = False
        btnFileTransmit.Enabled = False

        Exit Sub
ErrTrap:
        ' can't raise an error here because there's nothing up the call stack to trap it
        Call HandleError(Err.Number, Err.Source & " | Main.cmdStop_Click", Err.Description)
    End Sub

    '***********************************************************************

    'PROCEDURE:   txtSequence_KeyPress()

    'DESCRIPTION: Event handler for when the user presses a key in the Tx sequence edit box

    'PARAMETERS:  KeyAscii - the key pressed

    'RETURNED:    N/A

    '*********************************************************************
    Private Sub txtSequence_KeyPress(ByVal eventSender As System.Object, ByVal eventArgs As System.Windows.Forms.KeyPressEventArgs) Handles txtSequence.KeyPress
        Dim KeyAscii As Short = Asc(eventArgs.KeyChar)
        On Error GoTo ErrTrap
        Dim record As Object
        Dim recordOK As Boolean
        Dim checkKey As String

        ' the backspace key is allowed
        If (KeyAscii = BS) Then
            GoTo EventExitSub
        End If

        ' convert the key to uppercase
        checkKey = UCase(Chr(KeyAscii))

        recordOK = False

        ' check each allowed record type
        For Each record In astrRecTypeID_m
            If (checkKey = record) Then
                ' the key is an allowed record type so stop looking
                recordOK = True
                Exit For
            End If
        Next record

        If (recordOK = True) Then
            ' replace the pressed key with the uppercase version
            KeyAscii = Asc(checkKey)
        Else
            ' bad key so nullify it
            Beep()
            KeyAscii = 0
        End If

        GoTo EventExitSub
ErrTrap:
        ' can't raise an error here because there's nothing up the call stack to trap it
        Call HandleError(Err.Number, Err.Source & " | Main.txtSequence_KeyPress", Err.Description)
EventExitSub:
        eventArgs.KeyChar = Chr(KeyAscii)
        If KeyAscii = 0 Then
            eventArgs.Handled = True
        End If
    End Sub

    '***********************************************************************

    'PROCEDURE:   cmdTx_Click()

    'DESCRIPTION: Event handler for when the user clicks cmdTx (Transmit button)

    'PARAMETERS:  N/A

    'RETURNED:    N/A

    '*********************************************************************
    Private Sub cmdTx_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles cmdTx.Click
        On Error GoTo ErrTrap
        lvwRecord.HideTextBox() 'do not show TextBox after click "Transmit". Snow 3/4/2016

        cmdTx.Enabled = False 'disable Transmit button before finish this transmission, prevent to click to transmit more before previous finished
        btnStopTransmit.Enabled = True
        txtSequence.Focus()
        Call objAnalyzer_m.TransmitRecords(txtSequence.Text)

        Exit Sub
ErrTrap:
        ' can't raise an error here because there's nothing up the call stack to trap it
        Call HandleError(Err.Number, Err.Source & " | Main.cmdTx_Click", Err.Description)
    End Sub

    '***********************************************************************

    'PROCEDURE:   txtFieldDelimiter_Change()

    'DESCRIPTION: Event handler for when the user changes the transmission field delimiter

    'PARAMETERS:  N/A

    'RETURNED:    N/A

    '*********************************************************************
    'UPGRADE_WARNING: Event txtFieldDelimiter.TextChanged may fire when form is initialized. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="88B12AE1-6DE0-48A0-86F1-60C0686C026A"'
    Private Sub txtFieldDelimiter_TextChanged(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles txtFieldDelimiter.TextChanged
        On Error GoTo ErrTrap

        If (objAnalyzer_m Is Nothing = False) Then
            objAnalyzer_m.TxFieldDelimiter = txtFieldDelimiter.Text
        End If

        Exit Sub
ErrTrap:
        ' can't raise an error here because there's nothing up the call stack to trap it
        Call HandleError(Err.Number, Err.Source & " | Main.txtFieldDelimiter_Change", Err.Description)
    End Sub

    '***********************************************************************

    'PROCEDURE:   txtFieldDelimiter_Validate()

    'DESCRIPTION: Event handler for when the transmission field delimiter is validated

    'PARAMETERS:  Cancel - whether or not to shift focus from the control

    'RETURNED:    N/A

    '*********************************************************************
    Private Sub txtFieldDelimiter_Validating(ByVal eventSender As System.Object, ByVal eventArgs As System.ComponentModel.CancelEventArgs) Handles txtFieldDelimiter.Validating
        Dim Cancel As Boolean = eventArgs.Cancel
        ' do not allow a zero length field delimiter
        If (txtFieldDelimiter.Text = "") Then
            Cancel = True
            Call MsgBox(My.Resources.ResourceManager.GetString("str" + CStr(BLANKDELIMITERERRTXT)), MsgBoxStyle.Exclamation Or MsgBoxStyle.OkOnly, "Error")
        Else
            Cancel = False
            objAnalyzer_m.TxFieldDelimiter = txtFieldDelimiter.Text
        End If
        GoTo EventExitSub
ErrTrap:
        ' can't raise an error here because there's nothing up the call stack to trap it
        Call HandleError(Err.Number, Err.Source & " | Main.txtFieldDelimiter_Validate", Err.Description)
EventExitSub:
        eventArgs.Cancel = Cancel
    End Sub

    '***********************************************************************

    'PROCEDURE:   cmdClear_Click()

    'DESCRIPTION: Event handler for when the user clicks clear

    'PARAMETERS:  N/A

    'RETURNED:    N/A

    '*********************************************************************
    Private Sub cmdClear_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles cmdClear.Click
        On Error GoTo ErrTrap
        txtDisplay.Text = ""

        Exit Sub
ErrTrap:
        ' can't raise an error here because there's nothing up the call stack to trap it
        Call HandleError(Err.Number, Err.Source & " | Main.cmdClear_Click", Err.Description)
    End Sub

    '***********************************************************************

    'PROCEDURE:   cmdSave_Click()

    'DESCRIPTION: Event handler for when the user clicks save

    'PARAMETERS:  N/A

    'RETURNED:    N/A

    '*********************************************************************
    Private Sub cmdSave_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles cmdSave.Click
        On Error GoTo ErrTrap
        Dim fso As Scripting.FileSystemObject

        ' set up the save dialog
        dlgSave.InitialDirectory = GetSetting(APPLICATIONTITLE, SECTIONKEY, LASTPATHKEY, My.Application.Info.DirectoryPath)

        Dim dr As DialogResult
        dr = dlgSave.ShowDialog() ' show the save dialog box
        If (dr = Windows.Forms.DialogResult.OK) Then
            fso = New Scripting.FileSystemObject
            Call SaveSetting(APPLICATIONTITLE, SECTIONKEY, LASTPATHKEY, fso.GetParentFolderName(dlgSave.FileName))

            txtDisplay.SaveFile(dlgSave.FileName, RichTextBoxStreamType.PlainText)
            'Call SaveFile(dlgSave.FileName, txtDisplay.Text)

            fso = Nothing
            GC.Collect() 'nalini
        End If

        Exit Sub
ErrTrap:
        'UPGRADE_NOTE: Object fso may not be destroyed until it is garbage collected. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
        fso = Nothing
        GC.Collect() 'nalini
        If (Err.Number <> DialogResult.Cancel) Then
            ' can't raise an error here because there's nothing up the call stack to trap it
            Call HandleError(Err.Number, Err.Source & " | Main.cmdSave_Click", Err.Description)
        End If
    End Sub

    '***********************************************************************

    'PROCEDURE:   cboRecordType_Click()

    'DESCRIPTION: Event handler for when the user clicks the record type combo box

    'PARAMETERS:  N/A

    'RETURNED:    N/A

    '*********************************************************************
    'UPGRADE_WARNING: Event cboRecordType.SelectedIndexChanged may fire when form is initialized. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="88B12AE1-6DE0-48A0-86F1-60C0686C026A"'
    Private Sub cboRecordType_SelectedIndexChanged(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles cboRecordType.SelectedIndexChanged
        On Error GoTo ErrTrap
        Dim item As mscomctl.ListItem
        Dim fieldList() As String
        Dim valueList() As String
        Dim index As Integer
        Dim maxIndex As Integer

        fieldList = objAnalyzer_m.RecFieldList(astrRecTypeID_m(cboRecordType.SelectedIndex))
        valueList = objAnalyzer_m.RecValueList(astrRecTypeID_m(cboRecordType.SelectedIndex))

        maxIndex = UBound(fieldList)

        Call lvwRecord.ListItems.Clear()

        For index = 0 To maxIndex

            item = lvwRecord.ListItems.Add(, , fieldList(index))
            'auto fill Date&Time in Header "Message Date and Time". -- Snow 3/25/2016
            'Not auto fill Test Order "Requested/Ordered Date and Time" because when Cancel an order, the auto filled current time will not match added order before. -- Snow 4/6/2016
            If (astrRecTypeID_m(cboRecordType.SelectedIndex) = "H" And index = 13) Then ' Or (astrRecTypeID_m(cboRecordType.SelectedIndex) = "O" And index = 6) Then
                item.SubItems(VALUECOL) = DateTime.Now.ToString("yyyyMMddHHmmss") 'fill current date/time
                'update value in database. also allow user to change to value
                objAnalyzer_m.RecFieldValue(astrRecTypeID_m(cboRecordType.SelectedIndex), fieldList(index)) = DateTime.Now.ToString("yyyyMMddHHmmss")
            Else
                item.SubItems(VALUECOL) = valueList(index)
            End If

        Next index

        '----------after change Record Type selection do not show previous TextBox. Snow Shi 3/1/2016
        lvwRecord.HideTextBox()
        Exit Sub
ErrTrap:
        ' can't raise an error here because there's nothing up the call stack to trap it
        Call HandleError(Err.Number, Err.Source & " | Main.lboRecordType_Click", Err.Description)
    End Sub

    '***********************************************************************

    'PROCEDURE:   lvwRecord_ItemClick()

    'DESCRIPTION: Event handler for when user clicks a row in the list view

    'PARAMETERS:  item - which item in the list was selected
    '             Button - which mouse button initiated the click
    '             SecondClick - whether it's the second time the item was clicked

    'RETURNED:    N/A

    '*********************************************************************
    Private Sub lvwRecord_ItemClick(ByVal sender As System.Object, ByVal e As AxExtLVCTL.__ExtLV_ItemClickEvent) Handles lvwRecord.ItemClick
        On Error GoTo ErrTrap

        With lvwRecord
            ' see if user clicked value column
            If (.SubItemClicked = VALUECOL) Then
                Call .ShowTextBox(e.item.SubItems(VALUECOL)) ' allow user to edit the value

                lvwRecord.Focus() 'set Focus on ListView so when mouse out of it will raise LostFocus Event
            End If
        End With
        Exit Sub
ErrTrap:
        ' can't raise an error here because there's nothing up the call stack to trap it
        Call HandleError(Err.Number, Err.Source & " | RecordEdit.lvwRecord_ItemClick", Err.Description)
    End Sub

    '***********************************************************************

    'PROCEDURE:   lvwRecord_ChangeComplete()

    'DESCRIPTION: Event handler for changing the text box value

    'PARAMETERS:  OrigText - previous text box value
    '             NewText - current text box value

    'RETURNED:    N/A

    '*********************************************************************
    Private Sub lvwRecord_ChangeComplete(ByVal eventSender As System.Object, ByVal eventArgs As AxExtLVCTL.__ExtLV_ChangeCompleteEvent) Handles lvwRecord.ChangeComplete
        On Error GoTo ErrTrap

        With lvwRecord
            ' replace the text with the entry
            'UPGRADE_WARNING: Lower bound of collection lvwRecord.SelectedItem has changed from 1 to 0. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="A3B628A0-A810-4AE2-BFA2-9E7A29EB9AD0"'

            .SelectedItem.SubItems(VALUECOL) = eventArgs.newText
            objAnalyzer_m.RecFieldValue(astrRecTypeID_m(cboRecordType.SelectedIndex), .SelectedItem.Text) = eventArgs.newText
        End With

        Exit Sub
ErrTrap:
        ' can't raise an error here because there's nothing up the call stack to trap it
        Call HandleError(Err.Number, Err.Source & " | RecordEdit.lvwRecord_ChangeComplete", Err.Description)
    End Sub


   

    '***********************************************************************

    'PROCEDURE:   lvwRecord_OnMoveOut()

    'DESCRIPTION: Event handler for Mouse Moveover to save text box value in case the value changed before moveover

    'PARAMETERS:  OrigText - previous text box value
    '             NewText - current text box value

    'RETURNED:    N/A

    '*********************************************************************
    Private Sub lvwRecord_OnMoveOut(ByVal eventSender As System.Object, ByVal eventArgs As EventArgs) Handles lvwRecord.Validated 'lvwRecord.Leave is OK too 
        On Error GoTo ErrTrap

        With lvwRecord
            ' replace the text with the entry
            'UPGRADE_WARNING: Lower bound of collection lvwRecord.SelectedItem has changed from 1 to 0. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="A3B628A0-A810-4AE2-BFA2-9E7A29EB9AD0"'

            '---- selectedItemValue after changed. save the change before move out of ListView lvwRecord to other controls. Snow Shi 3/2/2016
            objAnalyzer_m.RecFieldValue(astrRecTypeID_m(cboRecordType.SelectedIndex), .SelectedItem.Text) = eventSender.CtlText
            .SelectedItem.SubItems(1) = eventSender.ctlText
            .HideTextBox()

        End With

        Exit Sub
ErrTrap:
        ' can't raise an error here because there's nothing up the call stack to trap it
        Call HandleError(Err.Number, Err.Source & " | RecordEdit.lvwRecord_Validated", Err.Description)
    End Sub


    '***********************************************************************

    'PROCEDURE:   objAnalyzer_m_OnUpdate()

    'DESCRIPTION: Event handler for when state machine has an update

    'PARAMETERS:  update - the update message

    'RETURNED:    N/A

    '*********************************************************************
    'UPGRADE_NOTE: update was upgraded to update_Renamed. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
    Private Sub objAnalyzer_m_OnUpdate(ByVal update_Renamed As String) Handles objAnalyzer_m.OnUpdate
        On Error GoTo ErrTrap

        ' separate the TX and RX sequences with a carriage return regardless of verbosity mode
        If ((InStr(update_Renamed, My.Resources.ResourceManager.GetString("str" + CStr(TXSEQUENCETXT))) = 1) Or (InStr(update_Renamed, My.Resources.ResourceManager.GetString("str" + CStr(RXENQTXT))) = 1)) Then
            'added nwr 2/18/2014
            If txtDisplay.InvokeRequired = True Then
                txtDisplay.Invoke(New UpdateTextHandler(AddressOf AppendTxtBox), New Object() {vbCrLf})
            Else
                txtDisplay.Text = txtDisplay.Text & vbCrLf
            End If
        End If

        ' replace the control characters for readability
        update_Renamed = Replace(update_Renamed, Chr(&H2S), "<STX>")
        update_Renamed = Replace(update_Renamed, Chr(&H3S), "<ETX>")
        update_Renamed = Replace(update_Renamed, Chr(&HAS), "<LF>")
        update_Renamed = Replace(update_Renamed, Chr(&HDS), "<CR>")

        'display the update according to the verbosity mode
        If (chkVerbose.CheckState = System.Windows.Forms.CheckState.Unchecked) Then 'Verbose is not checked
            ' the only updates that get displayed in the non-verbose mode are the frames
            If (update_Renamed.Contains(My.Resources.ResourceManager.GetString("str" + CStr(TXFRAMETXT)))) Then
                ' trim off everything, but the frame
                update_Renamed = DateTime.Now.ToString("yyyyMMddHHmmss") & ", " & Replace(update_Renamed, My.Resources.ResourceManager.GetString("str" + CStr(TXFRAMETXT)), "")
            ElseIf (update_Renamed.Contains(My.Resources.ResourceManager.GetString("str" + CStr(RXFRAMETXT)))) Then
                ' trim off everything, but the frame
                update_Renamed = DateTime.Now.ToString("yyyyMMddHHmmss") & ", " & Replace(update_Renamed, My.Resources.ResourceManager.GetString("str" + CStr(RXFRAMETXT)), "")
                'ElseIf (update_Renamed.Contains("Order #")) Then
                '   update_Renamed = DateTime.Now.ToString("yyyyMMddHHmmss") & ", " & update_Renamed
            Else
                If (chkENQ.CheckState = System.Windows.Forms.CheckState.Unchecked) Then
                    Exit Sub  ' don't display anything
                Else 'ENQ is checked, show ENQ, ACK even in non-Verbose mode -- Snow 4/5/2016
                    If (update_Renamed.Contains(My.Resources.ResourceManager.GetString("str" + CStr(RXENQTXT))) Or update_Renamed.Contains(My.Resources.ResourceManager.GetString("str" + CStr(RXACKTXT))) Or update_Renamed.Contains(My.Resources.ResourceManager.GetString("str" + CStr(TXACKTXT))) Or update_Renamed.Contains(My.Resources.ResourceManager.GetString("str" + CStr(TXENQTXT))) Or update_Renamed.Contains(My.Resources.ResourceManager.GetString("str" + CStr(RXNAKTXT)))) Then
                        'update_Renamed = DateTime.Now.ToString("yyyyMMddHHmmss") & ", " & update_Renamed 'add Date&Time to each line at the beginning. -- Snow 3/25/2016
                    Else
                        Exit Sub  ' don't display anything
                    End If
                End If
            End If
        Else 'Verbose is checked
            'if ENQ checkbox is not checked, do not show ENQ, ACK/NAK even in Verbose mode -- Snow 4/5/2016
            If (chkENQ.CheckState = System.Windows.Forms.CheckState.Unchecked) Then
                If (update_Renamed.Contains(My.Resources.ResourceManager.GetString("str" + CStr(RXENQTXT))) Or update_Renamed.Contains(My.Resources.ResourceManager.GetString("str" + CStr(RXACKTXT))) Or update_Renamed.Contains(My.Resources.ResourceManager.GetString("str" + CStr(TXACKTXT))) Or update_Renamed.Contains(My.Resources.ResourceManager.GetString("str" + CStr(TXENQTXT))) Or update_Renamed.Contains(My.Resources.ResourceManager.GetString("str" + CStr(RXNAKTXT)))) Then

                    Exit Sub
                End If
            End If

            If (update_Renamed.Equals("")) Then
                'nothing
            Else

                ' separate the start and stop state machine sequences with a carriage return
                If (update_Renamed.Contains(My.Resources.ResourceManager.GetString("str" + CStr(STARTTXT))) Or update_Renamed.Contains(My.Resources.ResourceManager.GetString("str" + CStr(STOPTXT)))) Then
                    update_Renamed = vbCrLf & DateTime.Now.ToString("yyyyMMddHHmmss") & ", " & update_Renamed 'add Date&Time to each line at the beginning. -- Snow 3/25/2016
                ElseIf (update_Renamed.Contains(My.Resources.ResourceManager.GetString("str" + CStr(IDLETXT)))) Then
                    update_Renamed = DateTime.Now.ToString("yyyyMMddHHmmss") & ", " & update_Renamed & vbCrLf 'add line change and Date&Time to each line at the beginning of Idle. -- Snow 5/16/2016
                ElseIf (update_Renamed.Contains(My.Resources.ResourceManager.GetString("str" + CStr(RXENQTXT))) Or update_Renamed.Contains(My.Resources.ResourceManager.GetString("str" + CStr(RXACKTXT))) Or update_Renamed.Contains(My.Resources.ResourceManager.GetString("str" + CStr(TXACKTXT))) Or update_Renamed.Contains(My.Resources.ResourceManager.GetString("str" + CStr(TXENQTXT))) Or update_Renamed.Contains(My.Resources.ResourceManager.GetString("str" + CStr(RXNAKTXT)))) Then
                    'nothing already add Date in ASTM1381.vb
                    '    update_Renamed = DateTime.Now.ToString("yyyyMMddHHmmss") & ", " & update_Renamed
                Else
                    update_Renamed = DateTime.Now.ToString("yyyyMMddHHmmss") & ", " & update_Renamed 'add Date&Time to each line at the beginning. -- Snow 3/25/2016
                End If

            End If
        End If



        'added nwr 2/18/2014
        If txtDisplay.InvokeRequired = True Then
            txtDisplay.Invoke(New UpdateTextHandler(AddressOf AppendTxtBox), New Object() {vbCrLf & update_Renamed})
        Else
            If (update_Renamed.Equals("")) Then
                'don't append this line
            Else
                txtDisplay.Text = txtDisplay.Text & vbCrLf & update_Renamed
            End If
            txtDisplay.SelectionStart = Len(txtDisplay.Text)
        End If
        Exit Sub
ErrTrap:
        ' can't raise an error here because there's nothing up the call stack to trap it
        Call HandleError(Err.Number, Err.Source & " | Main.objAnalyzer_m_OnUpdate", Err.Description)
    End Sub

    '***********************************************************************

    'PROCEDURE:   objAnalyzer_m_OnError()

    'DESCRIPTION: Event handler for when the analyzer has an unexpected error

    'PARAMETERS:  N/A

    'RETURNED:    N/A

    '*********************************************************************
    Private Sub objAnalyzer_m_OnError() Handles objAnalyzer_m.OnError
        Err.Source = Err.Source & " | Main.objAnalyzer_m_OnError"
        Call HandleError(Err.Number, Err.Source, Err.Description)
    End Sub

    '***********************************************************************

    'PROCEDURE:   HandleError()

    'DESCRIPTION: Logs errors and displays message boxes to user if necessary

    'PARAMETERS:  inNumber - error number
    '             inSource - source code audit trail
    '             inDescription - description of error

    'RETURNED:    N/A

    '*********************************************************************
    Private Sub HandleError(ByVal inNumber As Integer, ByVal inSource As String, ByVal inDescription As String)
        On Error GoTo ErrTrap
        If (inNumber = 8002) Then
            Call MsgBox(inDescription, MsgBoxStyle.Critical Or MsgBoxStyle.OkOnly, "Error")
        Else

            Call MsgBox("Number:  " & inNumber & vbCrLf & vbCrLf & "Audit Trail:  " & inSource & vbCrLf & vbCrLf & "Description:  " & inDescription, MsgBoxStyle.Exclamation Or MsgBoxStyle.OkOnly, "Error")
        End If
        Exit Sub
ErrTrap:
        ' can't raise an error here because there's nothing up the call stack to trap it
        Call MsgBox("Number:  " & Err.Number & vbCrLf & vbCrLf & "Audit Trail:  " & Err.Source & vbCrLf & vbCrLf & "Description:  " & Err.Description, MsgBoxStyle.Exclamation Or MsgBoxStyle.OkOnly, "Error")
        Resume Next ' this entire procedure must be executed if possible
    End Sub

    '***********************************************************************

    'PROCEDURE:   txtIP1_KeyPress()

    'DESCRIPTION: Event handler for when the user presses a key inside this
    'textbox

    'PARAMETERS:  N/A

    'RETURNED:   N/A

    '*********************************************************************
    Private Sub txtIP1_KeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtIP1.KeyPress
        If ((Convert.ToByte(e.KeyChar()) <> BS) And (Char.IsDigit(e.KeyChar()) = False)) Then
            e.Handled = True
        End If
    End Sub

    '***********************************************************************

    'PROCEDURE:   txtIP2_KeyPress()

    'DESCRIPTION: Event handler for when the user presses a key inside this
    'textbox

    'PARAMETERS:  N/A

    'RETURNED:   N/A

    '*********************************************************************
    Private Sub txtIP2_KeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtIP2.KeyPress
        If ((Convert.ToByte(e.KeyChar()) <> BS) And (Char.IsDigit(e.KeyChar()) = False)) Then
            e.Handled = True
        End If
    End Sub
    '***********************************************************************

    'PROCEDURE:   txtIP3_KeyPress()

    'DESCRIPTION: Event handler for when the user presses a key inside this
    'textbox

    'PARAMETERS:  N/A

    'RETURNED:    N/A

    '*********************************************************************
    Private Sub txtIP3_KeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtIP3.KeyPress
        If ((Convert.ToByte(e.KeyChar()) <> BS) And (Char.IsDigit(e.KeyChar()) = False)) Then
            e.Handled = True
        End If
    End Sub

    '***********************************************************************

    'PROCEDURE:   txtIP1_TextChanged()

    'DESCRIPTION: Event handler for when the text inside the textbox changes

    'PARAMETERS:  N/A

    'RETURNED:    N/A

    '*********************************************************************
    Private Sub txtIP1_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtIP1.TextChanged
        If (txtIP1.TextLength = 3) Then
            txtIP2.Focus()
        End If
    End Sub

    '***********************************************************************

    'PROCEDURE:   txtIP2_TextChanged()

    'DESCRIPTION: Event handler for when the text inside the textbox changes

    'PARAMETERS:  

    'RETURNED:    

    '*********************************************************************
    Private Sub txtIP2_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtIP2.TextChanged
        If (txtIP2.TextLength = 3) Then
            txtIP3.Focus()
        End If
    End Sub

    '***********************************************************************

    'PROCEDURE:   txtIP4_KeyPress()

    'DESCRIPTION: Event handler for when the user presses a key inside this
    'textbox

    'PARAMETERS:  N/A

    'RETURNED:    N/A

    '*********************************************************************
    Private Sub txtIP4_KeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtIP4.KeyPress
        If ((Convert.ToByte(e.KeyChar()) <> BS) And (Char.IsDigit(e.KeyChar()) = False)) Then
            e.Handled = True
        End If
    End Sub

    '***********************************************************************

    'PROCEDURE:   txtIP3_TextChanged()

    'DESCRIPTION: Event handler for when the text inside the textbox changes

    'PARAMETERS:  

    'RETURNED:    

    '*********************************************************************
    Private Sub txtIP3_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtIP3.TextChanged
        If (txtIP3.TextLength = 3) Then
            txtIP4.Focus()
        End If
    End Sub

    '***********************************************************************

    'PROCEDURE:   txtPort_KeyPress()

    'DESCRIPTION: Event handler for when the user presses a key inside this
    'textbox

    'PARAMETERS:  

    'RETURNED:    

    '*********************************************************************
    Private Sub txtPort_KeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtPort.KeyPress
        If ((Convert.ToByte(e.KeyChar()) <> BS) And (Char.IsDigit(e.KeyChar()) = False)) Then
            e.Handled = True
        End If
    End Sub


    '***********************************************************************

    'PROCEDURE:   btnStop_Click()

    'DESCRIPTION: Event handler for when the user clicks on 'Stop' 
    'TCP/IP communication

    'PARAMETERS: N/A

    'RETURNED:   N/A

    '*********************************************************************
    Private Sub btnStop_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnStop.Click
        On Error GoTo ErrTrap

        ' stop the state machine
        Call objAnalyzer_m.CloseComm(TCPIPCOMM)

        ' change the UI to "idle"
        nudPortNumber.Enabled = True
        cmdStart.Enabled = True
        cmdStop.Enabled = False
        cmdTx.Enabled = False
        btnStopTransmit.Enabled = False

        txtIP1.Enabled = True
        txtIP2.Enabled = True
        txtIP3.Enabled = True
        txtIP4.Enabled = True
        txtPort.Enabled = True
        btnStart.Enabled = True
        btnStop.Enabled = False

        Exit Sub
ErrTrap:
        ' can't raise an error here because there's nothing up the call stack to trap it
        Call HandleError(Err.Number, Err.Source & " | Main.btnStop_Click", Err.Description)
    End Sub

    Private Sub ExitToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ExitToolStripMenuItem.Click
        Application.Exit()
    End Sub

    Private Sub AboutASTMTestToolToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AboutASTMTestToolToolStripMenuItem.Click
        Dim version As New About()
        version.ShowDialog()

    End Sub

    Private Sub ASTMProtocolToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ASTMProtocolToolStripMenuItem.Click
        System.Diagnostics.Process.Start("Manual.pdf")
    End Sub

    Private Sub ASTMToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ASTMToolStripMenuItem.Click
        System.Diagnostics.Process.Start("ASTM Protocol Sample Records.pdf")
    End Sub

    Private Sub ASTMProtocolInputRecordsToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ASTMProtocolInputRecordsToolStripMenuItem.Click
        System.Diagnostics.Process.Start("ASTM Protocol Input Records.pdf")
    End Sub

    Private Sub ASTMProtocolOutputRecordsToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ASTMProtocolOutputRecordsToolStripMenuItem.Click
        System.Diagnostics.Process.Start("ASTM Protocol Output Records.pdf")
    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

    'added nwr 2/18/2014
    Public Sub UpdateBtnEnabled(ByVal enabled As Boolean)
        cmdTx.Enabled = enabled
    End Sub

    'added nwr 2/18/2014
    Public Sub AppendTxtBox(ByVal text As String)
        txtDisplay.Text = txtDisplay.Text & text
        txtDisplay.SelectionStart = Len(txtDisplay.Text)
    End Sub

    'add Stop transmit button to enable function that terminate transmit process in the middle. -- Snow 4/5/2016
    Private Sub btnStopTransmit_Click(sender As System.Object, e As System.EventArgs) Handles btnStopTransmit.Click
        'stop sending signal, send EOT, clear buffer
        On Error GoTo ErrTrap

        lvwRecord.HideTextBox() 'do not show TextBox after click "Transmit". Snow 3/4/2016
        txtSequence.Focus()
        Call objAnalyzer_m.StopTransmit()

        cmdTx.Enabled = True
        btnStopTransmit.Enabled = False
        Exit Sub
ErrTrap:
        Call HandleError(Err.Number, Err.Source & " | Main.btnStopTransmit_Click", Err.Description)
    End Sub

    'Show ENQ, ACK/NAK when checked
    Private Sub chkENQ_CheckedChanged(sender As Object, e As EventArgs) Handles chkENQ.CheckedChanged

    End Sub

    '*************************************************************************************************************************************************************
    ' Transmit from a saved .xml which contains mandatory and optional fields values for only Test Order -- Sequence=HPOL
    ' the .xml file must be in same format with VetXML Kiosk using, per VetSync API v1.0 Developer Guide. And add new tags <ASTMToolDetails> which not in VetXML.*
    ' Snow 4/28/2016
    '*************************************************************************************************************************************************************

    Private Sub btnFileTransmit_Click(sender As Object, e As EventArgs) Handles btnFileTransmit.Click
        On Error GoTo ErrTrap

        If Not BackgroundWorker2.IsBusy = True Then
            btnFileTransmit.Enabled = False
            btnBrowseFile.Enabled = False
            BackgroundWorker2.RunWorkerAsync()
            BackgroundWorker2.CancelAsync()
            'BackgroundWorker2.Dispose()

        End If
        Threading.Thread.Sleep(1000) 'wait for 1 sec before exit Sub so have time to invoke BackgroundWorker.RunWorkerAsync()
        Exit Sub
ErrTrap:
        Call HandleError(Err.Number, Err.Source & " | Main.btnFileTransmit_Click", Err.Description)
    End Sub


    Private Sub BackgroundWorker2_ProgressChanged(ByVal sender As Object, ByVal e As System.ComponentModel.ProgressChangedEventArgs) Handles BackgroundWorker2.ProgressChanged

        '---ProgressBar1.Value = e.ProgressPercentage

    End Sub

    '*********************************
    ' after complete background work *
    ' Snow 4/28/2016                 *
    '*********************************
    Private Sub BackgroundWorker2_RunWorkerCompleted(ByVal sender As Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles BackgroundWorker2.RunWorkerCompleted
        btnFileTransmit.Enabled = True
        btnBrowseFile.Enabled = True
        MsgBox("All Order Sending is Done")

    End Sub

    Private Sub BackgroundWorker2_DoWork(sender As Object, e As DoWorkEventArgs) Handles BackgroundWorker2.DoWork


        'On Error GoTo ErrTrap
        Try

            Dim strElementName As String
            Dim strElementValue As String
            'create template array of H, P, O, L
            Dim arrayHeader(13) As String
            Dim arrayPatient(34) As String
            Dim arrayOrder(30) As String
            Dim arrayTerminator(2) As String
            arrayHeader = {"H", "\^&", "", "", "", "", "", "", "", "", "", "", "", DateTime.Now.ToString("yyyyMMddHHmmss")}
            arrayPatient = {"P", "1", "", "", "", "", "", "", "", "", "", "", "", "", "^^", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", ""}
            arrayOrder = {"O", "1", "", "", "", "", "", "", "", "", "", "N", "", "", "", "", "", "", "", "", "", "", "", "", "", "O", "", "", "", "", ""} '{"O", "1", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "O", "", "", "", "", "", "", "", ""}
            arrayTerminator = {"L", "1", ""}
            ' btnFileTransmit. call RunWorkerAsync on the BackgroundWorker

            'read .xml file

            Dim currentNum As Integer = 0
            Dim endRequestNum As Integer = 0
            Dim isMutipleRotor As Boolean = False
            Dim orderNum As Integer = 0
            If File.Exists(TextBoxFileName.Text) And TextBoxFileName.Text.ToLower.EndsWith(".xml") Then
                'txtDisplay.Text = txtDisplay.Text & vbCrLf & "Sending: HPOL" & vbCrLf //commented by Snow 1/14/2019 because txtDisplay accessed from a thread other than the thread it was created on
                Dim sr As System.Xml.XmlTextReader = New System.Xml.XmlTextReader(TextBoxFileName.Text)
                'arrayPatient(14) = "^^" 'pre-define delimiter between OnwerID^age^DOV

                Do While sr.Read() 'sr.Peek() >= 0
                    Select Case sr.NodeType
                        Case XmlNodeType.Element 'beginning of element
                            strElementName = ""
                            currentNum = currentNum + 1

                            If sr.HasAttributes Then 'If attributes exist
                                While sr.MoveToNextAttribute()

                                End While
                            End If

                            strElementName = sr.Name 're-assign Element name 
                            If (sr.Name = "LabRequest" And endRequestNum = currentNum - 1) Then '???don't need
                                isMutipleRotor = True
                                arrayHeader(2) = ""
                                arrayHeader(3) = ""
                                arrayOrder(4) = ""
                                arrayHeader(9) = ""
                                'arrayPatient(14) = "^^" 'pre-define delimiter between OnwerID^age^DOV
                            End If
                        Case XmlNodeType.Text 'Display the text in each element.
                            'Console.WriteLine(sr.Value)
                            'strElementName = sr.Name 'use Element name from last Element <> 
                            strElementValue = sr.Value 'get value for last Element <Element>value
                            currentNum = currentNum + 1
                            If (strElementName = "PIMSName") Then 'Sender name
                                arrayHeader(4) = strElementValue
                            ElseIf (strElementName = "OwnerName") Then
                                arrayPatient(14) = strElementValue + arrayPatient(14) 'SpecialField2=Owner^Age^DOV
                            ElseIf (strElementName = "VetID") Then 'Physician
                                arrayOrder(16) = strElementValue
                            ElseIf (strElementName = "AnimalID") Then
                                arrayPatient(2) = strElementValue
                            ElseIf (strElementName = "AnimalName") Then
                                arrayPatient(4) = strElementValue
                            ElseIf (strElementName = "Gender") Then
                                arrayPatient(8) = strElementValue
                            ElseIf (strElementName = "Species") Then
                                arrayPatient(15) = strElementValue 'Special Field2
                            ElseIf (strElementName = "Age") Then
                                arrayPatient(14) = arrayPatient(14).Substring(0, arrayPatient(14).IndexOf("^") + 1) + strElementValue + arrayPatient(14).Substring(arrayPatient(14).LastIndexOf("^")) 'arrayPatient(14) + "^" + strElementValue DOV
                            ElseIf (strElementName = "DateOfBirth") Then
                                arrayPatient(7) = strElementValue 'Collect DateTime
                            ElseIf (strElementName = "SampleID") Then
                                arrayPatient(3) = strElementValue
                            ElseIf (strElementName = "Race") Then
                                arrayPatient(9) = strElementValue
                            ElseIf (strElementName = "TelephoneNumber") Then
                                arrayPatient(12) = strElementValue
                            ElseIf (strElementName = "DateOfVaccination") Then
                                arrayPatient(14) = arrayPatient(14) + strElementValue 'arrayPatient(14) + "^" + strElementValue 'Special Field1
                            ElseIf (strElementName = "Location") Then
                                arrayPatient(25) = strElementValue
                            ElseIf (strElementName = "OrderDateTime") Then
                                arrayOrder(6) = strElementValue 'Order DateTime
                            ElseIf (strElementName = "ActionCode") Then
                                arrayOrder(11) = strElementValue 'Action Code: N-new, C-cancel, Q-qc test (11)
                            ElseIf (strElementName = "LaboratoryField1") Then
                                arrayOrder(20) = strElementValue 'laboratory field 1
                            ElseIf (strElementName = "TestCode") Then
                                arrayOrder(4) = strElementValue 'Universal Test ID ^^^rotorID
                            ElseIf (strElementName = "DeviceID") Then
                                arrayHeader(9) = strElementValue 'Serial#
                            End If
                            'arrayOrder(25) = "O"
                        Case XmlNodeType.EndElement 'Display end of element.
                            currentNum = currentNum + 1
                            strElementValue = ""
                            If (sr.Name = "LabRequest") Then
                                If (isMutipleRotor = True) Then

                                End If
                                endRequestNum = currentNum
                                'Task.Factory.StartNew(() >= objAnalyzer_m.TransmitRecords("HPOL", arrayHeader, arrayPatient, arrayOrder, arrayTerminator)
                                orderNum = orderNum + 1
                                If (arrayPatient(14) = "^^") Then 'if no input in [Speical Field 1]
                                    arrayPatient(14) = ""
                                End If

                                '-------------- start to send Order -------------------------
                                Call objAnalyzer_m.TransmitRecords("HPOL", arrayHeader, arrayPatient, arrayOrder, arrayTerminator, orderNum.ToString())

                                    If (chkENQ.CheckState = CheckState.Checked And chkVerbose.CheckState = CheckState.Checked) Then
                                        Threading.Thread.Sleep(2100) 'give time to finish this Call. Otherwise will jump to next sending
                                    ElseIf (chkENQ.CheckState = CheckState.Checked) Then
                                        Threading.Thread.Sleep(1950) 'give time to finish this Call. Otherwise will jump to next sending
                                    Else
                                        Threading.Thread.Sleep(1700) 'give time to finish this Call. Otherwise will jump to next sending Threading.Thread.Sleep(1500)
                                    End If
                                    '-------------- end of send Order -------------------------
                            ElseIf (sr.Name = "LabReport") Then 'close tag </LabReport> end of one patient
                                isMutipleRotor = False
                                arrayHeader = {"H", "\^&", "", "", "", "", "", "", "", "", "", "", "", DateTime.Now.ToString("yyyyMMddHHmmss")}
                                arrayPatient = {"P", "1", "", "", "", "", "", "", "", "", "", "", "", "", "^^", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", ""}
                                arrayOrder = {"O", "1", "", "", "", "", "", "", "", "", "", "N", "", "", "", "", "", "", "", "", "", "", "", "", "", "O", "", "", "", "", ""} '[11]N [25]O
                                arrayTerminator = {"L", "1", ""}
                            ElseIf (sr.Name = "LabReports") Then 'close tag </LabReports> end of the file
                                    isMutipleRotor = False

                                Exit Select
                            End If

                    End Select


                Loop
                sr.Close()


            End If


            Exit Sub

        Catch
            Call HandleError(Err.Number, Err.Source & " | Main.BackgroundWorker2_DoWork", Err.Description)
        End Try
    End Sub

    Private Sub btnCreateLibraryFile_Click(sender As Object, e As EventArgs) Handles btnCreateLibraryFile.Click
        On Error GoTo ErrTrap
        Dim createNewFile As CreateNewXMLFileForm = CreateNewXMLFileForm
        createNewFile.Location = New Point(10, 40)

        createNewFile.Show()
        Exit Sub
ErrTrap:
        Call HandleError(Err.Number, Err.Source & " | Main.btnCreateLibraryFile_Click", Err.Description)
    End Sub

    Private Sub btnBrowseFile_Click(sender As Object, e As EventArgs) Handles btnBrowseFile.Click
            On Error GoTo ErrTrap

            'Dim result As DialogResult = OpenFileDialog1.ShowDialog() ' Show the dialog.
            'If (result = DialogResult.OK) Then

            OpenFileDialog1.Title = "Test Order File"
            OpenFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) '"c:\Program Files (x86)" Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            OpenFileDialog1.Filter = "All files (*.xml)|*.xml|All files (*.XML)|*.XML"
            OpenFileDialog1.FilterIndex = 2
            OpenFileDialog1.RestoreDirectory = True
            If (OpenFileDialog1.ShowDialog() = DialogResult.OK) Then

                TextBoxFileName.Text = OpenFileDialog1.FileName
            End If
            'End If  
            Exit Sub
ErrTrap:
            Call HandleError(Err.Number, Err.Source & " | Main.btnBrowseFile_Click", Err.Description)
        End Sub

    Private Sub rdoPiccolo_CheckedChanged(sender As Object, e As EventArgs) Handles rdoPiccolo.CheckedChanged
        If (rdoPiccolo.Checked = True) Then
            rdoVetScan.Checked = False
        Else
            rdoVetScan.Checked = True
        End If
    End Sub

    Private Sub rdoVetScan_CheckedChanged(sender As Object, e As EventArgs) Handles rdoVetScan.CheckedChanged
        If (rdoVetScan.Checked = True) Then
            rdoPiccolo.Checked = False
        Else
            rdoPiccolo.Checked = True
        End If
    End Sub
End Class