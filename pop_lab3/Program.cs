using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace pop_lab3
{
    class Program
    {
        static void Main(string[] args)
        {
            Program program = new Program();
            program.Starter(3, 10,4,3);

            Console.ReadKey();
        }


        private void Starter(int storageSize, int itemNumbers,int producers_num, int consumer_num)
        {
            Access = new Semaphore(1, 1);
            Full = new Semaphore(storageSize, storageSize);
            Empty = new Semaphore(0, storageSize);

            
            for (int i = 0; i < consumer_num; i++)
            { 
                Thread threadConsumer = new Thread(Consumer);
                threadConsumer.Name = "Consumer#" + i;
                threadConsumer.Start(itemNumbers);
            }
            for (int i = 0; i < producers_num; i++)
            {
                Thread threadProducer = new Thread(Producer);
                threadProducer.Name = "Producer#" + i;
                threadProducer.Start(itemNumbers);
            }
        }

        private Semaphore Access;

        private Semaphore Full;
        private Semaphore Empty;

        private readonly List<string> storage = new List<string>();
        private int counter=0;
        private void Producer(Object itemNumbers)
        {
            int maxItem = 0;
            if (itemNumbers is int)
            {
                maxItem = (int)itemNumbers;
            }
            
            while (counter < maxItem)
            {
                Full.WaitOne();
                Access.WaitOne();
                if (counter == maxItem) {
                    Full.Release();
                    Access.Release();
                    break;
                }
                storage.Add("item " + counter);
                Console.WriteLine(Thread.CurrentThread.Name+" added item " + counter);
                counter++;

                Access.Release();
                Empty.Release();

            }
        }

        private void Consumer(Object itemNumbers)
        {
            int maxItem = 0;
            if (itemNumbers is int)
            {
                maxItem = (int)itemNumbers;
            }
            while (storage.Count>0)
            {
                Empty.WaitOne();
                Thread.Sleep(1000);
                Access.WaitOne();

                string item = storage.ElementAt(0);
                storage.RemoveAt(0);

                Full.Release();

                Access.Release();

                Console.WriteLine(Thread.CurrentThread.Name + " took " + item);
            }
        }
    }
}
s