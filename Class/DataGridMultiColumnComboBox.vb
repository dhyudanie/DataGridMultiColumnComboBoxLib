Imports System.Windows.Forms
Imports System.Drawing
Imports System.Data
Imports System.ComponentModel

Namespace DataGridMultiColumnComboBox

    ' ==========================================================
    ' Custom ComboBox dengan tampilan multi kolom
    ' ==========================================================
    Public Class MultiColumnComboBox
        Inherits ComboBox

        <Browsable(True)>
        Public Property ColumnWidths As Integer() = {100, 150, 150}

        <Browsable(True)>
        Public Property ColumnNames As String() = {}

        Private _dropDownWidth As Integer = 0

        <Browsable(True), Category("Layout")>
        Public Shadows Property DropDownWidth As Integer
            Get
                If _dropDownWidth > 0 Then
                    Return _dropDownWidth
                Else
                    Return MyBase.DropDownWidth
                End If
            End Get
            Set(value As Integer)
                _dropDownWidth = value
                MyBase.DropDownWidth = value
            End Set
        End Property


        Public Sub New()
            Me.DrawMode = DrawMode.OwnerDrawFixed
            Me.DropDownStyle = ComboBoxStyle.DropDownList
        End Sub

        Protected Overrides Sub OnDrawItem(e As DrawItemEventArgs)
            If e.Index < 0 Then Return
            e.DrawBackground()

            Dim rowView As DataRowView = TryCast(Me.Items(e.Index), DataRowView)
            If rowView IsNot Nothing AndAlso ColumnNames.Length > 0 Then
                Dim x As Integer = e.Bounds.Left
                For i As Integer = 0 To ColumnNames.Length - 1
                    Dim text As String = rowView(ColumnNames(i)).ToString()
                    Dim rect As New Rectangle(x, e.Bounds.Top, ColumnWidths(i), e.Bounds.Height)
                    TextRenderer.DrawText(e.Graphics, text, e.Font, rect, e.ForeColor, TextFormatFlags.Left)
                    x += ColumnWidths(i)
                Next
            Else
                TextRenderer.DrawText(e.Graphics, Me.Items(e.Index).ToString(), e.Font, e.Bounds, e.ForeColor)
            End If

            e.DrawFocusRectangle()
        End Sub
    End Class

    ' ==========================================================
    ' EditingControl untuk DataGridView
    ' ==========================================================
    Public Class DataGridViewMultiColumnComboBoxEditingControl
        Inherits MultiColumnComboBox
        Implements IDataGridViewEditingControl

        Private dataGridViewControl As DataGridView
        Private valueChanged As Boolean
        Private rowIndexNum As Integer

        Public Property EditingControlFormattedValue As Object Implements IDataGridViewEditingControl.EditingControlFormattedValue
            Get
                Return Me.Text
            End Get
            Set(value As Object)
                Me.Text = value?.ToString()
            End Set
        End Property

        Public Function GetEditingControlFormattedValue(context As DataGridViewDataErrorContexts) As Object Implements IDataGridViewEditingControl.GetEditingControlFormattedValue
            Return Me.Text
        End Function

        Public Sub ApplyCellStyleToEditingControl(cellStyle As DataGridViewCellStyle) Implements IDataGridViewEditingControl.ApplyCellStyleToEditingControl
            Me.Font = cellStyle.Font
            Me.ForeColor = cellStyle.ForeColor
            Me.BackColor = cellStyle.BackColor
        End Sub

        Public Property EditingControlRowIndex As Integer Implements IDataGridViewEditingControl.EditingControlRowIndex
            Get
                Return rowIndexNum
            End Get
            Set(value As Integer)
                rowIndexNum = value
            End Set
        End Property

        Public Function EditingControlWantsInputKey(keyData As Keys, dataGridViewWantsInputKey As Boolean) As Boolean Implements IDataGridViewEditingControl.EditingControlWantsInputKey
            Return True
        End Function

        Public Sub PrepareEditingControlForEdit(selectAll As Boolean) Implements IDataGridViewEditingControl.PrepareEditingControlForEdit
        End Sub

        Public ReadOnly Property RepositionEditingControlOnValueChange As Boolean Implements IDataGridViewEditingControl.RepositionEditingControlOnValueChange
            Get
                Return False
            End Get
        End Property

        Public Property EditingControlDataGridView As DataGridView Implements IDataGridViewEditingControl.EditingControlDataGridView
            Get
                Return dataGridViewControl
            End Get
            Set(value As DataGridView)
                dataGridViewControl = value
            End Set
        End Property

        Public Property EditingControlValueChanged As Boolean Implements IDataGridViewEditingControl.EditingControlValueChanged
            Get
                Return valueChanged
            End Get
            Set(value As Boolean)
                valueChanged = value
            End Set
        End Property

        Public ReadOnly Property EditingControlCursor As Cursor Implements IDataGridViewEditingControl.EditingPanelCursor
            Get
                Return Cursors.Default
            End Get
        End Property

        Protected Overrides Sub OnSelectedValueChanged(e As EventArgs)
            valueChanged = True
            Me.EditingControlDataGridView?.NotifyCurrentCellDirty(True)
            MyBase.OnSelectedValueChanged(e)
        End Sub
    End Class

    ' ==========================================================
    ' Kolom khusus untuk DataGridView
    ' ==========================================================
    Public Class DataGridViewMultiColumnComboBoxColumn
        Inherits DataGridViewColumn

        Public Sub New()
            MyBase.New(New DataGridViewMultiColumnComboBoxCell())
        End Sub

        Public Property DataSource As Object
        Public Property DisplayMember As String
        Public Property ValueMember As String
        Public Property ColumnNames As String()
        Public Property ColumnWidths As Integer()

        Public Property DropDownWidth As Integer

        Public Overrides Function Clone() As Object
            Dim c As DataGridViewMultiColumnComboBoxColumn =
        DirectCast(MyBase.Clone(), DataGridViewMultiColumnComboBoxColumn)
            c.DataSource = Me.DataSource
            c.DisplayMember = Me.DisplayMember
            c.ValueMember = Me.ValueMember
            c.ColumnNames = Me.ColumnNames
            c.ColumnWidths = Me.ColumnWidths
            c.DropDownWidth = Me.DropDownWidth
        End Function


        Public Overrides Property CellTemplate As DataGridViewCell
            Get
                Return MyBase.CellTemplate
            End Get
            Set(value As DataGridViewCell)
                If value IsNot Nothing AndAlso Not TypeOf value Is DataGridViewMultiColumnComboBoxCell Then
                    Throw New InvalidCastException("CellTemplate harus bertipe DataGridViewMultiColumnComboBoxCell")
                End If
                MyBase.CellTemplate = value
            End Set
        End Property
    End Class


    Public Class DataGridViewMultiColumnComboBoxCell
        Inherits DataGridViewTextBoxCell

        Public Property DataSource As Object
        Public Property DisplayMember As String
        Public Property ValueMember As String
        Public Property ColumnNames As String()
        Public Property ColumnWidths As Integer()

        Public Overrides Sub InitializeEditingControl(rowIndex As Integer, initialFormattedValue As Object, dataGridViewCellStyle As DataGridViewCellStyle)
            MyBase.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle)

            Dim ctl As DataGridViewMultiColumnComboBoxEditingControl =
                TryCast(DataGridView.EditingControl, DataGridViewMultiColumnComboBoxEditingControl)

            If ctl IsNot Nothing Then

                Dim col As DataGridViewMultiColumnComboBoxColumn =
                    TryCast(Me.OwningColumn, DataGridViewMultiColumnComboBoxColumn)

                Dim src = If(Me.DataSource, col?.DataSource)
                Dim disp = If(Me.DisplayMember, col?.DisplayMember)
                Dim valm = If(Me.ValueMember, col?.ValueMember)
                Dim names = If(Me.ColumnNames, col?.ColumnNames)
                Dim widths = If(Me.ColumnWidths, col?.ColumnWidths)
                Dim ddWidth = If(col?.DropDownWidth, 0)

                ctl.DataSource = src
                ctl.DisplayMember = If(String.IsNullOrEmpty(disp), Nothing, disp)
                ctl.ValueMember = If(String.IsNullOrEmpty(valm), Nothing, valm)
                If names IsNot Nothing Then ctl.ColumnNames = names
                If widths IsNot Nothing Then ctl.ColumnWidths = widths
                If ddWidth > 0 Then ctl.DropDownWidth = ddWidth
            End If
        End Sub

        Public Overrides ReadOnly Property EditType As Type
            Get
                Return GetType(DataGridViewMultiColumnComboBoxEditingControl)
            End Get
        End Property

        Public Overrides ReadOnly Property ValueType As Type
            Get
                Return GetType(String)
            End Get
        End Property

        Public Overrides ReadOnly Property DefaultNewRowValue As Object
            Get
                Return Nothing
            End Get
        End Property

        Public Overrides Function Clone() As Object
            Dim c As DataGridViewMultiColumnComboBoxCell =
                DirectCast(MyBase.Clone(), DataGridViewMultiColumnComboBoxCell)
            c.DataSource = Me.DataSource
            c.DisplayMember = Me.DisplayMember
            c.ValueMember = Me.ValueMember
            c.ColumnNames = Me.ColumnNames
            c.ColumnWidths = Me.ColumnWidths
            Return c
        End Function
    End Class


End Namespace
