// --------------------------------------------------------------------------------------------------------------------
// <copyright file="KrlVarHandler.cs" company="KUKA Deutschland GmbH">
//    Copyright (c) KUKA Deutschland GmbH 2006 - 2018
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kuka.FlexDrill.SmartHMI.Production.Exceptions;
using Kuka.FlexDrill.SmartHMI.Production.Geometry;
using Kuka.FlexDrill.SmartHMI.Production.OlpGenerator.Model;
using KUKARoboter.KRCModel.Robot;
using System;
using System.Collections.Generic;

namespace Kuka.FlexDrill.SmartHMI.Production.Helper
{
    public static class KrlVarHandler
    {
        #region Constants and Fields

        private const string RobotInterfaceNullMessage = "Robot interface is null.";

        private const string RealType = "REAL";

        private const string CharType = "CHAR";

        private const string IntType = "INT";

        private const string BoolType = "BOOL";

        private const string E6PosType = "E6POS";

        private const string PosType = "POS";

        private const string E6AxisType = "E6AXIS";

        #endregion Constants and Fields

        #region Interface

        public static bool ReadBoolVariable(string variableName, IRobot robot)
        {
            if (robot == null)
            {
                throw new ArgumentException(RobotInterfaceNullMessage);
            }

            var krlVariable = robot.KRLVariables[variableName];

            try
            {
                // The call "krlVariable.ADSValue" will generate a Cross exception if the variable is not declared
                if (krlVariable.ADSValue.KRLType != BoolType)
                {
                    // Throw an exception if the variable is not from type BOOL
                    throw new ArgumentException(
                       $"Can not read KRL variable '{variableName}' because it's not from type '{BoolType}'.");
                }

                return robot.KRLVariables[variableName].ConvertToBoolean(false);
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch
            {
                throw new KrlVarNotDefinedException(variableName);
            }
        }

        public static double ReadRealVariable(string variableName, IRobot robot)
        {
            if (robot == null)
            {
                throw new ArgumentException(RobotInterfaceNullMessage);
            }

            var krlVariable = robot.KRLVariables[variableName];

            try
            {
                // The call "krlVariable.ADSValue" will generate a Cross exception if the variable is not declared
                if (krlVariable.ADSValue.KRLType != RealType)
                {
                    // Throw an exception if the variable is not from type REAL
                    throw new ArgumentException(
                       $"Can not read KRL variable '{variableName}' because it's not from type '{RealType}'.");
                }

                return robot.KRLVariables[variableName].ConvertToDouble(false);
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch
            {
                throw new KrlVarNotDefinedException(variableName);
            }
        }

        public static string ReadCharVariable(string variableName, IRobot robot)
        {
            if (robot == null)
            {
                throw new ArgumentException(RobotInterfaceNullMessage);
            }

            var krlVariable = robot.KRLVariables[variableName];

            try
            {
                // The call "krlVariable.ADSValue" will generate a Cross exception if the variable is not declared
                if (krlVariable.ADSValue.KRLType != CharType)
                {
                    // Throw an exception if the variable is not from type REAL
                    throw new ArgumentException(
                       $"Can not read KRL variable '{variableName}' because it's not from type '{CharType}'.");
                }

                return robot.KRLVariables[variableName].ConvertToString(false);
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch
            {
                throw new KrlVarNotDefinedException(variableName);
            }
        }

        public static int ReadIntVariable(string variableName, IRobot robot)
        {
            if (robot == null)
            {
                throw new ArgumentException(RobotInterfaceNullMessage);
            }

            var krlVariable = robot.KRLVariables[variableName];

            try
            {
                // The call "krlVariable.ADSValue" will generate a Cross exception if the variable is not declared
                if (krlVariable.ADSValue.KRLType != IntType)
                {
                    // Throw an exception if the variable is not from type REAL
                    throw new ArgumentException(
                       $"Can not read KRL variable '{variableName}' because it's not from type '{IntType}'.");
                }

                return robot.KRLVariables[variableName].ConvertToInt32(false);
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch
            {
                throw new KrlVarNotDefinedException(variableName);
            }
        }

        public static CustomE6Pos ReadPosVariable(string variableName, IRobot robot)
        {
            if (robot == null)
            {
                throw new ArgumentException(RobotInterfaceNullMessage);
            }

            var krlVariable = robot.KRLVariables[variableName];

            try
            {
                // The call "krlVariable.ADSValue" will generate a Cross exception if the variable is not declared
                bool IsPos = ((krlVariable.ADSValue.KRLType == E6PosType) || (krlVariable.ADSValue.KRLType == PosType));
                if (!IsPos)
                {
                    // Throw an exception if the variable is not from type REAL
                    throw new ArgumentException(
                       $"Can not read KRL variable '{variableName}' because it's not from type '{E6PosType}' or '{PosType}'.");
                }
                CustomE6Pos Pos = new CustomE6Pos
                {
                    X = robot.KRLVariables[$"{variableName}.X"].ConvertToDouble(false),
                    Y = robot.KRLVariables[$"{variableName}.Y"].ConvertToDouble(false),
                    Z = robot.KRLVariables[$"{variableName}.Z"].ConvertToDouble(false),
                    A = robot.KRLVariables[$"{variableName}.A"].ConvertToDouble(false),
                    B = robot.KRLVariables[$"{variableName}.B"].ConvertToDouble(false),
                    C = robot.KRLVariables[$"{variableName}.C"].ConvertToDouble(false)
                };
                return Pos;
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch
            {
                throw new KrlVarNotDefinedException(variableName);
            }
        }

        public static CustomE6Axis ReadAxisVariable(string variableName, IRobot robot)
        {
            if (robot == null)
            {
                throw new ArgumentException(RobotInterfaceNullMessage);
            }

            var krlVariable = robot.KRLVariables[variableName];

            try
            {
                // The call "krlVariable.ADSValue" will generate a Cross exception if the variable is not declared
                bool IsE6Axis = ((krlVariable.ADSValue.KRLType == E6AxisType));
                if (!IsE6Axis)
                {
                    // Throw an exception if the variable is not from type REAL
                    throw new ArgumentException(
                       $"Can not read KRL variable '{variableName}' because it's not from type '{E6AxisType}'.");
                }
                CustomE6Axis EAxis = new CustomE6Axis
                {
                    A1 = robot.KRLVariables[$"{variableName}.A1"].ConvertToDouble(false),
                    A2 = robot.KRLVariables[$"{variableName}.A2"].ConvertToDouble(false),
                    A3 = robot.KRLVariables[$"{variableName}.A3"].ConvertToDouble(false),
                    A4 = robot.KRLVariables[$"{variableName}.A4"].ConvertToDouble(false),
                    A5 = robot.KRLVariables[$"{variableName}.A5"].ConvertToDouble(false),
                    A6 = robot.KRLVariables[$"{variableName}.A6"].ConvertToDouble(false),
                    E1 = robot.KRLVariables[$"{variableName}.E1"].ConvertToDouble(false),
                    E2 = robot.KRLVariables[$"{variableName}.E2"].ConvertToDouble(false)
                };
                return EAxis;
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch
            {
                throw new KrlVarNotDefinedException(variableName);
            }
        }

        public static EulerFrame ReadFrameVariable(string variableName, IRobot robot)
        {
            if (robot == null)
            {
                throw new ArgumentException(RobotInterfaceNullMessage);
            }
            try
            {
                EulerFrame lFrame = new EulerFrame();
                lFrame.Translation.X = robot.KRLVariables[$"{variableName}.X"].ConvertToDouble(false);
                lFrame.Translation.Y = robot.KRLVariables[$"{variableName}.Y"].ConvertToDouble(false);
                lFrame.Translation.Z = robot.KRLVariables[$"{variableName}.Z"].ConvertToDouble(false);
                lFrame.Orientation.Rz = robot.KRLVariables[$"{variableName}.A"].ConvertToDouble(false);
                lFrame.Orientation.Ry = robot.KRLVariables[$"{variableName}.B"].ConvertToDouble(false);
                lFrame.Orientation.Rx = robot.KRLVariables[$"{variableName}.C"].ConvertToDouble(false);

                return lFrame;
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch
            {
                throw new KrlVarNotDefinedException(variableName);
            }
        }

        public static void WriteFrameVariable(string variableName, EulerFrame AFrame, IRobot robot)
        {
            if (robot == null)
            {
                throw new ArgumentException(RobotInterfaceNullMessage);
            }

            try
            {
                robot.KRLVariables[$"{variableName}.X"].RawValue = AFrame.Translation.X;
                robot.KRLVariables[$"{variableName}.Y"].RawValue = AFrame.Translation.Y;
                robot.KRLVariables[$"{variableName}.Z"].RawValue = AFrame.Translation.Z;
                robot.KRLVariables[$"{variableName}.A"].RawValue = AFrame.Orientation.Rz;
                robot.KRLVariables[$"{variableName}.B"].RawValue = AFrame.Orientation.Ry;
                robot.KRLVariables[$"{variableName}.C"].RawValue = AFrame.Orientation.Rx;
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch
            {
                throw new KrlVarNotDefinedException(variableName);
            }
        }

        public static void WriteVariable(string variableName, object value, IRobot robot)
        {
            if (robot == null)
            {
                throw new ArgumentException(RobotInterfaceNullMessage);
            }

            try
            {
                robot.KRLVariables[variableName].RawValue = value;
            }
            catch
            {
                throw new KrlVarNotDefinedException(variableName);
            }
        }

        private static List<string> GetListOfTokenFromOlpFrame(string AOlpFrame)
        {
            List<string> Tokens = new List<string>();
            //
            //! Remove Parenthesis
            //
            AOlpFrame = AOlpFrame.Replace("(", "");
            AOlpFrame = AOlpFrame.Replace(")", "");
            AOlpFrame = AOlpFrame.Replace(" ", "");
            string[] SplittedLine = AOlpFrame.Split(',');

            foreach (string AItem in SplittedLine)
            {
                Tokens.Add(AItem);
            }

            return Tokens;
        }

        public static CustomE6Pos E6APosFromString(string AE6Pos)
        {
            try
            {
                CustomE6Pos lE6Pos = new CustomE6Pos();

                List<string> Tokens = new List<string>();
                Tokens = GetListOfTokenFromOlpFrame(AE6Pos);
                lE6Pos.X = Convert.ToDouble(Tokens[0]);
                lE6Pos.Y = Convert.ToDouble(Tokens[1]);
                lE6Pos.Z = Convert.ToDouble(Tokens[2]);
                lE6Pos.S = Convert.ToInt32(Tokens[3]);
                lE6Pos.A = Convert.ToDouble(Tokens[4]);
                lE6Pos.B = Convert.ToDouble(Tokens[5]);
                lE6Pos.C = Convert.ToDouble(Tokens[6]);
                lE6Pos.T = Convert.ToInt32(Tokens[7]);
                return lE6Pos;
            }
            catch (Exception e)
            {
                throw new Exception("Cannot Convert string to E6Pos" + e.Message);
            }
        }

        public static CustomE6Axis E6AAxisFromString(string AE6Axis)
        {
            try
            {
                CustomE6Axis lE6Axis = new CustomE6Axis();

                List<string> Tokens = new List<string>();
                Tokens = GetListOfTokenFromOlpFrame(AE6Axis);
                lE6Axis.A1 = Convert.ToDouble(Tokens[0]);
                lE6Axis.A2 = Convert.ToDouble(Tokens[1]);
                lE6Axis.A3 = Convert.ToDouble(Tokens[2]);
                lE6Axis.A4 = Convert.ToDouble(Tokens[3]);
                lE6Axis.A5 = Convert.ToDouble(Tokens[4]);
                lE6Axis.A6 = Convert.ToDouble(Tokens[5]);
                return lE6Axis;
            }
            catch (Exception e)
            {
                throw new Exception("Cannot Convert string to E6Axis" + e.Message);
            }
        }

        #endregion Interface
    }
}