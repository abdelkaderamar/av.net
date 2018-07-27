using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Av.API.Data;
using Newtonsoft.Json.Linq;

namespace Av.API
{
    public enum AvDataType { StockHisto, CryptoHisto};
    public class AvDataFactory
    {

        private AvDataFactory() { }

        static AvData CreateData(AvDataType dataType, JObject json)
        {
            switch (dataType)
            {
                case AvDataType.StockHisto:
                    throw new NotImplementedException();
                case AvDataType.CryptoHisto:
                    throw new NotImplementedException();
                default:
                    // log error message
                    return null;
            }
        }

    }
}
