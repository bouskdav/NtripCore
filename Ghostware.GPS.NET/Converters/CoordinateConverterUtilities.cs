using System.Runtime.InteropServices;

namespace Ghostware.GPS.NET.Converters
{
    internal class CoordinateConverterUtilities
    {
#if WIN64
        private const string DllImport = @"plugins/ETRS89_LAMBERT_UTM_64bits.dll";
#else
        private const string DllImport = @"plugins/ETRS89_LAMBERT_UTM_32bits.dll";
#endif

        #region Coordinate conversion functions using NGI DLL

        //Import the dll with the functions to calculate lambert coordinates
        [DllImport(DllImport, SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GeoETRS89ToLambert72(double Xi, double Yi, double Zi, ref double xo, ref double yo, ref double Ho);

        [DllImport(DllImport, SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int Lambert72ToLambert08(double Xi, double Yi, double Zi, ref double xo, ref double yo, ref double Ho);

        [DllImport(DllImport, SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int Lambert72ToGeoETRS89(double Xi, double Yi, double Zi, ref double xo, ref double yo, ref double Ho);

        #endregion
    }
}
