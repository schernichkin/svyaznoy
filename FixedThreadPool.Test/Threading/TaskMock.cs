using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Svyaznoy.Threading;

namespace Svyaznoy.Threading
{
    internal sealed class TaskMock: ITask
    {
        public TaskMock()
            : this(() => { })
        {
        }

        public TaskMock(Action action)
        {
            if (action == null) throw new ArgumentNullException("action");

            m_Action = action;
        }

        #region ITask Members

        public void Execute()
        {
            Action();
        }

        #endregion

        #region private Action Action

        private readonly Action m_Action;

        private Action Action { get { return m_Action; } }

        #endregion
    }
}
