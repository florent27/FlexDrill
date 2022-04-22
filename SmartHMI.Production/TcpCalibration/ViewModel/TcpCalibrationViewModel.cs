using Kuka.FlexDrill.SmartHMI.Production.Base;
using Kuka.FlexDrill.SmartHMI.Production.Helper;
using KukaRoboter.CoreUtil.Windows.Input;
using KUKARoboter.KRCModel.Robot.Interpreter;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Threading;

namespace Kuka.FlexDrill.SmartHMI.Production.TcpCalibration.ViewModel
{
    public class TcpCalibrationViewModel : FlexDrillViewModelBase
    {
        #region Constants and Fields

        [DllImport("Dll_IdentificationTCP.dll")]
        public static extern bool delta_TCP_Tool_XY(double[] angles, double[] mesures, int sizeTool, int direction_accostage, out double delta_X, out double delta_Y, out double score);

        [DllImport("Dll_IdentificationTCP.dll")]
        public static extern bool delta_TCP_Tool_Z(double[] angles, double[] mesures, int sizeTool, out double delta_Z, out double score);

        [DllImport("Dll_IdentificationTCP.dll")]
        public static extern bool delta_TCP_Tool_BC(double delta_Xb, double delta_Yb, double delta_Xh, double delta_Yh, double d, out double delta_B, out double delta_C, out double score);

        [DllImport("Dll_IdentificationTCP.dll")]
        public static extern bool delta_TCP_Vision_A(double[] X1, double[] Y1, double[] X2, double[] Y2, int sizeVisionA, out double alpha, out double score);

        [DllImport("Dll_IdentificationTCP.dll")]
        public static extern bool delta_TCP_Vision_XY(double[] X, double[] Y, int sizeVisionXY, double alpha, out double delta_X, out double delta_Y, out double score);

        private const int XPlus = 1;
        private const int XMinus = 2;
        private const int YPlus = 3;
        private const int YMinus = 4;
        private const int TCPDeltaXY = 1;
        private const int TCPDeltaZ = 2;
        private const int TCPDeltaRxRy = 3;
        private const int TCPAllCalib = 4;
        private const int TCPDeltaVisionXY = 5;

        private double tCPXYDx;
        private double tCPXYDy;
        private double tCPXYScore;
        private RelayCommand tCPCalibXYCommand;
        private RelayCommand saveCalibXYCommand;

        private double tCPZDz;
        private double tCPZScore;
        private RelayCommand tCPCalibZCommand;
        private RelayCommand saveCalibZCommand;

        private double tCPRxRyDRx;
        private double tCPRxRyDRy;
        private double tCPRxRyScore;
        private RelayCommand tCPCalibRxRyCommand;
        private RelayCommand saveCalibRxRyCommand;

        private double tCPVisionDx;
        private double tCPVisionDy;
        private double tCPVisionDRx;
        private double tCPVisionDRy;
        private RelayCommand tCPCalibVisionCommand;
        private RelayCommand saveCalibVisionCommand;

        private RelayCommand tCPCalibAllCommand;
        private RelayCommand saveAllCalibCommand;

        private DispatcherTimer RefreshValueTimer;

        #endregion Constants and Fields

        #region Constructors and Destructor

        public TcpCalibrationViewModel() : base("FlexDrill_TcpCalibration")
        {
        }

        #endregion Constructors and Destructor

        #region Interface

        /// <inheritdoc />
        public override void Initialize()
        {
            base.Initialize();
        }

        public void InitializePlugin()
        {
            RegisterEvents();
            RefreshValueTimer = new DispatcherTimer(DispatcherPriority.DataBind)
            {
                Interval = TimeSpan.FromMilliseconds(500)
            };
            RefreshValueTimer.Tick += RefreshOffsetValues;
            RefreshValueTimer.Start();
        }

        public double TCPXYDx
        {
            get { return tCPXYDx; }
            set
            {
                if (tCPXYDx != value)
                {
                    tCPXYDx = value;
                    FirePropertyChanged("TCPXYDx");
                }
            }
        }

        public double TCPXYDy
        {
            get { return tCPXYDy; }
            set
            {
                if (tCPXYDy != value)
                {
                    tCPXYDy = value;
                    FirePropertyChanged("TCPXYDy");
                }
            }
        }

