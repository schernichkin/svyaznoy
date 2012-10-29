using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Svyaznoy.Threading
{
    internal sealed class CountdownEvent
    {
        public CountdownEvent(int initialCount)
        {
            m_Count = initialCount;
        }

        public void Signal()
        {
            lock (this)
            {
                if (--Count == 0)
                {
                    Monitor.Pulse(this);
                }
            }
        }

        public void Wait()
        {
            lock (this)
            {
                if (Count > 0)
                {
                    Monitor.Wait(this);
                }
            }
        }

        #region private int InitialCount

        private volatile int m_Count;

        private int Count { get { return m_Count; } set { m_Count = value; } }

        #endregion
    }
}
