using System;
using System.Threading.Tasks;
using RosSharp.Message;

namespace RosSharp.Service
{

    public abstract class ServiceBase<TRequest, TResponse> : ITypedService<TRequest, TResponse>
        where TRequest : IMessage, new()
        where TResponse : IMessage, new()
    {
        private Func<TRequest, TResponse> _action;

        protected ServiceBase()
        {
        }

        protected ServiceBase(Func<TRequest, TResponse> action)
        {
            _action = action;
        }

        public abstract string ServiceType { get; }
        public abstract string Md5Sum { get; }
        public abstract string ServiceDefinition { get; }

        public TResponse Invoke(TRequest req)
        {
            return _action(req);
        }

        public Task<TResponse> InvokeTaskAsync(TRequest req)
        {
            return Task<TResponse>.Factory.FromAsync(_action.BeginInvoke, _action.EndInvoke, req, null);
        }

        void IService.SetAction(Func<IMessage, IMessage> action)
        {
            _action = req => (TResponse)action(req);
        }

        IMessage IService.CreateRequest()
        {
            return new TRequest();
        }

        IMessage IService.CreateResponse()
        {
            return new TResponse();
        }

        IMessage IService.Invoke(IMessage req)
        {
            return _action((TRequest)req);
        }
    }
}
