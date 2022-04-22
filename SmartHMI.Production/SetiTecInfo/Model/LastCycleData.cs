using Kuka.FlexDrill.SmartHMI.Production.Base;
using System.Collections.Generic;

namespace Kuka.FlexDrill.SmartHMI.Production.SetiTecInfo.Model
{
    public class LastCycleData : BindablObject
    {
        private string boxName;
        private string boxSn;
        private string motorName;
        private string motorSn;
        private string headUid;
        private string headType;
        private string headName;
        private string localCounter;
        private string globalCounter;
        private string cycleResult;

        private List<LastCycleDataStep> lastCycleDataSteps = new List<LastCycleDataStep>();

        public LastCycleData()
        {
            //! Create List
            LastCycleDataSteps = new List<LastCycleDataStep>();
            // Create 10 Steps
            for (int i = 0; i < 10; i++)
            {
                LastCycleDataStep Step = new LastCycleDataStep();
                LastCycleDataSteps.Add(Step);
            }
        }

        public List<LastCycleDataStep> LastCycleDataSteps
        {
            get
            {
                return lastCycleDataSteps;
            }
            set
            {
                SetField(ref lastCycleDataSteps, value);
            }
        }

        public string BoxName
        {
            get
            {
                return boxName;
            }
            set
            {
                SetField(ref boxName, value);
            }
        }

        public string BoxSn
        {
            get
            {
                return boxSn;
            }
            set
            {
                SetField(ref boxSn, value);
            }
        }

        public string MotorName
        {
            get
            {
                return motorName;
            }
            set
            {
                SetField(ref motorName, value);
            }
        }

        public string MotorSn
        {
            get
            {
                return motorSn;
            }
            set
            {
                SetField(ref motorSn, value);
            }
        }

        public string HeadUid
        {
            get
            {
                return headUid;
            }
            set
            {
                SetField(ref headUid, value);
            }
        }

        public string HeadType
        {
            get
            {
                return headType;
            }
            set
            {
                SetField(ref headType, value);
            }
        }

        public string HeadName
        {
            get
            {
                return headName;
            }
            set
            {
                SetField(ref headName, value);
            }
        }

        public string LocalCounter
        {
            get
            {
                return localCounter;
            }
            set
            {
                SetField(ref localCounter, value);
            }
        }

        public string GlobalCounter
        {
            get
            {
                return globalCounter;
            }
            set
            {
                SetField(ref globalCounter, value);
            }
        }

        public string CycleResult
        {
            get
            {
                return cycleResult;
            }
            set
            {
                SetField(ref cycleResult, value);
            }
        }
    }
}