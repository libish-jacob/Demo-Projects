using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Windows;

namespace ReactiveExtensions
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private event EventHandler<string> Event;

        public MainWindow()
        {
            InitializeComponent();            
        }

        static IEnumerable<string> EndlessBarrageOfEmail()
        {
            var random = new Random();
            var emails = new List<String> { "Here is an email!", "Another email!", "Yet another email!" };
            for (; ; )
            {
                // Return some random emails at random intervals.
                // this will start to send an edless stream of items.
                yield return emails[random.Next(emails.Count)];
                Thread.Sleep(random.Next(1000));
            }
        }
        void MainMethod()
        {
            var myInbox = EndlessBarrageOfEmail().ToObservable();
            var threadId = Thread.CurrentThread.ManagedThreadId;
            ////myInbox.Subscribe(email =>
            ////{
            ////    var executionThreadId = Thread.CurrentThread.ManagedThreadId;
            ////    Console.WriteLine($"You've got {email.Count()} new messages!  Here they are! This is initaited by {threadId} executed on {executionThreadId}");
                                
            ////    Console.WriteLine("> {0}", email);                
            ////    Console.WriteLine();
            ////});
            

            // Instead of making you wait 5 minutes, we will just check every three seconds instead.  
            // here as the result is an endless stream of data, we will process it in batch collected in 3 secs interval
            var getMailEveryThreeSeconds = myInbox.Buffer(TimeSpan.FromSeconds(3)); //  Was .BufferWithTime(...

            // Here the method EndlessBarrageOfEmail will not be executed untill subscribe is called. So EndlessBarrageOfEmail is executed by main thread here. So it will wait for the method to exit.
            // The callbacks are executed on a seperate thread.
            getMailEveryThreeSeconds.Subscribe(emails =>
            {   
                var executionThreadId = Thread.CurrentThread.ManagedThreadId;
                Console.WriteLine($"You've got {emails.Count()} new messages!  Here they are! This is initaited by {threadId} executed on {executionThreadId}");
                foreach (var email in emails)
                {   
                    Console.WriteLine("> {0}", email);
                }
                Console.WriteLine();
            });            
        }

        private void BatchExecution_Click(object sender, RoutedEventArgs e)
        {
            MainMethod();
        }

        private void NetEventAsObservable_Click(object sender, RoutedEventArgs e)
        {
            // dotnet event is made as suscribeable.

            var observableItem = Observable.FromEventPattern<string>(ev => Event += ev, ev => Event -= ev);
            var s = observableItem.Subscribe(args => Console.WriteLine("Received event for s subscriber"));
            var t = observableItem.Subscribe(args => Console.WriteLine("Received event for t subscriber"));

            Event(null, "hi");

            s.Dispose();
            t.Dispose();
        }

        private void IntervalSubscription_Click(object sender, RoutedEventArgs e)
        {   
            var clock = Observable.Interval(TimeSpan.FromSeconds(1))
            .Select((t, index) => DateTimeOffset.Now);
         
            clock.Subscribe(timeIndex =>
            {
                Console.WriteLine("Ding dong.  The time is now {0}.", timeIndex);
            });
        }


        /// have a look at Delay, Sample etc....
    }
}