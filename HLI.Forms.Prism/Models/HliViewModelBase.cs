// // --------------------------------------------------------------------------------------------------------------------
// // <copyright file="HLI.Forms.Prism.HliViewModelBase.cs" company="HL Interactive">
// //   Copyright © HL Interactive, Stockholm, Sweden, 2017
// // </copyright>
// // --------------------------------------------------------------------------------------------------------------------

using System;
using System.Net;
using System.Windows.Input;

using HLI.Forms.Core;
using HLI.Forms.Core.Models;
using HLI.Forms.Core.Services;
using HLI.Forms.Prism.Extensions;
using HLI.Forms.Prism.Interfaces;

using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;

using Xamarin.Forms;

namespace HLI.Forms.Prism.Models
{
    /// <summary>
    ///     Base class for HLI View Models.
    /// </summary>
    public abstract class HliViewModelBase : BindableBase, IHliViewModelBase
    {
        #region Fields

        /// <summary>
        ///     See <see cref="IsBusy" />
        /// </summary>
        private bool isBusy;

        /// <summary>
        ///     See <see cref="IsBusyReason" />
        /// </summary>
        private string isBusyReason;

        /// <summary>
        ///     Prism navigation service
        /// </summary>
        private INavigationService navigationService;

        /// <summary>
        ///     Query id used to refresh.
        /// </summary>
        private Guid? queryId;

        /// <summary>
        ///     Optional title.
        /// </summary>
        private string title;

        #endregion

        #region Constructors and Destructors

