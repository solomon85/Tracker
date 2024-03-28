using GPS_Tracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS_Tracker
{
    public abstract class ParserBase
    {
        public event Action<string> OnDataReceive;
        protected bool _showDiagnosticInfo;
        public void ShowDiagnosticInfo(string message)
        {
            if (_showDiagnosticInfo)
                OnDataReceive.Invoke(message);
        }

        public abstract int DecodeAVL(List<byte> receiveBytes, string IMEI);
    }
}
