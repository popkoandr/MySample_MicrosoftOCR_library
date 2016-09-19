# MySample_MicrosoftOCR_library
testing ocr library

This is Win8 app, for testing how to use OCR library (Microsoft product). Unfortunaly Shared and Phone project in the solution still being "empty", so it can be using only at PC.
User can select image, folder with images or take a photo from camera that need to be recognized. There is ability to show "overlay" recognized text on image and also to do other operation with it (edit, copy, save to file).

Program has many problem with stability during execution. Almost all of the code is placed on the code-behind and classes intended for opening file/folder picker, loading files, making overlay. 
Main logic of work with library is placed on RecognizeEngine.cs.

Snapshots are on the root folder.  
