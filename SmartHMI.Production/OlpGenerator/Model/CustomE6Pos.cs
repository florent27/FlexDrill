using System;
using System.ComponentModel;
using Kuka.FlexDrill.SmartHMI.Production.Geometry;
using KUKARoboter.KRCModel.Robot.Movement;

namespace Kuka.FlexDrill.SmartHMI.Production.OlpGenerator.Model
{
   public class CustomE6Pos : E6Pos, INotifyPropertyChanged
   {
      private double customX = 0.0;
      private double customY = 0.0;
      private double customZ = 0.0;
      private double customA = 0.0;
      private double customB = 0.0;
      private double customC = 0.0;
      private int customS = 0;
      private int customT = 0;
      private double customE1 = 0.0;
      private double customE2 = 0.0;
      private double customE3 = 0.0;
      private double customE4 = 0.0;
      private double customE5 = 0.0;
      private double customE6 = 0.0;
      private const string cszPos = "({0:F3}, {1:F3}, {2:F3}, 0, {3:F3}, {4:F3}, {5:F3}, 0)";

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

      public new int S
      {
         get
         {
            return customS;
         }
         set
         {
            customS = value;
            RaisePropertyChanged("S");
         }
      }

      public new int T
      {
         get
         {
            return customT;
         }
         set
         {
            customT = value;
            RaisePropertyChanged("T");
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
            customE6= value;
            RaisePropertyChanged("E6");
         }
      }

      public override string ToString()
      {
         return String.Format(cszPos, customX, customY, customZ, customA, customB, customC);
      }

      public Point ToPoint()
      {
         Point lPoint = new Point
         {
            X = this.X,
            Y = this.Y,
            Z = this.Z
         };
         return lPoint;
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
