namespace PGPerdas
{
    partial class frmPrincipal
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.cbModel = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cbMeterComplete = new System.Windows.Forms.CheckBox();
            this.lVPUMin = new System.Windows.Forms.Label();
            this.tbVPUMin = new System.Windows.Forms.TextBox();
            this.cbReinicializeNT = new System.Windows.Forms.CheckBox();
            this.bBalance = new System.Windows.Forms.Button();
            this.bReportVoltageSimple = new System.Windows.Forms.Button();
            this.bReportVoltageComplete = new System.Windows.Forms.Button();
            this.bReportSGT = new System.Windows.Forms.Button();
            this.bReportAnalisys = new System.Windows.Forms.Button();
            this.bMedidoresDB = new System.Windows.Forms.Button();
            this.bTrafosSubDB = new System.Windows.Forms.Button();
            this.bExportDB = new System.Windows.Forms.Button();
            this.bExecuteBD = new System.Windows.Forms.Button();
            this.clbOptionsRun = new System.Windows.Forms.CheckedListBox();
            this.tbObjectFeeder = new System.Windows.Forms.TextBox();
            this.lObjectFeeder = new System.Windows.Forms.Label();
            this.tbObjectBase = new System.Windows.Forms.TextBox();
            this.lObjetcBase = new System.Windows.Forms.Label();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.lbPswd = new System.Windows.Forms.Label();
            this.txtPswd = new System.Windows.Forms.TextBox();
            this.lbUser = new System.Windows.Forms.Label();
            this.txtUser = new System.Windows.Forms.TextBox();
            this.cbServer = new System.Windows.Forms.ComboBox();
            this.bDesconnection = new System.Windows.Forms.Button();
            this.bConnection = new System.Windows.Forms.Button();
            this.lServer = new System.Windows.Forms.Label();
            this.lDataBase = new System.Windows.Forms.Label();
            this.tbDataBase = new System.Windows.Forms.TextBox();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.bReportFile = new System.Windows.Forms.Button();
            this.bExecuteFile = new System.Windows.Forms.Button();
            this.bSelectPathFile = new System.Windows.Forms.Button();
            this.tbPathFile = new System.Windows.Forms.TextBox();
            this.lPathFile = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.rtbInfo = new System.Windows.Forms.RichTextBox();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.tbCommand = new System.Windows.Forms.TextBox();
            this.rtbAdmin = new System.Windows.Forms.RichTextBox();
            this.rtbLog = new System.Windows.Forms.RichTextBox();
            this.ofdSelectPathFileDSS = new System.Windows.Forms.OpenFileDialog();
            this.fbdSelectPathWrite = new System.Windows.Forms.FolderBrowserDialog();
            this.tabControl1.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage5.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage5);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(584, 356);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.cbModel);
            this.tabPage3.Controls.Add(this.label1);
            this.tabPage3.Controls.Add(this.cbMeterComplete);
            this.tabPage3.Controls.Add(this.lVPUMin);
            this.tabPage3.Controls.Add(this.tbVPUMin);
            this.tabPage3.Controls.Add(this.cbReinicializeNT);
            this.tabPage3.Controls.Add(this.bBalance);
            this.tabPage3.Controls.Add(this.bReportVoltageSimple);
            this.tabPage3.Controls.Add(this.bReportVoltageComplete);
            this.tabPage3.Controls.Add(this.bReportSGT);
            this.tabPage3.Controls.Add(this.bReportAnalisys);
            this.tabPage3.Controls.Add(this.bMedidoresDB);
            this.tabPage3.Controls.Add(this.bTrafosSubDB);
            this.tabPage3.Controls.Add(this.bExportDB);
            this.tabPage3.Controls.Add(this.bExecuteBD);
            this.tabPage3.Controls.Add(this.clbOptionsRun);
            this.tabPage3.Controls.Add(this.tbObjectFeeder);
            this.tabPage3.Controls.Add(this.lObjectFeeder);
            this.tabPage3.Controls.Add(this.tbObjectBase);
            this.tabPage3.Controls.Add(this.lObjetcBase);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(576, 330);
            this.tabPage3.TabIndex = 1;
            this.tabPage3.Text = "Execução via Banco de Dados";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // cbModel
            // 
            this.cbModel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbModel.FormattingEnabled = true;
            this.cbModel.Items.AddRange(new object[] {
            "1 - Modelo de carga 2 e 3",
            "2 - Modelo de carga 1",
            "3 - Modelo de carga 3"});
            this.cbModel.Location = new System.Drawing.Point(353, 231);
            this.cbModel.Name = "cbModel";
            this.cbModel.Size = new System.Drawing.Size(121, 21);
            this.cbModel.TabIndex = 20;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(220, 235);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(131, 13);
            this.label1.TabIndex = 19;
            this.label1.Text = "Modelos de carga (1, 2, 3)";
            // 
            // cbMeterComplete
            // 
            this.cbMeterComplete.AutoSize = true;
            this.cbMeterComplete.Location = new System.Drawing.Point(18, 142);
            this.cbMeterComplete.Name = "cbMeterComplete";
            this.cbMeterComplete.Size = new System.Drawing.Size(114, 17);
            this.cbMeterComplete.TabIndex = 17;
            this.cbMeterComplete.Text = "Medição Completa";
            this.cbMeterComplete.UseVisualStyleBackColor = true;
            // 
            // lVPUMin
            // 
            this.lVPUMin.AutoSize = true;
            this.lVPUMin.Location = new System.Drawing.Point(220, 208);
            this.lVPUMin.Name = "lVPUMin";
            this.lVPUMin.Size = new System.Drawing.Size(102, 13);
            this.lVPUMin.TabIndex = 16;
            this.lVPUMin.Text = "Tensão Mínima (pu)";
            // 
            // tbVPUMin
            // 
            this.tbVPUMin.Location = new System.Drawing.Point(353, 205);
            this.tbVPUMin.Name = "tbVPUMin";
            this.tbVPUMin.Size = new System.Drawing.Size(100, 20);
            this.tbVPUMin.TabIndex = 15;
            this.tbVPUMin.Text = "0,6";
            this.tbVPUMin.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbVPUMin_KeyPress);
            // 
            // cbReinicializeNT
            // 
            this.cbReinicializeNT.AutoSize = true;
            this.cbReinicializeNT.Location = new System.Drawing.Point(18, 119);
            this.cbReinicializeNT.Name = "cbReinicializeNT";
            this.cbReinicializeNT.Size = new System.Drawing.Size(185, 17);
            this.cbReinicializeNT.TabIndex = 14;
            this.cbReinicializeNT.Text = "Reinicializa Cargas Não Técnicas";
            this.cbReinicializeNT.UseVisualStyleBackColor = true;
            // 
            // bBalance
            // 
            this.bBalance.Location = new System.Drawing.Point(164, 293);
            this.bBalance.Name = "bBalance";
            this.bBalance.Size = new System.Drawing.Size(120, 23);
            this.bBalance.TabIndex = 13;
            this.bBalance.Text = "Executa Balanço";
            this.bBalance.UseVisualStyleBackColor = true;
            this.bBalance.Click += new System.EventHandler(this.bBalance_Click);
            // 
            // bReportVoltageSimple
            // 
            this.bReportVoltageSimple.Location = new System.Drawing.Point(416, 264);
            this.bReportVoltageSimple.Name = "bReportVoltageSimple";
            this.bReportVoltageSimple.Size = new System.Drawing.Size(120, 23);
            this.bReportVoltageSimple.TabIndex = 12;
            this.bReportVoltageSimple.Text = "Tensão Simples";
            this.bReportVoltageSimple.UseVisualStyleBackColor = true;
            this.bReportVoltageSimple.Click += new System.EventHandler(this.bReportVoltageSimple_Click);
            // 
            // bReportVoltageComplete
            // 
            this.bReportVoltageComplete.Location = new System.Drawing.Point(290, 264);
            this.bReportVoltageComplete.Name = "bReportVoltageComplete";
            this.bReportVoltageComplete.Size = new System.Drawing.Size(120, 23);
            this.bReportVoltageComplete.TabIndex = 11;
            this.bReportVoltageComplete.Text = "Tensão Completo";
            this.bReportVoltageComplete.UseVisualStyleBackColor = true;
            this.bReportVoltageComplete.Click += new System.EventHandler(this.bReportVoltageComplete_Click);
            // 
            // bReportSGT
            // 
            this.bReportSGT.Location = new System.Drawing.Point(164, 264);
            this.bReportSGT.Name = "bReportSGT";
            this.bReportSGT.Size = new System.Drawing.Size(120, 23);
            this.bReportSGT.TabIndex = 10;
            this.bReportSGT.Text = "Relatório SGT";
            this.bReportSGT.UseVisualStyleBackColor = true;
            this.bReportSGT.Click += new System.EventHandler(this.bReportSGT_Click);
            // 
            // bReportAnalisys
            // 
            this.bReportAnalisys.Location = new System.Drawing.Point(38, 264);
            this.bReportAnalisys.Name = "bReportAnalisys";
            this.bReportAnalisys.Size = new System.Drawing.Size(120, 23);
            this.bReportAnalisys.TabIndex = 9;
            this.bReportAnalisys.Text = "Relatório de Análises";
            this.bReportAnalisys.UseVisualStyleBackColor = true;
            this.bReportAnalisys.Click += new System.EventHandler(this.bReportAnalisys_Click);
            // 
            // bMedidoresDB
            // 
            this.bMedidoresDB.Location = new System.Drawing.Point(290, 293);
            this.bMedidoresDB.Name = "bMedidoresDB";
            this.bMedidoresDB.Size = new System.Drawing.Size(120, 23);
            this.bMedidoresDB.TabIndex = 8;
            this.bMedidoresDB.Text = "Executa Medidores";
            this.bMedidoresDB.UseVisualStyleBackColor = true;
            this.bMedidoresDB.Click += new System.EventHandler(this.bMedidoresDB_Click);
            // 
            // bTrafosSubDB
            // 
            this.bTrafosSubDB.Location = new System.Drawing.Point(38, 293);
            this.bTrafosSubDB.Name = "bTrafosSubDB";
            this.bTrafosSubDB.Size = new System.Drawing.Size(120, 23);
            this.bTrafosSubDB.TabIndex = 7;
            this.bTrafosSubDB.Text = "Executa TP";
            this.bTrafosSubDB.UseVisualStyleBackColor = true;
            this.bTrafosSubDB.Click += new System.EventHandler(this.bTrafosSubDB_Click);
            // 
            // bExportDB
            // 
            this.bExportDB.Location = new System.Drawing.Point(416, 293);
            this.bExportDB.Name = "bExportDB";
            this.bExportDB.Size = new System.Drawing.Size(120, 23);
            this.bExportDB.TabIndex = 6;
            this.bExportDB.Text = "Exporta";
            this.bExportDB.UseVisualStyleBackColor = true;
            this.bExportDB.Click += new System.EventHandler(this.bExportDB_Click);
            // 
            // bExecuteBD
            // 
            this.bExecuteBD.Location = new System.Drawing.Point(38, 235);
            this.bExecuteBD.Name = "bExecuteBD";
            this.bExecuteBD.Size = new System.Drawing.Size(120, 23);
            this.bExecuteBD.TabIndex = 5;
            this.bExecuteBD.Text = "Executa MT-BT";
            this.bExecuteBD.UseVisualStyleBackColor = true;
            this.bExecuteBD.Click += new System.EventHandler(this.bExecuteBD_Click);
            // 
            // clbOptionsRun
            // 
            this.clbOptionsRun.FormattingEnabled = true;
            this.clbOptionsRun.Items.AddRange(new object[] {
            "Convergência de Perda Não Técnica",
            "Transformadores ABNT",
            "Adequação Tensão Mínima das Cargas MT (0,93 pu)",
            "Adequação Tensão Mínima das Cargas BT (0,92 pu)",
            "Limitar Máxima Tensão de Barras e Reguladores (1,05 pu)",
            "Limitar o Ramal (30m)",
            "Utilizar Tap nos Transformadores",
            "Limitar Cargas BT (Potência ativa do transformador)",
            "Eliminar Transformadores Vazios",
            "Neutralizar Transformadores de Terceiros",
            "Neutralizar Redes de Terceiros (MT/BT)"});
            this.clbOptionsRun.Location = new System.Drawing.Point(223, 14);
            this.clbOptionsRun.Name = "clbOptionsRun";
            this.clbOptionsRun.Size = new System.Drawing.Size(313, 169);
            this.clbOptionsRun.TabIndex = 4;
            this.clbOptionsRun.SelectedIndexChanged += new System.EventHandler(this.clbOptionsRun_SelectedIndexChanged);
            // 
            // tbObjectFeeder
            // 
            this.tbObjectFeeder.Location = new System.Drawing.Point(18, 80);
            this.tbObjectFeeder.Name = "tbObjectFeeder";
            this.tbObjectFeeder.Size = new System.Drawing.Size(199, 20);
            this.tbObjectFeeder.TabIndex = 3;
            this.tbObjectFeeder.Text = "RIPO0113";
            // 
            // lObjectFeeder
            // 
            this.lObjectFeeder.AutoSize = true;
            this.lObjectFeeder.Location = new System.Drawing.Point(15, 64);
            this.lObjectFeeder.Name = "lObjectFeeder";
            this.lObjectFeeder.Size = new System.Drawing.Size(200, 13);
            this.lObjectFeeder.TabIndex = 2;
            this.lObjectFeeder.Text = "Alimentador (Separar alimentadores por ;)";
            // 
            // tbObjectBase
            // 
            this.tbObjectBase.Location = new System.Drawing.Point(18, 28);
            this.tbObjectBase.Name = "tbObjectBase";
            this.tbObjectBase.Size = new System.Drawing.Size(199, 20);
            this.tbObjectBase.TabIndex = 1;
            this.tbObjectBase.Text = "391";
            // 
            // lObjetcBase
            // 
            this.lObjetcBase.AutoSize = true;
            this.lObjetcBase.Location = new System.Drawing.Point(15, 12);
            this.lObjetcBase.Name = "lObjetcBase";
            this.lObjetcBase.Size = new System.Drawing.Size(80, 13);
            this.lObjetcBase.TabIndex = 0;
            this.lObjetcBase.Text = "Base Analisada";
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.lbPswd);
            this.tabPage1.Controls.Add(this.txtPswd);
            this.tabPage1.Controls.Add(this.lbUser);
            this.tabPage1.Controls.Add(this.txtUser);
            this.tabPage1.Controls.Add(this.cbServer);
            this.tabPage1.Controls.Add(this.bDesconnection);
            this.tabPage1.Controls.Add(this.bConnection);
            this.tabPage1.Controls.Add(this.lServer);
            this.tabPage1.Controls.Add(this.lDataBase);
            this.tabPage1.Controls.Add(this.tbDataBase);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(576, 330);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Banco de Dados";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // lbPswd
            // 
            this.lbPswd.AutoSize = true;
            this.lbPswd.Location = new System.Drawing.Point(273, 64);
            this.lbPswd.Name = "lbPswd";
            this.lbPswd.Size = new System.Drawing.Size(38, 13);
            this.lbPswd.TabIndex = 10;
            this.lbPswd.Text = "Senha";
            // 
            // txtPswd
            // 
            this.txtPswd.Location = new System.Drawing.Point(276, 80);
            this.txtPswd.Name = "txtPswd";
            this.txtPswd.PasswordChar = '*';
            this.txtPswd.Size = new System.Drawing.Size(227, 20);
            this.txtPswd.TabIndex = 9;
            this.txtPswd.Text = "Perdas2020";
            // 
            // lbUser
            // 
            this.lbUser.AutoSize = true;
            this.lbUser.Location = new System.Drawing.Point(273, 25);
            this.lbUser.Name = "lbUser";
            this.lbUser.Size = new System.Drawing.Size(43, 13);
            this.lbUser.TabIndex = 8;
            this.lbUser.Text = "Usuário";
            // 
            // txtUser
            // 
            this.txtUser.Location = new System.Drawing.Point(276, 41);
            this.txtUser.Name = "txtUser";
            this.txtUser.Size = new System.Drawing.Size(227, 20);
            this.txtUser.TabIndex = 7;
            this.txtUser.Text = "usr_GeoPerdas";
            // 
            // cbServer
            // 
            this.cbServer.FormattingEnabled = true;
            this.cbServer.Items.AddRange(new object[] {
            "bdgeodisthom,5480",
            "bdgeodist,5447"});
            this.cbServer.Location = new System.Drawing.Point(21, 40);
            this.cbServer.Name = "cbServer";
            this.cbServer.Size = new System.Drawing.Size(227, 21);
            this.cbServer.TabIndex = 6;
            this.cbServer.Text = "bdgeodisthom,5480";
            this.cbServer.SelectedIndexChanged += new System.EventHandler(this.cbServer_SelectedIndexChanged);
            // 
            // bDesconnection
            // 
            this.bDesconnection.Location = new System.Drawing.Point(21, 151);
            this.bDesconnection.Name = "bDesconnection";
            this.bDesconnection.Size = new System.Drawing.Size(100, 23);
            this.bDesconnection.TabIndex = 5;
            this.bDesconnection.Text = "Desconectar";
            this.bDesconnection.UseVisualStyleBackColor = true;
            this.bDesconnection.Click += new System.EventHandler(this.bDesconnection_Click);
            // 
            // bConnection
            // 
            this.bConnection.Location = new System.Drawing.Point(21, 122);
            this.bConnection.Name = "bConnection";
            this.bConnection.Size = new System.Drawing.Size(100, 23);
            this.bConnection.TabIndex = 4;
            this.bConnection.Text = "Conectar";
            this.bConnection.UseVisualStyleBackColor = true;
            this.bConnection.Click += new System.EventHandler(this.bConnection_Click);
            // 
            // lServer
            // 
            this.lServer.AutoSize = true;
            this.lServer.Location = new System.Drawing.Point(18, 24);
            this.lServer.Name = "lServer";
            this.lServer.Size = new System.Drawing.Size(92, 13);
            this.lServer.TabIndex = 2;
            this.lServer.Text = "Nome do Servidor";
            // 
            // lDataBase
            // 
            this.lDataBase.AutoSize = true;
            this.lDataBase.Location = new System.Drawing.Point(18, 64);
            this.lDataBase.Name = "lDataBase";
            this.lDataBase.Size = new System.Drawing.Size(133, 13);
            this.lDataBase.TabIndex = 1;
            this.lDataBase.Text = "Nome do Banco de Dados";
            // 
            // tbDataBase
            // 
            this.tbDataBase.Location = new System.Drawing.Point(21, 80);
            this.tbDataBase.Name = "tbDataBase";
            this.tbDataBase.Size = new System.Drawing.Size(227, 20);
            this.tbDataBase.TabIndex = 0;
            this.tbDataBase.Text = "NovoGeoPerdas";
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.bReportFile);
            this.tabPage4.Controls.Add(this.bExecuteFile);
            this.tabPage4.Controls.Add(this.bSelectPathFile);
            this.tabPage4.Controls.Add(this.tbPathFile);
            this.tabPage4.Controls.Add(this.lPathFile);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(576, 330);
            this.tabPage4.TabIndex = 2;
            this.tabPage4.Text = "Execução via Arquivo";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // bReportFile
            // 
            this.bReportFile.Location = new System.Drawing.Point(90, 73);
            this.bReportFile.Name = "bReportFile";
            this.bReportFile.Size = new System.Drawing.Size(75, 23);
            this.bReportFile.TabIndex = 4;
            this.bReportFile.Text = "Relatório";
            this.bReportFile.UseVisualStyleBackColor = true;
            this.bReportFile.Click += new System.EventHandler(this.bReportFile_Click);
            // 
            // bExecuteFile
            // 
            this.bExecuteFile.Location = new System.Drawing.Point(9, 73);
            this.bExecuteFile.Name = "bExecuteFile";
            this.bExecuteFile.Size = new System.Drawing.Size(75, 23);
            this.bExecuteFile.TabIndex = 3;
            this.bExecuteFile.Text = "Executa";
            this.bExecuteFile.UseVisualStyleBackColor = true;
            this.bExecuteFile.Click += new System.EventHandler(this.bExecuteFile_Click);
            // 
            // bSelectPathFile
            // 
            this.bSelectPathFile.Location = new System.Drawing.Point(264, 32);
            this.bSelectPathFile.Name = "bSelectPathFile";
            this.bSelectPathFile.Size = new System.Drawing.Size(75, 23);
            this.bSelectPathFile.TabIndex = 2;
            this.bSelectPathFile.Text = "Arquivo";
            this.bSelectPathFile.UseVisualStyleBackColor = true;
            this.bSelectPathFile.Click += new System.EventHandler(this.bSelectPathFile_Click);
            // 
            // tbPathFile
            // 
            this.tbPathFile.Location = new System.Drawing.Point(9, 35);
            this.tbPathFile.Name = "tbPathFile";
            this.tbPathFile.Size = new System.Drawing.Size(249, 20);
            this.tbPathFile.TabIndex = 1;
            // 
            // lPathFile
            // 
            this.lPathFile.AutoSize = true;
            this.lPathFile.Location = new System.Drawing.Point(6, 19);
            this.lPathFile.Name = "lPathFile";
            this.lPathFile.Size = new System.Drawing.Size(94, 13);
            this.lPathFile.TabIndex = 0;
            this.lPathFile.Text = "Diretório e Arquivo";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.rtbInfo);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(576, 330);
            this.tabPage2.TabIndex = 3;
            this.tabPage2.Text = "Informações";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // rtbInfo
            // 
            this.rtbInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbInfo.Location = new System.Drawing.Point(3, 3);
            this.rtbInfo.Name = "rtbInfo";
            this.rtbInfo.ReadOnly = true;
            this.rtbInfo.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.rtbInfo.Size = new System.Drawing.Size(570, 324);
            this.rtbInfo.TabIndex = 0;
            this.rtbInfo.Text = "";
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.tbCommand);
            this.tabPage5.Controls.Add(this.rtbAdmin);
            this.tabPage5.Location = new System.Drawing.Point(4, 22);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage5.Size = new System.Drawing.Size(576, 330);
            this.tabPage5.TabIndex = 4;
            this.tabPage5.Text = "Administração";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // tbCommand
            // 
            this.tbCommand.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tbCommand.Location = new System.Drawing.Point(3, 307);
            this.tbCommand.Name = "tbCommand";
            this.tbCommand.Size = new System.Drawing.Size(570, 20);
            this.tbCommand.TabIndex = 1;
            this.tbCommand.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbCommand_KeyDown);
            // 
            // rtbAdmin
            // 
            this.rtbAdmin.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbAdmin.Location = new System.Drawing.Point(3, 3);
            this.rtbAdmin.Name = "rtbAdmin";
            this.rtbAdmin.ReadOnly = true;
            this.rtbAdmin.Size = new System.Drawing.Size(570, 324);
            this.rtbAdmin.TabIndex = 0;
            this.rtbAdmin.Text = "";
            // 
            // rtbLog
            // 
            this.rtbLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbLog.Location = new System.Drawing.Point(0, 356);
            this.rtbLog.Name = "rtbLog";
            this.rtbLog.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.rtbLog.Size = new System.Drawing.Size(584, 365);
            this.rtbLog.TabIndex = 1;
            this.rtbLog.Text = "";
            // 
            // ofdSelectPathFileDSS
            // 
            this.ofdSelectPathFileDSS.Filter = "Arquivos DSS (*.dss)|*.dss";
            // 
            // frmPrincipal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(584, 721);
            this.Controls.Add(this.rtbLog);
            this.Controls.Add(this.tabControl1);
            this.Name = "frmPrincipal";
            this.Text = "ProgGeoPerdas - Apuração de Perdas Técnicas ";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResizeBegin += new System.EventHandler(this.Form1_ResizeBegin);
            this.ResizeEnd += new System.EventHandler(this.Form1_ResizeEnd);
            this.Resize += new System.EventHandler(this.Form1_Resize);
            this.tabControl1.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage4.ResumeLayout(false);
            this.tabPage4.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage5.ResumeLayout(false);
            this.tabPage5.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Label lServer;
        private System.Windows.Forms.Label lDataBase;
        private System.Windows.Forms.TextBox tbDataBase;
        private System.Windows.Forms.Button bExecuteBD;
        private System.Windows.Forms.CheckedListBox clbOptionsRun;
        private System.Windows.Forms.TextBox tbObjectFeeder;
        private System.Windows.Forms.Label lObjectFeeder;
        private System.Windows.Forms.TextBox tbObjectBase;
        private System.Windows.Forms.Label lObjetcBase;
        private System.Windows.Forms.Button bConnection;
        private System.Windows.Forms.RichTextBox rtbLog;
        private System.Windows.Forms.Button bDesconnection;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.Button bSelectPathFile;
        private System.Windows.Forms.TextBox tbPathFile;
        private System.Windows.Forms.Label lPathFile;
        private System.Windows.Forms.OpenFileDialog ofdSelectPathFileDSS;
        private System.Windows.Forms.Button bExecuteFile;
        private System.Windows.Forms.Button bExportDB;
        private System.Windows.Forms.FolderBrowserDialog fbdSelectPathWrite;
        private System.Windows.Forms.Button bReportFile;
        private System.Windows.Forms.Button bTrafosSubDB;
        private System.Windows.Forms.Button bMedidoresDB;
        private System.Windows.Forms.Button bReportAnalisys;
        private System.Windows.Forms.Button bReportSGT;
        private System.Windows.Forms.Button bReportVoltageComplete;
        private System.Windows.Forms.Button bReportVoltageSimple;
        private System.Windows.Forms.Button bBalance;
        private System.Windows.Forms.CheckBox cbReinicializeNT;
        private System.Windows.Forms.Label lVPUMin;
        private System.Windows.Forms.TextBox tbVPUMin;
        private System.Windows.Forms.CheckBox cbMeterComplete;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.RichTextBox rtbInfo;
        private System.Windows.Forms.ComboBox cbModel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.RichTextBox rtbAdmin;
        private System.Windows.Forms.TextBox tbCommand;
        private System.Windows.Forms.ComboBox cbServer;
        private System.Windows.Forms.Label lbPswd;
        private System.Windows.Forms.TextBox txtPswd;
        private System.Windows.Forms.Label lbUser;
        private System.Windows.Forms.TextBox txtUser;
    }
}

