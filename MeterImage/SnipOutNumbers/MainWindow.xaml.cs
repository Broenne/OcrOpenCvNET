using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SnipOutNumbers
{
    using System.Drawing;

    using Microsoft.Win32;

    using Image = System.Windows.Controls.Image;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            button.Click += new RoutedEventHandler(btnLoad_Click);

        }

        //http://www.java2s.com/Code/CSharp/GUI-Windows-Form/LoadimagetoImageBox.htm


        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            //... Create a new BitmapImage.
            //BitmapImage b = new BitmapImage();
            //b.BeginInit();
            //var path = @"C:/Users/Public/Pictures/Sample Pictures/Chrysantheme.jpg";
            //b.UriSource = new Uri(path, UriKind.RelativeOrAbsolute);
            //b.EndInit();

            //// ... Get Image reference from sender.
            //var image = sender as Image;

            BitmapImage imageSource = new BitmapImage();
            imageSource.BeginInit();
            imageSource.UriSource = new Uri(@"C:\Users\Public\Pictures\Sample Pictures\Desert.bmp");
            imageSource.EndInit();
            // ... Assign Source.
            this.image.Source = imageSource;

        }

        //private void Image_Loaded(object sender, RoutedEventArgs e)
        //{
        //    // ... Create a new BitmapImage.
        //    BitmapImage b = new BitmapImage();
        //    b.BeginInit();
        //    b.UriSource = new Uri(@"C:\Users\Public\Pictures\Sample Pictures\Chrysantheme.jpg");
        //    b.EndInit();

        //    // ... Get Image reference from sender.
        //    var image = sender as Image;
        //    // ... Assign Source.
        //    image.Source = b;
        //}

    }
}
