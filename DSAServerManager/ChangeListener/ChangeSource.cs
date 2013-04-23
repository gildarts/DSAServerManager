using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DSAServerManager
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class ChangeSource : IChangeSource
    {
        #region IChangeSource 成員

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<ChangeEventArgs> StatusChanged;

        /// <summary>
        /// 
        /// </summary>
        public bool Suspend { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="status"></param>
        protected void RaiseStatusChanged(ValueStatus status)
        {
            if (StatusChanged != null)
                StatusChanged(this, new ChangeEventArgs(status));
        }

        /// <summary>
        /// 
        /// </summary>
        public abstract void Reset();

        #endregion
    }
}
