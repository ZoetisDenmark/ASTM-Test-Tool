<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> Partial Class Main_Renamed
#Region "Windows Form Designer generated code "
	<System.Diagnostics.DebuggerNonUserCode()> Public Sub New()
		MyBase.New()
		'This call is required by the Windows Form Designer.
		InitializeComponent()
	End Sub
	'Form overrides dispose to clean up the component list.
	<System.Diagnostics.DebuggerNonUserCode()> Protected Overloads Overrides Sub Dispose(ByVal Disposing As Boolean)
		If Disposing Then
			If Not components Is Nothing Then
				components.Dispose()
			End If
		End If
		MyBase.Dispose(Disposing)
	End Sub
	'Required by the Windows Form Designer
	Private components As System.ComponentModel.IContainer
	Public ToolTip1 As System.Windows.Forms.ToolTip
    Public WithEvents txtFieldDelimiter As System.Windows.Forms.TextBox
    Public WithEvents cmdTx As System.Windows.Forms.Button
    Public WithEvents txtSequence As System.Windows.Forms.TextBox
    Public WithEvents lblTxFieldDelimiter As System.Windows.Forms.Label
    Public WithEvents lblTxSequence As System.Windows.Forms.Label
    Public WithEvents fraTx As System.Windows.Forms.GroupBox
    Public WithEvents cmdStart As System.Windows.Forms.Button
    Public WithEvents cmdStop As System.Windows.Forms.Button
    Public WithEvents lblPort As System.Windows.Forms.Label
    Public WithEvents fraComm As System.Windows.Forms.GroupBox
    Public WithEvents cboRecordType As System.Windows.Forms.ComboBox
    Public WithEvents lvwRecord As AxExtLVCTL.AxExtLV
    Public WithEvents lblRecordType As System.Windows.Forms.Label
    Public WithEvents fraRecord As System.Windows.Forms.GroupBox
    Public dlgSave As System.Windows.Forms.SaveFileDialog
    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Main_Renamed))
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.btnFileTransmit = New System.Windows.Forms.Button()
        Me.TextBoxFileName = New System.Windows.Forms.TextBox()
        Me.fraTx = New System.Windows.Forms.GroupBox()
        Me.btnStopTransmit = New System.Windows.Forms.Button()
        Me.txtFieldDelimiter = New System.Windows.Forms.TextBox()
        Me.cmdTx = New System.Windows.Forms.Button()
        Me.txtSequence = New System.Windows.Forms.TextBox()
        Me.lblTxFieldDelimiter = New System.Windows.Forms.Label()
        Me.lblTxSequence = New System.Windows.Forms.Label()
        Me.fraComm = New System.Windows.Forms.GroupBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.nudPortNumber = New System.Windows.Forms.NumericUpDown()
        Me.cmdStart = New System.Windows.Forms.Button()
        Me.cmdStop = New System.Windows.Forms.Button()
        Me.lblPort = New System.Windows.Forms.Label()
        Me.fraRecord = New System.Windows.Forms.GroupBox()
        Me.cboRecordType = New System.Windows.Forms.ComboBox()
        Me.lvwRecord = New AxExtLVCTL.AxExtLV()
        Me.lblRecordType = New System.Windows.Forms.Label()
        Me.btnCreateLibraryFile = New System.Windows.Forms.Button()
        Me.dlgSave = New System.Windows.Forms.SaveFileDialog()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.txtIP4 = New System.Windows.Forms.TextBox()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.txtPort = New System.Windows.Forms.TextBox()
        Me.btnStop = New System.Windows.Forms.Button()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.txtIP3 = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.txtIP2 = New System.Windows.Forms.TextBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.txtIP1 = New System.Windows.Forms.TextBox()
        Me.btnStart = New System.Windows.Forms.Button()
        Me.chkVerbose = New System.Windows.Forms.CheckBox()
        Me.cmdSave = New System.Windows.Forms.Button()
        Me.txtDisplay = New System.Windows.Forms.RichTextBox()
        Me.cmdClear = New System.Windows.Forms.Button()
        Me.fraDisplay = New System.Windows.Forms.GroupBox()
        Me.chkENQ = New System.Windows.Forms.CheckBox()
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip()
        Me.ASTMTestToolToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ExitToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.HelpToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.HelpTopicsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ASTMProtocolToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ASTMProtocolInputRecordsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ASTMProtocolOutputRecordsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ASTMToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.AboutASTMTestToolToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.GroupBoxHPOLLibraryFile = New System.Windows.Forms.GroupBox()
        Me.rdoPiccolo = New System.Windows.Forms.RadioButton()
        Me.rdoVetScan = New System.Windows.Forms.RadioButton()
        Me.btnBrowseFile = New System.Windows.Forms.Button()
        Me.FolderBrowserDialog1 = New System.Windows.Forms.FolderBrowserDialog()
        Me.OpenFileDialog1 = New System.Windows.Forms.OpenFileDialog()
        Me.BackgroundWorker2 = New System.ComponentModel.BackgroundWorker()
        Me.fraTx.SuspendLayout()
        Me.fraComm.SuspendLayout()
        CType(Me.nudPortNumber, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.fraRecord.SuspendLayout()
        CType(Me.lvwRecord, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupBox1.SuspendLayout()
        Me.fraDisplay.SuspendLayout()
        Me.MenuStrip1.SuspendLayout()
        Me.GroupBoxHPOLLibraryFile.SuspendLayout()
        Me.SuspendLayout()
        '
        'btnFileTransmit
        '
        Me.btnFileTransmit.Enabled = False
        Me.btnFileTransmit.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnFileTransmit.Location = New System.Drawing.Point(429, 13)
        Me.btnFileTransmit.Name = "btnFileTransmit"
        Me.btnFileTransmit.Size = New System.Drawing.Size(65, 25)
        Me.btnFileTransmit.TabIndex = 16
        Me.btnFileTransmit.Text = "Transmit"
        Me.ToolTip1.SetToolTip(Me.btnFileTransmit, "transmit Test Order to analyzer from xml file ")
        Me.btnFileTransmit.UseVisualStyleBackColor = True
        '
        'TextBoxFileName
        '
        Me.TextBoxFileName.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TextBoxFileName.Location = New System.Drawing.Point(43, 16)
        Me.TextBoxFileName.Name = "TextBoxFileName"
        Me.TextBoxFileName.ReadOnly = True
        Me.TextBoxFileName.Size = New System.Drawing.Size(299, 20)
        Me.TextBoxFileName.TabIndex = 19
        Me.TextBoxFileName.Text = "Select a saved library file"
        Me.ToolTip1.SetToolTip(Me.TextBoxFileName, "select a saved .xml file to transmit or view or edit")
        '
        'fraTx
        '
        Me.fraTx.BackColor = System.Drawing.SystemColors.Control
        Me.fraTx.Controls.Add(Me.btnStopTransmit)
        Me.fraTx.Controls.Add(Me.txtFieldDelimiter)
        Me.fraTx.Controls.Add(Me.cmdTx)
        Me.fraTx.Controls.Add(Me.txtSequence)
        Me.fraTx.Controls.Add(Me.lblTxFieldDelimiter)
        Me.fraTx.Controls.Add(Me.lblTxSequence)
        Me.fraTx.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.fraTx.ForeColor = System.Drawing.SystemColors.ControlText
        Me.fraTx.Location = New System.Drawing.Point(518, 26)
        Me.fraTx.Name = "fraTx"
        Me.fraTx.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.fraTx.Size = New System.Drawing.Size(235, 100)
        Me.fraTx.TabIndex = 18
        Me.fraTx.TabStop = False
        Me.fraTx.Text = "Transmission"
        '
        'btnStopTransmit
        '
        Me.btnStopTransmit.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnStopTransmit.Location = New System.Drawing.Point(159, 60)
        Me.btnStopTransmit.Name = "btnStopTransmit"
        Me.btnStopTransmit.Size = New System.Drawing.Size(65, 23)
        Me.btnStopTransmit.TabIndex = 21
        Me.btnStopTransmit.Text = "Stop"
        Me.btnStopTransmit.UseVisualStyleBackColor = True
        '
        'txtFieldDelimiter
        '
        Me.txtFieldDelimiter.AcceptsReturn = True
        Me.txtFieldDelimiter.BackColor = System.Drawing.SystemColors.Window
        Me.txtFieldDelimiter.Cursor = System.Windows.Forms.Cursors.IBeam
        Me.txtFieldDelimiter.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtFieldDelimiter.ForeColor = System.Drawing.SystemColors.WindowText
        Me.txtFieldDelimiter.Location = New System.Drawing.Point(94, 61)
        Me.txtFieldDelimiter.MaxLength = 1
        Me.txtFieldDelimiter.Name = "txtFieldDelimiter"
        Me.txtFieldDelimiter.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.txtFieldDelimiter.Size = New System.Drawing.Size(54, 20)
        Me.txtFieldDelimiter.TabIndex = 5
        Me.txtFieldDelimiter.Text = "l"
        '
        'cmdTx
        '
        Me.cmdTx.BackColor = System.Drawing.SystemColors.Control
        Me.cmdTx.Cursor = System.Windows.Forms.Cursors.Default
        Me.cmdTx.Enabled = False
        Me.cmdTx.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cmdTx.ForeColor = System.Drawing.SystemColors.ControlText
        Me.cmdTx.Location = New System.Drawing.Point(159, 24)
        Me.cmdTx.Name = "cmdTx"
        Me.cmdTx.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.cmdTx.Size = New System.Drawing.Size(65, 25)
        Me.cmdTx.TabIndex = 4
        Me.cmdTx.Text = "T&ransmit"
        Me.cmdTx.UseVisualStyleBackColor = False
        '
        'txtSequence
        '
        Me.txtSequence.AcceptsReturn = True
        Me.txtSequence.BackColor = System.Drawing.SystemColors.Window
        Me.txtSequence.Cursor = System.Windows.Forms.Cursors.IBeam
        Me.txtSequence.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtSequence.ForeColor = System.Drawing.SystemColors.WindowText
        Me.txtSequence.Location = New System.Drawing.Point(94, 27)
        Me.txtSequence.MaxLength = 0
        Me.txtSequence.Name = "txtSequence"
        Me.txtSequence.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.txtSequence.Size = New System.Drawing.Size(54, 20)
        Me.txtSequence.TabIndex = 3
        '
        'lblTxFieldDelimiter
        '
        Me.lblTxFieldDelimiter.BackColor = System.Drawing.SystemColors.Control
        Me.lblTxFieldDelimiter.Cursor = System.Windows.Forms.Cursors.Default
        Me.lblTxFieldDelimiter.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblTxFieldDelimiter.ForeColor = System.Drawing.SystemColors.ControlText
        Me.lblTxFieldDelimiter.Location = New System.Drawing.Point(12, 61)
        Me.lblTxFieldDelimiter.Name = "lblTxFieldDelimiter"
        Me.lblTxFieldDelimiter.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.lblTxFieldDelimiter.Size = New System.Drawing.Size(85, 17)
        Me.lblTxFieldDelimiter.TabIndex = 20
        Me.lblTxFieldDelimiter.Text = "Field Delimiter :"
        '
        'lblTxSequence
        '
        Me.lblTxSequence.BackColor = System.Drawing.SystemColors.Control
        Me.lblTxSequence.Cursor = System.Windows.Forms.Cursors.Default
        Me.lblTxSequence.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblTxSequence.ForeColor = System.Drawing.SystemColors.ControlText
        Me.lblTxSequence.Location = New System.Drawing.Point(12, 27)
        Me.lblTxSequence.Name = "lblTxSequence"
        Me.lblTxSequence.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.lblTxSequence.Size = New System.Drawing.Size(71, 17)
        Me.lblTxSequence.TabIndex = 19
        Me.lblTxSequence.Text = "Sequence :"
        '
        'fraComm
        '
        Me.fraComm.BackColor = System.Drawing.SystemColors.Control
        Me.fraComm.Controls.Add(Me.Label5)
        Me.fraComm.Controls.Add(Me.nudPortNumber)
        Me.fraComm.Controls.Add(Me.cmdStart)
        Me.fraComm.Controls.Add(Me.cmdStop)
        Me.fraComm.Controls.Add(Me.lblPort)
        Me.fraComm.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.fraComm.ForeColor = System.Drawing.SystemColors.ControlText
        Me.fraComm.Location = New System.Drawing.Point(8, 26)
        Me.fraComm.Name = "fraComm"
        Me.fraComm.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.fraComm.Size = New System.Drawing.Size(504, 44)
        Me.fraComm.TabIndex = 15
        Me.fraComm.TabStop = False
        Me.fraComm.Text = "Serial Communication"
        '
        'Label5
        '
        Me.Label5.BackColor = System.Drawing.SystemColors.Control
        Me.Label5.Cursor = System.Windows.Forms.Cursors.Default
        Me.Label5.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label5.ForeColor = System.Drawing.SystemColors.ControlText
        Me.Label5.Location = New System.Drawing.Point(97, 19)
        Me.Label5.Name = "Label5"
        Me.Label5.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Label5.Size = New System.Drawing.Size(250, 17)
        Me.Label5.TabIndex = 18
        Me.Label5.Text = "( Baud: 9600 Parity: None DataBits: 8 StopBits: 1 )"
        '
        'nudPortNumber
        '
        Me.nudPortNumber.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.nudPortNumber.Location = New System.Drawing.Point(43, 19)
        Me.nudPortNumber.Name = "nudPortNumber"
        Me.nudPortNumber.Size = New System.Drawing.Size(45, 20)
        Me.nudPortNumber.TabIndex = 17
        Me.nudPortNumber.Value = New Decimal(New Integer() {1, 0, 0, 0})
        '
        'cmdStart
        '
        Me.cmdStart.BackColor = System.Drawing.SystemColors.Control
        Me.cmdStart.Cursor = System.Windows.Forms.Cursors.Default
        Me.cmdStart.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cmdStart.ForeColor = System.Drawing.SystemColors.ControlText
        Me.cmdStart.Location = New System.Drawing.Point(348, 15)
        Me.cmdStart.Name = "cmdStart"
        Me.cmdStart.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.cmdStart.Size = New System.Drawing.Size(65, 25)
        Me.cmdStart.TabIndex = 1
        Me.cmdStart.Text = "&Start"
        Me.cmdStart.UseVisualStyleBackColor = False
        '
        'cmdStop
        '
        Me.cmdStop.BackColor = System.Drawing.SystemColors.Control
        Me.cmdStop.Cursor = System.Windows.Forms.Cursors.Default
        Me.cmdStop.Enabled = False
        Me.cmdStop.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cmdStop.ForeColor = System.Drawing.SystemColors.ControlText
        Me.cmdStop.Location = New System.Drawing.Point(429, 16)
        Me.cmdStop.Name = "cmdStop"
        Me.cmdStop.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.cmdStop.Size = New System.Drawing.Size(65, 25)
        Me.cmdStop.TabIndex = 2
        Me.cmdStop.Text = "S&top"
        Me.cmdStop.UseVisualStyleBackColor = False
        '
        'lblPort
        '
        Me.lblPort.BackColor = System.Drawing.SystemColors.Control
        Me.lblPort.Cursor = System.Windows.Forms.Cursors.Default
        Me.lblPort.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblPort.ForeColor = System.Drawing.SystemColors.ControlText
        Me.lblPort.Location = New System.Drawing.Point(6, 20)
        Me.lblPort.Name = "lblPort"
        Me.lblPort.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.lblPort.Size = New System.Drawing.Size(31, 17)
        Me.lblPort.TabIndex = 17
        Me.lblPort.Text = "Port :"
        '
        'fraRecord
        '
        Me.fraRecord.BackColor = System.Drawing.SystemColors.Control
        Me.fraRecord.Controls.Add(Me.cboRecordType)
        Me.fraRecord.Controls.Add(Me.lvwRecord)
        Me.fraRecord.Controls.Add(Me.lblRecordType)
        Me.fraRecord.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.fraRecord.ForeColor = System.Drawing.SystemColors.ControlText
        Me.fraRecord.Location = New System.Drawing.Point(8, 181)
        Me.fraRecord.Name = "fraRecord"
        Me.fraRecord.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.fraRecord.Size = New System.Drawing.Size(745, 180)
        Me.fraRecord.TabIndex = 13
        Me.fraRecord.TabStop = False
        Me.fraRecord.Text = "Record"
        '
        'cboRecordType
        '
        Me.cboRecordType.BackColor = System.Drawing.SystemColors.Window
        Me.cboRecordType.Cursor = System.Windows.Forms.Cursors.Default
        Me.cboRecordType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboRecordType.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cboRecordType.ForeColor = System.Drawing.SystemColors.WindowText
        Me.cboRecordType.Location = New System.Drawing.Point(58, 11)
        Me.cboRecordType.Name = "cboRecordType"
        Me.cboRecordType.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.cboRecordType.Size = New System.Drawing.Size(149, 22)
        Me.cboRecordType.TabIndex = 6
        '
        'lvwRecord
        '
        Me.lvwRecord.Enabled = True
        Me.lvwRecord.Location = New System.Drawing.Point(8, 39)
        Me.lvwRecord.Name = "lvwRecord"
        Me.lvwRecord.OcxState = CType(resources.GetObject("lvwRecord.OcxState"), System.Windows.Forms.AxHost.State)
        Me.lvwRecord.Size = New System.Drawing.Size(729, 143)
        Me.lvwRecord.TabIndex = 11
        '
        'lblRecordType
        '
        Me.lblRecordType.BackColor = System.Drawing.SystemColors.Control
        Me.lblRecordType.Cursor = System.Windows.Forms.Cursors.Default
        Me.lblRecordType.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblRecordType.ForeColor = System.Drawing.SystemColors.ControlText
        Me.lblRecordType.Location = New System.Drawing.Point(11, 15)
        Me.lblRecordType.Name = "lblRecordType"
        Me.lblRecordType.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.lblRecordType.Size = New System.Drawing.Size(41, 17)
        Me.lblRecordType.TabIndex = 14
        Me.lblRecordType.Text = "Type :"
        '
        'btnCreateLibraryFile
        '
        Me.btnCreateLibraryFile.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnCreateLibraryFile.Location = New System.Drawing.Point(637, 12)
        Me.btnCreateLibraryFile.Name = "btnCreateLibraryFile"
        Me.btnCreateLibraryFile.Size = New System.Drawing.Size(97, 25)
        Me.btnCreateLibraryFile.TabIndex = 17
        Me.btnCreateLibraryFile.Text = "Create New File"
        Me.btnCreateLibraryFile.UseVisualStyleBackColor = True
        '
        'GroupBox1
        '
        Me.GroupBox1.BackColor = System.Drawing.SystemColors.Control
        Me.GroupBox1.Controls.Add(Me.txtIP4)
        Me.GroupBox1.Controls.Add(Me.Label6)
        Me.GroupBox1.Controls.Add(Me.txtPort)
        Me.GroupBox1.Controls.Add(Me.btnStop)
        Me.GroupBox1.Controls.Add(Me.Label4)
        Me.GroupBox1.Controls.Add(Me.txtIP3)
        Me.GroupBox1.Controls.Add(Me.Label1)
        Me.GroupBox1.Controls.Add(Me.Label3)
        Me.GroupBox1.Controls.Add(Me.txtIP2)
        Me.GroupBox1.Controls.Add(Me.Label2)
        Me.GroupBox1.Controls.Add(Me.txtIP1)
        Me.GroupBox1.Controls.Add(Me.btnStart)
        Me.GroupBox1.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupBox1.ForeColor = System.Drawing.SystemColors.ControlText
        Me.GroupBox1.Location = New System.Drawing.Point(8, 82)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.GroupBox1.Size = New System.Drawing.Size(504, 44)
        Me.GroupBox1.TabIndex = 19
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "TCP / IP Communication"
        '
        'txtIP4
        '
        Me.txtIP4.AcceptsReturn = True
        Me.txtIP4.BackColor = System.Drawing.SystemColors.Window
        Me.txtIP4.Cursor = System.Windows.Forms.Cursors.IBeam
        Me.txtIP4.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtIP4.ForeColor = System.Drawing.SystemColors.WindowText
        Me.txtIP4.Location = New System.Drawing.Point(196, 19)
        Me.txtIP4.MaxLength = 3
        Me.txtIP4.Name = "txtIP4"
        Me.txtIP4.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.txtIP4.Size = New System.Drawing.Size(32, 20)
        Me.txtIP4.TabIndex = 24
        '
        'Label6
        '
        Me.Label6.BackColor = System.Drawing.SystemColors.Control
        Me.Label6.Cursor = System.Windows.Forms.Cursors.Default
        Me.Label6.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label6.ForeColor = System.Drawing.SystemColors.ControlText
        Me.Label6.Location = New System.Drawing.Point(185, 22)
        Me.Label6.Name = "Label6"
        Me.Label6.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Label6.Size = New System.Drawing.Size(15, 17)
        Me.Label6.TabIndex = 23
        Me.Label6.Text = "."
        '
        'txtPort
        '
        Me.txtPort.AcceptsReturn = True
        Me.txtPort.BackColor = System.Drawing.SystemColors.Window
        Me.txtPort.Cursor = System.Windows.Forms.Cursors.IBeam
        Me.txtPort.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtPort.ForeColor = System.Drawing.SystemColors.WindowText
        Me.txtPort.Location = New System.Drawing.Point(286, 19)
        Me.txtPort.MaxLength = 0
        Me.txtPort.Name = "txtPort"
        Me.txtPort.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.txtPort.Size = New System.Drawing.Size(44, 20)
        Me.txtPort.TabIndex = 24
        '
        'btnStop
        '
        Me.btnStop.BackColor = System.Drawing.SystemColors.Control
        Me.btnStop.Cursor = System.Windows.Forms.Cursors.Default
        Me.btnStop.Enabled = False
        Me.btnStop.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnStop.ForeColor = System.Drawing.SystemColors.ControlText
        Me.btnStop.Location = New System.Drawing.Point(429, 11)
        Me.btnStop.Name = "btnStop"
        Me.btnStop.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.btnStop.Size = New System.Drawing.Size(65, 25)
        Me.btnStop.TabIndex = 2
        Me.btnStop.Text = "S&top"
        Me.btnStop.UseVisualStyleBackColor = False
        '
        'Label4
        '
        Me.Label4.BackColor = System.Drawing.SystemColors.Control
        Me.Label4.Cursor = System.Windows.Forms.Cursors.Default
        Me.Label4.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label4.ForeColor = System.Drawing.SystemColors.ControlText
        Me.Label4.Location = New System.Drawing.Point(250, 21)
        Me.Label4.Name = "Label4"
        Me.Label4.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Label4.Size = New System.Drawing.Size(35, 17)
        Me.Label4.TabIndex = 23
        Me.Label4.Text = "Port  :"
        '
        'txtIP3
        '
        Me.txtIP3.AcceptsReturn = True
        Me.txtIP3.BackColor = System.Drawing.SystemColors.Window
        Me.txtIP3.Cursor = System.Windows.Forms.Cursors.IBeam
        Me.txtIP3.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtIP3.ForeColor = System.Drawing.SystemColors.WindowText
        Me.txtIP3.Location = New System.Drawing.Point(154, 19)
        Me.txtIP3.MaxLength = 3
        Me.txtIP3.Name = "txtIP3"
        Me.txtIP3.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.txtIP3.Size = New System.Drawing.Size(32, 20)
        Me.txtIP3.TabIndex = 22
        '
        'Label1
        '
        Me.Label1.BackColor = System.Drawing.SystemColors.Control
        Me.Label1.Cursor = System.Windows.Forms.Cursors.Default
        Me.Label1.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.ForeColor = System.Drawing.SystemColors.ControlText
        Me.Label1.Location = New System.Drawing.Point(4, 19)
        Me.Label1.Name = "Label1"
        Me.Label1.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Label1.Size = New System.Drawing.Size(66, 17)
        Me.Label1.TabIndex = 17
        Me.Label1.Text = "IP Address :"
        '
        'Label3
        '
        Me.Label3.BackColor = System.Drawing.SystemColors.Control
        Me.Label3.Cursor = System.Windows.Forms.Cursors.Default
        Me.Label3.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label3.ForeColor = System.Drawing.SystemColors.ControlText
        Me.Label3.Location = New System.Drawing.Point(143, 22)
        Me.Label3.Name = "Label3"
        Me.Label3.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Label3.Size = New System.Drawing.Size(15, 17)
        Me.Label3.TabIndex = 21
        Me.Label3.Text = "."
        '
        'txtIP2
        '
        Me.txtIP2.AcceptsReturn = True
        Me.txtIP2.BackColor = System.Drawing.SystemColors.Window
        Me.txtIP2.Cursor = System.Windows.Forms.Cursors.IBeam
        Me.txtIP2.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtIP2.ForeColor = System.Drawing.SystemColors.WindowText
        Me.txtIP2.Location = New System.Drawing.Point(111, 19)
        Me.txtIP2.MaxLength = 3
        Me.txtIP2.Name = "txtIP2"
        Me.txtIP2.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.txtIP2.Size = New System.Drawing.Size(32, 20)
        Me.txtIP2.TabIndex = 20
        '
        'Label2
        '
        Me.Label2.BackColor = System.Drawing.SystemColors.Control
        Me.Label2.Cursor = System.Windows.Forms.Cursors.Default
        Me.Label2.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.ForeColor = System.Drawing.SystemColors.ControlText
        Me.Label2.Location = New System.Drawing.Point(101, 21)
        Me.Label2.Name = "Label2"
        Me.Label2.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Label2.Size = New System.Drawing.Size(15, 17)
        Me.Label2.TabIndex = 19
        Me.Label2.Text = "."
        '
        'txtIP1
        '
        Me.txtIP1.AcceptsReturn = True
        Me.txtIP1.BackColor = System.Drawing.SystemColors.Window
        Me.txtIP1.Cursor = System.Windows.Forms.Cursors.IBeam
        Me.txtIP1.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtIP1.ForeColor = System.Drawing.SystemColors.WindowText
        Me.txtIP1.Location = New System.Drawing.Point(70, 19)
        Me.txtIP1.MaxLength = 3
        Me.txtIP1.Name = "txtIP1"
        Me.txtIP1.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.txtIP1.Size = New System.Drawing.Size(32, 20)
        Me.txtIP1.TabIndex = 18
        '
        'btnStart
        '
        Me.btnStart.BackColor = System.Drawing.SystemColors.Control
        Me.btnStart.Cursor = System.Windows.Forms.Cursors.Default
        Me.btnStart.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnStart.ForeColor = System.Drawing.SystemColors.ControlText
        Me.btnStart.Location = New System.Drawing.Point(348, 11)
        Me.btnStart.Name = "btnStart"
        Me.btnStart.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.btnStart.Size = New System.Drawing.Size(65, 25)
        Me.btnStart.TabIndex = 1
        Me.btnStart.Text = "&Start"
        Me.btnStart.UseVisualStyleBackColor = False
        '
        'chkVerbose
        '
        Me.chkVerbose.BackColor = System.Drawing.SystemColors.Control
        Me.chkVerbose.Checked = True
        Me.chkVerbose.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkVerbose.Cursor = System.Windows.Forms.Cursors.Default
        Me.chkVerbose.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.chkVerbose.ForeColor = System.Drawing.SystemColors.ControlText
        Me.chkVerbose.Location = New System.Drawing.Point(8, 19)
        Me.chkVerbose.Name = "chkVerbose"
        Me.chkVerbose.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.chkVerbose.Size = New System.Drawing.Size(74, 17)
        Me.chkVerbose.TabIndex = 7
        Me.chkVerbose.Text = "Verbose"
        Me.chkVerbose.UseVisualStyleBackColor = False
        '
        'cmdSave
        '
        Me.cmdSave.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.cmdSave.BackColor = System.Drawing.SystemColors.Control
        Me.cmdSave.Cursor = System.Windows.Forms.Cursors.Default
        Me.cmdSave.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cmdSave.ForeColor = System.Drawing.SystemColors.ControlText
        Me.cmdSave.Location = New System.Drawing.Point(8, 286)
        Me.cmdSave.Name = "cmdSave"
        Me.cmdSave.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.cmdSave.Size = New System.Drawing.Size(65, 25)
        Me.cmdSave.TabIndex = 8
        Me.cmdSave.Text = "S&ave"
        Me.cmdSave.UseVisualStyleBackColor = False
        '
        'txtDisplay
        '
        Me.txtDisplay.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtDisplay.Font = New System.Drawing.Font("Consolas", 9.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtDisplay.Location = New System.Drawing.Point(8, 38)
        Me.txtDisplay.Name = "txtDisplay"
        Me.txtDisplay.ReadOnly = True
        Me.txtDisplay.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical
        Me.txtDisplay.Size = New System.Drawing.Size(729, 238)
        Me.txtDisplay.TabIndex = 10
        Me.txtDisplay.TabStop = False
        Me.txtDisplay.Text = ""
        '
        'cmdClear
        '
        Me.cmdClear.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.cmdClear.BackColor = System.Drawing.SystemColors.Control
        Me.cmdClear.Cursor = System.Windows.Forms.Cursors.Default
        Me.cmdClear.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cmdClear.ForeColor = System.Drawing.SystemColors.ControlText
        Me.cmdClear.Location = New System.Drawing.Point(79, 286)
        Me.cmdClear.Name = "cmdClear"
        Me.cmdClear.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.cmdClear.Size = New System.Drawing.Size(65, 25)
        Me.cmdClear.TabIndex = 9
        Me.cmdClear.Text = "&Clear"
        Me.cmdClear.UseVisualStyleBackColor = False
        '
        'fraDisplay
        '
        Me.fraDisplay.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.fraDisplay.Controls.Add(Me.chkENQ)
        Me.fraDisplay.Controls.Add(Me.cmdClear)
        Me.fraDisplay.Controls.Add(Me.chkVerbose)
        Me.fraDisplay.Controls.Add(Me.txtDisplay)
        Me.fraDisplay.Controls.Add(Me.cmdSave)
        Me.fraDisplay.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.fraDisplay.Location = New System.Drawing.Point(8, 367)
        Me.fraDisplay.Name = "fraDisplay"
        Me.fraDisplay.Size = New System.Drawing.Size(745, 317)
        Me.fraDisplay.TabIndex = 20
        Me.fraDisplay.TabStop = False
        Me.fraDisplay.Text = "Communications"
        '
        'chkENQ
        '
        Me.chkENQ.AutoSize = True
        Me.chkENQ.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.chkENQ.Location = New System.Drawing.Point(100, 19)
        Me.chkENQ.Name = "chkENQ"
        Me.chkENQ.Size = New System.Drawing.Size(99, 18)
        Me.chkENQ.TabIndex = 11
        Me.chkENQ.Text = "ENQ, ACK/NAK"
        Me.chkENQ.UseVisualStyleBackColor = True
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ASTMTestToolToolStripMenuItem, Me.HelpToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Size = New System.Drawing.Size(759, 24)
        Me.MenuStrip1.TabIndex = 21
        Me.MenuStrip1.Text = "MenuStrip1"
        '
        'ASTMTestToolToolStripMenuItem
        '
        Me.ASTMTestToolToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ExitToolStripMenuItem})
        Me.ASTMTestToolToolStripMenuItem.Name = "ASTMTestToolToolStripMenuItem"
        Me.ASTMTestToolToolStripMenuItem.Size = New System.Drawing.Size(103, 20)
        Me.ASTMTestToolToolStripMenuItem.Text = "ASTM Test Tool"
        '
        'ExitToolStripMenuItem
        '
        Me.ExitToolStripMenuItem.Name = "ExitToolStripMenuItem"
        Me.ExitToolStripMenuItem.Size = New System.Drawing.Size(92, 22)
        Me.ExitToolStripMenuItem.Text = "Exit"
        '
        'HelpToolStripMenuItem
        '
        Me.HelpToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.HelpTopicsToolStripMenuItem, Me.AboutASTMTestToolToolStripMenuItem})
        Me.HelpToolStripMenuItem.Name = "HelpToolStripMenuItem"
        Me.HelpToolStripMenuItem.Size = New System.Drawing.Size(44, 20)
        Me.HelpToolStripMenuItem.Text = "Help"
        '
        'HelpTopicsToolStripMenuItem
        '
        Me.HelpTopicsToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ASTMProtocolToolStripMenuItem, Me.ASTMProtocolInputRecordsToolStripMenuItem, Me.ASTMProtocolOutputRecordsToolStripMenuItem, Me.ASTMToolStripMenuItem})
        Me.HelpTopicsToolStripMenuItem.Name = "HelpTopicsToolStripMenuItem"
        Me.HelpTopicsToolStripMenuItem.Size = New System.Drawing.Size(203, 22)
        Me.HelpTopicsToolStripMenuItem.Text = "Help Topics"
        '
        'ASTMProtocolToolStripMenuItem
        '
        Me.ASTMProtocolToolStripMenuItem.Name = "ASTMProtocolToolStripMenuItem"
        Me.ASTMProtocolToolStripMenuItem.Size = New System.Drawing.Size(250, 22)
        Me.ASTMProtocolToolStripMenuItem.Text = "Manual..."
        '
        'ASTMProtocolInputRecordsToolStripMenuItem
        '
        Me.ASTMProtocolInputRecordsToolStripMenuItem.Name = "ASTMProtocolInputRecordsToolStripMenuItem"
        Me.ASTMProtocolInputRecordsToolStripMenuItem.Size = New System.Drawing.Size(250, 22)
        Me.ASTMProtocolInputRecordsToolStripMenuItem.Text = "ASTM Protocol Input Records..."
        '
        'ASTMProtocolOutputRecordsToolStripMenuItem
        '
        Me.ASTMProtocolOutputRecordsToolStripMenuItem.Name = "ASTMProtocolOutputRecordsToolStripMenuItem"
        Me.ASTMProtocolOutputRecordsToolStripMenuItem.Size = New System.Drawing.Size(250, 22)
        Me.ASTMProtocolOutputRecordsToolStripMenuItem.Text = "ASTM Protocol Output Records"
        '
        'ASTMToolStripMenuItem
        '
        Me.ASTMToolStripMenuItem.Name = "ASTMToolStripMenuItem"
        Me.ASTMToolStripMenuItem.Size = New System.Drawing.Size(250, 22)
        Me.ASTMToolStripMenuItem.Text = "ASTM Protocol Sample Records..."
        '
        'AboutASTMTestToolToolStripMenuItem
        '
        Me.AboutASTMTestToolToolStripMenuItem.Name = "AboutASTMTestToolToolStripMenuItem"
        Me.AboutASTMTestToolToolStripMenuItem.Size = New System.Drawing.Size(203, 22)
        Me.AboutASTMTestToolToolStripMenuItem.Text = "About ASTM Test Tool..."
        '
        'GroupBoxHPOLLibraryFile
        '
        Me.GroupBoxHPOLLibraryFile.Controls.Add(Me.rdoPiccolo)
        Me.GroupBoxHPOLLibraryFile.Controls.Add(Me.rdoVetScan)
        Me.GroupBoxHPOLLibraryFile.Controls.Add(Me.TextBoxFileName)
        Me.GroupBoxHPOLLibraryFile.Controls.Add(Me.btnBrowseFile)
        Me.GroupBoxHPOLLibraryFile.Controls.Add(Me.btnCreateLibraryFile)
        Me.GroupBoxHPOLLibraryFile.Controls.Add(Me.btnFileTransmit)
        Me.GroupBoxHPOLLibraryFile.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupBoxHPOLLibraryFile.Location = New System.Drawing.Point(8, 133)
        Me.GroupBoxHPOLLibraryFile.Name = "GroupBoxHPOLLibraryFile"
        Me.GroupBoxHPOLLibraryFile.Size = New System.Drawing.Size(745, 42)
        Me.GroupBoxHPOLLibraryFile.TabIndex = 22
        Me.GroupBoxHPOLLibraryFile.TabStop = False
        Me.GroupBoxHPOLLibraryFile.Text = "HPOL Library File"
        '
        'rdoPiccolo
        '
        Me.rdoPiccolo.Font = New System.Drawing.Font("Arial", 7.5!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.rdoPiccolo.Location = New System.Drawing.Point(562, 25)
        Me.rdoPiccolo.Margin = New System.Windows.Forms.Padding(0)
        Me.rdoPiccolo.Name = "rdoPiccolo"
        Me.rdoPiccolo.Size = New System.Drawing.Size(65, 15)
        Me.rdoPiccolo.TabIndex = 21
        Me.rdoPiccolo.TabStop = True
        Me.rdoPiccolo.Text = "Piccolo"
        Me.rdoPiccolo.TextAlign = System.Drawing.ContentAlignment.TopCenter
        Me.rdoPiccolo.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.rdoPiccolo.UseVisualStyleBackColor = True
        '
        'rdoVetScan
        '
        Me.rdoVetScan.Checked = True
        Me.rdoVetScan.Font = New System.Drawing.Font("Arial", 7.5!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.rdoVetScan.Location = New System.Drawing.Point(562, 10)
        Me.rdoVetScan.Margin = New System.Windows.Forms.Padding(0)
        Me.rdoVetScan.Name = "rdoVetScan"
        Me.rdoVetScan.Size = New System.Drawing.Size(65, 15)
        Me.rdoVetScan.TabIndex = 20
        Me.rdoVetScan.TabStop = True
        Me.rdoVetScan.Text = "VetScan"
        Me.rdoVetScan.TextAlign = System.Drawing.ContentAlignment.TopRight
        Me.rdoVetScan.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.rdoVetScan.UseVisualStyleBackColor = True
        '
        'btnBrowseFile
        '
        Me.btnBrowseFile.Enabled = False
        Me.btnBrowseFile.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnBrowseFile.Location = New System.Drawing.Point(348, 13)
        Me.btnBrowseFile.Name = "btnBrowseFile"
        Me.btnBrowseFile.Size = New System.Drawing.Size(65, 23)
        Me.btnBrowseFile.TabIndex = 18
        Me.btnBrowseFile.Text = "Browse"
        Me.btnBrowseFile.UseVisualStyleBackColor = True
        '
        'FolderBrowserDialog1
        '
        Me.FolderBrowserDialog1.SelectedPath = "C:\Program Files (x86)\ASTM"
        '
        'OpenFileDialog1
        '
        Me.OpenFileDialog1.FileName = "OpenFileDialog1"
        '
        'BackgroundWorker2
        '
        Me.BackgroundWorker2.WorkerReportsProgress = True
        Me.BackgroundWorker2.WorkerSupportsCancellation = True
        '
        'Main_Renamed
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 14.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.SystemColors.Control
        Me.ClientSize = New System.Drawing.Size(759, 696)
        Me.Controls.Add(Me.GroupBoxHPOLLibraryFile)
        Me.Controls.Add(Me.fraDisplay)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.fraTx)
        Me.Controls.Add(Me.fraComm)
        Me.Controls.Add(Me.fraRecord)
        Me.Controls.Add(Me.MenuStrip1)
        Me.Cursor = System.Windows.Forms.Cursors.Default
        Me.Font = New System.Drawing.Font("Arial", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Location = New System.Drawing.Point(45, 63)
        Me.MainMenuStrip = Me.MenuStrip1
        Me.Name = "Main_Renamed"
        Me.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Main - ASTM Test Tool"
        Me.TransparencyKey = System.Drawing.Color.RoyalBlue
        Me.fraTx.ResumeLayout(False)
        Me.fraTx.PerformLayout()
        Me.fraComm.ResumeLayout(False)
        CType(Me.nudPortNumber, System.ComponentModel.ISupportInitialize).EndInit()
        Me.fraRecord.ResumeLayout(False)
        CType(Me.lvwRecord, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.fraDisplay.ResumeLayout(False)
        Me.fraDisplay.PerformLayout()
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        Me.GroupBoxHPOLLibraryFile.ResumeLayout(False)
        Me.GroupBoxHPOLLibraryFile.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents nudPortNumber As System.Windows.Forms.NumericUpDown
    Public WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Public WithEvents btnStart As System.Windows.Forms.Button
    Public WithEvents btnStop As System.Windows.Forms.Button
    Public WithEvents Label1 As System.Windows.Forms.Label
    Public WithEvents txtIP1 As System.Windows.Forms.TextBox
    Public WithEvents Label2 As System.Windows.Forms.Label
    Public WithEvents txtPort As System.Windows.Forms.TextBox
    Public WithEvents Label4 As System.Windows.Forms.Label
    Public WithEvents txtIP3 As System.Windows.Forms.TextBox
    Public WithEvents Label3 As System.Windows.Forms.Label
    Public WithEvents txtIP2 As System.Windows.Forms.TextBox
    Public WithEvents Label5 As System.Windows.Forms.Label
    Public WithEvents txtIP4 As System.Windows.Forms.TextBox
    Public WithEvents Label6 As System.Windows.Forms.Label
    Public WithEvents chkVerbose As System.Windows.Forms.CheckBox
    Public WithEvents cmdSave As System.Windows.Forms.Button
    Public WithEvents cmdClear As System.Windows.Forms.Button
    Public WithEvents txtDisplay As System.Windows.Forms.RichTextBox
    Friend WithEvents fraDisplay As System.Windows.Forms.GroupBox
    Friend WithEvents MenuStrip1 As System.Windows.Forms.MenuStrip
    Friend WithEvents ASTMTestToolToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ExitToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents HelpToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents HelpTopicsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ASTMProtocolToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ASTMToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents AboutASTMTestToolToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ASTMProtocolInputRecordsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ASTMProtocolOutputRecordsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents btnStopTransmit As System.Windows.Forms.Button
    Friend WithEvents chkENQ As CheckBox
    Friend WithEvents btnFileTransmit As Windows.Forms.Button
    Friend WithEvents btnCreateLibraryFile As Windows.Forms.Button
    Friend WithEvents GroupBoxHPOLLibraryFile As GroupBox
    Friend WithEvents FolderBrowserDialog1 As FolderBrowserDialog
    Friend WithEvents btnBrowseFile As Windows.Forms.Button
    Friend WithEvents OpenFileDialog1 As OpenFileDialog
    Friend WithEvents TextBoxFileName As TextBox
    Friend WithEvents BackgroundWorker2 As System.ComponentModel.BackgroundWorker
    Friend WithEvents rdoPiccolo As RadioButton
    Friend WithEvents rdoVetScan As RadioButton
#End Region
End Class