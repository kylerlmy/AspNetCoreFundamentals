using System;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using System.Linq;
namespace Synchronization
{
    class Program
    {
        static void Main(string[] args)
        {
            // SpinLockSample1();
            // MutexSample();
            SemaphoreExample();

            //Main2();

            Console.ReadLine();

        }

        static void LockExample()
        {
            var account = new Account(1000);
            var tasks = new Task[100];
            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = Task.Run(() => RandomlyUpdate(account));
            }
            Task.WaitAll(tasks);
        }
        static void RandomlyUpdate(Account account)
        {
            var rnd = new Random();
            for (int i = 0; i < 10; i++)
            {
                var amount = rnd.Next(1, 100);
                bool doCredit = rnd.NextDouble() < 0.5;
                if (doCredit)
                {
                    account.Credit(amount);
                }
                else
                {
                    account.Debit(amount);
                }
            }
        }

        static void SpinLockSample1()
        {
            var sl = new SpinLock();

            var stringbuilder = new StringBuilder();

            Action action = () =>
              {
                  bool gotLock = false;
                  for (int i = 0; i < 10000; i++)
                  {
                      gotLock = false;
                      try
                      {
                          sl.Enter(ref gotLock);
                          stringbuilder.Append((i % 10).ToString());
                      }
                      finally
                      {
                          if (gotLock)
                              sl.Exit();
                      }
                  }
              };

            Parallel.Invoke(action, action, action);

            Console.WriteLine($"stringBuilder.Length = {stringbuilder.Length}(should be 30000)");
            Console.WriteLine($"number of occurrences of '5' in stringbuiler:{stringbuilder.ToString().Where(c => (c == '5')).Count()}(should be 3000)");

        }

        static void SemaphoreExample()
        {
            //限制访问共享资源的线程数目

            // A semaphore that simulates a limited resource pool.
            //
            var pool = new Semaphore(0, 3);
            // A padding interval to make the output more orderly.
            var initPadding = 0;

            Action<int> worker = (num) =>
            {
                // Each worker thread begins by requesting the
                // semaphore.
                Console.WriteLine("Thread {0} begins " +
                    "and waits for the semaphore.", num);
                pool.WaitOne();

                // A padding interval to make the output more orderly.
                int padding = Interlocked.Add(ref initPadding, 100);

                Console.WriteLine("Thread {0} enters the semaphore.", num);

                // The thread's "work" consists of sleeping for 
                // about a second. Each thread "works" a little 
                // longer, just to make the output more orderly.
                //
                Thread.Sleep(1000 + padding);

                Console.WriteLine("Thread {0} releases the semaphore.", num);
                Console.WriteLine("Thread {0} previous semaphore count: {1}",
                    num, pool.Release());

            };

            //同时运行5个任务
            var tasks = new Task[5];
            for (int i = 0; i < tasks.Length; i++)
            {
                var number = i;
                tasks[i] = Task.Run(() => worker(number));
            }

            // Wait for half a second, to allow all the
            // threads to start and to block on the semaphore.
            //
            Thread.Sleep(500);

            // The main thread starts out holding the entire
            // semaphore count. Calling Release(3) brings the 
            // semaphore count back to its maximum value, and
            // allows the waiting threads to enter the semaphore,
            // up to three at a time.
            //
            Console.WriteLine("Main thread calls Release(3).");
            pool.Release(3);

            Console.WriteLine("Main thread exits.");
        }

        static void MutexSample()
        {

            //Mutex 只能用于进程内同步
            var mutex = new Mutex();
            var numIterations = 1;

            Action action = () =>
            {
                for (int i = 0; i < numIterations; i++)
                {
                    Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId} is requestring the mutex");
                    mutex.WaitOne();

                    Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId} has entered the protected area");
                    // place code to access non-reentrant resources here

                    //simulate some work

                    Thread.Sleep(500);
                    Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId} is leaving the protected area");
                    mutex.ReleaseMutex();
                    Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId} has released the mutex");
                }
            };

            Parallel.Invoke(action, action, action);

        }
    }

}
