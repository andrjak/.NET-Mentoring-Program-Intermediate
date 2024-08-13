/*
*  Create a Task and attach continuations to it according to the following criteria:
   a.    Continuation task should be executed regardless of the result of the parent task.
   b.    Continuation task should be executed when the parent task finished without success.
   c.    Continuation task should be executed when the parent task would be finished with fail and parent task thread should be reused for continuation
   d.    Continuation task should be executed outside of the thread pool when the parent task would be cancelled
   Demonstrate the work of the each case with console utility.
*/
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MultiThreading.Task6.Continuation
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Create a Task and attach continuations to it according to the following criteria:");
            Console.WriteLine("a.    Continuation task should be executed regardless of the result of the parent task.");
            Console.WriteLine("b.    Continuation task should be executed when the parent task finished without success.");
            Console.WriteLine("c.    Continuation task should be executed when the parent task would be finished with fail and parent task thread should be reused for continuation.");
            Console.WriteLine("d.    Continuation task should be executed outside of the thread pool when the parent task would be cancelled.");
            Console.WriteLine("Demonstrate the work of the each case with console utility.");
            Console.WriteLine();

            // feel free to add your code

            // A
            TaskDecorator(ShouldBeExecutedRegardlessOfTheResultOfTheParentTask, "A");

            // B
            TaskDecorator(ShouldBeExecutedWhenTheParentTaskWasCompletedWithoutSuccess, "B");

            // C
            TaskDecorator(ShouldBeExecutedWhenTheParentTaskFailedAndParentTaskThreadShouldBeReusedForContinuation, "C");

            // D
            TaskDecorator(ShouldBeExecutedOutsideOfTheThreadPoolWhenTheParentTaskIsCancelled, "D");

            Console.ReadLine();
        }

        // A
        public static void ShouldBeExecutedRegardlessOfTheResultOfTheParentTask()
        {
            using var cts = new CancellationTokenSource();
            CancellationToken token = GetCancellationTokenWithTimer(cts);

            var task1 = Task.Run(ParentBody)
            .ContinueWith(ChildBody);

            var task2 = Task.Run(() =>
            {
                ParentBody();
                throw null;
            })
            .ContinueWith(ChildBody);

            var task3 = Task.Run(() =>
            {
                ParentBody();
                token.ThrowIfCancellationRequested();
            }, token)
            .ContinueWith(ChildBody);

            Task.WaitAll(task1, task2, task3);
        }

        // B
        public static void ShouldBeExecutedWhenTheParentTaskWasCompletedWithoutSuccess()
        {
            using var cts = new CancellationTokenSource();
            CancellationToken token = GetCancellationTokenWithTimer(cts);

            var task1 = Task.Run(() =>
            {
                ParentBody();
                token.ThrowIfCancellationRequested();
            }, token)
            .ContinueWith(ChildBody, TaskContinuationOptions.NotOnRanToCompletion);

            var task2 = Task.Run(() =>
            {
                ParentBody();
                throw null;
            }, token)
            .ContinueWith(ChildBody, TaskContinuationOptions.NotOnRanToCompletion);

            Task.WaitAll(task1, task2);
        }

        // C
        public static void ShouldBeExecutedWhenTheParentTaskFailedAndParentTaskThreadShouldBeReusedForContinuation()
        {
            var task1 = Task.Run(() =>
            {
                ParentBody();
                throw null;
            })
            .ContinueWith(ChildBody, TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnFaulted);

            var task2 = Task.Run(ParentBody);

            // Not executed, end status Canceled 
            var task3 = task2.ContinueWith(ChildBody, TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnFaulted);

            Task.WaitAll(task1, task2);
            Console.WriteLine($"Child not started end status: {task3.Status}");
        }

        // D
        public static void ShouldBeExecutedOutsideOfTheThreadPoolWhenTheParentTaskIsCancelled()
        {
            using var cts = new CancellationTokenSource();
            CancellationToken token = GetCancellationTokenWithTimer(cts);

            var task1 = Task.Run(() =>
            {
                ParentBody();
                token.ThrowIfCancellationRequested();
            }, token)
            .ContinueWith(ChildBody, TaskContinuationOptions.OnlyOnCanceled
                | TaskContinuationOptions.RunContinuationsAsynchronously
                | TaskContinuationOptions.LongRunning);

            var task2 = Task.Run(ParentBody, token);

            // Not executed, end status Canceled 
            var task3 = task2.ContinueWith(ChildBody, TaskContinuationOptions.OnlyOnCanceled
                | TaskContinuationOptions.RunContinuationsAsynchronously
                | TaskContinuationOptions.LongRunning);

            Task.WaitAll(task1, task2);
            Console.WriteLine($"Child not started end status: {task3.Status}");
        }

        public static void ChildBody(Task lastTask)
        {
            Console.WriteLine("Start Child");
            Thread.Sleep(2000);
            Console.WriteLine($"End Child, with status: {lastTask.Status}");
        }

        public static void ParentBody()
        {
            Console.WriteLine("Start Parent");
            Thread.Sleep(2000);
            Console.WriteLine("End Parent");
        }

        public static CancellationToken GetCancellationTokenWithTimer(CancellationTokenSource cts)
        {
            CancellationToken token = cts.Token;
            var timer = new Timer(tokenSource =>
            {
                (tokenSource as CancellationTokenSource).Cancel();
            },
            cts,
            1000,
            Timeout.Infinite);

            return token;
        }

        public static void TaskDecorator(Action task, string taskNumber)
        {
            Console.WriteLine($"----- Start Task: {taskNumber}");
            Console.WriteLine();
            task();
            Console.WriteLine();
            Console.WriteLine($"----- End Task: {taskNumber}");
        }
    }
}
