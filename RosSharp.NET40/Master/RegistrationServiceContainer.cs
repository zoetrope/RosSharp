using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RosSharp.Master
{
    internal class RegistrationServiceContainer
    {
        private Dictionary<string, Uri> _serviceUris = new Dictionary<string, Uri>();

        public void RegisterService(string service, Uri serviceUri, Uri slaveUri)
        {
            _serviceUris.Add(service, serviceUri);
        }

        public void UnregisterService(string service, Uri serviceUri)
        {
            throw new NotImplementedException();
        }

        public Uri LookUp(string service)
        {
            if(_serviceUris.ContainsKey(service))
            {
                return _serviceUris[service];
            }
            else
            {
                return null;
            }
        }
    }
}
