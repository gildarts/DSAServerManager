using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DSAServerManager
{
    /// <summary>
    /// 
    /// </summary>
    public enum ValueStatus { 
        /// <summary>
        /// 
        /// </summary>
        Clean,
        /// <summary>
        /// 
        /// </summary>
        Dirty }

    /// <summary>
    /// 
    /// </summary>
    public class ChangeEventArgs : EventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="status"></param>
        public ChangeEventArgs(ValueStatus status)
        {
            Status = status;
        }

        /// <summary>
        /// 
        /// </summary>
        public ValueStatus Status { get; private set; }
    }
}
