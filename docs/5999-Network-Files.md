
* [Index](0000-Index.md)
* [Features implementation](5000-Features-implementation.md)




Network files
=================

This feature allows you to put random files in a network-specific folder.

A manifest must be created for security and configuration options.

* There is an information page at `/Files/NetworkFileInfo/filename`
* There is an download page at `/Files/NetworkFile/filename`
* If the document is made of HTML or markdown, it can be displayed within the website at `/Pages/NetworkFile/filename`
    * The manifest must specify `AllowWebDisplay: true`
    * The manifest must specify `MimeType: 'text/html'` or `MimeType: 'text/markdown'`
    * The file must be safe for web display.

