﻿using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;



namespace Projektrichtig
{
    /// <summary>
    /// Interaktionslogik für HauptMenueFenster.xaml
    /// </summary>
    public partial class HauptMenueFenster : System.Windows.Window
    {
        private System.Timers.Timer MyDataTimer;
        FrameSource frameSource = Cv2.CreateFrameSource_Camera(0);
        public HauptMenueFenster()
        {
            InitializeComponent();
           

        }




        private void MyDataTimer_Elapsed(object source, ElapsedEventArgs e)
        {

            try
            {
                MyDataTimer.Stop();
                MyDataTimer.Interval = 1 * 1;
                this.Dispatcher.Invoke(new Action(LoadImages));
                MyDataTimer.Start();
            }
            catch (Exception ex)
            {
            }
            finally
            {
                MyDataTimer.Start();
            }
        }
        private void LoadImages()
        {
            using (var mat = new Mat())
            {
                frameSource.NextFrame(mat);

                var wb = WriteableBitmapConverter.ToWriteableBitmap(mat);
                KameraImage.Source = null;
                KameraImage.Source = wb;
            }
        }

        private void ButtonStart_Kamera(object sender, RoutedEventArgs e)
        {
            MyDataTimer = new System.Timers.Timer();
            MyDataTimer.Elapsed += new ElapsedEventHandler(MyDataTimer_Elapsed);
            MyDataTimer.Enabled = true;

            MyDataTimer.Interval = 1 * 1;
            MyDataTimer.Start();
        }

        private void ButtonStop_Kamera(object sender, RoutedEventArgs e)
        {
            KameraImage.Source = null;

            MyDataTimer.Stop();
        }

        public void KameraFotoMachen(object sender, RoutedEventArgs e)
        {
            using (var mat = new Mat())
            {
                frameSource.NextFrame(mat);

                var wbFoto = WriteableBitmapConverter.ToWriteableBitmap(mat);
                KameraFoto.Source = null;
                KameraFoto.Source = wbFoto;
            }
        }

        private void ÜberprüfenML(object sender, RoutedEventArgs e)
        {
            MLModelFinger.ModelInput sampleData = new MLModelFinger.ModelInput();

            byte[] imageBytes = BitmapSourceToBytes((BitmapSource)KameraFoto.Source);
            sampleData.ImageSource = imageBytes;

            //Load model and predict output
            var result = MLModelFinger.Predict(sampleData);



            // Zeige das Ergebnis in einer MessageBox an

            TextboxErgebnisML.Text = result.PredictedLabel;
          
        }



        public byte[] BitmapSourceToBytes(BitmapSource bitmapSource)
        {
            using (var stream = new MemoryStream())
            {
                var encoder = new PngBitmapEncoder(); // Du kannst auch einen anderen Encoder verwenden, abhängig von deinem Bildformat
                encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
                encoder.Save(stream);
                return stream.ToArray();
            }
        }

    }



    
}