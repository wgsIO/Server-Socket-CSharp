using System;
using System.Collections.Generic;
using System.Text;

namespace TestServer.Manager
{

    public class InstanceLazy {
        public static Dictionary<string, InstanceLazy> instances = new Dictionary<string, InstanceLazy>();

        public static InstanceManager<A> getOrCreate<A>(string _name)
        {
            InstanceLazy _result;
            if (!instances.ContainsKey(_name)) _result = new InstanceManager<A>();
            else _result = instances[_name];
            return (InstanceManager<A>)_result;
        }
    }

    public class InstanceManager<T> : InstanceLazy
    {

        public string name { get; private set; }

        private T value;

        public T getOrComputer(Func<T> supplier)
        {
            T _result = value;
            return (_result == null) ? maybeCompute(supplier) : _result;
        }

        public T maybeCompute(Func<T> supplier)
        {
            if (value == null) value = supplier();
            return value;
        }

    }
}
