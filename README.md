# DataGridMultiColumnComboBoxLib

Custom WinForms DataGridView ComboBox dengan dukungan multi kolom.

## Fitur
- Multi kolom di dropdown
- Atur lebar tiap kolom (`ColumnWidths`)
- Atur lebar dropdown (`DropDownWidth`)
- Bisa digunakan sebagai DataGridView column

## Contoh penggunaan (VB.NET)
```vbnet
Dim col As New DataGridMultiColumnComboBox.DataGridViewMultiColumnComboBoxColumn()
col.HeaderText = "Organisasi"
col.DataSource = dtOrgList
col.DisplayMember = "org_kode"
col.ValueMember = "org_kode"
col.ColumnNames = {"org_kode", "org_dept", "org_bag", "org_subbag"}
col.ColumnWidths = {120, 200, 200, 200}
col.DropDownWidth = 750
DataGridView1.Columns.Add(col)
