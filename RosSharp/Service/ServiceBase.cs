#region License Terms

// ================================================================================
// RosSharp
// 
// Software License Agreement (BSD License)
// 
// Copyright (C) 2012 zoetrope
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
//     * Redistributions of source code must retain the above copyright
//       notice, this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright
//       notice, this list of conditions and the following disclaimer in the
//       documentation and/or other materials provided with the distribution.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
// (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
// LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
// ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
// (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// ================================================================================

#endregion

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

        #region ITypedService<TRequest,TResponse> Members

        public event Func<string, Task> Disposing = _ => Task.Factory.StartNew(() => { });

        public abstract string ServiceType { get; }
        public abstract string Md5Sum { get; }
        public abstract string ServiceDefinition { get; }

        public TResponse Invoke(TRequest req)
        {
            return _action(req);
        }

        public Task<TResponse> InvokeAsync(TRequest req)
        {
            return Task<TResponse>.Factory.FromAsync(_action.BeginInvoke, _action.EndInvoke, req, null);
        }

        void IService.SetAction(Func<IMessage, IMessage> action)
        {
            _action = req => (TResponse) action(req);
        }

        public Task DisposeAsync()
        {
            return Disposing(null);
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
            return _action((TRequest) req);
        }

        public void Dispose()
        {
            DisposeAsync().Wait();
        }

        #endregion
    }
}