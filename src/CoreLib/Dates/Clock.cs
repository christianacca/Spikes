using System;

namespace Eca.Commons.Dates
{
    public static class Clock
    {
        #region Class Members

        static Clock()
        {
            Default = new ClockImpl();
        }


        public static IClock Default { get; private set; }


        public static bool IsFrozen
        {
            get { return Default.IsFrozen; }
        }

        public static DateTime Now
        {
            get { return Default.Now; }
            set { Default.Now = value; }
        }


        public static IClock Create()
        {
            return new ClockImpl();
        }


        public static void ResetToSystemClock()
        {
            Default.ResetToSystemClock();
        }

        #endregion


        private class ClockImpl : IClock
        {
            #region Member Variables

            private DateTime? _now;

            #endregion


            #region IClock Members

            bool IClock.IsFrozen
            {
                get { return _now != null; }
            }

            DateTime IClock.Now
            {
                get { return _now ?? DateTime.Now; }
                set { _now = value; }
            }


            void IClock.ResetToSystemClock()
            {
                _now = null;
            }

            #endregion
        }
    }



    public interface IClock
    {
        DateTime Now { get; set; }
        bool IsFrozen { get; }
        void ResetToSystemClock();
    }
}