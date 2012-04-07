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
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using CookComputing.XmlRpc;

namespace RosSharp.Parameter
{
    internal interface IParameterCoverter<T>
    {
        T ConvertTo(object value);
        object ConvertFrom(T value);
    }

    internal sealed class PrimitiveParameterConverter<T> : IParameterCoverter<T>
    {
        #region IParameterCoverter<T> Members

        public T ConvertTo(object value)
        {
            return (T) value;
        }

        public object ConvertFrom(T value)
        {
            return value;
        }

        #endregion
    }

    internal sealed class ListParameterConverter<T> : IParameterCoverter<T>
    {
        #region IParameterCoverter<T> Members

        public T ConvertTo(object value)
        {
            throw new NotImplementedException();
        }

        public object ConvertFrom(T value)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    internal sealed class DictionaryParameterConverter<T> : IParameterCoverter<T>
    {
        #region IParameterCoverter<T> Members

        public T ConvertTo(object value)
        {
            throw new NotImplementedException();
        }

        public object ConvertFrom(T value)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public sealed class DictionaryParameter : DynamicObject
    {
        private XmlRpcStruct _xmlRpcStruct;
        
        public DictionaryParameter(XmlRpcStruct members)
        {
            _xmlRpcStruct = members;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = null;
            if(!_xmlRpcStruct.ContainsKey(binder.Name))
            {
                return false;
            }

            result = _xmlRpcStruct[binder.Name];
            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            _xmlRpcStruct[binder.Name] = value;
            return true;
        }
    }
}