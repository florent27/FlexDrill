using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Kuka.FlexDrill.Process.Logger;
using Kuka.FlexDrill.SmartHMI.Production.Helper;
using KUKARoboter.KRCModel.Robot;

namespace Kuka.FlexDrill.SmartHMI.Production.Geometry
{
   public enum GeometryFunctionsCode
   {
      InverseFrame = 1,
      MultiplyFrame = 2,
      FrameAngle = 3,
      FrameDistance = 4,
      FrameFrom3Points = 5,
      AverageFrameFromNPoints = 6
   }
   public static class GeometryFunctions
   {
      [DllImport("Geometry.dll")]
      public static extern Point DoVectorsAddition(Point APoint1, Point APoint2);
      [DllImport("Geometry.dll")]
      public static extern Frame DoFrameFrom3Points(Point APoint1, Point APoint2, Point APoint3);

      [DllImport("Geometry.dll")]
      public static extern Frame DoInverseFrame(Frame AFrame);

      [DllImport("Geometry.dll")]
      public static extern Frame DoEulerFrameToFrame(EulerFrame AFrame);

      [DllImport("Geometry.dll")]
      public static extern EulerFrame DoFrameToEulerFrame(Frame AFrame);

      [DllImport("Geometry.dll")]
      public static extern double DoFramesAngle(Frame ALeftFrame, Frame ARightFrame);

      [DllImport("Geometry.dll")]
      public static extern double DoFramesDistance(Frame ALeftFrame, Frame ARightFrame);

      [DllImport("Geometry.dll")]
      public static extern Frame DoMultiplyFrames(Frame ALeftFrame, Frame ARightFrame);

      [DllImport("Geometry.dll")]
      public static extern int DoAverageFrameFromNPoints(out Frame AComputedFrame, Frame ATheoreticalFrame, PointPair[] APointPairList, int APointPairListCnt, double AMaxDistance, double AMaxAngle);

      public static void ExecuteGeometryFunction(int AFctCode, IRobot ARobot)
      {

         switch (AFctCode)
         {  
            case (int)GeometryFunctionsCode.InverseFrame:

               InverseFrame(ARobot);
               break;

            case (int)GeometryFunctionsCode.MultiplyFrame:
               MultiplyFrames(ARobot);
               break;

            case (int)GeometryFunctionsCode.FrameAngle:
               FrameAngle(ARobot);
               break;

            case (int)GeometryFunctionsCode.FrameDistance:
               FrameDistance(ARobot);
               break;

            case (int)GeometryFunctionsCode.FrameFrom3Points:
               FrameFrom3Points(ARobot);
               break;

            case (int)GeometryFunctionsCode.AverageFrameFromNPoints:
               AverageFrameFromNPoints(ARobot);
               break;
         }

      }

      private static void AverageFrameFromNPoints(IRobot ARobot)
      {
         try
         {
            //! Read KRL
            EulerFrame lETheoFrame = new EulerFrame();
            Frame lTheoFrame = new Frame();

            lETheoFrame = KrlVarHandler.ReadFrameVariable(KrlVariableNames.AverageFrameFromNPts_TheoFrame, ARobot);
            lTheoFrame = DoEulerFrameToFrame(lETheoFrame);

            int lPointPairListCnt =
               KrlVarHandler.ReadIntVariable(KrlVariableNames.AverageFrameFromNPts_PointPairListCnt, ARobot);
            double lMaxDistance =
               KrlVarHandler.ReadRealVariable(KrlVariableNames.AverageFrameFromNPts_MaxDistance, ARobot);
            double lMaxAngle = KrlVarHandler.ReadRealVariable(KrlVariableNames.AverageFrameFromNPts_MaxAngle, ARobot);

            PointPair[] lPointPairList = new PointPair[lPointPairListCnt];
            string VarName;
            for (int i = 1; i < (lPointPairListCnt + 1); i++)
            {
               VarName = string.Format(KrlVariableNames.PointPairListFmt_TheoriticalPoint, i);
               lPointPairList[i - 1].TheoreticalPoint = KrlVarHandler.ReadFrameVariable(VarName, ARobot).Translation;

               VarName = string.Format(KrlVariableNames.PointPairListFmt_MeasuredPoint, i);
               lPointPairList[i - 1].MeasuredPoint = KrlVarHandler.ReadFrameVariable(VarName, ARobot).Translation;
            }

            // Compute
            Frame lComputedFrame = new Frame();
            EulerFrame lEComputedFrame = new EulerFrame();

            int Res = DoAverageFrameFromNPoints(out lComputedFrame, lTheoFrame, lPointPairList, lPointPairListCnt, lMaxDistance, lMaxAngle);
            lEComputedFrame = DoFrameToEulerFrame(lComputedFrame);

            //! Write KRL
            KrlVarHandler.WriteFrameVariable(KrlVariableNames.AverageFrameFromNPts_ComputedFrame, lEComputedFrame, ARobot);

            if (Res != 1)
            {
               KrlVarHandler.WriteVariable(KrlVariableNames.GeometryFctExecutedOK, false, ARobot);
            }
            else
            {
               KrlVarHandler.WriteVariable(KrlVariableNames.GeometryFctExecutedOK, true, ARobot);
               //! Log
               Logger.WriteLog(
                  string.Format("AverageFrameFromNPoints - ComputedFrame = (X {0} Y {1} Z {2} A {3} B {4} C{5})",
                     lEComputedFrame.Translation.X, lEComputedFrame.Translation.Y, lEComputedFrame.Translation.Z,
                     lEComputedFrame.Orientation.Rz, lEComputedFrame.Orientation.Ry, lEComputedFrame.Orientation.Rx),
                  false);
            }
         }
         catch (Exception e)
         {
            throw e = new Exception("An Error Occured while Executing Geometry Function - Message " + e.Message);
         }
         finally
         {
            KrlVarHandler.WriteVariable(KrlVariableNames.GeometryFctExecuted, true, ARobot);
         }
      }

