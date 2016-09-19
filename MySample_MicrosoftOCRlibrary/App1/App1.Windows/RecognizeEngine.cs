using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;
using WindowsPreview.Media.Ocr;


namespace App1
{
    public class RecognizeEngine
    {
        private readonly OcrEngine _ocrEngine;
        public string AnswerEngine { get; private set; }

        public RecognizeEngine()
        {
            _ocrEngine = new OcrEngine(OcrLanguage.English);
            AnswerEngine = "";
        }
        
        public async Task<OcrResult> GetRecognizeResult(WriteableBitmap bitmap)
        {
            OcrResult ocrResult;
            if (CanRecognaize(bitmap))
            {
                ocrResult =
                    await
                        _ocrEngine.RecognizeAsync((uint) bitmap.PixelHeight, (uint) bitmap.PixelWidth,
                            bitmap.PixelBuffer.ToArray());
            }
            else
            {
                AnswerEngine = "Image size is not supported." + Environment.NewLine + "Please, select photo from gallery with dimensions are between 40 and 2600 pixels.";
                ocrResult = null;
            }

            return ocrResult;
        }

        public void RecognizeTextFromImage(OcrResult ocrResult)
        {

            AnswerEngine = "";
                //if OCR result dont have any lines then no yext was recognized

                if (ocrResult != null && ocrResult.Lines != null)
                {
                    
                    //iterate every lines of text
                    foreach (var line in ocrResult.Lines)
                    {
                        //iterate every words in line
                        foreach (var word in line.Words)
                        {
                            AnswerEngine += word.Text + " ";
                        }
                        AnswerEngine += Environment.NewLine;
                    }
                }
                else
                {
                    AnswerEngine = "There are no symbols at image";
                }
        }
        
        public bool CanRecognaize(WriteableBitmap bitmap)
        {
            if (bitmap.PixelHeight < 40 ||
                bitmap.PixelHeight > 2600 ||
                bitmap.PixelWidth < 40 ||
                bitmap.PixelWidth > 2600)
            {
                return false;
            }
            return true;
        }
    }
}
