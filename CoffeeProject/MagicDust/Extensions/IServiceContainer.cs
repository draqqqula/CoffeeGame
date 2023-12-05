using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicDustLibrary.Extensions
{
    public interface IServiceContainer
    {
        public void AddService<T1, T2>();
        public void AddKeyedService<T1, T2>(string key);
        public T GetService<T>();
        public IEnumerable<T> GetServices<T>();
        public T GetKeyedService<T>(string key);
    }
}
