using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace IPTest
{
    class Program
    {
        static string[] target = new string[]
        {
            "0.0.0.0/3", "32.0.0.0/4",
        "48.0.0.0/5", "56.0.0.0/6", "60.0.0.0/7", "62.0.0.0/9", "62.128.0.0/10",
        "62.192.0.0/12", "62.208.0.0/15", "62.210.0.0/20", "62.210.16.0/21",
        "62.210.24.0/23", "62.210.27.0/24", "62.210.28.0/22", "62.210.32.0/19",
        "62.210.64.0/18", "62.210.128.0/17", "62.211.0.0/16", "62.212.0.0/14",
        "62.216.0.0/13", "62.224.0.0/11", "63.0.0.0/8", "64.0.0.0/2", "128.0.0.0/2",
        "192.0.0.0/9", "192.128.0.0/11", "192.160.0.0/13", "192.168.0.0/24",
        "192.168.2.0/23", "192.168.4.0/22", "192.168.8.0/21", "192.168.16.0/20",
        "192.168.32.0/19", "192.168.64.0/18", "192.168.128.0/17", "192.169.0.0/16",
        "192.170.0.0/15", "192.172.0.0/14", "192.176.0.0/12", "192.192.0.0/10",
        "193.0.0.0/8", "194.0.0.0/7",
        "196.0.0.0/6", "200.0.0.0/5", "208.0.0.0/4"
        };
        /*
         *2020-07-23 14:58:26 Routes: 0.0.0.0/0, 10.4.10.1/32, 10.4.10.4/30
2020-07-23 14:58:26 Routes excluded: 62.210.26.0/24, 192.168.1.173/24 
        fe80:0:0:0:f660:e2ff:fef0:23ee/64
2020-07-23 14:58:26 VpnService routes installed: 0.0.0.0/3, 32.0.0.0/4,
        48.0.0.0/5, 56.0.0.0/6, 60.0.0.0/7, 62.0.0.0/9, 62.128.0.0/10,
        62.192.0.0/12, 62.208.0.0/15, 62.210.0.0/20, 62.210.16.0/21,
        62.210.24.0/23, 62.210.27.0/24, 62.210.28.0/22, 62.210.32.0/19, 
        62.210.64.0/18, 62.210.128.0/17, 62.211.0.0/16, 62.212.0.0/14,
        62.216.0.0/13, 62.224.0.0/11, 63.0.0.0/8, 64.0.0.0/2, 128.0.0.0/2,
        192.0.0.0/9, 192.128.0.0/11, 192.160.0.0/13, 192.168.0.0/24, 
        192.168.2.0/23, 192.168.4.0/22, 192.168.8.0/21, 192.168.16.0/20, 
        192.168.32.0/19, 192.168.64.0/18, 192.168.128.0/17, 192.169.0.0/16, 
        192.170.0.0/15, 192.172.0.0/14, 192.176.0.0/12, 192.192.0.0/10,
        193.0.0.0/8, 194.0.0.0/7, 
        196.0.0.0/6, 200.0.0.0/5, 208.0.0.0/4, 224.0.0.0/3*/
        static void Main(string[] args)
        {

            var excluded = new List<IPNetwork>();
            var exclude1 = IPNetwork.Parse("62.210.26.0/24");
            var exclude2 = IPNetwork.Parse("192.168.1.173/24");
            excluded.Add(exclude1);
            excluded.Add(exclude2);
            excluded.Add(IPNetwork.Parse("224.0.0.0/3"));

            var actualNet = new List<IPNetwork>() { IPNetwork.Parse("0.0.0.0/0") };

            var newRoutes = new List<IPNetwork>();
            var removed = new List<IPNetwork>();

            foreach (var route in excluded)
            {
                newRoutes.Clear();
                removed.Clear();
                foreach (var main in actualNet)
                {
                    if (main.Overlap(route) == false) continue;
                    removed.Add(main);
                    RecursiveAdd(main, route, newRoutes);
                    //var current = main;
                    //while (current.Cidr < 32)
                    //{
                    //    if (route.Contains(current))
                    //        break;
                    //    var splitted = Split(current);
                    //    var firstOverLap = route.Overlap(splitted[0]);
                    //    var secondOverlap = route.Overlap(splitted[1]);

                    //    if (firstOverLap && secondOverlap)
                    //    {
                    //        break;
                    //    }
                    //    else if (firstOverLap)
                    //    {
                    //        current = splitted[0];
                    //        newRoutes.Add(splitted[1]);
                    //    }
                    //    else if (secondOverlap)
                    //    {
                    //        current = splitted[1];
                    //        newRoutes.Add(splitted[0]);
                    //    }
                    //}

                }
                var allNew = string.Join(",", newRoutes.Select(x => x.Network));
                var allRemoved = string.Join(",", removed.Select(x => x.Network));
                Console.WriteLine($"New {allNew}");
                Console.WriteLine($"Removed {allRemoved}");
                removed.ForEach(x => actualNet.Remove(x));
                actualNet.AddRange(newRoutes);
            }
            var all = IPNetwork.Supernet(actualNet.ToArray());
            var stringLinst = new List<string>();
            foreach(var dt in all)
            {
                var m = $"{dt.Network}/{dt.Cidr}";
                stringLinst.Add(m);
                Console.WriteLine(m);
            }

            foreach(var item in stringLinst)
            {
                if (!target.Contains(item))
                {
                    Console.WriteLine("Error List: " + item);
                }
            }

            foreach (var item in target)
            {
                if (!stringLinst.Contains(item))
                {
                    Console.WriteLine("Error Target: " + item);
                }
            }

            Console.ReadLine();
        }

        static List<IPNetwork> RecursiveAdd(IPNetwork target, IPNetwork subtract, List<IPNetwork> newNetwork)
        {

            if (subtract.Contains(target) || target.Cidr >= 31)
                return newNetwork;
            var splitted = Split(target);
            var firstOverLap = subtract.Overlap(splitted[0]);
            var secondOverlap = subtract.Overlap(splitted[1]);

            if (firstOverLap && secondOverlap)
            {
                RecursiveAdd(splitted[0], subtract, newNetwork);
                RecursiveAdd(splitted[1], subtract, newNetwork);
            }
            else if (firstOverLap)
            {
                newNetwork.Add(splitted[1]);
                RecursiveAdd(splitted[0], subtract, newNetwork);
            }
            else if (secondOverlap)
            {
                newNetwork.Add(splitted[0]);
                RecursiveAdd(splitted[1], subtract, newNetwork);
            }
            
            return newNetwork;
        }

        static IPNetwork[] Split(IPNetwork iPNetwork)
        {
           return iPNetwork.Subnet((byte)(iPNetwork.Cidr+1)).ToArray();
        }
    }
}
