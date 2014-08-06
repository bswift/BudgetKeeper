<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frm_OM_Test
	Inherits System.Windows.Forms.Form

	'Form overrides dispose to clean up the component list.
	<System.Diagnostics.DebuggerNonUserCode()> _
	Protected Overrides Sub Dispose(ByVal disposing As Boolean)
		Try
			If disposing AndAlso components IsNot Nothing Then
				components.Dispose()
			End If
		Finally
			MyBase.Dispose(disposing)
		End Try
	End Sub

	'Required by the Windows Form Designer
	Private components As System.ComponentModel.IContainer

	'NOTE: The following procedure is required by the Windows Form Designer
	'It can be modified using the Windows Form Designer.  
	'Do not modify it using the code editor.
	<System.Diagnostics.DebuggerStepThrough()> _
	Private Sub InitializeComponent()
		Me.lbl_Username = New System.Windows.Forms.Label()
		Me.lbl_Password = New System.Windows.Forms.Label()
		Me.txt_Username = New System.Windows.Forms.TextBox()
		Me.txt_Password = New System.Windows.Forms.TextBox()
		Me.btn_Login = New System.Windows.Forms.Button()
		Me.SuspendLayout()
		'
		'lbl_Username
		'
		Me.lbl_Username.AutoSize = True
		Me.lbl_Username.Location = New System.Drawing.Point(78, 73)
		Me.lbl_Username.Name = "lbl_Username"
		Me.lbl_Username.Size = New System.Drawing.Size(87, 20)
		Me.lbl_Username.TabIndex = 0
		Me.lbl_Username.Text = "Username:"
		'
		'lbl_Password
		'
		Me.lbl_Password.AutoSize = True
		Me.lbl_Password.Location = New System.Drawing.Point(78, 115)
		Me.lbl_Password.Name = "lbl_Password"
		Me.lbl_Password.Size = New System.Drawing.Size(82, 20)
		Me.lbl_Password.TabIndex = 1
		Me.lbl_Password.Text = "Password:"
		'
		'txt_Username
		'
		Me.txt_Username.Location = New System.Drawing.Point(171, 73)
		Me.txt_Username.Name = "txt_Username"
		Me.txt_Username.Size = New System.Drawing.Size(217, 26)
		Me.txt_Username.TabIndex = 2
		Me.txt_Username.Text = "bk_admin"
		'
		'txt_Password
		'
		Me.txt_Password.Location = New System.Drawing.Point(171, 115)
		Me.txt_Password.Name = "txt_Password"
		Me.txt_Password.Size = New System.Drawing.Size(217, 26)
		Me.txt_Password.TabIndex = 3
		Me.txt_Password.Text = "MCD14lb$:"
		'
		'btn_Login
		'
		Me.btn_Login.Location = New System.Drawing.Point(171, 179)
		Me.btn_Login.Name = "btn_Login"
		Me.btn_Login.Size = New System.Drawing.Size(146, 43)
		Me.btn_Login.TabIndex = 4
		Me.btn_Login.Text = "Login"
		Me.btn_Login.UseVisualStyleBackColor = True
		'
		'frm_OM_Test
		'
		Me.AutoScaleDimensions = New System.Drawing.SizeF(9.0!, 20.0!)
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.ClientSize = New System.Drawing.Size(485, 282)
		Me.Controls.Add(Me.btn_Login)
		Me.Controls.Add(Me.txt_Password)
		Me.Controls.Add(Me.txt_Username)
		Me.Controls.Add(Me.lbl_Password)
		Me.Controls.Add(Me.lbl_Username)
		Me.Name = "frm_OM_Test"
		Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
		Me.Text = "OM Test"
		Me.ResumeLayout(False)
		Me.PerformLayout()

	End Sub
	Friend WithEvents lbl_Username As System.Windows.Forms.Label
	Friend WithEvents lbl_Password As System.Windows.Forms.Label
	Friend WithEvents txt_Username As System.Windows.Forms.TextBox
	Friend WithEvents txt_Password As System.Windows.Forms.TextBox
	Friend WithEvents btn_Login As System.Windows.Forms.Button

End Class