        public double TCPXYScore
        {
            get { return tCPXYScore; }
            set
            {
                if (tCPXYScore != value)
                {
                    tCPXYScore = value;
                    FirePropertyChanged("TCPXYScore");
                }
            }
        }

        public double TCPZDz
        {
            get { return tCPZDz; }
            set
            {
                if (tCPZDz != value)
                {
                    tCPZDz = value;
                    FirePropertyChanged("TCPZDz");
                }
            }
        }

        public double TCPZScore
        {
            get { return tCPZScore; }
            set
            {
                if (tCPZScore != value)
                {
                    tCPZScore = value;
                    FirePropertyChanged("TCPZScore");
                }
            }
        }

        public double TCPRxRyDRx
        {
            get { return tCPRxRyDRx; }
            set
            {
                if (tCPRxRyDRx != value)
                {
                    tCPRxRyDRx = value;
                    FirePropertyChanged("TCPRxRyDRx");
                }
            }
        }

        public double TCPRxRyDRy
        {
            get { return tCPRxRyDRy; }
            set
            {
                if (tCPRxRyDRy != value)
                {
                    tCPRxRyDRy = value;
                    FirePropertyChanged("TCPRxRyDRy");
                }
            }
        }

        public double TCPRxRyScore
        {
            get { return tCPRxRyScore; }
            set
            {
                if (tCPRxRyScore != value)
                {
                    tCPRxRyScore = value;
                    FirePropertyChanged("TCPRxRyScore");
                }
            }
        }

        public double TCPVisionDx
        {
            get { return tCPVisionDx; }
            set
            {
                if (tCPVisionDx != value)
                {
                    tCPVisionDx = value;
                    FirePropertyChanged("TCPVisionDx");
                }
            }
        }

        public double TCPVisionDy
        {
            get { return tCPVisionDy; }
            set
            {
                if (tCPVisionDy != value)
                {
                    tCPVisionDy = value;
                    FirePropertyChanged("TCPVisionDy");
                }
            }
        }

        public double TCPVisionDRx
        {
            get { return tCPVisionDRx; }
            set
            {
                if (tCPVisionDRx != value)
                {
                    tCPVisionDRx = value;
                    FirePropertyChanged("TCPVisionDRx");
                }
            }
        }

        public double TCPVisionDRy
        {
            get { return tCPVisionDRy; }
            set
            {
                if (tCPVisionDRy != value)
                {
                    tCPVisionDRy = value;
                    FirePropertyChanged("TCPVisionDRy");
                }
            }
        }

        public RelayCommand TCPCalibXYCommand
        {
            get
            {
                if (tCPCalibXYCommand == null)
                {
                    tCPCalibXYCommand = new RelayCommand(action => StartTCPCalibration(TCPDeltaXY), cond => CanStartTCPCalibration);
                }

                return tCPCalibXYCommand;
            }
        }

        public RelayCommand SaveCalibXYCommand
        {
            get
            {
                if (saveCalibXYCommand == null)
                {
                    saveCalibXYCommand = new RelayCommand(action => SaveTCPCalibration(TCPDeltaXY), cond => CanSaveTCPCalibration);
                }

                return saveCalibXYCommand;
            }
        }

        public RelayCommand TCPCalibZCommand
        {
            get
            {
                if (tCPCalibZCommand == null)
                {
                    tCPCalibZCommand = new RelayCommand(action => StartTCPCalibration(TCPDeltaZ), cond => CanSaveTCPCalibration);
                }

                return tCPCalibZCommand;
            }
        }

        public RelayCommand SaveCalibZCommand
        {
            get
            {
                if (saveCalibZCommand == null)
                {
                    saveCalibZCommand = new RelayCommand(action => SaveTCPCalibration(TCPDeltaZ), cond => CanSaveTCPCalibration);
                }

                return saveCalibZCommand;
            }
        }

        public RelayCommand TCPCalibRxRyCommand
        {
            get
            {
                if (tCPCalibRxRyCommand == null)
                {
                    tCPCalibRxRyCommand = new RelayCommand(action => StartTCPCalibration(TCPDeltaRxRy), cond => CanStartTCPCalibration);
                }

                return tCPCalibRxRyCommand;
            }
        }

