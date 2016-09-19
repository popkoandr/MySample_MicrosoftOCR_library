using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Controls;
using Windows.Storage.AccessCache;
using Windows.Storage.FileProperties;
using Windows.Storage.Provider;
using Windows.Storage.Search;
using Windows.UI.Xaml.Media.Imaging;

namespace App1
{
    public class ImageFileLoader
    {
        private readonly List<Image> _imagesFromFolder;
        private WriteableBitmap _bitmap;
        public ImageFileLoader()
        {
            _imagesFromFolder = new List<Image>();
        }
        public async  Task<List<Image>> GetImagesListFromFolder(StorageFolder folder)
        {
            StorageApplicationPermissions.FutureAccessList.AddOrReplace("PickedFolderToken", folder);
            var options = new QueryOptions();

            options.FileTypeFilter.Add(".jpg");
            options.FileTypeFilter.Add(".png");
            options.FileTypeFilter.Add(".jpeg");
            options.FileTypeFilter.Add(".gif");

            var query = folder.CreateFileQueryWithOptions(options);
            var files = await query.GetFilesAsync();
            
            foreach (StorageFile file in files)
            {
                ImageProperties imgProperties = await file.Properties.GetImagePropertiesAsync();

                using (var imgStream = await file.OpenAsync(FileAccessMode.Read))
                {
                  var  bitmapImage = new WriteableBitmap((int)imgProperties.Width, (int)imgProperties.Height);
                    bitmapImage.SetSource(imgStream);
                    Image image = new Image()
                    {
                        Width = 256,
                        Height = 144,
                        Source = bitmapImage,
                    };
                   _imagesFromFolder.Add(image);
                }
            }
            return _imagesFromFolder;
        }

        public async Task<WriteableBitmap> GetImageFromFile(StorageFile file)
        {
            
            ImageProperties imgProperties = await file.Properties.GetImagePropertiesAsync();
            _bitmap = new WriteableBitmap((int)imgProperties.Width, (int)imgProperties.Height);
            using (var imgStream = await file.OpenAsync(FileAccessMode.Read))
            {
                _bitmap.SetSource(imgStream);
            }
            return _bitmap;
        }

        public async Task<string> SaveTextFromImageToFile(StorageFile file, string text)
        {
            string answer = "";
            if (file != null)
            {
                // Prevent updates to the remote version of the file until we finish making changes and call CompleteUpdatesAsync.
                CachedFileManager.DeferUpdates(file);
                // write to file
                await FileIO.WriteTextAsync(file, text);
                // Let Windows know that we're finished changing the file so the other app can update the remote version of the file.
                // Completing updates may require Windows to ask for user input.
                FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(file);
                if (status == FileUpdateStatus.Complete)
                {
                    answer = "File " + file.Name + " was saved.";
                }
                else
                {
                    answer = "File " + file.Name + " couldn't be saved.";
                }
            }
            else
            {
                answer = "Operation cancelled.";
            }
            return answer;
        }
    }
}
