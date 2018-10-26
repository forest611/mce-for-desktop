using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        int size = 60;
        int delay = 5000;
        static int id = 0;

        public MainWindow()
        {
            InitializeComponent();
            getCSV();
            setButton();

            //時計処理
            DispatcherTimer timer = new DispatcherTimer() { Interval = new TimeSpan(0, 0, 1) };
            timer.Tick += (s, ex) => { time.Content = DateTime.Now.ToString("h:mm:ss"); };
            timer.Start();

            Task.Run(() =>
            {
                getCSV();
                updateButtonContent();
                Console.WriteLine("task");
                Task.Delay(delay);
            });

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
                    var csv = new System.IO.StreamReader($"http://man10.red/mce/{i}/index.csv", System.Text.Encoding.GetEncoding("UTF-8"));
                    using (csv) ;
                    itemData.Add(i, csv.ReadLine().Split(new string[] { " ", "　" }, StringSplitOptions.RemoveEmptyEntries));

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
            data.Content = itemData[id][0] + "        " + itemData[id][1];

        }

        private void update(object sender, RoutedEventArgs e)
        {
            data.Content = itemData[id][0] + "        " + itemData[id][1];
        }
    }
}
