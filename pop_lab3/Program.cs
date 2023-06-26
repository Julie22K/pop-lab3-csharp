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
            program.Starter(4, 20, 6, 4);

            Console.ReadKey();
        }
        private void Starter(int storageSize, int itemNumbers, int producers_num, int consumer_num)
        {
            Access = new Semaphore(1, 1);
            Full = new Semaphore(storageSize, storageSize);
            Empty = new Semaphore(0, storageSize);


            for (int i = 0; i < consumer_num; i++)
            {
                Thread threadConsumer = new Thread(Consumer);
                threadConsumer.Name = "Consumer#" + (i + 1);
                int itemNumbersForOneConsumer = (i != consumer_num - 1) ? itemNumbers / consumer_num : itemNumbers-(i*(itemNumbers / consumer_num)) ;
               // Console.WriteLine("Consumer#" + itemNumbersForOneConsumer);
                threadConsumer.Start(itemNumbersForOneConsumer);
            }

            for (int i = 0; i < producers_num; i++)
            {
                Thread threadProducer = new Thread(Producer);
                threadProducer.Name = "Producer#" + (i + 1);
                int itemNumbersForOneProducer = (i != producers_num - 1) ? itemNumbers / producers_num : itemNumbers - (i * (itemNumbers / producers_num));
                //Console.WriteLine("Producer#" + itemNumbersForOneProducer);
                threadProducer.Start(itemNumbersForOneProducer);
            }
        }
        private Semaphore Access;
        private Semaphore Full;
        private Semaphore Empty;


        private int num_of_item = 0;
        private List<string> storage = new List<string>();
        private void Producer(Object itemNumbers)
        {
            int maxItem = 1;
            if (itemNumbers is int)
            {
                maxItem = (int)itemNumbers;
            }

            for (int i = 0; i < maxItem; i++)
            {
                Full.WaitOne();
                Access.WaitOne();

                storage.Add("item " + num_of_item);
                Console.WriteLine(Thread.CurrentThread.Name + " added item " + num_of_item);
                num_of_item++;

                Empty.Release();
                Access.Release();

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