        public RelayCommand SaveCalibRxRyCommand
        {
            get
            {
                if (saveCalibRxRyCommand == null)
                {
                    saveCalibRxRyCommand = new RelayCommand(action => SaveTCPCalibration(TCPDeltaRxRy), cond => CanSaveTCPCalibration);
                }

                return saveCalibRxRyCommand;
            }
        }

        public RelayCommand TCPCalibVisionCommand
        {
            get
            {
                if (tCPCalibVisionCommand == null)
                {
                    tCPCalibVisionCommand = new RelayCommand(action => StartTCPCalibration(TCPDeltaVisionXY), cond => CanStartTCPCalibration);
                }

                return tCPCalibVisionCommand;
            }
        }

        public RelayCommand SaveCalibVisionCommand
        {
            get
            {
                if (saveCalibVisionCommand == null)
                {
                    saveCalibVisionCommand = new RelayCommand(action => SaveTCPCalibration(TCPDeltaVisionXY), cond => CanSaveTCPCalibration);
                }

                return saveCalibVisionCommand;
            }
        }

        public RelayCommand TCPCalibAllCommand
        {
            get
            {
                if (tCPCalibAllCommand == null)
                {
                    tCPCalibAllCommand = new RelayCommand(action => StartTCPCalibration(TCPAllCalib), cond => CanStartTCPCalibration);
                }

                return tCPCalibAllCommand;
            }
        }

        public RelayCommand SaveAllCalibCommand
        {
            get
            {
                if (saveAllCalibCommand == null)
                {
                    saveAllCalibCommand = new RelayCommand(action => SaveTCPCalibration(TCPAllCalib), cond => CanSaveTCPCalibration);
                }

                return saveAllCalibCommand;
            }
        }

        public bool CanStartTCPCalibration
        {
            get
            {
                return (Robot.Interpreters[InterpreterTypes.Robot].ProgramState == ProStates.Free);
            }
        }

        public bool CanSaveTCPCalibration
        {
            get
            {
                return (Robot.Interpreters[InterpreterTypes.Robot].ProgramState == ProStates.Free);
            }
        }

        #endregion Interface

        #region Methods

        private void StartTCPCalibration(int ACalibrationType)
        {
            FlexDrillService.StartTCPCalibration(ACalibrationType);
        }

        private void SaveTCPCalibration(int ACalibrationType)
        {
            KrlVarHandler.WriteVariable(KrlVariableNames.TCPCalibrationType, ACalibrationType, Robot);
            KrlVarHandler.WriteVariable(KrlVariableNames.SaveTCPCalibration, true, Robot);
        }

        private void RefreshOffsetValues(object sender, EventArgs e)
        {
            ReadValues();
        }

        private void ReadValues()
        {
            try
            {
                TCPXYDx = KrlVarHandler.ReadRealVariable(KrlVariableNames.TCPCalibXYDx, Robot);
                TCPXYDy = KrlVarHandler.ReadRealVariable(KrlVariableNames.TCPCalibXYDy, Robot);
                TCPXYScore = KrlVarHandler.ReadRealVariable(KrlVariableNames.TCPCalibXYScore, Robot);
                TCPZDz = KrlVarHandler.ReadRealVariable(KrlVariableNames.TCPCalibZDz, Robot);
                TCPZScore = KrlVarHandler.ReadRealVariable(KrlVariableNames.TCPCalibZScore, Robot);
                TCPRxRyDRx = KrlVarHandler.ReadRealVariable(KrlVariableNames.TCPCalibRxRyDRx, Robot);
                TCPRxRyDRy = KrlVarHandler.ReadRealVariable(KrlVariableNames.TCPCalibRxRyDRy, Robot);
                TCPRxRyScore = KrlVarHandler.ReadRealVariable(KrlVariableNames.TCPCalibRxRyScore, Robot);
                TCPVisionDx = KrlVarHandler.ReadRealVariable(KrlVariableNames.TCPCalibVisionXYDx, Robot);
                TCPVisionDy = KrlVarHandler.ReadRealVariable(KrlVariableNames.TCPCalibVisionXYDy, Robot);
                TCPVisionDRx = KrlVarHandler.ReadRealVariable(KrlVariableNames.TCPCalibVisionXYDRx, Robot);
                TCPVisionDRy = KrlVarHandler.ReadRealVariable(KrlVariableNames.TCPCalibVisionXYDRy, Robot);
            }
            catch (Exception e)
            {
                FlexDrillService.Log.WriteMessage(TraceEventType.Error, "Exception on ReadValues. " + e.Message + ".", e.StackTrace);
            }
        }

