/******************************************************************************/
/*                                                                            */
/*   Program: MyWpfWokerThreadApp                                             */
/*   Example for                                                              */
/*                                                                            */
/*   28.11.2014 1.0.0.0 uhwgmxorg Start                                       */
/*                                                                            */
/******************************************************************************/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace MyWpfWokerThreadApp
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Thrad and sync. var's
        /// </summary>
        /// <param name="e"></param>
        public delegate void MyThreadEventHandler(MyEventArgs e);
        public virtual Dispatcher DispatcherObject { get; protected set; }
        Thread[] Threads = new Thread[3];
        private static ManualResetEvent[] Events = new ManualResetEvent[3];
        private MyThreadEventHandler[] HandleThreadEvents = new MyThreadEventHandler[3];

        /// <summary>
        /// UI Properties
        /// </summary>
        private string threadValue01;
        public string ThreadValue01 { get { return threadValue01; } set { threadValue01 = value; OnPropertyChanged("ThreadValue01"); } }
        private string threadValue02;
        public string ThreadValue02 { get { return threadValue02; } set { threadValue02 = value; OnPropertyChanged("ThreadValue02"); } }
        private string threadValue03;
        public string ThreadValue03 { get { return threadValue03; } set { threadValue03 = value; OnPropertyChanged("ThreadValue03"); } }
        private string message;
        public string Message { get { return message; } set { message = value; OnPropertyChanged("Message"); } }

        /// <summary>
        /// MainWindow
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            // some UI init
            ThreadValue01 = "00";
            ThreadValue02 = "00";
            ThreadValue03 = "00";
            Message = "Click to delete Massage";

            StartThreads();
        }

        /******************************/
        /*       Button Events        */
        /******************************/
        #region Button Events

        /// <summary>
        /// Button_Click_1
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (Events[0].WaitOne(0))
            {
                Events[0].Reset();
                Message = "Suspend Thread 1";
            }
            else
            {
                Events[0].Set();
                Message = "Resume Thread 1";
            }
        }

        /// <summary>
        /// Button_Click_2
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (Events[1].WaitOne(0))
            {
                Events[1].Reset();
                Message = "Suspend Thread 2";
            }
            else
            {
                Events[1].Set();
                Message = "Resume Thread 2";
            }
        }

        /// <summary>
        /// Button_Click_3
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            if (Events[2].WaitOne(0))
            {
                Events[2].Reset();
                Message = "Suspend Thread 3";
            }
            else
            {
                Events[2].Set();
                Message = "Resume Thread 3";
            }
        }

        #endregion

        /******************************/
        /*      Menu Events          */
        /******************************/
        #region Menu Events

        #endregion

        /******************************/
        /*      Other Events          */
        /******************************/
        #region Other Events

        /// <summary>
        /// Label_MouseDown
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Label_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Message = "";
        }

        /// <summary>
        /// HandleThreadEvent
        /// </summary>
        /// <param name="e"></param>
        private void HandleThreadEvent(MyEventArgs e)
        {
            if (DispatcherObject.Thread != Thread.CurrentThread)
                DispatcherObject.Invoke(new MyThreadEventHandler(DispatchThreadEvent), DispatcherPriority.ApplicationIdle, e);
            else
                DispatchThreadEvent(e);
        }

        /// <summary>
        /// Window_Closed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closed(object sender, EventArgs e)
        {
            // In this case exit hard
            Environment.Exit(0);
        }

        #endregion

        /******************************/
        /*      Other Functions       */
        /******************************/
        #region Other Functions

        /// <summary>
        /// StartThreads
        /// </summary>
        void StartThreads()
        {
            DispatcherObject = Dispatcher.CurrentDispatcher;
            StartTread(ref Threads[0], ref Events[0], HandleThreadEvent, 1,"Thread 1");
            StartTread(ref Threads[1], ref Events[1], HandleThreadEvent, 2, "Thread 2");
            StartTread(ref Threads[2], ref Events[2], HandleThreadEvent, 3, "Thread 3");
        }

        /// <summary>
        /// StartTread
        /// </summary>
        /// <param name="thread"></param>
        /// <param name="threadEventHandler"></param>
        /// <param name="thradName"></param>
        private void StartTread(ref Thread thread,ref ManualResetEvent mEvent, MyThreadEventHandler threadEventHandler, int myId, string thradName)
        {
            ManualResetEvent Event = new ManualResetEvent(true);
            mEvent = Event; 
            thread = new Thread(
            () =>
            {
                int n = 0;
                Thread.CurrentThread.Name = thradName;
                while (true)
                {
                    Event.WaitOne();
                    n++; if (n > 99) n = 0;
                    threadEventHandler(new MyEventArgs(myId, n.ToString()));
                    Thread.Sleep(500);
                }
            });
            Message = "Start thread: "+ thradName;
            thread.Start();
        }

        /// <summary>
        /// DispatchThreadEvent
        /// </summary>
        void DispatchThreadEvent(MyEventArgs e)
        {
            switch(e.Id)
            {
                case 1:
                    ThreadValue01 = e.Content;
                    break;
                case 2:
                    ThreadValue02 = e.Content;
                    break;
                case 3:
                    ThreadValue03 = e.Content;
                    break;
            }
        }

        /// <summary>
        /// OnPropertyChanged
        /// </summary>
        /// <param name="p"></param>
        private void OnPropertyChanged(string p)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(p));
        }

        #endregion
    }
}
