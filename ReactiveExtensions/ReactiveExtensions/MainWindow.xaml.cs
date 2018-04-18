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
        public MainWindow()
        {
            InitializeComponent();
            MainMethod();
        }

        static IEnumerable<string> EndlessBarrageOfEmail()
        {
            var random = new Random();
            var emails = new List<String> { "Here is an email!", "Another email!", "Yet another email!" };
            for (; ; )
            {
                // Return some random emails at random intervals.
                yield return emails[random.Next(emails.Count)];
                Thread.Sleep(random.Next(1000));
            }
        }
        static void MainMethod()
        {
            var myInbox = EndlessBarrageOfEmail().ToObservable();

            // Instead of making you wait 5 minutes, we will just check every three seconds instead. :)
            var getMailEveryThreeSeconds = myInbox.Buffer(TimeSpan.FromSeconds(3)); //  Was .BufferWithTime(...

            getMailEveryThreeSeconds.Subscribe(emails =>
            {
                Console.WriteLine("You've got {0} new messages!  Here they are!", emails.Count());
                foreach (var email in emails)
                {
                    Console.WriteLine("> {0}", email);
                }
                Console.WriteLine();
            });

            Console.ReadKey();
        }
    }
}