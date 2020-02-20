using System;
using System.Collections.Generic;
using System.Text;

namespace TestServer.Utils
{
    [System.Serializable]
    class ThreeValue<A, B, C> : ICloneable
    {

        private A one;
        private B two;
        private C three;

        public ThreeValue(A _one, B _two, C _three)
        {
            one = _one;
            two = _two;
            three = _three;
        }

        public A getOne() => one;
        public B getTwo() => two;
        public C getThree() => three;

        public bool oneIsNull() => getOne() == null;
        public bool twoIsNull() => getTwo() == null;
        public bool threeIsNull() => getThree() == null;

        public bool isEmpty() => oneIsNull() && twoIsNull() && threeIsNull();

        public ThreeValue<A, B, C> valueOf(A _one, B _two, C _three) => new ThreeValue<A, B, C>(_one, _two, _three);

        public Object[] toArray() => new object[] { one, two, three };

        public object Clone() => this;

    }
}