        private void RegisterEvents()
        {
            Robot.KRLVariables[KrlVariableNames.ComputeXYMeasures].Changed += OnComputeXYMeasuresChanged;
            Robot.KRLVariables[KrlVariableNames.ComputeZMeasures].Changed += OnComputeZMeasuresChanged;
            Robot.KRLVariables[KrlVariableNames.ComputeRxRyMeasures].Changed += OnComputeRxRyMeasuresChanged;
            Robot.KRLVariables[KrlVariableNames.ComputeRxRy1Measures].Changed += OnComputeRxRy1MeasuresChanged;
            Robot.KRLVariables[KrlVariableNames.ComputeRxRy2Measures].Changed += OnComputeRxRy2MeasuresChanged;
            Robot.KRLVariables[KrlVariableNames.ComputeVisionXY1Measures].Changed += OnComputeVisionXY1MeasuresChanged;
            Robot.KRLVariables[KrlVariableNames.ComputeVisionXY2Measures].Changed += OnComputeVisionXY2MeasuresChanged;
        }

        internal void ReleaseEvents()
        {
            RefreshValueTimer.Stop();
            Robot.KRLVariables[KrlVariableNames.ComputeXYMeasures].Changed -= OnComputeXYMeasuresChanged;
            Robot.KRLVariables[KrlVariableNames.ComputeZMeasures].Changed -= OnComputeZMeasuresChanged;
            Robot.KRLVariables[KrlVariableNames.ComputeRxRyMeasures].Changed -= OnComputeRxRyMeasuresChanged;
            Robot.KRLVariables[KrlVariableNames.ComputeRxRy1Measures].Changed -= OnComputeRxRy1MeasuresChanged;
            Robot.KRLVariables[KrlVariableNames.ComputeRxRy2Measures].Changed -= OnComputeRxRy2MeasuresChanged;
            Robot.KRLVariables[KrlVariableNames.ComputeVisionXY1Measures].Changed -= OnComputeVisionXY1MeasuresChanged;
            Robot.KRLVariables[KrlVariableNames.ComputeVisionXY2Measures].Changed -= OnComputeVisionXY2MeasuresChanged;
        }

        private void OnComputeXYMeasuresChanged(object sender, EventArgs e)
        {
            if (KrlVarHandler.ReadBoolVariable(KrlVariableNames.ComputeXYMeasures, Robot))
            {
                ComputeDeltaXY();
            }
        }

        private void OnComputeZMeasuresChanged(object sender, EventArgs e)
        {
            if (KrlVarHandler.ReadBoolVariable(KrlVariableNames.ComputeZMeasures, Robot))
            {
                ComputeDeltaZ();
            }
        }

        private void OnComputeRxRy1MeasuresChanged(object sender, EventArgs e)
        {
            if (KrlVarHandler.ReadBoolVariable(KrlVariableNames.ComputeRxRy1Measures, Robot))
            {
                ComputeRxRy1();
            }
        }

        private void OnComputeRxRy2MeasuresChanged(object sender, EventArgs e)
        {
            if (KrlVarHandler.ReadBoolVariable(KrlVariableNames.ComputeRxRy2Measures, Robot))
            {
                ComputeRxRy2();
            }
        }

        private void OnComputeRxRyMeasuresChanged(object sender, EventArgs e)
        {
            if (KrlVarHandler.ReadBoolVariable(KrlVariableNames.ComputeRxRyMeasures, Robot))
            {
                ComputeDeltaRxRy();
            }
        }

