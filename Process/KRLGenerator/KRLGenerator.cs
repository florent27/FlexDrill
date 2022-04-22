// --------------------------------------------------------------------------------------------------------------------
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="KRLGenerator.cs" company="KUKA Systems Aerospace">
//    Copyright (c) KUKA Systems Aerospace 2006 - 2018
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kuka.FlexDrill.Process.CustomClasses;
using Kuka.FlexDrill.Process.Exceptions;
using Kuka.FlexDrill.Process.Helper;
using Kuka.FlexDrill.Process.OLPParser;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Text;

namespace Kuka.FlexDrill.Process.KRLGenerator
{
    public static class KrlGenerator
    {
        #region Constants and Fields

        private const string cszExecuteProcessFmt = "ExecuteProcess(\"{0}\", {1})";
        private const string cszMoveJointFmt = "MoveJoint(\"{0}\")";
        private const string cszMoveCartFmt = "MoveCart(\"{0}\", {1}, \"{2}\", {3})";
        private const string cszDefineOlpObjectFrameFmt = "  DefineOLPData(\"{0}\", \"{1}\", \"{2}\", \"{3}\")";
        private const string cszAddOpCorrectorFmt = "AddOperationCorrector({0},\"{1}\", \"{2}\", {3})";
        private const string cszAddOperationTargetFmt = "AddOperationTarget({0}, \"{1}\")";
        private const string cszDefineEscapeFrameFmt = "DefineEscapeFrame(\"{0}\")";
        private const string cszDefineOffsetFrameFmt = "DefineOffsetFrame(\"{0}\")";
        private const string cszDoUpdateOperationDataFmt = "DoUpdateOperationData({0}, \"{1}\", {2}, {3})";
        private const string cszDoUpdatePointDataFmt = "DoUpdatePointData({0}, {1}, \"{2}\", {3})";
        private const string cszFrameFormatFmt = "X {0}, Y {1}, Z {2}, A {3}, B {4}, C {5}";
        private const string cszE6AxisFormatFmt = "A1 {0}, A2 {1}, A3 {2}, A4 {3}, A5 {4}, A6 {5}";
        private const string cszE6PosFormatFmt = "X {0}, Y {1}, Z {2}, S {3}, A {4}, B {5}, C {6}, T {7}";
        private const string cszDefineOLPSafetyNameFmt = "  DefineOLPSafetyName(\"{0}\")";
        private const string cszDefineOLPMessageFmt = "DefineOLPPointMessage(\"{0}\")";
        private const string cszDefineOLPSpeedFmt = "DefineOLPSpeed({0})";
        private const string cszDefineEffectorToUse = "DefineEffectorToUse(\"{0}\")";
        private const string cszDefinePointIsTrajOnly = "DefinePointIsTrajOnly({0})";
        private const string cszDefinePatternName = "DefinePatternName(\"{0}\")";
        private const string cszDefineMoveType = "DefineMoveType(\"{0}\")";
        private const string cszDefineLoadNextJob = "DefineCanLoadNextJob({0})";

        private const string cszAccess = "&ACCESS RVO";
        private const string cszRev = "&REL 1";
        private const string cszProgramPath = "&PARAM DISKPATH = Program";
        private const string cszProgramNameFmt = "DEF {0}()";
        private const string cszFoldInit = ";FOLD Init Flex Drill Program";
        private const string cszInitSetiTec = "  SetiTec(#INIT_HEAD)";
        private const string cszInitFlexDrill = "  InitFlexDrill()";
        private const string cszBasInit = "  BAS(#INITMOV,0 )";
        private const string cszBCO = "  PTP $POS_ACT";
        private const string cszEndFoldInit = ";ENDFOLD Init Flex Drill Program";

        private const string cszJoint = "JOINT";
        private const string cszCart = "CART";
        private const string cszCartEscape = "CARTEscape";
        private const string cszEnd = "END";
        private const string cszGP0 = "G_GP0()";
        private const string cszFalse = "FALSE";
        private const string cszTrue = "TRUE";
        private const string cszTrajOnly = "TRAJONLY";
        private const string cszHalt = "HALT";

        private const string cszCommentFOLDOpNameFmt = "; FOLD Operation: {0}";
        private const string cszCommentENDFOLDOpNameFmt = "; ENDFOLD Operation: {0}";
        private const string cszCommentEscapeFrame = "; Escape Frame";
        private const string cszCommentOffsetFrame = "; Offset Frame";
        private const string cszCommentPatternName = "; Pattern Name";
        private const string cszCommentFOLDPointName = "; FOLD Point: {0} ({1})";
        private const string cszCommentENDFOLDPointName = "; ENDFOLD Point: {0}";
        private const string cszCommentReturnGP0 = "; Return To GP0";
        private const string cszCommentOPCorrectors = "; Operation Correctors";
        private const string cszCommentOPTargets = "; Operation Targets";
        private const string cszCommentHaltProgram = "; Halt Program";
        private const string cszCommentCellProgram = "; CellProgram: {0}";
        private const string cszCommentUpdateOpData = "; Update Operation Data";
        private const string cszCommentUpdatePointData = "; Update Point Data";
        private const string cszCommentOLPSpeed = "; OLP Speed";
        private const string cszCommentEffectorToUse = "; Effector To Use";
        private const string cszCommentGoToEscape = "; Go To Escape";
        private const string cszCommentGoToPoint = "; Go To Point";
        private const string cszCommentMoveType = "; Move Type";
        private const string cszCommentCanLoadNextJob = "; Can Load Next Job";

        private const int ciTCP1 = 1;
        private const int ciTCP2 = 2;
        private const int ciTCP3 = 3;

        private const int ciTCP10 = 10;

