using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using KukaRoboter.Krl;

namespace Kuka.FlexDrill.SmartHMI.Production.Geometry
{
   
   public struct Point
   {
      public double X, Y, Z;
   }

   public struct PointPair
   {
      public int Index;
      public int ContextId;
      public Point TheoreticalPoint;
      public Point MeasuredPoint;
      public double MeasureDate;
   }

   public struct Euler
   {
      public double Rx, Ry, Rz;
   }

   public struct Quaternion
   {
      public double q1, q2, q3, q4;
   }

   public struct Matrix3x3
   {
      public double Xi, Yi, Zi, Xj, Yj, Zj, Xk, Yk, Zk;
   }

   public struct Frame
   {
      public Point Translation;
      public Matrix3x3 Orientation;
   }

   public struct EulerFrame
   {
      const string cszPos = "({0:F3}, {1:F3}, {2:F3}, {3:F3}, {4:F3}, {5:F3})";

      public Point Translation;
      public Euler Orientation;

      public override string ToString()
      {
         return String.Format(cszPos, Translation.X, Translation.Y, Translation.Z, Orientation.Rz, Orientation.Ry, Orientation.Rx);
      }
   }

}
