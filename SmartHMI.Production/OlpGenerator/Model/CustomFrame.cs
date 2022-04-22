using KUKARoboter.KRCModel.Robot.Movement;
using System;
using System.ComponentModel;

namespace Kuka.FlexDrill.SmartHMI.Production.OlpGenerator.Model
{
    public class CustomFrame : Frame, INotifyPropertyChanged
    {
        private double customX = 0.0;
        private double customY = 0.0;
        private double customZ = 0.0;
        private double customA = 0.0;
        private double customB = 0.0;
        private double customC = 0.0;
        private const string cszFrame = "({0:F3}, {1:F3}, {2:F3}, {3:F3}, {4:F3}, {5:F3})";

        public new double X
        {
            get
            {
                return customX;
            }
            set
            {
                customX = value;
                RaisePropertyChanged("X");
            }
        }

        public new double Y
        {
            get
            {
                return customY;
            }
            set
            {
                customY = value;
                RaisePropertyChanged("Y");
            }
        }

        public new double Z
        {
            get
            {
                return customZ;
            }
            set
            {
                customZ = value;
                RaisePropertyChanged("Z");
            }
        }

        public new double A
        {
            get
            {
                return customA;
            }
            set
            {
                customA = value;
                RaisePropertyChanged("A");
            }
        }

        public new double B
        {
            get
            {
                return customB;
            }
            set
            {
                customB = value;
                RaisePropertyChanged("B");
            }
        }

        public new double C
        {
            get
            {
                return customC;
            }
            set
            {
                customC = value;
                RaisePropertyChanged("C");
            }
        }

        public override string ToString()
        {
            return String.Format(cszFrame, customX, customY, customZ, customA, customB, customC);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null))
            {
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}