        private void OnComputeVisionXY1MeasuresChanged(object sender, EventArgs e)
        {
            if (KrlVarHandler.ReadBoolVariable(KrlVariableNames.ComputeVisionXY1Measures, Robot))
            {
                ComputeVisionDeltaXY1();
            }
        }

        private void OnComputeVisionXY2MeasuresChanged(object sender, EventArgs e)
        {
            if (KrlVarHandler.ReadBoolVariable(KrlVariableNames.ComputeVisionXY2Measures, Robot))
            {
                ComputeVisionDeltaXY2();
            }
        }

        private void ComputeDeltaXY()
        {
            try
            {
                // Read Size Of List
                int lSize = KrlVarHandler.ReadIntVariable(KrlVariableNames.XYListSize, Robot);
                double[] AnglesXY = new double[lSize];
                double[] MeasuresXY = new double[lSize];

                // Read Measure
                for (int i = 0; i < (lSize - 1); i++)
                {
                    string VarName = string.Format(KrlVariableNames.TCPCalibXYListFmt_Angle, i + 1);
                    AnglesXY[i] = KrlVarHandler.ReadRealVariable(VarName, Robot);

                    VarName = string.Format(KrlVariableNames.TCPCalibXYListFmt_Measure, i + 1);
                    MeasuresXY[i] = KrlVarHandler.ReadRealVariable(VarName, Robot);
                }

                double Dx, Dy, Score;
                int lNbOfMeasure = KrlVarHandler.ReadIntVariable(KrlVariableNames.IterationNumber, Robot);
                int ClampDirection;
                if (KrlVarHandler.ReadBoolVariable(KrlVariableNames.ZCalibAroundY, Robot))
                {
                    ClampDirection = XPlus;
                }
                else
                {
                    ClampDirection = YPlus;
                }

                FlexDrillService.Log.WriteMessage(TraceEventType.Information, "delta_TCP_Tool_XY Measure= " + (lNbOfMeasure + 1).ToString() + " CLampDirection = " + ClampDirection.ToString(), String.Empty);
                bool Res = delta_TCP_Tool_XY(AnglesXY, MeasuresXY, (lNbOfMeasure + 1), ClampDirection, out Dx, out Dy, out Score);
                FlexDrillService.Log.WriteMessage(TraceEventType.Information, "delta_TCP_Tool_XY Results : Dx = " + Dx.ToString() + " Dy = " + Dy.ToString() + " Score = " + Score.ToString(), String.Empty);

                if (Res)
                {
                    if (Score < 0.0)
                    {
                        Score = 0.0;
                    }

                    if (Score > 100.0)
                    {
                        Score = 100.0;
                    }

                    KrlVarHandler.WriteVariable(KrlVariableNames.TCPCalibXYDx, Dx, Robot);
                    KrlVarHandler.WriteVariable(KrlVariableNames.TCPCalibXYDy, Dy, Robot);
                    KrlVarHandler.WriteVariable(KrlVariableNames.TCPCalibXYScore, Score, Robot);
                }

                KrlVarHandler.WriteVariable(KrlVariableNames.MeasuresXYComputedOK, Res, Robot);
                KrlVarHandler.WriteVariable(KrlVariableNames.MeasuresXYComputed, true, Robot);
            }
            catch (Exception e)
            {
                FlexDrillService.Log.WriteMessage(TraceEventType.Error, "Exception on ComputeDeltaXY. " + e.Message + ".", e.StackTrace);
            }
        }

