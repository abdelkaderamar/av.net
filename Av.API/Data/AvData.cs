using Newtonsoft.Json.Linq;

namespace Av.API.Data
{
    public abstract class AvData
    {
        public AvData()
        {
        }

        public abstract void Init(JObject json);
    }
}