      private static void FrameFrom3Points(IRobot ARobot)
      {
         try
         {
            //! Read KRL
            Frame lPoint1 = new Frame();
            Frame lPoint2 = new Frame();
            Frame lPoint3 = new Frame();
            EulerFrame lEPoint1 = new EulerFrame();
            EulerFrame lEPoint2 = new EulerFrame();
            EulerFrame lEPoint3 = new EulerFrame();

            lEPoint1 = KrlVarHandler.ReadFrameVariable(KrlVariableNames.FrameFrom3Points_Point1, ARobot);
            lEPoint2 = KrlVarHandler.ReadFrameVariable(KrlVariableNames.FrameFrom3Points_Point2, ARobot);
            lEPoint3 = KrlVarHandler.ReadFrameVariable(KrlVariableNames.FrameFrom3Points_Point3, ARobot);
            lPoint1 = DoEulerFrameToFrame(lEPoint1);
            lPoint2 = DoEulerFrameToFrame(lEPoint2);
            lPoint3 = DoEulerFrameToFrame(lEPoint3);

            //! Compute
            EulerFrame lEComputedFrame = new EulerFrame();
            Frame lComputedFrame = new Frame();

            lComputedFrame = DoFrameFrom3Points(lPoint1.Translation, lPoint2.Translation, lPoint3.Translation);
            lEComputedFrame = DoFrameToEulerFrame(lComputedFrame);

            //! Write KRL
            KrlVarHandler.WriteVariable(KrlVariableNames.FrameFrom3Points_ComputedFrame, lEComputedFrame, ARobot);
            KrlVarHandler.WriteVariable(KrlVariableNames.GeometryFctExecutedOK, true, ARobot);
            //! Log
            Logger.WriteLog(
               string.Format("FrameFrom3Points - ComputedFrame = (X {0} Y {1} Z {2} A {3} B {4} C{5})",
                  lEComputedFrame.Translation.X, lEComputedFrame.Translation.Y, lEComputedFrame.Translation.Z,
                  lEComputedFrame.Orientation.Rz, lEComputedFrame.Orientation.Ry, lEComputedFrame.Orientation.Rx),
               false);
         }
         catch (Exception e)
         {
            throw e = new Exception("An Error Occured while Executing Geometry Function - Message " + e.Message);
         }
         finally
         {
            KrlVarHandler.WriteVariable(KrlVariableNames.GeometryFctExecuted, true, ARobot);
         }
      }

      private static void FrameDistance(IRobot ARobot)
      {
         try
         {
            //! Read KRL Variable
            EulerFrame lEFrameLeft = new EulerFrame();
            EulerFrame lEFrameRight = new EulerFrame();
            Frame lFrameLeft = new Frame();
            Frame lFrameRight = new Frame();

            lEFrameLeft = KrlVarHandler.ReadFrameVariable(KrlVariableNames.FrameDistance_LeftFrame, ARobot);
            lEFrameRight = KrlVarHandler.ReadFrameVariable(KrlVariableNames.FrameDistance_RightFrame, ARobot);
            lFrameLeft = DoEulerFrameToFrame(lEFrameLeft);
            lFrameRight = DoEulerFrameToFrame(lEFrameRight);

            //! Compute 
            double lDistance = DoFramesDistance(lFrameLeft, lFrameRight);

            //! Write KRL
            KrlVarHandler.WriteVariable(KrlVariableNames.FrameDistance_ComputedDistance, lDistance, ARobot);
            KrlVarHandler.WriteVariable(KrlVariableNames.GeometryFctExecutedOK, true, ARobot);
            //! Log
            Logger.WriteLog(
               string.Format("FrameDistance- ComputedDistance = {0}", lDistance), false);
         }
         catch (Exception e)
         {
            throw e = new Exception("An Error Occured while Executing Geometry Function - Message " + e.Message);
         }
         finally
         {
            KrlVarHandler.WriteVariable(KrlVariableNames.GeometryFctExecuted, true, ARobot);
         }
      }

