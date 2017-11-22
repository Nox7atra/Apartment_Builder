
namespace Nox7atra.ApartmentEditor
{
    public class ApartmentConfig
    {
        public static ApartmentDrawConfig Current;
        private static ApartmentDrawConfig? _Backup;
        public static void MakeBackup()
        {
            _Backup = Current;
        }
        public static void ApplyBackup()
        {
            if (_Backup.HasValue)
            {
                Current = _Backup.Value;
                _Backup = null;
            }
        }
    }
    public struct ApartmentDrawConfig
    {
        public int UnitsInMeters;
        public bool IsDrawSizes;
        public bool IsDrawPositions;
        public bool IsDrawSquare;

    }
}