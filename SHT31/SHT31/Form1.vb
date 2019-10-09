Imports System.IO.Ports '定義Serial連接傳輸功能
Imports System.Net.Mail '定義Mail發送功能
Public Class Form1

    Dim comPORT As String     '定義COMPORT宣告
    Dim receivedData As String = ""     '定義字串接收
    Dim myMail As New MailMessage() '定義mail發送


    'Serial物件
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        For Each sp As String In My.Computer.Ports.SerialPortNames
            ComboBox1.Items.Add(sp)
        Next
    End Sub

    '定義COMPORT選擇
    Private Sub comPort_ComboBox_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox1.SelectedIndexChanged
        comPORT = ""
        If (ComboBox1.SelectedItem <> "") Then
            comPORT = ComboBox1.SelectedItem
        End If
    End Sub

    '按鈕開關各項重要功能區塊
    Private Sub connect_BTN_Click(sender As Object, e As EventArgs) Handles Button1.Click

        ' 開啟功能判斷式
        If (Button1.Text = "Connect") Then
            If (comPORT <> "") Then
                'Serial 通訊段
                SerialPort1.Close()
                SerialPort1.PortName = comPORT
                SerialPort1.BaudRate = 9600
                SerialPort1.DataBits = 8
                SerialPort1.Parity = Parity.None
                SerialPort1.StopBits = StopBits.One
                SerialPort1.Handshake = Handshake.None
                SerialPort1.Encoding = System.Text.Encoding.Default
                SerialPort1.ReadTimeout = 10000
                SerialPort1.Open()

                'Button 顯示訊息
                Button1.Text = "Dis-Connect"

                'Timer 運作機制
                Timer1.Enabled = True
                Timer2.Enabled = True
                Timer3.Enabled = True
                Timer4.Enabled = True
                Timer5.Enabled = True
                Timer6.Enabled = True
                Timer7.Enabled = True
                Timer8.Enabled = True
                Timer9.Enabled = True
                Timer10.Enabled = True

                '通訊開啟標籤顯示
                Label2.Text = "Timer: ON"
                Label2.ForeColor = Color.ForestGreen

            Else

                MsgBox("Select a COM port first")

            End If

        Else
            '通訊關閉後
            SerialPort1.Close()

            '按鈕顯示回復初始設定
            Button1.Text = "Connect"

            'Timer關閉
            Timer1.Enabled = False
            Timer2.Enabled = False
            Timer3.Enabled = False
            Timer4.Enabled = False
            Timer5.Enabled = False
            Timer6.Enabled = False
            Timer7.Enabled = False
            Timer8.Enabled = False
            Timer9.Enabled = False
            Timer10.Enabled = False

            '標籤顯示關閉
            Label2.Text = "Timer: OFF"
            Label2.ForeColor = Color.Gray

            '溫溼度顯示標籤回復預設顏色
            Label5.BackColor = Color.White
            Label6.BackColor = Color.White
            Label8.BackColor = Color.White
            Label10.BackColor = Color.White

            '音效停止撥放
            My.Computer.Audio.Stop()
        End If
    End Sub

    '接收Serial 資料
    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick

        'Threading.Thread.Sleep(3000)
        Timer1.Interval = 3000
        receivedData = ReceiveSerialData()
        'TextBox1.Text = Label9.Text & "," & receivedData

    End Sub

    '接收Serial 資料
    Function ReceiveSerialData() As String
        Dim Incoming As String
        Try
            Incoming = SerialPort1.ReadLine() '解決掉資料問題
            If Incoming Is Nothing Then
                Return "nothing" & vbCrLf
            Else
                Return Incoming
            End If
        Catch ex As TimeoutException
            Return "Error: Serial Port read timed out."
        End Try

    End Function

    ' 顯示接收資料
    Private Sub Timer2_Tick(sender As Object, e As EventArgs) Handles Timer2.Tick

        Dim s As String

        Label7.Text = receivedData
        Label8.Text = receivedData
        Label10.Text = receivedData

        s = Label7.Text 'ID
        s = Label8.Text 'T
        s = Label10.Text 'H

        Try
            Dim ReceiveSerialData() As String
            ReceiveSerialData = s.Split(New Char() {","c})

            Label7.Text = ReceiveSerialData(0)
            Label8.Text = ReceiveSerialData(1)
            Label10.Text = ReceiveSerialData(2)

        Catch ex As Exception
        End Try

    End Sub

    '儲存接收到的資料
    Private Sub Timer3_Tick(sender As Object, e As EventArgs) Handles Timer3.Tick

        Try
            Dim file As System.IO.StreamWriter
            file = My.Computer.FileSystem.OpenTextFileWriter(TextBox2.Text, True)
            file.WriteLine(Label7.Text & "," & Label9.Text & "," & Label8.Text & "," & Label10.Text)
            file.Close()
        Catch ex As Exception
        End Try

    End Sub

    '顯示儲存路徑
    Private Sub SaveFileDialog1_FileOk(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles SaveFileDialog1.FileOk
        TextBox2.Text = SaveFileDialog1.FileName
    End Sub

    '超溫狀態鎖定與解除
    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox1.CheckedChanged
        If CheckBox1.Checked Then
            TextBox3.Enabled = False
        Else
            TextBox3.Enabled = True
        End If
    End Sub


    '超濕狀態鎖定與解除
    Private Sub CheckBox2_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox2.CheckedChanged

        If CheckBox2.Checked Then
            TextBox4.Enabled = False
        Else
            TextBox4.Enabled = True
        End If

    End Sub


    '溫度燈號
    Private Sub Timer4_Tick(sender As Object, e As EventArgs) Handles Timer4.Tick

        '設定警告值變數
        Try
            Dim T As Double = TextBox3.Text

            If Label8.Text > T Then
                Label8.BackColor = Color.Red
                Label5.BackColor = Color.Red
            Else
                Label8.BackColor = Color.White
                Label5.BackColor = Color.White
            End If

        Catch ex As Exception
        End Try
    End Sub

    '濕度燈號
    Private Sub Timer5_Tick(sender As Object, e As EventArgs) Handles Timer5.Tick
        '設定濕度警告值判斷
        Try
            Dim H As Integer = TextBox4.Text

            If Label10.Text > H Then
                Label10.BackColor = Color.Red
                Label6.BackColor = Color.Red

            Else
                Label10.BackColor = Color.White
                Label6.BackColor = Color.White
            End If
        Catch ex As Exception
        End Try
    End Sub

    '寄送Mail副程式
    Public Sub SendMail()
        Try
            'Dim myMail As New MailMessage()
            'myMail.From = New MailAddress("Tradebiz.Sensor@gmail.com", "Jasper") '發送者/ 發送人 	
            myMail.From = New MailAddress(TextBox1.Text)
            'myMail.To.Add("jaspers.dorfler@gmail.com")  '寫入收件者
            myMail.To.Add(TextBox6.Text)  '寫入收件者
            'myMail.Bcc.Add("456@gmail.com") '隱藏收件者 
            'myMail.CC.Add("789@gmail.com")  '寫入副本
            'myMail.SubjectEncoding = Encoding.UTF8  '主題編碼格式 
            'myMail.Subject = "The Value Over Alram!" '寫入主題 
            myMail.Subject = TextBox7.Text '寫入主題
            myMail.IsBodyHtml = True    'HTML語法(true:開啟false:關閉) 	
            'myMail.BodyEncoding = Encoding.UTF8 '內文編碼格式 
            'myMail.Body = "Please Notice!" '寫入內文 
            myMail.Body = TextBox8.Text '寫入內文
            'myMail.Attachments.Add(New System.Net.Mail.Attachment("C:\Files\FileA.txt"))  '附件 

            Dim mySmtp As New SmtpClient()  '建立SMTP連線 	
            mySmtp.Credentials = New System.Net.NetworkCredential("Tradebiz.Sensor@gmail.com", "~!Jaspers0324")  '連線驗證 
            mySmtp.Port = 587   'SMTP Port 
            mySmtp.Host = "smtp.gmail.com"  'SMTP主機名 	
            mySmtp.EnableSsl = True '開啟SSL驗證 
            mySmtp.Send(myMail) '發送 	
        Catch ex As Exception
        End Try
    End Sub


    '選取路徑顯示TextBox
    Private Sub OpenFileDialog1_FileOk(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles OpenFileDialog1.FileOk
        TextBox5.Text = OpenFileDialog1.FileName
    End Sub

    '建立撥放音效副程式
    Sub PlayLoopingBackgroundSoundFile()
        My.Computer.Audio.Play(TextBox5.Text, AudioPlayMode.BackgroundLoop)
    End Sub

    '建立停止音效副程式
    Sub StopBackgroundSound()
        My.Computer.Audio.Stop()
    End Sub

    '溫度超出檢查範圍寄送Mail
    Private Sub Timer6_Tick(sender As Object, e As EventArgs) Handles Timer6.Tick
        '設定溫度警告值判斷
        Try
            '設定警告值變數
            Dim T As Double = TextBox3.Text
            If Label8.Text > T And CheckBox3.Checked Then
                SendMail() 'mail send
            End If
        Catch ex As Exception
        End Try
    End Sub

    '濕度超出檢查範圍寄送Mail
    Private Sub Timer7_Tick(sender As Object, e As EventArgs) Handles Timer7.Tick
        '設定濕度警告值判斷
        Try
            '設定警告值變數
            Dim H As Double = TextBox4.Text
            If Label10.Text > H And CheckBox4.Checked Then
                SendMail() 'mail send
            End If
        Catch ex As Exception
        End Try
    End Sub

    'Alarm 音效播放
    Private Sub Timer8_Tick(sender As Object, e As EventArgs) Handles Timer8.Tick
        Try
            '設定警告值變數
            Dim T As Double = TextBox3.Text
            Dim H As Double = TextBox4.Text
            If Label8.Text > T And CheckBox5.Checked Or Label10.Text > H And CheckBox5.Checked Then
                PlayLoopingBackgroundSoundFile()
            End If
        Catch ex As Exception
        End Try
    End Sub

    '系統時間
    Private Sub Timer9_Tick(sender As Object, e As EventArgs) Handles Timer9.Tick
        Label9.Text = Date.Now.ToString("dd-MM-yyyy hh:mm:ss")
    End Sub

    Private Sub Timer10_Tick(sender As Object, e As EventArgs) Handles Timer10.Tick
        Try
            Chart1.Series("Humidity").Points.AddY(Label8.Text)
            Chart1.Series("Temperature").Points.AddY(Label10.Text)
            Chart1.ChartAreas("ChartArea1").AxisY.Minimum = 0
            Chart1.ChartAreas("ChartArea1").AxisY.Maximum = 100
            'Chart1.ChartAreas("ChartArea1").AxisY.Interval = 10
            Chart1.ChartAreas("ChartArea1").AxisX.Title = "Data Line"
            Chart1.ResetAutoValues()
        Catch ex As Exception
        End Try
    End Sub

    '上層選單開啟音效參數按鈕
    Private Sub ToolStripButton1_Click(sender As Object, e As EventArgs) Handles ToolStripButton1.Click

        OpenFileDialog1.Filter = "wav|*.wav" '過濾資料類型

        If OpenFileDialog1.ShowDialog() = Windows.Forms.DialogResult.OK Then
            OpenFileDialog1.Title = "Speichern WAV-Datei"
        End If

    End Sub

    '上層選單儲存紀錄檔案按鈕
    Private Sub ToolStripButton2_Click(sender As Object, e As EventArgs) Handles ToolStripButton2.Click

        SaveFileDialog1.Filter = "CSV|*.csv"

        If SaveFileDialog1.ShowDialog() = Windows.Forms.DialogResult.OK Then
            SaveFileDialog1.Title = "Speichern CSV-Datei"
        End If

    End Sub

    '顯示版本上層按鈕
    Private Sub ToolStripButton3_Click(sender As Object, e As EventArgs) Handles ToolStripButton3.Click
        MsgBox("Version: 1.0.0.2")
    End Sub

    '結束程式上層按鈕
    Private Sub ToolStripButton4_Click(sender As Object, e As EventArgs) Handles ToolStripButton4.Click
        End
    End Sub

    '儲存紀錄
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click

        SaveFileDialog1.Filter = "CSV|*.csv" '過濾資料類型

        If SaveFileDialog1.ShowDialog() = Windows.Forms.DialogResult.OK Then
            SaveFileDialog1.Title = "Speichern CSV-Datei"
        End If

    End Sub

    '開啟音效
    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click

        OpenFileDialog1.Filter = "wav|*.wav" '過濾資料類型

        If OpenFileDialog1.ShowDialog() = Windows.Forms.DialogResult.OK Then
            OpenFileDialog1.Title = "Speichern WAV-Datei"
        End If

    End Sub

End Class
