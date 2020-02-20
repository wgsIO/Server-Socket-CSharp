using System;
using System.Collections.Generic;
using System.Text;

namespace TestServer.Utils
{
    [System.Serializable]
    public class MultiValue<A, B> : ICloneable
    {

        private A one;
        private B two;

        public MultiValue(A _one, B _two)
        {
            one = _one;
            two = _two;
        }

        public A getOne() => one;
        public B getTwo() => two;

        public bool oneIsNull() => getOne() == null;
        public bool twoIsNull() => getTwo() == null;

        public bool isEmpty() => oneIsNull() && twoIsNull();

        public MultiValue<A, B> valueOf(A _one, B _two) => new MultiValue<A, B>(_one, _two);

        public Object[] toArray() => new object[] { one, two };

        public object Clone() => this;

    }
}
