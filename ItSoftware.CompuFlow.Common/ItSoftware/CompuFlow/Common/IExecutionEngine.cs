using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ItSoftware.CompuFlow.Common.Status;
using ItSoftware.CompuFlow.Common;
namespace ItSoftware.CompuFlow.Common
{
    public interface IExecutionEngine
    {
        bool EndFlow { get; set; }
        bool LogToEvents { get; set; }
        void AddStatusInformation(StatusProgressItem pi);
        void Zip(string directory, string[] filenames, string zipFilename);
        IResponseInBrowser ResponseInBrowser { get; }
        string FormatException(Exception x);
    }
}
