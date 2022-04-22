using Kuka.FlexDrill.SmartHMI.Production.Base;

namespace Kuka.FlexDrill.SmartHMI.Production.SetiTecInfo.Model
{
    public class LastCycleDataStep : BindablObject
    {
        private string stepNumber;
        private string stopCode;
        private string duration;
        private string distanceM1;
        private string distanceM2;
        private string m1MaxAmperage;
        private string m2MaxAmperage;
        private string m1NoLoadAmperage;
        private string m2NoLoadAmperage;

        public string StepNumber
        {
            get
            {
                return stepNumber;
            }
            set
            {
                SetField(ref stepNumber, value);
            }
        }

        public string StopCode
        {
            get
            {
                return stopCode;
            }
            set
            {
                SetField(ref stopCode, value);
            }
        }

        public string Duration
        {
            get
            {
                return duration;
            }
            set
            {
                SetField(ref duration, value);
            }
        }

        public string DistanceM1
        {
            get
            {
                return distanceM1;
            }
            set
            {
                SetField(ref distanceM1, value);
            }
        }

        public string DistanceM2
        {
            get
            {
                return distanceM2;
            }
            set
            {
                SetField(ref distanceM2, value);
            }
        }

        public string M1MaxAmperage
        {
            get
            {
                return m1MaxAmperage;
            }
            set
            {
                SetField(ref m1MaxAmperage, value);
            }
        }

        public string M2MaxAmperage
        {
            get
            {
                return m2MaxAmperage;
            }
            set
            {
                SetField(ref m2MaxAmperage, value);
            }
        }

        public string M1NoLoadAmperage
        {
            get
            {
                return m1NoLoadAmperage;
            }
            set
            {
                SetField(ref m1NoLoadAmperage, value);
            }
        }

        public string M2NoLoadAmperage
        {
            get
            {
                return m2NoLoadAmperage;
            }
            set
            {
                SetField(ref m2NoLoadAmperage, value);
            }
        }
    }
}