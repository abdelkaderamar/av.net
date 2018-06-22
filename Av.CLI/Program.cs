using Av.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Av.CLI
{
    class Program
    {
        static void Main(string[] args)
        {
            StockAvProvider provider = new StockAvProvider(args[0]);
            Console.WriteLine(provider.requestDaily("SGO.PA"));
            Console.WriteLine(provider.requestWeekly("SGO.PA"));
            Console.WriteLine(provider.requestMonthly("SGO.PA"));
        }
    }
}
