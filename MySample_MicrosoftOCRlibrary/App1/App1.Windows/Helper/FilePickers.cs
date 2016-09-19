using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Media.Capture;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace App1
{
    class FilePickers
    {
        /// <summary>
        /// Method allows to open filePicker to select image/-s for opening
        /// </summary>
        /// <returns></returns>
        public static async Task<StorageFile> OpenImageFilePicker()
        {

            var picker = new FileOpenPicker()
            {
                SuggestedStartLocation = PickerLocationId.PicturesLibrary,
                FileTypeFilter = { ".jpg", ".jpeg", ".png" },
            };
            var file = await picker.PickSingleFileAsync();
            return file;
        }

        public static async Task<StorageFolder> OpenFolderPicker()
        {
            FolderPicker folderPicker = new FolderPicker();
            folderPicker.SuggestedStartLocation = PickerLocationId.ComputerFolder;
            folderPicker.FileTypeFilter.Add(".jpg");
            folderPicker.FileTypeFilter.Add(".png");
            folderPicker.FileTypeFilter.Add(".jpeg");
            folderPicker.FileTypeFilter.Add(".gif");

            StorageFolder folder = await folderPicker.PickSingleFolderAsync();

            return folder;
        }

        public static async Task<StorageFile> OpenSaveFilePicker()
        {
            FileSavePicker savePicker = new FileSavePicker();

            savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;

            // Dropdown of file types the user can save the file as
            savePicker.FileTypeChoices.Add("Plain Text", new List<string>() { ".txt" });
            // Default file name if the user does not type one in or select a file to replace
            savePicker.SuggestedFileName = "New Document";

            StorageFile file = await savePicker.PickSaveFileAsync();
            return file;
        }

        public static async Task<StorageFile> OperaCameraPhotoPicker()
        {
            try
            {
                CameraCaptureUI dialog = new CameraCaptureUI();
                Size aspectRatio = new Size(16, 9);
                dialog.PhotoSettings.CroppedAspectRatio = aspectRatio;

                StorageFile file = await dialog.CaptureFileAsync(CameraCaptureUIMode.Photo);
                return file;
            }
            catch
            {
                // ignored
            }
            return null;
        }
    }
}
