Option Strict Off
Option Explicit On
Module Utility
	'**********************************************************************************
	
	'File:  Utility.bas
	
	'Description:  This module contains the shared information for the project
	
	'Compiler:  This module is part of a project that is designed to be edited and compiled
	'in Visual Basic 6.0.  Choose "File->Make" from within the IDE to make the program.
	
	'$History: Utility.bas $
	' 
	' *****************  Version 1  *****************
	' User: Ballard      Date: 4/09/04    Time: 9:34a
	' Created in $/ASTM/Source
	' Added to SourceSafe.
	
	
	'constants
	
	Public Const APPERR As Integer = vbObjectError + &H3000s
	Public Const COMPERR As Integer = APPERR + &H100s
	
	Public Const RXENQTXT As Byte = 101
	Public Const TXACKTXT As Byte = 102
	Public Const WAITFRAMETXT As Byte = 103
	Public Const RXFRAMETXT As Byte = 104
	Public Const PARSEFRAMETXT As Byte = 105
	Public Const TXNAKTXT As Byte = 106
	Public Const RXEOTTXT As Byte = 107
	Public Const IDLETXT As Byte = 108
	Public Const CALCCHECKSUMTXT As Byte = 109
	Public Const REPCHECKSUMTXT As Byte = 110
	Public Const PARSERECTXT As Byte = 111
	Public Const STARTTXT As Byte = 112
	Public Const STOPTXT As Byte = 113
	Public Const BUILDRECTXT As Byte = 114
	Public Const BUILDFRAMETXT As Byte = 115
	Public Const TXENQTXT As Byte = 116
	Public Const WAITRESPONSETXT As Byte = 117
	Public Const RXACKTXT As Byte = 118
	Public Const TXFRAMETXT As Byte = 119
	Public Const RXNAKTXT As Byte = 120
	Public Const TXEOTTXT As Byte = 121
	Public Const TXBUSYTXT As Byte = 122
	Public Const TXFRAMEAGAINTXT As Byte = 123
	Public Const MAXRETRYTXT As Byte = 124
	Public Const TXSEQUENCETXT As Byte = 125
	
	Public Const TIMEOUTTXT As Byte = 201
	Public Const FRAMEERRTXT As Byte = 202
	Public Const EOFERRTXT As Byte = 203
	Public Const RECFIELDERRTXT As Byte = 204
	Public Const BLANKDELIMITERERRTXT As Byte = 205
	
	'***********************************************************************
	
	'PROCEDURE:   SaveFile()
	
	'DESCRIPTION: Saves text to a file
	
	'PARAMETERS:  inFilename - the file name to save
	'             inText - the text to save to the file
	
	'RETURNED:    N/A
	
	'*********************************************************************
	Public Sub SaveFile(ByVal inFilename As String, ByVal inText As String)
		On Error GoTo ErrTrap
		
        FileOpen(1, inFilename, OpenMode.Output) ' open the output file
       
        PrintLine(1, inText)

        FileClose(1) ' close the output file

        Exit Sub
ErrTrap:
        Call Err.Raise(Err.Number, Err.Source & " | Utility.SaveFile", Err.Description)
    End Sub
End Module