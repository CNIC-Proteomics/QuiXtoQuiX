using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using QuiXoT.math;

namespace QuiXoT
{

    public partial class OPfitWindow : Form
    {

        fitDataStrt[] fitData=new fitDataStrt[1500];
        int stepsCount=0;
        Comb.mzI[] expData;

        public OPfitWindow()
        {
            InitializeComponent();

            this.Disposed += new EventHandler(OPfitWindow_Disposed);

        }

        void OPfitWindow_Disposed(object sender, EventArgs e)
        {
            sender = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        private void OPfitWindow_Load(object sender, EventArgs e)
        {

        }

        public void init(Comb.mzI[] data) 
        {

            stepsCount = 0;

            for (int i = 0; i < fitData.GetUpperBound(0); i++)
            {
                fitData[i].A = 0;
                fitData[i].alpha= 0; 
                fitData[i].B = 0;
                fitData[i].deltaMz = 0;
                fitData[i].f = 0;
                fitData[i].sigma = 0;
                fitData[i].signoise = 0;
            }
            
            expData =(Comb.mzI[])data.Clone();
        }

        public void addStep(fitDataStrt fitDataStep,double sumSQStep)
        {
            string Item;
            stepsCount++;

            fitData[stepsCount] = fitDataStep;

            Item = stepsCount.ToString() + " : ";
            Item += " SumSQ: " + sumSQStep.ToString();
            Item += " A: " + fitDataStep.A.ToString();
            Item += " B: " + fitDataStep.B.ToString();
            Item += " eff: " + fitDataStep.f.ToString();
            Item += " sigma: " + fitDataStep.sigma.ToString();
            Item += " SigNoise: " + fitDataStep.signoise.ToString();
            Item += " deltaMZ: " + fitDataStep.deltaMz.ToString();
            Item += " alpha: " + fitDataStep.alpha.ToString();


            this.lBoxSteps.Items.Add(Item);

        }

        public void addStep(fitDataStrt fitDataStep, double sumSQStep, TimeSpan time)
        {
            string Item;
            stepsCount++;

            fitData[stepsCount] = fitDataStep;

            Item = stepsCount.ToString() + " : ";
            Item += " time: " + time.Milliseconds;
            Item += " SumSQ: " + sumSQStep.ToString();
            Item += " A: " + fitDataStep.A.ToString();
            Item += " B: " + fitDataStep.B.ToString();
            Item += " eff: " + fitDataStep.f.ToString();
            Item += " sigma: " + fitDataStep.sigma.ToString();
            Item += " SigNoise: " + fitDataStep.signoise.ToString();
            Item += " deltaMZ: " + fitDataStep.deltaMz.ToString();
            Item += " alpha: " + fitDataStep.alpha.ToString();


            this.lBoxSteps.Items.Add(Item);

        }

        public void addStep(fitDataStrt fitDataStep, double sumSQStep, TimeSpan time, int sign)
        {

            string Item;
            stepsCount++;

            fitData[stepsCount] = fitDataStep;

            if (sign < 0)
            {
                Item = "-";
            }
            else 
            {
                Item = "";
            }
            
            Item += stepsCount.ToString() + " : ";
            Item += " time: " + time.Milliseconds;
            Item += " SumSQ: " + sumSQStep.ToString();
            Item += " A: " + fitDataStep.A.ToString();
            Item += " B: " + fitDataStep.B.ToString();
            Item += " eff: " + fitDataStep.f.ToString();
            Item += " sigma: " + fitDataStep.sigma.ToString();
            Item += " SigNoise: " + fitDataStep.signoise.ToString();
            Item += " deltaMZ: " + fitDataStep.deltaMz.ToString();
            Item += " alpha: " + fitDataStep.alpha.ToString();


            this.lBoxSteps.Items.Add(Item);
 
        }
    }
}