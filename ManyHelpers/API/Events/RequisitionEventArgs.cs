using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManyHelpers.API.Events {
    public class RequisitionEventArgs : ApiBaseEventArgs {
        public string Requisition { get; set; }

        public RequisitionEventArgs(string endpoint, string verb, string requisition) {
            EndPoint = endpoint;
            Verb = verb;
            Requisition = requisition;
        }
    }


    public delegate void RequisitionEventHandler(object sender, RequisitionEventArgs e);
}