        private void ComputeDeltaZ()
        {
            try
            {
                // Read Size Of List
                int lSize = KrlVarHandler.ReadIntVariable(KrlVariableNames.ZListSize, Robot);
                double[] AnglesZ = new double[lSize];
                double[] MeasuresZ = new double[lSize];

                // Read Measure
                for (int i = 0; i < (lSize - 1); i++)
                {
                    string VarName = string.Format(KrlVariableNames.TCPCalibZListFmt_Angle, i + 1);
                    AnglesZ[i] = KrlVarHandler.ReadRealVariable(VarName, Robot);

                    VarName = string.Format(KrlVariableNames.TCPCalibZListFmt_Measure, i + 1);
                    MeasuresZ[i] = KrlVarHandler.ReadRealVariable(VarName, Robot);
                }

                double DzComputed, Score, Dz;
                int lNbOfMeasure = KrlVarHandler.ReadIntVariable(KrlVariableNames.IterationNumber, Robot);

                FlexDrillService.Log.WriteMessage(TraceEventType.Information,
                   "delta_TCP_Tool_Z Measure= " + (lNbOfMeasure + 1).ToString(), String.Empty);
                bool Res = delta_TCP_Tool_Z(AnglesZ, MeasuresZ, (lNbOfMeasure + 1), out DzComputed, out Score);
                FlexDrillService.Log.WriteMessage(TraceEventType.Information, "delta_TCP_Tool_XY Results : DzComputed = " + DzComputed.ToString() + " Score = " + Score.ToString(), String.Empty);

                double lStartDistance = KrlVarHandler.ReadRealVariable(KrlVariableNames.TCPCalibStartDistance, Robot);
                if (Res)
                {
                    Dz = lStartDistance - DzComputed;
                    if (Score < 0.0)
                    {
                        Score = 0.0;
                    }

                    if (Score > 100.0)
                    {
                        Score = 100.0;
                    }

                    KrlVarHandler.WriteVariable(KrlVariableNames.TCPCalibZDz, Dz, Robot);
                    KrlVarHandler.WriteVariable(KrlVariableNames.TCPCalibZScore, Score, Robot);
                }
                KrlVarHandler.WriteVariable(KrlVariableNames.MeasuresZComputedOK, Res, Robot);
                KrlVarHandler.WriteVariable(KrlVariableNames.MeasuresZComputed, true, Robot);
            }
            catch (Exception e)
            {
                FlexDrillService.Log.WriteMessage(TraceEventType.Error, "Exception on ComputeDeltaz. " + e.Message + ".", e.StackTrace);
            }
        }

        private void ComputeRxRy1()
        {
            ComputeDeltaRxRyx(true,
               KrlVariableNames.TCPCalibRxRy1Dx,
               KrlVariableNames.TCPCalibRxRy1Dy,
               KrlVariableNames.TCPCalibRxRy1Score,
               KrlVariableNames.MeasuresRxRy1ComputedOK,
               KrlVariableNames.MeasuresRxRy1Computed);
        }

        private void ComputeRxRy2()
        {
            ComputeDeltaRxRyx(false,
               KrlVariableNames.TCPCalibRxRy2Dx,
               KrlVariableNames.TCPCalibRxRy2Dy,
               KrlVariableNames.TCPCalibRxRy2Score,
               KrlVariableNames.MeasuresRxRy2ComputedOK,
               KrlVariableNames.MeasuresRxRy2Computed);
        }

