﻿#region license
// Copyright (c) 2009 Mauricio Scheffer
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;
using System.Linq;
using System.Web;

namespace BooWebConsole {
    public class ControllerFactory : IHttpHandlerFactory {
        public IHttpHandler GetHandler(HttpContext context, string requestType, string url, string pathTranslated) {
            var name = context.Request.RawUrl.Split('/').Last().Split('?')[0].Split('.')[0];
            name = name.ToUpperInvariant().First() + new string(name.ToLowerInvariant().Skip(1).ToArray());
            return (IHttpHandler) Activator.CreateInstance(Type.GetType(string.Format("BooWebConsole.{0}Controller", name)));
        }

        public void ReleaseHandler(IHttpHandler handler) {}
    }
}