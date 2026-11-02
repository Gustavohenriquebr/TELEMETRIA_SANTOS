using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

// Biblioteca de manipulação de diretórios
using System.IO;

// Biblioteca de conexão com SQL Server
using System.Data.SqlClient;

// Biblioteca de conexão com OpenDSS
using OpenDSSengine;


// Biblioteca de conexão com Excel do Office
using Excel = Microsoft.Office.Interop.Excel;
using System.Reflection; 

namespace PGPerdas
{

    public partial class frmPrincipal : Form
    {

        delegate void mudaTextFormCallback(string text, Form objRef);

        private string strCommandConnection;
        private SqlConnection sqlConnServer;
        private SqlCommand sqlCommServer;
        private SqlDataReader sqlDtRdrServer;

        private String strBaseSelected;
        private ArrayList strFeedersSelected = new ArrayList();

        private System.IO.StreamWriter swFile;
        //private long lngindex = 0;
        private String[] strFileData = new String[1000000];

        private DSS dssServer;
        private Text dssText;
        private Circuit dssCircuit;
        private Solution dssSolution;
        private CtrlQueue dssControlQueue;
        private int intConverged = 0;

        private int intRealizaCnvrgcPNT;
        private int intUsaTrafoABNT;
        private int intAdequarTensaoCargasMT;
        private int intAdequarTensaoCargasBT;
        private int intAdequarTensaoSuperior;
        private int intAdequarRamal;
        private int intAdequarModeloCarga;
        private int intAdequarTapTrafo;
        private int intAdequarPotenciaCarga;
        private int intAdequarTrafoVazio;
        private int intNeutralizarTrafoTerceiros;
        private int intNeutralizarRedeTerceiros;

        private double dblVPUMin;

        private int intModeloConverge;

        private double dblEnergyInjectDeclared_MWh;
        private double dblEnergyLoadsMTDeclared_MWh;
        private double dblEnergyLoadsBTDeclared_MWh;
        private double dblEnergyLoadsMTNTCalculated_MWh;
        private double dblEnergyLoadsBTNTCalculated_MWh;
        private double dblPropEnergyLoadsMT_pu;
        private double dblPropEnergyLoadsBT_pu;

        private double dblEnergyInjectDeclaredYearly_MWh;
        private double dblEnergyInjectCalculatedYearly_MWh;

        private double dblTotalPower_kW;
        private double dblTotalEnergy_kWh;
        private double dblTecnicalLossPower_kW;
        private double dblTecnicalLossEnergy_kWh;
        private double dblTotalLoadEnergy_kWh;

        private double dblNoLoadLosses_kWh;
        private double dblLineLosses_kWh;

        private double dblTransformerLosses_kWh;
        private double dblA3aA3aTransformerLosses_kWh;
        private double dblA3aA4TransformerLosses_kWh;
        private double dblA4A3aTransformerLosses_kWh;
        private double dblA3aBTransformerLosses_kWh;
        private double dblBA3aTransformerLosses_kWh;
        private double dblA4A4TransformerLosses_kWh;
        private double dblA4BTransformerLosses_kWh;
        private double dblBA4TransformerLosses_kWh;
        private double dblA3aA3aTransformerNoLoadLosses_kWh;
        private double dblA3aA4TransformerNoLoadLosses_kWh;
        private double dblA4A3aTransformerNoLoadLosses_kWh;
        private double dblA3aBTransformerNoLoadLosses_kWh;
        private double dblBA3aTransformerNoLoadLosses_kWh;
        private double dblA4A4TransformerNoLoadLosses_kWh;
        private double dblA4BTransformerNoLoadLosses_kWh;
        private double dblBA4TransformerNoLoadLosses_kWh;

        private double dblMTLosses_kWh;
        private double dblA3aLosses_kWh;
        private double dblA4Losses_kWh;
        private double dblBTLosses_kWh;

        private double dblLineMTLosses_kWh;
        private double dblLineA3aLosses_kWh;
        private double dblLineA4Losses_kWh;
        private double dblLineBTLosses_kWh;

        private double dblMTLoad_kWh;
        private double dblBTLoad_kWh;

        public frmPrincipal()
        {
            InitializeComponent();
            mudaTitApp();
            defInfoVersoes();

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            mthdStarDSSServer();
            cbModel.SelectedIndex = 0;
            for (int i = 0; i < clbOptionsRun.Items.Count; i++)
            {
                clbOptionsRun.SetItemChecked(i, true);
            }  
            /*
            this.mthdInputMensageLog(GeoPerdasTools.mthdGetDSSCommandCircuit("a", 13.8, 1.2, "1"));
            this.mthdInputMensageLog(this.mthdGetDSSCommandLinecode("b", 1.2, 3.2, 5.6));
            this.mthdInputMensageLog(this.mthdGetDSSCommandMTSwitch("c", "ABC", "1", "2"));
            this.mthdInputMensageLog(this.mthdGetDSSCommandLine("d", "ABC", "2", "3", "b", 0.5));
            this.mthdInputMensageLog(this.mthdGetDSSCommandRegulatorMT("e", 1, "A", "A", "3", "4", 13.8, 5, 1.05, 0.2, 0.35, 0.45));
            this.mthdInputMensageLog(this.mthdGetDSSCommandRegulatorMT("e", 2, "B", "B", "3", "4", 13.8, 5, 1.05, 0.2, 0.35, 0.45));
            this.mthdInputMensageLog(this.mthdGetDSSCommandRegulatorMT("e", 3, "C", "C", "3", "4", 13.8, 5, 1.05, 0.2, 0.35, 0.45));
            this.mthdInputMensageLog(this.mthdGetDSSCommandLoadshape("f_DU", 1.01, 1.02, 1.03, 1.04, 1.05, 1.06, 1.07, 1.08, 1.09, 1.10, 1.11, 1.12, 1.13, 1.14, 1.15, 1.16, 1.17, 1.18, 1.19, 1.20, 1.21, 1.22, 1.23, 1.24));
            this.mthdInputMensageLog(this.mthdGetDSSCommandLoadshape("f_SA", 1.01, 1.02, 1.03, 1.04, 1.05, 1.06, 1.07, 1.08, 1.09, 1.10, 1.11, 1.12, 1.13, 1.14, 1.15, 1.16, 1.17, 1.18, 1.19, 1.20, 1.21, 1.22, 1.23, 1.24));
            this.mthdInputMensageLog(this.mthdGetDSSCommandLoadshape("f_DO", 1.01, 1.02, 1.03, 1.04, 1.05, 1.06, 1.07, 1.08, 1.09, 1.10, 1.11, 1.12, 1.13, 1.14, 1.15, 1.16, 1.17, 1.18, 1.19, 1.20, 1.21, 1.22, 1.23, 1.24));
            this.mthdInputMensageLog(this.mthdGetDSSCommandLine("g", "ABC", "4", "5", "b", 0.5));
            this.mthdInputMensageLog(this.mthdGetDSSCommandNewLoad("h", "ABC", "5", 13.8, 10, "f_DU", 1, 1));
            this.mthdInputMensageLog(this.mthdGetDSSCommandTransformer("i", 0, "ABC", "ABC", "XX", "5", "6", 13.8, 0.38, 112.5, 1.45, 0.45));
            this.mthdInputMensageLog(this.mthdGetDSSCommandLine("j", "ABC", "6", "7", "b", 0.5));
            this.mthdInputMensageLog(this.mthdGetDSSCommandLine("k", "ABC", "7", "8", "b", 0.5));
            this.mthdInputMensageLog(this.mthdGetDSSCommandNewLoad("l", "ABC", "8", 0.38, 0.5, "f_DU", 2, 1));
            this.mthdInputMensageLog(this.mthdGetDSSCommandEditLoad("l", 0.75, "f_DU"));
            this.mthdInputMensageLog(GeoPerdasTools.mthdGetNumberDaysDUSADOMonth(2, 2015, "DU").ToString());
            this.mthdInputMensageLog(GeoPerdasTools.mthdGetNumberDaysDUSADOMonth(2, 2015, "SA").ToString());
            this.mthdInputMensageLog(GeoPerdasTools.mthdGetNumberDaysDUSADOMonth(2, 2015, "DO").ToString());
            this.mthdInputMensageLog(GeoPerdasTools.mthdTransformerTotalPowerLoss_per("A", 50, 13.8).ToString());
            this.mthdInputMensageLog(GeoPerdasTools.mthdTransformerNoLoadPowerLoss_per("A", 50, 13.8).ToString());
            this.mthdInputMensageLog(GeoPerdasTools.mthdTransformerTotalPowerLoss_per("A", 5, 20.8).ToString());
            this.mthdInputMensageLog(GeoPerdasTools.mthdTransformerNoLoadPowerLoss_per("A", 5, 20.8).ToString());
            this.mthdInputMensageLog(GeoPerdasTools.mthdTransformerTotalPowerLoss_per("A", 12, 30.8).ToString());
            this.mthdInputMensageLog(GeoPerdasTools.mthdTransformerNoLoadPowerLoss_per("A", 12, 30.8).ToString());
            this.mthdInputMensageLog(GeoPerdasTools.mthdTransformerTotalPowerLoss_per("ABC", 200, 14.8).ToString());
            this.mthdInputMensageLog(GeoPerdasTools.mthdTransformerNoLoadPowerLoss_per("ABC", 200, 14.8).ToString());
            this.mthdInputMensageLog(GeoPerdasTools.mthdTransformerTotalPowerLoss_per("ABC", 75, 32.8).ToString());
            this.mthdInputMensageLog(GeoPerdasTools.mthdTransformerNoLoadPowerLoss_per("ABC", 75, 32.8).ToString());
            this.mthdInputMensageLog(GeoPerdasTools.mthdQuerySQLFeeders("ACA01C1 ;ACA0 1C3; ACA01C1"));
            */
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.mthdCloseDataBase();
        }

        private void bConnection_Click(object sender, EventArgs e)
        {
            Conectar();
        }

        private bool Conectar()
        {
            bool result = false;
            if (this.cbServer.Text == "" || this.cbServer.Text == null)
            {
                this.mthdInputMensageLog("O nome do servidor não foi estabelecido.");
            }
            else if (this.tbDataBase.Text == "" || this.tbDataBase.Text == null)
            {
                this.mthdInputMensageLog("O nome da base de dados não foi estabelecida.");
            }
            else
            {
                this.mthdCloseDataBase();
                if (mthdOpenDataBase(this.cbServer.Text, this.tbDataBase.Text, this.txtUser.Text, this.txtPswd.Text))
                    result = true;
            }
            return result;
        }

        private void bDesconnection_Click(object sender, EventArgs e)
        {
            this.mthdCloseDataBase();
        }

        private void bExecuteBD_Click(object sender, EventArgs e)
        {
            int i = 0, intCtrlCnvrg = 0, intQuantIntrtn = 0;
            String strTempSqlCommand, strTensoesBase;
            int intAno = 0;
            double tolerance;
            double dblEnerInjDU_kWh = 0, dblEnerInjSA_kWh = 0, dblEnerInjDO_kWh = 0;
            double dblPerdaEnerTecDU_kWh = 0, dblPerdaEnerTecSA_kWh = 0, dblPerdaEnerTecDO_kWh = 0;
            double dblEnerForncDU_kWh = 0, dblEnerForncSA_kWh = 0, dblEnerForncDO_kWh = 0;
            double dblDelta = 0, dblDeltaMT = 0, dblDeltaBT = 0, dblDelta_MWh = 0, dblDeltaBT_MWh = 0, dblDeltaMT_MWh = 0, dblErrorEnerInj = 0;

            if (checkBase())
            {
                try
                {

                    // Verifica as opções solicitadas pelo usuário
                    this.mthdUpdateOptionsRun();

                    // Captura a base a ser calculada
                    this.strBaseSelected = this.tbObjectBase.Text;

                    // Captura o ano da base
                    this.sqlCommServer = new SqlCommand("SELECT t1.* FROM dbo.[" + this.strBaseSelected + "Base] AS t1 WHERE t1.CodBase = " + this.strBaseSelected, this.sqlConnServer);
                    this.sqlCommServer.CommandTimeout = 0;
                    this.sqlDtRdrServer = this.sqlCommServer.ExecuteReader();
                    this.sqlDtRdrServer.Read();
                    intAno = Convert.ToInt32(this.sqlDtRdrServer["Ano"]);
                    this.sqlDtRdrServer.Close();

                    // Loop da base e dos alimentadores
                    this.strFeedersSelected.Clear();
                    if (this.tbObjectFeeder.Text == null || this.tbObjectFeeder.Text == "")
                        this.sqlCommServer = new SqlCommand("SELECT t1.* FROM dbo.[" + this.strBaseSelected + "CircMT] AS t1 WHERE t1.CodBase = " + this.strBaseSelected + " AND t1.CircAtipResultante = 0", this.sqlConnServer);
                    else
                        this.sqlCommServer = new SqlCommand("SELECT t1.* FROM dbo.[" + this.strBaseSelected + "CircMT] AS t1 WHERE t1.CodBase = " + this.strBaseSelected + " AND " + GeoPerdasTools.mthdQuerySQLFeeders(this.tbObjectFeeder.Text) + " AND t1.CircAtipResultante = 0", this.sqlConnServer);
                    this.sqlCommServer.CommandTimeout = 0;
                    this.sqlDtRdrServer = this.sqlCommServer.ExecuteReader();
                    while (this.sqlDtRdrServer.Read())
                    {
                        this.strFeedersSelected.Add(this.sqlDtRdrServer["CodAlim"].ToString());
                    }
                    this.sqlDtRdrServer.Close();

                    foreach (String strFeeder in strFeedersSelected)
                    {

                        try
                        {

                            if (!this.mthdDataBaseOpened()) { Conectar(); }

                            // Modelo de convergência
                            // Modelo 1 corresponde ao modelo de convergência primário
                            // Modelo 2 corresponde ao modelo de convergência secundário
                            this.intModeloConverge = 1;

                            // Variáveis de avaliação de troca de modelo
                            this.dblEnergyInjectDeclaredYearly_MWh = 0;
                            this.dblEnergyInjectCalculatedYearly_MWh = 0;

                            // Início do processo
                            this.mthdInputMensageLog("Início do processo para o alimentador " + strFeeder + ".");

                            this.mthdStarCommands();

                            // Zera as cargas não técnicas calculadas anteriormente para reiniciar o processo
                            if (this.cbReinicializeNT.Checked == true)
                            {
                                this.sqlCommServer = new SqlCommand("dbo.iadAuxCargaBTNTDem", this.sqlConnServer);
                                this.sqlCommServer.CommandType = CommandType.StoredProcedure;
                                this.sqlCommServer.Parameters.AddWithValue("@CodBase", this.strBaseSelected);
                                this.sqlCommServer.Parameters.AddWithValue("@CodOperation", 1);
                                this.sqlCommServer.Parameters.AddWithValue("@CodAlim", strFeeder);
                                this.sqlCommServer.ExecuteNonQuery();

                                this.sqlCommServer = new SqlCommand("dbo.iadAuxCargaMTNTDem", this.sqlConnServer);
                                this.sqlCommServer.CommandType = CommandType.StoredProcedure;
                                this.sqlCommServer.Parameters.AddWithValue("@CodBase", this.strBaseSelected);
                                this.sqlCommServer.Parameters.AddWithValue("@CodOperation", 1);
                                this.sqlCommServer.Parameters.AddWithValue("@CodAlim", strFeeder);
                                this.sqlCommServer.ExecuteNonQuery();
                            }

                            // Cria estrutura de tabelas para convergência não técnica
                            if (this.intRealizaCnvrgcPNT == 1)
                            {
                                this.sqlCommServer = new SqlCommand("dbo.CriaTabelaDemandaCargaNTAlimentador", this.sqlConnServer);
                                this.sqlCommServer.CommandType = CommandType.StoredProcedure;
                                this.sqlCommServer.Parameters.AddWithValue("@BaseAnalisada", this.strBaseSelected);
                                this.sqlCommServer.Parameters.AddWithValue("@AlimAnalisado", strFeeder);
                                this.sqlCommServer.ExecuteNonQuery();
                            }

                            // Início do código
                            this.mthdInsertCommandInLine("Clear");

                            // Loop dos códigos dos circuitos
                            this.sqlCommServer = new SqlCommand("SELECT t1.* FROM dbo.[" + this.strBaseSelected + "CircMT] AS t1 WHERE t1.CodBase = " + this.strBaseSelected + " AND t1.CodAlim = '" + strFeeder + "'", this.sqlConnServer);
                            this.sqlCommServer.CommandTimeout = 0;
                            this.mthdInsertCommandInLine("! Criação da seção de circuitos");
                            this.sqlDtRdrServer = this.sqlCommServer.ExecuteReader();
                            while (this.sqlDtRdrServer.Read())
                            {
                                this.mthdGetDSSCommandCircuit(strFeeder, Convert.ToDouble(this.sqlDtRdrServer["TenNom_kV"]), Convert.ToDouble(this.sqlDtRdrServer["TenOpe_pu"]), this.sqlDtRdrServer["CodPonAcopl"].ToString(), this.intAdequarTensaoSuperior);
                            }
                            this.sqlDtRdrServer.Close();
                            this.mthdInputMensageLog("Passou pelo circuito.");

                            // Loop dos códigos dos condutores
                            this.sqlCommServer = new SqlCommand("SELECT t1.* FROM dbo.[" + this.strBaseSelected + "CodCondutor] AS t1 WHERE t1.CodBase = " + this.strBaseSelected, this.sqlConnServer);
                            this.sqlCommServer.CommandTimeout = 0;
                            this.mthdInsertCommandInLine("! Criação da seção de condutores");
                            this.sqlDtRdrServer = this.sqlCommServer.ExecuteReader();
                            while (this.sqlDtRdrServer.Read())
                            {
                                this.mthdGetDSSCommandLinecode(this.sqlDtRdrServer["CodCond"].ToString(), Convert.ToDouble(this.sqlDtRdrServer["Resis_ohms_km"]), Convert.ToDouble(this.sqlDtRdrServer["Reat_ohms_km"]), Convert.ToDouble(this.sqlDtRdrServer["CorrMax_A"]));
                            }
                            this.sqlDtRdrServer.Close();
                            this.mthdInputMensageLog("Passou pelos condutores.");

                            // Loop dos códigos das curvas MT
                            this.sqlCommServer = new SqlCommand("SELECT t1.* FROM dbo.AuxCrvCrgMTHor AS t1 WHERE t1.CodBase = " + this.strBaseSelected, this.sqlConnServer);
                            this.sqlCommServer.CommandTimeout = 0;
                            this.mthdInsertCommandInLine("! Criação da seção de curvas MT");
                            this.sqlDtRdrServer = this.sqlCommServer.ExecuteReader();
                            while (this.sqlDtRdrServer.Read())
                            {
                                this.mthdGetDSSCommandLoadshape(this.sqlDtRdrServer["CodCrvCrg"].ToString() + "_" + this.sqlDtRdrServer["TipoDia"].ToString(), Convert.ToDouble(this.sqlDtRdrServer["PotAtvNorm01"]), Convert.ToDouble(this.sqlDtRdrServer["PotAtvNorm02"]), Convert.ToDouble(this.sqlDtRdrServer["PotAtvNorm03"]), Convert.ToDouble(this.sqlDtRdrServer["PotAtvNorm04"]), Convert.ToDouble(this.sqlDtRdrServer["PotAtvNorm05"]), Convert.ToDouble(this.sqlDtRdrServer["PotAtvNorm06"]), Convert.ToDouble(this.sqlDtRdrServer["PotAtvNorm07"]), Convert.ToDouble(this.sqlDtRdrServer["PotAtvNorm08"]), Convert.ToDouble(this.sqlDtRdrServer["PotAtvNorm09"]), Convert.ToDouble(this.sqlDtRdrServer["PotAtvNorm10"]), Convert.ToDouble(this.sqlDtRdrServer["PotAtvNorm11"]), Convert.ToDouble(this.sqlDtRdrServer["PotAtvNorm12"]), Convert.ToDouble(this.sqlDtRdrServer["PotAtvNorm13"]), Convert.ToDouble(this.sqlDtRdrServer["PotAtvNorm14"]), Convert.ToDouble(this.sqlDtRdrServer["PotAtvNorm15"]), Convert.ToDouble(this.sqlDtRdrServer["PotAtvNorm16"]), Convert.ToDouble(this.sqlDtRdrServer["PotAtvNorm17"]), Convert.ToDouble(this.sqlDtRdrServer["PotAtvNorm18"]), Convert.ToDouble(this.sqlDtRdrServer["PotAtvNorm19"]), Convert.ToDouble(this.sqlDtRdrServer["PotAtvNorm20"]), Convert.ToDouble(this.sqlDtRdrServer["PotAtvNorm21"]), Convert.ToDouble(this.sqlDtRdrServer["PotAtvNorm22"]), Convert.ToDouble(this.sqlDtRdrServer["PotAtvNorm23"]), Convert.ToDouble(this.sqlDtRdrServer["PotAtvNorm24"]));
                            }
                            this.sqlDtRdrServer.Close();
                            this.mthdInputMensageLog("Passou pelas curvas MT.");

                            // Loop dos códigos das curvas BT
                            this.sqlCommServer = new SqlCommand("SELECT t1.* FROM dbo.AuxCrvCrgBTHor AS t1 WHERE t1.CodBase = " + this.strBaseSelected, this.sqlConnServer);
                            this.sqlCommServer.CommandTimeout = 0;
                            this.mthdInsertCommandInLine("! Criação da seção de curvas BT");
                            this.sqlDtRdrServer = this.sqlCommServer.ExecuteReader();
                            while (this.sqlDtRdrServer.Read())
                            {
                                this.mthdGetDSSCommandLoadshape(this.sqlDtRdrServer["CodCrvCrg"].ToString() + "_" + this.sqlDtRdrServer["TipoDia"].ToString(), Convert.ToDouble(this.sqlDtRdrServer["PotAtvNorm01"]), Convert.ToDouble(this.sqlDtRdrServer["PotAtvNorm02"]), Convert.ToDouble(this.sqlDtRdrServer["PotAtvNorm03"]), Convert.ToDouble(this.sqlDtRdrServer["PotAtvNorm04"]), Convert.ToDouble(this.sqlDtRdrServer["PotAtvNorm05"]), Convert.ToDouble(this.sqlDtRdrServer["PotAtvNorm06"]), Convert.ToDouble(this.sqlDtRdrServer["PotAtvNorm07"]), Convert.ToDouble(this.sqlDtRdrServer["PotAtvNorm08"]), Convert.ToDouble(this.sqlDtRdrServer["PotAtvNorm09"]), Convert.ToDouble(this.sqlDtRdrServer["PotAtvNorm10"]), Convert.ToDouble(this.sqlDtRdrServer["PotAtvNorm11"]), Convert.ToDouble(this.sqlDtRdrServer["PotAtvNorm12"]), Convert.ToDouble(this.sqlDtRdrServer["PotAtvNorm13"]), Convert.ToDouble(this.sqlDtRdrServer["PotAtvNorm14"]), Convert.ToDouble(this.sqlDtRdrServer["PotAtvNorm15"]), Convert.ToDouble(this.sqlDtRdrServer["PotAtvNorm16"]), Convert.ToDouble(this.sqlDtRdrServer["PotAtvNorm17"]), Convert.ToDouble(this.sqlDtRdrServer["PotAtvNorm18"]), Convert.ToDouble(this.sqlDtRdrServer["PotAtvNorm19"]), Convert.ToDouble(this.sqlDtRdrServer["PotAtvNorm20"]), Convert.ToDouble(this.sqlDtRdrServer["PotAtvNorm21"]), Convert.ToDouble(this.sqlDtRdrServer["PotAtvNorm22"]), Convert.ToDouble(this.sqlDtRdrServer["PotAtvNorm23"]), Convert.ToDouble(this.sqlDtRdrServer["PotAtvNorm24"]));
                            }
                            this.sqlDtRdrServer.Close();
                            this.mthdInputMensageLog("Passou pelas curvas BT.");

                            // Loop das chaves MT
                            this.sqlCommServer = new SqlCommand("SELECT t1.CodChvMT, t1.CodFas, t1.De, t1.Para FROM dbo.[" + this.strBaseSelected + "ChaveMT] AS t1 WHERE t1.CodBase = " + this.strBaseSelected + " AND t1.CodAlimAtrib = '" + strFeeder + "' AND t1.EstChv = 2", this.sqlConnServer);
                            this.sqlCommServer.CommandTimeout = 0;
                            this.mthdInsertCommandInLine("! Criação da seção de chaves MT");
                            this.sqlDtRdrServer = this.sqlCommServer.ExecuteReader();
                            while (this.sqlDtRdrServer.Read())
                            {
                                this.mthdGetDSSCommandMTSwitch("CMT_" + this.sqlDtRdrServer["CodChvMT"].ToString(), this.sqlDtRdrServer["CodFas"].ToString(), this.sqlDtRdrServer["De"].ToString(), this.sqlDtRdrServer["Para"].ToString());
                            }
                            this.sqlDtRdrServer.Close();
                            this.mthdInputMensageLog("Passou pelas chaves MT.");

                            // Loop dos segmentos MT
                            this.sqlCommServer = new SqlCommand("SELECT t1.CodSegmMT, t1.CodFas, t1.CodCond, t1.Comp_km, t1.De, t1.Para, t1.Propr FROM dbo.[" + this.strBaseSelected + "SegmentoMT] AS t1 WHERE t1.CodBase = " + this.strBaseSelected + " AND t1.CodAlimAtrib = '" + strFeeder + "'", this.sqlConnServer);
                            this.sqlCommServer.CommandTimeout = 0;
                            this.mthdInsertCommandInLine("! Criação da seção de segmentos MT");
                            this.sqlDtRdrServer = this.sqlCommServer.ExecuteReader();
                            while (this.sqlDtRdrServer.Read())
                            {
                                this.mthdGetDSSCommandLineMT("SMT_" + this.sqlDtRdrServer["CodSegmMT"].ToString(), this.sqlDtRdrServer["CodFas"].ToString(), this.sqlDtRdrServer["De"].ToString(), this.sqlDtRdrServer["Para"].ToString(), this.sqlDtRdrServer["CodCond"].ToString(), Convert.ToDouble(this.sqlDtRdrServer["Comp_km"]), this.sqlDtRdrServer["Propr"].ToString(), 0, intNeutralizarRedeTerceiros);
                            }
                            this.sqlDtRdrServer.Close();
                            this.mthdInputMensageLog("Passou pelos segmentos MT.");

                            // Loop dos reguladores MT
                            this.sqlCommServer = new SqlCommand("SELECT t1.CodRegulMT, t1.CodBnc, t1.TipRegul, t1.CodFasPrim, t1.CodFasSecu, t1.PotNom_kVA, t1.TenRgl_pu, t1.[ReatHL_%], t1.PerdTtl_W, t1.PerdVz_W, t1.TnsLnh1_kV, t1.De, t1.Para, t1.CodFasCoinc FROM dbo.[" + this.strBaseSelected + "ReguladorMT] AS t1 WHERE t1.CodBase = " + this.strBaseSelected + " AND t1.CodAlimAtrib = '" + strFeeder + "'", this.sqlConnServer);
                            this.sqlCommServer.CommandTimeout = 0;
                            this.mthdInsertCommandInLine("! Criação da seção de reguladores MT");
                            this.sqlDtRdrServer = this.sqlCommServer.ExecuteReader();
                            while (this.sqlDtRdrServer.Read())
                            {
                                this.mthdGetDSSCommandRegulatorMT("REG_" + this.sqlDtRdrServer["CodRegulMT"].ToString(), Convert.ToInt32(this.sqlDtRdrServer["CodBnc"]), Convert.ToInt32(this.sqlDtRdrServer["TipRegul"]), this.sqlDtRdrServer["CodFasPrim"].ToString(), this.sqlDtRdrServer["CodFasSecu"].ToString(), this.sqlDtRdrServer["CodFasCoinc"].ToString(), this.sqlDtRdrServer["De"].ToString(), this.sqlDtRdrServer["Para"].ToString(), Convert.ToDouble(this.sqlDtRdrServer["TnsLnh1_kV"]), Convert.ToDouble(this.sqlDtRdrServer["PotNom_kVA"]), Convert.ToDouble(this.sqlDtRdrServer["TenRgl_pu"]), Convert.ToDouble(this.sqlDtRdrServer["ReatHL_%"]), (100 * Convert.ToDouble(this.sqlDtRdrServer["PerdTtl_W"])) / (1000 * Convert.ToDouble(this.sqlDtRdrServer["PotNom_kVA"])), (100 * Convert.ToDouble(this.sqlDtRdrServer["PerdVz_W"])) / (1000 * Convert.ToDouble(this.sqlDtRdrServer["PotNom_kVA"])));
                            }
                            this.sqlDtRdrServer.Close();
                            this.mthdInputMensageLog("Passou pelos reguladores MT.");

                            // Loop dos transformadores MT-MT ou MT-BT
                            this.sqlCommServer = new SqlCommand("SELECT t1.* FROM dbo.[" + this.strBaseSelected + "TrafoMTMTMTBT] AS t1 WHERE t1.CodBase = " + this.strBaseSelected + " AND t1.CodAlimAtrib = '" + strFeeder + "'", this.sqlConnServer);
                            this.sqlCommServer.CommandTimeout = 0;
                            this.mthdInsertCommandInLine("! Criação da seção de transformadores MT-MT ou MT-BT");
                            this.sqlDtRdrServer = this.sqlCommServer.ExecuteReader();
                            while (this.sqlDtRdrServer.Read())
                            {
                                this.mthdGetDSSCommandTransformer4Wires("TRF_" + this.sqlDtRdrServer["CodTrafo"].ToString(), Convert.ToInt32(this.sqlDtRdrServer["CodBnc"]), Convert.ToInt32(this.sqlDtRdrServer["MRT"]), Convert.ToInt32(this.sqlDtRdrServer["TipTrafo"]), this.sqlDtRdrServer["CodFasPrim"].ToString(), this.sqlDtRdrServer["CodFasSecu"].ToString(), this.sqlDtRdrServer["CodFasTerc"].ToString(), this.sqlDtRdrServer["De"].ToString(), this.sqlDtRdrServer["Para"].ToString(), Convert.ToDouble(this.sqlDtRdrServer["TnsLnh1_kV"]), Convert.ToDouble(this.sqlDtRdrServer["TenSecu_kV"]), Convert.ToDouble(this.sqlDtRdrServer["Tap_pu"]), Convert.ToDouble(this.sqlDtRdrServer["PotNom_kVA"]), (GeoPerdasTools.mthdTransformerTotalPowerLoss2019_per(this.sqlDtRdrServer["CodFasPrim"].ToString(), Convert.ToDouble(this.sqlDtRdrServer["PotNom_kVA"]), Convert.ToDouble(this.sqlDtRdrServer["TnsLnh1_kV"])) != 0 && this.intUsaTrafoABNT == 1) ? GeoPerdasTools.mthdTransformerTotalPowerLoss2019_per(this.sqlDtRdrServer["CodFasPrim"].ToString(), Convert.ToDouble(this.sqlDtRdrServer["PotNom_kVA"]), Convert.ToDouble(this.sqlDtRdrServer["TnsLnh1_kV"])) : Convert.ToDouble(this.sqlDtRdrServer["PerdTtl_%"]), (GeoPerdasTools.mthdTransformerNoLoadPowerLoss2019_per(this.sqlDtRdrServer["CodFasPrim"].ToString(), Convert.ToDouble(this.sqlDtRdrServer["PotNom_kVA"]), Convert.ToDouble(this.sqlDtRdrServer["TnsLnh1_kV"])) != 0 && this.intUsaTrafoABNT == 1) ? GeoPerdasTools.mthdTransformerNoLoadPowerLoss2019_per(this.sqlDtRdrServer["CodFasPrim"].ToString(), Convert.ToDouble(this.sqlDtRdrServer["PotNom_kVA"]), Convert.ToDouble(this.sqlDtRdrServer["TnsLnh1_kV"])) : Convert.ToDouble(this.sqlDtRdrServer["PerdVz_%"]), this.sqlDtRdrServer["Propr"].ToString(), (this.intAdequarTrafoVazio == 1) ? Convert.ToInt32(this.sqlDtRdrServer["SemCarga"]) : 0);
                            }
                            this.sqlDtRdrServer.Close();
                            this.mthdInputMensageLog("Passou pelos transformadores MT-MT ou MT-BT.");

                            // Loop das chaves BT
                            this.sqlCommServer = new SqlCommand("SELECT t1.CodChvBT, t1.CodFas, t1.De, t1.Para FROM dbo.[" + this.strBaseSelected + "ChaveBT] AS t1 WHERE t1.CodBase = " + this.strBaseSelected + " AND t1.CodAlimAtrib = '" + strFeeder + "' AND t1.EstChv = 2", this.sqlConnServer);
                            this.sqlCommServer.CommandTimeout = 0;
                            this.mthdInsertCommandInLine("! Criação da seção de chaves BT");
                            this.sqlDtRdrServer = this.sqlCommServer.ExecuteReader();
                            while (this.sqlDtRdrServer.Read())
                            {
                                this.mthdGetDSSCommandBTSwitch("CBT_" + this.sqlDtRdrServer["CodChvBT"].ToString(), this.sqlDtRdrServer["CodFas"].ToString(), this.sqlDtRdrServer["De"].ToString(), this.sqlDtRdrServer["Para"].ToString());
                            }
                            this.sqlDtRdrServer.Close();
                            this.mthdInputMensageLog("Passou pelas chaves BT.");

                            // Loop dos segmentos BT
                            this.sqlCommServer = new SqlCommand("SELECT t1.*, [dbo].[EscolhaPropriedade]([t2].[M1_Propr], [t2].[M2_Propr], [t2].[M3_Propr]) AS [Propr] FROM dbo.[" + this.strBaseSelected + "SegmentoBT] AS t1 LEFT JOIN [dbo].[" + this.strBaseSelected + "AuxTrafoMTMTMTBT] AS t2 ON ([t1].[CodBase] = [t2].[M1_CodBase] AND [t1].[CodTrafoAtrib] = [t2].[M1_CodTrafo]) WHERE t1.CodBase = " + this.strBaseSelected + " AND t1.CodAlimAtrib = '" + strFeeder + "'", this.sqlConnServer);
                            this.sqlCommServer.CommandTimeout = 0;
                            this.mthdInsertCommandInLine("! Criação da seção de segmentos BT");
                            this.sqlDtRdrServer = this.sqlCommServer.ExecuteReader();
                            while (this.sqlDtRdrServer.Read())
                            {
                                this.mthdGetDSSCommandLineBT("SBT_" + this.sqlDtRdrServer["CodSegmBT"].ToString(), this.sqlDtRdrServer["CodFas"].ToString(), this.sqlDtRdrServer["CodPonAcopl1"].ToString(), this.sqlDtRdrServer["CodPonAcopl2"].ToString(), this.sqlDtRdrServer["CodCond"].ToString(), Convert.ToDouble(this.sqlDtRdrServer["Comp_km"]), this.sqlDtRdrServer["Propr"].ToString(), 0, intNeutralizarRedeTerceiros);
                            }
                            this.sqlDtRdrServer.Close();
                            this.mthdInputMensageLog("Passou pelos segmentos BT.");

                            // Loop dos ramais
                            this.sqlCommServer = new SqlCommand("SELECT t1.*, [dbo].[EscolhaPropriedade]([t2].[M1_Propr], [t2].[M2_Propr], [t2].[M3_Propr]) AS [Propr] FROM dbo.[" + this.strBaseSelected + "RamalBT] AS t1 LEFT JOIN [dbo].[" + this.strBaseSelected + "AuxTrafoMTMTMTBT] AS t2 ON ([t1].[CodBase] = [t2].[M1_CodBase] AND [t1].[CodTrafoAtrib] = [t2].[M1_CodTrafo]) WHERE t1.CodBase = " + this.strBaseSelected + " AND t1.CodAlimAtrib = '" + strFeeder + "'", this.sqlConnServer);
                            this.sqlCommServer.CommandTimeout = 0;
                            this.mthdInsertCommandInLine("! Criação da seção de ramais");
                            this.sqlDtRdrServer = this.sqlCommServer.ExecuteReader();
                            while (this.sqlDtRdrServer.Read())
                            {
                                this.mthdGetDSSCommandLineBT("RBT_" + this.sqlDtRdrServer["CodRmlBT"].ToString(), this.sqlDtRdrServer["CodFas"].ToString(), this.sqlDtRdrServer["CodPonAcopl1"].ToString(), this.sqlDtRdrServer["CodPonAcopl2"].ToString(), this.sqlDtRdrServer["CodCond"].ToString(), Convert.ToDouble(this.sqlDtRdrServer["Comp_km"]), this.sqlDtRdrServer["Propr"].ToString(), this.intAdequarRamal, intNeutralizarRedeTerceiros);
                            }
                            this.sqlDtRdrServer.Close();
                            this.mthdInputMensageLog("Passou pelos ramais.");

                            // Versão usando apenas o medidor de barramento
                            if (this.cbMeterComplete.Checked == false)
                            {
                                // Loop dos energymeters de barramento
                                i = 0;
                                this.sqlCommServer = new SqlCommand("SELECT t1.* FROM dbo.AuxMeterBarra AS t1 WHERE t1.CodBase = " + this.strBaseSelected + " AND t1.CodAlim = '" + strFeeder + "'", this.sqlConnServer);
                                this.sqlCommServer.CommandTimeout = 0;
                                this.mthdInsertCommandInLine("! Criação da seção de energymeters de barramento");
                                this.sqlDtRdrServer = this.sqlCommServer.ExecuteReader();
                                while (this.sqlDtRdrServer.Read())
                                {
                                    this.mthdGetDSSCommandEnergymeter(this.sqlDtRdrServer["Nome"].ToString().TrimEnd(), this.mthdNomeElemento(this.sqlDtRdrServer["Elem"].ToString().TrimEnd()) + this.sqlDtRdrServer["CodNomeElem"].ToString().TrimEnd());
                                    i++;
                                }
                                this.sqlDtRdrServer.Close();
                                this.mthdInputMensageLog("Passou pelos energymeters de barramento.");
                            }
                            // Versão usando a tabela auxiliar de medidores
                            if (this.cbMeterComplete.Checked == true)
                            {
                                // Loop dos energymeters usando AuxMeter
                                this.sqlCommServer = new SqlCommand("SELECT t1.* FROM dbo.AuxMeterCompleto AS t1 WHERE t1.CodBase = " + this.strBaseSelected + " AND t1.CodAlim = '" + strFeeder + "'", this.sqlConnServer);
                                this.sqlCommServer.CommandTimeout = 0;
                                this.mthdInsertCommandInLine("! Criação da seção de energymeters");
                                this.sqlDtRdrServer = this.sqlCommServer.ExecuteReader();
                                while (this.sqlDtRdrServer.Read())
                                {
                                    this.mthdGetDSSCommandEnergymeter(this.sqlDtRdrServer["Nome"].ToString().TrimEnd(), this.mthdNomeElemento(this.sqlDtRdrServer["Elem"].ToString().TrimEnd()) + this.sqlDtRdrServer["CodNomeElem"].ToString().TrimEnd());
                                }
                                this.sqlDtRdrServer.Close();
                                this.mthdInputMensageLog("Passou pelos energymeters.");
                            }
                            // Loop das tensões de base
                            strTensoesBase = "Set voltagebases=[";
                            i = 0;
                            this.sqlCommServer = new SqlCommand("SELECT t1.* FROM dbo.AuxVoltageBases AS t1 WHERE t1.CodBase = " + this.strBaseSelected + " AND t1.CodAlim = '" + strFeeder + "'", this.sqlConnServer);
                            this.sqlCommServer.CommandTimeout = 0;
                            this.mthdInsertCommandInLine("! Criação da seção de tensões de base");
                            this.sqlDtRdrServer = this.sqlCommServer.ExecuteReader();
                            while (this.sqlDtRdrServer.Read())
                            {
                                if (i == 0)
                                    strTensoesBase += this.sqlDtRdrServer["TnsLnh_kV"].ToString().Replace(",", ".");
                                else
                                    strTensoesBase += " " + this.sqlDtRdrServer["TnsLnh_kV"].ToString().Replace(",", ".");
                                i++;
                            }
                            this.sqlDtRdrServer.Close();
                            strTensoesBase += "]";
                            this.mthdInsertCommandInLine(strTensoesBase);
                            this.mthdInsertCommandInLine("Calcvoltagebases");
                            this.mthdInputMensageLog("Passou pelas tensões de base.");

                        ModeloConvergenciaSecundaria:

                            if (this.intModeloConverge == 1)
                            {

                                intCtrlCnvrg = 0;

                            }
                            else if (this.intModeloConverge == 2)
                            {

                                // Zera as cargas não técnicas calculadas anteriormente para reiniciar o processo
                                if (this.cbReinicializeNT.Checked == true)
                                {
                                    this.sqlCommServer = new SqlCommand("dbo.iadAuxCargaBTNTDem", this.sqlConnServer);
                                    this.sqlCommServer.CommandType = CommandType.StoredProcedure;
                                    this.sqlCommServer.Parameters.AddWithValue("@CodBase", this.strBaseSelected);
                                    this.sqlCommServer.Parameters.AddWithValue("@CodOperation", 1);
                                    this.sqlCommServer.Parameters.AddWithValue("@CodAlim", strFeeder);
                                    this.sqlCommServer.ExecuteNonQuery();

                                    this.sqlCommServer = new SqlCommand("dbo.iadAuxCargaMTNTDem", this.sqlConnServer);
                                    this.sqlCommServer.CommandType = CommandType.StoredProcedure;
                                    this.sqlCommServer.Parameters.AddWithValue("@CodBase", this.strBaseSelected);
                                    this.sqlCommServer.Parameters.AddWithValue("@CodOperation", 1);
                                    this.sqlCommServer.Parameters.AddWithValue("@CodAlim", strFeeder);
                                    this.sqlCommServer.ExecuteNonQuery();
                                }

                                intCtrlCnvrg = 1;

                                // Variáveis de avaliação de troca de modelo
                                this.dblEnergyInjectDeclaredYearly_MWh = 0;
                                this.dblEnergyInjectCalculatedYearly_MWh = 0;

                            }

                            for (int j = 1; j <= 12; j++)
                            {

                                // Inicia o Loop da convergência não técnica
                                intQuantIntrtn = 0;
                                do
                                {

                                    // Captura a energia injetada declarada pelo agente
                                    this.sqlCommServer = new SqlCommand("SELECT t1.EnerCirc" + String.Format("{0:00}", j) + "_MWh FROM dbo.[" + this.strBaseSelected + "CircMT] AS t1 WHERE t1.CodBase = " + this.strBaseSelected + " AND t1.CodAlim = '" + strFeeder + "'", this.sqlConnServer);
                                    this.sqlCommServer.CommandTimeout = 0;
                                    this.sqlDtRdrServer = this.sqlCommServer.ExecuteReader();
                                    this.sqlDtRdrServer.Read();
                                    this.dblEnergyInjectDeclared_MWh = Convert.ToDouble(this.sqlDtRdrServer["EnerCirc" + String.Format("{0:00}", j) + "_MWh"]);
                                    this.sqlDtRdrServer.Close();
                                    this.mthdInputMensageLog("Passou pelas obtenção da energia injetada declarada.");

                                    // Captura a energia das cargas MT declaradas pelo agente
                                    this.sqlCommServer = new SqlCommand("SELECT Sum(t1.EnerMedid" + String.Format("{0:00}", j) + "_MWh) AS EnerMedid FROM dbo.[" + this.strBaseSelected + "CargaMT] AS t1 WHERE t1.CodBase = " + this.strBaseSelected + " AND t1.CodAlimAtrib = '" + strFeeder + "'", this.sqlConnServer);
                                    this.sqlCommServer.CommandTimeout = 0;
                                    this.sqlDtRdrServer = this.sqlCommServer.ExecuteReader();
                                    this.sqlDtRdrServer.Read();
                                    this.dblEnergyLoadsMTDeclared_MWh = (DBNull.Value.Equals(this.sqlDtRdrServer["EnerMedid"]) ? 0 : Convert.ToDouble(this.sqlDtRdrServer["EnerMedid"]));
                                    this.sqlDtRdrServer.Close();
                                    this.mthdInputMensageLog("Passou pelas obtenção da energia cargas MT declaradas.");

                                    // Captura a energia das cargas BT declaradas pelo agente
                                    this.sqlCommServer = new SqlCommand("SELECT Sum(t1.EnerMedid" + String.Format("{0:00}", j) + "_MWh) AS EnerMedid FROM dbo.[" + this.strBaseSelected + "CargaBT] AS t1 WHERE t1.CodBase = " + this.strBaseSelected + " AND t1.CodAlimAtrib = '" + strFeeder + "'", this.sqlConnServer);
                                    this.sqlCommServer.CommandTimeout = 0;
                                    this.sqlDtRdrServer = this.sqlCommServer.ExecuteReader();
                                    this.sqlDtRdrServer.Read();
                                    this.dblEnergyLoadsBTDeclared_MWh = (DBNull.Value.Equals(this.sqlDtRdrServer["EnerMedid"]) ? 0 : Convert.ToDouble(this.sqlDtRdrServer["EnerMedid"]));
                                    this.sqlDtRdrServer.Close();
                                    this.mthdInputMensageLog("Passou pelas obtenção da energia cargas BT declaradas.");

                                    if (this.intRealizaCnvrgcPNT == 1)
                                    {
                                        // Captura a energia das cargas MT Não Técnica declaradas pelo agente
                                        this.sqlCommServer = new SqlCommand("SELECT Sum(t1.EnerMedid" + String.Format("{0:00}", j) + "_MWh) AS EnerMedid FROM dbo.[" + this.strBaseSelected + "AuxCargaMTNTDem" + strFeeder + "] AS t1 WHERE t1.CodBase = " + this.strBaseSelected + " AND t1.CodAlimAtrib = '" + strFeeder + "'", this.sqlConnServer);
                                        this.sqlCommServer.CommandTimeout = 0;
                                        this.sqlDtRdrServer = this.sqlCommServer.ExecuteReader();
                                        this.sqlDtRdrServer.Read();
                                        this.dblEnergyLoadsMTNTCalculated_MWh = (DBNull.Value.Equals(this.sqlDtRdrServer["EnerMedid"]) ? 0 : Convert.ToDouble(this.sqlDtRdrServer["EnerMedid"]));
                                        this.sqlDtRdrServer.Close();
                                        this.mthdInputMensageLog("Passou pelas obtenção da energia cargas MT Não Técnicas declaradas.");

                                        // Captura a energia das cargas BT Não Técnica declaradas pelo agente
                                        this.sqlCommServer = new SqlCommand("SELECT Sum(t1.EnerMedid" + String.Format("{0:00}", j) + "_MWh) AS EnerMedid FROM dbo.[" + this.strBaseSelected + "AuxCargaBTNTDem" + strFeeder + "] AS t1 WHERE t1.CodBase = " + this.strBaseSelected + " AND t1.CodAlimAtrib = '" + strFeeder + "'", this.sqlConnServer);
                                        this.sqlCommServer.CommandTimeout = 0;
                                        this.sqlDtRdrServer = this.sqlCommServer.ExecuteReader();
                                        this.sqlDtRdrServer.Read();
                                        this.dblEnergyLoadsBTNTCalculated_MWh = (DBNull.Value.Equals(this.sqlDtRdrServer["EnerMedid"]) ? 0 : Convert.ToDouble(this.sqlDtRdrServer["EnerMedid"]));
                                        this.sqlDtRdrServer.Close();
                                        this.mthdInputMensageLog("Passou pelas obtenção da energia cargas BT Não Técnicas declaradas.");

                                        // Captura a proporção da energia que deve ficar no MT
                                        this.sqlCommServer = new SqlCommand("SELECT t1.PropPerdNTecnMT" + String.Format("{0:00}", j) + "_pu AS PropPerdNTecnMT FROM dbo.AuxProporPerdNTecn AS t1 WHERE t1.CodBase = " + this.strBaseSelected + " AND t1.CodAlim = '" + strFeeder + "'", this.sqlConnServer);
                                        this.sqlCommServer.CommandTimeout = 0;
                                        this.sqlDtRdrServer = this.sqlCommServer.ExecuteReader();
                                        this.sqlDtRdrServer.Read();
                                        this.dblPropEnergyLoadsMT_pu = (DBNull.Value.Equals(this.sqlDtRdrServer["PropPerdNTecnMT"]) ? 0 : Convert.ToDouble(this.sqlDtRdrServer["PropPerdNTecnMT"]));
                                        this.sqlDtRdrServer.Close();
                                        this.mthdInputMensageLog("Passou pelas obtenção da proporção de energia não técnica de MT.");

                                        // Captura a proporção da energia que deve ficar no BT
                                        this.sqlCommServer = new SqlCommand("SELECT t1.PropPerdNTecnBT" + String.Format("{0:00}", j) + "_pu AS PropPerdNTecnBT FROM dbo.AuxProporPerdNTecn AS t1 WHERE t1.CodBase = " + this.strBaseSelected + " AND t1.CodAlim = '" + strFeeder + "'", this.sqlConnServer);
                                        this.sqlCommServer.CommandTimeout = 0;
                                        this.sqlDtRdrServer = this.sqlCommServer.ExecuteReader();
                                        this.sqlDtRdrServer.Read();
                                        this.dblPropEnergyLoadsBT_pu = (DBNull.Value.Equals(this.sqlDtRdrServer["PropPerdNTecnBT"]) ? 0 : Convert.ToDouble(this.sqlDtRdrServer["PropPerdNTecnBT"]));
                                        this.sqlDtRdrServer.Close();
                                        this.mthdInputMensageLog("Passou pelas obtenção da proporção de energia não técnica de BT.");
                                    }

                                    for (int k = 1; k <= 3; k++)
                                    {

                                        if (j == 1 && k == 1 && intCtrlCnvrg == 0)
                                        {

                                            // Flag que sinaliza que a carga não deve ser escrita novamente quando houver necessidade de convergência da NT
                                            intCtrlCnvrg = 1;

                                            // Loop das cargas MT
                                            this.sqlCommServer = new SqlCommand("SELECT DISTINCT t1.CodBase, t1.CodConsMT, t1.CodFas, t1.CodPonAcopl, t1.TipCrvaCarga, t1.DemMax" + GeoPerdasTools.mthdGetTypeDayMonth(j, k) + "_kW, t1.CodAlimAtrib, t1.TnsLnh_kV FROM dbo.[" + this.strBaseSelected + "AuxCargaMTDem] AS t1 WHERE t1.CodBase = " + this.strBaseSelected + " AND t1.CodAlimAtrib = '" + strFeeder + "'", this.sqlConnServer);
                                            this.sqlCommServer.CommandTimeout = 0;
                                            this.mthdInsertCommandInLine("! Criação da seção de cargas MT");
                                            this.sqlDtRdrServer = this.sqlCommServer.ExecuteReader();
                                            while (this.sqlDtRdrServer.Read())
                                            {
                                                this.mthdGetDSSCommandNewLoad("MT_" + this.sqlDtRdrServer["CodConsMT"].ToString(), this.sqlDtRdrServer["CodFas"].ToString(), this.sqlDtRdrServer["CodPonAcopl"].ToString(), Convert.ToDouble(this.sqlDtRdrServer["TnsLnh_kV"]), Convert.ToDouble(this.sqlDtRdrServer["TnsLnh_kV"]), Convert.ToDouble(this.sqlDtRdrServer["DemMax" + GeoPerdasTools.mthdGetTypeDayMonth(j, k) + "_kW"]), 0, this.sqlDtRdrServer["TipCrvaCarga"].ToString() + "_" + GeoPerdasTools.mthdGetTypeDay(k), 1, this.intAdequarTensaoCargasMT, 0);
                                            }
                                            this.sqlDtRdrServer.Close();
                                            this.mthdInputMensageLog("Passou pelas cargas MT.");

                                            // Loop das cargas BT
                                            this.sqlCommServer = new SqlCommand("SELECT DISTINCT t1.CodBase, t1.CodConsBT, t1.CodFas, t1.CodPonAcopl, t1.TipCrvaCarga, t1.DemMax" + GeoPerdasTools.mthdGetTypeDayMonth(j, k) + "_kW, 0.92 * (ISNULL([M1_PotNom_kVA],0)+ISNULL([M2_PotNom_kVA],0)+ISNULL([M3_PotNom_kVA],0)) AS DemMaxTrafo_kW, t1.CodAlimAtrib, t1.TnsLnh_kV, t2.TnsFasBas_kV FROM dbo.[" + this.strBaseSelected + "AuxCargaBTDem] AS t1 LEFT JOIN dbo.[" + this.strBaseSelected + "AuxTrafoMTMTMTBT] AS t2 ON (t1.CodBase = t2.M1_CodBase AND t1.CodTrafoAtrib = t2.M1_CodTrafo) WHERE t1.CodBase = " + this.strBaseSelected + " AND t1.CodAlimAtrib = '" + strFeeder + "'", this.sqlConnServer);
                                            this.sqlCommServer.CommandTimeout = 0;
                                            this.mthdInsertCommandInLine("! Criação da seção de cargas BT");
                                            this.sqlDtRdrServer = this.sqlCommServer.ExecuteReader();
                                            while (this.sqlDtRdrServer.Read())
                                            {
                                                this.mthdGetDSSCommandNewLoad("BT_" + this.sqlDtRdrServer["CodConsBT"].ToString(), this.sqlDtRdrServer["CodFas"].ToString(), this.sqlDtRdrServer["CodPonAcopl"].ToString(), Convert.ToDouble(this.sqlDtRdrServer["TnsLnh_kV"]), Convert.ToDouble(this.sqlDtRdrServer["TnsFasBas_kV"]), Convert.ToDouble(this.sqlDtRdrServer["DemMax" + GeoPerdasTools.mthdGetTypeDayMonth(j, k) + "_kW"]), Convert.ToDouble(this.sqlDtRdrServer["DemMaxTrafo_kW"]), this.sqlDtRdrServer["TipCrvaCarga"].ToString() + "_" + GeoPerdasTools.mthdGetTypeDay(k), 2, this.intAdequarTensaoCargasBT, 0);
                                            }
                                            this.sqlDtRdrServer.Close();
                                            this.mthdInputMensageLog("Passou pelas cargas BT.");

                                            if (this.intRealizaCnvrgcPNT == 1)
                                            {
                                                // Loop das cargas MT Não Técnicas
                                                this.sqlCommServer = new SqlCommand("SELECT DISTINCT t1.CodBase, t1.CodConsMT, t1.CodFas, t1.CodPonAcopl, t1.TipCrvaCarga, t1.DemMax" + GeoPerdasTools.mthdGetTypeDayMonth(j, k) + "_kW, t1.CodAlimAtrib, t1.TnsLnh_kV FROM dbo.[" + this.strBaseSelected + "AuxCargaMTNTDem" + strFeeder + "] AS t1 WHERE t1.CodBase = " + this.strBaseSelected + " AND t1.CodAlimAtrib = '" + strFeeder + "'", this.sqlConnServer);
                                                this.sqlCommServer.CommandTimeout = 0;
                                                this.mthdInsertCommandInLine("! Criação da seção de cargas MT Não Técnicas");
                                                this.sqlDtRdrServer = this.sqlCommServer.ExecuteReader();
                                                while (this.sqlDtRdrServer.Read())
                                                {
                                                    this.mthdGetDSSCommandNewLoad("MT_" + this.sqlDtRdrServer["CodConsMT"].ToString() + "_NT", this.sqlDtRdrServer["CodFas"].ToString(), this.sqlDtRdrServer["CodPonAcopl"].ToString(), Convert.ToDouble(this.sqlDtRdrServer["TnsLnh_kV"]), Convert.ToDouble(this.sqlDtRdrServer["TnsLnh_kV"]), Convert.ToDouble(this.sqlDtRdrServer["DemMax" + GeoPerdasTools.mthdGetTypeDayMonth(j, k) + "_kW"]), 0, this.sqlDtRdrServer["TipCrvaCarga"].ToString() + "_" + GeoPerdasTools.mthdGetTypeDay(k), 1, this.intAdequarTensaoCargasMT, 0);
                                                }
                                                this.sqlDtRdrServer.Close();
                                                this.mthdInputMensageLog("Passou pelas cargas MT Não Técnicas.");

                                                // Loop das cargas BT Não Técnicas
                                                this.sqlCommServer = new SqlCommand("SELECT DISTINCT t1.CodBase, t1.CodConsBT, t1.CodFas, t1.CodPonAcopl, t1.TipCrvaCarga, t1.DemMax" + GeoPerdasTools.mthdGetTypeDayMonth(j, k) + "_kW, 0.92 * (ISNULL([M1_PotNom_kVA],0)+ISNULL([M2_PotNom_kVA],0)+ISNULL([M3_PotNom_kVA],0)) AS DemMaxTrafo_kW, t1.CodAlimAtrib, t1.TnsLnh_kV, t2.TnsFasBas_kV FROM dbo.[" + this.strBaseSelected + "AuxCargaBTNTDem" + strFeeder + "] AS t1 LEFT JOIN dbo.[" + this.strBaseSelected + "AuxTrafoMTMTMTBT] AS t2 ON (t1.CodBase = t2.M1_CodBase AND t1.CodTrafoAtrib = t2.M1_CodTrafo) WHERE t1.CodBase = " + this.strBaseSelected + " AND t1.CodAlimAtrib = '" + strFeeder + "'", this.sqlConnServer);
                                                this.sqlCommServer.CommandTimeout = 0;
                                                this.mthdInsertCommandInLine("! Criação da seção de cargas BT Não Técnicas");
                                                this.sqlDtRdrServer = this.sqlCommServer.ExecuteReader();
                                                while (this.sqlDtRdrServer.Read())
                                                {
                                                    this.mthdGetDSSCommandNewLoad("BT_" + this.sqlDtRdrServer["CodConsBT"].ToString() + "_NT", this.sqlDtRdrServer["CodFas"].ToString(), this.sqlDtRdrServer["CodPonAcopl"].ToString(), Convert.ToDouble(this.sqlDtRdrServer["TnsLnh_kV"]), Convert.ToDouble(this.sqlDtRdrServer["TnsFasBas_kV"]), Convert.ToDouble(this.sqlDtRdrServer["DemMax" + GeoPerdasTools.mthdGetTypeDayMonth(j, k) + "_kW"]), Convert.ToDouble(this.sqlDtRdrServer["DemMaxTrafo_kW"]), this.sqlDtRdrServer["TipCrvaCarga"].ToString() + "_" + GeoPerdasTools.mthdGetTypeDay(k), 2, this.intAdequarTensaoCargasBT, 0);
                                                }
                                                this.sqlDtRdrServer.Close();
                                                this.mthdInputMensageLog("Passou pelas cargas BT Não Técnicas.");
                                            }

                                        }
                                        else
                                        {

                                            // Reinício do código
                                            this.mthdStarCommands();

                                            // Loop das cargas MT
                                            this.sqlCommServer = new SqlCommand("SELECT DISTINCT t1.CodBase, t1.CodConsMT, t1.CodFas, t1.CodPonAcopl, t1.TipCrvaCarga, t1.DemMax" + GeoPerdasTools.mthdGetTypeDayMonth(j, k) + "_kW, t1.CodAlimAtrib, t1.TnsLnh_kV FROM dbo.[" + this.strBaseSelected + "AuxCargaMTDem] AS t1 WHERE t1.CodBase = " + this.strBaseSelected + " AND t1.CodAlimAtrib = '" + strFeeder + "'", this.sqlConnServer);
                                            this.sqlCommServer.CommandTimeout = 0;
                                            this.mthdInsertCommandInLine("! Criação da seção de cargas MT");
                                            this.sqlDtRdrServer = this.sqlCommServer.ExecuteReader();
                                            while (this.sqlDtRdrServer.Read())
                                            {
                                                this.mthdGetDSSCommandEditLoad("MT_" + this.sqlDtRdrServer["CodConsMT"].ToString(), this.sqlDtRdrServer["CodFas"].ToString(), Convert.ToDouble(this.sqlDtRdrServer["DemMax" + GeoPerdasTools.mthdGetTypeDayMonth(j, k) + "_kW"]), 0, this.sqlDtRdrServer["TipCrvaCarga"].ToString() + "_" + GeoPerdasTools.mthdGetTypeDay(k), 1, 0);
                                            }
                                            this.sqlDtRdrServer.Close();
                                            this.mthdInputMensageLog("Passou pelas cargas MT.");

                                            // Loop das cargas BT
                                            this.sqlCommServer = new SqlCommand("SELECT DISTINCT t1.CodBase, t1.CodConsBT, t1.CodFas, t1.CodPonAcopl, t1.TipCrvaCarga, t1.DemMax" + GeoPerdasTools.mthdGetTypeDayMonth(j, k) + "_kW, 0.92 * (ISNULL([M1_PotNom_kVA],0)+ISNULL([M2_PotNom_kVA],0)+ISNULL([M3_PotNom_kVA],0)) AS DemMaxTrafo_kW, t1.CodAlimAtrib, t1.TnsLnh_kV FROM dbo.[" + this.strBaseSelected + "AuxCargaBTDem] AS t1 LEFT JOIN dbo.[" + this.strBaseSelected + "AuxTrafoMTMTMTBT] AS t2 ON (t1.CodBase = t2.M1_CodBase AND t1.CodTrafoAtrib = t2.M1_CodTrafo) WHERE t1.CodBase = " + this.strBaseSelected + " AND t1.CodAlimAtrib = '" + strFeeder + "'", this.sqlConnServer);
                                            this.sqlCommServer.CommandTimeout = 0;
                                            this.mthdInsertCommandInLine("! Criação da seção de cargas BT");
                                            this.sqlDtRdrServer = this.sqlCommServer.ExecuteReader();
                                            while (this.sqlDtRdrServer.Read())
                                            {
                                                this.mthdGetDSSCommandEditLoad("BT_" + this.sqlDtRdrServer["CodConsBT"].ToString(), this.sqlDtRdrServer["CodFas"].ToString(), Convert.ToDouble(this.sqlDtRdrServer["DemMax" + GeoPerdasTools.mthdGetTypeDayMonth(j, k) + "_kW"]), Convert.ToDouble(this.sqlDtRdrServer["DemMaxTrafo_kW"]), this.sqlDtRdrServer["TipCrvaCarga"].ToString() + "_" + GeoPerdasTools.mthdGetTypeDay(k), 2, 0);
                                            }
                                            this.sqlDtRdrServer.Close();
                                            this.mthdInputMensageLog("Passou pelas cargas BT.");

                                            if (this.intRealizaCnvrgcPNT == 1)
                                            {
                                                // Loop das cargas MT Não Técnicas
                                                this.sqlCommServer = new SqlCommand("SELECT DISTINCT t1.CodBase, t1.CodConsMT, t1.CodFas, t1.CodPonAcopl, t1.TipCrvaCarga, t1.DemMax" + GeoPerdasTools.mthdGetTypeDayMonth(j, k) + "_kW, t1.CodAlimAtrib, t1.TnsLnh_kV FROM dbo.[" + this.strBaseSelected + "AuxCargaMTNTDem" + strFeeder + "] AS t1 WHERE t1.CodBase = " + this.strBaseSelected + " AND t1.CodAlimAtrib = '" + strFeeder + "'", this.sqlConnServer);
                                                this.sqlCommServer.CommandTimeout = 0;
                                                this.mthdInsertCommandInLine("! Criação da seção de cargas MT Não Técnicas");
                                                this.sqlDtRdrServer = this.sqlCommServer.ExecuteReader();
                                                while (this.sqlDtRdrServer.Read())
                                                {
                                                    this.mthdGetDSSCommandEditLoad("MT_" + this.sqlDtRdrServer["CodConsMT"].ToString() + "_NT", this.sqlDtRdrServer["CodFas"].ToString(), Convert.ToDouble(this.sqlDtRdrServer["DemMax" + GeoPerdasTools.mthdGetTypeDayMonth(j, k) + "_kW"]), 0, this.sqlDtRdrServer["TipCrvaCarga"].ToString() + "_" + GeoPerdasTools.mthdGetTypeDay(k), 1, 0);
                                                }
                                                this.sqlDtRdrServer.Close();
                                                this.mthdInputMensageLog("Passou pelas cargas MT Não Técnicas.");

                                                // Loop das cargas BT Não Técnicas
                                                this.sqlCommServer = new SqlCommand("SELECT DISTINCT t1.CodBase, t1.CodConsBT, t1.CodFas, t1.CodPonAcopl, t1.TipCrvaCarga, t1.DemMax" + GeoPerdasTools.mthdGetTypeDayMonth(j, k) + "_kW, 0.92 * (ISNULL([M1_PotNom_kVA],0)+ISNULL([M2_PotNom_kVA],0)+ISNULL([M3_PotNom_kVA],0)) AS DemMaxTrafo_kW, t1.CodAlimAtrib, t1.TnsLnh_kV FROM dbo.[" + this.strBaseSelected + "AuxCargaBTNTDem" + strFeeder + "] AS t1 LEFT JOIN dbo.[" + this.strBaseSelected + "AuxTrafoMTMTMTBT] AS t2 ON (t1.CodBase = t2.M1_CodBase AND t1.CodTrafoAtrib = t2.M1_CodTrafo) WHERE t1.CodBase = " + this.strBaseSelected + " AND t1.CodAlimAtrib = '" + strFeeder + "'", this.sqlConnServer);
                                                this.sqlCommServer.CommandTimeout = 0;
                                                this.mthdInsertCommandInLine("! Criação da seção de cargas BT Não Técnicas");
                                                this.sqlDtRdrServer = this.sqlCommServer.ExecuteReader();
                                                while (this.sqlDtRdrServer.Read())
                                                {
                                                    this.mthdGetDSSCommandEditLoad("BT_" + this.sqlDtRdrServer["CodConsBT"].ToString() + "_NT", this.sqlDtRdrServer["CodFas"].ToString(), Convert.ToDouble(this.sqlDtRdrServer["DemMax" + GeoPerdasTools.mthdGetTypeDayMonth(j, k) + "_kW"]), Convert.ToDouble(this.sqlDtRdrServer["DemMaxTrafo_kW"]), this.sqlDtRdrServer["TipCrvaCarga"].ToString() + "_" + GeoPerdasTools.mthdGetTypeDay(k), 2, 0);
                                                }
                                                this.sqlDtRdrServer.Close();
                                                this.mthdInputMensageLog("Passou pelas cargas BT Não Técnicas.");
                                            }

                                        }

                                        // Ajustes de execução da apuração
                                        tolerance = 0.00001;
                                        do
                                        {
                                            tolerance = tolerance * 10;
                                            this.mthdInsertCommandInLine("Set mode = daily");
                                            this.mthdInsertCommandInLine("Set tolerance = " + tolerance.ToString().Replace(",", "."));
                                            this.mthdInsertCommandInLine("Set maxcontroliter = 10");
                                            this.mthdInsertCommandInLine("!Set algorithm = newton");
                                            this.mthdInsertCommandInLine("!Solve mode = direct");
                                            this.mthdInsertCommandInLine("Solve");
                                            this.mthdEndCommands();
                                        } while (this.intConverged == 0);

                                        // Caputa as energia do dia útil, sábado e domingo para a avaliação da convergência não técnica
                                        if (k == 1)
                                        {
                                            dblEnerInjDU_kWh = this.dblTotalEnergy_kWh * GeoPerdasTools.mthdGetNumberDaysDUSADOMonth(j, intAno, GeoPerdasTools.mthdGetTypeDay(k));
                                            dblPerdaEnerTecDU_kWh = this.dblTecnicalLossEnergy_kWh * GeoPerdasTools.mthdGetNumberDaysDUSADOMonth(j, intAno, GeoPerdasTools.mthdGetTypeDay(k));
                                            dblEnerForncDU_kWh = (this.dblMTLoad_kWh + this.dblBTLoad_kWh) * GeoPerdasTools.mthdGetNumberDaysDUSADOMonth(j, intAno, GeoPerdasTools.mthdGetTypeDay(k));
                                        }
                                        else if (k == 2)
                                        {
                                            dblEnerInjSA_kWh = this.dblTotalEnergy_kWh * GeoPerdasTools.mthdGetNumberDaysDUSADOMonth(j, intAno, GeoPerdasTools.mthdGetTypeDay(k));
                                            dblPerdaEnerTecSA_kWh = this.dblTecnicalLossEnergy_kWh * GeoPerdasTools.mthdGetNumberDaysDUSADOMonth(j, intAno, GeoPerdasTools.mthdGetTypeDay(k));
                                            dblEnerForncSA_kWh = (this.dblMTLoad_kWh + this.dblBTLoad_kWh) * GeoPerdasTools.mthdGetNumberDaysDUSADOMonth(j, intAno, GeoPerdasTools.mthdGetTypeDay(k));
                                        }
                                        else if (k == 3)
                                        {
                                            dblEnerInjDO_kWh = this.dblTotalEnergy_kWh * GeoPerdasTools.mthdGetNumberDaysDUSADOMonth(j, intAno, GeoPerdasTools.mthdGetTypeDay(k));
                                            dblPerdaEnerTecDO_kWh = this.dblTecnicalLossEnergy_kWh * GeoPerdasTools.mthdGetNumberDaysDUSADOMonth(j, intAno, GeoPerdasTools.mthdGetTypeDay(k));
                                            dblEnerForncDO_kWh = (this.dblMTLoad_kWh + this.dblBTLoad_kWh) * GeoPerdasTools.mthdGetNumberDaysDUSADOMonth(j, intAno, GeoPerdasTools.mthdGetTypeDay(k));
                                        }

                                        // Armazena os resultados na tabela do servidor
                                        this.sqlCommServer = new SqlCommand("DELETE FROM dbo.[" + this.strBaseSelected + "AuxResultado] WHERE CodBase = " + this.strBaseSelected + " AND CodAlim = '" + strFeeder + "' AND TipoDiaMes = '" + GeoPerdasTools.mthdGetTypeDayMonth(j, k) + "' AND TipoRodada = '" + this.mthdGetOptionsToString() + "'", this.sqlConnServer);
                                        this.sqlCommServer.CommandTimeout = 0;
                                        this.sqlCommServer.ExecuteNonQuery();
                                        strTempSqlCommand = "INSERT INTO dbo.[" + this.strBaseSelected + "AuxResultado] (CodBase, CodAlim, TipoDiaMes, TipoRodada, Dias, Tolerancia, PotenciaMaxInj_kW, EnergiaInj_kWh, EnergiaFornc_kWh, PerdaPotenciaMaxTecnica_kW, PerdaEnergiaTecnica_kWh, PerdaEnergiaFerro_kWh, PerdaEnergiaTrafo_kWh, PerdaEnergiaFerroA3aA3a_kWh, PerdaEnergiaTrafoA3aA3a_kWh, PerdaEnergiaFerroA3aA4_kWh, PerdaEnergiaTrafoA3aA4_kWh, PerdaEnergiaFerroA3aB_kWh, PerdaEnergiaTrafoA3aB_kWh, PerdaEnergiaFerroA4A4_kWh, PerdaEnergiaTrafoA4A4_kWh, PerdaEnergiaFerroA4B_kWh, PerdaEnergiaTrafoA4B_kWh, PerdaEnergiaFerroA4A3a_kWh, PerdaEnergiaTrafoA4A3a_kWh, PerdaEnergiaFerroBA3a_kWh, PerdaEnergiaTrafoBA3a_kWh, PerdaEnergiaFerroBA4_kWh, PerdaEnergiaTrafoBA4_kWh, PerdaEnergiaLinhas_kWh, PerdaEnergiaMT_kWh, PerdaEnergiaBT_kWh, PerdaEnergiaLinhasMT_kWh, PerdaEnergiaLinhasBT_kWh, PerdaEnergiaLinhasMTA3a_kWh, PerdaEnergiaLinhasMTA4_kWh, EnergiaForncMT_kWh, EnergiaForncBT_kWh) ";
                                        strTempSqlCommand += "VALUES (" + this.strBaseSelected + ", '" + strFeeder + "', '" + GeoPerdasTools.mthdGetTypeDayMonth(j, k) + "', '" + this.mthdGetOptionsToString() + "', " + GeoPerdasTools.mthdGetNumberDaysDUSADOMonth(j, intAno, GeoPerdasTools.mthdGetTypeDay(k)) + ", " + tolerance.ToString().Replace(",", ".") + ", " + String.Format("{0:F9}", GeoPerdasTools.mthdNumberCorrection(this.dblTotalPower_kW)).Replace(",", ".") + ", " + String.Format("{0:F9}", GeoPerdasTools.mthdNumberCorrection(this.dblTotalEnergy_kWh)).Replace(",", ".") + ", " + String.Format("{0:F9}", GeoPerdasTools.mthdNumberCorrection(this.dblTotalLoadEnergy_kWh)).Replace(",", ".") + ", " + String.Format("{0:F9}", GeoPerdasTools.mthdNumberCorrection(this.dblTecnicalLossPower_kW)).Replace(",", ".") + ", " + String.Format("{0:F9}", GeoPerdasTools.mthdNumberCorrection(this.dblTecnicalLossEnergy_kWh)).Replace(",", ".") + ", " + String.Format("{0:F9}", GeoPerdasTools.mthdNumberCorrection(this.dblNoLoadLosses_kWh)).Replace(",", ".") + ", " + String.Format("{0:F9}", GeoPerdasTools.mthdNumberCorrection(this.dblTransformerLosses_kWh)).Replace(",", ".") + ", " + String.Format("{0:F9}", GeoPerdasTools.mthdNumberCorrection(this.dblA3aA3aTransformerNoLoadLosses_kWh)).Replace(",", ".") + ", " + String.Format("{0:F9}", GeoPerdasTools.mthdNumberCorrection(this.dblA3aA3aTransformerLosses_kWh)).Replace(",", ".") + ", " + String.Format("{0:F9}", GeoPerdasTools.mthdNumberCorrection(this.dblA3aA4TransformerNoLoadLosses_kWh)).Replace(",", ".") + ", " + String.Format("{0:F9}", GeoPerdasTools.mthdNumberCorrection(this.dblA3aA4TransformerLosses_kWh)).Replace(",", ".") + ", " + String.Format("{0:F9}", GeoPerdasTools.mthdNumberCorrection(this.dblA3aBTransformerNoLoadLosses_kWh)).Replace(",", ".") + ", " + String.Format("{0:F9}", GeoPerdasTools.mthdNumberCorrection(this.dblA3aBTransformerLosses_kWh)).Replace(",", ".") + ", " + String.Format("{0:F9}", GeoPerdasTools.mthdNumberCorrection(this.dblA4A4TransformerNoLoadLosses_kWh)).Replace(",", ".") + ", " + String.Format("{0:F9}", GeoPerdasTools.mthdNumberCorrection(this.dblA4A4TransformerLosses_kWh)).Replace(",", ".") + ", " + String.Format("{0:F9}", GeoPerdasTools.mthdNumberCorrection(this.dblA4BTransformerNoLoadLosses_kWh)).Replace(",", ".") + ", " + String.Format("{0:F9}", GeoPerdasTools.mthdNumberCorrection(this.dblA4BTransformerLosses_kWh)).Replace(",", ".") + ", " + String.Format("{0:F9}", GeoPerdasTools.mthdNumberCorrection(this.dblA4A3aTransformerNoLoadLosses_kWh)).Replace(",", ".") + ", " + String.Format("{0:F9}", GeoPerdasTools.mthdNumberCorrection(this.dblA4A3aTransformerLosses_kWh)).Replace(",", ".") + ", " + String.Format("{0:F9}", GeoPerdasTools.mthdNumberCorrection(this.dblBA3aTransformerNoLoadLosses_kWh)).Replace(",", ".") + ", " + String.Format("{0:F9}", GeoPerdasTools.mthdNumberCorrection(this.dblBA3aTransformerLosses_kWh)).Replace(",", ".") + ", " + String.Format("{0:F9}", GeoPerdasTools.mthdNumberCorrection(this.dblBA4TransformerNoLoadLosses_kWh)).Replace(",", ".") + ", " + String.Format("{0:F9}", GeoPerdasTools.mthdNumberCorrection(this.dblBA4TransformerLosses_kWh)).Replace(",", ".") + ", " + String.Format("{0:F9}", GeoPerdasTools.mthdNumberCorrection(this.dblLineLosses_kWh)).Replace(",", ".") + ", " + String.Format("{0:F9}", GeoPerdasTools.mthdNumberCorrection(this.dblMTLosses_kWh)).Replace(",", ".") + ", " + String.Format("{0:F9}", GeoPerdasTools.mthdNumberCorrection(this.dblBTLosses_kWh)).Replace(",", ".") + ", " + String.Format("{0:F9}", GeoPerdasTools.mthdNumberCorrection(this.dblLineMTLosses_kWh)).Replace(",", ".") + ", " + String.Format("{0:F9}", GeoPerdasTools.mthdNumberCorrection(this.dblLineBTLosses_kWh)).Replace(",", ".") + ", " + String.Format("{0:F9}", GeoPerdasTools.mthdNumberCorrection(this.dblLineA3aLosses_kWh)).Replace(",", ".") + ", " + String.Format("{0:F9}", GeoPerdasTools.mthdNumberCorrection(this.dblLineA4Losses_kWh)).Replace(",", ".") + ", " + String.Format("{0:F9}", GeoPerdasTools.mthdNumberCorrection(this.dblMTLoad_kWh)).Replace(",", ".") + ", " + String.Format("{0:F9}", GeoPerdasTools.mthdNumberCorrection(this.dblBTLoad_kWh)).Replace(",", ".") + ")";
                                        this.sqlCommServer = new SqlCommand(strTempSqlCommand, this.sqlConnServer);
                                        this.sqlCommServer.CommandTimeout = 0;
                                        this.sqlCommServer.ExecuteNonQuery();

                                    }

                                    this.mthdInputMensageLog(GeoPerdasTools.mthdNumberCorrection(this.dblEnergyInjectDeclared_MWh).ToString() + " = " + GeoPerdasTools.mthdNumberCorrection(this.dblEnergyLoadsMTDeclared_MWh).ToString() + "+" + GeoPerdasTools.mthdNumberCorrection(this.dblEnergyLoadsBTDeclared_MWh).ToString() + "+" + GeoPerdasTools.mthdNumberCorrection(dblEnergyLoadsMTNTCalculated_MWh).ToString() + "+" + GeoPerdasTools.mthdNumberCorrection(dblEnergyLoadsBTNTCalculated_MWh).ToString() + "+" + (GeoPerdasTools.mthdNumberCorrection(dblPerdaEnerTecDU_kWh) / 1000 + GeoPerdasTools.mthdNumberCorrection(dblPerdaEnerTecSA_kWh) / 1000 + GeoPerdasTools.mthdNumberCorrection(dblPerdaEnerTecDO_kWh) / 1000).ToString());

                                    if (this.intModeloConverge == 1)
                                    {
                                        dblDelta = (GeoPerdasTools.mthdNumberCorrection(this.dblEnergyInjectDeclared_MWh) - GeoPerdasTools.mthdNumberCorrection(this.dblEnergyLoadsMTDeclared_MWh) - GeoPerdasTools.mthdNumberCorrection(this.dblEnergyLoadsBTDeclared_MWh) - GeoPerdasTools.mthdNumberCorrection(dblEnergyLoadsMTNTCalculated_MWh) - GeoPerdasTools.mthdNumberCorrection(dblEnergyLoadsBTNTCalculated_MWh) - GeoPerdasTools.mthdNumberCorrection(dblPerdaEnerTecDU_kWh) / 1000 - GeoPerdasTools.mthdNumberCorrection(dblPerdaEnerTecSA_kWh) / 1000 - GeoPerdasTools.mthdNumberCorrection(dblPerdaEnerTecDO_kWh) / 1000) / (GeoPerdasTools.mthdNumberCorrection(this.dblEnergyLoadsMTDeclared_MWh) + GeoPerdasTools.mthdNumberCorrection(this.dblEnergyLoadsBTDeclared_MWh) + GeoPerdasTools.mthdNumberCorrection(this.dblEnergyLoadsMTNTCalculated_MWh) + GeoPerdasTools.mthdNumberCorrection(this.dblEnergyLoadsBTNTCalculated_MWh));
                                    }
                                    else if (this.intModeloConverge == 2)
                                    {
                                        dblDelta = (GeoPerdasTools.mthdNumberCorrection(this.dblEnergyInjectDeclared_MWh) - dblEnerInjDU_kWh / 1000 - dblEnerInjSA_kWh / 1000 - dblEnerInjDO_kWh / 1000) / (GeoPerdasTools.mthdNumberCorrection(this.dblEnergyLoadsMTDeclared_MWh) + GeoPerdasTools.mthdNumberCorrection(this.dblEnergyLoadsBTDeclared_MWh) + GeoPerdasTools.mthdNumberCorrection(this.dblEnergyLoadsMTNTCalculated_MWh) + GeoPerdasTools.mthdNumberCorrection(this.dblEnergyLoadsBTNTCalculated_MWh));
                                    }
                                    dblDelta_MWh = dblDelta * (GeoPerdasTools.mthdNumberCorrection(this.dblEnergyLoadsMTDeclared_MWh) + GeoPerdasTools.mthdNumberCorrection(this.dblEnergyLoadsBTDeclared_MWh));
                                    dblDeltaMT_MWh = dblDelta_MWh * this.dblPropEnergyLoadsMT_pu;
                                    dblDeltaBT_MWh = dblDelta_MWh * this.dblPropEnergyLoadsBT_pu;
                                
                                    if (GeoPerdasTools.mthdNumberCorrection(this.dblEnergyLoadsMTDeclared_MWh) + GeoPerdasTools.mthdNumberCorrection(this.dblEnergyLoadsMTNTCalculated_MWh) != 0)
                                        dblDeltaMT = dblDeltaMT_MWh / (GeoPerdasTools.mthdNumberCorrection(this.dblEnergyLoadsMTDeclared_MWh) + GeoPerdasTools.mthdNumberCorrection(this.dblEnergyLoadsMTNTCalculated_MWh));
                                    else
                                        dblDeltaMT = 0;
                                
                                    if (GeoPerdasTools.mthdNumberCorrection(this.dblEnergyLoadsBTDeclared_MWh) + GeoPerdasTools.mthdNumberCorrection(this.dblEnergyLoadsBTNTCalculated_MWh) != 0)
                                        dblDeltaBT = dblDeltaBT_MWh / (GeoPerdasTools.mthdNumberCorrection(this.dblEnergyLoadsBTDeclared_MWh) + GeoPerdasTools.mthdNumberCorrection(this.dblEnergyLoadsBTNTCalculated_MWh));
                                    else
                                        dblDeltaBT = 0;
                                
                                    this.mthdInputMensageLog("Delta (MWh): " + dblDelta_MWh.ToString());

                                    if (this.intRealizaCnvrgcPNT == 1)
                                    {
                                        // Executa a stored procedure de atualização das cargas BT Não Técnicas
                                        this.sqlCommServer = new SqlCommand("dbo.AtualizaDemandaCargaBTNT", this.sqlConnServer);
                                        this.sqlCommServer.CommandType = CommandType.StoredProcedure;
                                        this.sqlCommServer.Parameters.AddWithValue("@BaseAnalisada", this.strBaseSelected);
                                        this.sqlCommServer.Parameters.AddWithValue("@AlimAnalisado", strFeeder);
                                        this.sqlCommServer.Parameters.AddWithValue("@Mes", j);
                                        this.sqlCommServer.Parameters.AddWithValue("@DeltaBT", dblDeltaBT);
                                        this.sqlCommServer.ExecuteNonQuery();

                                        // Executa a stored procedure de atualização das cargas MT Não Técnicas
                                        this.sqlCommServer = new SqlCommand("dbo.AtualizaDemandaCargaMTNT", this.sqlConnServer);
                                        this.sqlCommServer.CommandType = CommandType.StoredProcedure;
                                        this.sqlCommServer.Parameters.AddWithValue("@BaseAnalisada", this.strBaseSelected);
                                        this.sqlCommServer.Parameters.AddWithValue("@AlimAnalisado", strFeeder);
                                        this.sqlCommServer.Parameters.AddWithValue("@Mes", j);
                                        this.sqlCommServer.Parameters.AddWithValue("@DeltaMT", dblDeltaMT);
                                        this.sqlCommServer.ExecuteNonQuery();
                                    }

                                    intQuantIntrtn++;
                                } while ((this.intRealizaCnvrgcPNT == 1) && (Math.Abs(dblDelta_MWh) > 0.5) && (intQuantIntrtn <= 10));

                                this.dblEnergyInjectDeclaredYearly_MWh += GeoPerdasTools.mthdNumberCorrection(this.dblEnergyInjectDeclared_MWh);
                                this.dblEnergyInjectCalculatedYearly_MWh += (dblEnerInjDU_kWh + dblEnerInjSA_kWh + dblEnerInjDO_kWh) / 1000;

                                if (dblDelta_MWh > 0.5)
                                    this.mthdInputMensageLog("O processo realizou " + intQuantIntrtn.ToString() + " iterações e foi concluído foi concluído com energia superior 0.5 Mwh: " + dblDelta_MWh.ToString());
                                else
                                    this.mthdInputMensageLog("O processo realizou " + intQuantIntrtn.ToString() + " iterações e foi concluído foi concluído com energia inferior ou igual 0.5 Mwh: " + dblDelta_MWh.ToString());

                            }

                            dblErrorEnerInj = (this.dblEnergyInjectDeclaredYearly_MWh - this.dblEnergyInjectCalculatedYearly_MWh) / this.dblEnergyInjectDeclaredYearly_MWh;
                            if (this.intRealizaCnvrgcPNT == 1 && this.intModeloConverge == 1 && dblErrorEnerInj < (7.3 / 100))
                            {
                                this.mthdInputMensageLog("O processo precisa ser executado com modelo de convergência secundário (erro " + (dblErrorEnerInj * 100).ToString() + "% inferior a 7.3%)");
                                this.intModeloConverge = 2;
                                goto ModeloConvergenciaSecundaria;
                            }
                        
                            // Executa a stored procedure de atualização dos resultados mensais e anuais do alimentador
                            this.sqlCommServer = new SqlCommand("dbo.AtualizaResultadosAlimentador", this.sqlConnServer);
                            this.sqlCommServer.CommandType = CommandType.StoredProcedure;
                            this.sqlCommServer.Parameters.AddWithValue("@BaseAnalisada", this.strBaseSelected);
                            this.sqlCommServer.Parameters.AddWithValue("@AlimAnalisado", strFeeder);
                            this.sqlCommServer.Parameters.AddWithValue("@TipoRodada", this.mthdGetOptionsToString());
                            this.sqlCommServer.ExecuteNonQuery();

                            if (this.intRealizaCnvrgcPNT == 1)
                            {
                                // Executa a stored procedure de armazenamento final dos resultados das cargas nao tecnicas do alimentador
                                this.sqlCommServer = new SqlCommand("dbo.ArmazenaCargaNTAlimentador", this.sqlConnServer);
                                this.sqlCommServer.CommandType = CommandType.StoredProcedure;
                                this.sqlCommServer.Parameters.AddWithValue("@BaseAnalisada", this.strBaseSelected);
                                this.sqlCommServer.Parameters.AddWithValue("@AlimAnalisado", strFeeder);
                                this.sqlCommServer.ExecuteNonQuery();
                            }

                            // Término do processo
                            this.mthdInputMensageLog("Término do processo para o alimentador " + strFeeder + ".");
                            this.rtbLog.Refresh();


                        }
                        catch (Exception xcptn)
                        {
                            this.mthdInputMensageLog("Problema não identificado no alimentador " + strFeeder + " (" + xcptn.Message.ToString() + ").");
                            this.sqlDtRdrServer.Dispose();
                        }
                            
                        /*
                        this.strFileData[this.lngindex] = "<FIM DOS DADOS>"; this.lngindex++;
                        using (swFile = new System.IO.StreamWriter(@"C:\Users\renatosousa\Desktop\Teste_" + this.strBaseSelected + "_" + strFeeder + "_" + this.mthdGetOptionsToString() + ".txt", false, Encoding.GetEncoding(1252)))
                        {
                            foreach (String strLine in this.strFileData)
                            {
                                if (strLine != "<FIM DOS DADOS>")
                                    swFile.WriteLine(strLine);
                                else
                                    break;
                            }
                        }
                        */
                    }

                    // Executa a stored procedure de atualização dos resultados mensais e anuais
                    //this.sqlCommServer = new SqlCommand("dbo.AtualizaResultados", this.sqlConnServer);
                    //this.sqlCommServer.CommandType = CommandType.StoredProcedure;
                    //this.sqlCommServer.Parameters.AddWithValue("@BaseAnalisada", this.strBaseSelected);
                    //this.sqlCommServer.ExecuteNonQuery();

                    // Executa consulta que retorna diversos resultados
                    //this.sqlCommServer1 = new SqlCommand("SELECT * FROM dbo.AuxTramo AS t1", this.sqlConnServer);
                    //this.sqlDtRdrServer1 = this.sqlCommServer1.ExecuteReader();
                    //while (this.sqlDtRdrServer1.Read())
                    //{
                    //    this.mthdInputMensageLog(this.sqlDtRdrServer1["CodBase"] + ">" + this.sqlDtRdrServer1["CodNomeElem"]);
                    //}
                    //this.sqlDtRdrServer1.Close();

                    // Executa consulta que retorna não retorna resultados
                    //this.sqlCommServer = new SqlCommand("DELETE FROM AuxLog", this.sqlConnServer);
                    //this.sqlCommServer.ExecuteNonQuery();

                    // Executa uma stored procedure
                    //this.sqlCommServer = new SqlCommand("dbo.UPDATE_DEMCARGABTNT", this.sqlConnServer);
                    //this.sqlCommServer.CommandType = CommandType.StoredProcedure;
                    //this.sqlCommServer.Parameters.AddWithValue("@BaseAnalisada", this.strBaseSelected);
                    //this.sqlCommServer.Parameters.AddWithValue("@AlimAnalisado", strFeeder);
                    //this.sqlCommServer.Parameters.AddWithValue("@Mes", j);
                    //this.sqlCommServer.Parameters.AddWithValue("@DeltaBT", 34.56);
                    //this.sqlCommServer.ExecuteNonQuery();

                    // Ou executa uma stored procedure desta outra forma
                    //this.sqlCommServer = new SqlCommand("dbo.UPDATE_DEMCARGABTNT", this.sqlConnServer);
                    //this.sqlCommServer.CommandType = CommandType.StoredProcedure;
                    //this.sqlCommServer.Parameters.Add(new SqlParameter("@BaseAnalisada", SqlDbType.Int)).Value = this.strBaseSelected;
                    //this.sqlCommServer.Parameters.Add(new SqlParameter("@AlimAnalisado", SqlDbType.NVarChar)).Value = strFeeder;
                    //this.sqlCommServer.Parameters.Add(new SqlParameter("@Mes", SqlDbType.Int)).Value = j;
                    //this.sqlCommServer.Parameters.Add(new SqlParameter("@DeltaBT", SqlDbType.Decimal)).Value =  34.56;
                    //this.sqlCommServer.ExecuteNonQuery();

                }
                catch (Exception xcptn)
                {
                    this.mthdInputMensageLog("Problema não identificado (" + xcptn.Message.ToString() + ").");
                    if (sqlDtRdrServer != null)
                        this.sqlDtRdrServer.Dispose();
                    if (sqlCommServer != null)
                        this.sqlCommServer.Dispose();

                }
            }
        }

        private void bExecuteFile_Click(object sender, EventArgs e)
        {
            if (this.tbPathFile.Text == "" || this.tbPathFile.Text == null)
            {
                this.mthdInputMensageLog("O arquivo não foi estabelecido.");
                return;
            }
            else
            {
                this.mthdLoadCircuit(this.tbPathFile.Text);
                this.mthdSolveCircuit();
                //this.mthdInputMensageLog("\t" + "Tensões de Sequencia");
                //this.mthdInputMensageLog("\t" + "Barra" + "\t" + "Módulo da Tensão de Sequencia 0 (kV)" + "\t" + "Módulo da Tensão de Sequencia Positiva (kV)" + "\t" + "Módulo da Tensão de Sequencia Negativa (kV)" + "\t" + "V0.Real (kV)" + "\t" + "V0.Imaginário (kV)" + "\t" + "V1.Real (kV)" + "\t" + "V1.Imaginário (kV)" + "\t" + "V2.Real (kV)" + "\t" + "V2.Imaginário (kV)");
                //this.getSequenceVoltages();
                //this.mthdInputMensageLog("\t" + "Tensões Complexas por Fase (pu)");
                //this.mthdInputMensageLog("\t" + "Barra" + "\t" + "(Tensão Fase 1).Real (pu)" + "\t" + "(Tensão Fase 1).Imaginária (pu)" + "\t" + "(Tensão Fase 2).Real (pu)" + "\t" + "(Tensão Fase 2).Imaginária (pu)" + "\t" + "(Tensão Fase 3).Real (pu)" + "\t" + "(Tensão Fase 3).Imaginária (pu)");
                //this.getVoltages();
                //this.mthdInputMensageLog("\t" + "Tensões Polares por Fase (pu)");
                //this.mthdInputMensageLog("\t" + "Barra" + "\t" + "(Módulo da Tensão da Fase 1 ; Ângulo da Tensão da Fase 1) (pu)" + "\t" + "(Módulo da Tensão da Fase 2 ; Ângulo da Tensão da Fase 2) (pu)" + "\t" + "(Módulo da Tensão da Fase 3 ; Ângulo da Tensão da Fase 3) (pu)");
                //this.getMagnitudeAngle();
                //this.mthdInputMensageLog("\t" + "Correntes de Sequencia");
                //this.getSequenceCurrents();
                //this.mthdInputMensageLog("\t" + "Potências");
                //this.mthdInputMensageLog("\t" + "Carga / Geração" + "\t" + "(Potência Ativa (kW) ; Potência Reativa (kvar))");
                //this.getLoadPowers();
                this.mthdGetInfoMeters();
                this.mthdInputMensageLog("\tPotenciaMaxInj_kW\tEnergiaInj_kWh\tEnergiaFornc_kWh\tPerdaPotenciaMaxTecnica_kW\tPerdaEnergiaTecnica_kWh\tPerdaEnergiaFerro_kWh\tPerdaEnergiaTrafo_kWh\tPerdaEnergiaFerroA3aA3a_kWh\tPerdaEnergiaTrafoA3aA3a_kWh\tPerdaEnergiaFerroA3aA4_kWh\tPerdaEnergiaTrafoA3aA4_kWh\tPerdaEnergiaFerroA3aB_kWh\tPerdaEnergiaTrafoA3aB_kWh\tPerdaEnergiaFerroA4A4_kWh\tPerdaEnergiaTrafoA4A4_kWh\tPerdaEnergiaFerroA4B_kWh\tPerdaEnergiaTrafoA4B_kWh\tPerdaEnergiaFerroA4A3a_kWh\tPerdaEnergiaTrafoA4A3a_kWh\tPerdaEnergiaFerroBA3a_kWh\tPerdaEnergiaTrafoBA3a_kWh\tPerdaEnergiaFerroBA4_kWh\tPerdaEnergiaTrafoBA4_kWh\tPerdaEnergiaLinhas_kWh\tPerdaEnergiaMT_kWh\tPerdaEnergiaBT_kWh\tPerdaEnergiaLinhasMT_kWh\tPerdaEnergiaLinhasBT_kWh\tPerdaEnergiaLinhasMTA3a_kWh\tPerdaEnergiaLinhasMTA4_kWh\tEnergiaForncMT_kWh\tEnergiaForncBT_kWh");
                this.mthdInputMensageLog("\t" + this.dblTotalPower_kW.ToString() + "\t" + this.dblTotalEnergy_kWh.ToString() + "\t" + this.dblTotalLoadEnergy_kWh.ToString() + "\t" + this.dblTecnicalLossPower_kW.ToString() + "\t" + this.dblTecnicalLossEnergy_kWh.ToString() + "\t" + this.dblNoLoadLosses_kWh.ToString() + "\t" + this.dblTransformerLosses_kWh.ToString() + "\t" + this.dblA3aA3aTransformerNoLoadLosses_kWh.ToString() + "\t" + this.dblA3aA3aTransformerLosses_kWh.ToString() + "\t" + this.dblA3aA4TransformerNoLoadLosses_kWh.ToString() + "\t" + this.dblA3aA4TransformerLosses_kWh.ToString() + "\t" + this.dblA3aBTransformerNoLoadLosses_kWh.ToString() + "\t" + this.dblA3aBTransformerLosses_kWh.ToString() + "\t" + this.dblA4A4TransformerNoLoadLosses_kWh.ToString() + "\t" + this.dblA4A4TransformerLosses_kWh.ToString() + "\t" + this.dblA4BTransformerNoLoadLosses_kWh.ToString() + "\t" + this.dblA4BTransformerLosses_kWh.ToString() + "\t" + this.dblA4A3aTransformerNoLoadLosses_kWh.ToString() + "\t" + this.dblA4A3aTransformerLosses_kWh.ToString() + "\t" + this.dblBA3aTransformerNoLoadLosses_kWh.ToString() + "\t" + this.dblBA3aTransformerLosses_kWh.ToString() + "\t" + this.dblBA4TransformerNoLoadLosses_kWh.ToString() + "\t" + this.dblBA4TransformerLosses_kWh.ToString() + "\t" + this.dblLineLosses_kWh.ToString() + "\t" + this.dblMTLosses_kWh.ToString() + "\t" + this.dblBTLosses_kWh.ToString() + "\t" + this.dblLineMTLosses_kWh.ToString() + "\t" + this.dblLineBTLosses_kWh.ToString() + "\t" + this.dblLineA3aLosses_kWh.ToString() + "\t" + this.dblLineA4Losses_kWh.ToString() + "\t" + this.dblMTLoad_kWh.ToString() + "\t" + this.dblBTLoad_kWh.ToString());
                this.mthdWriteMeterReportHTML(Path.GetDirectoryName(this.tbPathFile.Text));
            }
        }

        private bool mthdOpenDataBase(String strServer, String strDataBase, string User, string Pswd)
        {
            try
            {
                //_strConn = @"Data Source=bdgeodisthom,5480;Initial Catalog=" + Banco + ";User Id='sde';Password='homsigr2019';Integrated Security=False;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;Min Pool Size=5;Max Pool Size=250"; 
                this.strCommandConnection = "Data Source=" + strServer;
                this.strCommandConnection += ";Initial Catalog=" + strDataBase;
                this.strCommandConnection += ";User Id='" + User + "'";
                this.strCommandConnection += ";Password='"+ Pswd + "'";
                this.strCommandConnection += ";Integrated Security=False";
                this.strCommandConnection += ";Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;Min Pool Size=5;Max Pool Size=250";
                this.sqlConnServer = new SqlConnection(this.strCommandConnection);

                this.sqlConnServer.Open();
                this.mthdInputMensageLog("A conexão com o banco de dados foi estabelecida.");
                return true;
            }
            catch (Exception xcptn)
            {
                this.mthdInputMensageLog("Não foi possível estabelecer conexão com o banco de dados (" + xcptn.Message.ToString() + ").");
                return false;
            }
        }

        private bool mthdDataBaseOpened()
        {
            if (this.sqlConnServer != null)
            {
                if (this.sqlConnServer.State != ConnectionState.Closed) { return true; }
            }
            return false;
        }

        private void mthdCloseDataBase()
        {
            if (this.sqlConnServer != null)
                this.sqlConnServer.Close();
            else
                this.mthdInputMensageLog("Não há qualquer conexão ativa.");
        }

        private void mthdInputMensageLog(String mensage)
        {
            this.rtbLog.AppendText("[" + DateTime.Now.ToString() + "] " + mensage + Environment.NewLine);
        }

        private void mthdStarDSSServer()
        {
            this.dssServer = new DSS();

            if (!(this.dssServer.Start(0)))
            {
                this.mthdInputMensageLog("A inicialização do servidor DSS falhou.");
                return;
            }
            else
            {
                this.mthdInputMensageLog("A inicialização do servidor DSS obteve sucesso.");
                this.dssText = this.dssServer.Text;
                this.dssServer.AllowForms = false;
            }
        }

        private void mthdLoadCircuit(String strPathFile)
        {
            if (strPathFile == "" || strPathFile == null)
            {
                this.mthdInputMensageLog("O caminho e o arquivo não foi informado.");
                return;
            }
            if (!(this.dssServer.Start(0)))
            {
                this.mthdInputMensageLog("O servidor DSS não foi inicializado.");
                return;
            }

            this.dssText.Command = "clear";
            this.dssText.Command = "compile " + "(" + strPathFile + ")";
            this.dssCircuit = this.dssServer.ActiveCircuit;
            this.dssSolution = this.dssCircuit.Solution;
            this.dssControlQueue = this.dssCircuit.CtrlQueue;

            this.mthdInputMensageLog("O circuito foi carregado.");
        }

        private void mthdStarCommands()
        {
            if (!(this.dssServer.Start(0)))
            {
                this.mthdInputMensageLog("O servidor DSS não foi inicializado.");
                return;
            }
        }

        private void mthdInsertCommandInLine(String strCommand)
        {
            if (strCommand == "" || strCommand == null)
            {
                this.mthdInputMensageLog("O comando não foi informado.");
                return;
            }
            if (!(this.dssServer.Start(0)))
            {
                this.mthdInputMensageLog("O servidor DSS não foi inicializado.");
                return;
            }
//this.strFileData[this.lngindex] = strCommand; this.lngindex++;
            this.dssText.Command = strCommand;
        }

        private void mthdEndCommands()
        {
            if (!(this.dssServer.Start(0)))
            {
                this.mthdInputMensageLog("O servidor DSS não foi inicializado.");
                return;
            }
            this.dssCircuit = this.dssServer.ActiveCircuit;
            this.dssSolution = this.dssCircuit.Solution;
            this.dssControlQueue = this.dssCircuit.CtrlQueue;
            if (this.dssSolution.Converged)
            {
                this.intConverged = 1;
                this.mthdInputMensageLog("Solução convergiu.");
                //this.mthdInputMensageLog("\t" + "Tensões de Sequencia");
                //this.mthdInputMensageLog("\t" + "Barra" + "\t" + "Módulo da Tensão de Sequencia 0 (kV)" + "\t" + "Módulo da Tensão de Sequencia Positiva (kV)" + "\t" + "Módulo da Tensão de Sequencia Negativa (kV)" + "\t" + "V0.Real (kV)" + "\t" + "V0.Imaginário (kV)" + "\t" + "V1.Real (kV)" + "\t" + "V1.Imaginário (kV)" + "\t" + "V2.Real (kV)" + "\t" + "V2.Imaginário (kV)");
                //this.getSequenceVoltages();
                //this.mthdInputMensageLog("\t" + "Tensões Complexas por Fase (pu)");
                //this.mthdInputMensageLog("\t" + "Barra" + "\t" + "(Tensão Fase 1).Real (pu)" + "\t" + "(Tensão Fase 1).Imaginária (pu)" + "\t" + "(Tensão Fase 2).Real (pu)" + "\t" + "(Tensão Fase 2).Imaginária (pu)" + "\t" + "(Tensão Fase 3).Real (pu)" + "\t" + "(Tensão Fase 3).Imaginária (pu)");
                //this.getVoltages();
                //this.mthdInputMensageLog("\t" + "Tensões Polares por Fase (pu)");
                //this.mthdInputMensageLog("\t" + "Barra" + "\t" + "(Módulo da Tensão da Fase 1 ; Ângulo da Tensão da Fase 1) (pu)" + "\t" + "(Módulo da Tensão da Fase 2 ; Ângulo da Tensão da Fase 2) (pu)" + "\t" + "(Módulo da Tensão da Fase 3 ; Ângulo da Tensão da Fase 3) (pu)");
                //this.getMagnitudeAngle();
                //this.mthdInputMensageLog("\t" + "Correntes de Sequencia");
                //this.getSequenceCurrents();
                //this.mthdInputMensageLog("\t" + "Potências");
                //this.mthdInputMensageLog("\t" + "Carga / Geração" + "\t" + "(Potência Ativa (kW) ; Potência Reativa (kvar))");
                //this.getLoadPowers();
                this.mthdGetInfoMeters();
                this.mthdInputMensageLog("\tPotenciaMaxInj_kW\tEnergiaInj_kWh\tEnergiaFornc_kWh\tPerdaPotenciaMaxTecnica_kW\tPerdaEnergiaTecnica_kWh\tPerdaEnergiaFerro_kWh\tPerdaEnergiaTrafo_kWh\tPerdaEnergiaFerroA3aA3a_kWh\tPerdaEnergiaTrafoA3aA3a_kWh\tPerdaEnergiaFerroA3aA4_kWh\tPerdaEnergiaTrafoA3aA4_kWh\tPerdaEnergiaFerroA3aB_kWh\tPerdaEnergiaTrafoA3aB_kWh\tPerdaEnergiaFerroA4A4_kWh\tPerdaEnergiaTrafoA4A4_kWh\tPerdaEnergiaFerroA4B_kWh\tPerdaEnergiaTrafoA4B_kWh\tPerdaEnergiaFerroA4A3a_kWh\tPerdaEnergiaTrafoA4A3a_kWh\tPerdaEnergiaFerroBA3a_kWh\tPerdaEnergiaTrafoBA3a_kWh\tPerdaEnergiaFerroBA4_kWh\tPerdaEnergiaTrafoBA4_kWh\tPerdaEnergiaLinhas_kWh\tPerdaEnergiaMT_kWh\tPerdaEnergiaBT_kWh\tPerdaEnergiaLinhasMT_kWh\tPerdaEnergiaLinhasBT_kWh\tPerdaEnergiaLinhasMTA3a_kWh\tPerdaEnergiaLinhasMTA4_kWh\tEnergiaForncMT_kWh\tEnergiaForncBT_kWh");
                this.mthdInputMensageLog("\t" + this.dblTotalPower_kW.ToString() + "\t" + this.dblTotalEnergy_kWh.ToString() + "\t" + this.dblTotalLoadEnergy_kWh.ToString() + "\t" + this.dblTecnicalLossPower_kW.ToString() + "\t" + this.dblTecnicalLossEnergy_kWh.ToString() + "\t" + this.dblNoLoadLosses_kWh.ToString() + "\t" + this.dblTransformerLosses_kWh.ToString() + "\t" + this.dblA3aA3aTransformerNoLoadLosses_kWh.ToString() + "\t" + this.dblA3aA3aTransformerLosses_kWh.ToString() + "\t" + this.dblA3aA4TransformerNoLoadLosses_kWh.ToString() + "\t" + this.dblA3aA4TransformerLosses_kWh.ToString() + "\t" + this.dblA3aBTransformerNoLoadLosses_kWh.ToString() + "\t" + this.dblA3aBTransformerLosses_kWh.ToString() + "\t" + this.dblA4A4TransformerNoLoadLosses_kWh.ToString() + "\t" + this.dblA4A4TransformerLosses_kWh.ToString() + "\t" + this.dblA4BTransformerNoLoadLosses_kWh.ToString() + "\t" + this.dblA4BTransformerLosses_kWh.ToString() + "\t" + this.dblA4A3aTransformerNoLoadLosses_kWh.ToString() + "\t" + this.dblA4A3aTransformerLosses_kWh.ToString() + "\t" + this.dblBA3aTransformerNoLoadLosses_kWh.ToString() + "\t" + this.dblBA3aTransformerLosses_kWh.ToString() + "\t" + this.dblBA4TransformerNoLoadLosses_kWh.ToString() + "\t" + this.dblBA4TransformerLosses_kWh.ToString() + "\t" + this.dblLineLosses_kWh.ToString() + "\t" + this.dblMTLosses_kWh.ToString() + "\t" + this.dblBTLosses_kWh.ToString() + "\t" + this.dblLineMTLosses_kWh.ToString() + "\t" + this.dblLineBTLosses_kWh.ToString() + "\t" + this.dblLineA3aLosses_kWh.ToString() + "\t" + this.dblLineA4Losses_kWh.ToString() + "\t" + this.dblMTLoad_kWh.ToString() + "\t" + this.dblBTLoad_kWh.ToString());
            }
            else
            {
                this.intConverged = 0;
                this.mthdInputMensageLog("Solução não convergiu.");
            }
        }

        private void mthdSolveCircuit()
        {
            if (this.dssSolution.Converged)
            {
                this.intConverged = 1;
                this.mthdInputMensageLog("Solução convergiu.");
            }
            else
            {
                this.intConverged = 0;
                this.mthdInputMensageLog("Solução não convergiu.");
            }
        }

        private void bSelectPathFile_Click(object sender, EventArgs e)
        {
            this.ofdSelectPathFileDSS.ShowDialog();
            this.tbPathFile.Text = this.ofdSelectPathFileDSS.FileName;
        }

        private void getSequenceVoltages()
        {
            Bus dssBus;
            int i, j;
            dynamic V, cpxV;
            String strTexto;

            for (i = 0; i < this.dssCircuit.NumBuses; i++)
            {
                dssBus = this.dssCircuit.Buses[i];
                strTexto = "\t" + this.dssCircuit.ActiveBus.Name;

                V = dssBus.SeqVoltages;
                cpxV = dssBus.CplxSeqVoltages;

                for (j = 0; j < V.Length; j++)
                {
                    strTexto += "\t" + V[j];
                }

                for (j = 0; j < cpxV.Length; j++)
                {
                    strTexto += "\t" + cpxV[j];
                }

                this.mthdInputMensageLog(strTexto);
            }
        }

        private void getVoltages()
        {
            Bus dssBus;
            int i, j;
            dynamic V;
            String strTexto;

            for (i = 0; i < this.dssCircuit.NumBuses; i++)
            {
                dssBus = this.dssCircuit.Buses[i];
                strTexto = "\t" + this.dssCircuit.ActiveBus.Name;

                V = dssBus.puVoltages;

                for (j = 0; j < V.Length; j++)
                {
                    strTexto += "\t" + V[j];
                }

                this.mthdInputMensageLog(strTexto);
            }
        }

        private void getMagnitudeAngle()
        {
            Bus dssBus;
            int i, j;
            dynamic V;
            String strTexto;

            for (i = 0; i < this.dssCircuit.NumBuses; i++)
            {
                dssBus = this.dssCircuit.Buses[i];
                strTexto = "\t" + this.dssCircuit.ActiveBus.Name;

                V = dssBus.puVoltages;

                for (j = 0; j < V.Length; j = j + 2)
                {
                    strTexto += "\t" + "(" + Math.Sqrt(V[j] * V[j] + V[j + 1] * V[j + 1]) + "; " + angleComplex(V[j], V[j + 1]) + ")";
                }

                this.mthdInputMensageLog(strTexto);
            }
        }

        public static double angleComplex(double Re, double Im)
        {
            double angle;
            if (Re != 0)
            {
                angle = Math.Atan(Im / Re) * 180 / Math.PI;
                if (Re < 0)
                {
                    angle = angle + 180;
                    return angle;
                }
                else
                    return angle;
            }
            else
            {
                if (Im > 0)
                    return 90;
                else if (Im < 0)
                    return -90;
                else
                    return 0;
            }
        }

        private void getSequenceCurrents()
        {
            CktElement dssElement;
            int i, j;
            dynamic SeqCurr, cpxSeqValues;
            String strTexto;

            dssElement = this.dssCircuit.ActiveCktElement;

            i = this.dssCircuit.FirstPDElement();

            while (i > 0)
            {
                if (dssElement.NumPhases == 3)
                {
                    strTexto = "\t" + dssElement.Name;

                    SeqCurr = dssElement.SeqCurrents;
                    cpxSeqValues = dssElement.CplxSeqCurrents;

                    for (j = 0; j < SeqCurr.Length; j++)
                    {
                        strTexto += "\t" + SeqCurr[j];
                    }

                    for (j = 0; j < cpxSeqValues.Length; j++)
                    {
                        strTexto += "\t" + cpxSeqValues[j];
                    }
                    this.mthdInputMensageLog(strTexto);
                }
                i = this.dssCircuit.NextPDElement();
            }
        }

        private void getLoadPowers()
        {
            CktElement dssElement;
            int i, j;
            dynamic PCPowers;
            double SumkW, Sumkvar;
            String strTexto;

            dssElement = this.dssCircuit.ActiveCktElement;

            i = this.dssCircuit.Loads.First;

            while (i > 0)
            {
                strTexto = "\t" + dssElement.Name;

                PCPowers = dssElement.Powers;

                SumkW = 0;
                Sumkvar = 0;

                for (j = 0; j < PCPowers.Length; j = j + 2)
                {
                    SumkW = SumkW + PCPowers[j];
                    Sumkvar = Sumkvar + PCPowers[j + 1];
                }

                strTexto += "\t" + "(" + SumkW + "; " + Sumkvar + ")";

                this.mthdInputMensageLog(strTexto);
                i = this.dssCircuit.Loads.Next;
            }
        }

        private void mthdGetInfoMeters()
        {
            int i = 0;
            Meters dssMeter;

            this.dblTotalPower_kW = 0;
            this.dblTotalEnergy_kWh = 0;
            this.dblTecnicalLossPower_kW = 0;
            this.dblTecnicalLossEnergy_kWh = 0;
            this.dblTotalLoadEnergy_kWh = 0;

            this.dblNoLoadLosses_kWh = 0;
            this.dblLineLosses_kWh = 0;

            this.dblTransformerLosses_kWh = 0;
            this.dblA3aA3aTransformerLosses_kWh = 0;
            this.dblA3aA4TransformerLosses_kWh = 0;
            this.dblA4A3aTransformerLosses_kWh = 0;
            this.dblA3aBTransformerLosses_kWh = 0;
            this.dblBA3aTransformerLosses_kWh = 0;
            this.dblA4A4TransformerLosses_kWh = 0;
            this.dblA4BTransformerLosses_kWh = 0;
            this.dblBA4TransformerLosses_kWh = 0;
            this.dblA3aA3aTransformerNoLoadLosses_kWh = 0;
            this.dblA3aA4TransformerNoLoadLosses_kWh = 0;
            this.dblA4A3aTransformerNoLoadLosses_kWh = 0;
            this.dblA3aBTransformerNoLoadLosses_kWh = 0;
            this.dblBA3aTransformerNoLoadLosses_kWh = 0;
            this.dblA4A4TransformerNoLoadLosses_kWh = 0;
            this.dblA4BTransformerNoLoadLosses_kWh = 0;
            this.dblBA4TransformerNoLoadLosses_kWh = 0;

            this.dblMTLosses_kWh = 0;
            this.dblA3aLosses_kWh = 0;
            this.dblA4Losses_kWh = 0;
            this.dblBTLosses_kWh = 0;

            this.dblLineMTLosses_kWh = 0;
            this.dblLineA3aLosses_kWh = 0;
            this.dblLineA4Losses_kWh = 0;
            this.dblLineBTLosses_kWh = 0;

            this.dblMTLoad_kWh = 0;
            this.dblBTLoad_kWh = 0;

            foreach (String strNomeMedidor in this.dssCircuit.Meters.AllNames)
            {
                this.dssCircuit.Meters.Name = strNomeMedidor;
                dssMeter = this.dssCircuit.Meters;
                i = 0;
                foreach (String strRegister in dssMeter.RegisterNames)
                {
                    if (GeoPerdasTools.mthdLeftString(dssMeter.Name, 6) == "busa3a")
                    {
                        if (dssMeter.RegisterNames[i] == "44 kV Losses" || dssMeter.RegisterNames[i] == "40 kV Losses" || dssMeter.RegisterNames[i] == "34.5 kV Losses" || dssMeter.RegisterNames[i] == "33 kV Losses")
                            this.dblA3aLosses_kWh += dssMeter.RegisterValues[i];
                        if (dssMeter.RegisterNames[i] == "44 kV Line Loss" || dssMeter.RegisterNames[i] == "40 kV Line Loss" || dssMeter.RegisterNames[i] == "34.5 kV Line Loss" || dssMeter.RegisterNames[i] == "33 kV Line Loss")
                            this.dblLineA3aLosses_kWh += dssMeter.RegisterValues[i];
                    }
                    if (GeoPerdasTools.mthdLeftString(dssMeter.Name, 6) == "busa4-")
                    {
                        if (dssMeter.RegisterNames[i] == "25 kV Losses" || dssMeter.RegisterNames[i] == "24.2 kV Losses" || dssMeter.RegisterNames[i] == "23.1 kV Losses" || dssMeter.RegisterNames[i] == "23 kV Losses" || dssMeter.RegisterNames[i] == "22 kV Losses" || dssMeter.RegisterNames[i] == "13.8 kV Losses" || dssMeter.RegisterNames[i] == "13.2 kV Losses" || dssMeter.RegisterNames[i] == "11.95 kV Losses" || dssMeter.RegisterNames[i] == "11.9 kV Losses" || dssMeter.RegisterNames[i] == "11.8 kV Losses" || dssMeter.RegisterNames[i] == "11.4 kV Losses" || dssMeter.RegisterNames[i] == "11 kV Losses" || dssMeter.RegisterNames[i] == "6.6 kV Losses" || dssMeter.RegisterNames[i] == "6.3 kV Losses" || dssMeter.RegisterNames[i] == "3.8 kV Losses")
                            this.dblA4Losses_kWh += dssMeter.RegisterValues[i];
                        if (dssMeter.RegisterNames[i] == "25 kV Line Loss" || dssMeter.RegisterNames[i] == "24.2 kV Line Loss" || dssMeter.RegisterNames[i] == "23.1 kV Line Loss" || dssMeter.RegisterNames[i] == "23 kV Line Loss" || dssMeter.RegisterNames[i] == "22 kV Line Loss" || dssMeter.RegisterNames[i] == "13.8 kV Line Loss" || dssMeter.RegisterNames[i] == "13.2 kV Line Loss" || dssMeter.RegisterNames[i] == "11.95 kV Line Loss" || dssMeter.RegisterNames[i] == "11.9 kV Line Loss" || dssMeter.RegisterNames[i] == "11.8 kV Line Loss" || dssMeter.RegisterNames[i] == "11.4 kV Line Loss" || dssMeter.RegisterNames[i] == "11 kV Line Loss" || dssMeter.RegisterNames[i] == "6.6 kV Line Loss" || dssMeter.RegisterNames[i] == "6.3 kV Line Loss" || dssMeter.RegisterNames[i] == "3.8 kV Line Loss")
                            this.dblLineA4Losses_kWh += dssMeter.RegisterValues[i];
                    }
                    if (GeoPerdasTools.mthdLeftString(dssMeter.Name, 7) == "ta3aa3a")
                    {
                        if (dssMeter.RegisterNames[i] == "44 kV Losses" || dssMeter.RegisterNames[i] == "40 kV Losses" || dssMeter.RegisterNames[i] == "34.5 kV Losses" || dssMeter.RegisterNames[i] == "33 kV Losses")
                            this.dblA3aLosses_kWh += dssMeter.RegisterValues[i];
                        if (dssMeter.RegisterNames[i] == "44 kV Line Loss" || dssMeter.RegisterNames[i] == "40 kV Line Loss" || dssMeter.RegisterNames[i] == "34.5 kV Line Loss" || dssMeter.RegisterNames[i] == "33 kV Line Loss")
                            this.dblLineA3aLosses_kWh += dssMeter.RegisterValues[i];
                        if (dssMeter.RegisterNames[i] == "Transformer Losses")
                            this.dblA3aA3aTransformerLosses_kWh += dssMeter.RegisterValues[i];
                        if (dssMeter.RegisterNames[i] == "No Load Losses kWh")
                            this.dblA3aA3aTransformerNoLoadLosses_kWh += dssMeter.RegisterValues[i];
                    }
                    if (GeoPerdasTools.mthdLeftString(dssMeter.Name, 7) == "ta3aa4-")
                    {
                        if (dssMeter.RegisterNames[i] == "44 kV Losses" || dssMeter.RegisterNames[i] == "40 kV Losses" || dssMeter.RegisterNames[i] == "34.5 kV Losses" || dssMeter.RegisterNames[i] == "33 kV Losses")
                            this.dblA3aLosses_kWh += dssMeter.RegisterValues[i];
                        if (dssMeter.RegisterNames[i] == "44 kV Line Loss" || dssMeter.RegisterNames[i] == "40 kV Line Loss" || dssMeter.RegisterNames[i] == "34.5 kV Line Loss" || dssMeter.RegisterNames[i] == "33 kV Line Loss")
                            this.dblLineA3aLosses_kWh += dssMeter.RegisterValues[i];
                        if (dssMeter.RegisterNames[i] == "Transformer Losses")
                            this.dblA3aA4TransformerLosses_kWh += dssMeter.RegisterValues[i];
                        if (dssMeter.RegisterNames[i] == "No Load Losses kWh")
                            this.dblA3aA4TransformerNoLoadLosses_kWh += dssMeter.RegisterValues[i];
                    }
                    if (GeoPerdasTools.mthdLeftString(dssMeter.Name, 7) == "ta3ab--")
                    {
                        if (dssMeter.RegisterNames[i] == "44 kV Losses" || dssMeter.RegisterNames[i] == "40 kV Losses" || dssMeter.RegisterNames[i] == "34.5 kV Losses" || dssMeter.RegisterNames[i] == "33 kV Losses")
                            this.dblA3aLosses_kWh += dssMeter.RegisterValues[i];
                        if (dssMeter.RegisterNames[i] == "44 kV Line Loss" || dssMeter.RegisterNames[i] == "40 kV Line Loss" || dssMeter.RegisterNames[i] == "34.5 kV Line Loss" || dssMeter.RegisterNames[i] == "33 kV Line Loss")
                            this.dblLineA3aLosses_kWh += dssMeter.RegisterValues[i];
                        if (dssMeter.RegisterNames[i] == "Transformer Losses")
                            this.dblA3aBTransformerLosses_kWh += dssMeter.RegisterValues[i];
                        if (dssMeter.RegisterNames[i] == "No Load Losses kWh")
                            this.dblA3aBTransformerNoLoadLosses_kWh += dssMeter.RegisterValues[i];
                    }
                    if (GeoPerdasTools.mthdLeftString(dssMeter.Name, 7) == "ta4-a3a")
                    {
                        if (dssMeter.RegisterNames[i] == "25 kV Losses" || dssMeter.RegisterNames[i] == "24.2 kV Losses" || dssMeter.RegisterNames[i] == "23.1 kV Losses" || dssMeter.RegisterNames[i] == "23 kV Losses" || dssMeter.RegisterNames[i] == "22 kV Losses" || dssMeter.RegisterNames[i] == "13.8 kV Losses" || dssMeter.RegisterNames[i] == "13.2 kV Losses" || dssMeter.RegisterNames[i] == "11.95 kV Losses" || dssMeter.RegisterNames[i] == "11.9 kV Losses" || dssMeter.RegisterNames[i] == "11.8 kV Losses" || dssMeter.RegisterNames[i] == "11.4 kV Losses" || dssMeter.RegisterNames[i] == "11 kV Losses" || dssMeter.RegisterNames[i] == "6.6 kV Losses" || dssMeter.RegisterNames[i] == "6.3 kV Losses" || dssMeter.RegisterNames[i] == "3.8 kV Losses")
                            this.dblA4Losses_kWh += dssMeter.RegisterValues[i];
                        if (dssMeter.RegisterNames[i] == "25 kV Line Loss" || dssMeter.RegisterNames[i] == "24.2 kV Line Loss" || dssMeter.RegisterNames[i] == "23.1 kV Line Loss" || dssMeter.RegisterNames[i] == "23 kV Line Loss" || dssMeter.RegisterNames[i] == "22 kV Line Loss" || dssMeter.RegisterNames[i] == "13.8 kV Line Loss" || dssMeter.RegisterNames[i] == "13.2 kV Line Loss" || dssMeter.RegisterNames[i] == "11.95 kV Line Loss" || dssMeter.RegisterNames[i] == "11.9 kV Line Loss" || dssMeter.RegisterNames[i] == "11.8 kV Line Loss" || dssMeter.RegisterNames[i] == "11.4 kV Line Loss" || dssMeter.RegisterNames[i] == "11 kV Line Loss" || dssMeter.RegisterNames[i] == "6.6 kV Line Loss" || dssMeter.RegisterNames[i] == "6.3 kV Line Loss" || dssMeter.RegisterNames[i] == "3.8 kV Line Loss")
                            this.dblLineA4Losses_kWh += dssMeter.RegisterValues[i];
                        if (dssMeter.RegisterNames[i] == "Transformer Losses")
                            this.dblA4A3aTransformerLosses_kWh += dssMeter.RegisterValues[i];
                        if (dssMeter.RegisterNames[i] == "No Load Losses kWh")
                            this.dblA4A3aTransformerNoLoadLosses_kWh += dssMeter.RegisterValues[i];
                    }
                    if (GeoPerdasTools.mthdLeftString(dssMeter.Name, 7) == "tb--a3a")
                    {
                        if (dssMeter.RegisterNames[i] == "44 kV Losses" || dssMeter.RegisterNames[i] == "40 kV Losses" || dssMeter.RegisterNames[i] == "34.5 kV Losses" || dssMeter.RegisterNames[i] == "33 kV Losses")
                            this.dblA3aLosses_kWh += dssMeter.RegisterValues[i];
                        if (dssMeter.RegisterNames[i] == "44 kV Line Loss" || dssMeter.RegisterNames[i] == "40 kV Line Loss" || dssMeter.RegisterNames[i] == "34.5 kV Line Loss" || dssMeter.RegisterNames[i] == "33 kV Line Loss")
                            this.dblLineA3aLosses_kWh += dssMeter.RegisterValues[i];
                        if (dssMeter.RegisterNames[i] == "Transformer Losses")
                            this.dblBA3aTransformerLosses_kWh += dssMeter.RegisterValues[i];
                        if (dssMeter.RegisterNames[i] == "No Load Losses kWh")
                            this.dblBA3aTransformerNoLoadLosses_kWh += dssMeter.RegisterValues[i];
                    }
                    if (GeoPerdasTools.mthdLeftString(dssMeter.Name, 7) == "ta4-a4-")
                    {
                        if (dssMeter.RegisterNames[i] == "25 kV Losses" || dssMeter.RegisterNames[i] == "24.2 kV Losses" || dssMeter.RegisterNames[i] == "23.1 kV Losses" || dssMeter.RegisterNames[i] == "23 kV Losses" || dssMeter.RegisterNames[i] == "22 kV Losses" || dssMeter.RegisterNames[i] == "13.8 kV Losses" || dssMeter.RegisterNames[i] == "13.2 kV Losses" || dssMeter.RegisterNames[i] == "11.95 kV Losses" || dssMeter.RegisterNames[i] == "11.9 kV Losses" || dssMeter.RegisterNames[i] == "11.8 kV Losses" || dssMeter.RegisterNames[i] == "11.4 kV Losses" || dssMeter.RegisterNames[i] == "11 kV Losses" || dssMeter.RegisterNames[i] == "6.6 kV Losses" || dssMeter.RegisterNames[i] == "6.3 kV Losses" || dssMeter.RegisterNames[i] == "3.8 kV Losses")
                            this.dblA4Losses_kWh += dssMeter.RegisterValues[i];
                        if (dssMeter.RegisterNames[i] == "25 kV Line Loss" || dssMeter.RegisterNames[i] == "24.2 kV Line Loss" || dssMeter.RegisterNames[i] == "23.1 kV Line Loss" || dssMeter.RegisterNames[i] == "23 kV Line Loss" || dssMeter.RegisterNames[i] == "22 kV Line Loss" || dssMeter.RegisterNames[i] == "13.8 kV Line Loss" || dssMeter.RegisterNames[i] == "13.2 kV Line Loss" || dssMeter.RegisterNames[i] == "11.95 kV Line Loss" || dssMeter.RegisterNames[i] == "11.9 kV Line Loss" || dssMeter.RegisterNames[i] == "11.8 kV Line Loss" || dssMeter.RegisterNames[i] == "11.4 kV Line Loss" || dssMeter.RegisterNames[i] == "11 kV Line Loss" || dssMeter.RegisterNames[i] == "6.6 kV Line Loss" || dssMeter.RegisterNames[i] == "6.3 kV Line Loss" || dssMeter.RegisterNames[i] == "3.8 kV Line Loss")
                            this.dblLineA4Losses_kWh += dssMeter.RegisterValues[i];
                        if (dssMeter.RegisterNames[i] == "Transformer Losses")
                            this.dblA4A4TransformerLosses_kWh += dssMeter.RegisterValues[i];
                        if (dssMeter.RegisterNames[i] == "No Load Losses kWh")
                            this.dblA4A4TransformerNoLoadLosses_kWh += dssMeter.RegisterValues[i];
                    }
                    if (GeoPerdasTools.mthdLeftString(dssMeter.Name, 7) == "ta4-b--")
                    {
                        if (dssMeter.RegisterNames[i] == "25 kV Losses" || dssMeter.RegisterNames[i] == "24.2 kV Losses" || dssMeter.RegisterNames[i] == "23.1 kV Losses" || dssMeter.RegisterNames[i] == "23 kV Losses" || dssMeter.RegisterNames[i] == "22 kV Losses" || dssMeter.RegisterNames[i] == "13.8 kV Losses" || dssMeter.RegisterNames[i] == "13.2 kV Losses" || dssMeter.RegisterNames[i] == "11.95 kV Losses" || dssMeter.RegisterNames[i] == "11.9 kV Losses" || dssMeter.RegisterNames[i] == "11.8 kV Losses" || dssMeter.RegisterNames[i] == "11.4 kV Losses" || dssMeter.RegisterNames[i] == "11 kV Losses" || dssMeter.RegisterNames[i] == "6.6 kV Losses" || dssMeter.RegisterNames[i] == "6.3 kV Losses" || dssMeter.RegisterNames[i] == "3.8 kV Losses")
                            this.dblA4Losses_kWh += dssMeter.RegisterValues[i];
                        if (dssMeter.RegisterNames[i] == "25 kV Line Loss" || dssMeter.RegisterNames[i] == "24.2 kV Line Loss" || dssMeter.RegisterNames[i] == "23.1 kV Line Loss" || dssMeter.RegisterNames[i] == "23 kV Line Loss" || dssMeter.RegisterNames[i] == "22 kV Line Loss" || dssMeter.RegisterNames[i] == "13.8 kV Line Loss" || dssMeter.RegisterNames[i] == "13.2 kV Line Loss" || dssMeter.RegisterNames[i] == "11.95 kV Line Loss" || dssMeter.RegisterNames[i] == "11.9 kV Line Loss" || dssMeter.RegisterNames[i] == "11.8 kV Line Loss" || dssMeter.RegisterNames[i] == "11.4 kV Line Loss" || dssMeter.RegisterNames[i] == "11 kV Line Loss" || dssMeter.RegisterNames[i] == "6.6 kV Line Loss" || dssMeter.RegisterNames[i] == "6.3 kV Line Loss" || dssMeter.RegisterNames[i] == "3.8 kV Line Loss")
                            this.dblLineA4Losses_kWh += dssMeter.RegisterValues[i];
                        if (dssMeter.RegisterNames[i] == "Transformer Losses")
                            this.dblA4BTransformerLosses_kWh += dssMeter.RegisterValues[i];
                        if (dssMeter.RegisterNames[i] == "No Load Losses kWh")
                            this.dblA4BTransformerNoLoadLosses_kWh += dssMeter.RegisterValues[i];
                    }
                    if (GeoPerdasTools.mthdLeftString(dssMeter.Name, 7) == "tb--a4-")
                    {
                        if (dssMeter.RegisterNames[i] == "25 kV Losses" || dssMeter.RegisterNames[i] == "24.2 kV Losses" || dssMeter.RegisterNames[i] == "23.1 kV Losses" || dssMeter.RegisterNames[i] == "23 kV Losses" || dssMeter.RegisterNames[i] == "22 kV Losses" || dssMeter.RegisterNames[i] == "13.8 kV Losses" || dssMeter.RegisterNames[i] == "13.2 kV Losses" || dssMeter.RegisterNames[i] == "11.95 kV Losses" || dssMeter.RegisterNames[i] == "11.9 kV Losses" || dssMeter.RegisterNames[i] == "11.8 kV Losses" || dssMeter.RegisterNames[i] == "11.4 kV Losses" || dssMeter.RegisterNames[i] == "11 kV Losses" || dssMeter.RegisterNames[i] == "6.6 kV Losses" || dssMeter.RegisterNames[i] == "6.3 kV Losses" || dssMeter.RegisterNames[i] == "3.8 kV Losses")
                            this.dblA4Losses_kWh += dssMeter.RegisterValues[i];
                        if (dssMeter.RegisterNames[i] == "25 kV Line Loss" || dssMeter.RegisterNames[i] == "24.2 kV Line Loss" || dssMeter.RegisterNames[i] == "23.1 kV Line Loss" || dssMeter.RegisterNames[i] == "23 kV Line Loss" || dssMeter.RegisterNames[i] == "22 kV Line Loss" || dssMeter.RegisterNames[i] == "13.8 kV Line Loss" || dssMeter.RegisterNames[i] == "13.2 kV Line Loss" || dssMeter.RegisterNames[i] == "11.95 kV Line Loss" || dssMeter.RegisterNames[i] == "11.9 kV Line Loss" || dssMeter.RegisterNames[i] == "11.8 kV Line Loss" || dssMeter.RegisterNames[i] == "11.4 kV Line Loss" || dssMeter.RegisterNames[i] == "11 kV Line Loss" || dssMeter.RegisterNames[i] == "6.6 kV Line Loss" || dssMeter.RegisterNames[i] == "6.3 kV Line Loss" || dssMeter.RegisterNames[i] == "3.8 kV Line Loss")
                            this.dblLineA4Losses_kWh += dssMeter.RegisterValues[i];
                        if (dssMeter.RegisterNames[i] == "Transformer Losses")
                            this.dblBA4TransformerLosses_kWh += dssMeter.RegisterValues[i];
                        if (dssMeter.RegisterNames[i] == "No Load Losses kWh")
                            this.dblBA4TransformerNoLoadLosses_kWh += dssMeter.RegisterValues[i];
                    }
                    if (GeoPerdasTools.mthdLeftString(dssMeter.Name, 7) == "la3aa3a")
                    {
                        if (dssMeter.RegisterNames[i] == "44 kV Losses" || dssMeter.RegisterNames[i] == "40 kV Losses" || dssMeter.RegisterNames[i] == "34.5 kV Losses" || dssMeter.RegisterNames[i] == "33 kV Losses")
                            this.dblA3aLosses_kWh += dssMeter.RegisterValues[i];
                        if (dssMeter.RegisterNames[i] == "44 kV Line Loss" || dssMeter.RegisterNames[i] == "40 kV Line Loss" || dssMeter.RegisterNames[i] == "34.5 kV Line Loss" || dssMeter.RegisterNames[i] == "33 kV Line Loss")
                            this.dblLineA3aLosses_kWh += dssMeter.RegisterValues[i];
                    }
                    if (GeoPerdasTools.mthdLeftString(dssMeter.Name, 7) == "la3aa4-")
                    {
                        if (dssMeter.RegisterNames[i] == "25 kV Losses" || dssMeter.RegisterNames[i] == "24.2 kV Losses" || dssMeter.RegisterNames[i] == "23.1 kV Losses" || dssMeter.RegisterNames[i] == "23 kV Losses" || dssMeter.RegisterNames[i] == "22 kV Losses" || dssMeter.RegisterNames[i] == "13.8 kV Losses" || dssMeter.RegisterNames[i] == "13.2 kV Losses" || dssMeter.RegisterNames[i] == "11.95 kV Losses" || dssMeter.RegisterNames[i] == "11.9 kV Losses" || dssMeter.RegisterNames[i] == "11.8 kV Losses" || dssMeter.RegisterNames[i] == "11.4 kV Losses" || dssMeter.RegisterNames[i] == "11 kV Losses" || dssMeter.RegisterNames[i] == "6.6 kV Losses" || dssMeter.RegisterNames[i] == "6.3 kV Losses" || dssMeter.RegisterNames[i] == "3.8 kV Losses")
                            this.dblA4Losses_kWh += dssMeter.RegisterValues[i];
                        if (dssMeter.RegisterNames[i] == "25 kV Line Loss" || dssMeter.RegisterNames[i] == "24.2 kV Line Loss" || dssMeter.RegisterNames[i] == "23.1 kV Line Loss" || dssMeter.RegisterNames[i] == "23 kV Line Loss" || dssMeter.RegisterNames[i] == "22 kV Line Loss" || dssMeter.RegisterNames[i] == "13.8 kV Line Loss" || dssMeter.RegisterNames[i] == "13.2 kV Line Loss" || dssMeter.RegisterNames[i] == "11.95 kV Line Loss" || dssMeter.RegisterNames[i] == "11.9 kV Line Loss" || dssMeter.RegisterNames[i] == "11.8 kV Line Loss" || dssMeter.RegisterNames[i] == "11.4 kV Line Loss" || dssMeter.RegisterNames[i] == "11 kV Line Loss" || dssMeter.RegisterNames[i] == "6.6 kV Line Loss" || dssMeter.RegisterNames[i] == "6.3 kV Line Loss" || dssMeter.RegisterNames[i] == "3.8 kV Line Loss")
                            this.dblLineA4Losses_kWh += dssMeter.RegisterValues[i];
                    }
                    if (GeoPerdasTools.mthdLeftString(dssMeter.Name, 7) == "la4-a3a")
                    {
                        if (dssMeter.RegisterNames[i] == "44 kV Losses" || dssMeter.RegisterNames[i] == "40 kV Losses" || dssMeter.RegisterNames[i] == "34.5 kV Losses" || dssMeter.RegisterNames[i] == "33 kV Losses")
                            this.dblA3aLosses_kWh += dssMeter.RegisterValues[i];
                        if (dssMeter.RegisterNames[i] == "44 kV Line Loss" || dssMeter.RegisterNames[i] == "40 kV Line Loss" || dssMeter.RegisterNames[i] == "34.5 kV Line Loss" || dssMeter.RegisterNames[i] == "33 kV Line Loss")
                            this.dblLineA3aLosses_kWh += dssMeter.RegisterValues[i];
                    }
                    if (GeoPerdasTools.mthdLeftString(dssMeter.Name, 7) == "lb--a3a")
                    {
                        if (dssMeter.RegisterNames[i] == "44 kV Losses" || dssMeter.RegisterNames[i] == "40 kV Losses" || dssMeter.RegisterNames[i] == "34.5 kV Losses" || dssMeter.RegisterNames[i] == "33 kV Losses")
                            this.dblA3aLosses_kWh += dssMeter.RegisterValues[i];
                        if (dssMeter.RegisterNames[i] == "44 kV Line Loss" || dssMeter.RegisterNames[i] == "40 kV Line Loss" || dssMeter.RegisterNames[i] == "34.5 kV Line Loss" || dssMeter.RegisterNames[i] == "33 kV Line Loss")
                            this.dblLineA3aLosses_kWh += dssMeter.RegisterValues[i];
                    }
                    if (GeoPerdasTools.mthdLeftString(dssMeter.Name, 7) == "la4-a4-")
                    {
                        if (dssMeter.RegisterNames[i] == "25 kV Losses" || dssMeter.RegisterNames[i] == "24.2 kV Losses" || dssMeter.RegisterNames[i] == "23.1 kV Losses" || dssMeter.RegisterNames[i] == "23 kV Losses" || dssMeter.RegisterNames[i] == "22 kV Losses" || dssMeter.RegisterNames[i] == "13.8 kV Losses" || dssMeter.RegisterNames[i] == "13.2 kV Losses" || dssMeter.RegisterNames[i] == "11.95 kV Losses" || dssMeter.RegisterNames[i] == "11.9 kV Losses" || dssMeter.RegisterNames[i] == "11.8 kV Losses" || dssMeter.RegisterNames[i] == "11.4 kV Losses" || dssMeter.RegisterNames[i] == "11 kV Losses" || dssMeter.RegisterNames[i] == "6.6 kV Losses" || dssMeter.RegisterNames[i] == "6.3 kV Losses" || dssMeter.RegisterNames[i] == "3.8 kV Losses")
                            this.dblA4Losses_kWh += dssMeter.RegisterValues[i];
                        if (dssMeter.RegisterNames[i] == "25 kV Line Loss" || dssMeter.RegisterNames[i] == "24.2 kV Line Loss" || dssMeter.RegisterNames[i] == "23.1 kV Line Loss" || dssMeter.RegisterNames[i] == "23 kV Line Loss" || dssMeter.RegisterNames[i] == "22 kV Line Loss" || dssMeter.RegisterNames[i] == "13.8 kV Line Loss" || dssMeter.RegisterNames[i] == "13.2 kV Line Loss" || dssMeter.RegisterNames[i] == "11.95 kV Line Loss" || dssMeter.RegisterNames[i] == "11.9 kV Line Loss" || dssMeter.RegisterNames[i] == "11.8 kV Line Loss" || dssMeter.RegisterNames[i] == "11.4 kV Line Loss" || dssMeter.RegisterNames[i] == "11 kV Line Loss" || dssMeter.RegisterNames[i] == "6.6 kV Line Loss" || dssMeter.RegisterNames[i] == "6.3 kV Line Loss" || dssMeter.RegisterNames[i] == "3.8 kV Line Loss")
                            this.dblLineA4Losses_kWh += dssMeter.RegisterValues[i];
                    }
                    if (GeoPerdasTools.mthdLeftString(dssMeter.Name, 7) == "lb--a4-")
                    {
                        if (dssMeter.RegisterNames[i] == "25 kV Losses" || dssMeter.RegisterNames[i] == "24.2 kV Losses" || dssMeter.RegisterNames[i] == "23.1 kV Losses" || dssMeter.RegisterNames[i] == "23 kV Losses" || dssMeter.RegisterNames[i] == "22 kV Losses" || dssMeter.RegisterNames[i] == "13.8 kV Losses" || dssMeter.RegisterNames[i] == "13.2 kV Losses" || dssMeter.RegisterNames[i] == "11.95 kV Losses" || dssMeter.RegisterNames[i] == "11.9 kV Losses" || dssMeter.RegisterNames[i] == "11.8 kV Losses" || dssMeter.RegisterNames[i] == "11.4 kV Losses" || dssMeter.RegisterNames[i] == "11 kV Losses" || dssMeter.RegisterNames[i] == "6.6 kV Losses" || dssMeter.RegisterNames[i] == "6.3 kV Losses" || dssMeter.RegisterNames[i] == "3.8 kV Losses")
                            this.dblA4Losses_kWh += dssMeter.RegisterValues[i];
                        if (dssMeter.RegisterNames[i] == "25 kV Line Loss" || dssMeter.RegisterNames[i] == "24.2 kV Line Loss" || dssMeter.RegisterNames[i] == "23.1 kV Line Loss" || dssMeter.RegisterNames[i] == "23 kV Line Loss" || dssMeter.RegisterNames[i] == "22 kV Line Loss" || dssMeter.RegisterNames[i] == "13.8 kV Line Loss" || dssMeter.RegisterNames[i] == "13.2 kV Line Loss" || dssMeter.RegisterNames[i] == "11.95 kV Line Loss" || dssMeter.RegisterNames[i] == "11.9 kV Line Loss" || dssMeter.RegisterNames[i] == "11.8 kV Line Loss" || dssMeter.RegisterNames[i] == "11.4 kV Line Loss" || dssMeter.RegisterNames[i] == "11 kV Line Loss" || dssMeter.RegisterNames[i] == "6.6 kV Line Loss" || dssMeter.RegisterNames[i] == "6.3 kV Line Loss" || dssMeter.RegisterNames[i] == "3.8 kV Line Loss")
                            this.dblLineA4Losses_kWh += dssMeter.RegisterValues[i];
                    }

                    if (strRegister == "Zone Max kW")
                        this.dblTotalPower_kW += dssMeter.RegisterValues[i];
                    if (strRegister == "Zone kWh" || strRegister == "Zone Losses kWh")
                        this.dblTotalEnergy_kWh += dssMeter.RegisterValues[i];
                    if (strRegister == "Zone Max kW Losses")
                        this.dblTecnicalLossPower_kW += dssMeter.RegisterValues[i];
                    if (strRegister == "Zone Losses kWh")
                        this.dblTecnicalLossEnergy_kWh += dssMeter.RegisterValues[i];
                    if (i == 60 || i == 61 || i == 62 || i == 63 || i == 64 || i == 65 || i == 66)
                        this.dblTotalLoadEnergy_kWh += dssMeter.RegisterValues[i];
                    if (strRegister == "No Load Losses kWh")
                        this.dblNoLoadLosses_kWh += dssMeter.RegisterValues[i];
                    if (strRegister == "Line Losses")
                        this.dblLineLosses_kWh += dssMeter.RegisterValues[i];
                    if (strRegister == "Transformer Losses")
                        this.dblTransformerLosses_kWh += dssMeter.RegisterValues[i];
                    if (strRegister == "44 kV Losses" || strRegister == "40 kV Losses" || strRegister == "34.5 kV Losses" || strRegister == "33 kV Losses" || strRegister == "25 kV Losses" || strRegister == "24.2 kV Losses" || strRegister == "23.1 kV Losses" || strRegister == "23 kV Losses" || strRegister == "22 kV Losses" || strRegister == "13.8 kV Losses" || strRegister == "13.2 kV Losses" || strRegister == "11.95 kV Losses" || strRegister == "11.9 kV Losses" || strRegister == "11.8 kV Losses" || strRegister == "11.4 kV Losses" || strRegister == "11 kV Losses" || strRegister == "6.6 kV Losses" || strRegister == "6.3 kV Losses" || strRegister == "3.8 kV Losses")
                        this.dblMTLosses_kWh += dssMeter.RegisterValues[i];
                    if (strRegister == "0.44 kV Losses" || strRegister == "0.4 kV Losses" || strRegister == "0.38 kV Losses" || strRegister == "0.254 kV Losses" || strRegister == "0.24 kV Losses" || strRegister == "0.231 kV Losses" || strRegister == "0.23 kV Losses" || strRegister == "0.22 kV Losses" || strRegister == "0.19 kV Losses" || strRegister == "0.127 kV Losses" || strRegister == "0.12 kV Losses" || strRegister == "0.115 kV Losses" || strRegister == "0.11 kV Losses")
                        this.dblBTLosses_kWh += dssMeter.RegisterValues[i];
                    if (strRegister == "44 kV Line Loss" || strRegister == "40 kV Line Loss" || strRegister == "34.5 kV Line Loss" || strRegister == "33 kV Line Loss" || strRegister == "25 kV Line Loss" || strRegister == "24.2 kV Line Loss" || strRegister == "23.1 kV Line Loss" || strRegister == "23 kV Line Loss" || strRegister == "22 kV Line Loss" || strRegister == "13.8 kV Line Loss" || strRegister == "13.2 kV Line Loss" || strRegister == "11.95 kV Line Loss" || strRegister == "11.9 kV Line Loss" || strRegister == "11.8 kV Line Loss" || strRegister == "11.4 kV Line Loss" || strRegister == "11 kV Line Loss" || strRegister == "6.6 kV Line Loss" || strRegister == "6.3 kV Line Loss" || strRegister == "3.8 kV Line Loss")
                        this.dblLineMTLosses_kWh += dssMeter.RegisterValues[i];
                    if (strRegister == "0.44 kV Line Loss" || strRegister == "0.4 kV Line Loss" || strRegister == "0.38 kV Line Loss" || strRegister == "0.254 kV Line Loss" || strRegister == "0.24 kV Line Loss" || strRegister == "0.231 kV Line Loss" || strRegister == "0.23 kV Line Loss" || strRegister == "0.22 kV Line Loss" || strRegister == "0.19 kV Line Loss" || strRegister == "0.127 kV Line Loss" || strRegister == "0.12 kV Line Loss" || strRegister == "0.115 kV Line Loss" || strRegister == "0.11 kV Line Loss")
                        this.dblLineBTLosses_kWh += dssMeter.RegisterValues[i];
                    if (strRegister == "44 kV Load Energy" || strRegister == "40 kV Load Energy" || strRegister == "34.5 kV Load Energy" || strRegister == "33 kV Load Energy" || strRegister == "25 kV Load Energy" || strRegister == "24.2 kV Load Energy" || strRegister == "23.1 kV Load Energy" || strRegister == "23 kV Load Energy" || strRegister == "22 kV Load Energy" || strRegister == "13.8 kV Load Energy" || strRegister == "13.2 kV Load Energy" || strRegister == "11.95 kV Load Energy" || strRegister == "11.9 kV Load Energy" || strRegister == "11.8 kV Load Energy" || strRegister == "11.4 kV Load Energy" || strRegister == "11 kV Load Energy" || strRegister == "6.6 kV Load Energy" || strRegister == "6.3 kV Load Energy" || strRegister == "3.8 kV Load Energy")
                        this.dblMTLoad_kWh += dssMeter.RegisterValues[i];
                    if (strRegister == "0.44 kV Load Energy" || strRegister == "0.4 kV Load Energy" || strRegister == "0.38 kV Load Energy" || strRegister == "0.254 kV Load Energy" || strRegister == "0.24 kV Load Energy" || strRegister == "0.231 kV Load Energy" || strRegister == "0.23 kV Load Energy" || strRegister == "0.22 kV Load Energy" || strRegister == "0.19 kV Load Energy" || strRegister == "0.127 kV Load Energy" || strRegister == "0.12 kV Load Energy" || strRegister == "0.115 kV Load Energy" || strRegister == "0.11 kV Load Energy")
                        this.dblBTLoad_kWh += dssMeter.RegisterValues[i];
                    i++;
                }
            }
        }

        private void mthdGetDSSCommandCircuit(String strName, double dblBase_kV, double dblOperation_pu, String strBus, int intConstraintOperation)
        {
            this.mthdInsertCommandInLine(GeoPerdasTools.mthdWriteLanguageDSSCommandCircuit(strName, dblBase_kV, dblOperation_pu, strBus, intConstraintOperation));
        }

        private String mthdWriteLanguageDSSCommandLinecode(String strName, double dblResis_ohms_km, double dblReat_ohms_km, double dblCorrMax_A)
        {
            String result;

            result = "New \"Linecode." + strName + "_1\" nphases=1 basefreq=60 r1=" + dblResis_ohms_km.ToString().Replace(",", ".") + " x1=" + dblReat_ohms_km.ToString().Replace(",", ".") + " units=km normamps=" + dblCorrMax_A.ToString().Replace(",", ".") + Environment.NewLine;
            result += "New \"Linecode." + strName + "_2\" nphases=2 basefreq=60 r1=" + dblResis_ohms_km.ToString().Replace(",", ".") + " x1=" + dblReat_ohms_km.ToString().Replace(",", ".") + " units=km normamps=" + dblCorrMax_A.ToString().Replace(",", ".") + Environment.NewLine;
            result += "New \"Linecode." + strName + "_3\" nphases=3 basefreq=60 r1=" + dblResis_ohms_km.ToString().Replace(",", ".") + " x1=" + dblReat_ohms_km.ToString().Replace(",", ".") + " units=km normamps=" + dblCorrMax_A.ToString().Replace(",", ".") + Environment.NewLine;
            result += "New \"Linecode." + strName + "_4\" nphases=4 basefreq=60 r1=" + dblResis_ohms_km.ToString().Replace(",", ".") + " x1=" + dblReat_ohms_km.ToString().Replace(",", ".") + " units=km normamps=" + dblCorrMax_A.ToString().Replace(",", ".");

            return result;
        }

        private void mthdGetDSSCommandLinecode(String strName, double dblResis_ohms_km, double dblReat_ohms_km, double dblCorrMax_A)
        {
            this.mthdInsertCommandInLine("New \"Linecode." + strName + "_1\" nphases=1 basefreq=60 r1=" + dblResis_ohms_km.ToString().Replace(",", ".") + " x1=" + dblReat_ohms_km.ToString().Replace(",", ".") + " units=km normamps=" + dblCorrMax_A.ToString().Replace(",", "."));
            this.mthdInsertCommandInLine("New \"Linecode." + strName + "_2\" nphases=2 basefreq=60 r1=" + dblResis_ohms_km.ToString().Replace(",", ".") + " x1=" + dblReat_ohms_km.ToString().Replace(",", ".") + " units=km normamps=" + dblCorrMax_A.ToString().Replace(",", "."));
            this.mthdInsertCommandInLine("New \"Linecode." + strName + "_3\" nphases=3 basefreq=60 r1=" + dblResis_ohms_km.ToString().Replace(",", ".") + " x1=" + dblReat_ohms_km.ToString().Replace(",", ".") + " units=km normamps=" + dblCorrMax_A.ToString().Replace(",", "."));
            this.mthdInsertCommandInLine("New \"Linecode." + strName + "_4\" nphases=4 basefreq=60 r1=" + dblResis_ohms_km.ToString().Replace(",", ".") + " x1=" + dblReat_ohms_km.ToString().Replace(",", ".") + " units=km normamps=" + dblCorrMax_A.ToString().Replace(",", "."));
        }

        private void mthdGetDSSCommandMTSwitch(String strName, String strCodFas, String strBus1, String strBus2)
        {
            this.mthdInsertCommandInLine(GeoPerdasTools.mthdWriteLanguageDSSCommandMTSwitch(strName, strCodFas, strBus1, strBus2));
        }

        private void mthdGetDSSCommandLineMT(String strName, String strCodFas, String strBus1, String strBus2, String strCodCond, double dblComp_km, String strProperty, int intConstraintLength, int intConstraintProperty)
        {
            this.mthdInsertCommandInLine(GeoPerdasTools.mthdWriteLanguageDSSCommandLineMT(strName, strCodFas, strBus1, strBus2, strCodCond, dblComp_km, strProperty, intConstraintLength, intConstraintProperty));
        }

        private void mthdGetDSSCommandBTSwitch(String strName, String strCodFas, String strBus1, String strBus2)
        {
            this.mthdInsertCommandInLine(GeoPerdasTools.mthdWriteLanguageDSSCommandBTSwitch(strName, strCodFas, strBus1, strBus2));
        }

        private void mthdGetDSSCommandLineBT(String strName, String strCodFas, String strBus1, String strBus2, String strCodCond, double dblComp_km, String strProperty, int intConstraintLength, int intConstraintProperty)
        {
            this.mthdInsertCommandInLine(GeoPerdasTools.mthdWriteLanguageDSSCommandLineBT(strName, strCodFas, strBus1, strBus2, strCodCond, dblComp_km, strProperty, intConstraintLength, intConstraintProperty));
        }

        private String mthdWriteLanguageDSSCommandRegulatorMT(String strName, int intCodBnc, int intTipRegul, String strCodFasPrim, String strCodFasSecu, String strCodCoinc, String strBus1, String strBus2, double dblTensaoPrimTrafo_kV, double PotNom_kVA, double dblTenRgl_pu, double ReatHL_per, double dblPerdTtl_per, double dblPerdVz_per)
        {
            String result;

            result = "New \"Transformer." + strName + GeoPerdasTools.mthdNamingBank(intCodBnc) + "\" phases=" + GeoPerdasTools.mthdNumberPhasesRegulators(intTipRegul, strCodFasPrim) + " windings=2 buses=[\"" + strBus1 + GeoPerdasTools.mthdPhasing(strCodFasPrim) + "\" \"" + strBus2 + GeoPerdasTools.mthdPhasing(strCodFasSecu) + "\"] conns=[" + this.mthdConexTrafo(strCodFasPrim) + " " + this.mthdConexTrafo(strCodFasSecu) + "] kvs=[" + this.mthdTensaoEnrolamento(strCodFasPrim, dblTensaoPrimTrafo_kV).ToString().Replace(",", ".") + " " + this.mthdTensaoEnrolamento(strCodFasSecu, dblTensaoPrimTrafo_kV).ToString().Replace(",", ".") + "] kvas=[" + PotNom_kVA.ToString().Replace(",", ".") + " " + PotNom_kVA.ToString().Replace(",", ".") + "] xhl=" + ReatHL_per.ToString().Replace(",", ".") + " %loadloss=" + (dblPerdTtl_per - dblPerdVz_per).ToString().Replace(",", ".") + " %noloadloss=" + dblPerdVz_per.ToString().Replace(",", ".") + Environment.NewLine;
            result += "New \"Regcontrol." + strName + GeoPerdasTools.mthdNamingBank(intCodBnc) + "\" transformer=\"" + strName + GeoPerdasTools.mthdNamingBank(intCodBnc) + "\" winding=2 vreg=" + (dblTenRgl_pu * 100).ToString().Replace(",", ".") + " band=2 ptratio=" + (this.mthdTensaoEnrolamento(strCodFasSecu, dblTensaoPrimTrafo_kV) * 10).ToString().Replace(",", ".");
            if (intTipRegul == 2 && intCodBnc == 2)
                result += Environment.NewLine + "New \"Line." + strName + "_J\" phases=" + GeoPerdasTools.mthdNumberPhasesTransformers(strCodCoinc) + " bus1=\"" + strBus1 + GeoPerdasTools.mthdPhasing(strCodCoinc) + "\" bus2=\"" + strBus2 + GeoPerdasTools.mthdPhasing(strCodCoinc) + "\" r0=1e-3  r1=1e-3  x0=0  x1=0  c0=0 c1=0";

            return result;
        }

        private void mthdGetDSSCommandRegulatorMT(String strName, int intCodBnc, int intTipRegul, String strCodFasPrim, String strCodFasSecu, String strCodCoinc, String strBus1, String strBus2, double dblTensaoPrimTrafo_kV, double PotNom_kVA, double dblTenRgl_pu, double ReatHL_per, double dblPerdTtl_per, double dblPerdVz_per)
        {
            this.mthdInsertCommandInLine("New \"Transformer." + strName + GeoPerdasTools.mthdNamingBank(intCodBnc) + "\" phases=" + GeoPerdasTools.mthdNumberPhasesRegulators(intTipRegul, strCodFasPrim) + " windings=2 buses=[\"" + strBus1 + GeoPerdasTools.mthdPhasing(strCodFasPrim) + "\" \"" + strBus2 + GeoPerdasTools.mthdPhasing(strCodFasSecu) + "\"] conns=[" + this.mthdConexTrafo(strCodFasPrim) + " " + this.mthdConexTrafo(strCodFasSecu) + "] kvs=[" + this.mthdTensaoEnrolamento(strCodFasPrim, dblTensaoPrimTrafo_kV).ToString().Replace(",", ".") + " " + this.mthdTensaoEnrolamento(strCodFasSecu, dblTensaoPrimTrafo_kV).ToString().Replace(",", ".") + "] kvas=[" + PotNom_kVA.ToString().Replace(",", ".") + " " + PotNom_kVA.ToString().Replace(",", ".") + "] xhl=" + ReatHL_per.ToString().Replace(",", ".") + " %loadloss=" + (dblPerdTtl_per - dblPerdVz_per).ToString().Replace(",", ".") + " %noloadloss=" + dblPerdVz_per.ToString().Replace(",", "."));
            this.mthdInsertCommandInLine("New \"Regcontrol." + strName + GeoPerdasTools.mthdNamingBank(intCodBnc) + "\" transformer=\"" + strName + GeoPerdasTools.mthdNamingBank(intCodBnc) + "\" winding=2 vreg=" + (dblTenRgl_pu * 100).ToString().Replace(",", ".") + " band=2 ptratio=" + (this.mthdTensaoEnrolamento(strCodFasSecu, dblTensaoPrimTrafo_kV) * 10).ToString().Replace(",", "."));
            if (intTipRegul == 2 && intCodBnc == 2)
                this.mthdInsertCommandInLine("New \"Line." + strName + "_J\" phases=" + GeoPerdasTools.mthdNumberPhasesTransformers(strCodCoinc) + " bus1=\"" + strBus1 + GeoPerdasTools.mthdPhasing(strCodCoinc) + "\" bus2=\"" + strBus2 + GeoPerdasTools.mthdPhasing(strCodCoinc) + "\" r0=1e-3  r1=1e-3  x0=0  x1=0  c0=0 c1=0");
        }

        private String mthdConexTrafo(String strCodFas)
        {
            if (strCodFas == "A" || strCodFas == "B" || strCodFas == "C" || strCodFas == "AN" || strCodFas == "BN" || strCodFas == "CN" || strCodFas == "ABN" || strCodFas == "BCN" || strCodFas == "CAN" || strCodFas == "ABCN")
                return "Wye";
            else if (strCodFas == "AB" || strCodFas == "BC" || strCodFas == "CA" || strCodFas == "ABC")
                return "Delta";
            return "";
        }

        private double mthdTensaoEnrolamento(String strCodFas, double dblTensao_kV)
        {
            if (strCodFas == "A" || strCodFas == "B" || strCodFas == "C" || strCodFas == "AN" || strCodFas == "BN" || strCodFas == "CN" || strCodFas == "ABN" || strCodFas == "BCN" || strCodFas == "CAN" || strCodFas == "ABCN")
                return (dblTensao_kV / Math.Pow(3, 0.5));
            else if (strCodFas == "AB" || strCodFas == "BC" || strCodFas == "CA" || strCodFas == "ABC")
                return dblTensao_kV;
            return 0.0;
        }

        private double mthdTensaoEnrolamentoSecundario(String strCodFas, double dblTensao_kV)
        {
            if (strCodFas == "A" || strCodFas == "B" || strCodFas == "C" || strCodFas == "AN" || strCodFas == "BN" || strCodFas == "CN")
                return dblTensao_kV;
            else if (strCodFas == "ABN" || strCodFas == "BCN" || strCodFas == "CAN" || strCodFas == "ABCN")
                return (dblTensao_kV / Math.Pow(3, 0.5));
            else if (strCodFas == "AB" || strCodFas == "BC" || strCodFas == "CA" || strCodFas == "ABC")
                return dblTensao_kV;
            return 0.0;
        }

        private void mthdGetDSSCommandLoadshape(String strName, double PotAtvNorm01, double PotAtvNorm02, double PotAtvNorm03, double PotAtvNorm04, double PotAtvNorm05, double PotAtvNorm06, double PotAtvNorm07, double PotAtvNorm08, double PotAtvNorm09, double PotAtvNorm10, double PotAtvNorm11, double PotAtvNorm12, double PotAtvNorm13, double PotAtvNorm14, double PotAtvNorm15, double PotAtvNorm16, double PotAtvNorm17, double PotAtvNorm18, double PotAtvNorm19, double PotAtvNorm20, double PotAtvNorm21, double PotAtvNorm22, double PotAtvNorm23, double PotAtvNorm24)
        {
            this.mthdInsertCommandInLine(GeoPerdasTools.mthdWriteLanguageDSSCommandLoadshape(strName, PotAtvNorm01, PotAtvNorm02, PotAtvNorm03, PotAtvNorm04, PotAtvNorm05, PotAtvNorm06, PotAtvNorm07, PotAtvNorm08, PotAtvNorm09, PotAtvNorm10, PotAtvNorm11, PotAtvNorm12, PotAtvNorm13, PotAtvNorm14, PotAtvNorm15, PotAtvNorm16, PotAtvNorm17, PotAtvNorm18, PotAtvNorm19, PotAtvNorm20, PotAtvNorm21, PotAtvNorm22, PotAtvNorm23, PotAtvNorm24));
        }

        private String mthdWriteLanguageDSSCommandTransformer(String strName, int intCodBnc, int intMRT, int intTipTrafo, String strCodFasPrim, String strCodFasSecu, String strCodFasTerc, String strBus1, String strBus2, double dblTensaoPrimTrafo_kV, double dblTensaoSecuTrafo_kV, double dblTap_pu, double dblPotNom_kVA, double dblPerdTtl_per, double dblPerdVz_per, String strPropriedade, int intSemCarga)
        {
            String result = "";
            double dblPerdTtlAlt_per = dblPerdTtl_per, dblPerdVzAlt_per = dblPerdVz_per;
            double dblPerdLodAlt_per = dblPerdTtl_per - dblPerdVz_per;

            if (strPropriedade == "TC" && intNeutralizarTrafoTerceiros == 1)
            {
                dblPerdTtlAlt_per = 0;
                dblPerdVzAlt_per = 0;
                dblPerdLodAlt_per = 0;
            }
            if (intSemCarga == 0)
                if (intMRT == 1)
                {
                    result += "New \"Transformer." + strName + GeoPerdasTools.mthdNamingBank(intCodBnc) + "\" phases=" + GeoPerdasTools.mthdNumberPhasesTransformers(strCodFasPrim) + " windings=" + this.mthdEnrrolamentoTrafo(strCodFasTerc, strCodFasSecu) + " buses=[" + this.mthdBusesTrafo(strCodFasPrim, strCodFasSecu, strCodFasTerc, "MRT_" + strBus1 + strName + GeoPerdasTools.mthdNamingBank(intCodBnc), strBus2) + "] conns=[" + this.mthdConnsTrafo(strCodFasPrim, strCodFasSecu, strCodFasTerc) + "] kvs=[" + this.mthdkVsTrafo(intTipTrafo, strCodFasPrim, strCodFasSecu, strCodFasTerc, dblTensaoPrimTrafo_kV, dblTensaoSecuTrafo_kV) + "] " + (this.intAdequarTapTrafo == 1 ? "taps=[" + mthdTapsTrafo(strCodFasTerc, dblTap_pu) + "] " : "") + "kvas=[" + this.mthdkVAsTrafo(strCodFasSecu, strCodFasTerc, dblPotNom_kVA) + "] %loadloss=" + dblPerdLodAlt_per.ToString().Replace(",", ".") + " %noloadloss=" + dblPerdVzAlt_per.ToString().Replace(",", ".");
                    result += Environment.NewLine + this.mthdWriteLanguageDSSCommandLinecode("LC_MRT_" + strName + GeoPerdasTools.mthdNamingBank(intCodBnc), 15E3, 0, 0);
                    result += Environment.NewLine + GeoPerdasTools.mthdWriteLanguageDSSCommandLineMT("Resist_MTR_" + strName + GeoPerdasTools.mthdNamingBank(intCodBnc), strCodFasPrim, strBus1, "MRT_" + strBus1 + strName + GeoPerdasTools.mthdNamingBank(intCodBnc), "LC_MRT_" + strName + GeoPerdasTools.mthdNamingBank(intCodBnc), 1E-3, "PR", 0, 0);
                    result += Environment.NewLine + "New \"Reactor." + strName + GeoPerdasTools.mthdNamingBank(intCodBnc) + "_R\" phases=1 bus1=" + strBus2 + ".4 R=15 X=0 basefreq=60";
                }
                else
                {
                    result += "New \"Transformer." + strName + GeoPerdasTools.mthdNamingBank(intCodBnc) + "\" phases=" + GeoPerdasTools.mthdNumberPhasesTransformers(strCodFasPrim) + " windings=" + this.mthdEnrrolamentoTrafo(strCodFasTerc, strCodFasSecu) + " buses=[" + this.mthdBusesTrafo(strCodFasPrim, strCodFasSecu, strCodFasTerc, strBus1, strBus2) + "] conns=[" + this.mthdConnsTrafo(strCodFasPrim, strCodFasSecu, strCodFasTerc) + "] kvs=[" + this.mthdkVsTrafo(intTipTrafo, strCodFasPrim, strCodFasSecu, strCodFasTerc, dblTensaoPrimTrafo_kV, dblTensaoSecuTrafo_kV) + "] " + (this.intAdequarTapTrafo == 1 ? "taps=[" + mthdTapsTrafo(strCodFasTerc, dblTap_pu) + "] " : "") + "kvas=[" + this.mthdkVAsTrafo(strCodFasSecu, strCodFasTerc, dblPotNom_kVA) + "] %loadloss=" + dblPerdLodAlt_per.ToString().Replace(",", ".") + " %noloadloss=" + dblPerdVzAlt_per.ToString().Replace(",", ".");
                    result += Environment.NewLine + "New \"Reactor." + strName + GeoPerdasTools.mthdNamingBank(intCodBnc) + "_R\" phases=1 bus1=" + strBus2 + ".4 R=15 X=0 basefreq=60";
                }
            else
                if (intMRT == 1)
                {
                    result += "!New \"Transformer." + strName + GeoPerdasTools.mthdNamingBank(intCodBnc) + "\" phases=" + GeoPerdasTools.mthdNumberPhasesTransformers(strCodFasPrim) + " windings=" + this.mthdEnrrolamentoTrafo(strCodFasTerc, strCodFasSecu) + " buses=[" + this.mthdBusesTrafo(strCodFasPrim, strCodFasSecu, strCodFasTerc, "MRT_" + strBus1 + strName + GeoPerdasTools.mthdNamingBank(intCodBnc), strBus2) + "] conns=[" + this.mthdConnsTrafo(strCodFasPrim, strCodFasSecu, strCodFasTerc) + "] kvs=[" + this.mthdkVsTrafo(intTipTrafo, strCodFasPrim, strCodFasSecu, strCodFasTerc, dblTensaoPrimTrafo_kV, dblTensaoSecuTrafo_kV) + "] " + (this.intAdequarTapTrafo == 1 ? "taps=[" + mthdTapsTrafo(strCodFasTerc, dblTap_pu) + "] " : "") + "kvas=[" + this.mthdkVAsTrafo(strCodFasSecu, strCodFasTerc, dblPotNom_kVA) + "] %loadloss=" + dblPerdLodAlt_per.ToString().Replace(",", ".") + " %noloadloss=" + dblPerdVzAlt_per.ToString().Replace(",", ".");
                    result += Environment.NewLine + "!" + this.mthdWriteLanguageDSSCommandLinecode("LC_MRT_" + strName + GeoPerdasTools.mthdNamingBank(intCodBnc), 15E3, 0, 0);
                    result += Environment.NewLine + "!" + GeoPerdasTools.mthdWriteLanguageDSSCommandLineMT("Resist_MTR_" + strName + GeoPerdasTools.mthdNamingBank(intCodBnc), strCodFasPrim, strBus1, "MRT_" + strBus1 + strName + GeoPerdasTools.mthdNamingBank(intCodBnc), "ABC", 1E-3, "PR", 0, 0);
                    result += Environment.NewLine + "!New \"Reactor." + strName + GeoPerdasTools.mthdNamingBank(intCodBnc) + "_R\" phases=1 bus1=" + strBus2 + ".4 R=15 X=0 basefreq=60";
                }
                else
                {
                    result += "!New \"Transformer." + strName + GeoPerdasTools.mthdNamingBank(intCodBnc) + "\" phases=" + GeoPerdasTools.mthdNumberPhasesTransformers(strCodFasPrim) + " windings=" + this.mthdEnrrolamentoTrafo(strCodFasTerc, strCodFasSecu) + " buses=[" + this.mthdBusesTrafo(strCodFasPrim, strCodFasSecu, strCodFasTerc, strBus1, strBus2) + "] conns=[" + this.mthdConnsTrafo(strCodFasPrim, strCodFasSecu, strCodFasTerc) + "] kvs=[" + this.mthdkVsTrafo(intTipTrafo, strCodFasPrim, strCodFasSecu, strCodFasTerc, dblTensaoPrimTrafo_kV, dblTensaoSecuTrafo_kV) + "] " + (this.intAdequarTapTrafo == 1 ? "taps=[" + mthdTapsTrafo(strCodFasTerc, dblTap_pu) + "] " : "") + "kvas=[" + this.mthdkVAsTrafo(strCodFasSecu, strCodFasTerc, dblPotNom_kVA) + "] %loadloss=" + dblPerdLodAlt_per.ToString().Replace(",", ".") + " %noloadloss=" + dblPerdVzAlt_per.ToString().Replace(",", ".");
                    result += Environment.NewLine + "!New \"Reactor." + strName + GeoPerdasTools.mthdNamingBank(intCodBnc) + "_R\" phases=1 bus1=" + strBus2 + ".4 R=15 X=0 basefreq=60";
                }

            return result;
        }

        private void mthdGetDSSCommandTransformer(String strName, int intCodBnc, int intMRT, int intTipTrafo, String strCodFasPrim, String strCodFasSecu, String strCodFasTerc, String strBus1, String strBus2, double dblTensaoPrimTrafo_kV, double dblTensaoSecuTrafo_kV, double dblTap_pu, double dblPotNom_kVA, double dblPerdTtl_per, double dblPerdVz_per, String strPropriedade, int intSemCarga)
        {
            double dblPerdTtlAlt_per = dblPerdTtl_per, dblPerdVzAlt_per = dblPerdVz_per;
            double dblPerdLodAlt_per = dblPerdTtl_per - dblPerdVz_per;

            if (strPropriedade == "TC" && intNeutralizarTrafoTerceiros == 1)
            {
                dblPerdTtlAlt_per = 0;
                dblPerdVzAlt_per = 0;
                dblPerdLodAlt_per = 0;
            }
            if (intSemCarga == 0)
                if (intMRT == 1)
                {
                    this.mthdInsertCommandInLine("New \"Transformer." + strName + GeoPerdasTools.mthdNamingBank(intCodBnc) + "\" phases=" + GeoPerdasTools.mthdNumberPhasesTransformers(strCodFasPrim) + " windings=" + this.mthdEnrrolamentoTrafo(strCodFasTerc, strCodFasSecu) + " buses=[" + this.mthdBusesTrafo(strCodFasPrim, strCodFasSecu, strCodFasTerc, "MRT_" + strBus1 + strName + GeoPerdasTools.mthdNamingBank(intCodBnc), strBus2) + "] conns=[" + this.mthdConnsTrafo(strCodFasPrim, strCodFasSecu, strCodFasTerc) + "] kvs=[" + this.mthdkVsTrafo(intTipTrafo, strCodFasPrim, strCodFasSecu, strCodFasTerc, dblTensaoPrimTrafo_kV, dblTensaoSecuTrafo_kV) + "] " + (this.intAdequarTapTrafo == 1 ? "taps=[" + mthdTapsTrafo(strCodFasTerc, dblTap_pu) + "] " : "") + "kvas=[" + this.mthdkVAsTrafo(strCodFasSecu, strCodFasTerc, dblPotNom_kVA) + "] %loadloss=" + dblPerdLodAlt_per.ToString().Replace(",", ".") + " %noloadloss=" + dblPerdVzAlt_per.ToString().Replace(",", "."));
                    this.mthdGetDSSCommandLinecode("LC_MRT_" + strName + GeoPerdasTools.mthdNamingBank(intCodBnc), 15E3, 0, 0);
                    this.mthdInsertCommandInLine(GeoPerdasTools.mthdWriteLanguageDSSCommandLineMT("Resist_MTR_" + strName + GeoPerdasTools.mthdNamingBank(intCodBnc), strCodFasPrim, strBus1, "MRT_" + strBus1 + strName + GeoPerdasTools.mthdNamingBank(intCodBnc), "LC_MRT_" + strName + GeoPerdasTools.mthdNamingBank(intCodBnc), 1E-3, "PR", 0, 0));
                }
                else
                {
                    this.mthdInsertCommandInLine("New \"Transformer." + strName + GeoPerdasTools.mthdNamingBank(intCodBnc) + "\" phases=" + GeoPerdasTools.mthdNumberPhasesTransformers(strCodFasPrim) + " windings=" + this.mthdEnrrolamentoTrafo(strCodFasTerc, strCodFasSecu) + " buses=[" + this.mthdBusesTrafo(strCodFasPrim, strCodFasSecu, strCodFasTerc, strBus1, strBus2) + "] conns=[" + this.mthdConnsTrafo(strCodFasPrim, strCodFasSecu, strCodFasTerc) + "] kvs=[" + this.mthdkVsTrafo(intTipTrafo, strCodFasPrim, strCodFasSecu, strCodFasTerc, dblTensaoPrimTrafo_kV, dblTensaoSecuTrafo_kV) + "] " + (this.intAdequarTapTrafo == 1 ? "taps=[" + mthdTapsTrafo(strCodFasTerc, dblTap_pu) + "] " : "") + "kvas=[" + this.mthdkVAsTrafo(strCodFasSecu, strCodFasTerc, dblPotNom_kVA) + "] %loadloss=" + dblPerdLodAlt_per.ToString().Replace(",", ".") + " %noloadloss=" + dblPerdVzAlt_per.ToString().Replace(",", "."));
                }
            else
                if (intMRT == 1)
                {
                    this.mthdInsertCommandInLine("!New \"Transformer." + strName + GeoPerdasTools.mthdNamingBank(intCodBnc) + "\" phases=" + GeoPerdasTools.mthdNumberPhasesTransformers(strCodFasPrim) + " windings=" + this.mthdEnrrolamentoTrafo(strCodFasTerc, strCodFasSecu) + " buses=[" + this.mthdBusesTrafo(strCodFasPrim, strCodFasSecu, strCodFasTerc, "MRT_" + strBus1 + strName + GeoPerdasTools.mthdNamingBank(intCodBnc), strBus2) + "] conns=[" + this.mthdConnsTrafo(strCodFasPrim, strCodFasSecu, strCodFasTerc) + "] kvs=[" + this.mthdkVsTrafo(intTipTrafo, strCodFasPrim, strCodFasSecu, strCodFasTerc, dblTensaoPrimTrafo_kV, dblTensaoSecuTrafo_kV) + "] " + (this.intAdequarTapTrafo == 1 ? "taps=[" + mthdTapsTrafo(strCodFasTerc, dblTap_pu) + "] " : "") + "kvas=[" + this.mthdkVAsTrafo(strCodFasSecu, strCodFasTerc, dblPotNom_kVA) + "] %loadloss=" + dblPerdLodAlt_per.ToString().Replace(",", ".") + " %noloadloss=" + dblPerdVzAlt_per.ToString().Replace(",", "."));
                    this.mthdGetDSSCommandLinecode("LC_MRT_" + strName + GeoPerdasTools.mthdNamingBank(intCodBnc), 15E3, 0, 0);
                    this.mthdInsertCommandInLine("!" + GeoPerdasTools.mthdWriteLanguageDSSCommandLineMT("Resist_MTR_" + strName + GeoPerdasTools.mthdNamingBank(intCodBnc), strCodFasPrim, strBus1, "MRT_" + strBus1 + strName + GeoPerdasTools.mthdNamingBank(intCodBnc), "ABC", 1E-3, "PR", 0, 0));
                }
                else
                {
                    this.mthdInsertCommandInLine("!New \"Transformer." + strName + GeoPerdasTools.mthdNamingBank(intCodBnc) + "\" phases=" + GeoPerdasTools.mthdNumberPhasesTransformers(strCodFasPrim) + " windings=" + this.mthdEnrrolamentoTrafo(strCodFasTerc, strCodFasSecu) + " buses=[" + this.mthdBusesTrafo(strCodFasPrim, strCodFasSecu, strCodFasTerc, strBus1, strBus2) + "] conns=[" + this.mthdConnsTrafo(strCodFasPrim, strCodFasSecu, strCodFasTerc) + "] kvs=[" + this.mthdkVsTrafo(intTipTrafo, strCodFasPrim, strCodFasSecu, strCodFasTerc, dblTensaoPrimTrafo_kV, dblTensaoSecuTrafo_kV) + "] " + (this.intAdequarTapTrafo == 1 ? "taps=[" + mthdTapsTrafo(strCodFasTerc, dblTap_pu) + "] " : "") + "kvas=[" + this.mthdkVAsTrafo(strCodFasSecu, strCodFasTerc, dblPotNom_kVA) + "] %loadloss=" + dblPerdLodAlt_per.ToString().Replace(",", ".") + " %noloadloss=" + dblPerdVzAlt_per.ToString().Replace(",", "."));
                }
        }

        private void mthdGetDSSCommandTransformer4Wires(String strName, int intCodBnc, int intMRT, int intTipTrafo, String strCodFasPrim, String strCodFasSecu, String strCodFasTerc, String strBus1, String strBus2, double dblTensaoPrimTrafo_kV, double dblTensaoSecuTrafo_kV, double dblTap_pu, double dblPotNom_kVA, double dblPerdTtl_per, double dblPerdVz_per, String strPropriedade, int intSemCarga)
        {
            double dblPerdTtlAlt_per = dblPerdTtl_per, dblPerdVzAlt_per = dblPerdVz_per;
            double dblPerdLodAlt_per = dblPerdTtl_per - dblPerdVz_per;

            if (strPropriedade == "TC" && intNeutralizarTrafoTerceiros == 1)
            {
                dblPerdTtlAlt_per = 0;
                dblPerdVzAlt_per = 0;
                dblPerdLodAlt_per = 0;
            }
            if (intSemCarga == 0)
                if (intMRT == 1)
                {
                    this.mthdInsertCommandInLine("New \"Transformer." + strName + GeoPerdasTools.mthdNamingBank(intCodBnc) + "\" phases=" + GeoPerdasTools.mthdNumberPhasesTransformers(strCodFasPrim) + " windings=" + this.mthdEnrrolamentoTrafo(strCodFasTerc, strCodFasSecu) + " buses=[" + this.mthdBusesTrafo(strCodFasPrim, strCodFasSecu, strCodFasTerc, "MRT_" + strBus1 + strName + GeoPerdasTools.mthdNamingBank(intCodBnc), strBus2) + "] conns=[" + this.mthdConnsTrafo(strCodFasPrim, strCodFasSecu, strCodFasTerc) + "] kvs=[" + this.mthdkVsTrafo(intTipTrafo, strCodFasPrim, strCodFasSecu, strCodFasTerc, dblTensaoPrimTrafo_kV, dblTensaoSecuTrafo_kV) + "] " + (this.intAdequarTapTrafo == 1 ? "taps=[" + mthdTapsTrafo(strCodFasTerc, dblTap_pu) + "] " : "") + "kvas=[" + this.mthdkVAsTrafo(strCodFasSecu, strCodFasTerc, dblPotNom_kVA) + "] %loadloss=" + dblPerdLodAlt_per.ToString().Replace(",", ".") + " %noloadloss=" + dblPerdVzAlt_per.ToString().Replace(",", "."));
                    this.mthdGetDSSCommandLinecode("LC_MRT_" + strName + GeoPerdasTools.mthdNamingBank(intCodBnc), 15E3, 0, 0);
                    this.mthdInsertCommandInLine(GeoPerdasTools.mthdWriteLanguageDSSCommandLineMT("Resist_MTR_" + strName + GeoPerdasTools.mthdNamingBank(intCodBnc), strCodFasPrim, strBus1, "MRT_" + strBus1 + strName + GeoPerdasTools.mthdNamingBank(intCodBnc), "LC_MRT_" + strName + GeoPerdasTools.mthdNamingBank(intCodBnc), 1E-3, "PR", 0, 0));
                    this.mthdInsertCommandInLine("New \"Reactor." + strName + GeoPerdasTools.mthdNamingBank(intCodBnc) + "_R\" phases=1 bus1=" + strBus2 + ".4 R=15 X=0 basefreq=60");
                }
                else
                {
                    this.mthdInsertCommandInLine("New \"Transformer." + strName + GeoPerdasTools.mthdNamingBank(intCodBnc) + "\" phases=" + GeoPerdasTools.mthdNumberPhasesTransformers(strCodFasPrim) + " windings=" + this.mthdEnrrolamentoTrafo(strCodFasTerc, strCodFasSecu) + " buses=[" + this.mthdBusesTrafo(strCodFasPrim, strCodFasSecu, strCodFasTerc, strBus1, strBus2) + "] conns=[" + this.mthdConnsTrafo(strCodFasPrim, strCodFasSecu, strCodFasTerc) + "] kvs=[" + this.mthdkVsTrafo(intTipTrafo, strCodFasPrim, strCodFasSecu, strCodFasTerc, dblTensaoPrimTrafo_kV, dblTensaoSecuTrafo_kV) + "] " + (this.intAdequarTapTrafo == 1 ? "taps=[" + mthdTapsTrafo(strCodFasTerc, dblTap_pu) + "] " : "") + "kvas=[" + this.mthdkVAsTrafo(strCodFasSecu, strCodFasTerc, dblPotNom_kVA) + "] %loadloss=" + dblPerdLodAlt_per.ToString().Replace(",", ".") + " %noloadloss=" + dblPerdVzAlt_per.ToString().Replace(",", "."));
                    this.mthdInsertCommandInLine("New \"Reactor." + strName + GeoPerdasTools.mthdNamingBank(intCodBnc) + "_R\" phases=1 bus1=" + strBus2 + ".4 R=15 X=0 basefreq=60");
                }
            else
                if (intMRT == 1)
                {
                    this.mthdInsertCommandInLine("!New \"Transformer." + strName + GeoPerdasTools.mthdNamingBank(intCodBnc) + "\" phases=" + GeoPerdasTools.mthdNumberPhasesTransformers(strCodFasPrim) + " windings=" + this.mthdEnrrolamentoTrafo(strCodFasTerc, strCodFasSecu) + " buses=[" + this.mthdBusesTrafo(strCodFasPrim, strCodFasSecu, strCodFasTerc, "MRT_" + strBus1 + strName + GeoPerdasTools.mthdNamingBank(intCodBnc), strBus2) + "] conns=[" + this.mthdConnsTrafo(strCodFasPrim, strCodFasSecu, strCodFasTerc) + "] kvs=[" + this.mthdkVsTrafo(intTipTrafo, strCodFasPrim, strCodFasSecu, strCodFasTerc, dblTensaoPrimTrafo_kV, dblTensaoSecuTrafo_kV) + "] " + (this.intAdequarTapTrafo == 1 ? "taps=[" + mthdTapsTrafo(strCodFasTerc, dblTap_pu) + "] " : "") + "kvas=[" + this.mthdkVAsTrafo(strCodFasSecu, strCodFasTerc, dblPotNom_kVA) + "] %loadloss=" + dblPerdLodAlt_per.ToString().Replace(",", ".") + " %noloadloss=" + dblPerdVzAlt_per.ToString().Replace(",", "."));
                    this.mthdGetDSSCommandLinecode("LC_MRT_" + strName + GeoPerdasTools.mthdNamingBank(intCodBnc), 15E3, 0, 0);
                    this.mthdInsertCommandInLine("!" + GeoPerdasTools.mthdWriteLanguageDSSCommandLineMT("Resist_MTR_" + strName + GeoPerdasTools.mthdNamingBank(intCodBnc), strCodFasPrim, strBus1, "MRT_" + strBus1 + strName + GeoPerdasTools.mthdNamingBank(intCodBnc), "ABC", 1E-3, "PR", 0, 0));
                    this.mthdInsertCommandInLine("!New \"Reactor." + strName + GeoPerdasTools.mthdNamingBank(intCodBnc) + "_R\" phases=1 bus1=" + strBus2 + ".4 R=15 X=0 basefreq=60");
                }
                else
                {
                    this.mthdInsertCommandInLine("!New \"Transformer." + strName + GeoPerdasTools.mthdNamingBank(intCodBnc) + "\" phases=" + GeoPerdasTools.mthdNumberPhasesTransformers(strCodFasPrim) + " windings=" + this.mthdEnrrolamentoTrafo(strCodFasTerc, strCodFasSecu) + " buses=[" + this.mthdBusesTrafo(strCodFasPrim, strCodFasSecu, strCodFasTerc, strBus1, strBus2) + "] conns=[" + this.mthdConnsTrafo(strCodFasPrim, strCodFasSecu, strCodFasTerc) + "] kvs=[" + this.mthdkVsTrafo(intTipTrafo, strCodFasPrim, strCodFasSecu, strCodFasTerc, dblTensaoPrimTrafo_kV, dblTensaoSecuTrafo_kV) + "] " + (this.intAdequarTapTrafo == 1 ? "taps=[" + mthdTapsTrafo(strCodFasTerc, dblTap_pu) + "] " : "") + "kvas=[" + this.mthdkVAsTrafo(strCodFasSecu, strCodFasTerc, dblPotNom_kVA) + "] %loadloss=" + dblPerdLodAlt_per.ToString().Replace(",", ".") + " %noloadloss=" + dblPerdVzAlt_per.ToString().Replace(",", "."));
                    this.mthdInsertCommandInLine("!New \"Reactor." + strName + GeoPerdasTools.mthdNamingBank(intCodBnc) + "_R\" phases=1 bus1=" + strBus2 + ".4 R=15 X=0 basefreq=60");
                }
        }

        private String mthdEnrrolamentoTrafo(String strCodFas1, String strCodFas2)
        {
            if (strCodFas1 == "XX" && strCodFas2 != "ABN")
                return "2";
            else if (strCodFas1 == "BN" || strCodFas1 == "CN" || strCodFas1 == "AN" || strCodFas2 == "ABN")
                return "3";
            return "0";
        }

        private String mthdBusesTrafo(String strCodFas1, String strCodFas2, String strCodFas3, String strBus1, String strBus2)
        {
            if (strCodFas3 == "BN" || strCodFas3 == "CN" || strCodFas3 == "AN" || strCodFas2 == "ABN")
                return "\"" + strBus1 + GeoPerdasTools.mthdPhasing(strCodFas1) + "\" \"" + strBus2 + GeoPerdasTools.mthdPhasing4Wires(strCodFas2) + "\" \"" + strBus2 + GeoPerdasTools.mthdPhasingTertiary4Wires(strCodFas3) + "\"";
            else if (strCodFas3 == "XX" || strCodFas2 == "AN" || strCodFas2 == "BN" || strCodFas2 == "CN" || strCodFas2 == "AB" || strCodFas2 == "BC" || strCodFas2 == "CA" || strCodFas2 == "AC" || strCodFas2 == "ABCN" || strCodFas2 == "ABC")
                return "\"" + strBus1 + GeoPerdasTools.mthdPhasing(strCodFas1) + "\" \"" + strBus2 + GeoPerdasTools.mthdPhasing4Wires(strCodFas2) + "\"";
            return "";
        }

        private String mthdConnsTrafo(String strCodFas1, String strCodFas2, String strCodFas3)
        {
            if (strCodFas3 == "BN" || strCodFas3 == "CN" || strCodFas3 == "AN" || strCodFas2 == "ABN")
                return this.mthdConexTrafo(strCodFas1) + " " + this.mthdConexTrafo(strCodFas2) + " " + this.mthdConexTrafo(strCodFas3);
            else if (strCodFas3 == "XX" || strCodFas2 == "AN" || strCodFas2 == "BN" || strCodFas2 == "CN" || strCodFas2 == "AB" || strCodFas2 == "BC" || strCodFas2 == "CA" || strCodFas2 == "AC" || strCodFas2 == "ABCN" || strCodFas2 == "ABC")
                return this.mthdConexTrafo(strCodFas1) + " " + this.mthdConexTrafo(strCodFas2);
            return "";
        }

        private String mthdkVsTrafo(int intTipTrafo, String strCodFas1, String strCodFas2, String strCodFas3, double dblTensaoPrimTrafo_kV, double dblTensaoSecuTrafo_kV)
        {
            if (intTipTrafo == 4)
                return dblTensaoPrimTrafo_kV.ToString().Replace(",", ".") + " " + dblTensaoSecuTrafo_kV.ToString().Replace(",", ".");
            else if (strCodFas3 == "BN" || strCodFas3 == "CN" || strCodFas3 == "AN" || strCodFas2 == "ABN")
                return (this.mthdTensaoEnrolamento(strCodFas1, dblTensaoPrimTrafo_kV)).ToString().Replace(",", ".") + " " + (dblTensaoSecuTrafo_kV / 2).ToString().Replace(",", ".") + " " + (dblTensaoSecuTrafo_kV / 2).ToString().Replace(",", ".");
            else if (strCodFas3 == "XX" && (strCodFas2 == "AN" || strCodFas2 == "BN" || strCodFas2 == "CN" || strCodFas2 == "AB" || strCodFas2 == "BC" || strCodFas2 == "CA" || strCodFas2 == "AC" || strCodFas2 == "ABC"))
                return (this.mthdTensaoEnrolamento(strCodFas1, dblTensaoPrimTrafo_kV)).ToString().Replace(",", ".") + " " + (this.mthdTensaoEnrolamentoSecundario(strCodFas2, dblTensaoSecuTrafo_kV)).ToString().Replace(",", ".");
            else if (strCodFas3 == "XX" && strCodFas2 == "ABCN")
                return (this.mthdTensaoEnrolamento(strCodFas1, dblTensaoPrimTrafo_kV)).ToString().Replace(",", ".") + " " + dblTensaoSecuTrafo_kV.ToString().Replace(",", ".");
            return "";
        }

        private String mthdkVAsTrafo(String strCodFas2, String strCodFas3, double dblPotNom_kVA)
        {
            if (strCodFas3 == "BN" || strCodFas3 == "CN" || strCodFas3 == "AN" || strCodFas2 == "ABN")
                return dblPotNom_kVA.ToString().Replace(",", ".") + " " + dblPotNom_kVA.ToString().Replace(",", ".") + " " + dblPotNom_kVA.ToString().Replace(",", ".");
            else if (strCodFas3 == "XX" || strCodFas2 == "AN" || strCodFas2 == "BN" || strCodFas2 == "CN" || strCodFas2 == "AB" || strCodFas2 == "BC" || strCodFas2 == "CA" || strCodFas2 == "AC" || strCodFas2 == "ABCN" || strCodFas2 == "ABC")
                return dblPotNom_kVA.ToString().Replace(",", ".") + " " + dblPotNom_kVA.ToString().Replace(",", ".");
            return "";
        }

        private String mthdTapsTrafo(String strCodFas3, double dblTap_pu)
        {
            if (strCodFas3 != "XX")
                return "1 " + dblTap_pu.ToString().Replace(",", ".") + " " + dblTap_pu.ToString().Replace(",", ".");
            else
                return "1 " + dblTap_pu.ToString().Replace(",", ".");
        }

        private String mthdWriteLanguageDSSCommandNewLoad(String strName, String strCodFas, String strBus, double dblTensaoLine_kV, double dblTensaoPhase_kV, double dblDemMax_kW, double dblDemMaxTrafo_kW, String strCodCrvCrg, int intTipoCarga, int intAlteraModelo, int intExecucaoSnapShot)
        {
            String result = "";
            double dblDemMaxCorrigida_kW;
            String strCurvaCarga;

            if (intTipoCarga == 2 && dblDemMax_kW > dblDemMaxTrafo_kW)
                dblDemMaxCorrigida_kW = dblDemMaxTrafo_kW;
            else
                dblDemMaxCorrigida_kW = dblDemMax_kW;

            if (intExecucaoSnapShot == 1)
                strCurvaCarga = "";
            else
                strCurvaCarga = " daily=\"" + strCodCrvCrg + "\" status=variable";

            if (intTipoCarga == 1)
            {
                if (intAlteraModelo == 1)
                {
                    result = "New \"Load." + strName + "_M1\" bus1=\"" + strBus + GeoPerdasTools.mthdPhasing4Wires(strCodFas) + "\" phases=" + GeoPerdasTools.mthdNumberPhasesLoads(strCodFas) + " conn=" + this.mthdTipoConexaoCargas(strCodFas) + " model=" + this.mthdGetTypeModel(1) + " kv=" + GeoPerdasTools.mthdTensionLoads(strCodFas, dblTensaoLine_kV, dblTensaoPhase_kV).ToString().Replace(",", ".") + " kw=" + ((this.intAdequarPotenciaCarga == 1) ? (dblDemMaxCorrigida_kW / 2).ToString().Replace(",", ".") : (dblDemMax_kW / 2).ToString().Replace(",", ".")) + " pf=0.92" + strCurvaCarga + " vmaxpu=1.5 vminpu=0.93" + Environment.NewLine;
                    result += "New \"Load." + strName + "_M2\" bus1=\"" + strBus + GeoPerdasTools.mthdPhasing4Wires(strCodFas) + "\" phases=" + GeoPerdasTools.mthdNumberPhasesLoads(strCodFas) + " conn=" + this.mthdTipoConexaoCargas(strCodFas) + " model=" + this.mthdGetTypeModel(2) + " kv=" + GeoPerdasTools.mthdTensionLoads(strCodFas, dblTensaoLine_kV, dblTensaoPhase_kV).ToString().Replace(",", ".") + " kw=" + ((this.intAdequarPotenciaCarga == 1) ? (dblDemMaxCorrigida_kW / 2).ToString().Replace(",", ".") : (dblDemMax_kW / 2).ToString().Replace(",", ".")) + " pf=0.92" + strCurvaCarga + " vmaxpu=1.5 vminpu=0.93";
                }
                else
                {
                    result = "New \"Load." + strName + "_M1\" bus1=\"" + strBus + GeoPerdasTools.mthdPhasing4Wires(strCodFas) + "\" phases=" + GeoPerdasTools.mthdNumberPhasesLoads(strCodFas) + " conn=" + this.mthdTipoConexaoCargas(strCodFas) + " model=" + this.mthdGetTypeModel(1) + " kv=" + GeoPerdasTools.mthdTensionLoads(strCodFas, dblTensaoLine_kV, dblTensaoPhase_kV).ToString().Replace(",", ".") + " kw=" + ((this.intAdequarPotenciaCarga == 1) ? (dblDemMaxCorrigida_kW / 2).ToString().Replace(",", ".") : (dblDemMax_kW / 2).ToString().Replace(",", ".")) + " pf=0.92" + strCurvaCarga + " vmaxpu=1.5 vminpu=" + this.dblVPUMin.ToString().Replace(",", ".") + Environment.NewLine;
                    result += "New \"Load." + strName + "_M2\" bus1=\"" + strBus + GeoPerdasTools.mthdPhasing4Wires(strCodFas) + "\" phases=" + GeoPerdasTools.mthdNumberPhasesLoads(strCodFas) + " conn=" + this.mthdTipoConexaoCargas(strCodFas) + " model=" + this.mthdGetTypeModel(2) + " kv=" + GeoPerdasTools.mthdTensionLoads(strCodFas, dblTensaoLine_kV, dblTensaoPhase_kV).ToString().Replace(",", ".") + " kw=" + ((this.intAdequarPotenciaCarga == 1) ? (dblDemMaxCorrigida_kW / 2).ToString().Replace(",", ".") : (dblDemMax_kW / 2).ToString().Replace(",", ".")) + " pf=0.92" + strCurvaCarga + " vmaxpu=1.5 vminpu=" + this.dblVPUMin.ToString().Replace(",", ".");
                }
            }
            else if (intTipoCarga == 2)
            {
                if (intAlteraModelo == 1)
                {
                    result = "New \"Load." + strName + "_M1\" bus1=\"" + strBus + GeoPerdasTools.mthdPhasing4Wires(strCodFas) + "\" phases=" + GeoPerdasTools.mthdNumberPhasesLoads(strCodFas) + " conn=" + this.mthdTipoConexaoCargas(strCodFas) + " model=" + this.mthdGetTypeModel(1) + " kv=" + GeoPerdasTools.mthdTensionLoads(strCodFas, dblTensaoLine_kV, dblTensaoPhase_kV).ToString().Replace(",", ".") + " kw=" + ((this.intAdequarPotenciaCarga == 1) ? (dblDemMaxCorrigida_kW / 2).ToString().Replace(",", ".") : (dblDemMax_kW / 2).ToString().Replace(",", ".")) + " pf=0.92" + strCurvaCarga + " vmaxpu=1.5 vminpu=0.92" + ((intTipoCarga == 2 && dblDemMax_kW > dblDemMaxTrafo_kW && this.intAdequarPotenciaCarga == 1) ? " ! Carga limitada" : "") + Environment.NewLine;
                    result += "New \"Load." + strName + "_M2\" bus1=\"" + strBus + GeoPerdasTools.mthdPhasing4Wires(strCodFas) + "\" phases=" + GeoPerdasTools.mthdNumberPhasesLoads(strCodFas) + " conn=" + this.mthdTipoConexaoCargas(strCodFas) + " model=" + this.mthdGetTypeModel(2) + " kv=" + GeoPerdasTools.mthdTensionLoads(strCodFas, dblTensaoLine_kV, dblTensaoPhase_kV).ToString().Replace(",", ".") + " kw=" + ((this.intAdequarPotenciaCarga == 1) ? (dblDemMaxCorrigida_kW / 2).ToString().Replace(",", ".") : (dblDemMax_kW / 2).ToString().Replace(",", ".")) + " pf=0.92" + strCurvaCarga + " vmaxpu=1.5 vminpu=0.92" + ((intTipoCarga == 2 && dblDemMax_kW > dblDemMaxTrafo_kW && this.intAdequarPotenciaCarga == 1) ? " ! Carga limitada" : "");
                }
                else
                {
                    result = "New \"Load." + strName + "_M1\" bus1=\"" + strBus + GeoPerdasTools.mthdPhasing4Wires(strCodFas) + "\" phases=" + GeoPerdasTools.mthdNumberPhasesLoads(strCodFas) + " conn=" + this.mthdTipoConexaoCargas(strCodFas) + " model=" + this.mthdGetTypeModel(1) + " kv=" + GeoPerdasTools.mthdTensionLoads(strCodFas, dblTensaoLine_kV, dblTensaoPhase_kV).ToString().Replace(",", ".") + " kw=" + ((this.intAdequarPotenciaCarga == 1) ? (dblDemMaxCorrigida_kW / 2).ToString().Replace(",", ".") : (dblDemMax_kW / 2).ToString().Replace(",", ".")) + " pf=0.92" + strCurvaCarga + " vmaxpu=1.5 vminpu=" + this.dblVPUMin.ToString().Replace(",", ".") + ((intTipoCarga == 2 && dblDemMax_kW > dblDemMaxTrafo_kW && this.intAdequarPotenciaCarga == 1) ? " ! Carga limitada" : "") + Environment.NewLine;
                    result += "New \"Load." + strName + "_M2\" bus1=\"" + strBus + GeoPerdasTools.mthdPhasing4Wires(strCodFas) + "\" phases=" + GeoPerdasTools.mthdNumberPhasesLoads(strCodFas) + " conn=" + this.mthdTipoConexaoCargas(strCodFas) + " model=" + this.mthdGetTypeModel(2) + " kv=" + GeoPerdasTools.mthdTensionLoads(strCodFas, dblTensaoLine_kV, dblTensaoPhase_kV).ToString().Replace(",", ".") + " kw=" + ((this.intAdequarPotenciaCarga == 1) ? (dblDemMaxCorrigida_kW / 2).ToString().Replace(",", ".") : (dblDemMax_kW / 2).ToString().Replace(",", ".")) + " pf=0.92" + strCurvaCarga + " vmaxpu=1.5 vminpu=" + this.dblVPUMin.ToString().Replace(",", ".") + ((intTipoCarga == 2 && dblDemMax_kW > dblDemMaxTrafo_kW && this.intAdequarPotenciaCarga == 1) ? " ! Carga limitada" : "");
                }
            }

            return result;
        }

        private String mthdGetTypeModel(int intPart)
        {
            if (this.intAdequarModeloCarga == 1)
                return (intPart == 1 ? "2" : "3");
            else if (this.intAdequarModeloCarga == 2)
                return (intPart == 1 ? "1" : "1");
            else if (this.intAdequarModeloCarga == 3)
                return (intPart == 1 ? "3" : "3");
            return "";
        }

        private void mthdGetDSSCommandNewLoad(String strName, String strCodFas, String strBus, double dblTensaoLine_kV, double dblTensaoPhase_kV, double dblDemMax_kW, double dblDemMaxTrafo_kW, String strCodCrvCrg, int intTipoCarga, int intAlteraModelo, int intExecucaoSnapShot)
        {

            double dblDemMaxCorrigida_kW;
            String strCurvaCarga;

            if (intTipoCarga == 2 && dblDemMax_kW > dblDemMaxTrafo_kW)
                dblDemMaxCorrigida_kW = dblDemMaxTrafo_kW;
            else
                dblDemMaxCorrigida_kW = dblDemMax_kW;

            if (intExecucaoSnapShot == 1)
                strCurvaCarga = "";
            else
                strCurvaCarga = " daily=\"" + strCodCrvCrg + "\" status=variable";

            if (intTipoCarga == 1)
            {
                if (intAlteraModelo == 1)
                {
                    this.mthdInsertCommandInLine("New \"Load." + strName + "_M1\" bus1=\"" + strBus + GeoPerdasTools.mthdPhasing4Wires(strCodFas) + "\" phases=" + GeoPerdasTools.mthdNumberPhasesLoads(strCodFas) + " conn=" + this.mthdTipoConexaoCargas(strCodFas) + " model=" + this.mthdGetTypeModel(1) + " kv=" + GeoPerdasTools.mthdTensionLoads(strCodFas, dblTensaoLine_kV, dblTensaoPhase_kV).ToString().Replace(",", ".") + " kw=" + ((this.intAdequarPotenciaCarga == 1) ? (dblDemMaxCorrigida_kW / 2).ToString().Replace(",", ".") : (dblDemMax_kW / 2).ToString().Replace(",", ".")) + " pf=0.92" + strCurvaCarga + " vmaxpu=1.5 vminpu=0.93");
                    this.mthdInsertCommandInLine("New \"Load." + strName + "_M2\" bus1=\"" + strBus + GeoPerdasTools.mthdPhasing4Wires(strCodFas) + "\" phases=" + GeoPerdasTools.mthdNumberPhasesLoads(strCodFas) + " conn=" + this.mthdTipoConexaoCargas(strCodFas) + " model=" + this.mthdGetTypeModel(2) + " kv=" + GeoPerdasTools.mthdTensionLoads(strCodFas, dblTensaoLine_kV, dblTensaoPhase_kV).ToString().Replace(",", ".") + " kw=" + ((this.intAdequarPotenciaCarga == 1) ? (dblDemMaxCorrigida_kW / 2).ToString().Replace(",", ".") : (dblDemMax_kW / 2).ToString().Replace(",", ".")) + " pf=0.92" + strCurvaCarga + " vmaxpu=1.5 vminpu=0.93");
                }
                else
                {
                    this.mthdInsertCommandInLine("New \"Load." + strName + "_M1\" bus1=\"" + strBus + GeoPerdasTools.mthdPhasing4Wires(strCodFas) + "\" phases=" + GeoPerdasTools.mthdNumberPhasesLoads(strCodFas) + " conn=" + this.mthdTipoConexaoCargas(strCodFas) + " model=" + this.mthdGetTypeModel(1) + " kv=" + GeoPerdasTools.mthdTensionLoads(strCodFas, dblTensaoLine_kV, dblTensaoPhase_kV).ToString().Replace(",", ".") + " kw=" + ((this.intAdequarPotenciaCarga == 1) ? (dblDemMaxCorrigida_kW / 2).ToString().Replace(",", ".") : (dblDemMax_kW / 2).ToString().Replace(",", ".")) + " pf=0.92" + strCurvaCarga + " vmaxpu=1.5 vminpu=" + this.dblVPUMin.ToString().Replace(",", "."));
                    this.mthdInsertCommandInLine("New \"Load." + strName + "_M2\" bus1=\"" + strBus + GeoPerdasTools.mthdPhasing4Wires(strCodFas) + "\" phases=" + GeoPerdasTools.mthdNumberPhasesLoads(strCodFas) + " conn=" + this.mthdTipoConexaoCargas(strCodFas) + " model=" + this.mthdGetTypeModel(2) + " kv=" + GeoPerdasTools.mthdTensionLoads(strCodFas, dblTensaoLine_kV, dblTensaoPhase_kV).ToString().Replace(",", ".") + " kw=" + ((this.intAdequarPotenciaCarga == 1) ? (dblDemMaxCorrigida_kW / 2).ToString().Replace(",", ".") : (dblDemMax_kW / 2).ToString().Replace(",", ".")) + " pf=0.92" + strCurvaCarga + " vmaxpu=1.5 vminpu=" + this.dblVPUMin.ToString().Replace(",", "."));
                }
            }
            else if (intTipoCarga == 2)
            {
                if (intAlteraModelo == 1)
                {
                    this.mthdInsertCommandInLine("New \"Load." + strName + "_M1\" bus1=\"" + strBus + GeoPerdasTools.mthdPhasing4Wires(strCodFas) + "\" phases=" + GeoPerdasTools.mthdNumberPhasesLoads(strCodFas) + " conn=" + this.mthdTipoConexaoCargas(strCodFas) + " model=" + this.mthdGetTypeModel(1) + " kv=" + GeoPerdasTools.mthdTensionLoads(strCodFas, dblTensaoLine_kV, dblTensaoPhase_kV).ToString().Replace(",", ".") + " kw=" + ((this.intAdequarPotenciaCarga == 1) ? (dblDemMaxCorrigida_kW / 2).ToString().Replace(",", ".") : (dblDemMax_kW / 2).ToString().Replace(",", ".")) + " pf=0.92" + strCurvaCarga + " vmaxpu=1.5 vminpu=0.92");
                    this.mthdInsertCommandInLine("New \"Load." + strName + "_M2\" bus1=\"" + strBus + GeoPerdasTools.mthdPhasing4Wires(strCodFas) + "\" phases=" + GeoPerdasTools.mthdNumberPhasesLoads(strCodFas) + " conn=" + this.mthdTipoConexaoCargas(strCodFas) + " model=" + this.mthdGetTypeModel(2) + " kv=" + GeoPerdasTools.mthdTensionLoads(strCodFas, dblTensaoLine_kV, dblTensaoPhase_kV).ToString().Replace(",", ".") + " kw=" + ((this.intAdequarPotenciaCarga == 1) ? (dblDemMaxCorrigida_kW / 2).ToString().Replace(",", ".") : (dblDemMax_kW / 2).ToString().Replace(",", ".")) + " pf=0.92" + strCurvaCarga + " vmaxpu=1.5 vminpu=0.92");
                }
                else
                {
                    this.mthdInsertCommandInLine("New \"Load." + strName + "_M1\" bus1=\"" + strBus + GeoPerdasTools.mthdPhasing4Wires(strCodFas) + "\" phases=" + GeoPerdasTools.mthdNumberPhasesLoads(strCodFas) + " conn=" + this.mthdTipoConexaoCargas(strCodFas) + " model=" + this.mthdGetTypeModel(1) + " kv=" + GeoPerdasTools.mthdTensionLoads(strCodFas, dblTensaoLine_kV, dblTensaoPhase_kV).ToString().Replace(",", ".") + " kw=" + ((this.intAdequarPotenciaCarga == 1) ? (dblDemMaxCorrigida_kW / 2).ToString().Replace(",", ".") : (dblDemMax_kW / 2).ToString().Replace(",", ".")) + " pf=0.92" + strCurvaCarga + " vmaxpu=1.5 vminpu=" + this.dblVPUMin.ToString().Replace(",", "."));
                    this.mthdInsertCommandInLine("New \"Load." + strName + "_M2\" bus1=\"" + strBus + GeoPerdasTools.mthdPhasing4Wires(strCodFas) + "\" phases=" + GeoPerdasTools.mthdNumberPhasesLoads(strCodFas) + " conn=" + this.mthdTipoConexaoCargas(strCodFas) + " model=" + this.mthdGetTypeModel(2) + " kv=" + GeoPerdasTools.mthdTensionLoads(strCodFas, dblTensaoLine_kV, dblTensaoPhase_kV).ToString().Replace(",", ".") + " kw=" + ((this.intAdequarPotenciaCarga == 1) ? (dblDemMaxCorrigida_kW / 2).ToString().Replace(",", ".") : (dblDemMax_kW / 2).ToString().Replace(",", ".")) + " pf=0.92" + strCurvaCarga + " vmaxpu=1.5 vminpu=" + this.dblVPUMin.ToString().Replace(",", "."));
                }
            }

        }

        private String mthdTipoConexaoCargas(String strCodFas)
        {
            if (strCodFas == "A" || strCodFas == "B" || strCodFas == "C" || strCodFas == "AN" || strCodFas == "BN" || strCodFas == "CN")
                return "Wye";
            else if (strCodFas == "AB" || strCodFas == "BC" || strCodFas == "CA" || strCodFas == "ABN" || strCodFas == "BCN" || strCodFas == "CAN" || strCodFas == "ABC" || strCodFas == "ABCN")
                return "Delta";
            return "";
        }

        private void mthdGetDSSCommandEditLoad(String strName, String strCodFas, double dblDemMax_kW, double dblDemMaxTrafo_kW, String strCodCrvCrg, int intTipoCarga, int intExecucaoSnapShot)
        {

            double dblDemMaxCorrigida_kW;
            String strCurvaCarga;

            if (intTipoCarga == 2 && dblDemMax_kW > dblDemMaxTrafo_kW)
                dblDemMaxCorrigida_kW = dblDemMaxTrafo_kW;
            else
                dblDemMaxCorrigida_kW = dblDemMax_kW;

            if (intExecucaoSnapShot == 1)
                strCurvaCarga = "";
            else
                strCurvaCarga = " daily=\"" + strCodCrvCrg + "\"";

            this.mthdInsertCommandInLine("Edit \"Load." + strName + "_M1\" kw=" + ((this.intAdequarPotenciaCarga == 1) ? (dblDemMaxCorrigida_kW / 2).ToString().Replace(",", ".") : (dblDemMax_kW / 2).ToString().Replace(",", ".") ) + strCurvaCarga);
            this.mthdInsertCommandInLine("Edit \"Load." + strName + "_M2\" kw=" + ((this.intAdequarPotenciaCarga == 1) ? (dblDemMaxCorrigida_kW / 2).ToString().Replace(",", ".") : (dblDemMax_kW / 2).ToString().Replace(",", ".") ) + strCurvaCarga);
        }

        private void mthdGetDSSCommandEnergymeter(String strName, String strElement)
        {
            this.mthdInsertCommandInLine(GeoPerdasTools.mthdWriteLanguageDSSCommandEnergymeter(strName, strElement));
        }

        private String mthdNomeSegmentoTensaoBus(double dblTensao_kV)
        {
            if (dblTensao_kV > 25)
                return "A3a";
            if (dblTensao_kV <= 25 && dblTensao_kV > 1)
                return "A4-";
            if (dblTensao_kV <= 1)
                return "B--";
            return "";
        }

        private String mthdNomeElemento(String strTipoElemento)
        {
            if (strTipoElemento == "RML")
                return "Line.RBT_";
            else if (strTipoElemento == "SEGMBT")
                return "Line.SBT_";
            else if (strTipoElemento == "SEGMMT")
                return "Line.SMT_";
            else if (strTipoElemento == "CHVMT")
                return "Line.CMT_";
            else if (strTipoElemento == "TRAFO")
                return "Transformer.TRF_";
            else if (strTipoElemento == "REGUL")
                return "Transformer.REG_";
            return "";
        }

        private void mthdWriteLineInFile(String Line)
        {
            //this.swFile.WriteLine(Line);
        }

        private void clbOptionsRun_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.mthdUpdateOptionsRun();
        }

        private void mthdUpdateOptionsRun()
        {
            this.intRealizaCnvrgcPNT = (this.clbOptionsRun.CheckedItems.Contains("Convergência de Perda Não Técnica") == true) ? 1 : 0;
            this.intUsaTrafoABNT = (this.clbOptionsRun.CheckedItems.Contains("Transformadores ABNT") == true) ? 1 : 0;
            this.intAdequarTensaoCargasMT = (this.clbOptionsRun.CheckedItems.Contains("Adequação Tensão Mínima das Cargas MT (0,93 pu)") == true) ? 1 : 0;
            this.intAdequarTensaoCargasBT = (this.clbOptionsRun.CheckedItems.Contains("Adequação Tensão Mínima das Cargas BT (0,92 pu)") == true) ? 1 : 0;
            this.intAdequarTensaoSuperior = (this.clbOptionsRun.CheckedItems.Contains("Limitar Máxima Tensão de Barras e Reguladores (1,05 pu)") == true) ? 1 : 0;
            this.intAdequarRamal = (this.clbOptionsRun.CheckedItems.Contains("Limitar o Ramal (30m)") == true) ? 1 : 0;
            this.intAdequarTapTrafo = (this.clbOptionsRun.CheckedItems.Contains("Utilizar Tap nos Transformadores") == true) ? 1 : 0;
            this.intAdequarPotenciaCarga = (this.clbOptionsRun.CheckedItems.Contains("Limitar Cargas BT (Potência ativa do transformador)") == true) ? 1 : 0;
            if (this.tbVPUMin.Text == "")
                this.tbVPUMin.Text = "0,5";
            this.dblVPUMin = Convert.ToDouble(this.tbVPUMin.Text.Replace(".", ","));
            this.intAdequarTrafoVazio = (this.clbOptionsRun.CheckedItems.Contains("Eliminar Transformadores Vazios") == true) ? 1 : 0;
            this.intAdequarModeloCarga = this.cbModel.SelectedIndex + 1;
            this.intNeutralizarTrafoTerceiros = (this.clbOptionsRun.CheckedItems.Contains("Neutralizar Transformadores de Terceiros") == true) ? 1 : 0;
            this.intNeutralizarRedeTerceiros = (this.clbOptionsRun.CheckedItems.Contains("Neutralizar Redes de Terceiros (MT/BT)") == true) ? 1 : 0;
        }

        private String mthdGetOptionsToString()
        {
            String result = "";

            result += (this.intRealizaCnvrgcPNT == 1 ? "N" : "-");
            result += (this.intUsaTrafoABNT == 1 ? "T" : "-");
            result += (this.intAdequarTensaoCargasMT == 1 ? "M" : "-");
            result += (this.intAdequarTensaoCargasBT == 1 ? "B" : "-");
            result += (this.intAdequarTensaoSuperior == 1 ? "S" : "-");
            result += (this.intAdequarRamal == 1 ? "R" : "-");
            result += (this.intAdequarModeloCarga == 1 ? "1" : (this.intAdequarModeloCarga == 2 ? "2" : (this.intAdequarModeloCarga == 3 ? "3" : "-")));
            result += (this.intAdequarPotenciaCarga == 1 ? "P" : "-");
            result += (this.intAdequarTrafoVazio == 1 ? "V" : "-");
            result += (this.intAdequarTapTrafo == 1 ? "T" : "-");
            result += (this.intNeutralizarTrafoTerceiros == 1 ? "T" : "-");
            result += (this.intNeutralizarRedeTerceiros == 1 ? "R" : "-");

            return result;
        }

        private void bExportDB_Click(object sender, EventArgs e)
        {

            int i;
            String strPath;
            String strTensoesBase;

            if (checkBase())
            {
                try
                {

                    // Obtem a pasta onde os arquivos serão gravados
                    if (this.fbdSelectPathWrite.ShowDialog() == DialogResult.Cancel)
                        return;
                    else
                        strPath = this.fbdSelectPathWrite.SelectedPath;

                    // Captura a base a ser calculada
                    this.strBaseSelected = this.tbObjectBase.Text;

                    // Loop da base e dos alimentadores
                    this.strFeedersSelected.Clear();
                    if (this.tbObjectFeeder.Text == null || this.tbObjectFeeder.Text == "")
                        this.sqlCommServer = new SqlCommand("SELECT t1.* FROM dbo.[" + this.strBaseSelected + "CircMT] AS t1 WHERE t1.CodBase = " + this.strBaseSelected, this.sqlConnServer);
                    else
                        this.sqlCommServer = new SqlCommand("SELECT t1.* FROM dbo.[" + this.strBaseSelected + "CircMT] AS t1 WHERE t1.CodBase = " + this.strBaseSelected + " AND " + GeoPerdasTools.mthdQuerySQLFeeders(this.tbObjectFeeder.Text), this.sqlConnServer);
                    this.sqlCommServer.CommandTimeout = 0;
                    this.sqlDtRdrServer = this.sqlCommServer.ExecuteReader();
                    while (this.sqlDtRdrServer.Read())
                    {
                        this.strFeedersSelected.Add(this.sqlDtRdrServer["CodAlim"].ToString());
                    }
                    this.sqlDtRdrServer.Close();

                    foreach (String strFeeder in strFeedersSelected)
                    {

                        // Início do processo
                        this.mthdInputMensageLog("Início do processo para o alimentador " + strFeeder + ".");

                        // Verifica as opções solicitadas pelo usuário
                        this.mthdUpdateOptionsRun();

                        // Loop dos códigos dos circuitos
                        this.sqlCommServer = new SqlCommand("SELECT t1.* FROM dbo.[" + this.strBaseSelected + "CircMT] AS t1 WHERE t1.CodBase = " + this.strBaseSelected + " AND t1.CodAlim = '" + strFeeder + "'", this.sqlConnServer);
                        this.sqlCommServer.CommandTimeout = 0;
                        this.strFileData[0] = "! Criação da seção de circuitos";
                        i = 1;
                        this.sqlDtRdrServer = this.sqlCommServer.ExecuteReader();
                        while (this.sqlDtRdrServer.Read())
                        {
                            this.strFileData[i] = GeoPerdasTools.mthdWriteLanguageDSSCommandCircuit(strFeeder, Convert.ToDouble(this.sqlDtRdrServer["TenNom_kV"]), Convert.ToDouble(this.sqlDtRdrServer["TenOpe_pu"]), this.sqlDtRdrServer["CodPonAcopl"].ToString(), this.intAdequarTensaoSuperior);
                            i++;
                        }
                        this.sqlDtRdrServer.Close();
                        this.strFileData[i] = "<FIM DOS DADOS>";
                        using (swFile = new System.IO.StreamWriter(strPath + @"\CircuitoMT_" + this.strBaseSelected + "_" + strFeeder + "_" + this.mthdGetOptionsToString() + ".dss", false, Encoding.GetEncoding(1252)))
                        {
                            foreach (String strLine in this.strFileData)
                            {
                                if (strLine != "<FIM DOS DADOS>")
                                    swFile.WriteLine(strLine);
                                else
                                    break;
                            }
                        }
                        this.mthdInputMensageLog("Passou pelo circuito.");

                        // Loop dos códigos dos condutores
                        this.sqlCommServer = new SqlCommand("SELECT t1.* FROM dbo.[" + this.strBaseSelected + "CodCondutor] AS t1 WHERE t1.CodBase = " + this.strBaseSelected, this.sqlConnServer);
                        this.sqlCommServer.CommandTimeout = 0;
                        this.strFileData[0] = "! Criação da seção de condutores";
                        i = 1;
                        this.sqlDtRdrServer = this.sqlCommServer.ExecuteReader();
                        while (this.sqlDtRdrServer.Read())
                        {
                            this.strFileData[i] = this.mthdWriteLanguageDSSCommandLinecode(this.sqlDtRdrServer["CodCond"].ToString(), Convert.ToDouble(this.sqlDtRdrServer["Resis_ohms_km"]), Convert.ToDouble(this.sqlDtRdrServer["Reat_ohms_km"]), Convert.ToDouble(this.sqlDtRdrServer["CorrMax_A"]));
                            i++;
                        }
                        this.sqlDtRdrServer.Close();
                        this.strFileData[i] = "<FIM DOS DADOS>";
                        using (swFile = new System.IO.StreamWriter(strPath + @"\CodCondutor_" + this.strBaseSelected + "_" + strFeeder + "_" + this.mthdGetOptionsToString() + ".dss", false, Encoding.GetEncoding(1252)))
                        {
                            foreach (String strLine in this.strFileData)
                            {
                                if (strLine != "<FIM DOS DADOS>")
                                    swFile.WriteLine(strLine);
                                else
                                    break;
                            }
                        }
                        this.mthdInputMensageLog("Passou pelos condutores.");

                        // Loop dos códigos das curvas MT
                        this.sqlCommServer = new SqlCommand("SELECT t1.* FROM dbo.AuxCrvCrgMTHor AS t1 WHERE t1.CodBase = " + this.strBaseSelected, this.sqlConnServer);
                        this.sqlCommServer.CommandTimeout = 0;
                        this.strFileData[0] = "! Criação da seção de curvas MT";
                        i = 1;
                        this.sqlDtRdrServer = this.sqlCommServer.ExecuteReader();
                        while (this.sqlDtRdrServer.Read())
                        {
                            this.strFileData[i] = GeoPerdasTools.mthdWriteLanguageDSSCommandLoadshape(this.sqlDtRdrServer["CodCrvCrg"].ToString() + "_" + this.sqlDtRdrServer["TipoDia"].ToString(), Convert.ToDouble(this.sqlDtRdrServer["PotAtvNorm01"]), Convert.ToDouble(this.sqlDtRdrServer["PotAtvNorm02"]), Convert.ToDouble(this.sqlDtRdrServer["PotAtvNorm03"]), Convert.ToDouble(this.sqlDtRdrServer["PotAtvNorm04"]), Convert.ToDouble(this.sqlDtRdrServer["PotAtvNorm05"]), Convert.ToDouble(this.sqlDtRdrServer["PotAtvNorm06"]), Convert.ToDouble(this.sqlDtRdrServer["PotAtvNorm07"]), Convert.ToDouble(this.sqlDtRdrServer["PotAtvNorm08"]), Convert.ToDouble(this.sqlDtRdrServer["PotAtvNorm09"]), Convert.ToDouble(this.sqlDtRdrServer["PotAtvNorm10"]), Convert.ToDouble(this.sqlDtRdrServer["PotAtvNorm11"]), Convert.ToDouble(this.sqlDtRdrServer["PotAtvNorm12"]), Convert.ToDouble(this.sqlDtRdrServer["PotAtvNorm13"]), Convert.ToDouble(this.sqlDtRdrServer["PotAtvNorm14"]), Convert.ToDouble(this.sqlDtRdrServer["PotAtvNorm15"]), Convert.ToDouble(this.sqlDtRdrServer["PotAtvNorm16"]), Convert.ToDouble(this.sqlDtRdrServer["PotAtvNorm17"]), Convert.ToDouble(this.sqlDtRdrServer["PotAtvNorm18"]), Convert.ToDouble(this.sqlDtRdrServer["PotAtvNorm19"]), Convert.ToDouble(this.sqlDtRdrServer["PotAtvNorm20"]), Convert.ToDouble(this.sqlDtRdrServer["PotAtvNorm21"]), Convert.ToDouble(this.sqlDtRdrServer["PotAtvNorm22"]), Convert.ToDouble(this.sqlDtRdrServer["PotAtvNorm23"]), Convert.ToDouble(this.sqlDtRdrServer["PotAtvNorm24"]));
                            i++;
                        }
                        this.sqlDtRdrServer.Close();
                        this.strFileData[i] = "<FIM DOS DADOS>";
                        using (swFile = new System.IO.StreamWriter(strPath + @"\CurvacargaMT_" + this.strBaseSelected + "_" + strFeeder + "_" + this.mthdGetOptionsToString() + ".dss", false, Encoding.GetEncoding(1252)))
                        {
                            foreach (String strLine in this.strFileData)
                            {
                                if (strLine != "<FIM DOS DADOS>")
                                    swFile.WriteLine(strLine);
                                else
                                    break;
                            }
                        }
                        this.mthdInputMensageLog("Passou pelas curvas MT.");

                        // Loop dos códigos das curvas BT
                        this.sqlCommServer = new SqlCommand("SELECT t1.* FROM dbo.AuxCrvCrgBTHor AS t1 WHERE t1.CodBase = " + this.strBaseSelected, this.sqlConnServer);
                        this.sqlCommServer.CommandTimeout = 0;
                        this.strFileData[0] = "! Criação da seção de curvas BT";
                        i = 1;
                        this.sqlDtRdrServer = this.sqlCommServer.ExecuteReader();
                        while (this.sqlDtRdrServer.Read())
                        {
                            this.strFileData[i] = GeoPerdasTools.mthdWriteLanguageDSSCommandLoadshape(this.sqlDtRdrServer["CodCrvCrg"].ToString() + "_" + this.sqlDtRdrServer["TipoDia"].ToString(), Convert.ToDouble(this.sqlDtRdrServer["PotAtvNorm01"]), Convert.ToDouble(this.sqlDtRdrServer["PotAtvNorm02"]), Convert.ToDouble(this.sqlDtRdrServer["PotAtvNorm03"]), Convert.ToDouble(this.sqlDtRdrServer["PotAtvNorm04"]), Convert.ToDouble(this.sqlDtRdrServer["PotAtvNorm05"]), Convert.ToDouble(this.sqlDtRdrServer["PotAtvNorm06"]), Convert.ToDouble(this.sqlDtRdrServer["PotAtvNorm07"]), Convert.ToDouble(this.sqlDtRdrServer["PotAtvNorm08"]), Convert.ToDouble(this.sqlDtRdrServer["PotAtvNorm09"]), Convert.ToDouble(this.sqlDtRdrServer["PotAtvNorm10"]), Convert.ToDouble(this.sqlDtRdrServer["PotAtvNorm11"]), Convert.ToDouble(this.sqlDtRdrServer["PotAtvNorm12"]), Convert.ToDouble(this.sqlDtRdrServer["PotAtvNorm13"]), Convert.ToDouble(this.sqlDtRdrServer["PotAtvNorm14"]), Convert.ToDouble(this.sqlDtRdrServer["PotAtvNorm15"]), Convert.ToDouble(this.sqlDtRdrServer["PotAtvNorm16"]), Convert.ToDouble(this.sqlDtRdrServer["PotAtvNorm17"]), Convert.ToDouble(this.sqlDtRdrServer["PotAtvNorm18"]), Convert.ToDouble(this.sqlDtRdrServer["PotAtvNorm19"]), Convert.ToDouble(this.sqlDtRdrServer["PotAtvNorm20"]), Convert.ToDouble(this.sqlDtRdrServer["PotAtvNorm21"]), Convert.ToDouble(this.sqlDtRdrServer["PotAtvNorm22"]), Convert.ToDouble(this.sqlDtRdrServer["PotAtvNorm23"]), Convert.ToDouble(this.sqlDtRdrServer["PotAtvNorm24"]));
                            i++;
                        }
                        this.sqlDtRdrServer.Close();
                        this.strFileData[i] = "<FIM DOS DADOS>";
                        using (swFile = new System.IO.StreamWriter(strPath + @"\CurvacargaBT_" + this.strBaseSelected + "_" + strFeeder + "_" + this.mthdGetOptionsToString() + ".dss", false, Encoding.GetEncoding(1252)))
                        {
                            foreach (String strLine in this.strFileData)
                            {
                                if (strLine != "<FIM DOS DADOS>")
                                    swFile.WriteLine(strLine);
                                else
                                    break;
                            }
                        }
                        this.mthdInputMensageLog("Passou pelas curvas BT.");

                        // Loop dos chaves MT
                        this.sqlCommServer = new SqlCommand("SELECT t1.CodChvMT, t1.CodFas, t1.De, t1.Para FROM dbo.[" + this.strBaseSelected + "ChaveMT] AS t1 WHERE t1.CodBase = " + this.strBaseSelected + " AND t1.CodAlimAtrib = '" + strFeeder + "' AND t1.EstChv = 2", this.sqlConnServer);
                        this.sqlCommServer.CommandTimeout = 0;
                        this.strFileData[0] = "! Criação da seção de chaves MT";
                        i = 1;
                        this.sqlDtRdrServer = this.sqlCommServer.ExecuteReader();
                        while (this.sqlDtRdrServer.Read())
                        {
                            this.strFileData[i] = GeoPerdasTools.mthdWriteLanguageDSSCommandMTSwitch("CMT_" + this.sqlDtRdrServer["CodChvMT"].ToString(), this.sqlDtRdrServer["CodFas"].ToString(), this.sqlDtRdrServer["De"].ToString(), this.sqlDtRdrServer["Para"].ToString());
                            i++;
                        }
                        this.sqlDtRdrServer.Close();
                        this.strFileData[i] = "<FIM DOS DADOS>";
                        using (swFile = new System.IO.StreamWriter(strPath + @"\ChavesMT_" + this.strBaseSelected + "_" + strFeeder + "_" + this.mthdGetOptionsToString() + ".dss", false, Encoding.GetEncoding(1252)))
                        {
                            foreach (String strLine in this.strFileData)
                            {
                                if (strLine != "<FIM DOS DADOS>")
                                    swFile.WriteLine(strLine);
                                else
                                    break;
                            }
                        }
                        this.mthdInputMensageLog("Passou pelas chaves MT.");

                        // Loop dos segmentos MT
                        this.sqlCommServer = new SqlCommand("SELECT t1.CodSegmMT, t1.CodFas, t1.CodCond, t1.Comp_km, t1.De, t1.Para, t1.Propr FROM dbo.[" + this.strBaseSelected + "SegmentoMT] AS t1 WHERE t1.CodBase = " + this.strBaseSelected + " AND t1.CodAlimAtrib = '" + strFeeder + "'", this.sqlConnServer);
                        this.sqlCommServer.CommandTimeout = 0;
                        this.strFileData[0] = "! Criação da seção de segmentos MT";
                        i = 1;
                        this.sqlDtRdrServer = this.sqlCommServer.ExecuteReader();
                        while (this.sqlDtRdrServer.Read())
                        {
                            this.strFileData[i] = GeoPerdasTools.mthdWriteLanguageDSSCommandLineMT("SMT_" + this.sqlDtRdrServer["CodSegmMT"].ToString(), this.sqlDtRdrServer["CodFas"].ToString(), this.sqlDtRdrServer["De"].ToString(), this.sqlDtRdrServer["Para"].ToString(), this.sqlDtRdrServer["CodCond"].ToString(), Convert.ToDouble(this.sqlDtRdrServer["Comp_km"]), this.sqlDtRdrServer["Propr"].ToString(), 0, intNeutralizarRedeTerceiros);
                            i++;
                        }
                        this.sqlDtRdrServer.Close();
                        this.strFileData[i] = "<FIM DOS DADOS>";
                        using (swFile = new System.IO.StreamWriter(strPath + @"\SegmentosMT_" + this.strBaseSelected + "_" + strFeeder + "_" + this.mthdGetOptionsToString() + ".dss", false, Encoding.GetEncoding(1252)))
                        {
                            foreach (String strLine in this.strFileData)
                            {
                                if (strLine != "<FIM DOS DADOS>")
                                    swFile.WriteLine(strLine);
                                else
                                    break;
                            }
                        }
                        this.mthdInputMensageLog("Passou pelos segmentos MT.");

                        // Loop dos reguladores MT
                        this.sqlCommServer = new SqlCommand("SELECT t1.CodRegulMT, t1.CodBnc, t1.TipRegul, t1.CodFasPrim, t1.CodFasSecu, t1.PotNom_kVA, t1.TenRgl_pu, t1.[ReatHL_%], t1.PerdTtl_W, t1.PerdVz_W, t1.TnsLnh1_kV, t1.De, t1.Para, t1.CodFasCoinc FROM dbo.[" + this.strBaseSelected + "ReguladorMT] AS t1 WHERE t1.CodBase = " + this.strBaseSelected + " AND t1.CodAlimAtrib = '" + strFeeder + "'", this.sqlConnServer);
                        this.sqlCommServer.CommandTimeout = 0;
                        this.strFileData[0] = "! Criação da seção de reguladores MT";
                        i = 1;
                        this.sqlDtRdrServer = this.sqlCommServer.ExecuteReader();
                        while (this.sqlDtRdrServer.Read())
                        {
                            this.strFileData[i] = this.mthdWriteLanguageDSSCommandRegulatorMT("REG_" + this.sqlDtRdrServer["CodRegulMT"].ToString(), Convert.ToInt32(this.sqlDtRdrServer["CodBnc"]), Convert.ToInt32(this.sqlDtRdrServer["TipRegul"]), this.sqlDtRdrServer["CodFasPrim"].ToString(), this.sqlDtRdrServer["CodFasSecu"].ToString(), this.sqlDtRdrServer["CodFasCoinc"].ToString(), this.sqlDtRdrServer["De"].ToString(), this.sqlDtRdrServer["Para"].ToString(), Convert.ToDouble(this.sqlDtRdrServer["TnsLnh1_kV"]), Convert.ToDouble(this.sqlDtRdrServer["PotNom_kVA"]), Convert.ToDouble(this.sqlDtRdrServer["TenRgl_pu"]), Convert.ToDouble(this.sqlDtRdrServer["ReatHL_%"]), Convert.ToDouble(this.sqlDtRdrServer["PerdTtl_W"]) / (1000 * Convert.ToDouble(this.sqlDtRdrServer["PotNom_kVA"])), Convert.ToDouble(this.sqlDtRdrServer["PerdVz_W"]) / (1000 * Convert.ToDouble(this.sqlDtRdrServer["PotNom_kVA"])));
                            i++;
                        }
                        this.sqlDtRdrServer.Close();
                        this.strFileData[i] = "<FIM DOS DADOS>";
                        using (swFile = new System.IO.StreamWriter(strPath + @"\ReguladorMT_" + this.strBaseSelected + "_" + strFeeder + "_" + this.mthdGetOptionsToString() + ".dss", false, Encoding.GetEncoding(1252)))
                        {
                            foreach (String strLine in this.strFileData)
                            {
                                if (strLine != "<FIM DOS DADOS>")
                                    swFile.WriteLine(strLine);
                                else
                                    break;
                            }
                        }
                        this.mthdInputMensageLog("Passou pelos reguladores MT.");

                        // Loop dos transformadores MT-MT ou MT-BT
                        this.sqlCommServer = new SqlCommand("SELECT t1.* FROM dbo.[" + this.strBaseSelected + "TrafoMTMTMTBT] AS t1 WHERE t1.CodBase = " + this.strBaseSelected + " AND t1.CodAlimAtrib = '" + strFeeder + "'", this.sqlConnServer);
                        this.sqlCommServer.CommandTimeout = 0;
                        this.strFileData[0] = "! Criação da seção de transformadores MT-MT ou MT-BT";
                        i = 1;
                        this.sqlDtRdrServer = this.sqlCommServer.ExecuteReader();
                        while (this.sqlDtRdrServer.Read())
                        {
                            this.strFileData[i] = this.mthdWriteLanguageDSSCommandTransformer("TRF_" + this.sqlDtRdrServer["CodTrafo"].ToString(), Convert.ToInt32(this.sqlDtRdrServer["CodBnc"]), Convert.ToInt32(this.sqlDtRdrServer["MRT"]), Convert.ToInt32(this.sqlDtRdrServer["TipTrafo"]), this.sqlDtRdrServer["CodFasPrim"].ToString(), this.sqlDtRdrServer["CodFasSecu"].ToString(), this.sqlDtRdrServer["CodFasTerc"].ToString(), this.sqlDtRdrServer["De"].ToString(), this.sqlDtRdrServer["Para"].ToString(), Convert.ToDouble(this.sqlDtRdrServer["TnsLnh1_kV"]), Convert.ToDouble(this.sqlDtRdrServer["TenSecu_kV"]), Convert.ToDouble(this.sqlDtRdrServer["Tap_pu"]), Convert.ToDouble(this.sqlDtRdrServer["PotNom_kVA"]), (GeoPerdasTools.mthdTransformerTotalPowerLoss2019_per(this.sqlDtRdrServer["CodFasPrim"].ToString(), Convert.ToDouble(this.sqlDtRdrServer["PotNom_kVA"]), Convert.ToDouble(this.sqlDtRdrServer["TnsLnh1_kV"])) != 0 && this.intUsaTrafoABNT == 1) ? GeoPerdasTools.mthdTransformerTotalPowerLoss2019_per(this.sqlDtRdrServer["CodFasPrim"].ToString(), Convert.ToDouble(this.sqlDtRdrServer["PotNom_kVA"]), Convert.ToDouble(this.sqlDtRdrServer["TnsLnh1_kV"])) : Convert.ToDouble(this.sqlDtRdrServer["PerdTtl_%"]), (GeoPerdasTools.mthdTransformerNoLoadPowerLoss2019_per(this.sqlDtRdrServer["CodFasPrim"].ToString(), Convert.ToDouble(this.sqlDtRdrServer["PotNom_kVA"]), Convert.ToDouble(this.sqlDtRdrServer["TnsLnh1_kV"])) != 0 && this.intUsaTrafoABNT == 1) ? GeoPerdasTools.mthdTransformerNoLoadPowerLoss2019_per(this.sqlDtRdrServer["CodFasPrim"].ToString(), Convert.ToDouble(this.sqlDtRdrServer["PotNom_kVA"]), Convert.ToDouble(this.sqlDtRdrServer["TnsLnh1_kV"])) : Convert.ToDouble(this.sqlDtRdrServer["PerdVz_%"]), this.sqlDtRdrServer["Propr"].ToString(), (this.intAdequarTrafoVazio == 1) ? Convert.ToInt32(this.sqlDtRdrServer["SemCarga"]) : 0);
                            i++;
                        }
                        this.sqlDtRdrServer.Close();
                        this.strFileData[i] = "<FIM DOS DADOS>";
                        using (swFile = new System.IO.StreamWriter(strPath + @"\TransformadorMTMTMTBT_" + this.strBaseSelected + "_" + strFeeder + "_" + this.mthdGetOptionsToString() + ".dss", false, Encoding.GetEncoding(1252)))
                        {
                            foreach (String strLine in this.strFileData)
                            {
                                if (strLine != "<FIM DOS DADOS>")
                                    swFile.WriteLine(strLine);
                                else
                                    break;
                            }
                        }
                        this.mthdInputMensageLog("Passou pelos transformadores MT-MT ou MT-BT.");

                        // Loop dos chaves BT
                        this.sqlCommServer = new SqlCommand("SELECT t1.CodChvBT, t1.CodFas, t1.De, t1.Para FROM dbo.[" + this.strBaseSelected + "ChaveBT] AS t1 WHERE t1.CodBase = " + this.strBaseSelected + " AND t1.CodAlimAtrib = '" + strFeeder + "' AND t1.EstChv = 2", this.sqlConnServer);
                        this.sqlCommServer.CommandTimeout = 0;
                        this.strFileData[0] = "! Criação da seção de chaves BT";
                        i = 1;
                        this.sqlDtRdrServer = this.sqlCommServer.ExecuteReader();
                        while (this.sqlDtRdrServer.Read())
                        {
                            this.strFileData[i] = GeoPerdasTools.mthdWriteLanguageDSSCommandBTSwitch("CBT_" + this.sqlDtRdrServer["CodChvBT"].ToString(), this.sqlDtRdrServer["CodFas"].ToString(), this.sqlDtRdrServer["De"].ToString(), this.sqlDtRdrServer["Para"].ToString());
                            i++;
                        }
                        this.sqlDtRdrServer.Close();
                        this.strFileData[i] = "<FIM DOS DADOS>";
                        using (swFile = new System.IO.StreamWriter(strPath + @"\ChavesBT_" + this.strBaseSelected + "_" + strFeeder + "_" + this.mthdGetOptionsToString() + ".dss", false, Encoding.GetEncoding(1252)))
                        {
                            foreach (String strLine in this.strFileData)
                            {
                                if (strLine != "<FIM DOS DADOS>")
                                    swFile.WriteLine(strLine);
                                else
                                    break;
                            }
                        }
                        this.mthdInputMensageLog("Passou pelas chaves BT.");

                        // Loop dos segmentos BT
                        this.sqlCommServer = new SqlCommand("SELECT t1.*, [dbo].[EscolhaPropriedade]([t2].[M1_Propr], [t2].[M2_Propr], [t2].[M3_Propr]) AS [Propr] FROM dbo.[" + this.strBaseSelected + "SegmentoBT] AS t1 LEFT JOIN [dbo].[" + this.strBaseSelected + "AuxTrafoMTMTMTBT] AS t2 ON ([t1].[CodBase] = [t2].[M1_CodBase] AND [t1].[CodTrafoAtrib] = [t2].[M1_CodTrafo]) WHERE t1.CodBase = " + this.strBaseSelected + " AND t1.CodAlimAtrib = '" + strFeeder + "'", this.sqlConnServer);
                        this.sqlCommServer.CommandTimeout = 0;
                        this.strFileData[0] = "! Criação da seção de segmentos BT";
                        i = 1;
                        this.sqlDtRdrServer = this.sqlCommServer.ExecuteReader();
                        while (this.sqlDtRdrServer.Read())
                        {
                            this.strFileData[i] = GeoPerdasTools.mthdWriteLanguageDSSCommandLineBT("SBT_" + this.sqlDtRdrServer["CodSegmBT"].ToString(), this.sqlDtRdrServer["CodFas"].ToString(), this.sqlDtRdrServer["CodPonAcopl1"].ToString(), this.sqlDtRdrServer["CodPonAcopl2"].ToString(), this.sqlDtRdrServer["CodCond"].ToString(), Convert.ToDouble(this.sqlDtRdrServer["Comp_km"]), this.sqlDtRdrServer["Propr"].ToString(), 0, intNeutralizarRedeTerceiros);
                            i++;
                        }
                        this.sqlDtRdrServer.Close();
                        this.strFileData[i] = "<FIM DOS DADOS>";
                        using (swFile = new System.IO.StreamWriter(strPath + @"\SegmentosBT_" + this.strBaseSelected + "_" + strFeeder + "_" + this.mthdGetOptionsToString() + ".dss", false, Encoding.GetEncoding(1252)))
                        {
                            foreach (String strLine in this.strFileData)
                            {
                                if (strLine != "<FIM DOS DADOS>")
                                    swFile.WriteLine(strLine);
                                else
                                    break;
                            }
                        }
                        this.mthdInputMensageLog("Passou pelos segmentos BT.");

                        // Loop dos ramais
                        this.sqlCommServer = new SqlCommand("SELECT t1.*, [dbo].[EscolhaPropriedade]([t2].[M1_Propr], [t2].[M2_Propr], [t2].[M3_Propr]) AS [Propr] FROM dbo.[" + this.strBaseSelected + "RamalBT] AS t1 LEFT JOIN [dbo].[" + this.strBaseSelected + "AuxTrafoMTMTMTBT] AS t2 ON ([t1].[CodBase] = [t2].[M1_CodBase] AND [t1].[CodTrafoAtrib] = [t2].[M1_CodTrafo]) WHERE t1.CodBase = " + this.strBaseSelected + " AND t1.CodAlimAtrib = '" + strFeeder + "'", this.sqlConnServer);
                        this.sqlCommServer.CommandTimeout = 0;
                        this.strFileData[0] = "! Criação da seção de ramais";
                        i = 1;
                        this.sqlDtRdrServer = this.sqlCommServer.ExecuteReader();
                        while (this.sqlDtRdrServer.Read())
                        {
                            this.strFileData[i] = GeoPerdasTools.mthdWriteLanguageDSSCommandLineBT("RBT_" + this.sqlDtRdrServer["CodRmlBT"].ToString(), this.sqlDtRdrServer["CodFas"].ToString(), this.sqlDtRdrServer["CodPonAcopl1"].ToString(), this.sqlDtRdrServer["CodPonAcopl2"].ToString(), this.sqlDtRdrServer["CodCond"].ToString(), Convert.ToDouble(this.sqlDtRdrServer["Comp_km"]), this.sqlDtRdrServer["Propr"].ToString(), this.intAdequarRamal, this.intNeutralizarRedeTerceiros);
                            i++;
                        }
                        this.sqlDtRdrServer.Close();
                        this.strFileData[i] = "<FIM DOS DADOS>";
                        using (swFile = new System.IO.StreamWriter(strPath + @"\RamaisBT_" + this.strBaseSelected + "_" + strFeeder + "_" + this.mthdGetOptionsToString() + ".dss", false, Encoding.GetEncoding(1252)))
                        {
                            foreach (String strLine in this.strFileData)
                            {
                                if (strLine != "<FIM DOS DADOS>")
                                    swFile.WriteLine(strLine);
                                else
                                    break;
                            }
                        }
                        this.mthdInputMensageLog("Passou pelos ramais.");

                        // Versão usando o bus
                        if (this.cbMeterComplete.Checked == false)
                        {
                            // Loop dos energymeters de barramento
                            i = 0;
                            this.sqlCommServer = new SqlCommand("SELECT t1.* FROM dbo.AuxMeterBarra AS t1 WHERE t1.CodBase = " + this.strBaseSelected + " AND t1.CodAlim = '" + strFeeder + "'", this.sqlConnServer);
                            this.sqlCommServer.CommandTimeout = 0;
                            this.strFileData[i] = "! Criação da seção de energymeters de barramento";
                            i++;
                            this.sqlDtRdrServer = this.sqlCommServer.ExecuteReader();
                            while (this.sqlDtRdrServer.Read())
                            {
                                this.strFileData[i] = GeoPerdasTools.mthdWriteLanguageDSSCommandEnergymeter(this.sqlDtRdrServer["Nome"].ToString().TrimEnd(), this.mthdNomeElemento(this.sqlDtRdrServer["Elem"].ToString().TrimEnd()) + this.sqlDtRdrServer["CodNomeElem"].ToString().TrimEnd());
                                i++;
                            }
                            this.sqlDtRdrServer.Close();
                            this.strFileData[i] = "<FIM DOS DADOS>";
                            using (swFile = new System.IO.StreamWriter(strPath + @"\Medidores_" + this.strBaseSelected + "_" + strFeeder + "_" + this.mthdGetOptionsToString() + ".dss", false, Encoding.GetEncoding(1252)))
                            {
                                foreach (String strLine in this.strFileData)
                                {
                                    if (strLine != "<FIM DOS DADOS>")
                                        swFile.WriteLine(strLine);
                                    else
                                        break;
                                }
                            }
                            this.mthdInputMensageLog("Passou pelos energymeters de barramento.");
                        }

                        // Versão usando a tabela auxiliar de medidores
                        if (this.cbMeterComplete.Checked == true)
                        {

                            // Loop dos energymeters usando AuxMeter
                            this.sqlCommServer = new SqlCommand("SELECT t1.* FROM dbo.AuxMeterCompleto AS t1 WHERE t1.CodBase = " + this.strBaseSelected + " AND t1.CodAlim = '" + strFeeder + "'", this.sqlConnServer);
                            this.sqlCommServer.CommandTimeout = 0;
                            this.strFileData[0] = "! Criação da seção de energymeters";
                            i = 1;
                            this.sqlDtRdrServer = this.sqlCommServer.ExecuteReader();
                            while (this.sqlDtRdrServer.Read())
                            {
                                this.strFileData[i] = GeoPerdasTools.mthdWriteLanguageDSSCommandEnergymeter(this.sqlDtRdrServer["Nome"].ToString().TrimEnd(), this.mthdNomeElemento(this.sqlDtRdrServer["Elem"].ToString().TrimEnd()) + this.sqlDtRdrServer["CodNomeElem"].ToString().TrimEnd());
                                i++;
                            }
                            this.sqlDtRdrServer.Close();
                            this.strFileData[i] = "<FIM DOS DADOS>";
                            using (swFile = new System.IO.StreamWriter(strPath + @"\Medidores_" + this.strBaseSelected + "_" + strFeeder + "_" + this.mthdGetOptionsToString() + ".dss", false, Encoding.GetEncoding(1252)))
                            {
                                foreach (String strLine in this.strFileData)
                                {
                                    if (strLine != "<FIM DOS DADOS>")
                                        swFile.WriteLine(strLine);
                                    else
                                        break;
                                }
                            }
                            this.mthdInputMensageLog("Passou pelos energymeters.");
                        }

                        // Loop das tensões de base
                        strTensoesBase = "Set voltagebases=[";
                        i = 0;
                        this.sqlCommServer = new SqlCommand("SELECT t1.* FROM dbo.AuxVoltageBases AS t1 WHERE t1.CodBase = " + this.strBaseSelected + " AND t1.CodAlim = '" + strFeeder + "'", this.sqlConnServer);
                        this.sqlCommServer.CommandTimeout = 0;
                        this.strFileData[0] = "! Criação da seção de tensões de base";
                        this.sqlDtRdrServer = this.sqlCommServer.ExecuteReader();
                        while (this.sqlDtRdrServer.Read())
                        {
                            if (i == 0)
                                strTensoesBase += this.sqlDtRdrServer["TnsLnh_kV"].ToString().Replace(",", ".");
                            else
                                strTensoesBase += " " + this.sqlDtRdrServer["TnsLnh_kV"].ToString().Replace(",", ".");
                            i++;
                        }
                        this.sqlDtRdrServer.Close();
                        strTensoesBase += "]";
                        this.strFileData[1] = strTensoesBase.Replace(",", ".");
                        this.strFileData[2] = "Calcvoltagebases";
                        this.sqlCommServer = new SqlCommand("SELECT t1.* FROM dbo.AuxTensaoBaseBT AS t1 WHERE t1.CodBase = " + this.strBaseSelected + " AND t1.CodAlim = '" + strFeeder + "'", this.sqlConnServer);
                        this.sqlCommServer.CommandTimeout = 0;
                        this.strFileData[3] = "! Criação da seção de reset das tensões de base do BT";
                        this.sqlDtRdrServer = this.sqlCommServer.ExecuteReader();
                        i = 4;
                        while (this.sqlDtRdrServer.Read())
                        {
                            this.strFileData[i] = "!Setkvbase Bus=\"" + this.sqlDtRdrServer["CodPonAcopl"] + "\" kvln=" + this.sqlDtRdrServer["TnsFasBas_kV"].ToString().Replace(",", ".");
                            i++;
                        }
                        this.sqlDtRdrServer.Close();
                        this.strFileData[i] = "<FIM DOS DADOS>";
                        using (swFile = new System.IO.StreamWriter(strPath + @"\Tensoesbase_" + this.strBaseSelected + "_" + strFeeder + "_" + this.mthdGetOptionsToString() + ".dss", false, Encoding.GetEncoding(1252)))
                        {
                            foreach (String strLine in this.strFileData)
                            {
                                if (strLine != "<FIM DOS DADOS>")
                                    swFile.WriteLine(strLine);
                                else
                                    break;
                            }
                        }
                        this.mthdInputMensageLog("Passou pelas tensões de base.");

                        for (int j = 1; j <= 12; j++)
                        {
                            for (int k = 1; k <= 3; k++)
                            {

                                // Loop das cargas MT
                                this.sqlCommServer = new SqlCommand("SELECT DISTINCT t1.CodBase, t1.CodConsMT, t1.CodFas, t1.CodPonAcopl, t1.TipCrvaCarga, t1.DemMax" + GeoPerdasTools.mthdGetTypeDayMonth(j, k) + "_kW, t1.CodAlimAtrib, t1.TnsLnh_kV FROM dbo.[" + this.strBaseSelected + "AuxCargaMTDem] AS t1 WHERE t1.CodBase = " + this.strBaseSelected + " AND t1.CodAlimAtrib = '" + strFeeder + "'", this.sqlConnServer);
                                this.sqlCommServer.CommandTimeout = 0;
                                this.strFileData[0] = "! Criação da seção de cargas MT";
                                i = 1;
                                this.sqlDtRdrServer = this.sqlCommServer.ExecuteReader();
                                while (this.sqlDtRdrServer.Read())
                                {
                                    this.strFileData[i] = this.mthdWriteLanguageDSSCommandNewLoad("MT_" + this.sqlDtRdrServer["CodConsMT"].ToString(), this.sqlDtRdrServer["CodFas"].ToString(), this.sqlDtRdrServer["CodPonAcopl"].ToString(), Convert.ToDouble(this.sqlDtRdrServer["TnsLnh_kV"]), Convert.ToDouble(this.sqlDtRdrServer["TnsLnh_kV"]), Convert.ToDouble(this.sqlDtRdrServer["DemMax" + GeoPerdasTools.mthdGetTypeDayMonth(j, k) + "_kW"]), 0, this.sqlDtRdrServer["TipCrvaCarga"].ToString() + "_" + GeoPerdasTools.mthdGetTypeDay(k), 1, this.intAdequarTensaoCargasMT, 0);
                                    i++;
                                }
                                this.sqlDtRdrServer.Close();
                                this.strFileData[i] = "<FIM DOS DADOS>";
                                using (swFile = new System.IO.StreamWriter(strPath + @"\CargasMT_" + GeoPerdasTools.mthdGetTypeDayMonth(j, k) + "_" + this.strBaseSelected + "_" + strFeeder + "_" + this.mthdGetOptionsToString() + ".dss", false, Encoding.GetEncoding(1252)))
                                {
                                    foreach (String strLine in this.strFileData)
                                    {
                                        if (strLine != "<FIM DOS DADOS>")
                                            swFile.WriteLine(strLine);
                                        else
                                            break;
                                    }
                                }
                                this.mthdInputMensageLog("Passou pelas cargas MT.");

                                // Loop das cargas BT
                                this.sqlCommServer = new SqlCommand("SELECT DISTINCT t1.CodBase, t1.CodConsBT, t1.CodFas, t1.CodPonAcopl, t1.TipCrvaCarga, t1.DemMax" + GeoPerdasTools.mthdGetTypeDayMonth(j, k) + "_kW, 0.92 * (ISNULL([M1_PotNom_kVA],0)+ISNULL([M2_PotNom_kVA],0)+ISNULL([M3_PotNom_kVA],0)) AS DemMaxTrafo_kW, t1.CodAlimAtrib, t1.TnsLnh_kV, t2.TnsFasBas_kV FROM dbo.[" + this.strBaseSelected + "AuxCargaBTDem] AS t1 LEFT JOIN dbo.[" + this.strBaseSelected + "AuxTrafoMTMTMTBT] AS t2 ON (t1.CodBase = t2.M1_CodBase AND t1.CodTrafoAtrib = t2.M1_CodTrafo) WHERE t1.CodBase = " + this.strBaseSelected + " AND t1.CodAlimAtrib = '" + strFeeder + "'", this.sqlConnServer);
                                this.sqlCommServer.CommandTimeout = 0;
                                this.strFileData[0] = "! Criação da seção de cargas BT";
                                i = 1;
                                this.sqlDtRdrServer = this.sqlCommServer.ExecuteReader();
                                while (this.sqlDtRdrServer.Read())
                                {
                                    this.strFileData[i] = this.mthdWriteLanguageDSSCommandNewLoad("BT_" + this.sqlDtRdrServer["CodConsBT"].ToString(), this.sqlDtRdrServer["CodFas"].ToString(), this.sqlDtRdrServer["CodPonAcopl"].ToString(), Convert.ToDouble(this.sqlDtRdrServer["TnsLnh_kV"]), Convert.ToDouble(this.sqlDtRdrServer["TnsFasBas_kV"]), Convert.ToDouble(this.sqlDtRdrServer["DemMax" + GeoPerdasTools.mthdGetTypeDayMonth(j, k) + "_kW"]), Convert.ToDouble(this.sqlDtRdrServer["DemMaxTrafo_kW"]), this.sqlDtRdrServer["TipCrvaCarga"].ToString() + "_" + GeoPerdasTools.mthdGetTypeDay(k), 2, this.intAdequarTensaoCargasBT, 0);
                                    i++;
                                }
                                this.sqlDtRdrServer.Close();
                                this.strFileData[i] = "<FIM DOS DADOS>";
                                using (swFile = new System.IO.StreamWriter(strPath + @"\CargasBT_" + GeoPerdasTools.mthdGetTypeDayMonth(j, k) + "_" + this.strBaseSelected + "_" + strFeeder + "_" + this.mthdGetOptionsToString() + ".dss", false, Encoding.GetEncoding(1252)))
                                {
                                    foreach (String strLine in this.strFileData)
                                    {
                                        if (strLine != "<FIM DOS DADOS>")
                                            swFile.WriteLine(strLine);
                                        else
                                            break;
                                    }
                                }
                                this.mthdInputMensageLog("Passou pelas cargas BT.");

                                if (this.intRealizaCnvrgcPNT == 1)
                                {
                                    // Loop das cargas MT Não Técnicas
                                    this.sqlCommServer = new SqlCommand("SELECT DISTINCT t1.CodBase, t1.CodConsMT, t1.CodFas, t1.CodPonAcopl, t1.TipCrvaCarga, t1.DemMax" + GeoPerdasTools.mthdGetTypeDayMonth(j, k) + "_kW, t1.CodAlimAtrib, t1.TnsLnh_kV FROM dbo.[" + this.strBaseSelected + "AuxCargaMTNTDem] AS t1 WHERE t1.CodBase = " + this.strBaseSelected + " AND t1.CodAlimAtrib = '" + strFeeder + "'", this.sqlConnServer);
                                    this.sqlCommServer.CommandTimeout = 0;
                                    this.strFileData[0] = "! Criação da seção de cargas MT Não Técnicas";
                                    i = 1;
                                    this.sqlDtRdrServer = this.sqlCommServer.ExecuteReader();
                                    while (this.sqlDtRdrServer.Read())
                                    {
                                        this.strFileData[i] = this.mthdWriteLanguageDSSCommandNewLoad("MT_" + this.sqlDtRdrServer["CodConsMT"].ToString() + "_NT", this.sqlDtRdrServer["CodFas"].ToString(), this.sqlDtRdrServer["CodPonAcopl"].ToString(), Convert.ToDouble(this.sqlDtRdrServer["TnsLnh_kV"]), Convert.ToDouble(this.sqlDtRdrServer["TnsLnh_kV"]), Convert.ToDouble(this.sqlDtRdrServer["DemMax" + GeoPerdasTools.mthdGetTypeDayMonth(j, k) + "_kW"]), 0, this.sqlDtRdrServer["TipCrvaCarga"].ToString() + "_" + GeoPerdasTools.mthdGetTypeDay(k), 1, this.intAdequarTensaoCargasMT, 0);
                                        i++;
                                    }
                                    this.sqlDtRdrServer.Close();
                                    this.strFileData[i] = "<FIM DOS DADOS>";
                                    using (swFile = new System.IO.StreamWriter(strPath + @"\CargasMTNT_" + GeoPerdasTools.mthdGetTypeDayMonth(j, k) + "_" + this.strBaseSelected + "_" + strFeeder + "_" + this.mthdGetOptionsToString() + ".dss", false, Encoding.GetEncoding(1252)))
                                    {
                                        foreach (String strLine in this.strFileData)
                                        {
                                            if (strLine != "<FIM DOS DADOS>")
                                                swFile.WriteLine(strLine);
                                            else
                                                break;
                                        }
                                    }
                                    this.mthdInputMensageLog("Passou pelas cargas MT Não Técnicas.");

                                    // Loop das cargas BT Não Técnicas
                                    this.sqlCommServer = new SqlCommand("SELECT DISTINCT t1.CodBase, t1.CodConsBT, t1.CodFas, t1.CodPonAcopl, t1.TipCrvaCarga, t1.DemMax" + GeoPerdasTools.mthdGetTypeDayMonth(j, k) + "_kW, 0.92 * (ISNULL([M1_PotNom_kVA],0)+ISNULL([M2_PotNom_kVA],0)+ISNULL([M3_PotNom_kVA],0)) AS DemMaxTrafo_kW, t1.CodAlimAtrib, t1.TnsLnh_kV, t2.TnsFasBas_kV FROM dbo.[" + this.strBaseSelected + "AuxCargaBTNTDem] AS t1 LEFT JOIN dbo.[" + this.strBaseSelected + "AuxTrafoMTMTMTBT] AS t2 ON (t1.CodBase = t2.M1_CodBase AND t1.CodTrafoAtrib = t2.M1_CodTrafo) WHERE t1.CodBase = " + this.strBaseSelected + " AND t1.CodAlimAtrib = '" + strFeeder + "'", this.sqlConnServer);
                                    this.sqlCommServer.CommandTimeout = 0;
                                    this.strFileData[0] = "! Criação da seção de cargas BT Não Técnicas";
                                    i = 1;
                                    this.sqlDtRdrServer = this.sqlCommServer.ExecuteReader();
                                    while (this.sqlDtRdrServer.Read())
                                    {
                                        this.strFileData[i] = this.mthdWriteLanguageDSSCommandNewLoad("BT_" + this.sqlDtRdrServer["CodConsBT"].ToString() + "_NT", this.sqlDtRdrServer["CodFas"].ToString(), this.sqlDtRdrServer["CodPonAcopl"].ToString(), Convert.ToDouble(this.sqlDtRdrServer["TnsLnh_kV"]), Convert.ToDouble(this.sqlDtRdrServer["TnsFasBas_kV"]), Convert.ToDouble(this.sqlDtRdrServer["DemMax" + GeoPerdasTools.mthdGetTypeDayMonth(j, k) + "_kW"]), Convert.ToDouble(this.sqlDtRdrServer["DemMaxTrafo_kW"]), this.sqlDtRdrServer["TipCrvaCarga"].ToString() + "_" + GeoPerdasTools.mthdGetTypeDay(k), 2, this.intAdequarTensaoCargasBT, 0);
                                        i++;
                                    }
                                    this.sqlDtRdrServer.Close();
                                    this.strFileData[i] = "<FIM DOS DADOS>";
                                    using (swFile = new System.IO.StreamWriter(strPath + @"\CargasBTNT_" + GeoPerdasTools.mthdGetTypeDayMonth(j, k) + "_" + this.strBaseSelected + "_" + strFeeder + "_" + this.mthdGetOptionsToString() + ".dss", false, Encoding.GetEncoding(1252)))
                                    {
                                        foreach (String strLine in this.strFileData)
                                        {
                                            if (strLine != "<FIM DOS DADOS>")
                                                swFile.WriteLine(strLine);
                                            else
                                                break;
                                        }
                                    }
                                    this.mthdInputMensageLog("Passou pelas cargas BT Não Técnicas.");
                                }

                                // Loop dos arquivos Master
                                i = 0;
                                this.strFileData[i] = "! Criação da seção do arquivo master"; i++;
                                this.strFileData[i] = "Clear"; i++;
                                this.strFileData[i] = "Redirect 'CircuitoMT_" + this.strBaseSelected + "_" + strFeeder + "_" + this.mthdGetOptionsToString() + ".dss'"; i++;
                                this.strFileData[i] = "Redirect 'CodCondutor_" + this.strBaseSelected + "_" + strFeeder + "_" + this.mthdGetOptionsToString() + ".dss'"; i++;
                                this.strFileData[i] = "Redirect 'CurvacargaMT_" + this.strBaseSelected + "_" + strFeeder + "_" + this.mthdGetOptionsToString() + ".dss'"; i++;
                                this.strFileData[i] = "Redirect 'CurvacargaBT_" + this.strBaseSelected + "_" + strFeeder + "_" + this.mthdGetOptionsToString() + ".dss'"; i++;
                                this.strFileData[i] = "Redirect 'ChavesMT_" + this.strBaseSelected + "_" + strFeeder + "_" + this.mthdGetOptionsToString() + ".dss'"; i++;
                                this.strFileData[i] = "Redirect 'SegmentosMT_" + this.strBaseSelected + "_" + strFeeder + "_" + this.mthdGetOptionsToString() + ".dss'"; i++;
                                this.strFileData[i] = "Redirect 'ReguladorMT_" + this.strBaseSelected + "_" + strFeeder + "_" + this.mthdGetOptionsToString() + ".dss'"; i++;
                                this.strFileData[i] = "Redirect 'TransformadorMTMTMTBT_" + this.strBaseSelected + "_" + strFeeder + "_" + this.mthdGetOptionsToString() + ".dss'"; i++;
                                this.strFileData[i] = "Redirect 'ChavesBT_" + this.strBaseSelected + "_" + strFeeder + "_" + this.mthdGetOptionsToString() + ".dss'"; i++;
                                this.strFileData[i] = "Redirect 'SegmentosBT_" + this.strBaseSelected + "_" + strFeeder + "_" + this.mthdGetOptionsToString() + ".dss'"; i++;
                                this.strFileData[i] = "Redirect 'RamaisBT_" + this.strBaseSelected + "_" + strFeeder + "_" + this.mthdGetOptionsToString() + ".dss'"; i++;
                                this.strFileData[i] = "Redirect 'Medidores_" + this.strBaseSelected + "_" + strFeeder + "_" + this.mthdGetOptionsToString() + ".dss'"; i++;
                                this.strFileData[i] = "Redirect 'CargasMT_" + GeoPerdasTools.mthdGetTypeDayMonth(j, k) + "_" + this.strBaseSelected + "_" + strFeeder + "_" + this.mthdGetOptionsToString() + ".dss'"; i++;
                                this.strFileData[i] = "Redirect 'CargasBT_" + GeoPerdasTools.mthdGetTypeDayMonth(j, k) + "_" + this.strBaseSelected + "_" + strFeeder + "_" + this.mthdGetOptionsToString() + ".dss'"; i++;
                                if (this.intRealizaCnvrgcPNT == 1)
                                {
                                    this.strFileData[i] = "Redirect 'CargasMTNT_" + GeoPerdasTools.mthdGetTypeDayMonth(j, k) + "_" + this.strBaseSelected + "_" + strFeeder + "_" + this.mthdGetOptionsToString() + ".dss'"; i++;
                                    this.strFileData[i] = "Redirect 'CargasBTNT_" + GeoPerdasTools.mthdGetTypeDayMonth(j, k) + "_" + this.strBaseSelected + "_" + strFeeder + "_" + this.mthdGetOptionsToString() + ".dss'"; i++;
                                }
                                this.strFileData[i] = "Redirect 'Tensoesbase_" + this.strBaseSelected + "_" + strFeeder + "_" + this.mthdGetOptionsToString() + ".dss'"; i++;
                                this.strFileData[i] = "Set mode = daily"; i++;
                                this.strFileData[i] = "Set tolerance = 0.0001"; i++;
                                this.strFileData[i] = "Set maxcontroliter = 10"; i++;
                                this.strFileData[i] = "!Set algorithm = newton"; i++;
                                this.strFileData[i] = "!Solve mode = direct"; i++;
                                this.strFileData[i] = "Solve"; i++;
                                this.strFileData[i] = "Export meters"; i++;
                                this.strFileData[i] = "<FIM DOS DADOS>";
                                using (swFile = new System.IO.StreamWriter(strPath + @"\Master_" + GeoPerdasTools.mthdGetTypeDayMonth(j, k) + "_" + this.strBaseSelected + "_" + strFeeder + "_" + this.mthdGetOptionsToString() + ".dss", false, Encoding.GetEncoding(1252)))
                                {
                                    foreach (String strLine in this.strFileData)
                                    {
                                        if (strLine != "<FIM DOS DADOS>")
                                            swFile.WriteLine(strLine);
                                        else
                                            break;
                                    }
                                }
                                this.mthdInputMensageLog("Passou pelas criação dos masters.");

                            }

                        }

                        this.mthdInputMensageLog("Término do processo para o alimentador " + strFeeder + ".");

                    }

                }
                catch (Exception xcptn)
                {
                    this.mthdInputMensageLog("Problema não identificado (" + xcptn.Message.ToString() + ").");
                }
            }

        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            this.mthdResizeComponents();
        }

        private void Form1_ResizeBegin(object sender, EventArgs e)
        {
            this.mthdResizeComponents();
        }

        private void Form1_ResizeEnd(object sender, EventArgs e)
        {
            this.mthdResizeComponents();
        }

        private void mthdResizeComponents()
        {
            if (this.Width > 604)
            {
                this.tabControl1.Width = this.Width - (604 - 550);
                this.rtbLog.Width = this.Width - (604 - 550);
            }
            else
            {
                this.tabControl1.Width = 550;
                this.rtbLog.Width = 550;
            }
            if (this.Height > 477)
                this.rtbLog.Height = this.Height - (308 + 89);
            else
                this.rtbLog.Height = 80;
        }

        private void bReportFile_Click(object sender, EventArgs e)
        {

            String strPath;

            if (this.tbPathFile.Text == "" || this.tbPathFile.Text == null)
            {
                this.mthdInputMensageLog("O arquivo não foi estabelecido.");
                return;
            }
            else
            {

                try
                {
                    // Obtem a pasta onde o relatório será gravado
                    if (this.fbdSelectPathWrite.ShowDialog() == DialogResult.Cancel)
                        return;
                    else
                        strPath = this.fbdSelectPathWrite.SelectedPath;

                    this.mthdLoadCircuit(this.tbPathFile.Text);
                    this.mthdSolveCircuit();
                    this.mthdWriteReportHTML(strPath);
                    this.mthdWriteMeterReportHTML(strPath);
                }
                catch (Exception xcptn)
                {
                    this.mthdInputMensageLog("Problema não identificado (" + xcptn.Message.ToString() + ").");
                }

            }

        }

        private void mthdWriteReportHTML(String strPath)
        {

            int intLine, i, j, k;
            Bus dssBus;
            CktElement dssElement;
            dynamic Vseqn, cpxVseqn, V;
            dynamic Curr, Loss;
            dynamic vnodes;
            dynamic puvoltages;
            double[] tensoes = new double[6];

            try
            {
                intLine = 0;

                this.strFileData[intLine] = "<html>"; intLine++;

                this.strFileData[intLine] = "<head>"; intLine++;
                this.strFileData[intLine] = GeoPerdasTools.mthdToStringUnicode("<title>Relatório de resultado de execução do OpenDSS</title>"); intLine++;
                this.strFileData[intLine] = "</head>"; intLine++;

                this.strFileData[intLine] = "<body>"; intLine++;

                // Criação de relatório de tensão
                this.strFileData[intLine] = GeoPerdasTools.mthdToStringUnicode("<h1>Tensões</h1>"); intLine++;
                this.strFileData[intLine] = "<table border=\"1\">"; intLine++;
                this.strFileData[intLine] = "<tr><td>Barra ; Número de nós</td><td>Nós</td><td>Tensão de Base (kV)</td><td>|V0| (kV)</td><td>|V1| (kV)</td><td>|V2| (kV)</td><td>V2/V1 (%)</td><td>V0.Real (kV)</td><td>V0.Imaginary (kV)</td><td>V1.Real (kV)</td><td>V1.Imaginary (kV)</td><td>V2.Real (kV)</td><td>V2.Imaginary (kV)</td><td>Va.Real (V)</td><td>Va.Imaginary (V)</td><td>Vb.Real (V)</td><td>Vb.Imaginary (V)</td><td>Vc.Real (V)</td><td>Vc.Imaginary (V)</td><td>|Va| (V)</td><td>Ang(Va) (V)</td><td>|Vb| (V)</td><td>Ang(Vb) (V)</td><td>|Vc| (V)</td><td>Ang(Vc) (V)</td><td>|Va| (pu)</td><td>|Vb| (pu)</td><td>|Vc| (pu)</td></tr>"; intLine++;
                //this.strFileData[intLine] = "<tr>"; intLine++;
                for (i = 0; i < this.dssCircuit.NumBuses; i++)
                {

                    dssBus = this.dssCircuit.Buses[i];
                    this.strFileData[intLine] += "<tr><td>" + this.dssCircuit.ActiveBus.Name + " ; " + dssBus.NumNodes + "</td>";

                    Vseqn = dssBus.SeqVoltages;
                    cpxVseqn = dssBus.CplxSeqVoltages;
                    V = dssBus.Voltages;
                    vnodes = dssBus.Nodes;
                    puvoltages = dssBus.puVoltages;

                    this.strFileData[intLine] += "<td>";
                    for (j = 0; j < vnodes.Length; j++)
                    {
                        this.strFileData[intLine] += vnodes[j] + ",";
                    }
                    this.strFileData[intLine] += "</td>";
                    
                    this.strFileData[intLine] += "<td>" + dssBus.kVBase + "</td>";

                    if (dssBus.NumNodes == 3)
                    {
                        for (j = 0; j < Vseqn.Length; j++)
                        {
                            this.strFileData[intLine] += "<td>" + Vseqn[j] + "</td>";
                        }
                        this.strFileData[intLine] += "<td>" + Vseqn[2] / Vseqn[1] * 100 + "</td>";
                    }
                    if (dssBus.NumNodes != 3)
                        this.strFileData[intLine] += "<td></td><td></td><td></td><td></td>";

                    if (dssBus.NumNodes == 3)
                    {
                        for (j = 0; j < cpxVseqn.Length; j++)
                        {
                            this.strFileData[intLine] += "<td>" + cpxVseqn[j] + "</td>";
                        }
                    }
                    if (dssBus.NumNodes != 3)
                        this.strFileData[intLine] += "<td></td><td></td><td></td><td></td><td></td><td></td>";
                    
                    tensoes[0] = 0;
                    tensoes[1] = 0;
                    tensoes[2] = 0;
                    tensoes[3] = 0;
                    tensoes[4] = 0;
                    tensoes[5] = 0;
                    for (j = 0; j < vnodes.Length; j++)
                    {
                        if (vnodes[j] == 1)
                        {
                            tensoes[0] = V[2 * j + 0];
                            tensoes[1] = V[2 * j + 1];
                        }
                        if (vnodes[j] == 2)
                        {
                            tensoes[2] = V[2 * j + 0];
                            tensoes[3] = V[2 * j + 1];
                        }
                        if (vnodes[j] == 3)
                        {
                            tensoes[4] = V[2 * j + 0];
                            tensoes[5] = V[2 * j + 1];
                        }
                    }
                    for (j = 0; j < tensoes.Length; j++)
                    {
                        if (tensoes[j] == 0)
                            this.strFileData[intLine] += "<td></td>";
                        else
                            this.strFileData[intLine] += "<td>" + tensoes[j] + "</td>";
                    }
                    for (j = 0; j < tensoes.Length; j = j + 2)
                    {
                        if (tensoes[j] != 0 || tensoes[j+1] != 0)
                            this.strFileData[intLine] += "<td>" + Math.Sqrt(tensoes[j] * tensoes[j] + tensoes[j + 1] * tensoes[j + 1]) + "</td>" + "<td>" + angleComplex(tensoes[j], tensoes[j + 1]) + "</td>";
                        else
                            this.strFileData[intLine] += "<td></td><td></td>";
                    }

                    for (j = 0; j < tensoes.Length; j = j + 2)
                    {
                        if (tensoes[j] != 0 || tensoes[j+1] != 0)
                            this.strFileData[intLine] += "<td>" + Math.Sqrt(tensoes[j] * tensoes[j] + tensoes[j + 1] * tensoes[j + 1]) / (dssBus.kVBase * 1000) + "</td>";
                        else
                            this.strFileData[intLine] += "<td></td>";
                    }
/*
 * Código que exporta puvoltages
                    tensoes[0] = 0;
                    tensoes[1] = 0;
                    tensoes[2] = 0;
                    tensoes[3] = 0;
                    tensoes[4] = 0;
                    tensoes[5] = 0;
                    for (j = 0; j < vnodes.Length; j++)
                    {
                        if (vnodes[j] == 1)
                        {
                            tensoes[0] = puvoltages[2 * j + 0];
                            tensoes[1] = puvoltages[2 * j + 1];
                        }
                        if (vnodes[j] == 2)
                        {
                            tensoes[2] = puvoltages[2 * j + 0];
                            tensoes[3] = puvoltages[2 * j + 1];
                        }
                        if (vnodes[j] == 3)
                        {
                            tensoes[4] = puvoltages[2 * j + 0];
                            tensoes[5] = puvoltages[2 * j + 1];
                        }
                    }
                    for (j = 0; j < tensoes.Length; j++)
                    {
                        if (tensoes[j] == 0)
                            this.strFileData[intLine] += "<td></td>";
                        else
                            this.strFileData[intLine] += "<td>" + tensoes[j] + "</td>";
                    }
*/
                    intLine++;
                }
                //this.strFileData[intLine] += "</tr>"; intLine++;
                this.strFileData[intLine] += "</table>"; intLine++;

                // Criação de relatório de corrente
                dssElement = this.dssCircuit.ActiveCktElement;
                i = this.dssCircuit.FirstPDElement();

                this.strFileData[intLine] += "<h1>Correntes</h1>"; intLine++;
                this.strFileData[intLine] += "<table border=\"1\">"; intLine++;
                this.strFileData[intLine] += "<tr><td>Elemento</td><td>Terminal 1 - Real(Ia)</td><td>Terminal 1 - Imaginary(Ia)</td><td>Terminal 1 - Real(Ib)</td><td>Terminal 1 - Imaginary(Ib)</td><td>Terminal 1 - Real(Ic)</td><td>Terminal 1 - Imaginary(Ic)</td><td>Terminal 2 - Real(Ia)</td><td>Terminal 2 - Imaginary(Ia)</td><td>Terminal 2 - Real(Ib)</td><td>Terminal 2 - Imaginary(Ib)</td><td>Terminal 2 - Real(Ic)</td><td>Terminal 2 - Imaginary(Ic)</td></tr>"; intLine++;
                while (i > 0)
                {
                    if (dssElement.NumPhases == 3)
                    {
                        this.strFileData[intLine] += "<tr><td>" + dssElement.Name + "</td>";

                        Curr = dssElement.Currents;

                        for (j = 0; j < Curr.Length; j++)
                        {
                            this.strFileData[intLine] += "<td>" + Curr[j] + "</td>";
                        }
                        this.strFileData[intLine] += "</tr>";
                    }
                    /*
                    else if (dssElement.NumPhases == 2)
                    {
                        this.strFileData[intLine] += "<tr><td>" + dssElement.Name + "</td>";
                        Curr = dssElement.Currents;

                        for (j = 0; j < Curr.Length; j++)
                        {
                            this.strFileData[intLine] += "<td>" + Curr[j] + "</td>";
                            if (j == 3 || j == 7)
                                this.strFileData[intLine] += "<td></td><td></td>";
                        }
                        this.strFileData[intLine] += "</tr>";
                    }
                    else if (dssElement.NumPhases == 1)
                    {
                        this.strFileData[intLine] += "<tr><td>" + dssElement.Name + "</td>";
                        Curr = dssElement.Currents;

                        for (j = 0; j < Curr.Length; j++)
                        {
                            this.strFileData[intLine] += "<td>" + Curr[j] + "</td>";
                            if (j == 1 || j == 3)
                                this.strFileData[intLine] += "<td></td><td></td><td></td><td></td>";
                        }
                        this.strFileData[intLine] += "</tr>";
                    }
                    */
                    i = this.dssCircuit.NextPDElement();
                    intLine++;
                }
                this.strFileData[intLine] += "</table>"; intLine++;

                // Criação de relatório de potência
                i = this.dssCircuit.FirstPDElement();
                this.strFileData[intLine] += GeoPerdasTools.mthdToStringUnicode("<h1>Perdas de Potência</h1>"); intLine++;
                this.strFileData[intLine] += "<table border=\"1\">"; intLine++;
                while (i > 0)
                {
                    Loss = dssElement.Losses;

                    this.strFileData[intLine] += "<tr><td>" + dssElement.Name + "</td>";
                    for (j = 0; j < Loss.Length; j++)
                    {
                        this.strFileData[intLine] += "<td>" + Loss[j] + "</td>";
                    }
                    this.strFileData[intLine] += "</tr>";

                    i = this.dssCircuit.NextPDElement();
                    intLine++;
                }
                this.strFileData[intLine] += "</table>"; intLine++;

                this.strFileData[intLine] += "</body>"; intLine++;

                this.strFileData[intLine] += "</html>"; intLine++;

                // Realiza a gravação do arquivo
                this.strFileData[intLine] = "<FIM DOS DADOS>";
                using (swFile = new System.IO.StreamWriter(strPath + @"\Report_" + DateTime.Now.Year.ToString() + String.Format("{0:00}", DateTime.Now.Month) + String.Format("{0:00}", DateTime.Now.Day) + String.Format("{0:00}", DateTime.Now.Hour) + String.Format("{0:00}", DateTime.Now.Minute) + String.Format("{0:00}", DateTime.Now.Second) + ".html", false, Encoding.GetEncoding(1252)))
                {
                    foreach (String strLine in this.strFileData)
                    {
                        if (strLine != "<FIM DOS DADOS>")
                            swFile.WriteLine(strLine);
                        else
                            break;
                    }
                }

            }
            catch (Exception xcptn)
            {
                this.mthdInputMensageLog("Problema não identificado (" + xcptn.Message.ToString() + ").");
            }

        }

        private void mthdWriteMeterReportHTML(String strPath)
        {

            int intLine, i = 0;
            Meters dssMeter;

            try
            {
                intLine = 0;

                this.strFileData[intLine] = "<html>"; intLine++;

                this.strFileData[intLine] = "<head>"; intLine++;
                this.strFileData[intLine] = GeoPerdasTools.mthdToStringUnicode("<title>Relatório de resultado de medidores do OpenDSS</title>"); intLine++;
                this.strFileData[intLine] = "</head>"; intLine++;

                this.strFileData[intLine] = "<body>"; intLine++;

                foreach (String strNomeMedidor in this.dssCircuit.Meters.AllNames)
                {
                    this.strFileData[intLine] = "<h1>Nome do medidor: " + strNomeMedidor + "</h1>"; intLine++;
                    this.dssCircuit.Meters.Name = strNomeMedidor;
                    dssMeter = this.dssCircuit.Meters;
                    i = 0;
                    this.strFileData[intLine] = "<table border=\"1\">"; intLine++;
                    this.strFileData[intLine] = GeoPerdasTools.mthdToStringUnicode("<tr><td>Título do Registrador</td><td>Valor do Registrador</td></tr>"); intLine++;
                    foreach (String strRegister in dssMeter.RegisterNames)
                    {
                        this.strFileData[intLine] = "<tr><td>" + strRegister +"</td><td>" + dssMeter.RegisterValues[i] + "</td></tr>"; intLine++;
                        i++;
                    }
                    this.strFileData[intLine] = "</table>"; intLine++;
                }

                this.strFileData[intLine] = "</body>"; intLine++;

                this.strFileData[intLine] = "</html>"; intLine++;

                // Realiza a gravação do arquivo
                this.strFileData[intLine] = "<FIM DOS DADOS>";
                using (swFile = new System.IO.StreamWriter(strPath + @"\MeterReport_" + DateTime.Now.Year.ToString() + String.Format("{0:00}", DateTime.Now.Month) + String.Format("{0:00}", DateTime.Now.Day) + String.Format("{0:00}", DateTime.Now.Hour) + String.Format("{0:00}", DateTime.Now.Minute) + String.Format("{0:00}", DateTime.Now.Second) + ".html", false, Encoding.GetEncoding(1252)))
                {
                    foreach (String strLine in this.strFileData)
                    {
                        if (strLine != "<FIM DOS DADOS>")
                            swFile.WriteLine(strLine);
                        else
                            break;
                    }
                }
            }
            catch (Exception xcptn)
            {
                this.mthdInputMensageLog("Problema não identificado (" + xcptn.Message.ToString() + ").");
            }

        }

        private void bTrafosSubDB_Click(object sender, EventArgs e)
        {

            if (checkBase())
            {

                try
                {

                    // Executa a stored procedure de cálculo de transformadores de potência
                    this.sqlCommServer = new SqlCommand("dbo.CalculaPerdaTransformadoresPotencia", this.sqlConnServer);
                    this.sqlCommServer.CommandType = CommandType.StoredProcedure;
                    this.sqlCommServer.Parameters.AddWithValue("@BaseAnalisada", this.strBaseSelected);
                    this.sqlCommServer.ExecuteNonQuery();

                }
                catch (Exception xcptn)
                {

                    this.mthdInputMensageLog("Problema não identificado (" + xcptn.Message.ToString() + ").");
                    this.sqlCommServer.Dispose();

                }

            }

        }

        private void bMedidoresDB_Click(object sender, EventArgs e)
        {

            if (checkBase())
            {

                try
                {

                    // Executa a stored procedure de cálculo de medidores
                    this.sqlCommServer = new SqlCommand("dbo.CalculaPerdaMedidores", this.sqlConnServer);
                    this.sqlCommServer.CommandType = CommandType.StoredProcedure;
                    this.sqlCommServer.Parameters.AddWithValue("@BaseAnalisada", this.strBaseSelected);
                    this.sqlCommServer.ExecuteNonQuery();

                }
                catch (Exception xcptn)
                {

                    this.mthdInputMensageLog("Problema não identificado (" + xcptn.Message.ToString() + ").");
                    this.sqlCommServer.Dispose();

                }

            }

        }

        private void bReportAnalisys_Click(object sender, EventArgs e)
        {

            int intLine;
            String strPath;
            double dblEnerInj_MWh = 0, dblEnerForn_MWh = 0, dblPerdaTotal_MWh = 0;
            double dblPerdaTotalAlim_MWh = 0, dblPerdaTotalAlim_pc = 0, dblEnerCircAlim_MWh = 0;

            if (checkBase())
            {
                try
                {

                    // Captura a base a ser calculada
                    this.strBaseSelected = this.tbObjectBase.Text;

                    // Obtem a pasta onde o arquivo será gravado
                    if (this.fbdSelectPathWrite.ShowDialog() == DialogResult.Cancel)
                        return;
                    else
                        strPath = this.fbdSelectPathWrite.SelectedPath;

                    intLine = 0;

                    this.strFileData[intLine] = "<html>"; intLine++;

                    this.strFileData[intLine] = "<head>"; intLine++;
                    this.strFileData[intLine] = "<style type=\"text/css\">"; intLine++;
                    this.strFileData[intLine] = "<!--"; intLine++;
                    this.strFileData[intLine] = ".f01 {"; intLine++;
                    this.strFileData[intLine] = "font-size: 10px;"; intLine++;
                    this.strFileData[intLine] = "font-family: arial, helvetica, serif;"; intLine++;
                    this.strFileData[intLine] = "}"; intLine++;
                    this.strFileData[intLine] = "-->"; intLine++;
                    this.strFileData[intLine] = "</style>"; intLine++;
                    this.strFileData[intLine] = GeoPerdasTools.mthdToStringUnicode("<title>Relatório de análise da base da distribuidora</title>"); intLine++;
                    this.strFileData[intLine] = "</head>"; intLine++;

                    this.strFileData[intLine] = "<body>"; intLine++;

                    this.strFileData[intLine] = GeoPerdasTools.mthdToStringUnicode("<h1>Relatório de análise da base da distribuidora</h1>"); intLine++;

                    this.strFileData[intLine] = GeoPerdasTools.mthdToStringUnicode("<h2>Informações da base</h2>"); intLine++;
                    this.strFileData[intLine] = GeoPerdasTools.mthdToStringUnicode("<p>A seguir apresenta-se informações resumidas da base que está sendo analisada.</p>"); intLine++;
                    this.strFileData[intLine] = "<table border=\"1\">"; intLine++;
                    //this.sqlCommServer = new SqlCommand("SELECT t1.* FROM dbo.viewReportAnalisys03 AS t1 WHERE t1.CodBase = " + this.strBaseSelected, this.sqlConnServer);
                    this.sqlCommServer = new SqlCommand("dbo.sBalanEnerg", this.sqlConnServer);
                    this.sqlCommServer.CommandType = CommandType.StoredProcedure;
                    this.sqlCommServer.Parameters.AddWithValue("@CodBase", this.strBaseSelected);
                    this.sqlCommServer.CommandTimeout = 0;
                    this.sqlDtRdrServer = this.sqlCommServer.ExecuteReader();
                    while (this.sqlDtRdrServer.Read())
                    {
                        dblEnerInj_MWh = Convert.ToDouble(this.sqlDtRdrServer["EnerInj_MWh"]);
                        dblEnerForn_MWh = Convert.ToDouble(this.sqlDtRdrServer["EnerForn_MWh"]);
                        dblPerdaTotal_MWh = dblEnerInj_MWh - dblEnerForn_MWh;
                        this.strFileData[intLine] = GeoPerdasTools.mthdToStringUnicode("<tr><td>Código da distribuidora</td><td>" + this.strBaseSelected + "</td></tr>"); intLine++;
                        this.strFileData[intLine] = GeoPerdasTools.mthdToStringUnicode("<tr><td>Energia total injetada (MWh/ano)</td><td>" + dblEnerInj_MWh.ToString().Replace(".", ",") + "</td></tr>"); intLine++;
                        this.strFileData[intLine] = GeoPerdasTools.mthdToStringUnicode("<tr><td>Energia total fornecida (MWh/ano)</td><td>" + dblEnerForn_MWh.ToString().Replace(".", ",") + "</td></tr>"); intLine++;
                        this.strFileData[intLine] = GeoPerdasTools.mthdToStringUnicode("<tr><td>Perda total (MWh/ano)</td><td>" + dblPerdaTotal_MWh.ToString().Replace(".", ",") + "</td></tr>"); intLine++;
                        this.strFileData[intLine] = GeoPerdasTools.mthdToStringUnicode("<tr><td>Percentual da perda total em relação a energia total injetada (%)</td><td>" + (dblPerdaTotal_MWh / dblEnerInj_MWh * 100).ToString().Replace(".", ",") + "</td></tr>"); intLine++;
                        this.strFileData[intLine] = GeoPerdasTools.mthdToStringUnicode("<tr><td>Percentual da perda total em relação a energia total fornecida (%)</td><td>" + (dblPerdaTotal_MWh / dblEnerForn_MWh * 100).ToString().Replace(".", ",") + "</td></tr>"); intLine++;
                    }
                    this.sqlDtRdrServer.Close();
                    this.sqlCommServer = new SqlCommand("SELECT t1.* FROM dbo.[" + this.strBaseSelected + "Base] AS t1 WHERE t1.CodBase = " + this.strBaseSelected, this.sqlConnServer);
                    this.sqlCommServer.CommandTimeout = 0;
                    this.sqlDtRdrServer = this.sqlCommServer.ExecuteReader();
                    while (this.sqlDtRdrServer.Read())
                    {
                        this.strFileData[intLine] = GeoPerdasTools.mthdToStringUnicode("<tr><td>Data base dos dados</td><td>" + this.sqlDtRdrServer["DatBas"].ToString() + "</td></tr>"); intLine++;
                    }
                    this.sqlDtRdrServer.Close();
                    this.strFileData[intLine] = "</table>"; intLine++;

                    this.strFileData[intLine] = GeoPerdasTools.mthdToStringUnicode("<h2>Sumário executivo</h2>"); intLine++;

                    this.strFileData[intLine] = GeoPerdasTools.mthdToStringUnicode("<h2>Resumo das informações do balanço</h2>"); intLine++;

                    this.strFileData[intLine] = GeoPerdasTools.mthdToStringUnicode("<h2>Resumo das informações dos alimentadores</h2>"); intLine++;

                    // Loop dos códigos dos circuitos
                    //this.sqlCommServer = new SqlCommand("SELECT t1.* FROM dbo.viewReportAnalisys01 AS t1 WHERE t1.CodBase = " + this.strBaseSelected, this.sqlConnServer);
                    this.sqlCommServer = new SqlCommand("dbo.sCircMT", this.sqlConnServer);
                    this.sqlCommServer.CommandType = CommandType.StoredProcedure;
                    this.sqlCommServer.Parameters.AddWithValue("@CodBase", this.strBaseSelected);
                    this.sqlCommServer.CommandTimeout = 0;
                    this.sqlDtRdrServer = this.sqlCommServer.ExecuteReader();
                    this.strFileData[intLine] = "<table border=\"1\">"; intLine++;
                    this.strFileData[intLine] = GeoPerdasTools.mthdToStringUnicode("<tr><td><div class=\"f01\">Código do Alimentador</div></td><td><div class=\"f01\">Tensão de Operação</div></td><td><div class=\"f01\">Tensão Nominal (kV)</div></td><td><div class=\"f01\">Circuito Atípico (1=Sim)</div></td><td><div class=\"f01\">Circuito Atípico ANEEL (1=Sim)</div></td><td><div class=\"f01\">Circuito Atípico Resultante (1=Sim)</div></td><td><div class=\"f01\">Energia Injetada do Circuito Declarada (MWh/ano)</div></td><td><div class=\"f01\">Perda Técnica do Circuito Declarada (MWh/ano)</div></td><td><div class=\"f01\">Quantidade de Consumidores BT</div></td><td><div class=\"f01\">Energia Carga BT Regular Declarada (MWh/ano)</div></td><td><div class=\"f01\">Quantidade de Consumidores MT</div></td><td><div class=\"f01\">Energia Carga MT Regular Declarada (MWh/ano)</div></td><td><div class=\"f01\">Perda Total Declarada (MWh/ano)</div></td><td><div class=\"f01\">Perda Total Declarada sobre a Energia Injetada do Circuito Declarada (%)</div></td></tr>"); intLine++;
                    while (this.sqlDtRdrServer.Read())
                    {
                        dblEnerCircAlim_MWh = (DBNull.Value.Equals(this.sqlDtRdrServer["EnerCirc_MWh"]) ? 0 : Convert.ToDouble(this.sqlDtRdrServer["EnerCirc_MWh"]));
                        dblPerdaTotalAlim_MWh = (DBNull.Value.Equals(this.sqlDtRdrServer["EnerCirc_MWh"]) ? 0 : Convert.ToDouble(this.sqlDtRdrServer["EnerCirc_MWh"])) - (DBNull.Value.Equals(this.sqlDtRdrServer["EnerRegulMT_MWh"]) ? 0 : Convert.ToDouble(this.sqlDtRdrServer["EnerRegulMT_MWh"])) - (DBNull.Value.Equals(this.sqlDtRdrServer["EnerRegulBT_MWh"]) ? 0 : Convert.ToDouble(this.sqlDtRdrServer["EnerRegulBT_MWh"]));
                        dblPerdaTotalAlim_pc = (dblEnerCircAlim_MWh == 0 ? 0 : dblPerdaTotalAlim_MWh / dblEnerCircAlim_MWh * 100);
                        this.strFileData[intLine] = GeoPerdasTools.mthdToStringUnicode("<tr><td><div class=\"f01\">" + this.sqlDtRdrServer["CodAlim"].ToString() + "</div></td><td><div class=\"f01\">" + this.sqlDtRdrServer["TenOpe_pu"].ToString() + "</div></td><td><div class=\"f01\">" + this.sqlDtRdrServer["TenNom_kV"].ToString() + "</div></td><td><div class=\"f01\">" + this.sqlDtRdrServer["CircAtip"].ToString() + "</div></td><td><div class=\"f01\">" + this.sqlDtRdrServer["CircAtipANEEL"].ToString() + "</div></td><td><div class=\"f01\">" + this.sqlDtRdrServer["CircAtipResultante"].ToString() + "</div></td><td><div class=\"f01\">" + dblEnerCircAlim_MWh.ToString() + "</div></td><td><div class=\"f01\">" + (DBNull.Value.Equals(this.sqlDtRdrServer["PerdCirc_MWh"]) ? 0 : Convert.ToDouble(this.sqlDtRdrServer["PerdCirc_MWh"])).ToString() + "</div></td><td><div class=\"f01\">" + (DBNull.Value.Equals(this.sqlDtRdrServer["QuantConsBT"]) ? 0 : Convert.ToDouble(this.sqlDtRdrServer["QuantConsBT"])).ToString() + "</div></td><td><div class=\"f01\">" + (DBNull.Value.Equals(this.sqlDtRdrServer["EnerRegulBT_MWh"]) ? 0 : Convert.ToDouble(this.sqlDtRdrServer["EnerRegulBT_MWh"])).ToString() + "</div></td><td><div class=\"f01\">" + (DBNull.Value.Equals(this.sqlDtRdrServer["QuantConsMT"]) ? 0 : Convert.ToDouble(this.sqlDtRdrServer["QuantConsMT"])).ToString() + "</div></td><td><div class=\"f01\">" + (DBNull.Value.Equals(this.sqlDtRdrServer["EnerRegulMT_MWh"]) ? 0 : Convert.ToDouble(this.sqlDtRdrServer["EnerRegulMT_MWh"])).ToString() + "</div></td><td><div class=\"f01\">" + dblPerdaTotalAlim_MWh.ToString().Replace(".", ",") + "</div></td><td><div class=\"f01\">" + dblPerdaTotalAlim_pc.ToString().Replace(".", ",") + "</div></td></tr>"); intLine++;
                    }
                    this.sqlDtRdrServer.Close();
                    this.strFileData[intLine] = "</table>"; intLine++;

                    this.strFileData[intLine] = "</body>"; intLine++;

                    this.strFileData[intLine] = "</html>"; intLine++;

                    // Realiza a gravação do arquivo
                    this.strFileData[intLine] = "<FIM DOS DADOS>";
                    using (swFile = new System.IO.StreamWriter(strPath + @"\AnalisysReport_" + DateTime.Now.Year.ToString() + String.Format("{0:00}", DateTime.Now.Month) + String.Format("{0:00}", DateTime.Now.Day) + String.Format("{0:00}", DateTime.Now.Hour) + String.Format("{0:00}", DateTime.Now.Minute) + String.Format("{0:00}", DateTime.Now.Second) + ".html", false, Encoding.GetEncoding(1252)))
                    {
                        foreach (String strLine in this.strFileData)
                        {
                            if (strLine != "<FIM DOS DADOS>")
                                swFile.WriteLine(strLine);
                            else
                                break;
                        }
                    }

                    this.mthdInputMensageLog("Relatório de análise exportado.");

                }
                catch (Exception xcptn)
                {

                    this.mthdInputMensageLog("Problema não identificado (" + xcptn.Message.ToString() + ").");
                    this.sqlCommServer.Dispose();

                }

            }

        }

        private void bReportSGT_Click(object sender, EventArgs e)
        {

            int index = 1;
            String strPath;
            Excel.Application oXL;
	        Excel._Workbook oWB;
	        Excel._Worksheet oSheet;
	        Excel.Range oRng;

            if (checkBase())
            {

                try
                {

                    // Captura a base a ser calculada
                    this.strBaseSelected = this.tbObjectBase.Text;

                    // Obtem a pasta onde o arquivo será gravado
                    //if (this.fbdSelectPathWrite.ShowDialog() == DialogResult.Cancel)
                    //    return;
                    //else
                    //    strPath = this.fbdSelectPathWrite.SelectedPath;

                    // Inicializa o Excel e captura o objeto Application
                    oXL = new Excel.Application();
                    oXL.Visible = true;

                    // Captura um novo workbook
                    oWB = (Excel._Workbook)(oXL.Workbooks.Add(Missing.Value));
                    oSheet = (Excel._Worksheet)oWB.ActiveSheet;

                    // Cria a seção de informações da base da distribuidora
                    this.sqlCommServer = new SqlCommand("SELECT t1.* FROM dbo.Base AS t1 WHERE t1.CodBase = " + this.strBaseSelected, this.sqlConnServer);
                    this.sqlCommServer.CommandTimeout = 0;
                    this.sqlDtRdrServer = this.sqlCommServer.ExecuteReader();
                    while (this.sqlDtRdrServer.Read())
                    {
                        oSheet.Cells[index, 1] = "Distribuidora"; oSheet.Cells[index, 1].Font.Bold = true; oSheet.Cells[index, 1].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 1].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        oSheet.Cells[index, 2] = this.sqlDtRdrServer["CodDist"]; oSheet.Cells[index, 2].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 2].Borders.LineStyle = Excel.XlLineStyle.xlContinuous; index++;
                        oSheet.Cells[index, 1] = "Versão"; oSheet.Cells[index, 1].Font.Bold = true; oSheet.Cells[index, 1].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 1].Borders.LineStyle = Excel.XlLineStyle.xlContinuous; index++;
                        oSheet.Cells[index, 1] = "Data base (dd/mm/aaaa)"; oSheet.Cells[index, 1].Font.Bold = true; oSheet.Cells[index, 1].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 1].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        oSheet.Cells[index, 2] = this.sqlDtRdrServer["DatBas"]; oSheet.Cells[index, 2].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 2].Borders.LineStyle = Excel.XlLineStyle.xlContinuous; index++;
                    }
                    this.sqlDtRdrServer.Close();

                    // Cria a seção de informações de extensão de rede AT
                    index = index + 2;
                    this.sqlCommServer = new SqlCommand("SELECT t1.* FROM dbo.ExtensaoAT AS t1 WHERE t1.CodBase = " + this.strBaseSelected, this.sqlConnServer);
                    this.sqlCommServer.CommandTimeout = 0;
                    this.sqlDtRdrServer = this.sqlCommServer.ExecuteReader();
                    oSheet.get_Range("A"+index, "B"+index).Merge(true);
                    oSheet.get_Range("A" + index, "B" + index).Value = "TABELA FIS.1 - EXTENSÃO DE REDE AT"; oSheet.get_Range("A" + index, "B" + index).Font.Bold = true; oSheet.get_Range("A" + index, "B" + index).HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.get_Range("A" + index, "B" + index).Borders.LineStyle = Excel.XlLineStyle.xlContinuous; index++;
                    oSheet.Cells[index, 1] = "Tensão Nominal"; oSheet.Cells[index, 2] = "Comprimento"; oSheet.Cells[index, 1].Font.Bold = true; oSheet.Cells[index, 1].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 1].Borders.LineStyle = Excel.XlLineStyle.xlContinuous; oSheet.Cells[index, 2].Font.Bold = true; oSheet.Cells[index, 2].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 2].Borders.LineStyle = Excel.XlLineStyle.xlContinuous; index++;
                    oSheet.Cells[index, 1] = "kV"; oSheet.Cells[index, 2] = "km"; oSheet.Cells[index, 1].Font.Bold = true; oSheet.Cells[index, 1].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 1].Borders.LineStyle = Excel.XlLineStyle.xlContinuous; oSheet.Cells[index, 2].Font.Bold = true; oSheet.Cells[index, 2].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 2].Borders.LineStyle = Excel.XlLineStyle.xlContinuous; index++;
                    while (this.sqlDtRdrServer.Read())
                    {
                        oSheet.Cells[index, 1] = this.sqlDtRdrServer["Tensao_kV"]; oSheet.Cells[index, 1].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 1].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        oSheet.Cells[index, 2] = this.sqlDtRdrServer["Comp_km"]; oSheet.Cells[index, 2].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 2].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        index++;
                    }
                    this.sqlDtRdrServer.Close();

                    // Cria a seção de informações dos transformadores AT/AT e AT/MT
                    index = index + 2;
                    this.sqlCommServer = new SqlCommand("SELECT t1.* FROM dbo.AuxReportSGT01 AS t1 WHERE t1.CodBase = " + this.strBaseSelected, this.sqlConnServer);
                    this.sqlCommServer.CommandTimeout = 0;
                    this.sqlDtRdrServer = this.sqlCommServer.ExecuteReader();
                    oSheet.get_Range("A" + index, "D" + index).Merge(true);
                    oSheet.get_Range("A" + index, "D" + index).Value = "TABELA FIS.2 - TRANSFORMADORES AT/AT E AT/MT"; oSheet.get_Range("A" + index, "D" + index).Font.Bold = true; oSheet.get_Range("A" + index, "D" + index).HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.get_Range("A" + index, "D" + index).Borders.LineStyle = Excel.XlLineStyle.xlContinuous; index++;
                    oSheet.Cells[index, 1] = "Tensão Primária"; oSheet.Cells[index, 1].Font.Bold = true; oSheet.Cells[index, 1].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 1].Borders.LineStyle = Excel.XlLineStyle.xlContinuous; oSheet.Cells[index, 2] = "Tensão Secundária"; oSheet.Cells[index, 2].Font.Bold = true; oSheet.Cells[index, 2].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 2].Borders.LineStyle = Excel.XlLineStyle.xlContinuous; oSheet.Cells[index, 3] = "Quantidade"; oSheet.Cells[index, 3].Font.Bold = true; oSheet.Cells[index, 3].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 3].Borders.LineStyle = Excel.XlLineStyle.xlContinuous; oSheet.Cells[index, 4] = "Potência Total"; oSheet.Cells[index, 4].Font.Bold = true; oSheet.Cells[index, 4].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 4].Borders.LineStyle = Excel.XlLineStyle.xlContinuous; index++;
                    oSheet.Cells[index, 1] = "kV"; oSheet.Cells[index, 1].Font.Bold = true; oSheet.Cells[index, 1].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 1].Borders.LineStyle = Excel.XlLineStyle.xlContinuous; oSheet.Cells[index, 2] = "kV"; oSheet.Cells[index, 2].Font.Bold = true; oSheet.Cells[index, 2].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 2].Borders.LineStyle = Excel.XlLineStyle.xlContinuous; oSheet.Cells[index, 3] = "Unidades"; oSheet.Cells[index, 3].Font.Bold = true; oSheet.Cells[index, 3].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 3].Borders.LineStyle = Excel.XlLineStyle.xlContinuous; oSheet.Cells[index, 4] = "MVA"; oSheet.Cells[index, 4].Font.Bold = true; oSheet.Cells[index, 4].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 4].Borders.LineStyle = Excel.XlLineStyle.xlContinuous; index++;
                    while (this.sqlDtRdrServer.Read())
                    {
                        oSheet.Cells[index, 1] = this.sqlDtRdrServer["ClassTenPrim"]; oSheet.Cells[index, 1].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 1].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        oSheet.Cells[index, 2] = this.sqlDtRdrServer["ClassTenSecu"]; oSheet.Cells[index, 2].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 2].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        oSheet.Cells[index, 3] = this.sqlDtRdrServer["Quant"]; oSheet.Cells[index, 3].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 3].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        oSheet.Cells[index, 4] = this.sqlDtRdrServer["PotNom_MVA"]; oSheet.Cells[index, 4].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 4].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        index++;
                    }
                    this.sqlDtRdrServer.Close();

                    // Cria a seção de informações de bays de linha AT e MT
                    index = index + 2;
                    this.sqlCommServer = new SqlCommand("SELECT t1.* FROM dbo.Bays AS t1 WHERE t1.CodBase = " + this.strBaseSelected, this.sqlConnServer);
                    this.sqlCommServer.CommandTimeout = 0;
                    this.sqlDtRdrServer = this.sqlCommServer.ExecuteReader();
                    oSheet.get_Range("A" + index, "B" + index).Merge(true);
                    oSheet.get_Range("A" + index, "B" + index).Value = "TABELA FIS.3 - BAYS DE LINHA AT/AT E AT/MT"; oSheet.get_Range("A" + index, "B" + index).Font.Bold = true; oSheet.get_Range("A" + index, "B" + index).HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.get_Range("A" + index, "B" + index).Borders.LineStyle = Excel.XlLineStyle.xlContinuous; index++;
                    oSheet.Cells[index, 1] = "Tensão Nominal"; oSheet.Cells[index, 1].Font.Bold = true; oSheet.Cells[index, 1].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 1].Borders.LineStyle = Excel.XlLineStyle.xlContinuous; oSheet.Cells[index, 2] = "Quantidade"; oSheet.Cells[index, 2].Font.Bold = true; oSheet.Cells[index, 2].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 2].Borders.LineStyle = Excel.XlLineStyle.xlContinuous; index++;
                    oSheet.Cells[index, 1] = "kV"; oSheet.Cells[index, 1].Font.Bold = true; oSheet.Cells[index, 1].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 1].Borders.LineStyle = Excel.XlLineStyle.xlContinuous; oSheet.Cells[index, 2] = "Unidades"; oSheet.Cells[index, 2].Font.Bold = true; oSheet.Cells[index, 2].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 2].Borders.LineStyle = Excel.XlLineStyle.xlContinuous; index++;
                    while (this.sqlDtRdrServer.Read())
                    {
                        oSheet.Cells[index, 1] = this.sqlDtRdrServer["SubGrupo"]; oSheet.Cells[index, 1].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 1].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        oSheet.Cells[index, 2] = this.sqlDtRdrServer["Quant"]; oSheet.Cells[index, 2].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 2].Borders.LineStyle = Excel.XlLineStyle.xlContinuous; index++;
                    }
                    this.sqlDtRdrServer.Close();

                    // Cria a seção de informações dos segmentos MT
                    index = index + 2;
                    this.sqlCommServer = new SqlCommand("SELECT t1.* FROM dbo.AuxReportSGT02 AS t1 WHERE t1.CodBase = " + this.strBaseSelected, this.sqlConnServer);
                    this.sqlCommServer.CommandTimeout = 0;
                    this.sqlDtRdrServer = this.sqlCommServer.ExecuteReader();
                    oSheet.get_Range("A" + index, "E" + index).Merge(true);
                    oSheet.get_Range("A" + index, "E" + index).Value = "TABELA FIS.5 - SEGMENTOS MT"; oSheet.get_Range("A" + index, "E" + index).Font.Bold = true; oSheet.get_Range("A" + index, "E" + index).HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.get_Range("A" + index, "E" + index).Borders.LineStyle = Excel.XlLineStyle.xlContinuous; index++;
                    oSheet.Cells[index, 1] = "Tipo"; oSheet.Cells[index, 1].Font.Bold = true; oSheet.Cells[index, 1].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 1].Borders.LineStyle = Excel.XlLineStyle.xlContinuous; oSheet.Cells[index, 2] = "Urbano (UR)"; oSheet.Cells[index, 2].Font.Bold = true; oSheet.Cells[index, 2].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 2].Borders.LineStyle = Excel.XlLineStyle.xlContinuous; oSheet.Cells[index, 3] = "Não Urbano (NUR)"; oSheet.Cells[index, 3].Font.Bold = true; oSheet.Cells[index, 3].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 3].Borders.LineStyle = Excel.XlLineStyle.xlContinuous; oSheet.Cells[index, 4] = "Terceiros (TC)"; oSheet.Cells[index, 4].Font.Bold = true; oSheet.Cells[index, 4].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 4].Borders.LineStyle = Excel.XlLineStyle.xlContinuous; oSheet.Cells[index, 5] = "Não Classificado"; oSheet.Cells[index, 5].Font.Bold = true; oSheet.Cells[index, 5].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 5].Borders.LineStyle = Excel.XlLineStyle.xlContinuous; index++;
                    oSheet.Cells[index, 1].Font.Bold = true; oSheet.Cells[index, 1].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 1].Borders.LineStyle = Excel.XlLineStyle.xlContinuous; oSheet.Cells[index, 2] = "km"; oSheet.Cells[index, 2].Font.Bold = true; oSheet.Cells[index, 2].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 2].Borders.LineStyle = Excel.XlLineStyle.xlContinuous; oSheet.Cells[index, 3] = "km"; oSheet.Cells[index, 3].Font.Bold = true; oSheet.Cells[index, 3].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 3].Borders.LineStyle = Excel.XlLineStyle.xlContinuous; oSheet.Cells[index, 4] = "km"; oSheet.Cells[index, 4].Font.Bold = true; oSheet.Cells[index, 4].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 4].Borders.LineStyle = Excel.XlLineStyle.xlContinuous; oSheet.Cells[index, 5] = "km"; oSheet.Cells[index, 5].Font.Bold = true; oSheet.Cells[index, 5].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 5].Borders.LineStyle = Excel.XlLineStyle.xlContinuous; index++;
                    while (this.sqlDtRdrServer.Read())
                    {
                        oSheet.Cells[index, 1] = this.sqlDtRdrServer["CodTipo"]; oSheet.Cells[index, 1].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 1].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        oSheet.Cells[index, 2] = this.sqlDtRdrServer["Urbano_km"]; oSheet.Cells[index, 2].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 2].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        oSheet.Cells[index, 3] = this.sqlDtRdrServer["NaoUrbano_km"]; oSheet.Cells[index, 3].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 3].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        oSheet.Cells[index, 4] = this.sqlDtRdrServer["Terceiro_km"]; oSheet.Cells[index, 4].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 4].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        oSheet.Cells[index, 5] = this.sqlDtRdrServer["Indeterminado_km"]; oSheet.Cells[index, 5].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 5].Borders.LineStyle = Excel.XlLineStyle.xlContinuous; index++;
                    }
                    this.sqlDtRdrServer.Close();

                    // Cria a seção de informações dos transformadores MT/MT e MT/BT
                    index = index + 2;
                    this.sqlCommServer = new SqlCommand("SELECT t1.* FROM dbo.AuxReportSGT03 AS t1 WHERE t1.CodBase = " + this.strBaseSelected, this.sqlConnServer);
                    this.sqlCommServer.CommandTimeout = 0;
                    this.sqlDtRdrServer = this.sqlCommServer.ExecuteReader();
                    oSheet.get_Range("A" + index, "N" + index).Merge(true);
                    oSheet.get_Range("A" + index, "N" + index).Value = "TABELA FIS.8 - TRANSFORMADORES MT/MT E MT/BT"; oSheet.get_Range("A" + index, "N" + index).Font.Bold = true; oSheet.get_Range("A" + index, "N" + index).HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.get_Range("A" + index, "N" + index).Borders.LineStyle = Excel.XlLineStyle.xlContinuous; index++;
                    oSheet.Cells[index, 1] = "Tensão Primária"; oSheet.Cells[index, 1].Font.Bold = true; oSheet.Cells[index, 1].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 1].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    oSheet.Cells[index, 2] = "Tensão Secundária"; oSheet.Cells[index, 2].Font.Bold = true; oSheet.Cells[index, 2].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 2].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    oSheet.Cells[index, 3] = "Urbano (UR) - Quantidade"; oSheet.Cells[index, 3].Font.Bold = true; oSheet.Cells[index, 3].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 3].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    oSheet.Cells[index, 4] = "Urbano (UR) - Potência Total"; oSheet.Cells[index, 4].Font.Bold = true; oSheet.Cells[index, 4].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 4].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    oSheet.Cells[index, 5] = "Não Urbano (NUR) - Quantidade"; oSheet.Cells[index, 5].Font.Bold = true; oSheet.Cells[index, 5].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 5].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    oSheet.Cells[index, 6] = "Não Urbano (NUR) - Potência Total"; oSheet.Cells[index, 6].Font.Bold = true; oSheet.Cells[index, 6].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 6].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    oSheet.Cells[index, 7] = "Não Classificado Classe - Quantidade"; oSheet.Cells[index, 7].Font.Bold = true; oSheet.Cells[index, 7].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 7].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    oSheet.Cells[index, 8] = "Não Classificado Classe - Potência Total"; oSheet.Cells[index, 8].Font.Bold = true; oSheet.Cells[index, 8].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 8].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    oSheet.Cells[index, 9] = "Próprio (PR) - Quantidade"; oSheet.Cells[index, 9].Font.Bold = true; oSheet.Cells[index, 9].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 9].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    oSheet.Cells[index, 10] = "Próprio (PR) - Potência Total"; oSheet.Cells[index, 10].Font.Bold = true; oSheet.Cells[index, 10].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 10].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    oSheet.Cells[index, 11] = "Terceiros (TC) - Quantidade"; oSheet.Cells[index, 11].Font.Bold = true; oSheet.Cells[index, 11].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 11].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    oSheet.Cells[index, 12] = "Terceiros (TC) - Potência Total"; oSheet.Cells[index, 12].Font.Bold = true; oSheet.Cells[index, 12].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 12].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    oSheet.Cells[index, 13] = "Não Classificado Propriedade - Quantidade"; oSheet.Cells[index, 13].Font.Bold = true; oSheet.Cells[index, 13].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 13].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    oSheet.Cells[index, 14] = "Não Classificado Propriedade - Potência Total"; oSheet.Cells[index, 14].Font.Bold = true; oSheet.Cells[index, 14].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 14].Borders.LineStyle = Excel.XlLineStyle.xlContinuous; index++;
                    oSheet.Cells[index, 1] = "kV"; oSheet.Cells[index, 1].Font.Bold = true; oSheet.Cells[index, 1].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 1].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    oSheet.Cells[index, 2] = "kV"; oSheet.Cells[index, 2].Font.Bold = true; oSheet.Cells[index, 2].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 2].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    oSheet.Cells[index, 3] = "Unidades"; oSheet.Cells[index, 3].Font.Bold = true; oSheet.Cells[index, 3].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 3].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    oSheet.Cells[index, 4] = "kVA"; oSheet.Cells[index, 4].Font.Bold = true; oSheet.Cells[index, 4].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 4].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    oSheet.Cells[index, 5] = "Unidades"; oSheet.Cells[index, 5].Font.Bold = true; oSheet.Cells[index, 5].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 5].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    oSheet.Cells[index, 6] = "kVA"; oSheet.Cells[index, 6].Font.Bold = true; oSheet.Cells[index, 6].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 6].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    oSheet.Cells[index, 7] = "Unidades"; oSheet.Cells[index, 7].Font.Bold = true; oSheet.Cells[index, 7].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 7].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    oSheet.Cells[index, 8] = "kVA"; oSheet.Cells[index, 8].Font.Bold = true; oSheet.Cells[index, 8].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 8].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    oSheet.Cells[index, 9] = "Unidades"; oSheet.Cells[index, 9].Font.Bold = true; oSheet.Cells[index, 9].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 9].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    oSheet.Cells[index, 10] = "kVA"; oSheet.Cells[index, 10].Font.Bold = true; oSheet.Cells[index, 10].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 10].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    oSheet.Cells[index, 11] = "Unidades"; oSheet.Cells[index, 11].Font.Bold = true; oSheet.Cells[index, 11].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 11].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    oSheet.Cells[index, 12] = "kVA"; oSheet.Cells[index, 12].Font.Bold = true; oSheet.Cells[index, 12].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 12].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    oSheet.Cells[index, 13] = "Unidades"; oSheet.Cells[index, 13].Font.Bold = true; oSheet.Cells[index, 13].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 13].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    oSheet.Cells[index, 14] = "kVA"; oSheet.Cells[index, 14].Font.Bold = true; oSheet.Cells[index, 14].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 14].Borders.LineStyle = Excel.XlLineStyle.xlContinuous; index++;
                    while (this.sqlDtRdrServer.Read())
                    {
                        oSheet.Cells[index, 1] = "'" + this.sqlDtRdrServer["ClassTenPrim"]; oSheet.Cells[index, 1].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 1].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        oSheet.Cells[index, 2] = "'" + this.sqlDtRdrServer["ClassTenSecu"]; oSheet.Cells[index, 2].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 2].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        oSheet.Cells[index, 3] = this.sqlDtRdrServer["Quant_Urbano"]; oSheet.Cells[index, 3].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 3].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        oSheet.Cells[index, 4] = this.sqlDtRdrServer["PotNom_kVA_Urbano"]; oSheet.Cells[index, 4].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 4].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        oSheet.Cells[index, 5] = this.sqlDtRdrServer["Quant_NUrbano"]; oSheet.Cells[index, 5].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 5].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        oSheet.Cells[index, 6] = this.sqlDtRdrServer["PotNom_kVA_NUrbano"]; oSheet.Cells[index, 6].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 6].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        oSheet.Cells[index, 7] = this.sqlDtRdrServer["Quant_ClassIndeter"]; oSheet.Cells[index, 7].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 7].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        oSheet.Cells[index, 8] = this.sqlDtRdrServer["PotNom_kVA_ClassIndeter"]; oSheet.Cells[index, 8].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 8].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        oSheet.Cells[index, 9] = this.sqlDtRdrServer["Quant_Proprio"]; oSheet.Cells[index, 9].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 9].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        oSheet.Cells[index, 10] = this.sqlDtRdrServer["PotNom_kVA_Proprio"]; oSheet.Cells[index, 10].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 10].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        oSheet.Cells[index, 11] = this.sqlDtRdrServer["Quant_Terceiro"]; oSheet.Cells[index, 11].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 11].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        oSheet.Cells[index, 12] = this.sqlDtRdrServer["PotNom_kVA_Terceiro"]; oSheet.Cells[index, 12].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 12].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        oSheet.Cells[index, 13] = this.sqlDtRdrServer["Quant_ProprIndeter"]; oSheet.Cells[index, 13].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 13].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        oSheet.Cells[index, 14] = this.sqlDtRdrServer["PotNom_kVA_ProprIndeter"]; oSheet.Cells[index, 14].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 14].Borders.LineStyle = Excel.XlLineStyle.xlContinuous; index++;
                    }
                    this.sqlDtRdrServer.Close();

                    // Cria a seção de informações dos segmentos BT
                    index = index + 2;
                    this.sqlCommServer = new SqlCommand("SELECT t1.* FROM dbo.AuxReportSGT04 AS t1 WHERE t1.CodBase = " + this.strBaseSelected, this.sqlConnServer);
                    this.sqlCommServer.CommandTimeout = 0;
                    this.sqlDtRdrServer = this.sqlCommServer.ExecuteReader();
                    oSheet.get_Range("A" + index, "E" + index).Merge(true);
                    oSheet.get_Range("A" + index, "E" + index).Value = "TABELA FIS.9 - SEGMENTOS BT"; oSheet.get_Range("A" + index, "E" + index).Font.Bold = true; oSheet.get_Range("A" + index, "E" + index).HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.get_Range("A" + index, "E" + index).Borders.LineStyle = Excel.XlLineStyle.xlContinuous; index++;
                    oSheet.Cells[index, 1] = "Tipo"; oSheet.Cells[index, 1].Font.Bold = true; oSheet.Cells[index, 1].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 1].Borders.LineStyle = Excel.XlLineStyle.xlContinuous; oSheet.Cells[index, 2] = "Urbano (UR)"; oSheet.Cells[index, 2].Font.Bold = true; oSheet.Cells[index, 2].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 2].Borders.LineStyle = Excel.XlLineStyle.xlContinuous; oSheet.Cells[index, 3] = "Não Urbano (NUR)"; oSheet.Cells[index, 3].Font.Bold = true; oSheet.Cells[index, 3].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 3].Borders.LineStyle = Excel.XlLineStyle.xlContinuous; oSheet.Cells[index, 4] = "Terceiros (TC)"; oSheet.Cells[index, 4].Font.Bold = true; oSheet.Cells[index, 4].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 4].Borders.LineStyle = Excel.XlLineStyle.xlContinuous; oSheet.Cells[index, 5] = "Não Classificado"; oSheet.Cells[index, 5].Font.Bold = true; oSheet.Cells[index, 5].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 5].Borders.LineStyle = Excel.XlLineStyle.xlContinuous; index++;
                    oSheet.Cells[index, 1].Font.Bold = true; oSheet.Cells[index, 1].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 1].Borders.LineStyle = Excel.XlLineStyle.xlContinuous; oSheet.Cells[index, 2] = "km"; oSheet.Cells[index, 2].Font.Bold = true; oSheet.Cells[index, 2].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 2].Borders.LineStyle = Excel.XlLineStyle.xlContinuous; oSheet.Cells[index, 3] = "km"; oSheet.Cells[index, 3].Font.Bold = true; oSheet.Cells[index, 3].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 3].Borders.LineStyle = Excel.XlLineStyle.xlContinuous; oSheet.Cells[index, 4] = "km"; oSheet.Cells[index, 4].Font.Bold = true; oSheet.Cells[index, 4].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 4].Borders.LineStyle = Excel.XlLineStyle.xlContinuous; oSheet.Cells[index, 5] = "km"; oSheet.Cells[index, 5].Font.Bold = true; oSheet.Cells[index, 5].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 5].Borders.LineStyle = Excel.XlLineStyle.xlContinuous; index++;
                    while (this.sqlDtRdrServer.Read())
                    {
                        oSheet.Cells[index, 1] = this.sqlDtRdrServer["CodTipo"]; oSheet.Cells[index, 1].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 1].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        oSheet.Cells[index, 2] = this.sqlDtRdrServer["Urbano_km"]; oSheet.Cells[index, 2].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 2].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        oSheet.Cells[index, 3] = this.sqlDtRdrServer["NaoUrbano_km"]; oSheet.Cells[index, 3].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 3].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        oSheet.Cells[index, 4] = this.sqlDtRdrServer["Terceiro_km"]; oSheet.Cells[index, 4].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 4].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        oSheet.Cells[index, 5] = this.sqlDtRdrServer["Indeterminado_km"]; oSheet.Cells[index, 5].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 5].Borders.LineStyle = Excel.XlLineStyle.xlContinuous; index++;
                    }
                    this.sqlDtRdrServer.Close();

                    // Cria a seção de informações das energias fornecida
                    index = index + 2;
                    this.sqlCommServer = new SqlCommand("SELECT t1.* FROM dbo.viewReportSGT02 AS t1 WHERE t1.CodBase = " + this.strBaseSelected, this.sqlConnServer);
                    this.sqlCommServer.CommandTimeout = 0;
                    this.sqlDtRdrServer = this.sqlCommServer.ExecuteReader();
                    oSheet.get_Range("A" + index, "M" + index).Merge(true);
                    oSheet.get_Range("A" + index, "M" + index).Value = "TABELA ENE.1 - BALANÇO DE ENERGIA POR SUBGRUPO (FAIXA DE TENSÃO) - ENERGIA FORNECIDA"; oSheet.get_Range("A" + index, "M" + index).Font.Bold = true; oSheet.get_Range("A" + index, "M" + index).HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.get_Range("A" + index, "M" + index).Borders.LineStyle = Excel.XlLineStyle.xlContinuous; index++;
                    oSheet.Cells[index, 1] = "Subgrupo"; oSheet.Cells[index, 1].Font.Bold = true; oSheet.Cells[index, 1].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 1].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    oSheet.Cells[index, 2] = "Mês 01"; oSheet.Cells[index, 2].Font.Bold = true; oSheet.Cells[index, 2].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 2].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    oSheet.Cells[index, 3] = "Mês 02"; oSheet.Cells[index, 3].Font.Bold = true; oSheet.Cells[index, 3].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 3].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    oSheet.Cells[index, 4] = "Mês 03"; oSheet.Cells[index, 4].Font.Bold = true; oSheet.Cells[index, 4].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 4].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    oSheet.Cells[index, 5] = "Mês 04"; oSheet.Cells[index, 5].Font.Bold = true; oSheet.Cells[index, 5].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 5].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    oSheet.Cells[index, 6] = "Mês 05"; oSheet.Cells[index, 6].Font.Bold = true; oSheet.Cells[index, 6].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 6].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    oSheet.Cells[index, 7] = "Mês 06"; oSheet.Cells[index, 7].Font.Bold = true; oSheet.Cells[index, 7].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 7].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    oSheet.Cells[index, 8] = "Mês 07"; oSheet.Cells[index, 8].Font.Bold = true; oSheet.Cells[index, 8].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 8].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    oSheet.Cells[index, 9] = "Mês 08"; oSheet.Cells[index, 9].Font.Bold = true; oSheet.Cells[index, 9].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 9].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    oSheet.Cells[index, 10] = "Mês 09"; oSheet.Cells[index, 10].Font.Bold = true; oSheet.Cells[index, 10].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 10].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    oSheet.Cells[index, 11] = "Mês 10"; oSheet.Cells[index, 11].Font.Bold = true; oSheet.Cells[index, 11].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 11].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    oSheet.Cells[index, 12] = "Mês 11"; oSheet.Cells[index, 12].Font.Bold = true; oSheet.Cells[index, 12].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 12].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    oSheet.Cells[index, 13] = "Mês 12"; oSheet.Cells[index, 13].Font.Bold = true; oSheet.Cells[index, 13].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 13].Borders.LineStyle = Excel.XlLineStyle.xlContinuous; index++;
                    while (this.sqlDtRdrServer.Read())
                    {
                        oSheet.Cells[index, 1] = this.sqlDtRdrServer["NivTen"]; oSheet.Cells[index, 1].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 1].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        oSheet.Cells[index, 2] = this.sqlDtRdrServer["EnerForn01_MWh"]; oSheet.Cells[index, 2].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 2].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        oSheet.Cells[index, 3] = this.sqlDtRdrServer["EnerForn02_MWh"]; oSheet.Cells[index, 3].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 3].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        oSheet.Cells[index, 4] = this.sqlDtRdrServer["EnerForn03_MWh"]; oSheet.Cells[index, 4].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 4].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        oSheet.Cells[index, 5] = this.sqlDtRdrServer["EnerForn04_MWh"]; oSheet.Cells[index, 5].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 5].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        oSheet.Cells[index, 6] = this.sqlDtRdrServer["EnerForn05_MWh"]; oSheet.Cells[index, 6].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 6].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        oSheet.Cells[index, 7] = this.sqlDtRdrServer["EnerForn06_MWh"]; oSheet.Cells[index, 7].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 7].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        oSheet.Cells[index, 8] = this.sqlDtRdrServer["EnerForn07_MWh"]; oSheet.Cells[index, 8].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 8].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        oSheet.Cells[index, 9] = this.sqlDtRdrServer["EnerForn08_MWh"]; oSheet.Cells[index, 9].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 9].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        oSheet.Cells[index, 10] = this.sqlDtRdrServer["EnerForn09_MWh"]; oSheet.Cells[index, 10].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 10].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        oSheet.Cells[index, 11] = this.sqlDtRdrServer["EnerForn10_MWh"]; oSheet.Cells[index, 11].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 11].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        oSheet.Cells[index, 12] = this.sqlDtRdrServer["EnerForn11_MWh"]; oSheet.Cells[index, 12].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 12].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        oSheet.Cells[index, 13] = this.sqlDtRdrServer["EnerForn12_MWh"]; oSheet.Cells[index, 13].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 13].Borders.LineStyle = Excel.XlLineStyle.xlContinuous; index++;
                    }
                    this.sqlDtRdrServer.Close();

                    // Cria a seção de informações das energias fornecida
                    index = index + 2;
                    this.sqlCommServer = new SqlCommand("SELECT t1.* FROM dbo.viewReportSGT02 AS t1 WHERE t1.CodBase = " + this.strBaseSelected, this.sqlConnServer);
                    this.sqlCommServer.CommandTimeout = 0;
                    this.sqlDtRdrServer = this.sqlCommServer.ExecuteReader();
                    oSheet.get_Range("A" + index, "M" + index).Merge(true);
                    oSheet.get_Range("A" + index, "M" + index).Value = "TABELA ENE.1 - BALANÇO DE ENERGIA POR SUBGRUPO (FAIXA DE TENSÃO) - ENERGIA INJETADA POR GERAÇÃO"; oSheet.get_Range("A" + index, "M" + index).Font.Bold = true; oSheet.get_Range("A" + index, "M" + index).HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.get_Range("A" + index, "M" + index).Borders.LineStyle = Excel.XlLineStyle.xlContinuous; index++;
                    oSheet.Cells[index, 1] = "Subgrupo"; oSheet.Cells[index, 1].Font.Bold = true; oSheet.Cells[index, 1].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 1].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    oSheet.Cells[index, 2] = "Mês 01"; oSheet.Cells[index, 2].Font.Bold = true; oSheet.Cells[index, 2].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 2].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    oSheet.Cells[index, 3] = "Mês 02"; oSheet.Cells[index, 3].Font.Bold = true; oSheet.Cells[index, 3].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 3].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    oSheet.Cells[index, 4] = "Mês 03"; oSheet.Cells[index, 4].Font.Bold = true; oSheet.Cells[index, 4].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 4].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    oSheet.Cells[index, 5] = "Mês 04"; oSheet.Cells[index, 5].Font.Bold = true; oSheet.Cells[index, 5].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 5].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    oSheet.Cells[index, 6] = "Mês 05"; oSheet.Cells[index, 6].Font.Bold = true; oSheet.Cells[index, 6].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 6].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    oSheet.Cells[index, 7] = "Mês 06"; oSheet.Cells[index, 7].Font.Bold = true; oSheet.Cells[index, 7].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 7].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    oSheet.Cells[index, 8] = "Mês 07"; oSheet.Cells[index, 8].Font.Bold = true; oSheet.Cells[index, 8].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 8].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    oSheet.Cells[index, 9] = "Mês 08"; oSheet.Cells[index, 9].Font.Bold = true; oSheet.Cells[index, 9].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 9].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    oSheet.Cells[index, 10] = "Mês 09"; oSheet.Cells[index, 10].Font.Bold = true; oSheet.Cells[index, 10].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 10].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    oSheet.Cells[index, 11] = "Mês 10"; oSheet.Cells[index, 11].Font.Bold = true; oSheet.Cells[index, 11].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 11].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    oSheet.Cells[index, 12] = "Mês 11"; oSheet.Cells[index, 12].Font.Bold = true; oSheet.Cells[index, 12].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 12].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    oSheet.Cells[index, 13] = "Mês 12"; oSheet.Cells[index, 13].Font.Bold = true; oSheet.Cells[index, 13].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 13].Borders.LineStyle = Excel.XlLineStyle.xlContinuous; index++;
                    while (this.sqlDtRdrServer.Read())
                    {
                        oSheet.Cells[index, 1] = this.sqlDtRdrServer["NivTen"]; oSheet.Cells[index, 1].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 1].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        oSheet.Cells[index, 2] = this.sqlDtRdrServer["EnerInjGer01_MWh"]; oSheet.Cells[index, 2].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 2].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        oSheet.Cells[index, 3] = this.sqlDtRdrServer["EnerInjGer02_MWh"]; oSheet.Cells[index, 3].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 3].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        oSheet.Cells[index, 4] = this.sqlDtRdrServer["EnerInjGer03_MWh"]; oSheet.Cells[index, 4].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 4].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        oSheet.Cells[index, 5] = this.sqlDtRdrServer["EnerInjGer04_MWh"]; oSheet.Cells[index, 5].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 5].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        oSheet.Cells[index, 6] = this.sqlDtRdrServer["EnerInjGer05_MWh"]; oSheet.Cells[index, 6].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 6].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        oSheet.Cells[index, 7] = this.sqlDtRdrServer["EnerInjGer06_MWh"]; oSheet.Cells[index, 7].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 7].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        oSheet.Cells[index, 8] = this.sqlDtRdrServer["EnerInjGer07_MWh"]; oSheet.Cells[index, 8].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 8].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        oSheet.Cells[index, 9] = this.sqlDtRdrServer["EnerInjGer08_MWh"]; oSheet.Cells[index, 9].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 9].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        oSheet.Cells[index, 10] = this.sqlDtRdrServer["EnerInjGer09_MWh"]; oSheet.Cells[index, 10].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 10].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        oSheet.Cells[index, 11] = this.sqlDtRdrServer["EnerInjGer10_MWh"]; oSheet.Cells[index, 11].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 11].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        oSheet.Cells[index, 12] = this.sqlDtRdrServer["EnerInjGer11_MWh"]; oSheet.Cells[index, 12].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 12].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        oSheet.Cells[index, 13] = this.sqlDtRdrServer["EnerInjGer12_MWh"]; oSheet.Cells[index, 13].HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter; oSheet.Cells[index, 13].Borders.LineStyle = Excel.XlLineStyle.xlContinuous; index++;
                    }
                    this.sqlDtRdrServer.Close();
            
                    // Format A1:D1 as bold, vertical alignment = center.
                    //oSheet.get_Range("A1", "D1").Font.Bold = true;
                    //oSheet.get_Range("A1", "D1").VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;

                    // Create an array to multiple values at once.
                    /*
                    string[,] saNames = new string[5, 2];
                    saNames[0, 0] = "John";
                    saNames[0, 1] = "Smith";
                    saNames[1, 0] = "Tom";
                    saNames[1, 1] = "Brown";
                    saNames[2, 0] = "Sue";
                    saNames[2, 1] = "Thomas";
                    saNames[3, 0] = "Jane";
                    saNames[3, 1] = "Jones";
                    saNames[4, 0] = "Adam";
                    saNames[4, 1] = "Johnson";
                    */

                    //Fill A2:B6 with an array of values (First and Last Names).
                    //oSheet.get_Range("A2", "B6").Value2 = saNames;

                    //Fill C2:C6 with a relative formula (=A2 & " " & B2).
                    //oRng = oSheet.get_Range("C2", "C6");
                    //oRng.Formula = "=A2 & \" \" & B2";

                    //Fill D2:D6 with a formula(=RAND()*100000) and apply format.
                    //oRng = oSheet.get_Range("D2", "D6");
                    //oRng.Formula = "=RAND()*100000";
                    //oRng.NumberFormat = "$0.00";

                    //AutoFit columns A:D.
                    oRng = oSheet.get_Range("A1", "N100");
                    //oRng.EntireColumn.AutoFit();
                    oRng.EntireColumn.ColumnWidth = 14;

                    //Manipulate a variable number of columns for Quarterly Sales Data.
                    //DisplayQuarterlySales(oSheet);

                    //Make sure Excel is visible and give the user control
                    //of Microsoft Excel's lifetime.
                    oXL.Visible = true;
                    oXL.UserControl = true;

                    this.mthdInputMensageLog("Relatório da SGT exportado.");

                }
                catch (Exception xcptn)
                {
                    
                    this.mthdInputMensageLog("Problema não identificado (" + xcptn.Message.ToString() + ").");
                    this.sqlCommServer.Dispose();

                }

            }
            
        }

        private void bReportVoltageComplete_Click(object sender, EventArgs e)
        {

            int i = 0, intCtrlCnvrg = 0;
            String strTempSqlCommand, strTensoesBase;
            int intAno = 0;
            double tolerance;
            double dblEnerInjDU_kWh = 0, dblEnerInjSA_kWh = 0, dblEnerInjDO_kWh = 0;
            double dblPerdaEnerTecDU_kWh = 0, dblPerdaEnerTecSA_kWh = 0, dblPerdaEnerTecDO_kWh = 0;
            double dblEnerForncDU_kWh = 0, dblEnerForncSA_kWh = 0, dblEnerForncDO_kWh = 0;
            double dblDelta = 0, dblDeltaMT = 0, dblDeltaBT = 0, dblDelta_MWh = 0, dblDeltaBT_MWh = 0, dblDeltaMT_MWh = 0;
            Bus dssBus;
            dynamic Vseqn, cpxVseqn, V;
            dynamic vnodes;
            String strvnodes;
            double[] VseqnVetor = { 0, 0, 0 };
            double[] cpxVseqnVetor = { 0, 0, 0, 0, 0, 0 };
            double[] VVetor = { 0, 0, 0, 0, 0, 0 };

            if (checkBase())
            {

                try
                {

                    // Verifica as opções solicitadas pelo usuário
                    this.mthdUpdateOptionsRun();

                    // Captura a base a ser calculada
                    this.strBaseSelected = this.tbObjectBase.Text;
                    
                    // Loop da base e dos alimentadores
                    this.strFeedersSelected.Clear();
                    if (this.tbObjectFeeder.Text == null || this.tbObjectFeeder.Text == "")
                        this.sqlCommServer = new SqlCommand("SELECT t1.* FROM dbo.CircMT AS t1 WHERE t1.CodBase = " + this.strBaseSelected + " AND t1.CircAtipResultante = 0", this.sqlConnServer);
                    else
                        this.sqlCommServer = new SqlCommand("SELECT t1.* FROM dbo.CircMT AS t1 WHERE t1.CodBase = " + this.strBaseSelected + " AND " + GeoPerdasTools.mthdQuerySQLFeeders(this.tbObjectFeeder.Text) + " AND t1.CircAtipResultante = 0", this.sqlConnServer);
                    this.sqlCommServer.CommandTimeout = 0;
                    this.sqlDtRdrServer = this.sqlCommServer.ExecuteReader();
                    while (this.sqlDtRdrServer.Read())
                    {
                        this.strFeedersSelected.Add(this.sqlDtRdrServer["CodAlim"].ToString());
                    }
                    this.sqlDtRdrServer.Close();

                    foreach (String strFeeder in strFeedersSelected)
                    {

                        // Executa a stored procedure da limpeza das tensões de barras
                        this.sqlCommServer = new SqlCommand("dbo.iadAuxReportVoltage", this.sqlConnServer);
                        this.sqlCommServer.CommandType = CommandType.StoredProcedure;
                        this.sqlCommServer.Parameters.AddWithValue("@CodBase", this.strBaseSelected);
                        this.sqlCommServer.Parameters.AddWithValue("@CodAlim", strFeeder);
                        this.sqlCommServer.Parameters.AddWithValue("@CodOperation", 1);
                        this.sqlCommServer.Parameters.AddWithValue("@TipoDiaMes", "");
                        this.sqlCommServer.Parameters.AddWithValue("@TipoRodada", this.mthdGetOptionsToString());
                        this.sqlCommServer.Parameters.AddWithValue("@CodBarra", "");
                        this.sqlCommServer.Parameters.AddWithValue("@NodesNumber", 0);
                        this.sqlCommServer.Parameters.AddWithValue("@Nodes", "");
                        this.sqlCommServer.Parameters.AddWithValue("@VoltageBase_kV", 0);
                        this.sqlCommServer.Parameters.AddWithValue("@MOD_V0_V", 0);
                        this.sqlCommServer.Parameters.AddWithValue("@MOD_V1_V", 0);
                        this.sqlCommServer.Parameters.AddWithValue("@MOD_V2_V", 0);
                        this.sqlCommServer.Parameters.AddWithValue("@REAL_V0_kV", 0);
                        this.sqlCommServer.Parameters.AddWithValue("@IMAJ_V0_kV", 0);
                        this.sqlCommServer.Parameters.AddWithValue("@REAL_V1_kV", 0);
                        this.sqlCommServer.Parameters.AddWithValue("@IMAJ_V1_kV", 0);
                        this.sqlCommServer.Parameters.AddWithValue("@REAL_V2_kV", 0);
                        this.sqlCommServer.Parameters.AddWithValue("@IMAJ_V2_kV", 0);
                        this.sqlCommServer.Parameters.AddWithValue("@REAL_Va_V", 0);
                        this.sqlCommServer.Parameters.AddWithValue("@IMAJ_Va_V", 0);
                        this.sqlCommServer.Parameters.AddWithValue("@REAL_Vb_V", 0);
                        this.sqlCommServer.Parameters.AddWithValue("@IMAJ_Vb_V", 0);
                        this.sqlCommServer.Parameters.AddWithValue("@REAL_Vc_V", 0);
                        this.sqlCommServer.Parameters.AddWithValue("@IMAJ_Vc_V", 0);
                        this.sqlCommServer.Parameters.AddWithValue("@MOD_Va_V", 0);
                        this.sqlCommServer.Parameters.AddWithValue("@ANG_Va_V", 0);
                        this.sqlCommServer.Parameters.AddWithValue("@MOD_Vb_V", 0);
                        this.sqlCommServer.Parameters.AddWithValue("@ANG_Vb_V", 0);
                        this.sqlCommServer.Parameters.AddWithValue("@MOD_Vc_V", 0);
                        this.sqlCommServer.Parameters.AddWithValue("@ANG_Vc_V", 0);
                        this.sqlCommServer.ExecuteNonQuery();

                        // Início do processo
                        this.mthdInputMensageLog("Início do processo para o alimentador " + strFeeder + ".");

                        this.mthdStarCommands();

                        // Início do código
                        this.mthdInsertCommandInLine("Clear");

                        // Loop dos códigos dos circuitos
                        this.sqlCommServer = new SqlCommand("SELECT t1.* FROM dbo.CircMT AS t1 WHERE t1.CodBase = " + this.strBaseSelected + " AND t1.CodAlim = '" + strFeeder + "'", this.sqlConnServer);
                        this.sqlCommServer.CommandTimeout = 0;
                        this.mthdInsertCommandInLine("! Criação da seção de circuitos");
                        this.sqlDtRdrServer = this.sqlCommServer.ExecuteReader();
                        while (this.sqlDtRdrServer.Read())
                        {
                            this.mthdGetDSSCommandCircuit(strFeeder, Convert.ToDouble(this.sqlDtRdrServer["TenNom_kV"]), Convert.ToDouble(this.sqlDtRdrServer["TenOpe_pu"]), this.sqlDtRdrServer["CodPonAcopl"].ToString(), this.intAdequarTensaoSuperior);
                        }
                        this.sqlDtRdrServer.Close();
                        this.mthdInputMensageLog("Passou pelo circuito.");

                        // Loop dos códigos dos condutores
                        this.sqlCommServer = new SqlCommand("SELECT t1.* FROM dbo.CodCondutor AS t1 WHERE t1.CodBase = " + this.strBaseSelected, this.sqlConnServer);
                        this.sqlCommServer.CommandTimeout = 0;
                        this.mthdInsertCommandInLine("! Criação da seção de condutores");
                        this.sqlDtRdrServer = this.sqlCommServer.ExecuteReader();
                        while (this.sqlDtRdrServer.Read())
                        {
                            this.mthdGetDSSCommandLinecode(this.sqlDtRdrServer["CodCond"].ToString(), Convert.ToDouble(this.sqlDtRdrServer["Resis_ohms_km"]), Convert.ToDouble(this.sqlDtRdrServer["Reat_ohms_km"]), Convert.ToDouble(this.sqlDtRdrServer["CorrMax_A"]));
                        }
                        this.sqlDtRdrServer.Close();
                        this.mthdInputMensageLog("Passou pelos condutores.");

                        // Loop das chaves MT
                        this.sqlCommServer = new SqlCommand("SELECT t1.CodChvMT, t1.CodFas, t1.De, t1.Para FROM dbo.ChaveMT AS t1 WHERE t1.CodBase = " + this.strBaseSelected + " AND t1.CodAlimAtrib = '" + strFeeder + "' AND t1.EstChv = 2", this.sqlConnServer);
                        this.sqlCommServer.CommandTimeout = 0;
                        this.mthdInsertCommandInLine("! Criação da seção de chaves MT");
                        this.sqlDtRdrServer = this.sqlCommServer.ExecuteReader();
                        while (this.sqlDtRdrServer.Read())
                        {
                            this.mthdGetDSSCommandMTSwitch("CMT_" + this.sqlDtRdrServer["CodChvMT"].ToString(), this.sqlDtRdrServer["CodFas"].ToString(), this.sqlDtRdrServer["De"].ToString(), this.sqlDtRdrServer["Para"].ToString());
                        }
                        this.sqlDtRdrServer.Close();
                        this.mthdInputMensageLog("Passou pelas chaves MT.");

                        // Loop dos segmentos MT
                        this.sqlCommServer = new SqlCommand("SELECT t1.CodSegmMT, t1.CodFas, t1.CodCond, t1.Comp_km, t1.De, t1.Para, t1.Propr FROM dbo.SegmentoMT AS t1 WHERE t1.CodBase = " + this.strBaseSelected + " AND t1.CodAlimAtrib = '" + strFeeder + "'", this.sqlConnServer);
                        this.sqlCommServer.CommandTimeout = 0;
                        this.mthdInsertCommandInLine("! Criação da seção de segmentos MT");
                        this.sqlDtRdrServer = this.sqlCommServer.ExecuteReader();
                        while (this.sqlDtRdrServer.Read())
                        {
                            this.mthdGetDSSCommandLineMT("SMT_" + this.sqlDtRdrServer["CodSegmMT"].ToString(), this.sqlDtRdrServer["CodFas"].ToString(), this.sqlDtRdrServer["De"].ToString(), this.sqlDtRdrServer["Para"].ToString(), this.sqlDtRdrServer["CodCond"].ToString(), Convert.ToDouble(this.sqlDtRdrServer["Comp_km"]), this.sqlDtRdrServer["Propr"].ToString(), 0, intNeutralizarRedeTerceiros);
                        }
                        this.sqlDtRdrServer.Close();
                        this.mthdInputMensageLog("Passou pelos segmentos MT.");

                        // Loop dos reguladores MT
                        this.sqlCommServer = new SqlCommand("SELECT t1.CodRegulMT, t1.CodBnc, t1.TipRegul, t1.CodFasPrim, t1.CodFasSecu, t1.PotNom_kVA, t1.TenRgl_pu, t1.[ReatHL_%], t1.PerdTtl_W, t1.PerdVz_W, t1.TnsLnh1_kV, t1.De, t1.Para, t1.CodFasCoinc FROM dbo.ReguladorMT AS t1 WHERE t1.CodBase = " + this.strBaseSelected + " AND t1.CodAlimAtrib = '" + strFeeder + "'", this.sqlConnServer);
                        this.sqlCommServer.CommandTimeout = 0;
                        this.mthdInsertCommandInLine("! Criação da seção de reguladores MT");
                        this.sqlDtRdrServer = this.sqlCommServer.ExecuteReader();
                        while (this.sqlDtRdrServer.Read())
                        {
                            this.mthdGetDSSCommandRegulatorMT("REG_" + this.sqlDtRdrServer["CodRegulMT"].ToString(), Convert.ToInt32(this.sqlDtRdrServer["CodBnc"]), Convert.ToInt32(this.sqlDtRdrServer["TipRegul"]), this.sqlDtRdrServer["CodFasPrim"].ToString(), this.sqlDtRdrServer["CodFasSecu"].ToString(), this.sqlDtRdrServer["CodFasCoinc"].ToString(), this.sqlDtRdrServer["De"].ToString(), this.sqlDtRdrServer["Para"].ToString(), Convert.ToDouble(this.sqlDtRdrServer["TnsLnh1_kV"]), Convert.ToDouble(this.sqlDtRdrServer["PotNom_kVA"]), Convert.ToDouble(this.sqlDtRdrServer["TenRgl_pu"]), Convert.ToDouble(this.sqlDtRdrServer["ReatHL_%"]), (100 * Convert.ToDouble(this.sqlDtRdrServer["PerdTtl_W"])) / (1000 * Convert.ToDouble(this.sqlDtRdrServer["PotNom_kVA"])), (100 * Convert.ToDouble(this.sqlDtRdrServer["PerdVz_W"])) / (1000 * Convert.ToDouble(this.sqlDtRdrServer["PotNom_kVA"])));
                        }
                        this.sqlDtRdrServer.Close();
                        this.mthdInputMensageLog("Passou pelos reguladores MT.");

                        // Loop dos transformadores MT-MT ou MT-BT
                        this.sqlCommServer = new SqlCommand("SELECT t1.* FROM dbo.TrafoMTMTMTBT AS t1 WHERE t1.CodBase = " + this.strBaseSelected + " AND t1.CodAlimAtrib = '" + strFeeder + "'", this.sqlConnServer);
                        this.sqlCommServer.CommandTimeout = 0;
                        this.mthdInsertCommandInLine("! Criação da seção de transformadores MT-MT ou MT-BT");
                        this.sqlDtRdrServer = this.sqlCommServer.ExecuteReader();
                        while (this.sqlDtRdrServer.Read())
                        {
                            this.mthdGetDSSCommandTransformer4Wires("TRF_" + this.sqlDtRdrServer["CodTrafo"].ToString(), Convert.ToInt32(this.sqlDtRdrServer["CodBnc"]), Convert.ToInt32(this.sqlDtRdrServer["MRT"]), Convert.ToInt32(this.sqlDtRdrServer["TipTrafo"]), this.sqlDtRdrServer["CodFasPrim"].ToString(), this.sqlDtRdrServer["CodFasSecu"].ToString(), this.sqlDtRdrServer["CodFasTerc"].ToString(), this.sqlDtRdrServer["De"].ToString(), this.sqlDtRdrServer["Para"].ToString(), Convert.ToDouble(this.sqlDtRdrServer["TnsLnh1_kV"]), Convert.ToDouble(this.sqlDtRdrServer["TenSecu_kV"]), Convert.ToDouble(this.sqlDtRdrServer["Tap_pu"]), Convert.ToDouble(this.sqlDtRdrServer["PotNom_kVA"]), (GeoPerdasTools.mthdTransformerTotalPowerLoss2019_per(this.sqlDtRdrServer["CodFasPrim"].ToString(), Convert.ToDouble(this.sqlDtRdrServer["PotNom_kVA"]), Convert.ToDouble(this.sqlDtRdrServer["TnsLnh1_kV"])) != 0 && this.intUsaTrafoABNT == 1) ? GeoPerdasTools.mthdTransformerTotalPowerLoss2019_per(this.sqlDtRdrServer["CodFasPrim"].ToString(), Convert.ToDouble(this.sqlDtRdrServer["PotNom_kVA"]), Convert.ToDouble(this.sqlDtRdrServer["TnsLnh1_kV"])) : Convert.ToDouble(this.sqlDtRdrServer["PerdTtl_%"]), (GeoPerdasTools.mthdTransformerNoLoadPowerLoss2019_per(this.sqlDtRdrServer["CodFasPrim"].ToString(), Convert.ToDouble(this.sqlDtRdrServer["PotNom_kVA"]), Convert.ToDouble(this.sqlDtRdrServer["TnsLnh1_kV"])) != 0 && this.intUsaTrafoABNT == 1) ? GeoPerdasTools.mthdTransformerNoLoadPowerLoss2019_per(this.sqlDtRdrServer["CodFasPrim"].ToString(), Convert.ToDouble(this.sqlDtRdrServer["PotNom_kVA"]), Convert.ToDouble(this.sqlDtRdrServer["TnsLnh1_kV"])) : Convert.ToDouble(this.sqlDtRdrServer["PerdVz_%"]), this.sqlDtRdrServer["Propr"].ToString(), (this.intAdequarTrafoVazio == 1) ? Convert.ToInt32(this.sqlDtRdrServer["SemCarga"]) : 0);
                        }
                        this.sqlDtRdrServer.Close();
                        this.mthdInputMensageLog("Passou pelos transformadores MT-MT ou MT-BT.");

                        // Loop das chaves BT
                        this.sqlCommServer = new SqlCommand("SELECT t1.CodChvBT, t1.CodFas, t1.De, t1.Para FROM dbo.ChaveBT AS t1 WHERE t1.CodBase = " + this.strBaseSelected + " AND t1.CodAlimAtrib = '" + strFeeder + "' AND t1.EstChv = 2", this.sqlConnServer);
                        this.sqlCommServer.CommandTimeout = 0;
                        this.mthdInsertCommandInLine("! Criação da seção de chaves BT");
                        this.sqlDtRdrServer = this.sqlCommServer.ExecuteReader();
                        while (this.sqlDtRdrServer.Read())
                        {
                            this.mthdGetDSSCommandMTSwitch("CBT_" + this.sqlDtRdrServer["CodChvBT"].ToString(), this.sqlDtRdrServer["CodFas"].ToString(), this.sqlDtRdrServer["De"].ToString(), this.sqlDtRdrServer["Para"].ToString());
                        }
                        this.sqlDtRdrServer.Close();
                        this.mthdInputMensageLog("Passou pelas chaves BT.");

                        // Loop dos segmentos BT
                        this.sqlCommServer = new SqlCommand("SELECT t1.*, [dbo].[EscolhaPropriedade]([t2].[M1_Propr], [t2].[M2_Propr], [t2].[M3_Propr]) AS [Propr] FROM dbo.[" + this.strBaseSelected + "SegmentoBT] AS t1 LEFT JOIN [dbo].[" + this.strBaseSelected + "AuxTrafoMTMTMTBT] AS t2 ON ([t1].[CodBase] = [t2].[M1_CodBase] AND [t1].[CodTrafoAtrib] = [t2].[M1_CodTrafo]) WHERE t1.CodBase = " + this.strBaseSelected + " AND t1.CodAlimAtrib = '" + strFeeder + "'", this.sqlConnServer);
                        this.sqlCommServer.CommandTimeout = 0;
                        this.mthdInsertCommandInLine("! Criação da seção de segmentos BT");
                        this.sqlDtRdrServer = this.sqlCommServer.ExecuteReader();
                        while (this.sqlDtRdrServer.Read())
                        {
                            this.mthdGetDSSCommandLineBT("SBT_" + this.sqlDtRdrServer["CodSegmBT"].ToString(), this.sqlDtRdrServer["CodFas"].ToString(), this.sqlDtRdrServer["CodPonAcopl1"].ToString(), this.sqlDtRdrServer["CodPonAcopl2"].ToString(), this.sqlDtRdrServer["CodCond"].ToString(), Convert.ToDouble(this.sqlDtRdrServer["Comp_km"]), this.sqlDtRdrServer["Propr"].ToString(), 0, this.intNeutralizarRedeTerceiros);
                        }
                        this.sqlDtRdrServer.Close();
                        this.mthdInputMensageLog("Passou pelos segmentos BT.");

                        // Loop dos ramais
                        this.sqlCommServer = new SqlCommand("SELECT t1.*, [dbo].[EscolhaPropriedade]([t2].[M1_Propr], [t2].[M2_Propr], [t2].[M3_Propr]) AS [Propr] FROM dbo.[" + this.strBaseSelected + "RamalBT] AS t1 LEFT JOIN [dbo].[" + this.strBaseSelected + "AuxTrafoMTMTMTBT] AS t2 ON ([t1].[CodBase] = [t2].[M1_CodBase] AND [t1].[CodTrafoAtrib] = [t2].[M1_CodTrafo]) WHERE t1.CodBase = " + this.strBaseSelected + " AND t1.CodAlimAtrib = '" + strFeeder + "'", this.sqlConnServer);
                        this.sqlCommServer.CommandTimeout = 0;
                        this.mthdInsertCommandInLine("! Criação da seção de ramais");
                        this.sqlDtRdrServer = this.sqlCommServer.ExecuteReader();
                        while (this.sqlDtRdrServer.Read())
                        {
                            this.mthdGetDSSCommandLineBT("RBT_" + this.sqlDtRdrServer["CodRmlBT"].ToString(), this.sqlDtRdrServer["CodFas"].ToString(), this.sqlDtRdrServer["CodPonAcopl1"].ToString(), this.sqlDtRdrServer["CodPonAcopl2"].ToString(), this.sqlDtRdrServer["CodCond"].ToString(), Convert.ToDouble(this.sqlDtRdrServer["Comp_km"]), this.sqlDtRdrServer["Propr"].ToString(), this.intAdequarRamal, this.intNeutralizarRedeTerceiros);
                        }
                        this.sqlDtRdrServer.Close();
                        this.mthdInputMensageLog("Passou pelos ramais.");

                        // Versão usando apenas o medidor de barramento

                        // Loop dos energymeters de barramento
                        i = 0;
                        this.sqlCommServer = new SqlCommand("SELECT t1.* FROM dbo.AuxMeterBarra AS t1 WHERE t1.CodBase = " + this.strBaseSelected + " AND t1.CodAlim = '" + strFeeder + "'", this.sqlConnServer);
                        this.sqlCommServer.CommandTimeout = 0;
                        this.mthdInsertCommandInLine("! Criação da seção de energymeters de barramento");
                        this.sqlDtRdrServer = this.sqlCommServer.ExecuteReader();
                        while (this.sqlDtRdrServer.Read())
                        {
                            this.mthdGetDSSCommandEnergymeter(this.sqlDtRdrServer["Nome"].ToString().TrimEnd(), this.mthdNomeElemento(this.sqlDtRdrServer["Elem"].ToString().TrimEnd()) + this.sqlDtRdrServer["CodNomeElem"].ToString().TrimEnd());
                            i++;
                        }
                        this.sqlDtRdrServer.Close();
                        this.mthdInputMensageLog("Passou pelos energymeters de barramento.");

                        // Versão usando a tabela auxiliar de medidores

                        // Loop dos energymeters usando AuxMeter
                        /*
                        this.sqlCommServer = new SqlCommand("SELECT t1.* FROM dbo.AuxMeter AS t1 WHERE t1.CodBase = " + this.strBaseSelected + " AND t1.CodAlim = '" + strFeeder + "'", this.sqlConnServer);
                        this.sqlCommServer.CommandTimeout = 0;
                        this.mthdInsertCommandInLine("! Criação da seção de energymeters");
                        this.sqlDtRdrServer = this.sqlCommServer.ExecuteReader();
                        while (this.sqlDtRdrServer.Read())
                        {
                            this.mthdGetDSSCommandEnergymeter(this.sqlDtRdrServer["Nome"].ToString().TrimEnd(), this.mthdNomeElemento(this.sqlDtRdrServer["Elem"].ToString().TrimEnd()) + this.sqlDtRdrServer["CodNomeElem"].ToString().TrimEnd());
                        }
                        this.sqlDtRdrServer.Close();
                        this.mthdInputMensageLog("Passou pelos energymeters.");
                        */

                        // Loop das tensões de base
                        strTensoesBase = "Set voltagebases=[";
                        i = 0;
                        this.sqlCommServer = new SqlCommand("SELECT t1.* FROM dbo.AuxVoltageBases AS t1 WHERE t1.CodBase = " + this.strBaseSelected + " AND t1.CodAlim = '" + strFeeder + "'", this.sqlConnServer);
                        this.sqlCommServer.CommandTimeout = 0;
                        this.mthdInsertCommandInLine("! Criação da seção de tensões de base");
                        this.sqlDtRdrServer = this.sqlCommServer.ExecuteReader();
                        while (this.sqlDtRdrServer.Read())
                        {
                            if (i == 0)
                                strTensoesBase += this.sqlDtRdrServer["TnsLnh_kV"].ToString().Replace(",", ".");
                            else
                                strTensoesBase += " " + this.sqlDtRdrServer["TnsLnh_kV"].ToString().Replace(",", ".");
                            i++;
                        }
                        this.sqlDtRdrServer.Close();
                        strTensoesBase += "]";
                        this.mthdInsertCommandInLine(strTensoesBase);
                        this.mthdInsertCommandInLine("Calcvoltagebases");
                        this.mthdInputMensageLog("Passou pelas tensões de base.");

                        intCtrlCnvrg = 0;
                        for (int j = 1; j <= 12; j++)
                        {

                            for (int k = 1; k <= 3; k++)
                            {

                                if (j == 1 && k == 1 && intCtrlCnvrg == 0)
                                {

                                    // Flag que sinaliza que a carga não deve ser escrita novamente quando houver necessidade de convergência da NT
                                    intCtrlCnvrg = 1;

                                    // Loop das cargas MT
                                    this.sqlCommServer = new SqlCommand("SELECT DISTINCT t1.CodBase, t1.CodConsMT, t1.CodFas, t1.CodPonAcopl, t1.TipCrvaCarga, t1.DemMax" + GeoPerdasTools.mthdGetTypeDayMonth(j, k) + "_kW, t1.CodAlimAtrib, t1.TnsLnh_kV FROM dbo.[" + this.strBaseSelected + "AuxCargaMTDem] AS t1 WHERE t1.CodBase = " + this.strBaseSelected + " AND t1.CodAlimAtrib = '" + strFeeder + "'", this.sqlConnServer);
                                    this.sqlCommServer.CommandTimeout = 0;
                                    this.mthdInsertCommandInLine("! Criação da seção de cargas MT");
                                    this.sqlDtRdrServer = this.sqlCommServer.ExecuteReader();
                                    while (this.sqlDtRdrServer.Read())
                                    {
                                        this.mthdGetDSSCommandNewLoad("MT_" + this.sqlDtRdrServer["CodConsMT"].ToString(), this.sqlDtRdrServer["CodFas"].ToString(), this.sqlDtRdrServer["CodPonAcopl"].ToString(), Convert.ToDouble(this.sqlDtRdrServer["TnsLnh_kV"]), Convert.ToDouble(this.sqlDtRdrServer["TnsLnh_kV"]), Convert.ToDouble(this.sqlDtRdrServer["DemMax" + GeoPerdasTools.mthdGetTypeDayMonth(j, k) + "_kW"]), 0, this.sqlDtRdrServer["TipCrvaCarga"].ToString() + "_" + GeoPerdasTools.mthdGetTypeDay(k), 1, this.intAdequarTensaoCargasMT, 1);
                                    }
                                    this.sqlDtRdrServer.Close();
                                    this.mthdInputMensageLog("Passou pelas cargas MT.");

                                    // Loop das cargas BT
                                    this.sqlCommServer = new SqlCommand("SELECT DISTINCT t1.CodBase, t1.CodConsBT, t1.CodFas, t1.CodPonAcopl, t1.TipCrvaCarga, t1.DemMax" + GeoPerdasTools.mthdGetTypeDayMonth(j, k) + "_kW, 0.92 * (ISNULL([M1_PotNom_kVA],0)+ISNULL([M2_PotNom_kVA],0)+ISNULL([M3_PotNom_kVA],0)) AS DemMaxTrafo_kW, t1.CodAlimAtrib, t1.TnsLnh_kV, t2.TnsFasBas_kV FROM dbo.[" + this.strBaseSelected + "AuxCargaBTDem] AS t1 LEFT JOIN dbo.[" + this.strBaseSelected + "AuxTrafoMTMTMTBT] AS t2 ON (t1.CodBase = t2.M1_CodBase AND t1.CodTrafoAtrib = t2.M1_CodTrafo) WHERE t1.CodBase = " + this.strBaseSelected + " AND t1.CodAlimAtrib = '" + strFeeder + "'", this.sqlConnServer);
                                    this.sqlCommServer.CommandTimeout = 0;
                                    this.mthdInsertCommandInLine("! Criação da seção de cargas BT");
                                    this.sqlDtRdrServer = this.sqlCommServer.ExecuteReader();
                                    while (this.sqlDtRdrServer.Read())
                                    {
                                        this.mthdGetDSSCommandNewLoad("BT_" + this.sqlDtRdrServer["CodConsBT"].ToString(), this.sqlDtRdrServer["CodFas"].ToString(), this.sqlDtRdrServer["CodPonAcopl"].ToString(), Convert.ToDouble(this.sqlDtRdrServer["TnsLnh_kV"]), Convert.ToDouble(this.sqlDtRdrServer["TnsFasBas_kV"]), Convert.ToDouble(this.sqlDtRdrServer["DemMax" + GeoPerdasTools.mthdGetTypeDayMonth(j, k) + "_kW"]), Convert.ToDouble(this.sqlDtRdrServer["DemMaxTrafo_kW"]), this.sqlDtRdrServer["TipCrvaCarga"].ToString() + "_" + GeoPerdasTools.mthdGetTypeDay(k), 2, this.intAdequarTensaoCargasBT, 1);
                                    }
                                    this.sqlDtRdrServer.Close();
                                    this.mthdInputMensageLog("Passou pelas cargas BT.");

                                    if (this.intRealizaCnvrgcPNT == 1)
                                    {
                                        // Loop das cargas MT Não Técnicas
                                        this.sqlCommServer = new SqlCommand("SELECT DISTINCT t1.CodBase, t1.CodConsMT, t1.CodFas, t1.CodPonAcopl, t1.TipCrvaCarga, t1.DemMax" + GeoPerdasTools.mthdGetTypeDayMonth(j, k) + "_kW, t1.CodAlimAtrib, t1.TnsLnh_kV FROM dbo.[" + this.strBaseSelected + "AuxCargaMTNTDem] AS t1 WHERE t1.CodBase = " + this.strBaseSelected + " AND t1.CodAlimAtrib = '" + strFeeder + "'", this.sqlConnServer);
                                        this.sqlCommServer.CommandTimeout = 0;
                                        this.mthdInsertCommandInLine("! Criação da seção de cargas MT Não Técnicas");
                                        this.sqlDtRdrServer = this.sqlCommServer.ExecuteReader();
                                        while (this.sqlDtRdrServer.Read())
                                        {
                                            this.mthdGetDSSCommandNewLoad("MT_" + this.sqlDtRdrServer["CodConsMT"].ToString() + "_NT", this.sqlDtRdrServer["CodFas"].ToString(), this.sqlDtRdrServer["CodPonAcopl"].ToString(), Convert.ToDouble(this.sqlDtRdrServer["TnsLnh_kV"]), Convert.ToDouble(this.sqlDtRdrServer["TnsLnh_kV"]), Convert.ToDouble(this.sqlDtRdrServer["DemMax" + GeoPerdasTools.mthdGetTypeDayMonth(j, k) + "_kW"]), 0, this.sqlDtRdrServer["TipCrvaCarga"].ToString() + "_" + GeoPerdasTools.mthdGetTypeDay(k), 1, this.intAdequarTensaoCargasMT, 1);
                                        }
                                        this.sqlDtRdrServer.Close();
                                        this.mthdInputMensageLog("Passou pelas cargas MT Não Técnicas.");

                                        // Loop das cargas BT Não Técnicas
                                        this.sqlCommServer = new SqlCommand("SELECT DISTINCT t1.CodBase, t1.CodConsBT, t1.CodFas, t1.CodPonAcopl, t1.TipCrvaCarga, t1.DemMax" + GeoPerdasTools.mthdGetTypeDayMonth(j, k) + "_kW, 0.92 * (ISNULL([M1_PotNom_kVA],0)+ISNULL([M2_PotNom_kVA],0)+ISNULL([M3_PotNom_kVA],0)) AS DemMaxTrafo_kW, t1.CodAlimAtrib, t1.TnsLnh_kV, t2.TnsFasBas_kV FROM dbo.[" + this.strBaseSelected + "AuxCargaBTNTDem] AS t1 LEFT JOIN dbo.AuxTrafoMTMTMTBT AS t2 ON (t1.CodBase = t2.M1_CodBase AND t1.CodTrafoAtrib = t2.M1_CodTrafo) WHERE t1.CodBase = " + this.strBaseSelected + " AND t1.CodAlimAtrib = '" + strFeeder + "'", this.sqlConnServer);
                                        this.sqlCommServer.CommandTimeout = 0;
                                        this.mthdInsertCommandInLine("! Criação da seção de cargas BT Não Técnicas");
                                        this.sqlDtRdrServer = this.sqlCommServer.ExecuteReader();
                                        while (this.sqlDtRdrServer.Read())
                                        {
                                            this.mthdGetDSSCommandNewLoad("BT_" + this.sqlDtRdrServer["CodConsBT"].ToString() + "_NT", this.sqlDtRdrServer["CodFas"].ToString(), this.sqlDtRdrServer["CodPonAcopl"].ToString(), Convert.ToDouble(this.sqlDtRdrServer["TnsLnh_kV"]), Convert.ToDouble(this.sqlDtRdrServer["TnsFasBas_kV"]), Convert.ToDouble(this.sqlDtRdrServer["DemMax" + GeoPerdasTools.mthdGetTypeDayMonth(j, k) + "_kW"]), Convert.ToDouble(this.sqlDtRdrServer["DemMaxTrafo_kW"]), this.sqlDtRdrServer["TipCrvaCarga"].ToString() + "_" + GeoPerdasTools.mthdGetTypeDay(k), 2, this.intAdequarTensaoCargasBT, 1);
                                        }
                                        this.sqlDtRdrServer.Close();
                                        this.mthdInputMensageLog("Passou pelas cargas BT Não Técnicas.");
                                    }

                                }
                                else
                                {

                                    // Reinício do código
                                    this.mthdStarCommands();

                                    // Loop das cargas MT
                                    this.sqlCommServer = new SqlCommand("SELECT DISTINCT t1.CodBase, t1.CodConsMT, t1.CodFas, t1.CodPonAcopl, t1.TipCrvaCarga, t1.DemMax" + GeoPerdasTools.mthdGetTypeDayMonth(j, k) + "_kW, t1.CodAlimAtrib, t1.TnsLnh_kV FROM dbo.AuxCargaMTDem AS t1 WHERE t1.CodBase = " + this.strBaseSelected + " AND t1.CodAlimAtrib = '" + strFeeder + "'", this.sqlConnServer);
                                    this.sqlCommServer.CommandTimeout = 0;
                                    this.mthdInsertCommandInLine("! Criação da seção de cargas MT");
                                    this.sqlDtRdrServer = this.sqlCommServer.ExecuteReader();
                                    while (this.sqlDtRdrServer.Read())
                                    {
                                        this.mthdGetDSSCommandEditLoad("MT_" + this.sqlDtRdrServer["CodConsMT"].ToString(), this.sqlDtRdrServer["CodFas"].ToString(), Convert.ToDouble(this.sqlDtRdrServer["DemMax" + GeoPerdasTools.mthdGetTypeDayMonth(j, k) + "_kW"]), 0, this.sqlDtRdrServer["TipCrvaCarga"].ToString() + "_" + GeoPerdasTools.mthdGetTypeDay(k), 1, 1);
                                    }
                                    this.sqlDtRdrServer.Close();
                                    this.mthdInputMensageLog("Passou pelas cargas MT.");

                                    // Loop das cargas BT
                                    this.sqlCommServer = new SqlCommand("SELECT DISTINCT t1.CodBase, t1.CodConsBT, t1.CodFas, t1.CodPonAcopl, t1.TipCrvaCarga, t1.DemMax" + GeoPerdasTools.mthdGetTypeDayMonth(j, k) + "_kW, 0.92 * (ISNULL([M1_PotNom_kVA],0)+ISNULL([M2_PotNom_kVA],0)+ISNULL([M3_PotNom_kVA],0)) AS DemMaxTrafo_kW, t1.CodAlimAtrib, t1.TnsLnh_kV FROM dbo.AuxCargaBTDem AS t1 INNER JOIN dbo.AuxTrafoMTMTMTBT AS t2 ON (t1.CodBase = t2.M1_CodBase AND t1.CodTrafoAtrib = t2.M1_CodTrafo) WHERE t1.CodBase = " + this.strBaseSelected + " AND t1.CodAlimAtrib = '" + strFeeder + "'", this.sqlConnServer);
                                    this.sqlCommServer.CommandTimeout = 0;
                                    this.mthdInsertCommandInLine("! Criação da seção de cargas BT");
                                    this.sqlDtRdrServer = this.sqlCommServer.ExecuteReader();
                                    while (this.sqlDtRdrServer.Read())
                                    {
                                        this.mthdGetDSSCommandEditLoad("BT_" + this.sqlDtRdrServer["CodConsBT"].ToString(), this.sqlDtRdrServer["CodFas"].ToString(), Convert.ToDouble(this.sqlDtRdrServer["DemMax" + GeoPerdasTools.mthdGetTypeDayMonth(j, k) + "_kW"]), Convert.ToDouble(this.sqlDtRdrServer["DemMaxTrafo_kW"]), this.sqlDtRdrServer["TipCrvaCarga"].ToString() + "_" + GeoPerdasTools.mthdGetTypeDay(k), 2, 1);
                                    }
                                    this.sqlDtRdrServer.Close();
                                    this.mthdInputMensageLog("Passou pelas cargas BT.");

                                    if (this.intRealizaCnvrgcPNT == 1)
                                    {
                                        // Loop das cargas MT Não Técnicas
                                        this.sqlCommServer = new SqlCommand("SELECT DISTINCT t1.CodBase, t1.CodConsMT, t1.CodFas, t1.CodPonAcopl, t1.TipCrvaCarga, t1.DemMax" + GeoPerdasTools.mthdGetTypeDayMonth(j, k) + "_kW, t1.CodAlimAtrib, t1.TnsLnh_kV FROM dbo.[AuxCargaMTNTDem] AS t1 WHERE t1.CodBase = " + this.strBaseSelected + " AND t1.CodAlimAtrib = '" + strFeeder + "'", this.sqlConnServer);
                                        this.sqlCommServer.CommandTimeout = 0;
                                        this.mthdInsertCommandInLine("! Criação da seção de cargas MT Não Técnicas");
                                        this.sqlDtRdrServer = this.sqlCommServer.ExecuteReader();
                                        while (this.sqlDtRdrServer.Read())
                                        {
                                            this.mthdGetDSSCommandEditLoad("MT_" + this.sqlDtRdrServer["CodConsMT"].ToString() + "_NT", this.sqlDtRdrServer["CodFas"].ToString(), Convert.ToDouble(this.sqlDtRdrServer["DemMax" + GeoPerdasTools.mthdGetTypeDayMonth(j, k) + "_kW"]), 0, this.sqlDtRdrServer["TipCrvaCarga"].ToString() + "_" + GeoPerdasTools.mthdGetTypeDay(k), 1, 1);
                                        }
                                        this.sqlDtRdrServer.Close();
                                        this.mthdInputMensageLog("Passou pelas cargas MT Não Técnicas.");

                                        // Loop das cargas BT Não Técnicas
                                        this.sqlCommServer = new SqlCommand("SELECT DISTINCT t1.CodBase, t1.CodConsBT, t1.CodFas, t1.CodPonAcopl, t1.TipCrvaCarga, t1.DemMax" + GeoPerdasTools.mthdGetTypeDayMonth(j, k) + "_kW, 0.92 * (ISNULL([M1_PotNom_kVA],0)+ISNULL([M2_PotNom_kVA],0)+ISNULL([M3_PotNom_kVA],0)) AS DemMaxTrafo_kW, t1.CodAlimAtrib, t1.TnsLnh_kV FROM dbo.[AuxCargaBTNTDem] AS t1 INNER JOIN dbo.AuxTrafoMTMTMTBT AS t2 ON (t1.CodBase = t2.M1_CodBase AND t1.CodTrafoAtrib = t2.M1_CodTrafo) WHERE t1.CodBase = " + this.strBaseSelected + " AND t1.CodAlimAtrib = '" + strFeeder + "'", this.sqlConnServer);
                                        this.sqlCommServer.CommandTimeout = 0;
                                        this.mthdInsertCommandInLine("! Criação da seção de cargas BT Não Técnicas");
                                        this.sqlDtRdrServer = this.sqlCommServer.ExecuteReader();
                                        while (this.sqlDtRdrServer.Read())
                                        {
                                            this.mthdGetDSSCommandEditLoad("BT_" + this.sqlDtRdrServer["CodConsBT"].ToString() + "_NT", this.sqlDtRdrServer["CodFas"].ToString(), Convert.ToDouble(this.sqlDtRdrServer["DemMax" + GeoPerdasTools.mthdGetTypeDayMonth(j, k) + "_kW"]), Convert.ToDouble(this.sqlDtRdrServer["DemMaxTrafo_kW"]), this.sqlDtRdrServer["TipCrvaCarga"].ToString() + "_" + GeoPerdasTools.mthdGetTypeDay(k), 2, 1);
                                        }
                                        this.sqlDtRdrServer.Close();
                                        this.mthdInputMensageLog("Passou pelas cargas BT Não Técnicas.");
                                    }

                                }

                                // Ajustes de execução da apuração
                                tolerance = 0.00001;
                                do {
                                    tolerance = tolerance * 10;
                                    this.mthdInsertCommandInLine("Set tolerance = " + tolerance.ToString().Replace(",", "."));
                                    this.mthdInsertCommandInLine("Solve");
                                    this.mthdEndCommands();
                                } while (this.intConverged == 0);


                                for (i = 0; i < this.dssCircuit.NumBuses; i++)
                                {
                                    dssBus = this.dssCircuit.Buses[i];
                                    Vseqn = dssBus.SeqVoltages;
                                    cpxVseqn = dssBus.CplxSeqVoltages;
                                    V = dssBus.Voltages;
                                    vnodes = dssBus.Nodes;

                                    strvnodes = "";
                                    for (int z = 0; z < vnodes.Length; z++)
                                    {
                                        strvnodes += "." + vnodes[z];
                                    }

                                    VseqnVetor[0] = 0;
                                    VseqnVetor[1] = 0;
                                    VseqnVetor[2] = 0;
                                    if (dssBus.NumNodes == 3)
                                    {
                                        for (int z = 0; z < Vseqn.Length; z++)
                                        {
                                            VseqnVetor[z] = Vseqn[z];
                                        }
                                    }
                                    cpxVseqnVetor[0] = 0;
                                    cpxVseqnVetor[1] = 0;
                                    cpxVseqnVetor[2] = 0;
                                    cpxVseqnVetor[3] = 0;
                                    cpxVseqnVetor[4] = 0;
                                    cpxVseqnVetor[5] = 0;
                                    if (dssBus.NumNodes == 3)
                                    {
                                        for (int z = 0; z < cpxVseqn.Length; z++)
                                        {
                                            cpxVseqnVetor[z] = cpxVseqn[z];
                                        }
                                    }
                                    VVetor[0] = 0;
                                    VVetor[1] = 0;
                                    VVetor[2] = 0;
                                    VVetor[3] = 0;
                                    VVetor[4] = 0;
                                    VVetor[5] = 0;
                                    for (int z = 0; z < vnodes.Length; z++)
                                    {
                                        if (vnodes[z] == 1)
                                        {
                                            VVetor[0] = V[2 * z + 0];
                                            VVetor[1] = V[2 * z + 1];
                                        }
                                        if (vnodes[z] == 2)
                                        {
                                            VVetor[2] = V[2 * z + 0];
                                            VVetor[3] = V[2 * z + 1];
                                        }
                                        if (vnodes[z] == 3)
                                        {
                                            VVetor[4] = V[2 * z + 0];
                                            VVetor[5] = V[2 * z + 1];
                                        }
                                    }

                                    // Executa a stored procedure de escrita das cargas
                                    this.sqlCommServer = new SqlCommand("dbo.iadAuxReportVoltage", this.sqlConnServer);
                                    this.sqlCommServer.CommandType = CommandType.StoredProcedure;
                                    this.sqlCommServer.Parameters.AddWithValue("@CodBase", this.strBaseSelected);
                                    this.sqlCommServer.Parameters.AddWithValue("@CodAlim", strFeeder);
                                    this.sqlCommServer.Parameters.AddWithValue("@CodOperation", 2);
                                    this.sqlCommServer.Parameters.AddWithValue("@TipoDiaMes", GeoPerdasTools.mthdGetTypeDayMonth(j, k));
                                    this.sqlCommServer.Parameters.AddWithValue("@TipoRodada", this.mthdGetOptionsToString());
                                    this.sqlCommServer.Parameters.AddWithValue("@CodBarra", dssBus.Name);
                                    this.sqlCommServer.Parameters.AddWithValue("@NodesNumber", dssBus.NumNodes);
                                    this.sqlCommServer.Parameters.AddWithValue("@Nodes", strvnodes);
                                    this.sqlCommServer.Parameters.AddWithValue("@VoltageBase_kV", dssBus.kVBase);
                                    this.sqlCommServer.Parameters.AddWithValue("@MOD_V0_V", VseqnVetor[0]);
                                    this.sqlCommServer.Parameters.AddWithValue("@MOD_V1_V", VseqnVetor[1]);
                                    this.sqlCommServer.Parameters.AddWithValue("@MOD_V2_V", VseqnVetor[2]);
                                    this.sqlCommServer.Parameters.AddWithValue("@REAL_V0_kV", cpxVseqnVetor[0]);
                                    this.sqlCommServer.Parameters.AddWithValue("@IMAJ_V0_kV", cpxVseqnVetor[1]);
                                    this.sqlCommServer.Parameters.AddWithValue("@REAL_V1_kV", cpxVseqnVetor[2]);
                                    this.sqlCommServer.Parameters.AddWithValue("@IMAJ_V1_kV", cpxVseqnVetor[3]);
                                    this.sqlCommServer.Parameters.AddWithValue("@REAL_V2_kV", cpxVseqnVetor[4]);
                                    this.sqlCommServer.Parameters.AddWithValue("@IMAJ_V2_kV", cpxVseqnVetor[5]);
                                    this.sqlCommServer.Parameters.AddWithValue("@REAL_Va_V", VVetor[0]);
                                    this.sqlCommServer.Parameters.AddWithValue("@IMAJ_Va_V", VVetor[1]);
                                    this.sqlCommServer.Parameters.AddWithValue("@REAL_Vb_V", VVetor[2]);
                                    this.sqlCommServer.Parameters.AddWithValue("@IMAJ_Vb_V", VVetor[3]);
                                    this.sqlCommServer.Parameters.AddWithValue("@REAL_Vc_V", VVetor[4]);
                                    this.sqlCommServer.Parameters.AddWithValue("@IMAJ_Vc_V", VVetor[5]);
                                    this.sqlCommServer.Parameters.AddWithValue("@MOD_Va_V", Math.Sqrt(VVetor[0] * VVetor[0] + VVetor[1] * VVetor[1]));
                                    this.sqlCommServer.Parameters.AddWithValue("@ANG_Va_V", angleComplex(VVetor[0], VVetor[1]));
                                    this.sqlCommServer.Parameters.AddWithValue("@MOD_Vb_V", Math.Sqrt(VVetor[2] * VVetor[2] + VVetor[3] * VVetor[3]));
                                    this.sqlCommServer.Parameters.AddWithValue("@ANG_Vb_V", angleComplex(VVetor[2], VVetor[3]));
                                    this.sqlCommServer.Parameters.AddWithValue("@MOD_Vc_V", Math.Sqrt(VVetor[4] * VVetor[4] + VVetor[5] * VVetor[5]));
                                    this.sqlCommServer.Parameters.AddWithValue("@ANG_Vc_V", angleComplex(VVetor[4], VVetor[5]));
                                    this.sqlCommServer.ExecuteNonQuery();
                                }


                            }

                        }

                        // Término do processo
                        this.mthdInputMensageLog("Término do processo para o alimentador " + strFeeder + ".");
                        this.rtbLog.Refresh();

                    }

                }
                catch (Exception xcptn)
                {

                    this.mthdInputMensageLog("Problema não identificado (" + xcptn.Message.ToString() + ").");
                    this.sqlDtRdrServer.Dispose();
                    this.sqlCommServer.Dispose();

                }

            }

        }

        private void bReportVoltageSimple_Click(object sender, EventArgs e)
        {

            int i = 0, intCtrlCnvrg = 0;
            String strTempSqlCommand, strTensoesBase;
            int intAno = 0;
            double tolerance;
            double dblEnerInjDU_kWh = 0, dblEnerInjSA_kWh = 0, dblEnerInjDO_kWh = 0;
            double dblPerdaEnerTecDU_kWh = 0, dblPerdaEnerTecSA_kWh = 0, dblPerdaEnerTecDO_kWh = 0;
            double dblEnerForncDU_kWh = 0, dblEnerForncSA_kWh = 0, dblEnerForncDO_kWh = 0;
            double dblDelta = 0, dblDeltaMT = 0, dblDeltaBT = 0, dblDelta_MWh = 0, dblDeltaBT_MWh = 0, dblDeltaMT_MWh = 0;
            Bus dssBus;
            dynamic Vseqn, cpxVseqn, V;
            dynamic vnodes;
            String strvnodes;
            double[] VseqnVetor = { 0, 0, 0 };
            double[] cpxVseqnVetor = { 0, 0, 0, 0, 0, 0 };
            double[] VVetor = { 0, 0, 0, 0, 0, 0 };

            if (checkBase())
            {

                try
                {

                    // Verifica as opções solicitadas pelo usuário
                    this.mthdUpdateOptionsRun();

                    // Captura a base a ser calculada
                    this.strBaseSelected = this.tbObjectBase.Text;

                    // Loop da base e dos alimentadores
                    this.strFeedersSelected.Clear();
                    if (this.tbObjectFeeder.Text == null || this.tbObjectFeeder.Text == "")
                        this.sqlCommServer = new SqlCommand("SELECT t1.* FROM dbo.CircMT AS t1 WHERE t1.CodBase = " + this.strBaseSelected + " AND t1.CircAtipResultante = 0", this.sqlConnServer);
                    else
                        this.sqlCommServer = new SqlCommand("SELECT t1.* FROM dbo.CircMT AS t1 WHERE t1.CodBase = " + this.strBaseSelected + " AND " + GeoPerdasTools.mthdQuerySQLFeeders(this.tbObjectFeeder.Text) + " AND t1.CircAtipResultante = 0", this.sqlConnServer);
                    this.sqlCommServer.CommandTimeout = 0;
                    this.sqlDtRdrServer = this.sqlCommServer.ExecuteReader();
                    while (this.sqlDtRdrServer.Read())
                    {
                        this.strFeedersSelected.Add(this.sqlDtRdrServer["CodAlim"].ToString());
                    }
                    this.sqlDtRdrServer.Close();

                    foreach (String strFeeder in strFeedersSelected)
                    {

                        // Executa a stored procedure da limpeza das tensões de barras
                        this.sqlCommServer = new SqlCommand("dbo.iadAuxReportVoltageSimple", this.sqlConnServer);
                        this.sqlCommServer.CommandType = CommandType.StoredProcedure;
                        this.sqlCommServer.Parameters.AddWithValue("@CodBase", this.strBaseSelected);
                        this.sqlCommServer.Parameters.AddWithValue("@CodAlim", strFeeder);
                        this.sqlCommServer.Parameters.AddWithValue("@CodOperation", 1);
                        this.sqlCommServer.Parameters.AddWithValue("@TipoRodada", this.mthdGetOptionsToString());
                        this.sqlCommServer.Parameters.AddWithValue("@CodBarra", "");
                        this.sqlCommServer.Parameters.AddWithValue("@NodesNumber", 0);
                        this.sqlCommServer.Parameters.AddWithValue("@Nodes", "");
                        this.sqlCommServer.Parameters.AddWithValue("@VoltageBase_kV", 0);
                        this.sqlCommServer.Parameters.AddWithValue("@MOD_V0_V", 0);
                        this.sqlCommServer.Parameters.AddWithValue("@MOD_V1_V", 0);
                        this.sqlCommServer.Parameters.AddWithValue("@MOD_V2_V", 0);
                        this.sqlCommServer.Parameters.AddWithValue("@REAL_V0_kV", 0);
                        this.sqlCommServer.Parameters.AddWithValue("@IMAJ_V0_kV", 0);
                        this.sqlCommServer.Parameters.AddWithValue("@REAL_V1_kV", 0);
                        this.sqlCommServer.Parameters.AddWithValue("@IMAJ_V1_kV", 0);
                        this.sqlCommServer.Parameters.AddWithValue("@REAL_V2_kV", 0);
                        this.sqlCommServer.Parameters.AddWithValue("@IMAJ_V2_kV", 0);
                        this.sqlCommServer.Parameters.AddWithValue("@REAL_Va_V", 0);
                        this.sqlCommServer.Parameters.AddWithValue("@IMAJ_Va_V", 0);
                        this.sqlCommServer.Parameters.AddWithValue("@REAL_Vb_V", 0);
                        this.sqlCommServer.Parameters.AddWithValue("@IMAJ_Vb_V", 0);
                        this.sqlCommServer.Parameters.AddWithValue("@REAL_Vc_V", 0);
                        this.sqlCommServer.Parameters.AddWithValue("@IMAJ_Vc_V", 0);
                        this.sqlCommServer.Parameters.AddWithValue("@MOD_Va_V", 0);
                        this.sqlCommServer.Parameters.AddWithValue("@ANG_Va_V", 0);
                        this.sqlCommServer.Parameters.AddWithValue("@MOD_Vb_V", 0);
                        this.sqlCommServer.Parameters.AddWithValue("@ANG_Vb_V", 0);
                        this.sqlCommServer.Parameters.AddWithValue("@MOD_Vc_V", 0);
                        this.sqlCommServer.Parameters.AddWithValue("@ANG_Vc_V", 0);
                        this.sqlCommServer.ExecuteNonQuery();

                        // Início do processo
                        this.mthdInputMensageLog("Início do processo para o alimentador " + strFeeder + ".");

                        this.mthdStarCommands();

                        // Início do código
                        this.mthdInsertCommandInLine("Clear");

                        // Loop dos códigos dos circuitos
                        this.sqlCommServer = new SqlCommand("SELECT t1.* FROM dbo.CircMT AS t1 WHERE t1.CodBase = " + this.strBaseSelected + " AND t1.CodAlim = '" + strFeeder + "'", this.sqlConnServer);
                        this.sqlCommServer.CommandTimeout = 0;
                        this.mthdInsertCommandInLine("! Criação da seção de circuitos");
                        this.sqlDtRdrServer = this.sqlCommServer.ExecuteReader();
                        while (this.sqlDtRdrServer.Read())
                        {
                            this.mthdGetDSSCommandCircuit(strFeeder, Convert.ToDouble(this.sqlDtRdrServer["TenNom_kV"]), Convert.ToDouble(this.sqlDtRdrServer["TenOpe_pu"]), this.sqlDtRdrServer["CodPonAcopl"].ToString(), this.intAdequarTensaoSuperior);
                        }
                        this.sqlDtRdrServer.Close();
                        this.mthdInputMensageLog("Passou pelo circuito.");

                        // Loop dos códigos dos condutores
                        this.sqlCommServer = new SqlCommand("SELECT t1.* FROM dbo.CodCondutor AS t1 WHERE t1.CodBase = " + this.strBaseSelected, this.sqlConnServer);
                        this.sqlCommServer.CommandTimeout = 0;
                        this.mthdInsertCommandInLine("! Criação da seção de condutores");
                        this.sqlDtRdrServer = this.sqlCommServer.ExecuteReader();
                        while (this.sqlDtRdrServer.Read())
                        {
                            this.mthdGetDSSCommandLinecode(this.sqlDtRdrServer["CodCond"].ToString(), Convert.ToDouble(this.sqlDtRdrServer["Resis_ohms_km"]), Convert.ToDouble(this.sqlDtRdrServer["Reat_ohms_km"]), Convert.ToDouble(this.sqlDtRdrServer["CorrMax_A"]));
                        }
                        this.sqlDtRdrServer.Close();
                        this.mthdInputMensageLog("Passou pelos condutores.");

                        // Loop das chaves MT
                        this.sqlCommServer = new SqlCommand("SELECT t1.CodChvMT, t1.CodFas, t1.De, t1.Para FROM dbo.ChaveMT AS t1 WHERE t1.CodBase = " + this.strBaseSelected + " AND t1.CodAlimAtrib = '" + strFeeder + "' AND t1.EstChv = 2", this.sqlConnServer);
                        this.sqlCommServer.CommandTimeout = 0;
                        this.mthdInsertCommandInLine("! Criação da seção de chaves MT");
                        this.sqlDtRdrServer = this.sqlCommServer.ExecuteReader();
                        while (this.sqlDtRdrServer.Read())
                        {
                            this.mthdGetDSSCommandMTSwitch("CMT_" + this.sqlDtRdrServer["CodChvMT"].ToString(), this.sqlDtRdrServer["CodFas"].ToString(), this.sqlDtRdrServer["De"].ToString(), this.sqlDtRdrServer["Para"].ToString());
                        }
                        this.sqlDtRdrServer.Close();
                        this.mthdInputMensageLog("Passou pelas chaves MT.");

                        // Loop dos segmentos MT
                        this.sqlCommServer = new SqlCommand("SELECT t1.CodSegmMT, t1.CodFas, t1.CodCond, t1.Comp_km, t1.De, t1.Para, t1.Propr FROM dbo.SegmentoMT AS t1 WHERE t1.CodBase = " + this.strBaseSelected + " AND t1.CodAlimAtrib = '" + strFeeder + "'", this.sqlConnServer);
                        this.sqlCommServer.CommandTimeout = 0;
                        this.mthdInsertCommandInLine("! Criação da seção de segmentos MT");
                        this.sqlDtRdrServer = this.sqlCommServer.ExecuteReader();
                        while (this.sqlDtRdrServer.Read())
                        {
                            this.mthdGetDSSCommandLineMT("SMT_" + this.sqlDtRdrServer["CodSegmMT"].ToString(), this.sqlDtRdrServer["CodFas"].ToString(), this.sqlDtRdrServer["De"].ToString(), this.sqlDtRdrServer["Para"].ToString(), this.sqlDtRdrServer["CodCond"].ToString(), Convert.ToDouble(this.sqlDtRdrServer["Comp_km"]), this.sqlDtRdrServer["Propr"].ToString(), 0, this.intNeutralizarRedeTerceiros);
                        }
                        this.sqlDtRdrServer.Close();
                        this.mthdInputMensageLog("Passou pelos segmentos MT.");

                        // Loop dos reguladores MT
                        this.sqlCommServer = new SqlCommand("SELECT t1.CodRegulMT, t1.CodBnc, t1.TipRegul, t1.CodFasPrim, t1.CodFasSecu, t1.PotNom_kVA, t1.TenRgl_pu, t1.[ReatHL_%], t1.PerdTtl_W, t1.PerdVz_W, t1.TnsLnh1_kV, t1.De, t1.Para, t1.CodFasCoinc FROM dbo.ReguladorMT AS t1 WHERE t1.CodBase = " + this.strBaseSelected + " AND t1.CodAlimAtrib = '" + strFeeder + "'", this.sqlConnServer);
                        this.sqlCommServer.CommandTimeout = 0;
                        this.mthdInsertCommandInLine("! Criação da seção de reguladores MT");
                        this.sqlDtRdrServer = this.sqlCommServer.ExecuteReader();
                        while (this.sqlDtRdrServer.Read())
                        {
                            this.mthdGetDSSCommandRegulatorMT("REG_" + this.sqlDtRdrServer["CodRegulMT"].ToString(), Convert.ToInt32(this.sqlDtRdrServer["CodBnc"]), Convert.ToInt32(this.sqlDtRdrServer["TipRegul"]), this.sqlDtRdrServer["CodFasPrim"].ToString(), this.sqlDtRdrServer["CodFasSecu"].ToString(), this.sqlDtRdrServer["CodFasCoinc"].ToString(), this.sqlDtRdrServer["De"].ToString(), this.sqlDtRdrServer["Para"].ToString(), Convert.ToDouble(this.sqlDtRdrServer["TnsLnh1_kV"]), Convert.ToDouble(this.sqlDtRdrServer["PotNom_kVA"]), Convert.ToDouble(this.sqlDtRdrServer["TenRgl_pu"]), Convert.ToDouble(this.sqlDtRdrServer["ReatHL_%"]), (100 * Convert.ToDouble(this.sqlDtRdrServer["PerdTtl_W"])) / (1000 * Convert.ToDouble(this.sqlDtRdrServer["PotNom_kVA"])), (100 * Convert.ToDouble(this.sqlDtRdrServer["PerdVz_W"])) / (1000 * Convert.ToDouble(this.sqlDtRdrServer["PotNom_kVA"])));
                        }
                        this.sqlDtRdrServer.Close();
                        this.mthdInputMensageLog("Passou pelos reguladores MT.");

                        // Loop dos transformadores MT-MT ou MT-BT
                        this.sqlCommServer = new SqlCommand("SELECT t1.* FROM dbo.TrafoMTMTMTBT AS t1 WHERE t1.CodBase = " + this.strBaseSelected + " AND t1.CodAlimAtrib = '" + strFeeder + "'", this.sqlConnServer);
                        this.sqlCommServer.CommandTimeout = 0;
                        this.mthdInsertCommandInLine("! Criação da seção de transformadores MT-MT ou MT-BT");
                        this.sqlDtRdrServer = this.sqlCommServer.ExecuteReader();
                        while (this.sqlDtRdrServer.Read())
                        {
                            this.mthdGetDSSCommandTransformer4Wires("TRF_" + this.sqlDtRdrServer["CodTrafo"].ToString(), Convert.ToInt32(this.sqlDtRdrServer["CodBnc"]), Convert.ToInt32(this.sqlDtRdrServer["MRT"]), Convert.ToInt32(this.sqlDtRdrServer["TipTrafo"]), this.sqlDtRdrServer["CodFasPrim"].ToString(), this.sqlDtRdrServer["CodFasSecu"].ToString(), this.sqlDtRdrServer["CodFasTerc"].ToString(), this.sqlDtRdrServer["De"].ToString(), this.sqlDtRdrServer["Para"].ToString(), Convert.ToDouble(this.sqlDtRdrServer["TnsLnh1_kV"]), Convert.ToDouble(this.sqlDtRdrServer["TenSecu_kV"]), Convert.ToDouble(this.sqlDtRdrServer["Tap_pu"]), Convert.ToDouble(this.sqlDtRdrServer["PotNom_kVA"]), (GeoPerdasTools.mthdTransformerTotalPowerLoss2019_per(this.sqlDtRdrServer["CodFasPrim"].ToString(), Convert.ToDouble(this.sqlDtRdrServer["PotNom_kVA"]), Convert.ToDouble(this.sqlDtRdrServer["TnsLnh1_kV"])) != 0 && this.intUsaTrafoABNT == 1) ? GeoPerdasTools.mthdTransformerTotalPowerLoss2019_per(this.sqlDtRdrServer["CodFasPrim"].ToString(), Convert.ToDouble(this.sqlDtRdrServer["PotNom_kVA"]), Convert.ToDouble(this.sqlDtRdrServer["TnsLnh1_kV"])) : Convert.ToDouble(this.sqlDtRdrServer["PerdTtl_%"]), (GeoPerdasTools.mthdTransformerNoLoadPowerLoss2019_per(this.sqlDtRdrServer["CodFasPrim"].ToString(), Convert.ToDouble(this.sqlDtRdrServer["PotNom_kVA"]), Convert.ToDouble(this.sqlDtRdrServer["TnsLnh1_kV"])) != 0 && this.intUsaTrafoABNT == 1) ? GeoPerdasTools.mthdTransformerNoLoadPowerLoss2019_per(this.sqlDtRdrServer["CodFasPrim"].ToString(), Convert.ToDouble(this.sqlDtRdrServer["PotNom_kVA"]), Convert.ToDouble(this.sqlDtRdrServer["TnsLnh1_kV"])) : Convert.ToDouble(this.sqlDtRdrServer["PerdVz_%"]), this.sqlDtRdrServer["Propr"].ToString(), (this.intAdequarTrafoVazio == 1) ? Convert.ToInt32(this.sqlDtRdrServer["SemCarga"]) : 0);
                        }
                        this.sqlDtRdrServer.Close();
                        this.mthdInputMensageLog("Passou pelos transformadores MT-MT ou MT-BT.");

                        // Loop das chaves BT
                        this.sqlCommServer = new SqlCommand("SELECT t1.CodChvBT, t1.CodFas, t1.De, t1.Para FROM dbo.ChaveBT AS t1 WHERE t1.CodBase = " + this.strBaseSelected + " AND t1.CodAlimAtrib = '" + strFeeder + "' AND t1.EstChv = 2", this.sqlConnServer);
                        this.sqlCommServer.CommandTimeout = 0;
                        this.mthdInsertCommandInLine("! Criação da seção de chaves BT");
                        this.sqlDtRdrServer = this.sqlCommServer.ExecuteReader();
                        while (this.sqlDtRdrServer.Read())
                        {
                            this.mthdGetDSSCommandMTSwitch("CBT_" + this.sqlDtRdrServer["CodChvBT"].ToString(), this.sqlDtRdrServer["CodFas"].ToString(), this.sqlDtRdrServer["De"].ToString(), this.sqlDtRdrServer["Para"].ToString());
                        }
                        this.sqlDtRdrServer.Close();
                        this.mthdInputMensageLog("Passou pelas chaves BT.");

                        // Loop dos segmentos BT
                        this.sqlCommServer = new SqlCommand("SELECT t1.*, [dbo].[EscolhaPropriedade]([t2].[M1_Propr], [t2].[M2_Propr], [t2].[M3_Propr]) AS [Propr] FROM dbo.[" + this.strBaseSelected + "SegmentoBT] AS t1 LEFT JOIN [dbo].[" + this.strBaseSelected + "AuxTrafoMTMTMTBT] AS t2 ON ([t1].[CodBase] = [t2].[M1_CodBase] AND [t1].[CodTrafoAtrib] = [t2].[M1_CodTrafo]) WHERE t1.CodBase = " + this.strBaseSelected + " AND t1.CodAlimAtrib = '" + strFeeder + "'", this.sqlConnServer);
                        this.sqlCommServer.CommandTimeout = 0;
                        this.mthdInsertCommandInLine("! Criação da seção de segmentos BT");
                        this.sqlDtRdrServer = this.sqlCommServer.ExecuteReader();
                        while (this.sqlDtRdrServer.Read())
                        {
                            this.mthdGetDSSCommandLineBT("SBT_" + this.sqlDtRdrServer["CodSegmBT"].ToString(), this.sqlDtRdrServer["CodFas"].ToString(), this.sqlDtRdrServer["CodPonAcopl1"].ToString(), this.sqlDtRdrServer["CodPonAcopl2"].ToString(), this.sqlDtRdrServer["CodCond"].ToString(), Convert.ToDouble(this.sqlDtRdrServer["Comp_km"]), this.sqlDtRdrServer["Propr"].ToString(), 0, this.intNeutralizarRedeTerceiros);
                        }
                        this.sqlDtRdrServer.Close();
                        this.mthdInputMensageLog("Passou pelos segmentos BT.");

                        // Loop dos ramais
                        this.sqlCommServer = new SqlCommand("SELECT t1.*, [dbo].[EscolhaPropriedade]([t2].[M1_Propr], [t2].[M2_Propr], [t2].[M3_Propr]) AS [Propr] FROM dbo.[" + this.strBaseSelected + "RamalBT] AS t1 LEFT JOIN [dbo].[" + this.strBaseSelected + "AuxTrafoMTMTMTBT] AS t2 ON ([t1].[CodBase] = [t2].[M1_CodBase] AND [t1].[CodTrafoAtrib] = [t2].[M1_CodTrafo]) WHERE t1.CodBase = " + this.strBaseSelected + " AND t1.CodAlimAtrib = '" + strFeeder + "'", this.sqlConnServer);
                        this.sqlCommServer.CommandTimeout = 0;
                        this.mthdInsertCommandInLine("! Criação da seção de ramais");
                        this.sqlDtRdrServer = this.sqlCommServer.ExecuteReader();
                        while (this.sqlDtRdrServer.Read())
                        {
                            this.mthdGetDSSCommandLineBT("RBT_" + this.sqlDtRdrServer["CodRmlBT"].ToString(), this.sqlDtRdrServer["CodFas"].ToString(), this.sqlDtRdrServer["CodPonAcopl1"].ToString(), this.sqlDtRdrServer["CodPonAcopl2"].ToString(), this.sqlDtRdrServer["CodCond"].ToString(), Convert.ToDouble(this.sqlDtRdrServer["Comp_km"]), this.sqlDtRdrServer["Propr"].ToString(), this.intAdequarRamal, this.intNeutralizarRedeTerceiros);
                        }
                        this.sqlDtRdrServer.Close();
                        this.mthdInputMensageLog("Passou pelos ramais.");

                        // Versão usando apenas o medidor de barramento

                        // Loop dos energymeters de barramento
                        i = 0;
                        this.sqlCommServer = new SqlCommand("SELECT t1.* FROM dbo.AuxMeterBarra AS t1 WHERE t1.CodBase = " + this.strBaseSelected + " AND t1.CodAlim = '" + strFeeder + "'", this.sqlConnServer);
                        this.sqlCommServer.CommandTimeout = 0;
                        this.mthdInsertCommandInLine("! Criação da seção de energymeters de barramento");
                        this.sqlDtRdrServer = this.sqlCommServer.ExecuteReader();
                        while (this.sqlDtRdrServer.Read())
                        {
                            this.mthdGetDSSCommandEnergymeter(this.sqlDtRdrServer["Nome"].ToString().TrimEnd(), this.mthdNomeElemento(this.sqlDtRdrServer["Elem"].ToString().TrimEnd()) + this.sqlDtRdrServer["CodNomeElem"].ToString().TrimEnd());
                            i++;
                        }
                        this.sqlDtRdrServer.Close();
                        this.mthdInputMensageLog("Passou pelos energymeters de barramento.");

                        // Versão usando a tabela auxiliar de medidores

                        // Loop dos energymeters usando AuxMeter
                        /*
                        this.sqlCommServer = new SqlCommand("SELECT t1.* FROM dbo.AuxMeter AS t1 WHERE t1.CodBase = " + this.strBaseSelected + " AND t1.CodAlim = '" + strFeeder + "'", this.sqlConnServer);
                        this.sqlCommServer.CommandTimeout = 0;
                        this.mthdInsertCommandInLine("! Criação da seção de energymeters");
                        this.sqlDtRdrServer = this.sqlCommServer.ExecuteReader();
                        while (this.sqlDtRdrServer.Read())
                        {
                            this.mthdGetDSSCommandEnergymeter(this.sqlDtRdrServer["Nome"].ToString().TrimEnd(), this.mthdNomeElemento(this.sqlDtRdrServer["Elem"].ToString().TrimEnd()) + this.sqlDtRdrServer["CodNomeElem"].ToString().TrimEnd());
                        }
                        this.sqlDtRdrServer.Close();
                        this.mthdInputMensageLog("Passou pelos energymeters.");
                        */

                        // Loop das tensões de base
                        strTensoesBase = "Set voltagebases=[";
                        i = 0;
                        this.sqlCommServer = new SqlCommand("SELECT t1.* FROM dbo.AuxVoltageBases AS t1 WHERE t1.CodBase = " + this.strBaseSelected + " AND t1.CodAlim = '" + strFeeder + "'", this.sqlConnServer);
                        this.sqlCommServer.CommandTimeout = 0;
                        this.mthdInsertCommandInLine("! Criação da seção de tensões de base");
                        this.sqlDtRdrServer = this.sqlCommServer.ExecuteReader();
                        while (this.sqlDtRdrServer.Read())
                        {
                            if (i == 0)
                                strTensoesBase += this.sqlDtRdrServer["TnsLnh_kV"].ToString().Replace(",", ".");
                            else
                                strTensoesBase += " " + this.sqlDtRdrServer["TnsLnh_kV"].ToString().Replace(",", ".");
                            i++;
                        }
                        this.sqlDtRdrServer.Close();
                        strTensoesBase += "]";
                        this.mthdInsertCommandInLine(strTensoesBase);
                        this.mthdInsertCommandInLine("Calcvoltagebases");
                        this.mthdInputMensageLog("Passou pelas tensões de base.");

                        // Loop das cargas MT
                        this.sqlCommServer = new SqlCommand("SELECT DISTINCT t1.CodBase, t1.CodConsMT, t1.CodFas, t1.CodPonAcopl, t1.TipCrvaCarga, [dbo].[Max36Patamares]([t1].[DemMaxDU01_kW],[t1].[DemMaxDU02_kW],[t1].[DemMaxDU03_kW],[t1].[DemMaxDU04_kW],[t1].[DemMaxDU05_kW],[t1].[DemMaxDU06_kW],[t1].[DemMaxDU07_kW],[t1].[DemMaxDU08_kW],[t1].[DemMaxDU09_kW],[t1].[DemMaxDU10_kW],[t1].[DemMaxDU11_kW],[t1].[DemMaxDU12_kW],[t1].[DemMaxSA01_kW],[t1].[DemMaxSA02_kW],[t1].[DemMaxSA03_kW],[t1].[DemMaxSA04_kW],[t1].[DemMaxSA05_kW],[t1].[DemMaxSA06_kW],[t1].[DemMaxSA07_kW],[t1].[DemMaxSA08_kW],[t1].[DemMaxSA09_kW],[t1].[DemMaxSA10_kW],[t1].[DemMaxSA11_kW],[t1].[DemMaxSA12_kW],[t1].[DemMaxDO01_kW],[t1].[DemMaxDO02_kW],[t1].[DemMaxDO03_kW],[t1].[DemMaxDO04_kW],[t1].[DemMaxDO05_kW],[t1].[DemMaxDO06_kW],[t1].[DemMaxDO07_kW],[t1].[DemMaxDO08_kW],[t1].[DemMaxDO09_kW],[t1].[DemMaxDO10_kW],[t1].[DemMaxDO11_kW],[t1].[DemMaxDO12_kW]) AS [DemMax_kW], t1.CodAlimAtrib, t1.TnsLnh_kV FROM dbo.AuxCargaMTDem AS t1 WHERE t1.CodBase = " + this.strBaseSelected + " AND t1.CodAlimAtrib = '" + strFeeder + "'", this.sqlConnServer);
                        this.sqlCommServer.CommandTimeout = 0;
                        this.mthdInsertCommandInLine("! Criação da seção de cargas MT");
                        this.sqlDtRdrServer = this.sqlCommServer.ExecuteReader();
                        while (this.sqlDtRdrServer.Read())
                        {
                            this.mthdGetDSSCommandNewLoad("MT_" + this.sqlDtRdrServer["CodConsMT"].ToString(), this.sqlDtRdrServer["CodFas"].ToString(), this.sqlDtRdrServer["CodPonAcopl"].ToString(), Convert.ToDouble(this.sqlDtRdrServer["TnsLnh_kV"]), Convert.ToDouble(this.sqlDtRdrServer["TnsLnh_kV"]), Convert.ToDouble(this.sqlDtRdrServer["DemMax_kW"]), 0, null, 1, this.intAdequarTensaoCargasMT, 1);
                        }
                        this.sqlDtRdrServer.Close();
                        this.mthdInputMensageLog("Passou pelas cargas MT.");

                        // Loop das cargas BT
                        this.sqlCommServer = new SqlCommand("SELECT DISTINCT t1.CodBase, t1.CodConsBT, t1.CodFas, t1.CodPonAcopl, t1.TipCrvaCarga, [dbo].[Max36Patamares]([t1].[DemMaxDU01_kW],[t1].[DemMaxDU02_kW],[t1].[DemMaxDU03_kW],[t1].[DemMaxDU04_kW],[t1].[DemMaxDU05_kW],[t1].[DemMaxDU06_kW],[t1].[DemMaxDU07_kW],[t1].[DemMaxDU08_kW],[t1].[DemMaxDU09_kW],[t1].[DemMaxDU10_kW],[t1].[DemMaxDU11_kW],[t1].[DemMaxDU12_kW],[t1].[DemMaxSA01_kW],[t1].[DemMaxSA02_kW],[t1].[DemMaxSA03_kW],[t1].[DemMaxSA04_kW],[t1].[DemMaxSA05_kW],[t1].[DemMaxSA06_kW],[t1].[DemMaxSA07_kW],[t1].[DemMaxSA08_kW],[t1].[DemMaxSA09_kW],[t1].[DemMaxSA10_kW],[t1].[DemMaxSA11_kW],[t1].[DemMaxSA12_kW],[t1].[DemMaxDO01_kW],[t1].[DemMaxDO02_kW],[t1].[DemMaxDO03_kW],[t1].[DemMaxDO04_kW],[t1].[DemMaxDO05_kW],[t1].[DemMaxDO06_kW],[t1].[DemMaxDO07_kW],[t1].[DemMaxDO08_kW],[t1].[DemMaxDO09_kW],[t1].[DemMaxDO10_kW],[t1].[DemMaxDO11_kW],[t1].[DemMaxDO12_kW]) AS [DemMax_kW], 0.92 * (ISNULL([M1_PotNom_kVA],0)+ISNULL([M2_PotNom_kVA],0)+ISNULL([M3_PotNom_kVA],0)) AS DemMaxTrafo_kW, t1.CodAlimAtrib, t1.TnsLnh_kV, t2.TnsFasBas_kV FROM dbo.AuxCargaBTDem AS t1 LEFT JOIN dbo.AuxTrafoMTMTMTBT AS t2 ON (t1.CodBase = t2.M1_CodBase AND t1.CodTrafoAtrib = t2.M1_CodTrafo) WHERE t1.CodBase = " + this.strBaseSelected + " AND t1.CodAlimAtrib = '" + strFeeder + "'", this.sqlConnServer);
                        this.sqlCommServer.CommandTimeout = 0;
                        this.mthdInsertCommandInLine("! Criação da seção de cargas BT");
                        this.sqlDtRdrServer = this.sqlCommServer.ExecuteReader();
                        while (this.sqlDtRdrServer.Read())
                        {
                            this.mthdGetDSSCommandNewLoad("BT_" + this.sqlDtRdrServer["CodConsBT"].ToString(), this.sqlDtRdrServer["CodFas"].ToString(), this.sqlDtRdrServer["CodPonAcopl"].ToString(), Convert.ToDouble(this.sqlDtRdrServer["TnsLnh_kV"]), Convert.ToDouble(this.sqlDtRdrServer["TnsFasBas_kV"]), Convert.ToDouble(this.sqlDtRdrServer["DemMax_kW"]), Convert.ToDouble(this.sqlDtRdrServer["DemMaxTrafo_kW"]), null, 2, this.intAdequarTensaoCargasBT, 1);
                        }
                        this.sqlDtRdrServer.Close();
                        this.mthdInputMensageLog("Passou pelas cargas BT.");

                        if (this.intRealizaCnvrgcPNT == 1)
                        {
                            // Loop das cargas MT Não Técnicas
                            this.sqlCommServer = new SqlCommand("SELECT DISTINCT t1.CodBase, t1.CodConsMT, t1.CodFas, t1.CodPonAcopl, t1.TipCrvaCarga, [dbo].[Max36Patamares]([t1].[DemMaxDU01_kW],[t1].[DemMaxDU02_kW],[t1].[DemMaxDU03_kW],[t1].[DemMaxDU04_kW],[t1].[DemMaxDU05_kW],[t1].[DemMaxDU06_kW],[t1].[DemMaxDU07_kW],[t1].[DemMaxDU08_kW],[t1].[DemMaxDU09_kW],[t1].[DemMaxDU10_kW],[t1].[DemMaxDU11_kW],[t1].[DemMaxDU12_kW],[t1].[DemMaxSA01_kW],[t1].[DemMaxSA02_kW],[t1].[DemMaxSA03_kW],[t1].[DemMaxSA04_kW],[t1].[DemMaxSA05_kW],[t1].[DemMaxSA06_kW],[t1].[DemMaxSA07_kW],[t1].[DemMaxSA08_kW],[t1].[DemMaxSA09_kW],[t1].[DemMaxSA10_kW],[t1].[DemMaxSA11_kW],[t1].[DemMaxSA12_kW],[t1].[DemMaxDO01_kW],[t1].[DemMaxDO02_kW],[t1].[DemMaxDO03_kW],[t1].[DemMaxDO04_kW],[t1].[DemMaxDO05_kW],[t1].[DemMaxDO06_kW],[t1].[DemMaxDO07_kW],[t1].[DemMaxDO08_kW],[t1].[DemMaxDO09_kW],[t1].[DemMaxDO10_kW],[t1].[DemMaxDO11_kW],[t1].[DemMaxDO12_kW]) AS [DemMax_kW], t1.CodAlimAtrib, t1.TnsLnh_kV FROM dbo.[AuxCargaMTNTDem] AS t1 WHERE t1.CodBase = " + this.strBaseSelected + " AND t1.CodAlimAtrib = '" + strFeeder + "'", this.sqlConnServer);
                            this.sqlCommServer.CommandTimeout = 0;
                            this.mthdInsertCommandInLine("! Criação da seção de cargas MT Não Técnicas");
                            this.sqlDtRdrServer = this.sqlCommServer.ExecuteReader();
                            while (this.sqlDtRdrServer.Read())
                            {
                                this.mthdGetDSSCommandNewLoad("MT_" + this.sqlDtRdrServer["CodConsMT"].ToString() + "_NT", this.sqlDtRdrServer["CodFas"].ToString(), this.sqlDtRdrServer["CodPonAcopl"].ToString(), Convert.ToDouble(this.sqlDtRdrServer["TnsLnh_kV"]), Convert.ToDouble(this.sqlDtRdrServer["TnsLnh_kV"]), Convert.ToDouble(this.sqlDtRdrServer["DemMax_kW"]), 0, null, 1, this.intAdequarTensaoCargasMT, 1);
                            }
                            this.sqlDtRdrServer.Close();
                            this.mthdInputMensageLog("Passou pelas cargas MT Não Técnicas.");

                            // Loop das cargas BT Não Técnicas
                            this.sqlCommServer = new SqlCommand("SELECT DISTINCT t1.CodBase, t1.CodConsBT, t1.CodFas, t1.CodPonAcopl, t1.TipCrvaCarga, [dbo].[Max36Patamares]([t1].[DemMaxDU01_kW],[t1].[DemMaxDU02_kW],[t1].[DemMaxDU03_kW],[t1].[DemMaxDU04_kW],[t1].[DemMaxDU05_kW],[t1].[DemMaxDU06_kW],[t1].[DemMaxDU07_kW],[t1].[DemMaxDU08_kW],[t1].[DemMaxDU09_kW],[t1].[DemMaxDU10_kW],[t1].[DemMaxDU11_kW],[t1].[DemMaxDU12_kW],[t1].[DemMaxSA01_kW],[t1].[DemMaxSA02_kW],[t1].[DemMaxSA03_kW],[t1].[DemMaxSA04_kW],[t1].[DemMaxSA05_kW],[t1].[DemMaxSA06_kW],[t1].[DemMaxSA07_kW],[t1].[DemMaxSA08_kW],[t1].[DemMaxSA09_kW],[t1].[DemMaxSA10_kW],[t1].[DemMaxSA11_kW],[t1].[DemMaxSA12_kW],[t1].[DemMaxDO01_kW],[t1].[DemMaxDO02_kW],[t1].[DemMaxDO03_kW],[t1].[DemMaxDO04_kW],[t1].[DemMaxDO05_kW],[t1].[DemMaxDO06_kW],[t1].[DemMaxDO07_kW],[t1].[DemMaxDO08_kW],[t1].[DemMaxDO09_kW],[t1].[DemMaxDO10_kW],[t1].[DemMaxDO11_kW],[t1].[DemMaxDO12_kW]) AS [DemMax_kW], 0.92 * (ISNULL([M1_PotNom_kVA],0)+ISNULL([M2_PotNom_kVA],0)+ISNULL([M3_PotNom_kVA],0)) AS DemMaxTrafo_kW, t1.CodAlimAtrib, t1.TnsLnh_kV, t2.TnsFasBas_kV FROM dbo.[AuxCargaBTNTDem] AS t1 LEFT JOIN dbo.AuxTrafoMTMTMTBT AS t2 ON (t1.CodBase = t2.M1_CodBase AND t1.CodTrafoAtrib = t2.M1_CodTrafo) WHERE t1.CodBase = " + this.strBaseSelected + " AND t1.CodAlimAtrib = '" + strFeeder + "'", this.sqlConnServer);
                            this.sqlCommServer.CommandTimeout = 0;
                            this.mthdInsertCommandInLine("! Criação da seção de cargas BT Não Técnicas");
                            this.sqlDtRdrServer = this.sqlCommServer.ExecuteReader();
                            while (this.sqlDtRdrServer.Read())
                            {
                                this.mthdGetDSSCommandNewLoad("BT_" + this.sqlDtRdrServer["CodConsBT"].ToString() + "_NT", this.sqlDtRdrServer["CodFas"].ToString(), this.sqlDtRdrServer["CodPonAcopl"].ToString(), Convert.ToDouble(this.sqlDtRdrServer["TnsLnh_kV"]), Convert.ToDouble(this.sqlDtRdrServer["TnsFasBas_kV"]), Convert.ToDouble(this.sqlDtRdrServer["DemMax_kW"]), Convert.ToDouble(this.sqlDtRdrServer["DemMaxTrafo_kW"]), null, 2, this.intAdequarTensaoCargasBT, 1);
                            }
                            this.sqlDtRdrServer.Close();
                            this.mthdInputMensageLog("Passou pelas cargas BT Não Técnicas.");
                        }

                        // Ajustes de execução da apuração
                        tolerance = 0.00001;
                        do
                        {
                            tolerance = tolerance * 10;
                            this.mthdInsertCommandInLine("Set tolerance = " + tolerance.ToString().Replace(",", "."));
                            this.mthdInsertCommandInLine("Solve");
                            this.mthdEndCommands();
                        } while (this.intConverged == 0);


                        for (i = 0; i < this.dssCircuit.NumBuses; i++)
                        {
                            dssBus = this.dssCircuit.Buses[i];
                            Vseqn = dssBus.SeqVoltages;
                            cpxVseqn = dssBus.CplxSeqVoltages;
                            V = dssBus.Voltages;
                            vnodes = dssBus.Nodes;

                            strvnodes = "";
                            for (int z = 0; z < vnodes.Length; z++)
                            {
                                strvnodes += "." + vnodes[z];
                            }

                            VseqnVetor[0] = 0;
                            VseqnVetor[1] = 0;
                            VseqnVetor[2] = 0;
                            if (dssBus.NumNodes == 3)
                            {
                                for (int z = 0; z < Vseqn.Length; z++)
                                {
                                    VseqnVetor[z] = Vseqn[z];
                                }
                            }
                            cpxVseqnVetor[0] = 0;
                            cpxVseqnVetor[1] = 0;
                            cpxVseqnVetor[2] = 0;
                            cpxVseqnVetor[3] = 0;
                            cpxVseqnVetor[4] = 0;
                            cpxVseqnVetor[5] = 0;
                            if (dssBus.NumNodes == 3)
                            {
                                for (int z = 0; z < cpxVseqn.Length; z++)
                                {
                                    cpxVseqnVetor[z] = cpxVseqn[z];
                                }
                            }
                            VVetor[0] = 0;
                            VVetor[1] = 0;
                            VVetor[2] = 0;
                            VVetor[3] = 0;
                            VVetor[4] = 0;
                            VVetor[5] = 0;
                            for (int z = 0; z < vnodes.Length; z++)
                            {
                                if (vnodes[z] == 1)
                                {
                                    VVetor[0] = V[2 * z + 0];
                                    VVetor[1] = V[2 * z + 1];
                                }
                                if (vnodes[z] == 2)
                                {
                                    VVetor[2] = V[2 * z + 0];
                                    VVetor[3] = V[2 * z + 1];
                                }
                                if (vnodes[z] == 3)
                                {
                                    VVetor[4] = V[2 * z + 0];
                                    VVetor[5] = V[2 * z + 1];
                                }
                            }

                            // Executa a stored procedure de escrita das cargas
                            this.sqlCommServer = new SqlCommand("dbo.iadAuxReportVoltageSimple", this.sqlConnServer);
                            this.sqlCommServer.CommandType = CommandType.StoredProcedure;
                            this.sqlCommServer.Parameters.AddWithValue("@CodBase", this.strBaseSelected);
                            this.sqlCommServer.Parameters.AddWithValue("@CodAlim", strFeeder);
                            this.sqlCommServer.Parameters.AddWithValue("@CodOperation", 2);
                            this.sqlCommServer.Parameters.AddWithValue("@TipoRodada", this.mthdGetOptionsToString());
                            this.sqlCommServer.Parameters.AddWithValue("@CodBarra", dssBus.Name);
                            this.sqlCommServer.Parameters.AddWithValue("@NodesNumber", dssBus.NumNodes);
                            this.sqlCommServer.Parameters.AddWithValue("@Nodes", strvnodes);
                            this.sqlCommServer.Parameters.AddWithValue("@VoltageBase_kV", dssBus.kVBase);
                            this.sqlCommServer.Parameters.AddWithValue("@MOD_V0_V", VseqnVetor[0]);
                            this.sqlCommServer.Parameters.AddWithValue("@MOD_V1_V", VseqnVetor[1]);
                            this.sqlCommServer.Parameters.AddWithValue("@MOD_V2_V", VseqnVetor[2]);
                            this.sqlCommServer.Parameters.AddWithValue("@REAL_V0_kV", cpxVseqnVetor[0]);
                            this.sqlCommServer.Parameters.AddWithValue("@IMAJ_V0_kV", cpxVseqnVetor[1]);
                            this.sqlCommServer.Parameters.AddWithValue("@REAL_V1_kV", cpxVseqnVetor[2]);
                            this.sqlCommServer.Parameters.AddWithValue("@IMAJ_V1_kV", cpxVseqnVetor[3]);
                            this.sqlCommServer.Parameters.AddWithValue("@REAL_V2_kV", cpxVseqnVetor[4]);
                            this.sqlCommServer.Parameters.AddWithValue("@IMAJ_V2_kV", cpxVseqnVetor[5]);
                            this.sqlCommServer.Parameters.AddWithValue("@REAL_Va_V", VVetor[0]);
                            this.sqlCommServer.Parameters.AddWithValue("@IMAJ_Va_V", VVetor[1]);
                            this.sqlCommServer.Parameters.AddWithValue("@REAL_Vb_V", VVetor[2]);
                            this.sqlCommServer.Parameters.AddWithValue("@IMAJ_Vb_V", VVetor[3]);
                            this.sqlCommServer.Parameters.AddWithValue("@REAL_Vc_V", VVetor[4]);
                            this.sqlCommServer.Parameters.AddWithValue("@IMAJ_Vc_V", VVetor[5]);
                            this.sqlCommServer.Parameters.AddWithValue("@MOD_Va_V", Math.Sqrt(VVetor[0] * VVetor[0] + VVetor[1] * VVetor[1]));
                            this.sqlCommServer.Parameters.AddWithValue("@ANG_Va_V", angleComplex(VVetor[0], VVetor[1]));
                            this.sqlCommServer.Parameters.AddWithValue("@MOD_Vb_V", Math.Sqrt(VVetor[2] * VVetor[2] + VVetor[3] * VVetor[3]));
                            this.sqlCommServer.Parameters.AddWithValue("@ANG_Vb_V", angleComplex(VVetor[2], VVetor[3]));
                            this.sqlCommServer.Parameters.AddWithValue("@MOD_Vc_V", Math.Sqrt(VVetor[4] * VVetor[4] + VVetor[5] * VVetor[5]));
                            this.sqlCommServer.Parameters.AddWithValue("@ANG_Vc_V", angleComplex(VVetor[4], VVetor[5]));
                            this.sqlCommServer.ExecuteNonQuery();
                        }

                        // Término do processo
                        this.mthdInputMensageLog("Término do processo para o alimentador " + strFeeder + ".");
                        this.rtbLog.Refresh();

                    }

                }
                catch (Exception xcptn)
                {

                    this.mthdInputMensageLog("Problema não identificado (" + xcptn.Message.ToString() + ").");
                    this.sqlDtRdrServer.Dispose();
                    this.sqlCommServer.Dispose();

                }

            }

        }

        private bool checkBase()
        {
            bool result = false;

            if (this.tbObjectBase.Text == null || this.tbObjectBase.Text == "")
            {
                this.mthdInputMensageLog("A base a ser calculada não foi indicada.");
            }
            else if (!Conectar())
            {
                this.mthdInputMensageLog("A conexão com o banco de dados não foi estabelecida.");
            }
            else result = true;
            return result;
        }

        private void bBalance_Click(object sender, EventArgs e)
        {

            EnergyBalanceTools mySystem = new EnergyBalanceTools();

            if (checkBase())
            {
                try
                {

                    // Captura a base a ser calculada
                    this.strBaseSelected = this.tbObjectBase.Text;

                    // Loop das energias injetadas e fornecidas
                    this.sqlCommServer = new SqlCommand("SELECT t1.* FROM dbo.[" + this.strBaseSelected + "BalanEnerg] AS t1 WHERE t1.CodBase = " + this.strBaseSelected, this.sqlConnServer);
                    this.sqlCommServer.CommandTimeout = 0;
                    this.sqlDtRdrServer = this.sqlCommServer.ExecuteReader();
                    while (this.sqlDtRdrServer.Read())
                    {
                        mySystem.addInjectEnergy_MWh(this.sqlDtRdrServer["NivTen"].ToString(), Convert.ToDouble(this.sqlDtRdrServer["EnerInj01_MWh"]) + Convert.ToDouble(this.sqlDtRdrServer["EnerInj02_MWh"]) + Convert.ToDouble(this.sqlDtRdrServer["EnerInj03_MWh"]) + Convert.ToDouble(this.sqlDtRdrServer["EnerInj04_MWh"]) + Convert.ToDouble(this.sqlDtRdrServer["EnerInj05_MWh"]) + Convert.ToDouble(this.sqlDtRdrServer["EnerInj06_MWh"]) + Convert.ToDouble(this.sqlDtRdrServer["EnerInj07_MWh"]) + Convert.ToDouble(this.sqlDtRdrServer["EnerInj08_MWh"]) + Convert.ToDouble(this.sqlDtRdrServer["EnerInj09_MWh"]) + Convert.ToDouble(this.sqlDtRdrServer["EnerInj10_MWh"]) + Convert.ToDouble(this.sqlDtRdrServer["EnerInj11_MWh"]) + Convert.ToDouble(this.sqlDtRdrServer["EnerInj12_MWh"]));
                        mySystem.addSupplyEnergy_MWh(this.sqlDtRdrServer["NivTen"].ToString(), Convert.ToDouble(this.sqlDtRdrServer["EnerFornMercLivr01_MWh"]) + Convert.ToDouble(this.sqlDtRdrServer["EnerFornMercCatv01_MWh"]) + Convert.ToDouble(this.sqlDtRdrServer["EnerFornOutrDistr01_MWh"]) + Convert.ToDouble(this.sqlDtRdrServer["EnerFornMercLivr02_MWh"]) + Convert.ToDouble(this.sqlDtRdrServer["EnerFornMercCatv02_MWh"]) + Convert.ToDouble(this.sqlDtRdrServer["EnerFornOutrDistr02_MWh"]) + Convert.ToDouble(this.sqlDtRdrServer["EnerFornMercLivr03_MWh"]) + Convert.ToDouble(this.sqlDtRdrServer["EnerFornMercCatv03_MWh"]) + Convert.ToDouble(this.sqlDtRdrServer["EnerFornOutrDistr03_MWh"]) + Convert.ToDouble(this.sqlDtRdrServer["EnerFornMercLivr04_MWh"]) + Convert.ToDouble(this.sqlDtRdrServer["EnerFornMercCatv04_MWh"]) + Convert.ToDouble(this.sqlDtRdrServer["EnerFornOutrDistr04_MWh"]) + Convert.ToDouble(this.sqlDtRdrServer["EnerFornMercLivr05_MWh"]) + Convert.ToDouble(this.sqlDtRdrServer["EnerFornMercCatv05_MWh"]) + Convert.ToDouble(this.sqlDtRdrServer["EnerFornOutrDistr05_MWh"]) + Convert.ToDouble(this.sqlDtRdrServer["EnerFornMercLivr06_MWh"]) + Convert.ToDouble(this.sqlDtRdrServer["EnerFornMercCatv06_MWh"]) + Convert.ToDouble(this.sqlDtRdrServer["EnerFornOutrDistr06_MWh"]) + Convert.ToDouble(this.sqlDtRdrServer["EnerFornMercLivr07_MWh"]) + Convert.ToDouble(this.sqlDtRdrServer["EnerFornMercCatv07_MWh"]) + Convert.ToDouble(this.sqlDtRdrServer["EnerFornOutrDistr07_MWh"]) + Convert.ToDouble(this.sqlDtRdrServer["EnerFornMercLivr08_MWh"]) + Convert.ToDouble(this.sqlDtRdrServer["EnerFornMercCatv08_MWh"]) + Convert.ToDouble(this.sqlDtRdrServer["EnerFornOutrDistr08_MWh"]) + Convert.ToDouble(this.sqlDtRdrServer["EnerFornMercLivr09_MWh"]) + Convert.ToDouble(this.sqlDtRdrServer["EnerFornMercCatv09_MWh"]) + Convert.ToDouble(this.sqlDtRdrServer["EnerFornOutrDistr09_MWh"]) + Convert.ToDouble(this.sqlDtRdrServer["EnerFornMercLivr10_MWh"]) + Convert.ToDouble(this.sqlDtRdrServer["EnerFornMercCatv10_MWh"]) + Convert.ToDouble(this.sqlDtRdrServer["EnerFornOutrDistr10_MWh"]) + Convert.ToDouble(this.sqlDtRdrServer["EnerFornMercLivr11_MWh"]) + Convert.ToDouble(this.sqlDtRdrServer["EnerFornMercCatv11_MWh"]) + Convert.ToDouble(this.sqlDtRdrServer["EnerFornOutrDistr11_MWh"]) + Convert.ToDouble(this.sqlDtRdrServer["EnerFornMercLivr12_MWh"]) + Convert.ToDouble(this.sqlDtRdrServer["EnerFornMercCatv12_MWh"]) + Convert.ToDouble(this.sqlDtRdrServer["EnerFornOutrDistr12_MWh"]));
                        mySystem.addSupplyEnergywithoutNet_MWh(this.sqlDtRdrServer["NivTen"].ToString(), Convert.ToDouble(this.sqlDtRdrServer["EnerFornSemRedAssoc01_MWh"]) + Convert.ToDouble(this.sqlDtRdrServer["EnerFornSemRedAssoc02_MWh"]) + Convert.ToDouble(this.sqlDtRdrServer["EnerFornSemRedAssoc03_MWh"]) + Convert.ToDouble(this.sqlDtRdrServer["EnerFornSemRedAssoc04_MWh"]) + Convert.ToDouble(this.sqlDtRdrServer["EnerFornSemRedAssoc05_MWh"]) + Convert.ToDouble(this.sqlDtRdrServer["EnerFornSemRedAssoc06_MWh"]) + Convert.ToDouble(this.sqlDtRdrServer["EnerFornSemRedAssoc07_MWh"]) + Convert.ToDouble(this.sqlDtRdrServer["EnerFornSemRedAssoc08_MWh"]) + Convert.ToDouble(this.sqlDtRdrServer["EnerFornSemRedAssoc09_MWh"]) + Convert.ToDouble(this.sqlDtRdrServer["EnerFornSemRedAssoc10_MWh"]) + Convert.ToDouble(this.sqlDtRdrServer["EnerFornSemRedAssoc11_MWh"]) + Convert.ToDouble(this.sqlDtRdrServer["EnerFornSemRedAssoc12_MWh"]));
                    }
                    this.sqlDtRdrServer.Close();
                    this.mthdInputMensageLog("Passou pelas energias injetadas e fornecidas.");

                    // Loop das perdas de energia de alta tensão
                    this.sqlCommServer = new SqlCommand("SELECT t1.* FROM dbo.[" + this.strBaseSelected + "PerdaTecnciaAltaTensaoMedida] AS t1 WHERE t1.CodBase = " + this.strBaseSelected, this.sqlConnServer);
                    this.sqlCommServer.CommandTimeout = 0;
                    this.sqlDtRdrServer = this.sqlCommServer.ExecuteReader();
                    while (this.sqlDtRdrServer.Read())
                    {
                        mySystem.addTecnicalLossesFixedEnergy_MWh(this.sqlDtRdrServer["NivTen"].ToString(), Convert.ToDouble(this.sqlDtRdrServer["PerdTecnicaATMedid01_MWh"]) + Convert.ToDouble(this.sqlDtRdrServer["PerdTecnicaATMedid02_MWh"]) + Convert.ToDouble(this.sqlDtRdrServer["PerdTecnicaATMedid03_MWh"]) + Convert.ToDouble(this.sqlDtRdrServer["PerdTecnicaATMedid04_MWh"]) + Convert.ToDouble(this.sqlDtRdrServer["PerdTecnicaATMedid05_MWh"]) + Convert.ToDouble(this.sqlDtRdrServer["PerdTecnicaATMedid06_MWh"]) + Convert.ToDouble(this.sqlDtRdrServer["PerdTecnicaATMedid07_MWh"]) + Convert.ToDouble(this.sqlDtRdrServer["PerdTecnicaATMedid08_MWh"]) + Convert.ToDouble(this.sqlDtRdrServer["PerdTecnicaATMedid09_MWh"]) + Convert.ToDouble(this.sqlDtRdrServer["PerdTecnicaATMedid10_MWh"]) + Convert.ToDouble(this.sqlDtRdrServer["PerdTecnicaATMedid11_MWh"]) + Convert.ToDouble(this.sqlDtRdrServer["PerdTecnicaATMedid12_MWh"]));
                    }
                    this.sqlDtRdrServer.Close();
                    this.mthdInputMensageLog("Passou pelas perdas de energia de alta tensão.");

                    // Loop dos transformadores de potência
                    this.sqlCommServer = new SqlCommand("SELECT t1.* FROM dbo.[" + this.strBaseSelected + "TrafoATATATMT] AS t1 WHERE t1.CodBase = " + this.strBaseSelected, this.sqlConnServer);
                    this.sqlCommServer.CommandTimeout = 0;
                    this.sqlDtRdrServer = this.sqlCommServer.ExecuteReader();
                    while (this.sqlDtRdrServer.Read())
                    {
                        mySystem.addTransformer(this.sqlDtRdrServer["NivTenPrim"].ToString(), this.sqlDtRdrServer["NivTenSecu"].ToString(), this.sqlDtRdrServer["NivTenTerc"].ToString(), Convert.ToDouble(this.sqlDtRdrServer["EnerMedid_MWh"]), Convert.ToDouble(this.sqlDtRdrServer["PerdEnerFerr01_MWh"]) + Convert.ToDouble(this.sqlDtRdrServer["PerdEnerFerr02_MWh"]) + Convert.ToDouble(this.sqlDtRdrServer["PerdEnerFerr03_MWh"]) + Convert.ToDouble(this.sqlDtRdrServer["PerdEnerFerr04_MWh"]) + Convert.ToDouble(this.sqlDtRdrServer["PerdEnerFerr05_MWh"]) + Convert.ToDouble(this.sqlDtRdrServer["PerdEnerFerr06_MWh"]) + Convert.ToDouble(this.sqlDtRdrServer["PerdEnerFerr07_MWh"]) + Convert.ToDouble(this.sqlDtRdrServer["PerdEnerFerr08_MWh"]) + Convert.ToDouble(this.sqlDtRdrServer["PerdEnerFerr09_MWh"]) + Convert.ToDouble(this.sqlDtRdrServer["PerdEnerFerr10_MWh"]) + Convert.ToDouble(this.sqlDtRdrServer["PerdEnerFerr11_MWh"]) + Convert.ToDouble(this.sqlDtRdrServer["PerdEnerFerr12_MWh"]), Convert.ToDouble(this.sqlDtRdrServer["PerdEnerCobr01_MWh"]) + Convert.ToDouble(this.sqlDtRdrServer["PerdEnerCobr02_MWh"]) + Convert.ToDouble(this.sqlDtRdrServer["PerdEnerCobr03_MWh"]) + Convert.ToDouble(this.sqlDtRdrServer["PerdEnerCobr04_MWh"]) + Convert.ToDouble(this.sqlDtRdrServer["PerdEnerCobr05_MWh"]) + Convert.ToDouble(this.sqlDtRdrServer["PerdEnerCobr06_MWh"]) + Convert.ToDouble(this.sqlDtRdrServer["PerdEnerCobr07_MWh"]) + Convert.ToDouble(this.sqlDtRdrServer["PerdEnerCobr08_MWh"]) + Convert.ToDouble(this.sqlDtRdrServer["PerdEnerCobr09_MWh"]) + Convert.ToDouble(this.sqlDtRdrServer["PerdEnerCobr10_MWh"]) + Convert.ToDouble(this.sqlDtRdrServer["PerdEnerCobr11_MWh"]) + Convert.ToDouble(this.sqlDtRdrServer["PerdEnerCobr12_MWh"]), this.sqlDtRdrServer["AlocPerdTrafo"].ToString(), Convert.ToInt32(this.sqlDtRdrServer["SentFluxo"]));
                    }
                    this.sqlDtRdrServer.Close();
                    this.mthdInputMensageLog("Passou pelos transformadores de potência.");

                    // Loop das energias transformadas
                    this.sqlCommServer = new SqlCommand("SELECT t1.* FROM dbo.[" + this.strBaseSelected + "EnergPass] AS t1 WHERE t1.CodBase = " + this.strBaseSelected + " AND ([t1].[OrdmPrim] NOT IN (4,5) OR [t1].[OrdmSecu] NOT IN (4,5))", this.sqlConnServer);
                    this.sqlCommServer.CommandTimeout = 0;
                    this.sqlDtRdrServer = this.sqlCommServer.ExecuteReader();
                    while (this.sqlDtRdrServer.Read())
                    {
                        mySystem.addSegmentTransformation(this.sqlDtRdrServer["NivTenPrim"].ToString(), this.sqlDtRdrServer["NivTenSecu"].ToString(), Convert.ToDouble(this.sqlDtRdrServer["EnerPass01_MWh"]) + Convert.ToDouble(this.sqlDtRdrServer["EnerPass02_MWh"]) + Convert.ToDouble(this.sqlDtRdrServer["EnerPass03_MWh"]) + Convert.ToDouble(this.sqlDtRdrServer["EnerPass04_MWh"]) + Convert.ToDouble(this.sqlDtRdrServer["EnerPass05_MWh"]) + Convert.ToDouble(this.sqlDtRdrServer["EnerPass06_MWh"]) + Convert.ToDouble(this.sqlDtRdrServer["EnerPass07_MWh"]) + Convert.ToDouble(this.sqlDtRdrServer["EnerPass08_MWh"]) + Convert.ToDouble(this.sqlDtRdrServer["EnerPass09_MWh"]) + Convert.ToDouble(this.sqlDtRdrServer["EnerPass10_MWh"]) + Convert.ToDouble(this.sqlDtRdrServer["EnerPass11_MWh"]) + Convert.ToDouble(this.sqlDtRdrServer["EnerPass12_MWh"]));
                    }
                    this.sqlDtRdrServer.Close();
                    this.mthdInputMensageLog("Passou pelas energias transformadas.");

                    // Loop das proporções dos transformadores para as energias transformadas
                    this.sqlCommServer = new SqlCommand("SELECT DISTINCT [t1].[CodBase], [dbo].[NivelTensaoSimplificado]([t1].[NivTenPrim], [t1].[NivTenSecu], [t1].[NivTenTerc], [t1].[SentFluxo], 1) AS [NivTenPrimT], [dbo].[NivelTensaoSimplificado]([t1].[NivTenPrim], [t1].[NivTenSecu], [t1].[NivTenTerc], [t1].[SentFluxo], 2) AS [NivTenSecuT], [dbo].[NivelTensaoSimplificado]([t1].[NivTenPrim], [t1].[NivTenSecu], [t1].[NivTenTerc], [t1].[SentFluxo], 3) AS [NivTenTercT], [dbo].[NivelTensaoSimplificado]([t2].[NivTenPrim], [t2].[NivTenSecu], [t2].[NivTenSecu], 0, 1) AS [NivTenPrimET], [dbo].[NivelTensaoSimplificado]([t2].[NivTenPrim], [t2].[NivTenSecu], [t2].[NivTenSecu], 0, 2) AS [NivTenSecuET], (ISNULL([t2].[EnerPass01_MWh],0)+ISNULL([t2].[EnerPass02_MWh],0)+ISNULL([t2].[EnerPass03_MWh],0)+ISNULL([t2].[EnerPass04_MWh],0)+ISNULL([t2].[EnerPass05_MWh],0)+ISNULL([t2].[EnerPass06_MWh],0)+ISNULL([t2].[EnerPass07_MWh],0)+ISNULL([t2].[EnerPass08_MWh],0)+ISNULL([t2].[EnerPass09_MWh],0)+ISNULL([t2].[EnerPass10_MWh],0)+ISNULL([t2].[EnerPass11_MWh],0)+ISNULL([t2].[EnerPass12_MWh],0)) AS [EnerPass_MWh] FROM [dbo].[" + this.strBaseSelected + "TrafoATATATMT] [t1] INNER JOIN [dbo].[" + this.strBaseSelected + "EnergPass] [t2] ON ( [t1].[CodBase] = [t2].[CodBase] AND ( ( [dbo].[NivelTensaoSimplificado]([t1].[NivTenPrim], [t1].[NivTenSecu], [t1].[NivTenTerc], [t1].[SentFluxo], 3) IS NULL AND [dbo].[NivelTensaoSimplificado]([t1].[NivTenPrim], [t1].[NivTenSecu], [t1].[NivTenTerc], [t1].[SentFluxo], 1) = [dbo].[NivelTensaoSimplificado]([t2].[NivTenPrim], [t2].[NivTenSecu], [t2].[NivTenSecu], 0, 1) AND [dbo].[NivelTensaoSimplificado]([t1].[NivTenPrim], [t1].[NivTenSecu], [t1].[NivTenTerc], [t1].[SentFluxo], 2) = [dbo].[NivelTensaoSimplificado]([t2].[NivTenPrim], [t2].[NivTenSecu], [t2].[NivTenSecu], 0, 2) ) OR ( [dbo].[NivelTensaoSimplificado]([t1].[NivTenPrim], [t1].[NivTenSecu], [t1].[NivTenTerc], [t1].[SentFluxo], 3) IS NOT NULL AND (  (  [dbo].[NivelTensaoSimplificado]([t1].[NivTenPrim], [t1].[NivTenSecu], [t1].[NivTenTerc], [t1].[SentFluxo], 1) = [dbo].[NivelTensaoSimplificado]([t2].[NivTenPrim], [t2].[NivTenSecu], [t2].[NivTenSecu], 0, 1) AND [dbo].[NivelTensaoSimplificado]([t1].[NivTenPrim], [t1].[NivTenSecu], [t1].[NivTenTerc], [t1].[SentFluxo], 2) = [dbo].[NivelTensaoSimplificado]([t2].[NivTenPrim], [t2].[NivTenSecu], [t2].[NivTenSecu], 0, 2)  )  OR  (  [dbo].[NivelTensaoSimplificado]([t1].[NivTenPrim], [t1].[NivTenSecu], [t1].[NivTenTerc], [t1].[SentFluxo], 1) = [dbo].[NivelTensaoSimplificado]([t2].[NivTenPrim], [t2].[NivTenSecu], [t2].[NivTenSecu], 0, 1) AND [dbo].[NivelTensaoSimplificado]([t1].[NivTenPrim], [t1].[NivTenSecu], [t1].[NivTenTerc], [t1].[SentFluxo], 3) = [dbo].[NivelTensaoSimplificado]([t2].[NivTenPrim], [t2].[NivTenSecu], [t2].[NivTenSecu], 0, 2)  ) ) ) ) ) WHERE [t1].[CodBase] = " + this.strBaseSelected + " AND [t1].[OrdmPrim] <= 5 AND [t1].[OrdmSecu] <= 5 AND ([t1].[OrdmTerc] <= 5 OR [t1].[OrdmTerc] IS NULL) AND [t2].[OrdmPrim] <= 5 AND [t2].[OrdmSecu] <= 5", this.sqlConnServer);
                    this.sqlCommServer.CommandTimeout = 0;
                    this.sqlDtRdrServer = this.sqlCommServer.ExecuteReader();
                    while (this.sqlDtRdrServer.Read())
                    {
                        mySystem.addProportionTtoST(this.sqlDtRdrServer["NivTenPrimT"].ToString(), this.sqlDtRdrServer["NivTenSecuT"].ToString(), this.sqlDtRdrServer["NivTenTercT"].ToString(), this.sqlDtRdrServer["NivTenPrimET"].ToString(), this.sqlDtRdrServer["NivTenSecuET"].ToString(), Convert.ToDouble(this.sqlDtRdrServer["EnerPass_MWh"]));
                    }
                    this.sqlDtRdrServer.Close();
                    this.mthdInputMensageLog("Passou pelas proporções dos transformadores para as energias transformadas.");

                    // Loop das proporções de energias transformadas para os transformadores
                    this.sqlCommServer = new SqlCommand("SELECT DISTINCT [t1].[CodBase], [dbo].[NivelTensaoSimplificado]([t2].[NivTenPrim], [t2].[NivTenSecu], [t2].[NivTenSecu], 0, 1) AS [NivTenPrimET], [dbo].[NivelTensaoSimplificado]([t2].[NivTenPrim], [t2].[NivTenSecu], [t2].[NivTenSecu], 0, 2) AS [NivTenSecuET], [dbo].[NivelTensaoSimplificado]([t1].[NivTenPrim], [t1].[NivTenSecu], [t1].[NivTenTerc], [t1].[SentFluxo], 1) AS [NivTenPrimT], [dbo].[NivelTensaoSimplificado]([t1].[NivTenPrim], [t1].[NivTenSecu], [t1].[NivTenTerc], [t1].[SentFluxo], 2) AS [NivTenSecuT], [dbo].[NivelTensaoSimplificado]([t1].[NivTenPrim], [t1].[NivTenSecu], [t1].[NivTenTerc], [t1].[SentFluxo], 3) AS [NivTenTercT], [t1].[EnerMedid_MWh] FROM [dbo].[" + this.strBaseSelected + "TrafoATATATMT] [t1] INNER JOIN [dbo].[" + this.strBaseSelected + "EnergPass] [t2] ON ( [t1].[CodBase] = [t2].[CodBase] AND ( ( [dbo].[NivelTensaoSimplificado]([t1].[NivTenPrim], [t1].[NivTenSecu], [t1].[NivTenTerc], [t1].[SentFluxo], 3) IS NULL AND [dbo].[NivelTensaoSimplificado]([t1].[NivTenPrim], [t1].[NivTenSecu], [t1].[NivTenTerc], [t1].[SentFluxo], 1) = [dbo].[NivelTensaoSimplificado]([t2].[NivTenPrim], [t2].[NivTenSecu], [t2].[NivTenSecu], 0, 1) AND [dbo].[NivelTensaoSimplificado]([t1].[NivTenPrim], [t1].[NivTenSecu], [t1].[NivTenTerc], [t1].[SentFluxo], 2) = [dbo].[NivelTensaoSimplificado]([t2].[NivTenPrim], [t2].[NivTenSecu], [t2].[NivTenSecu], 0, 2) ) OR ( [dbo].[NivelTensaoSimplificado]([t1].[NivTenPrim], [t1].[NivTenSecu], [t1].[NivTenTerc], [t1].[SentFluxo], 3) IS NOT NULL AND ( ([dbo].[NivelTensaoSimplificado]([t1].[NivTenPrim], [t1].[NivTenSecu], [t1].[NivTenTerc], [t1].[SentFluxo], 1) = [dbo].[NivelTensaoSimplificado]([t2].[NivTenPrim], [t2].[NivTenSecu], [t2].[NivTenSecu], 0, 1) AND [dbo].[NivelTensaoSimplificado]([t1].[NivTenPrim], [t1].[NivTenSecu], [t1].[NivTenTerc], [t1].[SentFluxo], 2) = [dbo].[NivelTensaoSimplificado]([t2].[NivTenPrim], [t2].[NivTenSecu], [t2].[NivTenSecu], 0, 2)) OR ([dbo].[NivelTensaoSimplificado]([t1].[NivTenPrim], [t1].[NivTenSecu], [t1].[NivTenTerc], [t1].[SentFluxo], 1) = [dbo].[NivelTensaoSimplificado]([t2].[NivTenPrim], [t2].[NivTenSecu], [t2].[NivTenSecu], 0, 1) AND [dbo].[NivelTensaoSimplificado]([t1].[NivTenPrim], [t1].[NivTenSecu], [t1].[NivTenTerc], [t1].[SentFluxo], 3) = [dbo].[NivelTensaoSimplificado]([t2].[NivTenPrim], [t2].[NivTenSecu], [t2].[NivTenSecu], 0, 2)) ) ) ) ) WHERE [t1].[CodBase] = " + this.strBaseSelected + " AND [t1].[OrdmPrim] <= 5 AND [t1].[OrdmSecu] <= 5 AND ([t1].[OrdmTerc] <= 5 OR [t1].[OrdmTerc] IS NULL) AND [t2].[OrdmPrim] <= 5 AND [t2].[OrdmSecu] <= 5", this.sqlConnServer);
                    this.sqlCommServer.CommandTimeout = 0;
                    this.sqlDtRdrServer = this.sqlCommServer.ExecuteReader();
                    while (this.sqlDtRdrServer.Read())
                    {
                        mySystem.addProportionSTtoT(this.sqlDtRdrServer["NivTenPrimET"].ToString(), this.sqlDtRdrServer["NivTenSecuET"].ToString(), this.sqlDtRdrServer["NivTenPrimT"].ToString(), this.sqlDtRdrServer["NivTenSecuT"].ToString(), this.sqlDtRdrServer["NivTenTercT"].ToString(), Convert.ToDouble(this.sqlDtRdrServer["EnerMedid_MWh"]));
                    }
                    this.sqlDtRdrServer.Close();
                    this.mthdInputMensageLog("Passou pelas proporções de energias transformadas para os transformadores.");

                    mySystem.runBalance();

                    this.mthdInputMensageLog(mySystem.ToString());

                }
                catch (Exception xcptn)
                {

                    this.mthdInputMensageLog("Problema não identificado (" + xcptn.Message.ToString() + ").");
                    this.sqlDtRdrServer.Dispose();
                    this.sqlCommServer.Dispose();

                }                
            }
        }

        private void tbVPUMin_KeyPress(object sender, KeyPressEventArgs e)
        {

            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != ','))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == ',') && ((sender as TextBox).Text.IndexOf(',') > -1))
            {
                e.Handled = true;
            }

        }

        private void tbCommand_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return)
            {

                int intRealizouComando = 0;
                string[] strSeparators = new string[] { " " };
                string[] strCommands = this.tbCommand.Text.ToUpper().TrimStart(null).TrimEnd(null).Split(strSeparators, StringSplitOptions.RemoveEmptyEntries);

                if (this.tbCommand.Text.Length == 0)
                {
                    this.tbCommand.Clear();
                }

                if (strCommands.Length == 1 && strCommands[0] == "?")
                {

                    this.rtbAdmin.AppendText("[" + DateTime.Now.ToString() + "] " + this.tbCommand.Text + Environment.NewLine);

                    if (this.sqlConnServer == null)
                        this.rtbAdmin.AppendText("[" + DateTime.Now.ToString() + "] > A conexão com o banco de dados não foi estabelecida." + Environment.NewLine);
                    else
                    {
                        this.rtbAdmin.AppendText("[" + DateTime.Now.ToString() + "] > Listagem de comandos." + Environment.NewLine);
                        this.rtbAdmin.AppendText("?" + Environment.NewLine);
                        this.rtbAdmin.AppendText("PRINCIPAL <número_base>" + Environment.NewLine);
                        this.rtbAdmin.AppendText("ARMAZENACARGANTECNICA <número_base>" + Environment.NewLine);
                        this.rtbAdmin.AppendText("ARMAZENARESULTADO <número_base>" + Environment.NewLine);
                        this.rtbAdmin.AppendText("ARMAZENA <número_base>" + Environment.NewLine);
                        this.rtbAdmin.AppendText("RECARREGA <número_base>" + Environment.NewLine);
                    }

                    this.rtbAdmin.AppendText("[" + DateTime.Now.ToString() + "] > Fim da operação." + Environment.NewLine);
                    intRealizouComando = 1;
                    this.tbCommand.Clear();

                }

                if (strCommands.Length == 2 && strCommands[0] == "PRINCIPAL")
                {

                    this.rtbAdmin.AppendText("[" + DateTime.Now.ToString() + "] " + this.tbCommand.Text + Environment.NewLine);

                    if (this.sqlConnServer == null)
                        this.rtbAdmin.AppendText("[" + DateTime.Now.ToString() + "] > A conexão com o banco de dados não foi estabelecida." + Environment.NewLine);
                    else
                    {
                        this.rtbAdmin.AppendText("[" + DateTime.Now.ToString() + "] > Início da operação." + Environment.NewLine);
                        this.sqlCommServer = new SqlCommand("dbo.Principal", this.sqlConnServer);
                        this.sqlCommServer.CommandType = CommandType.StoredProcedure;
                        this.sqlCommServer.CommandTimeout = 0;
                        this.sqlCommServer.Parameters.AddWithValue("@CodBase", strCommands[1]);
                        this.sqlCommServer.ExecuteNonQuery();
                    }

                    this.rtbAdmin.AppendText("[" + DateTime.Now.ToString() + "] > Fim da operação." + Environment.NewLine);
                    intRealizouComando = 1;
                    this.tbCommand.Clear();

                }

                if (strCommands.Length == 2 && strCommands[0] == "RECARREGA")
                {

                    this.rtbAdmin.AppendText("[" + DateTime.Now.ToString() + "] " + this.tbCommand.Text + Environment.NewLine);

                    if (this.sqlConnServer == null)
                        this.rtbAdmin.AppendText("[" + DateTime.Now.ToString() + "] > A conexão com o banco de dados não foi estabelecida." + Environment.NewLine);
                    else
                    {
                        this.rtbAdmin.AppendText("[" + DateTime.Now.ToString() + "] > Início da operação." + Environment.NewLine);
                        this.sqlCommServer = new SqlCommand("dbo.Recarrega", this.sqlConnServer);
                        this.sqlCommServer.CommandType = CommandType.StoredProcedure;
                        this.sqlCommServer.CommandTimeout = 0;
                        this.sqlCommServer.Parameters.AddWithValue("@CodBase", strCommands[1]);
                        this.sqlCommServer.ExecuteNonQuery();
                    }

                    this.rtbAdmin.AppendText("[" + DateTime.Now.ToString() + "] > Fim da operação." + Environment.NewLine);
                    intRealizouComando = 1;
                    this.tbCommand.Clear();

                }

                if (strCommands.Length == 2 && strCommands[0] == "ARMAZENACARGANTECNICA")
                {

                    this.rtbAdmin.AppendText("[" + DateTime.Now.ToString() + "] " + this.tbCommand.Text + Environment.NewLine);

                    if (this.sqlConnServer == null)
                        this.rtbAdmin.AppendText("[" + DateTime.Now.ToString() + "] > A conexão com o banco de dados não foi estabelecida." + Environment.NewLine);
                    else
                    {
                        this.rtbAdmin.AppendText("[" + DateTime.Now.ToString() + "] > Início da operação." + Environment.NewLine);
                        this.sqlCommServer = new SqlCommand("dbo.ExecutaCargaArmazenamento", this.sqlConnServer);
                        this.sqlCommServer.CommandType = CommandType.StoredProcedure;
                        this.sqlCommServer.CommandTimeout = 0;
                        this.sqlCommServer.Parameters.AddWithValue("@CodBase", strCommands[1]);
                        this.sqlCommServer.Parameters.AddWithValue("@CodOperation", "2");
                        this.sqlCommServer.ExecuteNonQuery();
                    }

                    this.rtbAdmin.AppendText("[" + DateTime.Now.ToString() + "] > Fim da operação." + Environment.NewLine);
                    intRealizouComando = 1;
                    this.tbCommand.Clear();

                }

                if (strCommands.Length == 2 && strCommands[0] == "ARMAZENARESULTADO")
                {

                    this.rtbAdmin.AppendText("[" + DateTime.Now.ToString() + "] " + this.tbCommand.Text + Environment.NewLine);

                    if (this.sqlConnServer == null)
                        this.rtbAdmin.AppendText("[" + DateTime.Now.ToString() + "] > A conexão com o banco de dados não foi estabelecida." + Environment.NewLine);
                    else
                    {
                        this.rtbAdmin.AppendText("[" + DateTime.Now.ToString() + "] > Início da operação." + Environment.NewLine);
                        this.sqlCommServer = new SqlCommand("dbo.ExecutaCargaArmazenamento", this.sqlConnServer);
                        this.sqlCommServer.CommandType = CommandType.StoredProcedure;
                        this.sqlCommServer.CommandTimeout = 0;
                        this.sqlCommServer.Parameters.AddWithValue("@CodBase", strCommands[1]);
                        this.sqlCommServer.Parameters.AddWithValue("@CodOperation", "3");
                        this.sqlCommServer.ExecuteNonQuery();
                    }

                    this.rtbAdmin.AppendText("[" + DateTime.Now.ToString() + "] > Fim da operação." + Environment.NewLine);
                    intRealizouComando = 1;
                    this.tbCommand.Clear();

                }

                if (strCommands.Length == 2 && strCommands[0] == "ARMAZENA")
                {

                    this.rtbAdmin.AppendText("[" + DateTime.Now.ToString() + "] " + this.tbCommand.Text + Environment.NewLine);

                    if (this.sqlConnServer == null)
                        this.rtbAdmin.AppendText("[" + DateTime.Now.ToString() + "] > A conexão com o banco de dados não foi estabelecida." + Environment.NewLine);
                    else
                    {
                        this.rtbAdmin.AppendText("[" + DateTime.Now.ToString() + "] > Início da operação." + Environment.NewLine);
                        this.sqlCommServer = new SqlCommand("dbo.ExecutaCargaArmazenamento", this.sqlConnServer);
                        this.sqlCommServer.CommandType = CommandType.StoredProcedure;
                        this.sqlCommServer.CommandTimeout = 0;
                        this.sqlCommServer.Parameters.AddWithValue("@CodBase", strCommands[1]);
                        this.sqlCommServer.Parameters.AddWithValue("@CodOperation", "1");
                        this.sqlCommServer.ExecuteNonQuery();
                        this.sqlCommServer = new SqlCommand("dbo.ExecutaCargaArmazenamento", this.sqlConnServer);
                        this.sqlCommServer.CommandType = CommandType.StoredProcedure;
                        this.sqlCommServer.CommandTimeout = 0;
                        this.sqlCommServer.Parameters.AddWithValue("@CodBase", strCommands[1]);
                        this.sqlCommServer.Parameters.AddWithValue("@CodOperation", "3");
                        this.sqlCommServer.ExecuteNonQuery();
                        this.sqlCommServer = new SqlCommand("dbo.iadTabelaOperacao", this.sqlConnServer);
                        this.sqlCommServer.CommandType = CommandType.StoredProcedure;
                        this.sqlCommServer.CommandTimeout = 0;
                        this.sqlCommServer.Parameters.AddWithValue("@CodBase", strCommands[1]);
                        this.sqlCommServer.Parameters.AddWithValue("@CodOperation", "2");
                        this.sqlCommServer.ExecuteNonQuery();
                    }

                    this.rtbAdmin.AppendText("[" + DateTime.Now.ToString() + "] > Fim da operação." + Environment.NewLine);
                    intRealizouComando = 1;
                    this.tbCommand.Clear();

                }

                if (intRealizouComando == 0)
                {

                    this.rtbAdmin.AppendText("[" + DateTime.Now.ToString() + "] " + this.tbCommand.Text + Environment.NewLine);
                    this.rtbAdmin.AppendText("[" + DateTime.Now.ToString() + "] > Comando não reconhecido." + Environment.NewLine);
                    this.tbCommand.Clear();

                }

            }

        }

        private void cbServer_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbServer.SelectedItem == "bdgeodisthom,5480")
                tbDataBase.Text = "NovoGeoPerdas";
            if (cbServer.SelectedItem == "bdgeodist,5451")
                tbDataBase.Text = "GeoPerdas2019";
        }

        /// <summary>
        /// Define texto de objeto entre threads
        /// </summary>
        /// <param name="txt"></param>
        public void mudaFormText(string txt, Form objRef)
        {
            if (objRef.InvokeRequired)
            {
                mudaTextFormCallback d = new mudaTextFormCallback(mudaFormText);
                this.Invoke(d, new object[] { txt, objRef });
            }
            else
            {
                objRef.Text = txt;
            }
        }


        /// <summary>
        /// Muda Versao no Titulo da Aplicacao
        /// </summary>
        /// <returns></returns>
        public void mudaTitApp(string complemento)
        {
            mudaFormText("ProgGeoPerdas - Apuração de Perdas Técnicas - V " + complemento, this);
        }


        /// <summary>
        /// Muda o Titulo da Aplicacao
        /// </summary>
        /// <returns></returns>
        public void mudaTitApp()
        {
            string versao = VersaoSistema();

            mudaTitApp(versao);
        }

        /// <summary>
        /// Retorna VersaoSistema
        /// </summary>
        /// <returns>VersaoSistema</returns>
        public string VersaoSistema()
        {
            string version = "";

            try
            {
                version = System.Deployment.Application.ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                version = Assembly.GetEntryAssembly().GetName().Version.ToString();
            }

            return version;
        }

        private void defInfoVersoes()
        {
            rtbInfo.Text = Environment.NewLine +
                     "Versão V 1.0.0.10 de 2020-09-21:" + Environment.NewLine +
                     "  - Correção do limite de 30m em Ramais;" + Environment.NewLine +
                     "  - Entrada de Usuário e Senha para a conexão com o Banco de Dados." + Environment.NewLine + Environment.NewLine +
                     "Versão V 1.0.0.4 de 2019-03-23:" + Environment.NewLine +
                     "  - Todos itens da rodada selecionados por padrão;" + Environment.NewLine +
                     "  - Conexão automática de Banco ao executar funções;" + Environment.NewLine +
                     "  - Lista automática de instâncias e bancos;" + Environment.NewLine + Environment.NewLine +
                     "Versão 2018-12-13-V01" + Environment.NewLine +
                     " - Revisão das perda nos transformadores de distribuição em função da tabela de eficiência ABNT;" + Environment.NewLine +
                     "Versão 2018-12-06-V01" + Environment.NewLine +
                     " - Revisão do condutores de baixa tensão;" + Environment.NewLine +
                     "Versão 2017-12-21-V01" + Environment.NewLine +
                     " - Inclusão da tensão 44kV na captura das informações do Meter;" + Environment.NewLine +
                     "Versão 2017-11-13-V01" + Environment.NewLine +
                     " - Inclusão da tensão 11,95kV na captura das informações do Meter;" + Environment.NewLine +
                     "Versão 2017-10-02-V01" + Environment.NewLine +
                     " - Correção do parâmetro de perda cobre de transformadores;" + Environment.NewLine +
                     "Versão 2017-09-27-V01" + Environment.NewLine +
                     " - Inclusão da tensão 11,9kV na captura das informações do Meter;" + Environment.NewLine +
                     "Versão 2017-06-23-V01" + Environment.NewLine +
                     " - Inclusão da tensão 6,6kV na captura das informações do Meter;" + Environment.NewLine +
                     "Versão 2017-06-21-V01" + Environment.NewLine +
                     " - Inclusão da tensão 33kV na captura das informações do Meter;" + Environment.NewLine +
                     "Versão 2017-04-27-V01" + Environment.NewLine +
                     " - Ajuste das unidades de perda de regulador;" + Environment.NewLine +
                     "Versão 2016-11-25-V01" + Environment.NewLine +
                     " - Inclusão da tensão 6,3kV na captura das informações do Meter;" + Environment.NewLine +
                     "Versão 2016-07-20-V01" + Environment.NewLine +
                     " - Inclusão da tensão 24,2kV na captura das informações do Meter;" + Environment.NewLine +
                     "Versão 2016-05-24-V01" + Environment.NewLine +
                     " - Inclusão da tensão 11,8kV e 11kV na captura das informações do Meter;" + Environment.NewLine +
                     "Versão 2016-05-11-V01" + Environment.NewLine +
                     " - Inclusão das opções para neutralização de segmentos e transformadores de terceiros;" + Environment.NewLine +
                     "Versão 2016-05-02-V01" + Environment.NewLine +
                     " - Inclusão da tensão 13,2kV na captura das informações do Meter;" + Environment.NewLine +
                     "Versão 2016-04-28-V01" + Environment.NewLine +
                     " - Correções no processamento do balanço de energia;" + Environment.NewLine +
                     "Versão 2016-03-24-V01" + Environment.NewLine +
                     " - Inclusão da tensão 22kV na captura das informações do Meter;" + Environment.NewLine +
                     "Versão 2016-01-19-V01" + Environment.NewLine +
                     " - Revisão dos limites dos modelos de convergência;" + Environment.NewLine +
                     "Versão 2016-01-08-V01" + Environment.NewLine +
                     " - Implementação de comando de administração;" + Environment.NewLine +
                     "Versão 2015-12-23-V01" + Environment.NewLine +
                     " - Implementação do MRT nos transformadores;" + Environment.NewLine +
                     "Versão 2015-12-10-V01" + Environment.NewLine +
                     " - Implementação da escolha dos modelos de convergência;" + Environment.NewLine +
                     "Versão 2015-10-08-V01" + Environment.NewLine +
                     " - Aumento na quantidade máxima de itereações de 7 para 11;" + Environment.NewLine +
                     "Versão 2015-10-07-V01" + Environment.NewLine +
                     " - Inclusão de mensagem de verbose indicando o resultado da rodada (quantidade e diferenças de energia);" + Environment.NewLine +
                     "Versão 2015-09-28-V01" + Environment.NewLine +
                     " - Correção do limite do compimento máximo do ramal de 15 para 30 metros;" + Environment.NewLine +
                     " - Criação do parâmetro vmaxpu em 1.5;" + Environment.NewLine +
                     "Versão 2015-09-22-V01" + Environment.NewLine +
                     " - Correção do comando de tap em transformadores com comando de adição e retirada;" + Environment.NewLine +
                     " - Alteração do valor padrão da Tensão Mínima (pu) para 0,6;" + Environment.NewLine +
                     " - Inclusão de comando de escolha de 3 modelos de carga diferentes;" + Environment.NewLine +
                     "Versão 2015-09-21-V01" + Environment.NewLine +
                     " - Criação do comando de tap em transformadores;";
        }


    }

}