        private const int ciMaxTcpId = 16;

        private const int ciProcessStart = 0;
        private const int ciProcessRunning = 1;
        private const int ciProcessEnd = 2;

        private const string AmericanCultureInfo = "en-US";

        //
        //! Var
        //
        private static CellConfiguration FCellConfig;

        private static StringBuilder OutputString;
        private static ObservableCollection<Operation> AllOperations;
        private static Dictionary<string, int> ProcessTCP;

        #endregion Constants and Fields

        #region Interface

        /// <summary>
        /// Generate SRC Output File
        /// </summary>
        /// <param name="ACellProgram"></param>
        /// <param name="AOutputSRCFolderPath"></param>
        /// <returns></returns>
        public static bool DoGenerateSrcFile(CellProgram ACellProgram, string AOutputSRCFolderPath)
        {
            string szOutputFilePath = "";
            bool FctResult = false;
            //
            //! Log
            //
            if (ACellProgram != null)
            {
                Logger.Logger.WriteLog("Function Call - GenerateSRCFile(" + ACellProgram.Name + "," + AOutputSRCFolderPath + ")", false);
            }
            else
            {
                Logger.Logger.WriteLog("Function Call - GenerateSRCFile(null," + AOutputSRCFolderPath + ")", false);
            }

            try
            {
                OutputString = new StringBuilder();
                //
                //! Create TCP/Process Dictionary
                //
                ProcessTCP = new Dictionary<string, int>();
                FillProcessTcpDictionnary();
                //
                //! Get OLP Infos
                //
                FCellConfig = ACellProgram.CellConfiguration;
                AllOperations = ACellProgram.WorkSequence.LOperation;
                //
                //! Header
                //
                DoHeader(ACellProgram.WorkSequence);

                //
                //! Comment WorkSequence Name
                //
                AddToFile(string.Format(new CultureInfo(AmericanCultureInfo), cszCommentCellProgram, ACellProgram.WorkSequence.Name), true);
                //
                //! Parse All Operations
                //
                ParseAllOperations();
                //
                //! End
                //
                AddToFile(cszEnd, false);
                szOutputFilePath = Path.Combine(AOutputSRCFolderPath, Constants.OlpOutputFileNameWithExt);
                File.WriteAllText(szOutputFilePath, OutputString.ToString());
                FctResult = true;
            }
            catch (Exception ex)
            {
                if (!(ex is SrcGenerationException))
                {
                    throw new SrcGenerationException(string.Format(new CultureInfo(AmericanCultureInfo),
                       "Cannot Save SRC Output File in the Folder <{0}>!", AOutputSRCFolderPath));
                }
                else
                {
                    throw;
                }
            }
            return FctResult;
        }

        /// <summary>
        /// Fill Process / TCP Dictionnary
        /// </summary>
        private static void FillProcessTcpDictionnary()
        {
            ProcessTCP.Add(MacroList.cszDrill, ciTCP1);
            ProcessTCP.Add(MacroList.cszLocateTargetXY, ciTCP2);
            ProcessTCP.Add(MacroList.cszLocateTargetZRxRy, ciTCP3);
            ProcessTCP.Add(MacroList.cszClamp, ciTCP1);
            ProcessTCP.Add(MacroList.cszUnclamp, ciTCP1);
            ProcessTCP.Add(MacroList.cszChangeHead, ciTCP10);
            ProcessTCP.Add(MacroList.cszClearGlobalRelocData, ciTCP2);
            ProcessTCP.Add(MacroList.cszClearLocalRelocData, ciTCP2);
            ProcessTCP.Add(MacroList.cszStoreGlobalData, ciTCP2);
            ProcessTCP.Add(MacroList.cszStoreLocalData, ciTCP2);
            ProcessTCP.Add(MacroList.cszComputeObjectFrame, ciTCP2);
            ProcessTCP.Add(MacroList.cszTakePicture, ciTCP2);
        }

        #endregion Interface

        #region Methods

