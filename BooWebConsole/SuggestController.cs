#region license
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
using Boo.Lang.Interpreter;
using MiniMVC;

namespace BooWebConsole {
    public class SuggestController: Controller {
        public override IResult Execute(HttpContextBase context) {
            var q = context.Request["q"];
            string response = GetSuggestions(q);
            return new RawResult(response) {
                ContentType = "application/json"
            };
        }

        public string GetSuggestions(string q) {
            var interpreter = new InteractiveInterpreter();
            foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
                interpreter.References.Add(a);
            var suggestions = new string[0];
            if (!string.IsNullOrEmpty(q)) {
                suggestions = interpreter.SuggestCodeCompletion(q)
                    .Select(c => c.Name)
                    .Distinct()
                    .OrderBy(x => x)
                    .Select(c => string.Format("'{0}'", c))
                    .ToArray();
            }
            return string.Format("[{0}]", string.Join(",", suggestions));
        }
    }
}