using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NSubstituteSpike
{
    public class SubstituteFoo : IFoo, ISubstitute {
        Substitute ISubstitute.Instance
        {
            get; set;
        }

        IList<MethodBase> Calls = new List<MethodBase>();
        IDictionary<MethodBase, object> StubbedCalls = new Dictionary<MethodBase, object>();

        private void SetLastSubstitute()
        {
            ((ISubstitute)this).Instance.LastSubstitute = this;
        }

        private void SetLastCall(MethodBase m)
        {
            Calls.Add(m);
        }

        void ISubstitute.LastCallShouldReturn(object o)
        {
            StubbedCalls[Calls.Last()] = o;
        }

        public int Calculate()
        {
            
            SetLastSubstitute();
            SetLastCall(MethodBase.GetCurrentMethod());
            var methodBase = MethodBase.GetCurrentMethod();
            return StubbedCalls.ContainsKey(methodBase) ? (int) StubbedCalls[methodBase] : default(int);
        }

        public string Concat(string a, string b)
        {
            SetLastSubstitute();
            SetLastCall(MethodBase.GetCurrentMethod());
            var methodBase = MethodBase.GetCurrentMethod();
            return StubbedCalls.ContainsKey(methodBase) ? (string)StubbedCalls[methodBase] : default(string);
        }

        public Bar CreateBar()
        {
            SetLastSubstitute();
            SetLastCall(MethodBase.GetCurrentMethod());
            var methodBase = MethodBase.GetCurrentMethod();
            return StubbedCalls.ContainsKey(methodBase) ? (Bar)StubbedCalls[methodBase] : default(Bar);
        }

        private Guid _someId;
        public Guid SomeId
        {
            get
            {
                SetLastSubstitute();
                return _someId;
            }
            set
            {
                SetLastSubstitute();
                _someId = value;
            }
        }
    }
}