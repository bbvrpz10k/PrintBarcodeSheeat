
using BarcodeLib;
using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Printing;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PrintBarcodeSheeat
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly Regex _regex = new Regex("[^0-9.-]+"); //regex that matches disallowed text
        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }
        public IEnumerable<DataBarCode> DataPrintCode2 { get; set; }

        private int DPI = 96;
        PrintDialog dialog = new PrintDialog();
        public Settings SettingsProp { get; set; } = new Settings();

        public IEnumerable<DataBarCode> DataPrintCode
        {
            get { return (IEnumerable<DataBarCode>)GetValue(DataPrintCodeProperty); }
            set { SetValue(DataPrintCodeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DataPrintCodeProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DataPrintCodeProperty =
            DependencyProperty.Register("DataPrintCode", typeof(IEnumerable<DataBarCode>), typeof(MainWindow), new PropertyMetadata(new List<DataBarCode>() { new DataBarCode() {codeText="1",DescriptionText="Monitor"},
                new DataBarCode() {codeText="3",DescriptionText="Персональний"},
                new DataBarCode() {codeText = "4",DescriptionText = "Монітор"},
                new DataBarCode() {codeText="5",DescriptionText="Ситемний"},
                new DataBarCode() {codeText="6",DescriptionText="Тестування"},
                new DataBarCode() {codeText = "7",DescriptionText = "Сервер"} })); 


        public MainWindow()
        {
            InitializeComponent();
            //Prints.ItemsSource=PrinterSettings.InstalledPrinters;
            // Prints.SelectedItem= LocalPrintServer.GetDefaultPrintQueue().Name;
            Prints.ItemsSource = new PrintServer().GetPrintQueues(new[] { EnumeratedPrintQueueTypes.Local, EnumeratedPrintQueueTypes.Connections });
            var rr = LocalPrintServer.GetDefaultPrintQueue();
            foreach (var a in Prints.Items)
               if(a is PrintQueue aa)
                    Log(aa.Description);
            Prints.SelectedItem = LocalPrintServer.GetDefaultPrintQueue();
            Log(LocalPrintServer.GetDefaultPrintQueue().Description);
        }

        private void GeneratePage()
        {
            foreach (var bar in DataPrintCode)
            {
                if (bar.Settings is null)
                    bar.Settings = SettingsProp;
            }
            FixedDocument fixedDocument = new FixedDocument();
            PageContent pageContent1 = new PageContent();

            FixedPage fixedPage1 = new FixedPage();

            Canvas canvas1 = new Canvas();

            //fixedPage1.Children.Add(PreViewPrint);
            FixedPage page = new FixedPage();
            page.Background = Brushes.White;
            page.Width = DPI * SettingsProp.WidthPage;
            page.Height = DPI * SettingsProp.HeightPage;
            //TextBlock tbTitle = new TextBlock();
            //tbTitle.Text = "My Page Title";
            //tbTitle.FontSize = 24;
            //tbTitle.FontFamily = new FontFamily("Arial");
            //FixedPage.SetLeft(tbTitle, DPI * 0.75); // left margin
            //FixedPage.SetTop(tbTitle, DPI * 0.75); // top margin
            ////page.Children.Add((UIElement)tbTitle);
            //Border b = new Border();
            //b.BorderThickness = new Thickness(6);
            //b.BorderBrush = Brushes.Yellow;
            //b.IsEnabled=false;
            //FixedPage.SetLeft(b, DPI * 2);
            //FixedPage.SetTop(b, DPI * 2);
            //page.Children.Add((UIElement)b);
            ListBox list = new ListBox();
            list.ItemsSource = DataPrintCode;
            list.ItemTemplate = (DataTemplate)FindResource( "BarTemplate") ;
            list.ItemsPanel = (ItemsPanelTemplate)FindResource("PanelTemplate");
            list.Width = 850;
            //Decorator border = VisualTreeHelper.GetChild(list, 0) as Decorator;
            //// Get scrollviewer
            //ScrollViewer scrollViewer = border.Child as ScrollViewer;
            //scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
            FixedPage.SetLeft(list, DPI * SettingsProp.ListLeftMargin);
            FixedPage.SetTop(list, DPI * SettingsProp.ListTopMargin);
            page.Children.Add((UIElement)list);
            Size sz = new Size(DPI * 8.3, DPI * 11.7);
            page.Measure(sz);
            page.Arrange(new Rect(new Point(10,10), sz));
            ScrollViewer.SetHorizontalScrollBarVisibility(list, ScrollBarVisibility.Disabled);
            page.UpdateLayout();

            pageContent1.BeginInit();

            ((IAddChild)pageContent1).AddChild(page);

            pageContent1.EndInit();
            fixedDocument.Pages.Add(pageContent1);
            docViewer.Document = fixedDocument;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Barcode barcode = new Barcode();
            barcode.Alignment = AlignmentPositions.CENTER;
            using (var ms = new MemoryStream())
            {
                barcode.Encode(BarcodeLib.TYPE.CODE128, ((TextBox)sender).Text).Save(ms, ImageFormat.Bmp);
                ms.Seek(0, SeekOrigin.Begin);

                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = ms;
                bitmapImage.EndInit();

                FirstBarCode.Source = bitmapImage;
            }
             ;
        }

        private void DataView_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // Note that you can have more than one file.
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                // Assuming you have one file that you care about, pass it off to whatever
                // handling code you have defined.
                dataFromFile(files[0]);
            }

        }
        private void dataFromFile(string patch)
        {
            try
            {
                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = false,
                };
                using (var reader = new StreamReader(patch))
                using (var csv = new CsvReader(reader, config))
                {
                    DataPrintCode = csv.GetRecords<DataBarCode>().ToList();
                }
                GeneratePage();
            }
            catch (Exception exp)
            {
                
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();
            //if (printDialog.ShowDialog() == true)
            {
                printDialog.PrintVisual(FirstBarCode, "Распечатываем элемент Canvas");
            }
        }

        private void Prints_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GeneratePage();
            if (Prints.SelectedIndex != -1)
            {
                // The combo box's Text property returns the selected item's text, which is the printer name.
                //dialog.PrinterSettings.PrinterName = Prints.Text;
                dialog.PrintQueue = (PrintQueue)Prints.SelectedItem;
            }
        }
        void Log(string logdata)
        {
            string path="log.log";
            if (!File.Exists(path))
            {
                // Create a file to write to.
                using (StreamWriter sw = File.CreateText(path))
                {
                    sw.WriteLine(DateTime.Now.ToString() + ":---  " + logdata);
                }
            }
            else

            // This text is always added, making the file longer over time
            // if it is not deleted.
            using (StreamWriter sw = File.AppendText(path))
            {
                sw.WriteLine(DateTime.Now.ToString()+":---  "+logdata);
            }
        }

        private void docViewer_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {

            }
            catch (Exception ee)
            {

            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            GeneratePage();
        }

        private void NumberTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }
    }
}
