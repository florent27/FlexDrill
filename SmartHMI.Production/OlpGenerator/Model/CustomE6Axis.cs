using KUKARoboter.KRCModel.Robot.Movement;
using System;
using System.ComponentModel;

namespace Kuka.FlexDrill.SmartHMI.Production.OlpGenerator.Model
{
    public class CustomE6Axis : E6Axis, INotifyPropertyChanged
    {
        private double customA1 = 0.0;
        private double customA2 = 0.0;
        private double customA3 = 0.0;
        private double customA4 = 0.0;
        private double customA5 = 0.0;
        private double customA6 = 0.0;
        private double customE1 = 0.0;
        private double customE2 = 0.0;
        private double customE3 = 0.0;
        private double customE4 = 0.0;
        private double customE5 = 0.0;
        private double customE6 = 0.0;
        private const string cszAxis = "({0:F3}, {1:F3}, {2:F3}, {3:F3}, {4:F3}, {5:F3})";

        public new double A1
        {
            get
            {
                return customA1;
            }
            set
            {
                customA1 = value;
                RaisePropertyChanged("A1");
            }
        }

        public new double A2
        {
            get
            {
                return customA2;
            }
            set
            {
                customA2 = value;
                RaisePropertyChanged("A2");
            }
        }

        public new double A3
        {
            get
            {
                return customA3;
            }
            set
            {
                customA3 = value;
                RaisePropertyChanged("A3");
            }
        }

        public new double A4
        {
            get
            {
                return customA4;
            }
            set
            {
                customA4 = value;
                RaisePropertyChanged("A4");
            }
        }

        public new double A5
        {
            get
            {
                return customA5;
            }
            set
            {
                customA5 = value;
                RaisePropertyChanged("A5");
            }
        }

        public new double A6
        {
            get
            {
                return customA6;
            }
            set
            {
                customA6 = value;
                RaisePropertyChanged("A6");
            }
        }

        public new double E1
        {
            get
            {
                return customE1;
            }
            set
            {
                customE1 = value;
                RaisePropertyChanged("E1");
            }
        }

        public new double E2
        {
            get
            {
                return customE2;
            }
            set
            {
                customE2 = value;
                RaisePropertyChanged("E2");
            }
        }

        public new double E3
        {
            get
            {
                return customE3;
            }
            set
            {
                customE3 = value;
                RaisePropertyChanged("E3");
            }
        }

        public new double E4
        {
            get
            {
                return customE4;
            }
            set
            {
                customE4 = value;
                RaisePropertyChanged("E4");
            }
        }

        public new double E5
        {
            get
            {
                return customE5;
            }
            set
            {
                customE5 = value;
                RaisePropertyChanged("E5");
            }
        }

        public new double E6
        {
            get
            {
                return customE6;
            }
            set
            {
                customE6 = value;
                RaisePropertyChanged("E6");
            }
        }

        public override string ToString()
        {
            return String.Format(cszAxis, customA1, customA2, customA3, customA4, customA5, customA6);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null))
            {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
}