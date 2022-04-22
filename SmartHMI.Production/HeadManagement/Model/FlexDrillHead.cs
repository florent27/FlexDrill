// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FlexDrillHead.cs" company="KUKA Deutschland GmbH">
//    Copyright (c) KUKA Deutschland GmbH 2006 - 2018
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kuka.FlexDrill.SmartHMI.Production.Base;

namespace Kuka.FlexDrill.SmartHMI.Production.HeadManagement.Model
{
    public class FlexDrillHead : BindablObject
    {
        #region Constants and Fields

        private string name;

        private int slotIndex;

        private string customerInfo;

        private string amplitude;

        private string type;

        private string id;

        private string oscillation;

        private string gearBoxFeed;

        private string gearBoxSpeed;

        #endregion Constants and Fields

        #region Constructors and Destructor

        /// <summary>
        /// The constructor
        /// </summary>
        /// <param name="krlHeadKrlHeadIndex">1-based index!</param>
        public FlexDrillHead(int krlHeadKrlHeadIndex)
        {
            KrlHeadIndex = krlHeadKrlHeadIndex;
        }

        #endregion Constructors and Destructor

        #region Interface

        /// <summary>
        /// Copy the head detailed information
        /// </summary>
        /// <param name="head">The head to copy the information from</param>
        public void CopyDetailInfo(FlexDrillHead head)
        {
            if (head == null)
            {
                return;
            }

            // Actualize all properties, except the slotIndex
            Name = head.Name;
            CustomerInfo = head.CustomerInfo;
            Amplitude = head.Amplitude;
            Type = head.Type;
            Id = head.Id;
            Oscillation = head.Oscillation;
            GearBoxFeed = head.gearBoxFeed;
            GearBoxSpeed = head.GearBoxSpeed;
        }

        public string KrlHead => $"Heads[{KrlHeadIndex}]";

        public string KrlName => $"Heads[{KrlHeadIndex}].Name[]";

        public string KrlSlotIndex => $"Heads[{KrlHeadIndex}].SlotIndex";

        public string KrlType => $"Heads[{KrlHeadIndex}].Type[]";

        public string KrlUid => $"Heads[{KrlHeadIndex}].Uid[]";

        public string KrlOscillation => $"Heads[{KrlHeadIndex}].MicroPeckOsc[]";

        public string KrlAmplitude => $"Heads[{KrlHeadIndex}].MicroPeckAmplitude[]";

        public string KrlGearBoxFeed => $"Heads[{KrlHeadIndex}].GearBoxFeed[]";

        public string KrlGearBoxSpeed => $"Heads[{KrlHeadIndex}].GearBoxSpeed[]";

        public string KrlCustomerInfo => $"Heads[{KrlHeadIndex}].CustomerInfo[]";

        /// <summary>
        /// Gets the 1-based head index (1-based due to the KRL array index)
        /// </summary>
        public int KrlHeadIndex { get; }

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                SetField(ref name, value);
            }
        }

        /// <summary>
        /// Gets or sets the 1-based slot index
        /// </summary>
        public int SlotIndex
        {
            get
            {
                return slotIndex;
            }
            set
            {
                SetField(ref slotIndex, value);
            }
        }

        public string Type
        {
            get
            {
                return type;
            }
            set
            {
                SetField(ref type, value);
            }
        }

        public string Id
        {
            get
            {
                return id;
            }
            set
            {
                SetField(ref id, value);
            }
        }

        public string Oscillation
        {
            get
            {
                return oscillation;
            }
            set
            {
                SetField(ref oscillation, value);
            }
        }

        public string Amplitude
        {
            get
            {
                return amplitude;
            }
            set
            {
                SetField(ref amplitude, value);
            }
        }

        public string GearBoxFeed
        {
            get
            {
                return gearBoxFeed;
            }
            set
            {
                SetField(ref gearBoxFeed, value);
            }
        }

        public string GearBoxSpeed
        {
            get
            {
                return gearBoxSpeed;
            }
            set
            {
                SetField(ref gearBoxSpeed, value);
            }
        }

        public string CustomerInfo
        {
            get
            {
                return customerInfo;
            }
            set
            {
                SetField(ref customerInfo, value);
            }
        }

        #endregion Interface
    }
}