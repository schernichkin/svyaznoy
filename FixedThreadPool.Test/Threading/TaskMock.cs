using System;

namespace Svyaznoy.Threading
{
    internal sealed class TaskMock: ITask
    {
        public TaskMock()
            : this(() => { })
        {
        }

        public TaskMock(string name)
            : this()
        {
            _Name = name;
        }

        public TaskMock(Action action)
        {
            if (action == null) throw new ArgumentNullException("action");

            m_Action = action;
        }

        public override string ToString()
        {
            return base.ToString() + (string.IsNullOrEmpty(Name) ? "" : " ( " + Name + ")");
        }

        #region public string Name

        private readonly string _Name;

        public string Name
        {
            [System.Diagnostics.DebuggerStepThroughAttribute]
            get
            {
                return _Name;
            }
        }

        #endregion

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
