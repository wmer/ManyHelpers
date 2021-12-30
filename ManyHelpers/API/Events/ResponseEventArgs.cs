using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManyHelpers.API.Events {
    public class ResponseEventArgs : ApiBaseEventArgs {
        public string StatusCode { get; set; }
        public string Response { get; set; }

        public ResponseEventArgs(string endpoint, string verb, string statusCode, string response) {
            EndPoint = endpoint;
            Verb = verb;
            StatusCode = statusCode;
            Response = response;
        }
    }


    public delegate void ResponseEventHandler(object sender, ResponseEventArgs e);
}
