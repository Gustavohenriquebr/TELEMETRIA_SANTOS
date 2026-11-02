using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PGPerdas
{

    class EnergyBalanceTools
    {

        public EnergyBalanceTools()
        {

            this.setInjectEnergy_MWh();
            this.setSupplyEnergy_Mwh();
            this.setSupplyEnergywithoutNet_MWh();
            this.setSupplyEnergywithNet_MWh();
            this.setTecnicalLossesFixedEnergy_MWh();
            this.setTecnicalLossesTransformersEnergy_MWh();
            this.setTransformer();
            this.setSegmentTransformation();
            this.setProportionTtoST();
            this.setProportionSTtoT();
            this.setEnergyTransformerOutLevelDownUp_MWh();
            this.setEnergyTransformerEnterLevelDownUp_MWh();
            this.setEnergyTransformerEnterLevelUpDown_MWh();
            this.setEnergyTransformerOutLevelUpDown_MWh();

        }

        // Controle de ativação do nível
        private int[] inIsActiveLevel = new int[5];

        private void setIsActiveLevel()
        {

            for (int i = 0; i < this.inIsActiveLevel.GetLength(0); i++)
            {
                if (this.dblInjectEnergy_MWh[i] != 0 || this.dblSupplyEnergy_MWh[i] != 0 || this.dblSupplyEnergywithoutNet_MWh[i] != 0 || this.dblTecnicalLossesFixedEnergy_MWh[i] != 0)
                {
                    this.inIsActiveLevel[i] = 1;
                    break;
                }
                else
                {
                    this.inIsActiveLevel[i] = 0;
                }
            }

        }

        // Energia injetada no nível
        private double[] dblInjectEnergy_MWh = new double[5];

        public void setInjectEnergy_MWh()
        {

            for (int i = 0; i < this.dblInjectEnergy_MWh.GetLength(0); i++)
            {
                this.dblInjectEnergy_MWh[i] = 0;
            }

        }

        public void addInjectEnergy_MWh(String strSegmet, double dblEnergy_MWh)
        {

            int i = EnergyBalanceTools.getLevelNumber(strSegmet);

            if (i != -1 && dblEnergy_MWh != 0)
            {
                this.inIsActiveLevel[i] = 1;
                this.dblInjectEnergy_MWh[i] += dblEnergy_MWh;
            }

        }

        // Energia fornecida no nível
        private double[] dblSupplyEnergy_MWh = new double[5];

        public void setSupplyEnergy_Mwh()
        {

            for (int i = 0; i < this.dblSupplyEnergy_MWh.GetLength(0); i++)
            {
                this.dblSupplyEnergy_MWh[i] = 0;
            }

        }

        public void addSupplyEnergy_MWh(String strSegmet, double dblEnergy_MWh)
        {

            int i = EnergyBalanceTools.getLevelNumber(strSegmet);

            if (i != -1 && dblEnergy_MWh != 0)
            {
                this.inIsActiveLevel[i] = 1;
                this.dblSupplyEnergy_MWh[i] += dblEnergy_MWh;
            }

        }

        // Energia fornecida sem rede associada
        private double[] dblSupplyEnergywithoutNet_MWh = new double[5];

        public void setSupplyEnergywithoutNet_MWh()
        {

            for (int i = 0; i < this.dblSupplyEnergywithoutNet_MWh.GetLength(0); i++)
            {
                this.dblSupplyEnergywithoutNet_MWh[i] = 0;
            }

        }

        public void addSupplyEnergywithoutNet_MWh(String strSegmet, double dblEnergy_MWh)
        {

            int i = EnergyBalanceTools.getLevelNumber(strSegmet);

            if (i != -1 && dblEnergy_MWh != 0)
            {
                this.inIsActiveLevel[i] = 1;
                this.dblSupplyEnergywithoutNet_MWh[i] += dblEnergy_MWh;
            }

        }

        // Energia fornecida com rede associada
        private double[] dblSupplyEnergywithNet_MWh = new double[5];

        private void setSupplyEnergywithNet_MWh()
        {

            for (int i = 0; i < this.dblSupplyEnergywithNet_MWh.GetLength(0); i++)
            {
                this.dblSupplyEnergywithNet_MWh[i] = this.dblSupplyEnergy_MWh[i] - this.dblSupplyEnergywithoutNet_MWh[i];
            }

        }

        // Energia do nível no balanço
        private double[] dblBalanceLevelEnergy_MWh = new double[5];

        private void setBalanceLevelEnergy_MWh()
        {

            for (int i = 0; i < this.dblBalanceLevelEnergy_MWh.GetLength(0); i++)
            {
                this.dblBalanceLevelEnergy_MWh[i] = this.dblInjectEnergy_MWh[i] - this.dblSupplyEnergywithoutNet_MWh[i] - this.dblEnergyTransformerOutLevelDownUp_MWh[i] + this.dblEnergyTransformerEnterLevelDownUp_MWh[i] + this.dblEnergyTransformerEnterLevelUpDown_MWh[i];
            }

        }

        private void setBalanceLevelEnergy_MWh(int intSegmet)
        {

            if (intSegmet != -1)
            {
                this.dblBalanceLevelEnergy_MWh[intSegmet] = this.dblInjectEnergy_MWh[intSegmet] - this.dblSupplyEnergywithoutNet_MWh[intSegmet] - this.dblEnergyTransformerOutLevelDownUp_MWh[intSegmet] + this.dblEnergyTransformerEnterLevelDownUp_MWh[intSegmet] + this.dblEnergyTransformerEnterLevelUpDown_MWh[intSegmet];
            }

        }

        // Fator de correção do nível
        private double[] dblCorretionFatorLevel_pu = new double[5];

        // Perdas de energia do nível não corrigível
        private double[] dblTecnicalLossesFixedEnergy_MWh = new double[5];

        public void setTecnicalLossesFixedEnergy_MWh()
        {

            for (int i = 0; i < this.dblTecnicalLossesFixedEnergy_MWh.GetLength(0); i++)
            {
                this.dblTecnicalLossesFixedEnergy_MWh[i] = 0;
            }

        }

        public void addTecnicalLossesFixedEnergy_MWh(String strSegmet, double dblTecnicalLosses_MWh)
        {

            int i = EnergyBalanceTools.getLevelNumber(strSegmet);

            if (i != -1 && dblTecnicalLosses_MWh != 0)
            {
                this.inIsActiveLevel[i] = 1;
                this.dblTecnicalLossesFixedEnergy_MWh[i] += dblTecnicalLosses_MWh;
            }

        }

        // Perdas de energia do nível relativa aos transformadores de potência
        private double[] dblTecnicalLossesTransformersEnergy_MWh = new double[5];

        private void setTecnicalLossesTransformersEnergy_MWh()
        {

            for (int i = 0; i < this.dblTecnicalLossesTransformersEnergy_MWh.GetLength(0); i++)
            {
                this.dblTecnicalLossesTransformersEnergy_MWh[i] = 0;
            }

        }

        private void setTecnicalLossesTransformersEnergy_MWh(int intSegmet)
        {

            if (intSegmet != -1)
            {
                this.dblTecnicalLossesTransformersEnergy_MWh[intSegmet] = 0;
                for (int i = 0; i < this.intPrimTransformer.Count; i++)
                {
                    if ((int)this.intAllocationTransformer[i] == intSegmet)
                    this.dblTecnicalLossesTransformersEnergy_MWh[intSegmet] += (double)this.dblIronLossesTransformer_MWh[i] + (double)this.dblCopperLossesCorrectedTransformer_MWh[i];
                }
            }

        }

        private double[] dblTecnicalLossesVariableBeforeEnergy_MWh = new double[5];
        private double[] dblTecnicalLossesVariableAfterEnergy_MWh = new double[5];
        private double[] dblTecnicalLossesMeterEnergy_MWh = new double[5];

        // Diferença do balanço
        private double[] dblDiferenceBalanceEnergy_MWh = new double[5];

        private void setDiferenceBalanceEnergy_MWh()
        {

            for (int i = 0; i < this.dblDiferenceBalanceEnergy_MWh.GetLength(0); i++)
            {
                this.dblDiferenceBalanceEnergy_MWh[i] = this.dblBalanceLevelEnergy_MWh[i] - (this.dblTecnicalLossesFixedEnergy_MWh[i] - this.dblTecnicalLossesTransformersEnergy_MWh[i]) - this.dblEnergyTransformerOutLevelUpDown_MWh[i] - this.dblSupplyEnergywithNet_MWh[i];
            }

        }

        private void setDiferenceBalanceEnergy_MWh(int intSegmet)
        {

            if (intSegmet != -1)
            {
                this.dblDiferenceBalanceEnergy_MWh[intSegmet] = this.dblBalanceLevelEnergy_MWh[intSegmet] - (this.dblTecnicalLossesFixedEnergy_MWh[intSegmet] - this.dblTecnicalLossesTransformersEnergy_MWh[intSegmet]) - this.dblEnergyTransformerOutLevelUpDown_MWh[intSegmet] - this.dblSupplyEnergywithNet_MWh[intSegmet];
            }

        }

        // Transformadores de potência
        private ArrayList intPrimTransformer = new ArrayList();
        private ArrayList intSecuTransformer = new ArrayList();
        private ArrayList intTercTransformer = new ArrayList();
        private ArrayList dblEnergyTransformer_MWh = new ArrayList();
        private ArrayList dblIronLossesTransformer_MWh = new ArrayList();
        private ArrayList dblCopperLossesTransformer_MWh = new ArrayList();
        private ArrayList intAllocationTransformer = new ArrayList();
        private ArrayList dblEnergyTransformerBalanceBefore_MWh = new ArrayList();
        private ArrayList dblProportionTransformer_pu = new ArrayList();
        private ArrayList dblEnergyTransformerBalanceAfter_MWh = new ArrayList(); 
        private ArrayList dblCorretionFatorTransformer_pu = new ArrayList();
        private ArrayList dblCopperLossesCorrectedTransformer_MWh = new ArrayList();
        private ArrayList dblEnergyTransformerResult_MWh = new ArrayList();
        private ArrayList intFlow = new ArrayList();

        public void setTransformer()
        {

            this.intPrimTransformer.Clear();
            this.intSecuTransformer.Clear();
            this.intTercTransformer.Clear();
            this.dblEnergyTransformer_MWh.Clear();
            this.dblIronLossesTransformer_MWh.Clear();
            this.dblCopperLossesTransformer_MWh.Clear();
            this.intAllocationTransformer.Clear();
            this.dblEnergyTransformerBalanceBefore_MWh.Clear();
            this.dblProportionTransformer_pu.Clear();
            this.dblEnergyTransformerBalanceAfter_MWh.Clear();
            this.dblCorretionFatorTransformer_pu.Clear();
            this.dblCopperLossesCorrectedTransformer_MWh.Clear();
            this.dblEnergyTransformerResult_MWh.Clear();
            this.intFlow.Clear();

        }

        public void addTransformer(String strPrimSegmet, String strSecuSegmet, String strTercSegmet, double dblEnergy_MWh, double dblIronLosses_MWh, double dblCopperLosses_MWh, String strAllocation, int intFlow)
        {

            if (this.intPrimTransformer.Count != 0)
            {
                for (int i = 0; i < this.intPrimTransformer.Count; i++)
                {
                    if (intFlow == 0)
                    {
                        if (EnergyBalanceTools.getLevelNumber(strPrimSegmet) == (int)this.intPrimTransformer[i] && EnergyBalanceTools.getLevelNumber(strSecuSegmet) == (int)this.intSecuTransformer[i] && EnergyBalanceTools.getLevelNumber(strTercSegmet) == (int)this.intTercTransformer[i] && EnergyBalanceTools.getLevelNumber(strAllocation) == (int)this.intAllocationTransformer[i] && (dblEnergy_MWh != 0 || dblIronLosses_MWh != 0 || dblCopperLosses_MWh != 0))
                        {
                            this.dblEnergyTransformer_MWh[i] = (double)this.dblEnergyTransformer_MWh[i] + dblEnergy_MWh;
                            this.dblIronLossesTransformer_MWh[i] = (double)this.dblIronLossesTransformer_MWh[i] + dblIronLosses_MWh;
                            this.dblCopperLossesTransformer_MWh[i] = (double)this.dblCopperLossesTransformer_MWh[i] + dblCopperLosses_MWh;
                            return;
                        }
                    }
                    else
                    {
                        if (EnergyBalanceTools.getLevelNumber(strSecuSegmet) == (int)this.intPrimTransformer[i] && EnergyBalanceTools.getLevelNumber(strPrimSegmet) == (int)this.intSecuTransformer[i] && EnergyBalanceTools.getLevelNumber(strTercSegmet) == (int)this.intTercTransformer[i] && EnergyBalanceTools.getLevelNumber(strAllocation) == (int)this.intAllocationTransformer[i] && (dblEnergy_MWh != 0 || dblIronLosses_MWh != 0 || dblCopperLosses_MWh != 0))
                        {
                            this.dblEnergyTransformer_MWh[i] = (double)this.dblEnergyTransformer_MWh[i] + dblEnergy_MWh;
                            this.dblIronLossesTransformer_MWh[i] = (double)this.dblIronLossesTransformer_MWh[i] + dblIronLosses_MWh;
                            this.dblCopperLossesTransformer_MWh[i] = (double)this.dblCopperLossesTransformer_MWh[i] + dblCopperLosses_MWh;
                            return;
                        }
                    }
                }
            }
            if (dblEnergy_MWh != 0 || dblIronLosses_MWh != 0 || dblCopperLosses_MWh != 0)
            {
                if (intFlow == 0)
                {
                    this.intPrimTransformer.Add(EnergyBalanceTools.getLevelNumber(strPrimSegmet));
                    this.intSecuTransformer.Add(EnergyBalanceTools.getLevelNumber(strSecuSegmet));
                }
                else
                {
                    this.intPrimTransformer.Add(EnergyBalanceTools.getLevelNumber(strSecuSegmet));
                    this.intSecuTransformer.Add(EnergyBalanceTools.getLevelNumber(strPrimSegmet));
                }
                this.intTercTransformer.Add(EnergyBalanceTools.getLevelNumber(strTercSegmet));
                this.dblEnergyTransformer_MWh.Add(dblEnergy_MWh);
                this.dblIronLossesTransformer_MWh.Add(dblIronLosses_MWh);
                this.dblCopperLossesTransformer_MWh.Add(dblCopperLosses_MWh);
                this.intAllocationTransformer.Add(EnergyBalanceTools.getLevelNumber(strAllocation));
                this.dblEnergyTransformerBalanceBefore_MWh.Add(0.0);
                this.dblProportionTransformer_pu.Add(0.0);
                this.dblEnergyTransformerBalanceAfter_MWh.Add(0.0);
                this.dblCorretionFatorTransformer_pu.Add(0.0);
                this.dblCopperLossesCorrectedTransformer_MWh.Add(0.0);
                this.dblEnergyTransformerResult_MWh.Add(0.0);
                this.intFlow.Add(intFlow);
            }

        }

        private void updateProportionTransformer()
        {

            double dblEnergy_MWh = 0;
            for (int i = 0; i < this.intPrimTransformer.Count; i++)
            {
                dblEnergy_MWh = 0;
                for (int j = 0; j < this.intPrimTransformer.Count; j++)
                {
                    if ((int)this.intPrimTransformer[i] == (int)this.intPrimTransformer[j] && (int)this.intSecuTransformer[i] == (int)this.intSecuTransformer[j] && (int)this.intTercTransformer[i] == (int)this.intTercTransformer[j])
                    {
                        dblEnergy_MWh += (double)this.dblEnergyTransformer_MWh[j];
                    }
                }
                this.dblProportionTransformer_pu[i] = (double)this.dblEnergyTransformer_MWh[i] / dblEnergy_MWh;
            }

        }

        private void updateTransformerBalance()
        {

            for (int i = 0; i < this.intPrimTransformer.Count; i++)
            {
                double dblEnergy_MWh = 0;
                this.dblEnergyTransformerBalanceBefore_MWh[i] = dblEnergy_MWh;
                for (int j = 0; j < this.intPrimSegmentTransformation.Count; j++)
                {
                    for (int k = 0; k < this.intPrimSegmentTransformationSTtoT.Count; k++)
                    {
                        if ((int)this.intPrimTransformer[i] == (int)this.intPrimTransformerSTtoT[k] && (int)this.intSecuTransformer[i] == (int)this.intSecuTransformerSTtoT[k] && (int)this.intTercTransformer[i] == (int)this.intTercTransformerSTtoT[k] && (int)this.intPrimSegmentTransformation[j] == (int)this.intPrimSegmentTransformationSTtoT[k] && (int)this.intSecuSegmentTransformation[j] == (int)this.intSecuSegmentTransformationSTtoT[k])
                        {
                            dblEnergy_MWh += (double)this.dblEnergySegmentTransformationBalanceAfter_MWh[j] * (double)this.dblProportionSTtoT_pu[k];
                        }
                    }
                }
                this.dblEnergyTransformerBalanceBefore_MWh[i] = dblEnergy_MWh;
                this.dblEnergyTransformerBalanceAfter_MWh[i] = dblEnergy_MWh * (double)this.dblProportionTransformer_pu[i];
                //this.dblCorretionFatorTransformer_pu[i] = dblEnergy_MWh / (double)this.dblEnergyTransformer_MWh[i];
                this.dblCorretionFatorTransformer_pu[i] = (double)this.dblEnergyTransformerBalanceAfter_MWh[i] / (double)this.dblEnergyTransformer_MWh[i];
                this.dblCopperLossesCorrectedTransformer_MWh[i] = (double)this.dblCopperLossesTransformer_MWh[i] * Math.Pow((double)this.dblCorretionFatorTransformer_pu[i], 2);
                this.dblEnergyTransformerResult_MWh[i] = (double)this.dblEnergyTransformerBalanceBefore_MWh[i] - (double)this.dblIronLossesTransformer_MWh[i] - (double)this.dblCopperLossesCorrectedTransformer_MWh[i];
            }

        }

        // Energias transformadas
        private ArrayList intPrimSegmentTransformation = new ArrayList();
        private ArrayList intSecuSegmentTransformation = new ArrayList();
        private ArrayList dblEnergySegmentTransformation_MWh = new ArrayList();
        private ArrayList dblEnergySegmentTransformationBalanceBefore_MWh = new ArrayList();
        private ArrayList dblProportionSegmentTransformation_pu = new ArrayList();
        private ArrayList dblEnergySegmentTransformationBalanceAfter_MWh = new ArrayList();
        private ArrayList dblIronLossesSegmentTransformation_MWh = new ArrayList();
        private ArrayList dblCopperLossesSegmentTransformation_MWh = new ArrayList();
        private ArrayList dblEnergySegmentTransformationResult_MWh = new ArrayList();

        public void setSegmentTransformation()
        {

            this.intPrimSegmentTransformation.Clear();
            this.intSecuSegmentTransformation.Clear();
            this.dblEnergySegmentTransformation_MWh.Clear();
            this.dblEnergySegmentTransformationBalanceBefore_MWh.Clear();
            this.dblProportionSegmentTransformation_pu.Clear();
            this.dblEnergySegmentTransformationBalanceAfter_MWh.Clear();
            this.dblIronLossesSegmentTransformation_MWh.Clear();
            this.dblCopperLossesSegmentTransformation_MWh.Clear();
            this.dblEnergySegmentTransformationResult_MWh.Clear();

        }

        public void addSegmentTransformation(String strPrimSegmet, String strSecuSegmet, double dblEnergy_MWh)
        {

            if (this.intPrimSegmentTransformation.Count != 0)
            {
                for (int i = 0; i < this.intPrimSegmentTransformation.Count; i++)
                {
                    if (EnergyBalanceTools.getLevelNumber(strPrimSegmet) == (int)this.intPrimSegmentTransformation[i] && EnergyBalanceTools.getLevelNumber(strSecuSegmet) == (int)this.intSecuSegmentTransformation[i] && dblEnergy_MWh != 0)
                    {
                        this.dblEnergySegmentTransformation_MWh[i] = (double)this.dblEnergySegmentTransformation_MWh[i] + dblEnergy_MWh;
                        return;
                    }
                }
            }
            if (dblEnergy_MWh != 0)
            {
                this.intPrimSegmentTransformation.Add(EnergyBalanceTools.getLevelNumber(strPrimSegmet));
                this.intSecuSegmentTransformation.Add(EnergyBalanceTools.getLevelNumber(strSecuSegmet));
                this.dblEnergySegmentTransformation_MWh.Add(dblEnergy_MWh);
                this.dblEnergySegmentTransformationBalanceBefore_MWh.Add(0.0);
                this.dblProportionSegmentTransformation_pu.Add(0.0);
                this.dblEnergySegmentTransformationBalanceAfter_MWh.Add(0.0);
                this.dblIronLossesSegmentTransformation_MWh.Add(0.0);
                this.dblCopperLossesSegmentTransformation_MWh.Add(0.0);
                this.dblEnergySegmentTransformationResult_MWh.Add(0.0);
            }

        }

        private void updateProportionSegmentTransformation()
        {

            for (int i = 0; i < this.intPrimSegmentTransformation.Count; i++)
            {
                for (int j = 0; j < this.intPrimSegmentTransformation.Count; j++)
                {
                    if ((int)intPrimSegmentTransformation[i] == (int)intPrimSegmentTransformation[j] && (int)this.intPrimSegmentTransformation[j] < (int)this.intSecuSegmentTransformation[j])
                    {
                        this.dblProportionSegmentTransformation_pu[i] = (double)this.dblProportionSegmentTransformation_pu[i] + (double)this.dblEnergySegmentTransformation_MWh[j];
                    }
                }
            }
            for (int i = 0; i < this.intPrimSegmentTransformation.Count; i++)
            {
                this.dblProportionSegmentTransformation_pu[i] = (double)this.dblEnergySegmentTransformation_MWh[i] / (double)this.dblProportionSegmentTransformation_pu[i];
                if ((int)this.intPrimSegmentTransformation[i] >= (int)this.intSecuSegmentTransformation[i])
                {
                    this.dblProportionSegmentTransformation_pu[i] = (double)1;
                }
            }

        }

        private void updateEnergySegmentTransformationBalance()
        {

            for (int i = 0; i < this.intPrimSegmentTransformation.Count; i++)
            {
                if ((int)this.intPrimSegmentTransformation[i] < (int)this.intSecuSegmentTransformation[i])
                {
                    for (int j = 0; j < this.inIsActiveLevel.GetLength(0); j++)
                    {
                        if (this.inIsActiveLevel[j] == 1 && (int)this.intPrimSegmentTransformation[i] == j)
                        {
                            this.dblEnergySegmentTransformationBalanceBefore_MWh[i] = this.dblEnergyTransformerOutLevelUpDown_MWh[j];
                        }
                    }
                }
                else if ((int)this.intPrimSegmentTransformation[i] >= (int)this.intSecuSegmentTransformation[i])
                {
                    this.dblEnergySegmentTransformationBalanceBefore_MWh[i] = (double)this.dblEnergySegmentTransformation_MWh[i];
                }
            }

            for (int i = 0; i < this.intPrimSegmentTransformation.Count; i++)
            {
                this.dblEnergySegmentTransformationBalanceAfter_MWh[i] = (double)this.dblEnergySegmentTransformationBalanceBefore_MWh[i] * (double)this.dblProportionSegmentTransformation_pu[i];
            }
/*
                        for (int i = 0; i < this.intPrimSegmentTransformation.Count; i++)
                        {
                            double dblEnergy_MWh = 0;
                            this.dblEnergySegmentTransformationBalanceAfter_MWh[i] = dblEnergy_MWh;
                            for (int j = 0; j < this.intPrimSegmentTransformation.Count; j++)
                            {
                                if ((int)this.intPrimSegmentTransformation[i] == (int)this.intPrimSegmentTransformation[j] && (int)this.intSecuSegmentTransformation[i] == (int)this.intSecuSegmentTransformation[j])
                                {
                                    dblEnergy_MWh += this.dblEnergyTransformerOutLevelUpDown_MWh[(int)this.intPrimSegmentTransformation[i]] * (double)this.dblProportionSegmentTransformation_pu[j];
                                }
                            }
                            this.dblEnergySegmentTransformationBalanceAfter_MWh[i] = dblEnergy_MWh;
                        }
            */
        }

        private void updateSegmentTransformationBalance()
        {

            for (int i = 0; i < this.intPrimSegmentTransformation.Count; i++)
            {
                double dblIronLossesEnergy_MWh = 0;
                double dblCopperLossesEnergy_MWh = 0;
                this.dblIronLossesSegmentTransformation_MWh[i] = dblIronLossesEnergy_MWh;
                this.dblCopperLossesSegmentTransformation_MWh[i] = dblCopperLossesEnergy_MWh;
                for (int j = 0; j < this.intPrimTransformer.Count; j++)
                {
                    for (int k = 0; k < this.intPrimSegmentTransformationTtoST.Count; k++)
                    {
                        if ((int)this.intPrimTransformer[j] == (int)this.intPrimTransformerTtoST[k] && (int)this.intSecuTransformer[j] == (int)this.intSecuTransformerTtoST[k] && (int)this.intTercTransformer[j] == (int)this.intTercTransformerTtoST[k] && (int)this.intPrimSegmentTransformation[i] == (int)this.intPrimSegmentTransformationTtoST[k] && (int)this.intSecuSegmentTransformation[i] == (int)this.intSecuSegmentTransformationTtoST[k])
                        {
                            dblIronLossesEnergy_MWh += (double)this.dblIronLossesTransformer_MWh[j] * (double)this.dblProportionTtoST_pu[k];
                            dblCopperLossesEnergy_MWh += (double)this.dblCopperLossesCorrectedTransformer_MWh[j] * (double)this.dblProportionTtoST_pu[k];
                        }
                    }
                }
                this.dblIronLossesSegmentTransformation_MWh[i] = dblIronLossesEnergy_MWh;
                this.dblCopperLossesSegmentTransformation_MWh[i] = dblCopperLossesEnergy_MWh;
                this.dblEnergySegmentTransformationResult_MWh[i] = (double)this.dblEnergySegmentTransformationBalanceAfter_MWh[i] - dblIronLossesEnergy_MWh - dblCopperLossesEnergy_MWh;
            }

        }

        // Proporções que transformam dados de transformadores em energia transformadas
        private ArrayList intPrimTransformerTtoST = new ArrayList();
        private ArrayList intSecuTransformerTtoST = new ArrayList();
        private ArrayList intTercTransformerTtoST = new ArrayList();
        private ArrayList intPrimSegmentTransformationTtoST = new ArrayList();
        private ArrayList intSecuSegmentTransformationTtoST = new ArrayList();
        private ArrayList dblEnergyTtoST_MWh = new ArrayList();
        private ArrayList dblEnergyTotalTtoST_MWh = new ArrayList();
        private ArrayList dblProportionTtoST_pu = new ArrayList();

        public void setProportionTtoST()
        {

            this.intPrimTransformerTtoST.Clear();
            this.intSecuTransformerTtoST.Clear();
            this.intTercTransformerTtoST.Clear();
            this.intPrimSegmentTransformationTtoST.Clear();
            this.intSecuSegmentTransformationTtoST.Clear();
            this.dblEnergyTtoST_MWh.Clear();
            this.dblEnergyTotalTtoST_MWh.Clear();
            this.dblProportionTtoST_pu.Clear();

        }

        public void addProportionTtoST(String strPrimSegmetT, String strSecuSegmetT, String strTercSegmetT, String strPrimSegmetET, String strSecuSegmetET, double dblEnergyET_MWh)
        {

            if (this.intPrimTransformerTtoST.Count != 0)
            {
                for (int i = 0; i < this.intPrimTransformerTtoST.Count; i++)
                {
                    if (EnergyBalanceTools.getLevelNumber(strPrimSegmetT) == (int)this.intPrimTransformerTtoST[i] && EnergyBalanceTools.getLevelNumber(strSecuSegmetT) == (int)this.intSecuTransformerTtoST[i] && EnergyBalanceTools.getLevelNumber(strTercSegmetT) == (int)this.intTercTransformerTtoST[i] && EnergyBalanceTools.getLevelNumber(strPrimSegmetET) == (int)this.intPrimSegmentTransformationTtoST[i] && EnergyBalanceTools.getLevelNumber(strSecuSegmetET) == (int)this.intSecuSegmentTransformationTtoST[i] && dblEnergyET_MWh != 0)
                    {
                        this.dblEnergyTtoST_MWh[i] = (double)this.dblEnergyTtoST_MWh[i] + dblEnergyET_MWh;
                        this.dblEnergyTotalTtoST_MWh[i] = 0.0;
                        this.dblProportionTtoST_pu[i] = 0.0;
                        return;
                    }
                }
            }
            if (dblEnergyET_MWh != 0)
            {
                this.intPrimTransformerTtoST.Add(EnergyBalanceTools.getLevelNumber(strPrimSegmetT));
                this.intSecuTransformerTtoST.Add(EnergyBalanceTools.getLevelNumber(strSecuSegmetT));
                this.intTercTransformerTtoST.Add(EnergyBalanceTools.getLevelNumber(strTercSegmetT));
                this.intPrimSegmentTransformationTtoST.Add(EnergyBalanceTools.getLevelNumber(strPrimSegmetET));
                this.intSecuSegmentTransformationTtoST.Add(EnergyBalanceTools.getLevelNumber(strSecuSegmetET));
                this.dblEnergyTtoST_MWh.Add(dblEnergyET_MWh);
                this.dblEnergyTotalTtoST_MWh.Add(0.0);
                this.dblProportionTtoST_pu.Add(0.0);
            }

        }

        private void updateEnergyTotalTtoST()
        {

            for (int i = 0; i < this.intPrimTransformerTtoST.Count; i++)
            {
                for (int j = 0; j < this.intPrimTransformerTtoST.Count; j++)
                {
                    if ((int)intPrimTransformerTtoST[i] == (int)intPrimTransformerTtoST[j] && (int)intSecuTransformerTtoST[i] == (int)intSecuTransformerTtoST[j] && (int)intTercTransformerTtoST[i] == (int)intTercTransformerTtoST[j])
                    {
                        this.dblEnergyTotalTtoST_MWh[i] = (double)this.dblEnergyTotalTtoST_MWh[i] + (double)this.dblEnergyTtoST_MWh[j];
                    }
                }
            }
            this.updateProportionTtoST();

        }

        private void updateProportionTtoST()
        {

            for (int i = 0; i < this.intPrimTransformerTtoST.Count; i++)
            {
                this.dblProportionTtoST_pu[i] = (double)this.dblEnergyTtoST_MWh[i] / (double)this.dblEnergyTotalTtoST_MWh[i];
            }

        }

        // Proporções que transformam dados de energia transformadas em transformadores
        private ArrayList intPrimSegmentTransformationSTtoT = new ArrayList();
        private ArrayList intSecuSegmentTransformationSTtoT = new ArrayList();
        private ArrayList intPrimTransformerSTtoT = new ArrayList();
        private ArrayList intSecuTransformerSTtoT = new ArrayList();
        private ArrayList intTercTransformerSTtoT = new ArrayList();
        private ArrayList dblEnergySTtoT_MWh = new ArrayList();
        private ArrayList dblEnergyTotalSTtoT_MWh = new ArrayList();
        private ArrayList dblProportionSTtoT_pu = new ArrayList();

        public void setProportionSTtoT()
        {

            this.intPrimTransformerSTtoT.Clear();
            this.intSecuTransformerSTtoT.Clear();
            this.intTercTransformerSTtoT.Clear();
            this.intPrimSegmentTransformationSTtoT.Clear();
            this.intSecuSegmentTransformationSTtoT.Clear();
            this.dblEnergySTtoT_MWh.Clear();
            this.dblEnergyTotalSTtoT_MWh.Clear();
            this.dblProportionSTtoT_pu.Clear();

        }

        public void addProportionSTtoT(String strPrimSegmetET, String strSecuSegmetET, String strPrimSegmetT, String strSecuSegmetT, String strTercSegmetT, double dblEnergyT_MWh)
        {

            if (this.intPrimSegmentTransformationSTtoT.Count != 0)
            {
                for (int i = 0; i < this.intPrimSegmentTransformationSTtoT.Count; i++)
                {
                    if (EnergyBalanceTools.getLevelNumber(strPrimSegmetET) == (int)this.intPrimSegmentTransformationSTtoT[i] && EnergyBalanceTools.getLevelNumber(strSecuSegmetET) == (int)this.intSecuSegmentTransformationSTtoT[i] && EnergyBalanceTools.getLevelNumber(strPrimSegmetT) == (int)this.intPrimTransformerSTtoT[i] && EnergyBalanceTools.getLevelNumber(strSecuSegmetT) == (int)this.intSecuTransformerSTtoT[i] && EnergyBalanceTools.getLevelNumber(strTercSegmetT) == (int)this.intTercTransformerSTtoT[i] && dblEnergyT_MWh != 0)
                    {
                        this.dblEnergySTtoT_MWh[i] = ((double)this.dblEnergySTtoT_MWh[i]) + dblEnergyT_MWh;
                        this.dblEnergyTotalSTtoT_MWh[i] = 0.0;
                        return;
                    }
                }
            }
            if (dblEnergyT_MWh != 0)
            {
                this.intPrimSegmentTransformationSTtoT.Add(EnergyBalanceTools.getLevelNumber(strPrimSegmetET));
                this.intSecuSegmentTransformationSTtoT.Add(EnergyBalanceTools.getLevelNumber(strSecuSegmetET));
                this.intPrimTransformerSTtoT.Add(EnergyBalanceTools.getLevelNumber(strPrimSegmetT));
                this.intSecuTransformerSTtoT.Add(EnergyBalanceTools.getLevelNumber(strSecuSegmetT));
                this.intTercTransformerSTtoT.Add(EnergyBalanceTools.getLevelNumber(strTercSegmetT));
                this.dblEnergySTtoT_MWh.Add(dblEnergyT_MWh);
                this.dblEnergyTotalSTtoT_MWh.Add(0.0);
                this.dblProportionSTtoT_pu.Add(0.0);
            }

        }

        private void updateEnergyTotalSTtoT()
        {

            for (int i = 0; i < this.intPrimSegmentTransformationSTtoT.Count; i++)
            {
                for (int j = 0; j < this.intPrimSegmentTransformationSTtoT.Count; j++)
                {
                    if ((int)intPrimSegmentTransformationSTtoT[i] == (int)intPrimSegmentTransformationSTtoT[j] && (int)intSecuSegmentTransformationSTtoT[i] == (int)intSecuSegmentTransformationSTtoT[j])
                    {
                        this.dblEnergyTotalSTtoT_MWh[i] = (double)this.dblEnergyTotalSTtoT_MWh[i] + (double)this.dblEnergySTtoT_MWh[j];
                    }
                }
            }
            this.updateProportionSTtoT();

        }

        private void updateProportionSTtoT()
        {

            for (int i = 0; i < this.intPrimSegmentTransformationSTtoT.Count; i++)
            {
                this.dblProportionSTtoT_pu[i] = (double)this.dblEnergySTtoT_MWh[i] / (double)this.dblEnergyTotalSTtoT_MWh[i];
            }

        }

        // Energia de transformadores saindo do nível destinado a nível superior
        private double[] dblEnergyTransformerOutLevelDownUp_MWh = new double[5];

        private void setEnergyTransformerOutLevelDownUp_MWh()
        {

            for (int i = 0; i < this.dblEnergyTransformerOutLevelDownUp_MWh.GetLength(0); i++)
            {
                this.dblEnergyTransformerOutLevelDownUp_MWh[i] = 0;
            }

        }

        private void setEnergyTransformerOutLevelDownUp_MWh(int intSegmet)
        {

            if (intSegmet != -1)
            {
                this.dblEnergyTransformerOutLevelDownUp_MWh[intSegmet] = 0.0;
                for (int i = 0; i < this.intPrimSegmentTransformation.Count; i++)
                {
                    if (intSegmet == (int)this.intPrimSegmentTransformation[i] && (int)this.intPrimSegmentTransformation[i] > (int)this.intSecuSegmentTransformation[i])
                    {
                        this.dblEnergyTransformerOutLevelDownUp_MWh[intSegmet] = this.dblEnergyTransformerOutLevelDownUp_MWh[intSegmet] + (double)this.dblEnergySegmentTransformationBalanceAfter_MWh[i];
                    }
                }
            }

        }

        // Energia de transformadores entrando no nível proveniente de nível inferior
        private double[] dblEnergyTransformerEnterLevelDownUp_MWh = new double[5];

        private void setEnergyTransformerEnterLevelDownUp_MWh()
        {

            for (int i = 0; i < this.dblEnergyTransformerEnterLevelDownUp_MWh.GetLength(0); i++)
            {
                this.dblEnergyTransformerEnterLevelDownUp_MWh[i] = 0;
            }

        }

        private void setEnergyTransformerEnterLevelDownUp_MWh(int intSegmet)
        {

            if (intSegmet != -1)
            {
                this.dblEnergyTransformerEnterLevelDownUp_MWh[intSegmet] = 0.0;
                for (int i = 0; i < this.intPrimSegmentTransformation.Count; i++)
                {
                    if (intSegmet == (int)this.intSecuSegmentTransformation[i] && (int)this.intPrimSegmentTransformation[i] > (int)this.intSecuSegmentTransformation[i])
                    {
                        this.dblEnergyTransformerEnterLevelDownUp_MWh[intSegmet] = this.dblEnergyTransformerEnterLevelDownUp_MWh[intSegmet] + (double)this.dblEnergySegmentTransformationResult_MWh[i];
                    }
                }
            }

        }

        // Energia de transformadores entrando no nível proveniente de nível superior
        private double[] dblEnergyTransformerEnterLevelUpDown_MWh = new double[5];

        private void setEnergyTransformerEnterLevelUpDown_MWh()
        {

            for (int i = 0; i < this.dblEnergyTransformerEnterLevelUpDown_MWh.GetLength(0); i++)
            {
                this.dblEnergyTransformerEnterLevelUpDown_MWh[i] = 0;
            }

        }

        private void setEnergyTransformerEnterLevelUpDown_MWh(int intSegmet)
        {

            if (intSegmet != -1)
            {
                this.dblEnergyTransformerEnterLevelUpDown_MWh[intSegmet] = 0.0;
                for (int i = 0; i < this.intPrimSegmentTransformation.Count; i++)
                {
                    if (intSegmet == (int)this.intSecuSegmentTransformation[i] && (int)this.intPrimSegmentTransformation[i] < (int)this.intSecuSegmentTransformation[i])
                    {
                        this.dblEnergyTransformerEnterLevelUpDown_MWh[intSegmet] = this.dblEnergyTransformerEnterLevelUpDown_MWh[intSegmet] + (double)this.dblEnergySegmentTransformationResult_MWh[i];
                    }
                }
            }

        }

        // Energia de transformadores saindo do nível destinado a nível inferior
        private double[] dblEnergyTransformerOutLevelUpDown_MWh = new double[5];

        private void setEnergyTransformerOutLevelUpDown_MWh()
        {

            for (int i = 0; i < this.dblEnergyTransformerOutLevelUpDown_MWh.GetLength(0); i++)
            {
                this.dblEnergyTransformerOutLevelUpDown_MWh[i] = 0;
            }

        }

        private void setEnergyTransformerOutLevelUpDown_MWh(int intSegmet)
        {

            if (intSegmet != -1)
            {
                this.dblEnergyTransformerOutLevelUpDown_MWh[intSegmet] = this.dblBalanceLevelEnergy_MWh[intSegmet] - (this.dblTecnicalLossesFixedEnergy_MWh[intSegmet] - this.dblTecnicalLossesTransformersEnergy_MWh[intSegmet]) - this.dblSupplyEnergywithNet_MWh[intSegmet];
            }

        }
        
        private void setProportion()
        {
            /*
            for (int i = 0; i < this.strPrimTransformer.Count; i++)
            {
                for (int j = 0; j < this.strPrimSegmentTransformation.Count; j++)
                {
                    if ((strPrimTransformer[i].ToString() == strPrimSegmentTransformation[j].ToString() && strSecuTransformer[i].ToString() == strSecuSegmentTransformation[j].ToString()) || (strPrimTransformer[i].ToString() == strPrimSegmentTransformation[j].ToString() && strTercTransformer[i].ToString() == strSecuSegmentTransformation[j].ToString()))
                    {
                        this.strPrimTransformerTtoST.Add(strPrimTransformer[i]);
                        this.strSecuTransformerTtoST.Add(strSecuTransformer[i]);
                        this.strTercTransformerTtoST.Add(strTercTransformer[i]);
                        this.strPrimSegmentTransformationTtoST.Add(strPrimSegmentTransformation[j]);
                        this.strSecuSegmentTransformationTtoST.Add(strSecuSegmentTransformation[j]);
                        this.dblProportionTtoST_pu.Add((double[])this.dblEnergySegmentTransformation_MWh[j]);
                    }
                }
            }

            ArrayList strPrimST = new ArrayList();
            ArrayList dblEnergy_MWh = new ArrayList();
            for (int i = 0; i < this.strPrimSegmentTransformationTtoST.Count; i++)
            {
                if (!strPrimST.Contains(this.strPrimSegmentTransformationTtoST[i]))
                {
                    strPrimST.Add(this.strPrimSegmentTransformationTtoST[i]);
                    dblEnergy_MWh.Add((double[])this.dblProportionTtoST_pu[i]);
                }
                else
                {
                    int index = strPrimST.IndexOf(this.strPrimSegmentTransformationTtoST[i]);
                    double[] values1 = (double[])dblEnergy_MWh[index];
                    double[] values2 = (double[])this.dblProportionTtoST_pu[i];
                    double[] values3 = {0,0,0,0,0,0,0,0,0,0,0,0};
                    for (int j = 0; j <= 11; j++)
                        values3[j] = values1[j]+values2[j];
                    dblEnergy_MWh[index] = values3;
                }
            }

            for (int i = 0; i < this.strPrimSegmentTransformationTtoST.Count; i++)
            {
                int index = strPrimST.IndexOf(this.strPrimSegmentTransformationTtoST[i]);
                double[] values1 = (double[])this.dblProportionTtoST_pu[i];
                double[] values2 = (double[])dblEnergy_MWh[index];
                double[] values3 = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                for (int j = 0; j <= 11; j++)
                    values3[j] = values1[j] / values2[j];
                this.dblProportionTtoST_pu[i] = values3;
            }
            */
        }

        public void runBalance()
        {
            
            // Verifica que níveis estão ativos
            this.setIsActiveLevel();

            // Calcula as proporções da tabela de energias transformadas
            this.updateProportionSegmentTransformation();

            // Calcula as energias totais e proporções dos Transformadores para Energias Transformadas
            this.updateEnergyTotalTtoST();

            // Calcula as energias totais e proporções das Energias Transformadas para Transformadores
            this.updateEnergyTotalSTtoT();

            // Calcula a energia fornecida ao consumidores que transita na rede
            this.setSupplyEnergywithNet_MWh();

            // Calcula a energia passante do balanço nos níveis
            this.setBalanceLevelEnergy_MWh();

            // Calcula a energia que deverá sair do nível para baixo
            this.setEnergyTransformerOutLevelUpDown_MWh();

            // Calcula a energia que entra nos segmentos de transformação proveniente do balanço
            this.updateEnergySegmentTransformationBalance();

            // Calcula a diferença dos níveis
            this.setDiferenceBalanceEnergy_MWh();
            
            // Não tem mais utilidade
            //this.setProportion();

            for (int i = 0; i <= 100; i++)
            {
                for (int j = 0; j <= 4; j++)
                {
                    if (this.inIsActiveLevel[j] == 1)
                    {

                        this.setEnergyTransformerOutLevelDownUp_MWh(j);
                        this.setEnergyTransformerEnterLevelDownUp_MWh(j);
                        this.setTecnicalLossesTransformersEnergy_MWh(j);
                        this.setBalanceLevelEnergy_MWh(j);
                        this.setEnergyTransformerOutLevelUpDown_MWh(j);
                        this.updateEnergySegmentTransformationBalance();
                        this.setDiferenceBalanceEnergy_MWh(j);
                        this.updateProportionTransformer();
                        this.updateTransformerBalance();
                        this.updateSegmentTransformationBalance();
                        this.setEnergyTransformerEnterLevelUpDown_MWh(j);

                    }
                }
            }

        }

        public static int getLevelNumber(String strSegment)
        {

            int intLevel = -1;

            if (strSegment == "A1")
                intLevel = 0;
            else if (strSegment == "A2")
                intLevel = 1;
            else if (strSegment == "A3")
                intLevel = 2;
            else if (strSegment == "A3a")
                intLevel = 3;
            else if (strSegment == "A4")
                intLevel = 3;
            else if (strSegment == "MT")
                intLevel = 3;
            else if (strSegment == "B")
                intLevel = 4;

            return intLevel;

        }

        public static String getLevelSegment(int intSegment)
        {

            String strLevel = "NA";

            if (intSegment == 0)
                strLevel = "A1";
            else if (intSegment == 1)
                strLevel = "A2";
            else if (intSegment == 2)
                strLevel = "A3";
            else if (intSegment == 3)
                strLevel = "MT";
            else if (intSegment == 4)
                strLevel = "B";

            return strLevel;

        }

        public String ToString()
        {

            String strResult = "";

            strResult += "Segmento\tEnergia Injetada (MWh)\tEnergia sem Rede Associada (MWh)\tEnergia Proveniente de Níveis Inferiores (MWh)\tEnergia Destinada a Níveis Superiores (MWh)\tEnergia Proveniente de Níveis Superiores (MWh)\tEnergia Base do Nível (MWh)\tPerda Técnica de Energia Não Corrigível (MWh)\tPerda Técnica de Energia Relativa aos Transformadores (MWh)\tEnergia Destinada a Níveis Inferiores (MWh)\tEnergia Fornecida Descontada Energia sem Rede Associada (MWh)\tDifença (MWh)" + Environment.NewLine;
            for (int i = 0; i <= 4; i++)
            {
                if (this.inIsActiveLevel[i] == 1)
                {
                    strResult += EnergyBalanceTools.getLevelSegment(i);
                    strResult += "\t";
                    strResult += (this.dblInjectEnergy_MWh[i]).ToString();
                    strResult += "\t";
                    strResult += (this.dblSupplyEnergywithoutNet_MWh[i]).ToString();
                    strResult += "\t";
                    strResult += (this.dblEnergyTransformerEnterLevelDownUp_MWh[i]).ToString();
                    strResult += "\t";
                    strResult += (this.dblEnergyTransformerOutLevelDownUp_MWh[i]).ToString();
                    strResult += "\t";
                    strResult += (this.dblEnergyTransformerEnterLevelUpDown_MWh[i]).ToString();
                    strResult += "\t";
                    strResult += (this.dblBalanceLevelEnergy_MWh[i]).ToString();
                    strResult += "\t";
                    strResult += (this.dblTecnicalLossesFixedEnergy_MWh[i]).ToString();
                    strResult += "\t";
                    strResult += (this.dblTecnicalLossesTransformersEnergy_MWh[i]).ToString();
                    strResult += "\t";
                    strResult += (this.dblEnergyTransformerOutLevelUpDown_MWh[i]).ToString();
                    strResult += "\t";
                    strResult += (this.dblSupplyEnergywithNet_MWh[i]).ToString();
                    strResult += "\t";
                    strResult += (this.dblDiferenceBalanceEnergy_MWh[i]).ToString();
                    strResult += Environment.NewLine;
                }
            }

            strResult += "Segmento\tEnergia dos Transformadores (MWh)\tPerda de Energia Ferro (MWh)\tPerda de Energia Cobre (MWh)\tNível de Alocação\tEnergia do Balanço Originada da ET (MWh)\tProporção (p.u.)\tEnergia do Balanço Originada da ET Corrigida (MWh)\tFator de Correção (p.u.)\tPerda de Energia Cobre Corrigido (MWh)\tEnergia do Resultante (MWh)" + Environment.NewLine;
            for (int i = 0; i < this.intPrimTransformer.Count; i++)
            {
                strResult += EnergyBalanceTools.getLevelSegment((int)this.intPrimTransformer[i]) + "-" + EnergyBalanceTools.getLevelSegment((int)this.intSecuTransformer[i]) + "-" + EnergyBalanceTools.getLevelSegment((int)this.intTercTransformer[i]);
                strResult += "\t";
                strResult += ((double)this.dblEnergyTransformer_MWh[i]).ToString();
                strResult += "\t";
                strResult += ((double)this.dblIronLossesTransformer_MWh[i]).ToString();
                strResult += "\t";
                strResult += ((double)this.dblCopperLossesTransformer_MWh[i]).ToString();
                strResult += "\t";
                strResult += EnergyBalanceTools.getLevelSegment((int)this.intAllocationTransformer[i]);
                strResult += "\t";
                strResult += ((double)this.dblEnergyTransformerBalanceBefore_MWh[i]).ToString();
                strResult += "\t";
                strResult += ((double)this.dblProportionTransformer_pu[i]).ToString();
                strResult += "\t";
                strResult += ((double)this.dblEnergyTransformerBalanceAfter_MWh[i]).ToString();
                strResult += "\t";
                strResult += ((double)this.dblCorretionFatorTransformer_pu[i]).ToString();
                strResult += "\t";
                strResult += ((double)this.dblCopperLossesCorrectedTransformer_MWh[i]).ToString();
                strResult += "\t";
                strResult += ((double)this.dblEnergyTransformerResult_MWh[i]).ToString();
                strResult += Environment.NewLine;
            }

            strResult += "Segmento\tEnergia dos Segmentos de Transformação (MWh)\tEnergia da Barra dos Níveis (MWh)\tProporções (p.u.)\tEnergia Injetada Transformador do Balanço (MWh)\tPerda de Energia Ferro (MWh)\tPerda de Energia Cobre (MWh)\tEnergia do Resultante Transformador (MWh)" + Environment.NewLine;
            for (int i = 0; i < this.intPrimSegmentTransformation.Count; i++)
            {
                strResult += EnergyBalanceTools.getLevelSegment((int)this.intPrimSegmentTransformation[i]) + "-" + EnergyBalanceTools.getLevelSegment((int)this.intSecuSegmentTransformation[i]);
                strResult += "\t";
                strResult += ((double)this.dblEnergySegmentTransformation_MWh[i]).ToString();
                strResult += "\t";
                strResult += ((double)this.dblEnergySegmentTransformationBalanceBefore_MWh[i]).ToString();
                strResult += "\t";
                strResult += ((double)this.dblProportionSegmentTransformation_pu[i]).ToString();
                strResult += "\t";
                strResult += ((double)this.dblEnergySegmentTransformationBalanceAfter_MWh[i]).ToString();
                strResult += "\t";
                strResult += ((double)this.dblIronLossesSegmentTransformation_MWh[i]).ToString();
                strResult += "\t";
                strResult += ((double)this.dblCopperLossesSegmentTransformation_MWh[i]).ToString();
                strResult += "\t";
                strResult += ((double)this.dblEnergySegmentTransformationResult_MWh[i]).ToString();
                strResult += Environment.NewLine;
            }
            
            strResult += "Transformador\tSegmento de Transformação\tProporção (p.u.)" + Environment.NewLine;
            for (int i = 0; i < this.intPrimTransformerTtoST.Count; i++)
            {
                strResult += EnergyBalanceTools.getLevelSegment((int)this.intPrimTransformerTtoST[i]) + "-" + EnergyBalanceTools.getLevelSegment((int)this.intSecuTransformerTtoST[i]) + "-" + EnergyBalanceTools.getLevelSegment((int)this.intTercTransformerTtoST[i]);
                strResult += "\t";
                strResult += EnergyBalanceTools.getLevelSegment((int)this.intPrimSegmentTransformationTtoST[i]) + "-" + EnergyBalanceTools.getLevelSegment((int)this.intSecuSegmentTransformationTtoST[i]);
                strResult += "\t";
                strResult += ((double)this.dblProportionTtoST_pu[i]).ToString();
                strResult += Environment.NewLine;
            }
            
            strResult += "Segmento de Transformação\tTransformador\tProporção (p.u.)" + Environment.NewLine;
            for (int i = 0; i < this.intPrimTransformerSTtoT.Count; i++)
            {
                strResult += EnergyBalanceTools.getLevelSegment((int)this.intPrimSegmentTransformationSTtoT[i]) + "-" + EnergyBalanceTools.getLevelSegment((int)this.intSecuSegmentTransformationSTtoT[i]);
                strResult += "\t";
                strResult += EnergyBalanceTools.getLevelSegment((int)this.intPrimTransformerSTtoT[i]) + "-" + EnergyBalanceTools.getLevelSegment((int)this.intSecuTransformerSTtoT[i]) + "-" + EnergyBalanceTools.getLevelSegment((int)this.intTercTransformerSTtoT[i]);
                strResult += "\t";
                strResult += ((double)this.dblProportionSTtoT_pu[i]).ToString();
                strResult += Environment.NewLine;
            }
            
            return strResult;

        }

    }

}
