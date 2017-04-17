
* [Index](0000-Index.md)
* [Customization index](4000-Customization-index.md)


Theming
=========================================

You will obviously want to change the visual elements of your networks. This page will describe how theming works and the various things you can do to change the visuals.

Theming support
-------------------

Some code is on the way to make theming centralized.

The following groups can be changed:

* the views (the HTML part)
* the style (the CSS part)

After the necessary changes in the code, the `Themes` folder will be the central point to alter all the groups.

Note for later: the theme folders might even inherit in some way.

Changing the views
-------------------

The `Views` folder contains many .cshtml file that hold the HTML structure of all pages. Do not change these views directly, copy them into your theme!

### How to find the view associated to a page?

The website is using the default pattern  for ASP MVC projects. The website is composed of Controllers and Actions.

When you visit the `/Events` page, you are targeting the view for the `Events` controller and the `Index` view.  
When you visit the `/Events/New` page, you are targeting the view for the `Events` controller and the `New` view.  
When you visit the `/Event/123` page, you are targeting the view for the `Events` controller and the `Event` view.  

The `Views` folder contains one subfolder for each controller. The views are located in those subfolders and are named by the name of the action. There are exceptions and special views.

Here is a partial list of the views referred by URL.


    / (not authenticated)                   Redirects to /Welcome
    / (authenticated)                       /Views/Home/News.cshtml
    /Welcome                                /Views/Home/Welcome.cshtml
    /Peoples                                /Views/Peoples/Index.cshtml
    /Person/<name>                          /Views/Peoples/Person.cshtml
    /Account                                /Views/Account/Index.cshtml
    /Account/Settings                       /Views/Account/Settings.cshtml
    /Account/Region                         /Views/Account/Region.cshtml
    /Account/Profile                        /Views/Account/Profile.cshtml
    /Companies                              /Views/Companies/Index.cshtml
    /Company/<name>                         /Views/Companies/Company.cshtml
    /favicon.ico                            See DataController.Favicon
    /robots.txt                             See DataController.Robots
    /Conversations                          /Views/Messages/Index.cshtml

When you don't find the view file you are searching for, you may look at the code:

* In the `Global.asax.cs` file, the `RegisterMainRoutes` method contains the ASP MVC routes. A route maps a URL to a controller and an action.
* Find the controller class in the `Controllers` folder that seem to match the first segment of the url. Then find the method that seem to match the second segment of the url. Read the code and discover the view that is coded to be displayed.

### The `Themes` folder

Now you want to change some views. You need to create a folder that will hold a copy of the views you want to change. 

Let's say you want to change the home page (/Views/Home/Welcome.cshtml). 

* Create the `Themes` folder if it does not exist
* Create the `Themes/MyTheme` folder. Use the name you want. Avoid special characters.
* Create the `Themes/MyTheme/Home` folder.  It will hold the customized views for the Home controller.
* Copy `Views/Home/Welcome.cshtml` in the `Themes/MyTheme/Home` folder. Change it.
* Configure the network to use the theme you created. Ensure the web app has restarted. 
* You should see your view rendering on the welcome page.


Changing network-related resources
-------------------

The following items depend on the network (not on a theme).

* The network logos
    * The web logo
    * The email logo
    * The favicon
* The stylesheet
* The welcome background picture
* The localization files
* some other files (to be listed here)

Those resources are loaded from various folders and depend on the *network name* or the *instance name*.

### Main stylesheet

Start by creating `~/Content/Networks/<NetworkName>/Styles/Colors.less` empty. You will be able to alter predefined colors by setting them in this file. 

Now create the main stylesheet at `~/Content/Networks/<NetworkName>/Styles/Site.less`. Put the minimum content:


    @import "Colors.less";
    @import "../../../Site.less";
    
    // write more CSS here

### Site logo, pictures...


In `~/Content/Networks/<NetworkName>/Styles/` you MUST put those files:

* `HeaderLogo.png` is the picture you see in the site header
    * Width:  275
    * Height:  78-100
    * Type: PNG
* `Logo.png`
    * Width:  100-160
    * Height: 100-180
* `MailLogo.gif` will be displayed in the emails
    * Width:  316
    * Height: 88
    * Type: JPG/PNG/GIF
* `favicon.ico`
    * Anything compatible with the favicon specification
* `Hero.jpg`
    * Width:  >= 1920
    * Height:  >= 700
    * Type: JPG

In `~/Content/Networks/<NetworkName>/Documents/` you MAY put those files:

* `robots.txt`
* `Faq.md`
* `Terms.md`


### Localizations

Any text that is displayed can be localized by language and by network.

The base files are in the `Lang/Common` folder. Don't put customizations in these files.

Create the `Lang/Networks/<universe-name>.po` file (empty) to overwrite or add localizations. 