        /// <summary>
        /// Allow to parse all Operations of the WorkSequence
        /// </summary>
        private static void ParseAllOperations()
        {
            //
            //! Reset COunters
            //
            int iOperationToDo = 0;
            int iOperationCount = 0;
            foreach (Operation aOperation in AllOperations)
            {
                //
                //! Increment Operation Count
                //
                iOperationCount++;
                //
                //! Need To Parse OP
                //
                bool bParseOp = GetOpNeedToBeDone(aOperation);

                if (bParseOp)
                {
                    //
                    //! Inc Operation To Do
                    //
                    iOperationToDo++;
                    //
                    //! Fold Operation
                    //
                    AddToFile(string.Format(new CultureInfo(AmericanCultureInfo), cszCommentFOLDOpNameFmt, aOperation.Name), true);
                    //
                    //! Count Working Point
                    //
                    int WorkingPointCount = 0;
                    foreach (RobotPoint APoint in aOperation.RobotPoints.LRobotPoint)
                    {
                        bool IsTrajOnly = (APoint.Type.ToLower() == cszTrajOnly.ToLower());
                        bool IsPointSkipped = (APoint.WorkMode == RobotPointWorkMode.Skip);
                        bool IsPointDone = (APoint.WorkStatus != RobotPointWorkStatus.Idle);
                        bool IsNop = (APoint.ProcessData.Process.ToLower() == MacroList.cszNOP.ToLower());
                        if ((!IsTrajOnly) && (!IsPointSkipped) && (!IsPointDone) && (!IsNop))
                        {
                            WorkingPointCount++;
                        }
                    }
                    //
                    //! Effector To Use
                    //
                    DefineEffectorToUse(aOperation);
                    //
                    //! Update Operation Data
                    //
                    AddToFile(cszCommentUpdateOpData, false);
                    AddToFile(string.Format(new CultureInfo(AmericanCultureInfo), cszDoUpdateOperationDataFmt, iOperationCount, aOperation.Name, WorkingPointCount, cszTrue), true);
                    //
                    //! Halt If Necessary
                    //
                    if (aOperation.WorkMode == OperationWorkMode.HaltAndProcess)
                    {
                        AddHalt();
                    }
                    //
                    //! Correctors
                    //
                    //
                    DoAddCorrectors(aOperation);
                    //
                    //! Robot Points
                    //
                    DoAddRobotPoints(aOperation);
                    //
                    //! Halt If Necessary
                    //
                    if (aOperation.WorkMode == OperationWorkMode.ProcessAndHalt)
                    {
                        AddHalt();
                    }
                    //
                    //! Update Operation Data
                    //
                    AddToFile(cszCommentUpdateOpData, false);
                    AddToFile(string.Format(new CultureInfo(AmericanCultureInfo), cszDoUpdateOperationDataFmt, iOperationCount, aOperation.Name, WorkingPointCount, cszFalse), true);
                    //
                    //! Get Back To GP0 if needed
                    //
                    EndOperationGP0(aOperation);
                    //
                    //! End Fold Operation
                    //
                    AddToFile(string.Format(new CultureInfo(AmericanCultureInfo), cszCommentENDFOLDOpNameFmt, aOperation.Name), true);
                }
            }
            //
            //! Define Can Load Next Job
            //
            DefineCanLoadNextJob();
            //
            //! Send Robot To GP0
            //
            if (iOperationToDo > 0)
            {
                AddToFile(cszCommentReturnGP0, false);
                AddToFile(cszGP0, true);
            }
        }

        private static bool GetOpNeedToBeDone(Operation AOperation)
        {
            return (AOperation.WorkMode == OperationWorkMode.Run ||
                    AOperation.WorkMode == OperationWorkMode.HaltAndProcess ||
                    AOperation.WorkMode == OperationWorkMode.ProcessAndHalt) &&
                   (AOperation.WorkStatus == OperationWorkStatus.Idle ||
                   AOperation.WorkStatus == OperationWorkStatus.InProgress);
        }

        private static void EndOperationGP0(Operation aOperation)
        {
            //! Get index of Op
            int Idx = AllOperations.IndexOf(aOperation);

            //! Check If it last Op to be done
            //! No Need to Add Return if so, it will be done at the end of "ParseAllOperations"
            bool OtherOpToDo = false;
            for (int i = (Idx + 1); i < AllOperations.Count; i++)
            {
                if (!OtherOpToDo)
                {
                    if (GetOpNeedToBeDone(AllOperations[i]))
                    {
                        OtherOpToDo = true;
                    }
                }
            }

            if (OtherOpToDo)
            {
                if (Idx < (AllOperations.Count - 1))
                {
                    //! Get Next Op
                    Operation FNextOperation = AllOperations[Idx + 1];

                    bool bNoNeedToDoOp = ((FNextOperation.WorkMode == OperationWorkMode.Skip) ||
                                          (FNextOperation.WorkStatus == OperationWorkStatus.Done) ||
                                          (FNextOperation.WorkStatus == OperationWorkStatus.DoneWithError));

                    if (bNoNeedToDoOp)
                    {
                        AddToFile(cszCommentReturnGP0, false);
                        AddToFile(cszGP0, true);
                    }
                }
            }
        }

        /// <summary>
        /// Add Robot Points
        /// </summary>
        /// <param name="AOperation"></param>
        private static void DoAddRobotPoints(Operation AOperation)
        {
            int iRobotPointCount = 0;
            foreach (RobotPoint ARobotPoint in AOperation.RobotPoints.LRobotPoint)
            {
                //
                //! Inc Robot Point Count
                //
                iRobotPointCount++;
                //
                //! Comment Point Name
                //
                AddToFile(string.Format(new CultureInfo(AmericanCultureInfo), cszCommentFOLDPointName, ARobotPoint.Name, ARobotPoint.Type), true);
                //
                //! Define Traj Only
                //
                if (ARobotPoint.Type.ToLower() != cszTrajOnly.ToLower())
                {
                    AddToFile(string.Format(new CultureInfo(AmericanCultureInfo), cszDefinePointIsTrajOnly, cszFalse.ToUpper()), true);
                }
                else
                {
                    AddToFile(string.Format(new CultureInfo(AmericanCultureInfo), cszDefinePointIsTrajOnly, cszTrue.ToUpper()), true);
                }
                //
                //! Update Point Data
                //
                AddToFile(cszCommentUpdatePointData, false);
                AddToFile(
                   string.Format(new CultureInfo(AmericanCultureInfo), cszDoUpdatePointDataFmt, (int)ARobotPoint.WorkStatus, iRobotPointCount,
                      ARobotPoint.Name, ciProcessStart), true);
                //
                //! Robot Speed
                //
                DefineRobotSpeed(ARobotPoint);
                //
                //! Define Escape Frame
                //
                DefineEscapeFrame(ARobotPoint);
                //
                //! Define Offset Frame
                //
                DefineOffsetFrame(ARobotPoint);
                //
                //! Pattern Name
                //
                DefinePatternName(ARobotPoint);
                //
                //! Move Type
                //
                DefineMoveType(ARobotPoint);
                //
                //! Adjust Mode Depending on  Process Name
                //
                ParseLocationAndProcess(ARobotPoint);
                //
                //! Update Point Data
                //
                AddToFile(cszCommentUpdatePointData, false);
                AddToFile(
                   string.Format(new CultureInfo(AmericanCultureInfo), cszDoUpdatePointDataFmt, Constants.CurrentPointStatus, iRobotPointCount,
                      ARobotPoint.Name, ciProcessEnd), true);
                //
                //! End Fold Point
                //
                AddToFile(string.Format(new CultureInfo(AmericanCultureInfo), cszCommentENDFOLDPointName, ARobotPoint.Name), true);
            }
        }

