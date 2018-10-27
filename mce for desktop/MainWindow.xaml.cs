using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace mce_for_desktop
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        List<string> buttonName = new List<string>();
        List<Button> buttons = new List<Button>();
        static Dictionary<int, string[]> itemData = new Dictionary<int, string[]>();
        int size = 5;
        int delay = 10000;
        static int id = 0;

        public MainWindow()
        {
            InitializeComponent();
            getCSV();
            setButton();
            task();
            //時計処理
            DispatcherTimer timer = new DispatcherTimer() { Interval = new TimeSpan(0, 0, 1) };
            timer.Tick += (s, ex) => { time.Content = DateTime.Now.ToString("h:mm:ss"); };
            timer.Start();


        }



        private void setButton()
        {
            for(int i = 0;i!= size; i++)
            {
                buttonName.Add("button"+i);//list
                Button btn = new Button();
                btn.Height = 64;
                btn.Name = buttonName[i];//x:NAME
                btn.Click += Button_Click;
                btn.Content = itemData[i][0] + Environment.NewLine + itemData[i][1];
                panel.Children.Add(btn);
            }
        }

        private void getCSV()
        {
            itemData.Clear();
            for(int i = 0;i!=size; i++)
            {
                try
                {
                    //utf-8でcsvを読み込む 取得したデータを返す
                    //取得できなかった場合、 catchし、取得できないと返す
                    //$"http://man10.red/mce/{i}/index.csv", System.Text.Encoding.GetEncoding("UTF-8")
                    var request = new WebClient();
                    Stream st = request.OpenRead($"http://man10.red/mce/{i}/index.csv");
                    StreamReader sr = new StreamReader(st);
                    itemData.Add(i, sr.ReadLine().Split(new string[] { " ", "　" }, StringSplitOptions.RemoveEmptyEntries));

                }
                catch (System.Exception e)
                {
                    Console.WriteLine("csvを開くことができませんでした" + e.Message);
                    Console.WriteLine($"http://man10.red/mce/{i}/index.csv");
                    itemData.Add(i, new string[] { "取得不可", "$0", "" });
                }


            }
        }

        private void updateButtonContent()
        {
            for(int i = 0; i != buttons.Count(); i++)
            {
                buttons[i].Content = itemData[i][0] + Environment.NewLine + itemData[i][1];
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            id = buttonName.IndexOf(((Button)sender).Name);
            updateData();
            

        }

        private void update(object sender, RoutedEventArgs e)
        {
            updateData();
        }

        async private void task()
        {
            await Task.Run(() =>
            {
                getCSV();
                Dispatcher.Invoke(new Action(() =>
                {
                    updateData();
                    updateButtonContent();
                }));
                Console.WriteLine("task");
                Task.Delay(delay);
            });

        }
        private void updateData()
        {
            data.Content = itemData[id][0] + "        " + itemData[id][1];
        }

    }
}