        private void ComputeDeltaRxRyx(bool IsFirstHeight,
                                       string VarNameDx,
                                       string VarNameDy,
                                       string VarNameScore,
                                       string VarNameComputedOK,
                                       string VarNameComputed)
        {
            try
            {
                // Read Size Of List
                int lSize = KrlVarHandler.ReadIntVariable(KrlVariableNames.RxRyListSize, Robot);
                double[] AnglesRxRy = new double[lSize];
                double[] MeasuresRxRy = new double[lSize];

                string VarName;
                // Read Measure
                for (int i = 0; i < (lSize - 1); i++)
                {
                    if (IsFirstHeight)
                    {
                        VarName = string.Format(KrlVariableNames.TCPCalibRxRy1ListFmt_Angle, i + 1);
                    }
                    else
                    {
                        VarName = string.Format(KrlVariableNames.TCPCalibRxRy2ListFmt_Angle, i + 1);
                    }

                    AnglesRxRy[i] = KrlVarHandler.ReadRealVariable(VarName, Robot);

                    if (IsFirstHeight)
                    {
                        VarName = string.Format(KrlVariableNames.TCPCalibRxRy1ListFmt_Measure, i + 1);
                    }
                    else
                    {
                        VarName = string.Format(KrlVariableNames.TCPCalibRxRy2ListFmt_Measure, i + 1);
                    }

                    MeasuresRxRy[i] = KrlVarHandler.ReadRealVariable(VarName, Robot);
                }

                double Dx, Dy, Score;
                int lNbOfMeasure = KrlVarHandler.ReadIntVariable(KrlVariableNames.IterationNumber, Robot);
                int ClampDirection;
                if (KrlVarHandler.ReadBoolVariable(KrlVariableNames.ZCalibAroundY, Robot))
                {
                    ClampDirection = XPlus;
                }
                else
                {
                    ClampDirection = YPlus;
                }

                FlexDrillService.Log.WriteMessage(TraceEventType.Information, "delta_TCP_Tool_XY(Measure= " + (lNbOfMeasure + 1).ToString() + " CLampDirection = " + ClampDirection.ToString(), String.Empty);
                bool Res = delta_TCP_Tool_XY(AnglesRxRy, MeasuresRxRy, (lNbOfMeasure + 1), ClampDirection, out Dx, out Dy, out Score);
                FlexDrillService.Log.WriteMessage(TraceEventType.Information, "delta_TCP_Tool_XY Results : Dx = " + Dx.ToString() + " Dy = " + Dy.ToString() + " Score = " + Score.ToString(), String.Empty);

                if (Res)
                {
                    if (Score < 0.0)
                    {
                        Score = 0.0;
                    }

                    if (Score > 100.0)
                    {
                        Score = 100.0;
                    }

                    KrlVarHandler.WriteVariable(VarNameDx, Dx, Robot);
                    KrlVarHandler.WriteVariable(VarNameDy, Dy, Robot);
                    KrlVarHandler.WriteVariable(VarNameScore, Score, Robot);
                }

                KrlVarHandler.WriteVariable(VarNameComputedOK, Res, Robot);
                KrlVarHandler.WriteVariable(VarNameComputed, true, Robot);
            }
            catch (Exception e)
            {
                FlexDrillService.Log.WriteMessage(TraceEventType.Error, "Exception on ComputeDeltaRxRyx. " + e.Message + ".", e.StackTrace);
            }
        }

        private void ComputeDeltaRxRy()
        {
            try
            {
                double lXb = KrlVarHandler.ReadRealVariable(KrlVariableNames.TCPCalibRxRy2Dx, Robot);
                double lYb = KrlVarHandler.ReadRealVariable(KrlVariableNames.TCPCalibRxRy2Dy, Robot);
                double lXh = KrlVarHandler.ReadRealVariable(KrlVariableNames.TCPCalibRxRy1Dx, Robot);
                double lYh = KrlVarHandler.ReadRealVariable(KrlVariableNames.TCPCalibRxRy1Dy, Robot);
                double lDistance = KrlVarHandler.ReadRealVariable(KrlVariableNames.RxRyZDistance, Robot);

                double DB, DC, Score;
                FlexDrillService.Log.WriteMessage(TraceEventType.Information, "delta_TCP_Tool_BC(lXb =" + lXb.ToString() + " lYb =" + lYb.ToString() + " lXh = " + lXh.ToString() + " lYh =" + lYh.ToString() + " Distance = " + lDistance.ToString(), String.Empty);
                bool Res = delta_TCP_Tool_BC(lXb, lYb, lXh, lYh, lDistance, out DB, out DC, out Score);
                FlexDrillService.Log.WriteMessage(TraceEventType.Information, "delta_TCP_Tool_XY Results : DB = " + DB.ToString() + " DC = " + DC.ToString() + " Score = " + Score.ToString(), String.Empty);

                if (Res)
                {
                    if (Score < 0.0)
                    {
                        Score = 0.0;
                    }

                    if (Score > 100.0)
                    {
                        Score = 100.0;
                    }

                    KrlVarHandler.WriteVariable(KrlVariableNames.TCPCalibRxRyDRx, DC, Robot);
                    KrlVarHandler.WriteVariable(KrlVariableNames.TCPCalibRxRyDRy, DB, Robot);
                    KrlVarHandler.WriteVariable(KrlVariableNames.TCPCalibRxRyScore, Score, Robot);
                }
                KrlVarHandler.WriteVariable(KrlVariableNames.MeasuresRxRyComputedOK, Res, Robot);
                KrlVarHandler.WriteVariable(KrlVariableNames.MeasuresRxRyComputed, true, Robot);
            }
            catch (Exception e)
            {
                FlexDrillService.Log.WriteMessage(TraceEventType.Error, "Exception on ComputeDeltaRxRy. " + e.Message + ".", e.StackTrace);
            }
        }