        protected HliViewModelBase(INavigationService navigationService)
        {
            this.navigationService = navigationService;
            this.NavigateCommand = new DelegateCommand<string>(this.OnNavigateCommand, this.CanNavigateCommand);
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the add command.
        /// </summary>
        public ICommand AddCommand { get; set; }

        public ICommand BackCommand { get; set; }

        /// <summary>
        ///     Gets or sets the cancel command.
        /// </summary>
        public ICommand CancelCommand { get; set; }

        /// <summary>
        ///     Gets or sets the copy command.
        /// </summary>
        public ICommand CopyCommand { get; set; }

        /// <summary>
        ///     Determines if the ViewModel is busy loading data
        /// </summary>
        public bool IsBusy
        {
            get => this.isBusy;

            set => this.SetProperty(ref this.isBusy, value);
        }

        /// <summary>
        ///     Busy reason to show during async operations
        /// </summary>
        public string IsBusyReason
        {
            get => this.isBusyReason;

            set => this.SetProperty(ref this.isBusyReason, value);
        }

        /// <summary>
        ///     Indicates if the current device is anything else than <see cref="IsDevicePhone">phone</see>; i.e. tablet or
        ///     desktop.
        /// </summary>
        public bool IsDeviceNonPhone => Device.Idiom != TargetIdiom.Phone;

        public bool IsDeviceNonWindows => Device.RuntimePlatform != Device.Windows;

        /// <summary>
        ///     Indicates if the current device is a phone rather than tablet
        /// </summary>
        public bool IsDevicePhone => Device.Idiom == TargetIdiom.Phone;

        /// <summary>
        ///     Indicates if the current device is running Windows
        /// </summary>
        public bool IsDeviceWindows => Device.RuntimePlatform == Device.Windows;

        /// <summary>
        ///     Accessor for <see cref="Application.MainPage" />, if available
        /// </summary>
        public Page MainPage => Application.Current != null ? Application.Current.MainPage : null;

        /// <summary>
        ///     Navigates to the view supplied as parameter
        /// </summary>
        public ICommand NavigateCommand { get; set; }

        /// <summary>
        ///     Gets or sets the navigation service.
        /// </summary>
        public INavigationService NavigationService
        {
            get => this.navigationService;

            set => this.SetProperty(ref this.navigationService, value);
        }

        /// <summary>
        ///     ID used to refresh <see cref="HliViewModel{T}.Model" />
        /// </summary>
        public Guid? QueryId
        {
            get => this.queryId;

            set
            {
                if (this.SetProperty(ref this.queryId, value))
                {
                    this.QueryIdChanged();
                }
            }
        }

        /// <summary>
        ///     Gets or sets the refresh command.
        /// </summary>
        public ICommand RefreshCommand { get; set; }

        /// <summary>
        ///     Gets or sets the remove command.
        /// </summary>
        public ICommand RemoveCommand { get; set; }

        /// <summary>
        ///     Gets or sets the save command.
        /// </summary>
        public ICommand SaveCommand { get; set; }

        /// <summary>
        ///     Gets or sets the title.
        /// </summary>
        public string Title
        {
            get => this.title;

            set => this.SetProperty(ref this.title, value);
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Determines if Prism can navigate. Default <c>True</c>.
        /// </summary>
        /// <param name="parameters">Params</param>
        /// <returns><c>True</c> unless overridden</returns>
        public virtual bool CanNavigate(NavigationParameters parameters)
        {
            return true;
        }

        /// <summary>
        ///     Occurs when the VM is navigated from. Has no default implementation
        /// </summary>
        /// <param name="parameters"></param>
        public virtual void OnNavigatedFrom(NavigationParameters parameters)
        {
        }

        /// <summary>
        ///     Occurs when the VM is navigated to. Tries to set <see cref="QueryId" /> and calls <see cref="RefreshCommand" />
        /// </summary>
        /// <param name="parameters"></param>
        public virtual void OnNavigatedTo(NavigationParameters parameters)
        {
            var query = parameters.GetParameterAsGuid(Constants.NavigationParameterKeys.Id);
            if (query != null)
            {
                this.QueryId = query;
            }

            if (this.RefreshCommand != null && this.RefreshCommand.CanExecute(null))
            {
                this.RefreshCommand.Execute(null);
            }
        }

        /// <summary>
        ///     The query id changed.
        /// </summary>
        public virtual void QueryIdChanged()
        {
        }

        /// <summary>
        ///     Refreshes all commands in the VM
        /// </summary>
        public virtual void RefreshCommands()
        {
            this.RefreshCommand?.RaiseCanExecute();
            this.SaveCommand?.RaiseCanExecute();
            this.RemoveCommand?.RaiseCanExecute();
            this.AddCommand?.RaiseCanExecute();
            this.CopyCommand?.RaiseCanExecute();
            this.CancelCommand?.RaiseCanExecute();
            this.NavigateCommand?.RaiseCanExecute();
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Determines if <see cref="NavigateCommand" /> can execute
        /// </summary>
        /// <param name="target">Target uri</param>
        /// <returns><c>True</c> if can execute</returns>
        private bool CanNavigateCommand(string target)
        {
            return !string.IsNullOrWhiteSpace(target);
        }

        /// <summary>
        ///     Occurs when <see cref="NavigateCommand" /> executes
        /// </summary>
        /// <param name="target">Target uri</param>
        private async void OnNavigateCommand(string target)
        {
            if (this.NavigateCommand.CanExecute(target) == false || this.NavigationService == null)
            {
                MessagingCenter.Send(
                    new HliFeedbackMessage(HliFeedbackMessage.FeedbackType.Debug, "No navigation service"),
                    Constants.FeedbackKeys.Message);
                AppService.WriteDebug("No navigation service");
                return;
            }

            // Check for external targets
            var prefixIndex = target.IndexOf(":", StringComparison.Ordinal);

            // Prism navigation target (page)
            if (prefixIndex == -1)
            {
                await this.NavigationService.NavigateAsync(target);
                return;
            }

            var prefix = target.Substring(0, prefixIndex);
            switch (prefix)
            {
                case "map:":
                    var address = target.Substring(prefixIndex + 1);
                    switch (Device.RuntimePlatform)
                    {
                        case Device.iOS:
                            Device.OpenUri(new Uri($"http://maps.apple.com/?q={WebUtility.UrlEncode(address)}"));
                            break;
                        case Device.Android:
                            Device.OpenUri(new Uri($"geo:0,0?q={WebUtility.UrlEncode(address)}"));
                            break;
                        case Device.WinPhone:
                        case Device.Windows:
                            Device.OpenUri(new Uri($"bingmaps:?where={Uri.EscapeDataString(address)}"));
                            break;
                        default: throw new ArgumentOutOfRangeException();
                    }

                    break;
                default:
                    // "http:", "mailto:", "tel:" are native prefixes
                    Device.OpenUri(new Uri(target));
                    break;
            }
        }

        #endregion
    }
}