        /// <summary>
        /// Define Robot Speed Given By OLP
        /// </summary>
        /// <param name="aRobotPoint"></param>
        private static void DefineRobotSpeed(RobotPoint ARobotPoint)
        {
            try
            {
                if (ARobotPoint.MotionData.MotionSpeed != null)
                {
                    AddToFile(cszCommentOLPSpeed, false);
                    int Speed;
                    if (Int32.TryParse(ARobotPoint.MotionData.MotionSpeed, out Speed))
                    {
                        AddToFile(
                           string.Format(new CultureInfo(AmericanCultureInfo), cszDefineOLPSpeedFmt,
                              ARobotPoint.MotionData.MotionSpeed), true);
                    }
                }
            }
            catch
            {
                throw new SrcGenerationException("An error occured while defining Robot speed");
            }
        }

        /// <summary>
        /// Add Correctors
        /// </summary>
        /// <param name="AOperation"></param>
        private static void DoAddCorrectors(Operation AOperation)
        {
            int CorrectorCount = 0;
            //! Get All Correctors of Op
            //
            ObservableCollection<CorrectorData> FOperationCorrectors =
               KrlGeneratorUtils.GetAllCorrectorFromOperation(AOperation);
            if (FOperationCorrectors != null)
            {
                AddToFile(cszCommentOPCorrectors, false);
                foreach (CorrectorData ACorrector in FOperationCorrectors)
                {
                    //
                    //! Increment Corrector Count
                    //
                    CorrectorCount++;
                    //
                    //! Add Correctors Name and Type
                    //
                    try
                    {
                        int TargetCount;
                        TargetCount = ACorrector.Targets == null ? 0 : ACorrector.Targets.Count;
                        AddToFile(
                           string.Format(new CultureInfo(AmericanCultureInfo), cszAddOpCorrectorFmt, CorrectorCount,
                              ACorrector.Name, ACorrector.Type, TargetCount), true);
                    }
                    catch
                    {
                        throw new SrcGenerationException("An error occured while adding correctors");
                    }

                    //
                    //! Add Targets
                    //
                    try
                    {
                        if (ACorrector.Targets != null)
                        {
                            AddToFile(cszCommentOPTargets, false);
                            foreach (Target ATarget in ACorrector.Targets)
                            {
                                string lTargetFormated = FormatE6Pos(GetListOfTokenFromOlpFrame(ATarget.Location));
                                AddToFile(
                                   string.Format(new CultureInfo(AmericanCultureInfo), cszAddOperationTargetFmt,
                                      CorrectorCount,
                                      lTargetFormated), true);
                            }
                        }
                    }
                    catch
                    {
                        throw new SrcGenerationException("An error occured while adding corrector <" + ACorrector.Name +
                                                         "> targets");
                    }
                }
            }
        }

        /// <summary>
        /// Define Offset Frame
        /// </summary>
        /// <param name="aRobotPoint"></param>
        private static void DefineOffsetFrame(RobotPoint ARobotPoint)
        {
            try
            {
                if (ARobotPoint.MotionData.OffsetFrame != null)
                {
                    if (ARobotPoint.MotionData.MotionType.ToLower() != cszJoint.ToLower())
                    {
                        string lOffsetFrameFormated =
                           FormatFrame(GetListOfTokenFromOlpFrame(ARobotPoint.MotionData.OffsetFrame));
                        AddToFile(cszCommentOffsetFrame, false);
                        AddToFile(
                           string.Format(new CultureInfo(AmericanCultureInfo), cszDefineOffsetFrameFmt, lOffsetFrameFormated),
                           true);
                    }
                }
            }
            catch
            {
                throw new SrcGenerationException("An error occured while defining offset frame");
            }
        }

        /// <summary>
        /// Define Pattern Name
        /// </summary>
        /// <param name="aRobotPoint"></param>
        private static void DefinePatternName(RobotPoint ARobotPoint)
        {
            try
            {
                if (ARobotPoint.ProcessData.PatternName != null)
                {
                    if (ARobotPoint.ProcessData.PatternName != "")
                    {
                        AddToFile(cszCommentPatternName, false);
                        AddToFile(
                           string.Format(new CultureInfo(AmericanCultureInfo), cszDefinePatternName,
                              ARobotPoint.ProcessData.PatternName), true);
                    }
                }
            }
            catch
            {
                throw new SrcGenerationException("An error occured while defining pattern name");
            }
        }

        /// <summary>
        /// Define Escape France
        /// </summary>
        /// <param name="ARobotPoint"></param>
        private static void DefineEscapeFrame(RobotPoint ARobotPoint)
        {
            try
            {
                if (ARobotPoint.MotionData.EscapeFrame != null)
                {
                    if (ARobotPoint.MotionData.MotionType.ToLower() != cszJoint.ToLower())
                    {
                        string lEscapeFrameFormated =
                           FormatFrame(GetListOfTokenFromOlpFrame(ARobotPoint.MotionData.EscapeFrame));
                        AddToFile(cszCommentEscapeFrame, false);
                        AddToFile(
                           string.Format(new CultureInfo(AmericanCultureInfo), cszDefineEscapeFrameFmt, lEscapeFrameFormated),
                           true);
                    }
                }
            }
            catch
            {
                throw new SrcGenerationException("An error occured while defining escape frame");
            }
        }

