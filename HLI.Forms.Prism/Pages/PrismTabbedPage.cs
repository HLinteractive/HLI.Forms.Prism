// // --------------------------------------------------------------------------------------------------------------------
// // <copyright file="HLI.Forms.HliTabbedPage.cs" company="HL Interactive">
// //   Copyright © HL Interactive, Stockholm, Sweden, 2016
// // </copyright>
// // --------------------------------------------------------------------------------------------------------------------

using HLI.Forms.Core.Extensions;

using Prism.Mvvm;
using Xamarin.Forms;

namespace HLI.Forms.Prism.Pages
{
    /// <summary>
    ///     Automatically loads Resources from App, calls
    ///     <see cref="ViewModelLocator.SetAutowireViewModel" /> and allows you to set
    ///     <see cref="HasNavigationBar" />
    /// </summary>
    public class PrismTabbedPage : TabbedPage
    {
        #region Static Fields

        public static readonly BindableProperty HasNavigationBarProperty = BindableProperty.Create(
            nameof(HasNavigationBar),
            typeof(bool),
            typeof(PrismTabbedPage),
            true,
            propertyChanged: OnHasnavigationBarChanged);

        #endregion

        #region Constructors and Destructors

        // ReSharper disable once PublicConstructorInAbstractClass - public required by Prism
        public PrismTabbedPage()
        {
            this.LoadResourcesFromApp();
            ViewModelLocator.SetAutowireViewModel(this, true);
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Determines if navgation bar is show. Default is <c>True</c>.
        /// </summary>
        public bool HasNavigationBar
        {
            get => (bool)this.GetValue(HasNavigationBarProperty);

            set => this.SetValue(HasNavigationBarProperty, value);
        }

        #endregion
        
        #region Methods

        protected override void OnAppearing()
        {
            base.OnAppearing();
            this.SetNavigationBar();
        }
        
        private static void OnHasnavigationBarChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ((PrismTabbedPage)bindable).SetNavigationBar();
        }

        private void SetNavigationBar()
        {
            NavigationPage.SetHasNavigationBar(this, this.HasNavigationBar);
        }

        #endregion
    }
}