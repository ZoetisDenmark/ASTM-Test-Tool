Imports System.Data.OleDb

'----------------------------------------
'
' Snow 5/2016
'----------------------------------------
Public Class CreateNewXMLFileForm
    Inherits System.Windows.Forms.Form
    Private cxnDB_m As ADODB.Connection
    Private Const ASTM1394DB As String = "ASTM1394Order.MDB"
    Private bindingSource1 As New BindingSource()
    Dim dgv1 As DataGridView = New DataGridView
    Dim xmlFileContent As String
    Private analyzerType As String
    Public Event OnError()
    Dim array() As String

    Private Sub DatabaseConnection_Initialize()
        On Error GoTo ErrTrap
        ' setup connection and connect to the database
        cxnDB_m = New ADODB.Connection
        cxnDB_m.Provider = "Microsoft.Jet.OLEDB.4.0" '"Microsoft.Jet.OLEDB.4.0"
        cxnDB_m.Properties("Data Source").Value = My.Application.Info.DirectoryPath & "\" & ASTM1394DB '"\\fs1\Data\Departmental_Data\610 - Software Development\Members\Support Software\Databases\" & ASTM1394DB '"M:\Members\Support Software\Databases\" & ASTM1394DB 
        Call cxnDB_m.Open()
ErrTrap:
        Err.Source = Err.Source & " | ASTM1394.Class_Initialize"
        RaiseEvent OnError()
    End Sub


    Private Sub CreateNewXMLFileForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        On Error GoTo ErrTrap

        Dim rs As ADODB.Recordset
        Dim rs1 As ADODB.Recordset
        Dim strSQL As String
        Dim maxIndex As Integer
        Dim n As Integer

        rs = New ADODB.Recordset
        rs1 = New ADODB.Recordset
        Dim ds As DataSet = New DataSet
        rs.CursorLocation = ADODB.CursorLocationEnum.adUseClient

        Call DatabaseConnection_Initialize()

        If (My.Forms.Main_Renamed.rdoVetScan.Checked) Then
            analyzerType = "VetScan"
        Else
            analyzerType = "Piccolo"
        End If

        '-------- DataGridView -----------------
        Dim myDA As OleDbDataAdapter = New OleDbDataAdapter
        Dim myDS As DataSet = New DataSet
        Dim objDT As New DataTable()

        'Dim dgv1 As DataGridView = New DataGridView
        dgv1.Width = 1270
        dgv1.Height = 568
        dgv1.RowHeadersVisible = True
        dgv1.RowTemplate = New DataGridViewNumberedRow
        dgv1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
        dgv1.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single
        dgv1.CellBorderStyle = DataGridViewCellBorderStyle.Single
        dgv1.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.True
        dgv1.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter

        'dgv1.ColumnHeadersDefaultCellStyle.Font = New Font(dgv1.Font, FontStyle.Bold)
        'dgv1.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue


        strSQL = "SELECT [Name] FROM [Field]  WHERE [OrderForNewFile] is not NULL ORDER BY [OrderForNewFile]"
        Call rs.Open(strSQL, cxnDB_m, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockOptimistic, ADODB.CommandTypeEnum.adCmdText)

        ' make sure fields were found for the record type
        If (rs.EOF = True) Then
            Call Err.Raise(COMPERR, "ASTM1394", My.Resources.ResourceManager.GetString("str" + CStr(EOFERRTXT)))
        End If
        Call rs.MoveFirst()

        maxIndex = rs.RecordCount - 1

        For n = 0 To maxIndex
            rs.MoveNext() 'move cursor from first row to last row so RecordSet rs contains all rows
        Next

        myDA.Fill(myDS, rs, "MyTable")
        objDT = myDS.Tables("MyTable")

        Dim myDT As DataTable = New DataTable
        Dim myDR As DataRow
        myDR = myDT.NewRow

        For n = 0 To maxIndex
            Dim dtColumn As DataColumn

            dtColumn = New DataColumn()
            dtColumn.DataType = System.Type.GetType("System.String")
            dtColumn.ColumnName = objDT.Rows(n)(0).ToString()
            dtColumn.Caption = objDT.Rows(n)(0).ToString()
            dtColumn.ReadOnly = False
            'dtColumn.Unique = True

            myDT.Columns.Add(dtColumn)

        Next
        myDT.Rows.Add(myDR)
        'bindingSource1.DataSource = objDT
        dgv1.DataSource = myDT 'objDT 'bindingSource1
        'dgv1.AutoResizeColumns(
        '        DataGridViewAutoSizeColumnsMode.AllCellsExceptHeader)

        dgv1.EditMode = DataGridViewEditMode.EditOnEnter
        Panel1.Controls.Add(dgv1)



        '-------------- add dropdown to Universal Test ID column -------------------
        Dim myDA1 As OleDbDataAdapter = New OleDbDataAdapter
        Dim myDS1 As DataSet = New DataSet
        Dim dtRotorID As DataTable = New DataTable
        If (analyzerType = "VetScan") Then
            strSQL = "SELECT CStr(RotorID), RotorName FROM tblRotor WHERE AnalyzerType='VetScan VS2' AND [Active] = True  ORDER BY RotorName, RotorID"
        Else
            strSQL = "SELECT CStr(RotorID), RotorName FROM tblRotor WHERE AnalyzerType='Piccolo xpress' AND [Active] = True  ORDER BY RotorName, RotorID"
        End If

        Call rs1.Open(strSQL, cxnDB_m, ADODB.CursorTypeEnum.adOpenKeyset, ADODB.LockTypeEnum.adLockOptimistic, ADODB.CommandTypeEnum.adCmdText)
        ' make sure fields were found for the record type
        If (rs1.EOF = True) Then
            Call Err.Raise(COMPERR, "CreateNewXMLFile", My.Resources.ResourceManager.GetString("datatable RotorID"))
        End If
        Call rs1.MoveFirst()
        myDA1.Fill(myDS1, rs1, "RotorTable")
        dtRotorID = myDS1.Tables("RotorTable")

        Dim comboBoxColumn As DataGridViewComboBoxColumn = New DataGridViewComboBoxColumn()
        comboBoxColumn.Name = "Universal Test ID"
        comboBoxColumn.HeaderText = "Universal Test ID"
        comboBoxColumn.DataPropertyName = "Universal Test ID"
        comboBoxColumn.DataSource = dtRotorID
        comboBoxColumn.ValueMember = dtRotorID.Columns(0).ColumnName
        comboBoxColumn.DisplayMember = dtRotorID.Columns(1).ColumnName
        comboBoxColumn.DisplayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton
        dgv1.Columns.RemoveAt(12)
        dgv1.Columns.Insert(12, comboBoxColumn)

        ReDim array(dtRotorID.Rows.Count - 1)
        For i As Integer = 0 To dtRotorID.Rows.Count - 1
            array(i) = dtRotorID.Rows(i)(0).ToString()
        Next


        'add ListBox into Universal Test ID, so can select multiple Rotors ???
        'Dim listBox1 As ListBox = New ListBox()
        'Dim listBoxColumn As DataGridViewColumn = New DataGridViewColumn
        'listBox1.DataSource = dtRotorID
        'listBox1.ValueMember = dtRotorID.Columns(0).ColumnName
        'listBox1.DisplayMember = dtRotorID.Columns(1).ColumnName


        '---- add dropdown to Special Field 2 column
        Dim myDA2 As OleDbDataAdapter = New OleDbDataAdapter
        Dim myDS2 As DataSet = New DataSet
        Dim dtSpecialField2 As DataTable = New DataTable
        If (analyzerType = "VetScan") Then
            strSQL = "SELECT CStr(ID), ReferenceRangeLabel FROM tblReferenceRange WHERE AnalyzerType='VetScan VS2' ORDER BY ReferenceRangeLabel"
        Else
            strSQL = "SELECT CStr(ID), ReferenceRangeLabel FROM tblReferenceRange WHERE AnalyzerType='Piccolo xpress' ORDER BY DisplayOrder"
        End If

        Call rs1.Open(strSQL, cxnDB_m, ADODB.CursorTypeEnum.adOpenKeyset, ADODB.LockTypeEnum.adLockOptimistic, ADODB.CommandTypeEnum.adCmdText)
        ' make sure fields were found for the record type
        If (rs1.EOF = True) Then
            Call Err.Raise(COMPERR, "CreateNewXMLFile", My.Resources.ResourceManager.GetString("datatable RotorID"))
        End If
        Call rs1.MoveFirst()
        myDA2.Fill(myDS2, rs1, "SpecialField2Table")
        dtSpecialField2 = myDS2.Tables("SpecialField2Table")


        Dim comboBoxColumn2 As DataGridViewComboBoxColumn = New DataGridViewComboBoxColumn()
        comboBoxColumn2.Name = "Special Field 2"
        comboBoxColumn2.HeaderText = "Special Field 2"
        comboBoxColumn2.DataPropertyName = "Special Field 2"
        comboBoxColumn2.DataSource = dtSpecialField2
        comboBoxColumn2.ValueMember = dtSpecialField2.Columns(1).ColumnName 'dtSpecialField2.Columns(0).ColumnName not use ID
        comboBoxColumn2.DisplayMember = dtSpecialField2.Columns(1).ColumnName
        comboBoxColumn2.DropDownWidth = 110
        comboBoxColumn2.Width = 110

        dgv1.Columns.RemoveAt(10)
        dgv1.Columns.Insert(10, comboBoxColumn2)



        '-------------- Gender dropdown --------------
        strSQL = "SELECT CStr(ID),  IIf(IsNull([Gender]),'',[Gender]) As Gender FROM tblGender ORDER BY ID"
        Call rs1.Open(strSQL, cxnDB_m, ADODB.CursorTypeEnum.adOpenKeyset, ADODB.LockTypeEnum.adLockOptimistic, ADODB.CommandTypeEnum.adCmdText)
        ' make sure fields were found for the record type
        If (rs1.EOF = True) Then
            Call Err.Raise(COMPERR, "CreateNewXMLFile", My.Resources.ResourceManager.GetString("datatable Gender"))
        End If
        Call rs1.MoveFirst()
        myDA2.Fill(myDS2, rs1, "GenderTable")
        dtSpecialField2 = myDS2.Tables("GenderTable")

        Dim comboBoxColumn4 As DataGridViewComboBoxColumn = New DataGridViewComboBoxColumn()
        comboBoxColumn4.Name = "Patient Sex"
        comboBoxColumn4.HeaderText = "Patient Sex"
        comboBoxColumn4.DataPropertyName = "Patient Sex"
        comboBoxColumn4.DataSource = dtSpecialField2
        comboBoxColumn4.ValueMember = dtSpecialField2.Columns(1).ColumnName 'dtSpecialField2.Columns(0).ColumnName not use ID
        comboBoxColumn4.DisplayMember = dtSpecialField2.Columns(1).ColumnName
        comboBoxColumn4.DropDownWidth = 63
        comboBoxColumn4.Width = 63

        dgv1.Columns.RemoveAt(6)
        dgv1.Columns.Insert(6, comboBoxColumn4)

        '---------------- Race dropdown -------------
        If (analyzerType = "Piccolo") Then
            Dim myRace As DataSet = New DataSet
            Dim dtRace As DataTable = New DataTable

            strSQL = "SELECT CStr(ID),  IIf(IsNull([Race]),'',[Race]) AS Race FROM tblRace ORDER BY ID"
            Call rs1.Open(strSQL, cxnDB_m, ADODB.CursorTypeEnum.adOpenKeyset, ADODB.LockTypeEnum.adLockOptimistic, ADODB.CommandTypeEnum.adCmdText)
            ' make sure fields were found for the record type
            If (rs1.EOF = True) Then
                Call Err.Raise(COMPERR, "CreateNewXMLFile", My.Resources.ResourceManager.GetString("datatable Race"))
            End If
            Call rs1.MoveFirst()
            myDA2.Fill(myRace, rs1, "RaceTable")
            dtRace = myRace.Tables("RaceTable")

            Dim comboBoxColumn3 As DataGridViewComboBoxColumn = New DataGridViewComboBoxColumn()
            comboBoxColumn3.Name = "Patient Race/Ethnic Origin"
            comboBoxColumn3.HeaderText = "Patient Race/Ethnic Origin"
            comboBoxColumn3.DataPropertyName = "Patient Race/Ethnic Origin"
            comboBoxColumn3.DataSource = dtRace
            comboBoxColumn3.ValueMember = dtRace.Columns(1).ColumnName 'dtSpecialField2.Columns(0).ColumnName not use ID
            comboBoxColumn3.DisplayMember = dtRace.Columns(1).ColumnName
            comboBoxColumn3.DropDownWidth = 65
            comboBoxColumn3.Width = 65

            dgv1.Columns.RemoveAt(7)
            dgv1.Columns.Insert(7, comboBoxColumn3)
        End If

        Dim header_style As New DataGridViewCellStyle
        header_style.BackColor = Color.Yellow
        For Each column As DataGridViewColumn In dgv1.Columns
            column.Width = 63
            If (column.Name.Contains("Requested/Ordered")) Then
                column.Width = 120
            ElseIf (column.Name.Contains("Laboratory")) Then
                column.Width = 80
            ElseIf column.Name.Contains("Special Field 2") Then
                column.Width = 110
                column.DefaultCellStyle = header_style
            ElseIf (column.Name.Contains("Universal")) Then
                column.Width = 150
                column.DefaultCellStyle = header_style
            End If
        Next

        dgv1.RowHeadersWidth = 35
        dgv1.Columns(2).DefaultCellStyle = header_style

        Exit Sub