        /// <summary>
        /// Display Message From OLP
        /// </summary>
        /// <param name="ARobotPoint"></param>
        private static void DefineRobotPointMessage(RobotPoint ARobotPoint)
        {
            try
            {
                if (ARobotPoint.ProcessData.Message != null)
                {
                    if (ARobotPoint.ProcessData.Message != "")
                    {
                        AddToFile(
                           string.Format(new CultureInfo(AmericanCultureInfo), cszDefineOLPMessageFmt,
                              ARobotPoint.ProcessData.Message), true);
                    }
                }
            }
            catch
            {
                throw new SrcGenerationException("An error occured while defining robot point message");
            }
        }

        /// <summary>
        /// Define Move Type
        /// </summary>
        /// <param name="aRobotPoint"></param>
        private static void DefineMoveType(RobotPoint ARobotPoint)
        {
            try
            {
                AddToFile(cszCommentMoveType, false);
                AddToFile(string.Format(new CultureInfo(AmericanCultureInfo), cszDefineMoveType, ARobotPoint.MotionData.MotionType), true);
            }
            catch
            {
                throw new SrcGenerationException("An error occured while defining pattern Move Type");
            }
        }

        /// <summary>
        /// Add Halt
        /// </summary>
        private static void AddHalt()
        {
            AddToFile(cszCommentHaltProgram, false);
            AddToFile(cszHalt, true);
        }

        /// <summary>
        /// Create SRC Code depending on the Process Name
        /// </summary>
        /// <param name="AOperation"></param>
        /// <param name="ARobotPointData"></param>
        /// <param name="AEscapeFrame"></param>
        private static void ParseLocationAndProcess(RobotPoint ARobotPoint)
        {
            bool IsTrajOnly = (ARobotPoint.Type.ToLower() == cszTrajOnly.ToLower());
            bool IsPointSkipped = (ARobotPoint.WorkMode == RobotPointWorkMode.Skip);
            bool IsPointDone = (ARobotPoint.WorkStatus != RobotPointWorkStatus.Idle);

            //
            //! Cannot Change Process of Joint Robot Points
            //
            if (IsTrajOnly)
            {
                AddOlpProcess(ARobotPoint);
            }
            else
            {
                if (IsPointSkipped || IsPointDone)
                {
                    DoNopNeutralized(ARobotPoint);
                }
                else
                {
                    AddOlpProcess(ARobotPoint);
                }
            }
        }

        private static void AddOlpProcess(RobotPoint ARobotPoint)
        {
            string ProcessLowerCase = ARobotPoint.ProcessData.Process.ToLower();
            //
            //! Nop Process
            //
            if (ProcessLowerCase.IndexOf(MacroList.cszNOP) != -1)
            {
                if (ProcessLowerCase == MacroList.cszNOP)
                {
                    DoNop(ARobotPoint, ciTCP1);
                }
                else
                {
                    int TcpId = -1;
                    for (int i = 0; i < ciMaxTcpId; i++)
                    {
                        if (ProcessLowerCase == (MacroList.cszNOP + i.ToString()))
                        {
                            TcpId = i;
                        }
                    }
                    if (TcpId != -1)
                    {
                        DoNop(ARobotPoint, TcpId);
                    }
                    else
                    {
                        DoNop(ARobotPoint, ciTCP1);
                    }
                }
            }
            else
            {
                switch (ProcessLowerCase)
                {
                    case MacroList.cszClearGlobalRelocData:
                        DoClearGlobalRelocData(ARobotPoint);
                        break;

                    case MacroList.cszClearLocalRelocData:
                        DoClearLocalRelocData(ARobotPoint);
                        break;

                    case MacroList.cszLocateTargetXY:
                        DoLocateTargetXy(ARobotPoint);
                        break;

                    case MacroList.cszLocateTargetZRxRy:
                        DoLocateTargetZRxRy(ARobotPoint);
                        break;

                    case MacroList.cszTakePicture:
                        DoTakePicture(ARobotPoint);
                        break;

                    case MacroList.cszComputeObjectFrame:
                        DoComputeObjectFrame(ARobotPoint);
                        break;

                    default:
                        DoMultiMacro(ARobotPoint);
                        break;
                }
            }
        }

        /// <summary>
        /// Add ExecuteProcess Command To SRC file
        /// </summary>
        /// <param name="AProcessName"></param>
        /// <param name="AOperationID"></param>
        /// <param name="AOperation"></param>
        /// <param name="ACurrentPointID"></param>
        /// <param name="ARobotPointData"></param>
        private static void AddExecuteProcess(string AProcessName, RobotPoint ARobotPoint, bool AStopAdvance, bool AManageHalt)
        {
            if (AManageHalt)
            {
                //
                //! Halt If Necessary
                //
                if (ARobotPoint.WorkMode == RobotPointWorkMode.HaltAndProcess)
                {
                    AddHalt();
                }
            }

            //
            //! OLP Message
            //
            DefineRobotPointMessage(ARobotPoint);
            //
            //! Process
            //
            string StopAdvance;
            if (AStopAdvance)
            {
                StopAdvance = cszTrue;
            }
            else
            {
                StopAdvance = cszFalse;
            }
            AddToFile(string.Format(new CultureInfo(AmericanCultureInfo), cszExecuteProcessFmt, AProcessName, StopAdvance), true);
            if (AManageHalt)
            {
                //
                //! Halt If Necessary
                //
                if (ARobotPoint.WorkMode == RobotPointWorkMode.ProcessAndHalt)
                {
                    AddHalt();
                }
            }
        }

