using System;
using System.Collections.Generic;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using WindowsPreview.Media.Ocr;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace App1
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    /// 
    public enum NotifyType
    {
        StatusMessage,
        ErrorMessage
    };

    public sealed partial class MainPage : Page
    {
        private WriteableBitmap _bitmapImage;
        private OverlayText _overlayText;
        private List<Image> _imagesAsPictureToCarousel;

        private readonly RecognizeEngine _recognizeEngine;
        readonly ImageFileLoader _imageLoader;
        public MainPage()
        {
            this._imageLoader = new ImageFileLoader();
            this.InitializeComponent();

            _recognizeEngine = new RecognizeEngine();
        }
        void ClearResult()
        {
            PreviewImage.RenderTransform = null;
            TextFromImage.Text = "Text not recognized";
            TextFromImage.Style = (Style)Application.Current.Resources["ResultTextBlockStyle"];

            RecognizeButton.IsEnabled = true;
            OverlayButton.IsEnabled = false;
            RepeatButton.IsEnabled = false;
            CopyButton.IsEnabled = false;
            TextOverlay.Children.Clear();
        }
        private async void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            var file = await FilePickers.OpenImageFilePicker();
            if (file != null)
            {
                _bitmapImage = await _imageLoader.GetImageFromFile(file);
                PreviewImage.Source = _bitmapImage;
                ClearResult();
            }
        }     
        
        private async void RecognizeButton_Click(object sender, RoutedEventArgs e)
        {
            OcrResult result = await _recognizeEngine.GetRecognizeResult(_bitmapImage);
            if (result != null)
            {
                _recognizeEngine.RecognizeTextFromImage(result);//*
                if (_recognizeEngine.AnswerEngine != null)
                {
                    TextFromImage.Text = _recognizeEngine.AnswerEngine;
                    EditingTextBox.Text = _recognizeEngine.AnswerEngine;
                    EditingTextBox.Visibility = Visibility.Visible;
                    EditStackPanel.Visibility = Visibility.Visible;

                }
                else
                {
                    TextFromImage.Text = "Smth wrong";
                }
                _overlayText = new OverlayText();
                List<Border> borders = _overlayText.GetOverlayBordersList(result, _bitmapImage, PreviewImage);

                foreach (var border in borders)
                {
                    TextOverlay.Children.Add(border);
                }
            }

            TextFromImage.Style = (Style)Application.Current.Resources["RecognizedText"];
                RecognizeButton.IsEnabled = false;
                CopyButton.IsEnabled = true;
                OverlayButton.IsEnabled = true;
                RepeatButton.IsEnabled = true;
                SaveToFileButton.IsEnabled = true;
        }

        private async void MakePhotoButton_Click(object sender, RoutedEventArgs e)
        {
            var file = await FilePickers.OperaCameraPhotoPicker();
            _bitmapImage = await _imageLoader.GetImageFromFile(file);
            PreviewImage.Source = _bitmapImage;
            ClearResult();
        }
        private async void ChangeFolderButton_Click(object sender, RoutedEventArgs e)
        {
            StorageFolder storageFolder = await FilePickers.OpenFolderPicker();

            // Clear previous returned folder name, if it exists, between iterations of this scenario
            FolderPathTextBlock.Text = "";
            if(storageFolder != null)
            {
                // Application now has read/write access to all contents in the picked folder (including other sub-folder contents)
                FolderPathTextBlock.Text = "Current folder: " + storageFolder.Path;
                
                _imagesAsPictureToCarousel = await _imageLoader.GetImagesListFromFolder(storageFolder);
                PhotoPreviewControl.ListImagesAsPictureCarousel = _imagesAsPictureToCarousel;
            }
            else
            {
                FolderPathTextBlock.Text = "No folder was selected";
            }
        }
 
        private async void SaveToFileButton_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(_recognizeEngine.AnswerEngine))
            {
                StatusTextBlock.Text = "No text for saving";
            }
            else
            {
                StatusTextBlock.Text = "Saving.....";
                StorageFile file = await FilePickers.OpenSaveFilePicker();
                StatusTextBlock.Text = await _imageLoader.SaveTextFromImageToFile(file, _recognizeEngine.AnswerEngine);
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            ClearResult();
        }

        private void OverlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (TextOverlay.Visibility == Visibility.Visible)
            {
             // OverlayButton.Background = new SolidColorBrush(Colors.Red);
                TextOverlay.Visibility = Visibility.Collapsed;
            }
            else
                TextOverlay.Visibility = Visibility.Visible;
           // OverlayButton.Background = null;
        }
        
    }
 }
    