      private static void FrameAngle(IRobot ARobot)
      {
         try
         {
            //! Read KRL Variable
            EulerFrame lEFrameLeft = new EulerFrame();
            EulerFrame lEFrameRight = new EulerFrame();
            Frame lFrameLeft = new Frame();
            Frame lFrameRight = new Frame();

            lEFrameLeft = KrlVarHandler.ReadFrameVariable(KrlVariableNames.FrameAngle_LeftFrame, ARobot);
            lEFrameRight = KrlVarHandler.ReadFrameVariable(KrlVariableNames.FrameAngle_RightFrame, ARobot);
            lFrameLeft = DoEulerFrameToFrame(lEFrameLeft);
            lFrameRight = DoEulerFrameToFrame(lEFrameRight);

            //! Compute 
            double lAngle = DoFramesAngle(lFrameLeft, lFrameRight);

            //! Write KRL
            KrlVarHandler.WriteVariable(KrlVariableNames.FrameAngle_ComputedAngle, lAngle, ARobot);
            KrlVarHandler.WriteVariable(KrlVariableNames.GeometryFctExecutedOK, true, ARobot);
            //! Log
            Logger.WriteLog(
               string.Format("FrameAngle - ComputedAngle = {0}", lAngle), false);
         }
         catch (Exception e)
         {
            throw e = new Exception("An Error Occured while Executing Geometry Function - Message " + e.Message);
         }
         finally
         {
            KrlVarHandler.WriteVariable(KrlVariableNames.GeometryFctExecuted, true, ARobot);
         }
      }

      private static void MultiplyFrames(IRobot ARobot)
      {
         try
         {
            //! Read KRL Variable
            EulerFrame lEFrameLeft = new EulerFrame();
            EulerFrame lEFrameRight = new EulerFrame();
            Frame lFrameLeft = new Frame();
            Frame lFrameRight = new Frame();

            lEFrameLeft = KrlVarHandler.ReadFrameVariable(KrlVariableNames.MultipltFrame_LeftFrame, ARobot);
            lEFrameRight = KrlVarHandler.ReadFrameVariable(KrlVariableNames.MultipltFrame_RightFrame, ARobot);
            lFrameLeft = DoEulerFrameToFrame(lEFrameLeft);
            lFrameRight = DoEulerFrameToFrame(lEFrameRight);

            //! Compute 
            EulerFrame lEComputedFrame = new EulerFrame();
            Frame lComputedFrame = new Frame();

            lComputedFrame = DoMultiplyFrames(lFrameLeft, lFrameRight);
            lEComputedFrame = DoFrameToEulerFrame(lComputedFrame);

            //! Write KRL
            KrlVarHandler.WriteFrameVariable(KrlVariableNames.MultipltFrame_ComputedFrame, lEComputedFrame, ARobot);
            KrlVarHandler.WriteVariable(KrlVariableNames.GeometryFctExecutedOK, true, ARobot);
            //! Log
            Logger.WriteLog(
               string.Format("MultiplyFrames - ComputedFrame = (X {0} Y {1} Z {2} A {3} B {4} C{5})",
                  lEComputedFrame.Translation.X, lEComputedFrame.Translation.Y, lEComputedFrame.Translation.Z,
                  lEComputedFrame.Orientation.Rz, lEComputedFrame.Orientation.Ry, lEComputedFrame.Orientation.Rx),
               false);
         }
         catch (Exception e)
         {
            throw e = new Exception("An Error Occured while Executing Geometry Function - Message " + e.Message);
         }
         finally
         {
            KrlVarHandler.WriteVariable(KrlVariableNames.GeometryFctExecuted, true, ARobot);
         }

      }

      private static void InverseFrame(IRobot ARobot)
      {
         try
         {
            //! Read KRL Frame
            EulerFrame lEFrame = new EulerFrame();
            Frame lFrame = new Frame();

            lEFrame = KrlVarHandler.ReadFrameVariable(KrlVariableNames.InverseFrame_FrameToInverse, ARobot);
            lFrame = DoEulerFrameToFrame(lEFrame);

            //! Compute
            Frame lComputedFrame = new Frame();
            EulerFrame lEComputedFrame = new EulerFrame();

            lComputedFrame = DoInverseFrame(lFrame);
            lEComputedFrame = DoFrameToEulerFrame(lComputedFrame);

            //! Write KRL
            KrlVarHandler.WriteFrameVariable(KrlVariableNames.InverseFrame_ComputedFrame, lEComputedFrame, ARobot);
            KrlVarHandler.WriteVariable(KrlVariableNames.GeometryFctExecutedOK, true, ARobot);
            //! Log
            Logger.WriteLog(
               string.Format("InverseFrame - ComputedFrame = (X {0} Y {1} Z {2} A {3} B {4} C{5})",
                  lEComputedFrame.Translation.X, lEComputedFrame.Translation.Y, lEComputedFrame.Translation.Z,
                  lEComputedFrame.Orientation.Rz, lEComputedFrame.Orientation.Ry, lEComputedFrame.Orientation.Rx),
               false);
         }
         catch (Exception e)
         {
            throw e = new Exception("An Error Occured while Executing Geometry Function - Message " + e.Message);
         }
         finally
         {
            KrlVarHandler.WriteVariable(KrlVariableNames.GeometryFctExecuted, true, ARobot);
         }

      }

   }
}