        /// <summary>
        /// Adjust MoveCarte Function if CartEscape is selected
        /// </summary>
        /// <param name="ARobotPointData"></param>
        /// <param name="ATCPId"></param>
        private static void GoToEscapeFrame(RobotPoint ARobotPoint, int ATCPId)
        {
            try
            {
                if (ARobotPoint.MotionData.MotionType.ToLower() == cszCartEscape.ToLower())
                {
                    AddToFile(cszCommentGoToEscape, false);
                    string lE6PosFormated = FormatE6Pos(GetListOfTokenFromOlpFrame(ARobotPoint.MotionData.Location));
                    AddToFile(
                       string.Format(new CultureInfo(AmericanCultureInfo), cszMoveCartFmt, lE6PosFormated, ATCPId,
                          ARobotPoint.MotionData.CorrectorName, cszTrue), true);
                }
            }
            catch
            {
                throw new SrcGenerationException("An error occured while adding GoToEscapeFrame");
            }
        }

        /// <summary>
        /// Add Move Type To SRC File
        /// </summary>
        /// <param name="ARobotPointData"></param>
        /// <param name="ATCPId"></param>
        private static void GoToRobotPoint(RobotPoint ARobotPoint, int ATCPId)
        {
            try
            {
                if (ARobotPoint.MotionData.MotionType != null)
                {
                    AddToFile(cszCommentGoToPoint, false);
                    if (ARobotPoint.MotionData.MotionType.ToLower() == cszJoint.ToLower())
                    {
                        string lE6AxisFormated = FormatE6Axis(GetListOfTokenFromOlpFrame(ARobotPoint.MotionData.Location));
                        AddToFile(string.Format(new CultureInfo(AmericanCultureInfo), cszMoveJointFmt, lE6AxisFormated),
                           true);
                    }
                    else
                    {
                        string lE6PosFormated = FormatE6Pos(GetListOfTokenFromOlpFrame(ARobotPoint.MotionData.Location));
                        AddToFile(
                           string.Format(new CultureInfo(AmericanCultureInfo), cszMoveCartFmt, lE6PosFormated, ATCPId,
                              ARobotPoint.MotionData.CorrectorName, cszFalse), true);
                    }
                }
            }
            catch
            {
                throw new SrcGenerationException("An error occured while adding GoToRobotPoint");
            }
        }

        /// <summary>
        /// Define Base
        /// </summary>
        private static void DefineOlpObjectFrame()
        {
            try
            {
                string lBaseFormated = FormatFrame(GetListOfTokenFromOlpFrame(FCellConfig.ConfigurationData.ObjectFrame));
                AddToFile(
                   string.Format(new CultureInfo(AmericanCultureInfo), cszDefineOlpObjectFrameFmt, lBaseFormated,
                      FCellConfig.ObjectFrameName, FCellConfig.PartName, FCellConfig.WorkZoneName), false);
            }
            catch
            {
                throw new SrcGenerationException("An error occured while defining Olp Object Frame");
            }
        }

        /// <summary>
        /// Define Effector To Use
        /// </summary>
        private static void DefineEffectorToUse(Operation AOperation)
        {
            try
            {
                AddToFile(cszCommentEffectorToUse, false);
                if (AOperation.Effector != null)
                {
                    AddToFile(
                       string.Format(new CultureInfo(AmericanCultureInfo), cszDefineEffectorToUse, AOperation.Effector),
                       true);
                }
                else
                {
                    AddToFile(string.Format(new CultureInfo(AmericanCultureInfo), cszDefineEffectorToUse, " "), true);
                }
            }
            catch
            {
                throw new SrcGenerationException("An error occured while defining Effector To use");
            }
        }

        /// <summary>
        /// Define Can Load Next Job
        /// </summary>
        private static void DefineCanLoadNextJob()
        {
            AddToFile(cszCommentCanLoadNextJob, false);
            AddToFile(string.Format(new CultureInfo(AmericanCultureInfo), cszDefineLoadNextJob, cszTrue), true);
        }

        /// <summary>
        /// Define OLP Safety Name
        /// </summary>
        private static void DefineOlpSafetyName()
        {
            try
            {
                if (FCellConfig.SafetyName != null)
                {
                    if (FCellConfig.SafetyName != "")
                    {
                        AddToFile(
                           string.Format(new CultureInfo(AmericanCultureInfo), cszDefineOLPSafetyNameFmt,
                              FCellConfig.SafetyName), false);
                    }
                    else
                    {
                        AddToFile(string.Format(new CultureInfo(AmericanCultureInfo), cszDefineOLPSafetyNameFmt, " "), false);
                    }
                }
            }
            catch
            {
                throw new SrcGenerationException("An error occured while defining Olp safety name");
            }
        }

