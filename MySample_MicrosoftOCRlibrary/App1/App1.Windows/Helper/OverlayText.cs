using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using WindowsPreview.Media.Ocr;

namespace App1
{
    class OverlayText
    {
        private readonly List<Border> _overlayWordsList;

        public OverlayText()
        {
            _overlayWordsList = new List<Border>();
        }

        public ScaleTransform ScaleTransform { get; set; }
        public RotateTransform RotateTransform { get; set; }

        public List<Border> GetOverlayBordersList(OcrResult ocrResult, WriteableBitmap originalImageBitmap,
            Image previewImage)
        {
            if (ocrResult.Lines != null)
            {
               ScaleTransform = new ScaleTransform
                {
                    CenterX = 0,
                    CenterY = 0,
                    ScaleX = previewImage.ActualWidth/originalImageBitmap.PixelWidth,
                    ScaleY = previewImage.ActualHeight/originalImageBitmap.PixelHeight,
                };

                if (ocrResult.TextAngle != null)
                {
                    RotateTransform = new RotateTransform
                    {
                        Angle = (double)ocrResult.TextAngle,
                        CenterX = previewImage.ActualWidth / 2,
                        CenterY = previewImage.ActualHeight / 2
                    };

                    previewImage.RenderTransform = RotateTransform;
                }

                //iterate every lines of text
                foreach (var line in ocrResult.Lines)
                {
                    //iterate every words in line
                    foreach (var word in line.Words)
                    {
                        var originalRect = new Rect(word.Left, word.Top, word.Width, word.Height);
                        var overlayRect = ScaleTransform.TransformBounds(originalRect);

                        var wordTextBlock = new TextBlock()
                        {
                            Height = overlayRect.Height,
                            Width = overlayRect.Width,
                            FontSize = overlayRect.Height*0.8,
                            Text = word.Text,
                            Style = (Style) Application.Current.Resources["ExtractedWordTextStyle"]
                        };
                        var border = new Border()
                        {
                            Margin = new Thickness(overlayRect.Left, overlayRect.Top, 0, 0),
                            Width = overlayRect.Width,
                            Height = overlayRect.Height,
                            Child = wordTextBlock,
                            Style = (Style) Application.Current.Resources["ExtractedWordBorderStyle"]
                        };
                        _overlayWordsList.Add(border);
                    }
                }
            }

           return _overlayWordsList;
        }
    }
}
