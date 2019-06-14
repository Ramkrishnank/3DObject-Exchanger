using devDept.Eyeshot;
using devDept.Eyeshot.Entities;
using devDept.Eyeshot.Translators;
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
       // private static readonly Color NOT_MODIFIED_COLOR = Color.FromArgb(44, 44, 44);
        public static Snackbar Snackbar;
        public MainWindow()
        {
            InitializeComponent();
            //view.RotateGesture = new MouseGesture(MouseAction.LeftClick);
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
          
            string objFile = "Test.Step";
           SaveFileDialog saveFileDialog = new SaveFileDialog();
            var writeSTL = new WriteSTEP(new WriteParamsWithUnits(model1), objFile);
            var RS = new WriteParamsWithMaterials(model1);

            writeSTL.DoWork();
            saveFileDialog.Title = "Exported Model";
           if(saveFileDialog.ShowDialog() == true)
            {
                saveFileDialog.CheckFileExists = true;
                saveFileDialog.AddExtension = true;
                saveFileDialog.FileName = writeSTL.ToString();
                saveFileDialog.Filter = "Object files (*.STL)|*.STL|All files (*.*)|*.*";
                //saveFileDialog.FileOk(writeSTL);
            }

            string stlFile = "chair.stl";
            WriteSTL ws = new WriteSTL(new WriteParams(model1), stlFile, true);

            ws.DoWork();

            string fullPath = String.Format(@"{0}\{1}", System.Environment.CurrentDirectory, stlFile);
            MessageBox.Show(String.Format("File saved in {0}", fullPath));
            WriteSTEP wrStep = new WriteSTEP(new WriteParamsWithUnits(model1), "wing.stp");
            wrStep.DoWork();
            MessageBox.Show("File 'wing.stp' successfully written.");

        }
        private void btnImport_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Filter = "STL File|*.STL",
                Multiselect = false,
                AddExtension = true,
                CheckFileExists = true,
                CheckPathExists = true
            };
            openFileDialog.InitialDirectory = "c:\\";

          //  openFileDialog.Filter = "Object files (*.STL,*.OBJ,*.FBX,*.3DS)|*.STL,*.OBJ,*.FBX,*.3DS|All files (*.*)|*.*";
           
            if (openFileDialog.ShowDialog() == true)
            {
                //Get the path of specified file
                var filePath = openFileDialog.FileName;

                //Read the contents of the file into a stream
                var fileStream = openFileDialog.OpenFile();
                var fileContent = "";
                using (StreamReader reader = new StreamReader(fileStream))
                {
                    //devDept.Eyeshot.Translators.ReadFile filePath = new devDept.Eyeshot.Translators.ReadFile(filePath);
                    model1.Clear();
                    var readAutodesk = new ReadAutodesk (filePath);
                    readAutodesk.DoWork();
                    readAutodesk.AddToScene(model1);
                    var ReadObj = new ReadSTL(filePath);
                    ReadObj.DoWork();
                    Entity[] toAdd = ReadObj.Entities;
                    
                    //fileContent = reader.ReadToEnd();
                    //ModelVisual3D device3D = new ModelVisual3D();
                    //Viewport3DVisual viewport = new Viewport3DVisual();
                    //viewport readFile;
                   
                    
                    model1.Entities.AddRange(toAdd, System.Drawing.Color.Chocolate);
                    //model1.SetView(viewType.Top);
                    //model1.ZoomFit();
                    model1.Invalidate();
                }
                //view.Export(filePath);
              
            }
        }
        //private Model3D Display3D(string ModelPath)
        //{
        //    Model3D Device = null;
        //    try
        //    {
        //        view.RotateGesture = new MouseGesture(MouseAction.LeftClick);
        //        ModelImporter import = new ModelImporter();
        //        Device = import.Load(ModelPath);
        //    }
        //    catch (Exception E)
        //    {
        //        MessageBox.Show("Exception:" + E.StackTrace);
        //    }
        //    return Device;
        //}

        //protected override void OnContentRendered(EventArgs e)
        //{
        //    model1.GetGrid().AutoSize = true;
        //    model1.GetGrid().Step = 500;

        //    devDept.Eyeshot.Translators.ReadFile readFile = new devDept.Eyeshot.Translators.ReadFile(Assets + "160.eye");
        //    readFile.DoWork();
        //    model1.Entities.AddRange(readFile.Entities, System.Drawing.Color.DimGray);

        //    Block firstBlock = new Block("First");
        //    AddStlToBlock(firstBlock, "930.eye", System.Drawing.Color.DeepSkyBlue);
        //    AddStlToBlock(firstBlock, "940.eye", System.Drawing.Color.DeepSkyBlue);

        //    Block secondBlock = new Block("Second");
        //    AddStlToBlock(secondBlock, "570.eye", System.Drawing.Color.DodgerBlue);

        //    Block thirdBlock = new Block("Third");
        //    AddStlToBlock(thirdBlock, "590.eye", System.Drawing.Color.SlateBlue);

        //    firstBlock.Entities.Add(new TranslatingAlongY("Second"));
        //    secondBlock.Entities.Add(new TranslatingAlongZ("Third"));

        //    model1.Blocks.Add(firstBlock);
        //    model1.Blocks.Add(secondBlock);
        //    model1.Blocks.Add(thirdBlock);

        //    model1.Entities.Add(new TranslatingAlongX("First"));

        //    model1.SetView(viewType.Trimetric);

        //    model1.ZoomFit();

        //    // Turn off silhouettes to increase drawing speed
        //    model1.Rendered.SilhouettesDrawingMode = silhouettesDrawingType.Never;

        //    // Shadows are not currently supported in animations
        //    model1.Rendered.ShadowMode = shadowType.None;

        //    model1.StartAnimation(_interval);
        //    startButton.IsEnabled = false;

        //    base.OnContentRendered(e);
        //}
    }
}