        /// <summary>
        /// Add Src Header File
        /// </summary>
        private static void DoHeader(WorkSequence AWorkSequence)
        {
            try
            {
                AddToFile(cszAccess, false);
                AddToFile(cszRev, false);
                AddToFile(cszProgramPath, false);
                AddToFile(
                   string.Format(new CultureInfo(AmericanCultureInfo), cszProgramNameFmt, Constants.OlpOutputFileName),
                   false);
                AddToFile(cszFoldInit, false);
                AddToFile(cszBasInit, false);
                AddToFile(cszBCO, false);
                DefineOlpSafetyName();
                DefineOlpObjectFrame();
                AddToFile(cszInitFlexDrill, false);
                AddToFile(cszEndFoldInit, true);
            }
            catch
            {
                throw new SrcGenerationException("An error occured while doing src Header");
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

        private static string FormatFrame(List<string> AListOfTokens)
        {
            try
            {
                string lFrame = string.Format(new CultureInfo(AmericanCultureInfo), cszFrameFormatFmt,
                   AListOfTokens[0], AListOfTokens[1], AListOfTokens[2], AListOfTokens[3], AListOfTokens[4],
                   AListOfTokens[5]);
                lFrame = "{" + lFrame + "}";
                return lFrame;
            }
            catch
            {
                throw new SrcGenerationException("An error occured while formating frame");
            }
        }

        private static string FormatE6Axis(List<string> AListOfTokens)
        {
            try
            {
                string lFrame = string.Format(new CultureInfo(AmericanCultureInfo), cszE6AxisFormatFmt,
                   AListOfTokens[0], AListOfTokens[1], AListOfTokens[2], AListOfTokens[3], AListOfTokens[4],
                   AListOfTokens[5]);
                lFrame = "{" + lFrame + "}";
                return lFrame;
            }
            catch
            {
                throw new SrcGenerationException("An error occured while formating E6Axis");
            }
        }

        private static string FormatE6Pos(List<string> AListOfTokens)
        {
            try
            {
                string lFrame = string.Format(new CultureInfo(AmericanCultureInfo), cszE6PosFormatFmt,
                   AListOfTokens[0], AListOfTokens[1], AListOfTokens[2], AListOfTokens[3], AListOfTokens[4],
                   AListOfTokens[5], AListOfTokens[6], AListOfTokens[7]);
                lFrame = "{" + lFrame + "}";
                return lFrame;
            }
            catch
            {
                throw new SrcGenerationException("An error occured while Formating E6Pos");
            }
        }

        /// <summary>
        /// Add Line to file
        /// </summary>
        /// <param name="ALine"></param>
        private static void AddToFile(string ALine, bool AAddEmptyLine)
        {
            if (OutputString != null)
            {
                OutputString.AppendFormat(new CultureInfo(AmericanCultureInfo), "{0}\n", ALine);

                if (AAddEmptyLine)
                {
                    OutputString.Append(" \n");
                }
            }
        }

        #endregion Methods

        #region Process Definition

        private static void DoNopNeutralized(RobotPoint ARobotPoint)
        {
            try
            {
                int TCPID;
                string[] SplittedLine = ARobotPoint.ProcessData.Process.Split('/');
                try
                {
                    TCPID = ProcessTCP[SplittedLine[0].ToLower()];
                }
                catch (KeyNotFoundException)
                {
                    TCPID = ciTCP1;
                }

                if (ARobotPoint.MotionData.MotionType.ToLower() == cszCartEscape.ToLower())
                {
                    //! Go To Escape
                    GoToEscapeFrame(ARobotPoint, TCPID);
                    //! NOP Process
                    AddExecuteProcess(MacroList.cszNOP, ARobotPoint, false, false);
                }
                else if (ARobotPoint.MotionData.MotionType.ToLower() == cszCart.ToLower())
                {
                    //! Go To Point
                    GoToRobotPoint(ARobotPoint, TCPID);
                    //! NOP Process
                    AddExecuteProcess(MacroList.cszNOP, ARobotPoint, false, false);
                }
            }
            catch
            {
                throw new SrcGenerationException("An error occured in NopNeutralized Process");
            }
        }

        private static void DoNop(RobotPoint ARobotPoint, int ATcpId)
        {
            try
            {
                //! Go To Escape
                GoToEscapeFrame(ARobotPoint, ATcpId);
                //! Go To Point
                GoToRobotPoint(ARobotPoint, ATcpId);
                //! NOP Process
                AddExecuteProcess(MacroList.cszNOP, ARobotPoint, false, true);
                //! Go To Escape
                GoToEscapeFrame(ARobotPoint, ATcpId);
            }
            catch
            {
                throw new SrcGenerationException("An error occured in Nop Process");
            }
        }

        private static void DoClearGlobalRelocData(RobotPoint ARobotPoint)
        {
            try
            {
                bool StopAdvance = (ARobotPoint.Type.ToLower() != cszTrajOnly.ToLower());
                int TcpId = GetTcpIdFromDictionnary(MacroList.cszClearGlobalRelocData.ToLower());
                //! Go To Escape
                GoToEscapeFrame(ARobotPoint, TcpId);
                //! Go To Point
                GoToRobotPoint(ARobotPoint, TcpId);
                //! Clear Global Reloc Data Process
                AddExecuteProcess(MacroList.cszClearGlobalRelocData, ARobotPoint, StopAdvance, true);
                //! Go To Escape
                GoToEscapeFrame(ARobotPoint, TcpId);
            }
            catch
            {
                throw new SrcGenerationException("An error occured in DoClearGlobalRelocData Process");
            }
        }

        private static void DoClearLocalRelocData(RobotPoint ARobotPoint)
        {
            try
            {
                bool StopAdvance = (ARobotPoint.Type.ToLower() != cszTrajOnly.ToLower());
                int TcpId = GetTcpIdFromDictionnary(MacroList.cszClearLocalRelocData.ToLower());
                //! Go To Escape
                GoToEscapeFrame(ARobotPoint, TcpId);
                //! Go To Point
                GoToRobotPoint(ARobotPoint, TcpId);
                //! Clear Local Reloc Data Process
                AddExecuteProcess(MacroList.cszClearLocalRelocData, ARobotPoint, StopAdvance, true);
                //! Go To Escape
                GoToEscapeFrame(ARobotPoint, TcpId);
            }
            catch
            {
                throw new SrcGenerationException("An error occured in DoClearLocalRelocData Process");
            }
        }

        private static void DoComputeObjectFrame(RobotPoint ARobotPoint)
        {
            try
            {
                bool StopAdvance = (ARobotPoint.Type.ToLower() != cszTrajOnly.ToLower());
                int TcpId = GetTcpIdFromDictionnary(MacroList.cszComputeObjectFrame.ToLower());
                //! Go To Escape
                GoToEscapeFrame(ARobotPoint, TcpId);
                //! Go To Point
                GoToRobotPoint(ARobotPoint, TcpId);
                //! Compute Object Frame Data Process
                AddExecuteProcess(MacroList.cszComputeObjectFrame, ARobotPoint, StopAdvance, true);
                //! Go To Escape
                GoToEscapeFrame(ARobotPoint, TcpId);
            }
            catch
            {
                throw new SrcGenerationException("An error occured in DoComputeObjectFrame Process");
            }
        }

        private static void DoLocateTargetXy(RobotPoint ARobotPoint)
        {
            try
            {
                bool StopAdvance = (ARobotPoint.Type.ToLower() != cszTrajOnly.ToLower());
                int TcpId = GetTcpIdFromDictionnary(MacroList.cszLocateTargetXY.ToLower());
                //! Go To Escape
                GoToEscapeFrame(ARobotPoint, TcpId);
                //! Go To Point
                GoToRobotPoint(ARobotPoint, TcpId);
                //! Locate Target XY Process
                AddExecuteProcess(MacroList.cszLocateTargetXY, ARobotPoint, StopAdvance, true);
                //! Go To Escape
                GoToEscapeFrame(ARobotPoint, TcpId);
            }
            catch
            {
                throw new SrcGenerationException("An error occured in DoLocateTargetXy Process");
            }
        }

        private static void DoLocateTargetZRxRy(RobotPoint ARobotPoint)
        {
            try
            {
                bool StopAdvance = (ARobotPoint.Type.ToLower() != cszTrajOnly.ToLower());
                int TcpId = GetTcpIdFromDictionnary(MacroList.cszLocateTargetZRxRy.ToLower());
                //! Go To Escape
                GoToEscapeFrame(ARobotPoint, TcpId);
                //! Go To Point
                GoToRobotPoint(ARobotPoint, TcpId);
                //! Locate Target XY Process
                AddExecuteProcess(MacroList.cszLocateTargetZRxRy, ARobotPoint, StopAdvance, true);
                //! Go To Escape
                GoToEscapeFrame(ARobotPoint, TcpId);
            }
            catch
            {
                throw new SrcGenerationException("An error occured in DoLocateTargetZRxRy Process");
            }
        }

        private static void DoTakePicture(RobotPoint ARobotPoint)
        {
            try
            {
                bool StopAdvance = (ARobotPoint.Type.ToLower() != cszTrajOnly.ToLower());
                int TcpId = GetTcpIdFromDictionnary(MacroList.cszTakePicture.ToLower());
                //! Go To Escape
                GoToEscapeFrame(ARobotPoint, TcpId);
                //! Go To Point
                GoToRobotPoint(ARobotPoint, TcpId);
                //! Take Picture
                AddExecuteProcess(MacroList.cszTakePicture, ARobotPoint, StopAdvance, true);
                //! Go To Escape
                GoToEscapeFrame(ARobotPoint, TcpId);
            }
            catch
            {
                throw new SrcGenerationException("An error occured in DoTakePicture Process");
            }
        }

        private static void DoMultiMacro(RobotPoint ARobotPoint)
        {
            bool StopAdvance = (ARobotPoint.Type.ToLower() != cszTrajOnly.ToLower());

            string[] SplittedLine = ARobotPoint.ProcessData.Process.Split('/');

            int TcpId = GetTcpIdFromDictionnary(SplittedLine[0].ToLower());
            int prevTCPId = -1;

            //! Go To Escape with TCP of First Macro
            GoToEscapeFrame(ARobotPoint, TcpId);

            //
            //! Halt If Necessary
            //
            if (ARobotPoint.WorkMode == RobotPointWorkMode.HaltAndProcess)
            {
                AddHalt();
            }

            foreach (string AItem in SplittedLine)
            {
                TcpId = GetTcpIdFromDictionnary(AItem.ToLower());
                if (TcpId != prevTCPId)
                {
                    //! Go To Point
                    GoToRobotPoint(ARobotPoint, TcpId);
                }
                //! Process
                AddExecuteProcess(AItem.ToLower(), ARobotPoint, StopAdvance, false);
                prevTCPId = TcpId;
            }

            //
            //! Halt If Necessary
            //
            if (ARobotPoint.WorkMode == RobotPointWorkMode.ProcessAndHalt)
            {
                AddHalt();
            }

            //! Go To Escape with TCP of last macro
            GoToEscapeFrame(ARobotPoint, TcpId);
        }

        private static int GetTcpIdFromDictionnary(string AProcessName)
        {
            int TcpId;
            try
            {
                TcpId = ProcessTCP[AProcessName.ToLower()];
            }
            catch (KeyNotFoundException)
            {
                TcpId = ciTCP1;
            }
            return TcpId;
        }

        private static bool ProcessNeedsTcp(string AProcessName)
        {
            string ProcessLowerCase = AProcessName.ToLower();
            return ((ProcessLowerCase == MacroList.cszLocateTargetXY.ToLower()) ||
                    (ProcessLowerCase == MacroList.cszLocateTargetZRxRy.ToLower()) ||
                    (ProcessLowerCase == MacroList.cszTakePicture.ToLower()) ||
                    (ProcessLowerCase == MacroList.cszClamp.ToLower()) ||
                    (ProcessLowerCase == MacroList.cszUnclamp.ToLower()) ||
                    (ProcessLowerCase.IndexOf(MacroList.cszNOP) != -1) ||
                    (ProcessLowerCase.IndexOf(MacroList.cszDrill) != -1));
        }

        #endregion Process Definition
    }
}