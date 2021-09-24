using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameBase.Common.PlatformInterface
{
    public interface IQuickStorage
    {
        public void SetItem<T>(string key, T value);
        public T GetItem<T>(string key);
        public void Clear();
    }
}
