// SessionManager.cs
using System;

namespace Admissions_Reserve.Model
{
    public static class SessionManager
    {
        private static Applicants _currentApplicant;
        private static int? _currentApplicantId;

        public static Applicants CurrentApplicant
        {
            get
            {
                if (_currentApplicant == null && _currentApplicantId.HasValue)
                {
                    _currentApplicant = LoadApplicant(_currentApplicantId.Value);
                }
                return _currentApplicant;
            }
            set
            {
                _currentApplicant = value;
                _currentApplicantId = value?.Id;
            }
        }

        public static int? CurrentApplicantId => _currentApplicantId;

        public static void Clear()
        {
            _currentApplicant = null;
            _currentApplicantId = null;
        }

        public static Applicants LoadApplicant(int id)
        {
            var db = new Core();
            try
            {
                return db.context.Applicants.Find(id);
            }
            finally
            {
                db = null;
            }
        }

        public static void Refresh()
        {
            if (_currentApplicantId.HasValue)
            {
                _currentApplicant = LoadApplicant(_currentApplicantId.Value);
            }
        }
    }
}