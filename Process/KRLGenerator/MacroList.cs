namespace Kuka.FlexDrill.Process.KRLGenerator
{
    public static class MacroList
    {
        public const string cszNOP = "nop";

        public const string cszClearLocalRelocData = "clear_local_reloc_data";
        public const string cszClearGlobalRelocData = "clear_global_reloc_data";
        public const string cszLocateTargetXY = "locate_target_xy";
        public const string cszLocateTargetZRxRy = "locate_target_zrxry";
        public const string cszStoreLocalData = "store_local_data";
        public const string cszStoreGlobalData = "store_global_data";
        public const string cszComputeObjectFrame = "compute_object_frame";
        public const string cszDrill = "drill";
        public const string cszClamp = "clamp";
        public const string cszUnclamp = "unclamp";
        public const string cszChangeHead = "change_setitec_head";
        public const string cszTakePicture = "take_picture";
    }
}