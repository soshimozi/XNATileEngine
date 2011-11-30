using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EventBroker
{
    public delegate void GenericEventHandler(object sender, GlobalEventEventArgs e);

    public class GlobalEventEventArgs : EventArgs
    {
        public GlobalEventEventArgs()
        {
        }

        public GlobalEventEventArgs(string message)
        {
            Message = message;
        }

        public string Message
        {
            get;
            set;
        }

    }
}
