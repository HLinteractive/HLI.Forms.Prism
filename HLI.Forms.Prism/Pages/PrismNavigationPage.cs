// // --------------------------------------------------------------------------------------------------------------------
// // <copyright file="HLI.Forms.HliNavigationPage.cs" company="HL Interactive">
// //   Copyright © HL Interactive, Stockholm, Sweden, 2016
// // </copyright>
// // --------------------------------------------------------------------------------------------------------------------

using HLI.Forms.Core.Extensions;
using Prism.Mvvm;
using Xamarin.Forms;

namespace HLI.Forms.Prism.Pages
{
    /// <summary>
    ///     Automatically loads Resources from App and calls
    ///     <see cref="ViewModelLocator.SetAutowireViewModel" /> 
    /// </summary>
    public class PrismNavigationPage : NavigationPage
    {
        #region Constructors and Destructors

        // ReSharper disable once PublicConstructorInAbstractClass - public required by Prism
        public PrismNavigationPage()
        {
            this.LoadResourcesFromApp();
            ViewModelLocator.SetAutowireViewModel(this, true);
        }

        #endregion
        
        #region Methods

        protected override void OnAppearing()
        {
            base.OnAppearing();
            this.LoadResourcesFromApp();
        }
        
        #endregion

    }
}