ErrTrap:
        Err.Source = Err.Source & " | ASTM1394.Class_Initialize"
        RaiseEvent OnError()
    End Sub

    'Create new XML file with certain Tag template
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        On Error GoTo ErrTrap

        Dim xmlContentBegin1 As String = "<LabReports>"
        Dim xmlContentBegin2 As String = "<LabReport><Identification><ReportType>request</ReportType>"
        Dim xmlContentEnd1 As String = "</LabRequests></LabReport>"
        Dim xmlContentEnd2 As String = "</LabReports>"
        Dim xmlBody As String = ""
        Dim xmlContentCenter As String = ""
        Dim n As Integer
        For n = 0 To dgv1.RowCount - 1
            If (dgv1.Rows(n).IsNewRow = False) Then

                If (dgv1.Rows(n).Cells(2).Value.ToString().Trim() <> "" And dgv1.Rows(n).Cells(10).Value.ToString().Trim() <> "" And dgv1.Rows(n).Cells(12).Value.ToString().Trim() <> "") Then
                    xmlContentCenter = "<PIMSName>" & dgv1.Rows(n).Cells(0).Value.ToString().Trim() & "</PIMSName><OwnerName>"
                    If (dgv1.Rows(n).Cells(9).Value.ToString().Trim().Length > 2) Then
                        If (dgv1.Rows(n).Cells(9).Value.ToString().Trim().Substring(0, dgv1.Rows(n).Cells(9).Value.ToString().Trim().IndexOf("^")).Length > 14) Then
                            xmlContentCenter = xmlContentCenter & dgv1.Rows(n).Cells(9).Value.ToString().Trim().Substring(0, dgv1.Rows(n).Cells(9).Value.ToString().Trim().IndexOf("^")).Remove(14) & "</OwnerName>"
                        Else
                            xmlContentCenter = xmlContentCenter & dgv1.Rows(n).Cells(9).Value.ToString().Trim().Substring(0, dgv1.Rows(n).Cells(9).Value.ToString().Trim().IndexOf("^")) & "</OwnerName>"
                        End If
                    Else
                        xmlContentCenter = xmlContentCenter & "</OwnerName>"
                    End If
                    If (dgv1.Rows(n).Cells(14).Value.ToString().Trim().Length > 14) Then
                        xmlContentCenter = xmlContentCenter & "<VetID>" & dgv1.Rows(n).Cells(14).Value.ToString().Trim().Remove(14)
                    Else
                        xmlContentCenter = xmlContentCenter & "<VetID>" & dgv1.Rows(n).Cells(14).Value.ToString().Trim()
                    End If
                    xmlContentCenter = xmlContentCenter & "</VetID><VetName/></Identification><AnimalDetails>"
                    If (dgv1.Rows(n).Cells(2).Value.ToString().Trim().Length > 14) Then
                        xmlContentCenter = xmlContentCenter & "<AnimalID>" & dgv1.Rows(n).Cells(2).Value.ToString().Trim().Remove(14) & "</AnimalID>"
                    Else
                        xmlContentCenter = xmlContentCenter & "<AnimalID>" & dgv1.Rows(n).Cells(2).Value.ToString().Trim() & "</AnimalID>"
                    End If
                    If (dgv1.Rows(n).Cells(4).Value.ToString().Trim().Length > 14) Then
                        xmlContentCenter = xmlContentCenter & "<AnimalName>" & dgv1.Rows(n).Cells(4).Value.ToString().Trim().Remove(14) & "</AnimalName><Breed/>"
                    Else
                        xmlContentCenter = xmlContentCenter & "<AnimalName>" & dgv1.Rows(n).Cells(4).Value.ToString().Trim() & "</AnimalName><Breed/>"
                    End If
                    If (IsDBNull(dgv1.Rows(n).Cells(6).Value) Or dgv1.Rows(n).Cells(6).Value Is Nothing) Then
                        xmlContentCenter = xmlContentCenter & "<Gender/>"
                    Else
                        xmlContentCenter = xmlContentCenter & "<Gender>" & dgv1.Rows(n).Cells(6).Value.ToString().Trim() & "</Gender>"
                    End If
                    xmlContentCenter = xmlContentCenter & "<Species>" & dgv1.Rows(n).Cells(10).Value.ToString().Trim() & "</Species><Age>"
                    If (dgv1.Rows(n).Cells(9).Value.ToString().Trim().Length > 2) Then
                        xmlContentCenter = xmlContentCenter & dgv1.Rows(n).Cells(9).Value.ToString().Trim().Substring(dgv1.Rows(n).Cells(9).Value.ToString().Trim().IndexOf("^") + 1, dgv1.Rows(n).Cells(9).Value.ToString().Trim().LastIndexOf("^") - dgv1.Rows(n).Cells(9).Value.ToString().Trim().IndexOf("^") - 1) & "</Age>"
                    Else
                        xmlContentCenter = xmlContentCenter & "</Age>"
                    End If
                    xmlContentCenter = xmlContentCenter & "<DateOfBirth>" & dgv1.Rows(n).Cells(5).Value.ToString().Trim() & "</DateOfBirth><AbbreviatedHistory/></AnimalDetails>"
                    If (dgv1.Rows(n).Cells(3).Value.ToString().Trim().Length > 14) Then
                        xmlContentCenter = xmlContentCenter & "<ASTMToolDetails><SampleID>" & dgv1.Rows(n).Cells(3).Value.ToString().Trim().Remove(14) & "</SampleID>"
                    Else
                        xmlContentCenter = xmlContentCenter & "<ASTMToolDetails><SampleID>" & dgv1.Rows(n).Cells(3).Value.ToString().Trim() & "</SampleID>"
                    End If
                    If (dgv1.Rows(n).Cells(7).Value Is Nothing) Then '???? why not IsDBNull
                        xmlContentCenter = xmlContentCenter & "<Race/>"
                    Else
                        xmlContentCenter = xmlContentCenter & "<Race>" & dgv1.Rows(n).Cells(7).Value.ToString().Trim() & "</Race>"
                    End If
                    If (dgv1.Rows(n).Cells(8).Value.ToString().Trim().Length > 14) Then
                        xmlContentCenter = xmlContentCenter & "<TelephoneNumber>" & dgv1.Rows(n).Cells(8).Value.ToString().Trim().Remove(14) & "</TelephoneNumber><DateOfVaccination>"
                    Else
                        xmlContentCenter = xmlContentCenter & "<TelephoneNumber>" & dgv1.Rows(n).Cells(8).Value.ToString().Trim() & "</TelephoneNumber><DateOfVaccination>"
                    End If
                    If (dgv1.Rows(n).Cells(9).Value.ToString().Trim().Length > 2) Then
                        xmlContentCenter = xmlContentCenter & dgv1.Rows(n).Cells(9).Value.ToString().Trim().Substring(dgv1.Rows(n).Cells(9).Value.ToString().Trim().LastIndexOf("^") + 1) & "</DateOfVaccination>"
                    Else
                        xmlContentCenter = xmlContentCenter & "</DateOfVaccination>"
                    End If
                    If (dgv1.Rows(n).Cells(11).Value.ToString().Trim().Length > 14) Then
                        xmlContentCenter = xmlContentCenter & "<Location>" & dgv1.Rows(n).Cells(11).Value.ToString().Trim().Remove(14) & "</Location>"
                    Else
                        xmlContentCenter = xmlContentCenter & "<Location>" & dgv1.Rows(n).Cells(11).Value.ToString().Trim() & "</Location>"
                    End If
                    If (dgv1.Rows(n).Cells(13).Value.ToString().Trim() = "") Then
                        xmlContentCenter = xmlContentCenter & "<OrderDateTime>" & Date.Now.ToString("yyyyMMddHHmmss") & "</OrderDateTime>"
                    Else
                        xmlContentCenter = xmlContentCenter & "<OrderDateTime>" & dgv1.Rows(n).Cells(13).Value.ToString().Trim() & "</OrderDateTime>"
                        End If
                    If (dgv1.Rows(n).Cells(10).Value.ToString().Contains("Control")) Then
                        xmlContentCenter = xmlContentCenter & "<ActionCode>Q</ActionCode>"
                    Else
                        xmlContentCenter = xmlContentCenter & "<ActionCode>N</ActionCode>"
                    End If
                    If (dgv1.Rows(n).Cells(15).Value.ToString().Trim().Length > 17) Then
                        xmlContentCenter = xmlContentCenter & "<LaboratoryField1>" & dgv1.Rows(n).Cells(15).Value.ToString().Trim().Remove(17) & "</LaboratoryField1></ASTMToolDetails>"
                    Else
                        xmlContentCenter = xmlContentCenter & "<LaboratoryField1>" & dgv1.Rows(n).Cells(15).Value.ToString().Trim() & "</LaboratoryField1></ASTMToolDetails>"
                    End If
                    xmlContentCenter = xmlContentCenter & "<LabRequests>"
                    If (dgv1.Rows(n).Cells(12).Value.ToString() = "0") Then

                        'loop all rotors
                        For m As Integer = 0 To array.GetUpperBound(0)
                            If (array(m).ToString() <> "0") Then
                                xmlContentCenter = xmlContentCenter & "<LabRequest><TestCode>^^^" & array(m).ToString() & "</TestCode><DeviceID>" & dgv1.Rows(n).Cells(1).Value.ToString().Trim() & "</DeviceID></LabRequest>"
                            End If
                        Next
                    Else
                        xmlContentCenter = xmlContentCenter & "<LabRequest><TestCode>^^^" & dgv1.Rows(n).Cells(12).Value.ToString().Trim() & "</TestCode><DeviceID>" & dgv1.Rows(n).Cells(1).Value.ToString().Trim() & "</DeviceID></LabRequest>"
                    End If

                End If
                    xmlBody = xmlBody & xmlContentBegin2 & xmlContentCenter & xmlContentEnd1 & vbCrLf
            End If

        Next

        xmlFileContent = xmlContentBegin1 + xmlBody + xmlContentEnd2


        SaveFileDialog1.DefaultExt = "xml"
        SaveFileDialog1.Filter = "XML Files (*.xml)|*.XML"
        'SaveFileDialog1.InitialDirectory = "C:\"
        SaveFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
        SaveFileDialog1.OverwritePrompt = True
        SaveFileDialog1.CheckPathExists = True
        'SaveFileDialog1.ShowDialog()

        'If (SaveFileDialog1.ShowDialog() = DialogResult.OK) Then


        If (SaveFileDialog1.ShowDialog = Windows.Forms.DialogResult.OK) Then

            Dim filePathAndName As String = SaveFileDialog1.FileName
            My.Computer.FileSystem.WriteAllText(filePathAndName, xmlFileContent, False)
            MsgBox("the file " & filePathAndName & " has been saved")

        End If
        'SaveFileDialog1.Dispose()
        Exit Sub
ErrTrap:
                 Err.Source = Err.Source & " | ASTM1394.Class_Initialize"
                 RaiseEvent OnError()
             End Sub


End Class

Public Class DataGridViewNumberedRow
    Inherits DataGridViewRow

    Protected Overrides Sub PaintHeader(graphics As System.Drawing.Graphics, clipBounds As System.Drawing.Rectangle, rowBounds As System.Drawing.Rectangle, rowIndex As Integer, rowState As System.Windows.Forms.DataGridViewElementStates, isFirstDisplayedRow As Boolean, isLastVisibleRow As Boolean, paintParts As System.Windows.Forms.DataGridViewPaintParts)
        MyBase.PaintHeader(graphics, clipBounds, rowBounds, rowIndex, rowState, isFirstDisplayedRow, isLastVisibleRow, paintParts)

        Dim f As StringFormat = New StringFormat
        f.Alignment = StringAlignment.Near
        graphics.DrawString((rowIndex + 1).ToString(), SystemFonts.DialogFont, Brushes.Black, rowBounds, f)
    End Sub
End Class