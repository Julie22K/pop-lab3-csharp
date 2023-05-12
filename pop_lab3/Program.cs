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
            program.Starter(3, 10,6,3);

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
                threadConsumer.Name = "Consumer#" + (i+1);
                threadConsumer.Start(itemNumbers);
            }
            for (int i = 0; i < producers_num; i++)
            {
                Thread threadProducer = new Thread(Producer);
                threadProducer.Name = "Producer#" + (i+1);
                threadProducer.Start(itemNumbers);
            }
        }
        private Semaphore Access;

        private Semaphore Full;
        private Semaphore Empty;


        private int num_of_last_item=0;
        private List<string> storage= new List<string>();
        private void Producer(Object itemNumbers)
        {
            int maxItem = 0;
            if (itemNumbers is int)
            {
                maxItem = (int)itemNumbers;
            }

            for (int i = 0; i < maxItem; i++)
            {
                Full.WaitOne();
                Access.WaitOne();
                if (num_of_last_item < maxItem)
                {
                    storage.Add("item " + num_of_last_item);
                    Console.WriteLine(Thread.CurrentThread.Name + " added item " + num_of_last_item);
                    num_of_last_item++;

                    Empty.Release();
                    Access.Release();
                }
                else
                {
                    Access.Release();
                    break;

                }

            }
        }

        private void Consumer(Object itemNumbers)
        {
            int maxItem = 0;
            if (itemNumbers is int)
            {
                maxItem = (int)itemNumbers;
            }
            for (int i = 0; i < maxItem; i++)
            {
                if (num_of_last_item == maxItem) break;
                Empty.WaitOne();
                Thread.Sleep(1000);
                Access.WaitOne();

                string item = storage.ElementAt(0);
                storage.RemoveAt(0);
                Console.WriteLine(Thread.CurrentThread.Name + " took " + item);

                Full.Release();
                Access.Release();

            }
        }
    }
}
