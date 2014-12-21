using System;
using System.Runtime.InteropServices;
using NUnit.Framework;

namespace Eca.Spikes.DotNet
{
    [TestFixture]
    public class PInvokeExamples
    {
        [Test]
        public void CanMakeComputerBeep()
        {
            Sound.Beep();
        }


        public class Sound
        {
            [DllImport("User32.DLL")]
            private static extern bool MessageBeep(uint beepType);


            public static void Beep()
            {
                MessageBeep(0);
            }
        }
    }
}