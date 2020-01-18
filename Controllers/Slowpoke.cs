
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace WebApplication1.Controllers
{    
    public class Slowpoke
    {
        static Random random = new Random();
        public static int Sleep(int p1, int p2, int ms)
        {
            var t2s = random.Next(p1, p2) * ms;
            Thread.Sleep(t2s);
            return t2s;
        }
    }
}