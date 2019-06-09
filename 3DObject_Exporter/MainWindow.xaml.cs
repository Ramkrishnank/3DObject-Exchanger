using HelixToolkit.Wpf;
using MaterialDesignColors.WpfExample.Domain;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace _3DObject_Exporter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static Snackbar Snackbar;
        public MainWindow()
        {
            InitializeComponent();
            view.RotateGesture = new MouseGesture(MouseAction.LeftClick);
            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(2500);
            }).ContinueWith(t =>
            {
                //note you can use the message queue from any thread, but just for the demo here we 
                //need to get the message queue from the snackbar, so need to be on the dispatcher
                MainSnackbar.MessageQueue.Enqueue("Welcome to 3D Object Exchanger Tookit");
            }, TaskScheduler.FromCurrentSynchronizationContext());

           // DataContext = new MainWindowViewModel(MainSnackbar.MessageQueue);

            Snackbar = this.MainSnackbar;
        }

        private void UIElement_OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //until we had a StaysOpen glag to Drawer, this will help with scroll bars
            var dependencyObject = Mouse.Captured as DependencyObject;
            while (dependencyObject != null)
            {
                if (dependencyObject is ScrollBar) return;
                dependencyObject = VisualTreeHelper.GetParent(dependencyObject);
            }

            MenuToggleButton.IsChecked = false;
        }

        private async void MenuPopupButton_OnClick(object sender, RoutedEventArgs e)
        {
            //var sampleMessageDialog = new SampleMessageDialog
            //{
            //    Message = { Text = ((ButtonBase)sender).Content.ToString() }
            //};

           // await DialogHost.Show(sampleMessageDialog, "RootDialog");
        }

        private void OnCopy(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Parameter is string stringValue)
            {
                try
                {
                    Clipboard.SetDataObject(stringValue);
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.ToString());
                }
            }
        }
        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            //pgBar.IsEnabled=true;
            //pgBar.Value = 90;
           SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "Exported Model";
           if(saveFileDialog.ShowDialog() == true)
            {
                saveFileDialog.CheckFileExists = true;
                saveFileDialog.AddExtension = true;
                saveFileDialog.FileName = view.GetName();
                saveFileDialog.Filter = "Object files (*.STL)|*.STL|All files (*.*)|*.*";
                // saveFileDialog.FileOk(sender);
            }

        }
        private void btnImport_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = "c:\\";

            openFileDialog.Filter = "Object files (*.STL,*.OBJ,*.FBX,*.3DS)|*.STL,*.OBJ,*.FBX,*.3DS|All files (*.*)|*.*";
           
            if (openFileDialog.ShowDialog() == true)
            {
                //Get the path of specified file
                var filePath = openFileDialog.FileName;

                //Read the contents of the file into a stream
                var fileStream = openFileDialog.OpenFile();
                var fileContent = "";
                using (StreamReader reader = new StreamReader(fileStream))
                {
                    fileContent = reader.ReadToEnd();
                    ModelVisual3D device3D = new ModelVisual3D();
                    device3D.Content = Display3D(filePath);
                    view.Children.Add(device3D);
                }
                //view.Export(filePath);
              
            }
        }
        private Model3D Display3D(string ModelPath)
        {
            Model3D Device = null;
            try
            {
                view.RotateGesture = new MouseGesture(MouseAction.LeftClick);
                ModelImporter import = new ModelImporter();
                Device = import.Load(ModelPath);
            }
            catch (Exception E)
            {
                MessageBox.Show("Exception:" + E.StackTrace);
            }
            return Device;
        }
    }
}