        private void ComputeVisionDeltaXY1()
        {
            ComputeVisionDeltaXYx(true,
               KrlVariableNames.TCPCalibVisionXY1Dx,
               KrlVariableNames.TCPCalibVisionXY1Dy,
               KrlVariableNames.TCPCalibVisionXY1Score,
               KrlVariableNames.MeasuresVisXY1ComputedOK,
               KrlVariableNames.MeasuresVisXY1Computed);
        }

        private void ComputeVisionDeltaXY2()
        {
            ComputeVisionDeltaXYx(false,
               KrlVariableNames.TCPCalibVisionXY2Dx,
               KrlVariableNames.TCPCalibVisionXY2Dy,
               KrlVariableNames.TCPCalibVisionXY2Score,
               KrlVariableNames.MeasuresVisXY2ComputedOK,
               KrlVariableNames.MeasuresVisXY2Computed);
        }

        private void ComputeVisionDeltaXYx(bool IsFirstHeight,
                                           string VarNameDx,
                                           string VarNameDy,
                                           string VarNameScore,
                                           string VarNameComputedOK,
                                           string VarNameComputed)
        {
            try
            {
                // Read Size Of List
                int lSize = KrlVarHandler.ReadIntVariable(KrlVariableNames.VisionXYListSize, Robot);
                double[] MeasuresX = new double[lSize];
                double[] MeasuresY = new double[lSize];

                // Read Measure
                for (int i = 0; i < (lSize - 1); i++)
                {
                    string VarName;
                    if (IsFirstHeight)
                    {
                        VarName = string.Format(KrlVariableNames.TCPCalibVisionXY1List_MeasureX, i + 1);
                    }
                    else
                    {
                        VarName = string.Format(KrlVariableNames.TCPCalibVisionXY2List_MeasureX, i + 1);
                    }

                    MeasuresX[i] = KrlVarHandler.ReadRealVariable(VarName, Robot);

                    if (IsFirstHeight)
                    {
                        VarName = string.Format(KrlVariableNames.TCPCalibVisionXY1List_MeasureY, i + 1);
                    }
                    else
                    {
                        VarName = string.Format(KrlVariableNames.TCPCalibVisionXY2List_MeasureY, i + 1);
                    }

                    MeasuresY[i] = KrlVarHandler.ReadRealVariable(VarName, Robot);
                }

                double Dx, Dy, Score;
                int lNbOfMeasure = KrlVarHandler.ReadIntVariable(KrlVariableNames.IterationNumber, Robot);

                FlexDrillService.Log.WriteMessage(TraceEventType.Information, "delta_TCP_Vision_XY(Measure= " + (lNbOfMeasure + 1).ToString(), String.Empty);
                bool Res = delta_TCP_Vision_XY(MeasuresX, MeasuresY, (lNbOfMeasure + 1), 0.0, out Dx, out Dy, out Score);
                FlexDrillService.Log.WriteMessage(TraceEventType.Information, "delta_TCP_Vision_XY Results : Dx = " + Dx.ToString() + " Dy = " + Dy.ToString() + " Score = " + Score.ToString(), String.Empty);

                if (Res)
                {
                    if (Score < 0.0)
                    {
                        Score = 0.0;
                    }

                    if (Score > 100.0)
                    {
                        Score = 100.0;
                    }

                    KrlVarHandler.WriteVariable(VarNameDx, Dx, Robot);
                    KrlVarHandler.WriteVariable(VarNameDy, Dy, Robot);
                    KrlVarHandler.WriteVariable(VarNameScore, Score, Robot);
                }

                KrlVarHandler.WriteVariable(VarNameComputedOK, Res, Robot);
                KrlVarHandler.WriteVariable(VarNameComputed, true, Robot);
            }
            catch (Exception e)
            {
                FlexDrillService.Log.WriteMessage(TraceEventType.Error, "Exception on ComputeVisionx. " + e.Message + ".", e.StackTrace);
            }
        }

        #endregion Methods
    }
}