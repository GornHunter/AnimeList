using AnimeList.Model;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
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
using System.Xml.Linq;

namespace AnimeList
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Anime> myItems = null;
        private int index = -3;

        private int GetPictureIndex(object sender)
        {
            var image = sender as Image;
            if (image != null)
            {
                var container = lb1.ItemContainerGenerator.ContainerFromItem(image.DataContext) as ListBoxItem;
                if (container != null)
                {
                    int index = lb1.ItemContainerGenerator.IndexFromContainer(container);
                    // Do something with the selected index

                    return index;
                }
            }

            return -3;
        }

        private static T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                if (child is T result)
                {
                    return result;
                }
                else
                {
                    T grandChild = FindVisualChild<T>(child);
                    if (grandChild != null)
                    {
                        return grandChild;
                    }
                }
            }
            return null;
        }

        private List<Anime> GetAllAnime()
        {
            var anime = AnimeDatabase.GetAnime();

            anime.ForEach(item =>
            {
                var picture = item.GetValue("Picture").ToString() == "" ? new Uri("pack://application:,,,/Images/pic4.jpg") : new Uri(item.GetValue("Picture").ToString());
                myItems.Add(new Anime(item.GetValue("_id").ToString(), picture, item.GetValue("Title").ToString()));
            });

            return myItems;
        }

        public MainWindow()
        {
            InitializeComponent();

            myItems = new List<Anime>();

            #region
            //string line;
            //using (StreamReader reader = new StreamReader("list.txt"))
            //{
            //    while ((line = reader.ReadLine()) != null)
            //    {
            //        if (line.Split('.')[1] != "")
            //            myItems.Add(new Anime(new Uri("pack://application:,,,/Images/pic1.jpg"), line));
            //    }
            //}

            //lb1.ItemsSource = myItems;
            #endregion

            //AnimeDatabase.AddAnime(new Anime(new Uri("pack://application:,,,/Images/pic1.jpg"), "naruto"));
            //AnimeDatabase.AddAnime(new Anime(new Uri("pack://application:,,,/Images/pic2.jpg"), "one piece"));
            //AnimeDatabase.AddAnime(new Anime(new Uri("pack://application:,,,/Images/pic3.jpg"), "bleach"));
            //AnimeDatabase.AddAnime(new Anime(new Uri("pack://application:,,,/Images/pic4.jpg"), "fairy tail"));
            //AnimeDatabase.AddAnime(new Anime(new Uri("pack://application:,,,/Images/pic3.jpg"), "death note"));
            //AnimeDatabase.AddAnime(new Anime(new Uri("pack://application:,,,/Images/pic2.jpg"), "tokyo ghoul"));

            lb1.ItemsSource = GetAllAnime();
        }

        private void lb1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //listBox = sender as ListBox;

            //if (listBox.SelectedItem != null)
            //{
            //    Anime selectedAnime = listBox.SelectedItem as Anime;
            //    myItems.Remove(selectedAnime);
            //    lb1.ItemsSource = null;
            //    lb1.ItemsSource = myItems;
            //}
        }

        private void tb1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            TextBox textBox = new TextBox();
            
            textBox.Name = "textBox1";
            textBox.Text = ((TextBlock)sender).Text;
            textBox.VerticalAlignment = ((TextBlock)sender).VerticalAlignment;
            textBox.Margin = ((TextBlock)sender).Margin;
            textBox.FontSize = ((TextBlock)sender).FontSize;


            StackPanel stackPanel = (StackPanel)((TextBlock)sender).Parent;
            stackPanel.Children.Remove((TextBlock)sender);
            stackPanel.Children.Add(textBox);


            textBox.Height = 30;
            textBox.KeyDown += textBox1_KeyDown;
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                // Create a new TextBox
                TextBlock textBlock = new TextBlock();


                textBlock.Name = "tb1";
                textBlock.Text = ((TextBox)sender).Text;
                textBlock.VerticalAlignment = ((TextBox)sender).VerticalAlignment;
                textBlock.Margin = ((TextBox)sender).Margin;
                textBlock.FontSize = ((TextBox)sender).FontSize;


                StackPanel stackPanel = (StackPanel)((TextBox)sender).Parent;
                stackPanel.Children.Remove((TextBox)sender);
                stackPanel.Children.Add(textBlock);

                textBlock.Height = 30;
                textBlock.MouseDown += tb1_MouseDown;

                myItems[lb1.SelectedIndex].Title = textBlock.Text;
                Anime selectedAnime = (Anime)lb1.Items[lb1.SelectedIndex];
                AnimeDatabase.UpdateAnimeTitle(selectedAnime);
            }
        }

        private void img1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            index = GetPictureIndex(sender);

            if (index != -3)
            {
                Anime selectedAnime = (Anime)lb1.Items[index];

                myItems.Remove(selectedAnime);
                AnimeDatabase.DeleteAnime(selectedAnime);

                lb1.ItemsSource = null;
                lb1.ItemsSource = myItems;
            }
        }

        private void img1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Copy;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
            e.Handled = true;
        }

        private void img1_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Copy;
                index = GetPictureIndex(sender);
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
            e.Handled = true;
        }

        private void img1_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files.Length > 0 && files[0].EndsWith(".jpg"))
                {
                    var listBoxItem = lb1.ItemContainerGenerator.ContainerFromIndex(index) as ListBoxItem;
                    var imageControl = FindVisualChild<Image>(listBoxItem);

                    if (imageControl.Source != null)
                    {
                        imageControl.Source = new BitmapImage(new Uri(files[0]));
                        string name = imageControl.Source.ToString().Split('/')[9];
                        myItems[index].Picture = new Uri("pack://application:,,,/Images/" + name);

                        Anime selectedAnime = (Anime)lb1.Items[index];
                        AnimeDatabase.UpdateAnimePicture(selectedAnime);

                        lb1.ItemsSource = null;
                        lb1.ItemsSource = myItems;
                    }
                }
            }
        }

        private void btn1_Click(object sender, RoutedEventArgs e)
        {
            AnimeDatabase.AddAnime(new Anime(null, tbInput.Text));
            string id = AnimeDatabase.GetLastAddedAnimeId();
            myItems.Add(new Anime(id, new Uri("pack://application:,,,/Images/pic4.jpg"), tbInput.Text));
            tbInput.Text = "";

            lb1.ItemsSource = null;
            lb1.ItemsSource = myItems;
        }
    }
}
