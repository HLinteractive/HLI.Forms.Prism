![HL Interactive](https://www.dropbox.com/s/fdyzvkso9zs9ndf/HLi.Signature.DVDs.jpg?dl=1)
> HL Interactive (HLi)

- [HLI.Forms.Prism](#hliformsprism)
  * [Usage](#usage)
    + [Extensions](#extensions)
      - [App](#app)
      - [ICommand](#icommand)
      - [Navigation](#navigation)
    + [Prism Page base classes](#prism-page-base-classes)
    + [ViewModel list/detail base classes](#viewmodel-list-detail-base-classes)
  * [Delivery & Deployment](#delivery---deployment)
  * [Dependencies](#dependencies)
  * [NuGet Package Generation](#nuget-package-generation)
  * [Solution File Structure](#solution-file-structure)
  * [Changes and backward compatibility](#changes-and-backward-compatibility)

# HLI.Forms.Prism #
Xamarin.Forms Prism Pages, MVVM View Model Base classes, services, extensions etc.

![Build Status VSTS](https://nodessoft.visualstudio.com/_apis/public/build/definitions/3ed91d4b-9b9f-4c69-b511-406908c52385/24/badge)

## Usage
### Extensions
#### App
You can set the Prism naming policy to determine what directory is used for each MVVM part. Each parameter is optional with the below default values.   
Example **`App.xaml.cs`** (inherits `PrismApplication`):

```csharp
    protected override void ConfigureViewModelLocator()
    {
        base.ConfigureViewModelLocator();
        this.SetViewModelNamingPolicy("MyPageFolder", "MyViewModelFolder", "PageSuffix", "ViewModelSuffix");
    }
```

#### ICommand
To easily update an `ICommand`s `CanExecute` there's an extension:

```csharp
	ICommand SaveCommand { get; set; }
	SaveCommand.RaiseCanExecute();
```

#### Navigation
To navigate Prism deep links you use a path such as "MyTabbedPage/MyNavigationPage/UserPage". Since these are page classes you can use the `PageNamesToUri` helper to get typed `Page` names instead of strings: 
	
```csharp
	await this.NavigationService.NavigateAsync(NavigationExtensions.PageNamesToUri(nameof(MyNavigationPage), nameof(MyTabbedPage), nameof(UserPage);
```

When recieving Prism `NavigationParameters` you want to type check these. To help with this `NavigationParameters` has extensions to get as a specific type:

```csharp
	Guid query = parameters.GetParameterAsGuid("Id");
```

Similarily there's `GetParameterAsInt` and `GetParameterAsType` where you supply an type argument:

```csharp
	var feedback = parameters.GetParameterAsType<HliFeedbackMessage>("Feedback");
```

### Prism Page base classes
Prism base classes that call `SetAutowireViewModel` automatically and load resources from your `App` class.

Import the namespace and use as base class for your pages:

```xaml
	<pages:HliContentPage
	xmlns:pages="clr-namespace:HLI.Forms.Prism.Pages;assembly=HLI.Forms.Prism"
	HasNavigationBar="True">
```

### ViewModel list/detail base classes
Working with MVVM there are lot of demands on your View Models; navigation, commands, busy indication etc. This library contains Prism based base classes that help set up a new project quickly; allowing list/detail View Model pattern using factories.

Say we want our app to start at the "login" Page, where the **Model** is "User".

1. Set up Prism ViewModelLocator using **[the extension](#app)**
2. Create "LoginPage" in CS or XAML (using **[the base class](#pages)**)
3. Create "LoginViewModel". Inherit the `HliViewModel` base class:

```csharp
	public class LoginViewModel : HliViewModel<User>
	{
		// TODO: Constructor
	}
```

4. Register the page for navigation in `App.xaml.cs` according to Prism documentation.  
Your ViewModel will now be registered with the Page and have a great deal of code for free.  
See `this.` in *LoginViewModel.cs* to auto complete all the methods and properties.  
Or check out the **[HliViewModel source code](HLI.Forms.Prism/Models/HliViewModel.cs "HliViewModel.cs")** to learn more.

For list / detail scenarios `HliListViewModel` is used as the "master" View Model base class and each "item" child will use `HliViewmodel` as above. In this case you need to implement `IViewModelFactory` and register that class with the DI so the list View Model can find it.

## Delivery & Deployment
Download the nuget package through Package Manager Console:

> install-package HLI.Forms.Prism

## Dependencies
* **Projects**
* **Packages**
	* HLI.Core
	* HLI.Forms.Core
	* HLI.Globalization
	* Prism.Forms
	* Xamarin.Forms

## NuGet Package Generation
The project is configured to automatically generate a ***.nupkg** upon build with **`dotnet cli`**.

## Solution File Structure

* **HLI.Forms.Prism** - solution root folder
	* **HLI.Forms.Prism**  - main project
		* **Events** - Prism `PubSubEvent` classes
		* **Extensions** - see above
		* **Interfaces**
		* **Models** - ViewModel base classes (above)
		* **Pages** - Xamarin.Forms `Page` base classes (above)

## Changes and backward compatibility
* VS2017 CsProj based package generation to netstandard 1.4
