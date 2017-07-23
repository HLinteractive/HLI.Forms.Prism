// // --------------------------------------------------------------------------------------------------------------------
// // <copyright file="HLI.Forms.AppExtensions.cs" company="HL Interactive">
// //   Copyright © HL Interactive, Stockholm, Sweden, 2016
// // </copyright>
// // --------------------------------------------------------------------------------------------------------------------

using System;
using System.Reflection;
using HLI.Forms.Core.Services;
using Prism.Mvvm;

using Xamarin.Forms;

namespace HLI.Forms.Prism
{
    public static class AppExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Set naming policy for resolving ViewModels, based on Prism AutoWireupViewModel.
        /// </summary>
        /// <param name="app">this</param>
        /// <param name="pagesDirectory">Direcory Pages are located in</param>
        /// <param name="viewModelsDirectory">Directory view models are located in</param>
        /// <param name="pageSuffix">How pages are suffixed, ex "Page" for "MainPage"</param>
        /// <param name="viewModelSuffix">How view models are suffixed, ex "ViewModel" for "MainViewModel"</param>
        public static void SetViewModelNamingPolicy(
            this Application app,
            string pagesDirectory = "Pages",
            string viewModelsDirectory = "ViewModels",
            string pageSuffix = "Page",
            string viewModelSuffix = "ViewModel")
        {
            // Set ViewModel naming policy
            Type ViewTypeToViewModelTypeResolver(Type viewType)
            {
                try
                {
                    // Pages should be named "(something)Page" and placed in "Pages"
                    // View models should be named "(something)ViewModel" and placed in "ViewModels"
                    // For "MainPage" the view model should be "MainViewModel"
                    var viewName = viewType.FullName.Replace(pagesDirectory, viewModelsDirectory);
                    var viewAssemblyName = viewType.GetTypeInfo().Assembly.FullName;
                    var viewModelName = $"{viewName.Replace(pageSuffix, string.Empty)}{viewModelSuffix}, {viewAssemblyName}";
                    return Type.GetType(viewModelName);
                }
                catch (Exception ex)
                {
                    AppService.WriteDebug(ex);
                    throw;
                }
            }

            ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver(ViewTypeToViewModelTypeResolver);
        }
        
        #endregion
    }
}