// // --------------------------------------------------------------------------------------------------------------------
// // <copyright file="HLI.Forms.IHliViewModelBase.cs" company="HL Interactive">
// //   Copyright © HL Interactive, Stockholm, Sweden, 2016
// // </copyright>
// // --------------------------------------------------------------------------------------------------------------------

using System;
using System.Windows.Input;

using HLI.Forms.Prism.Models;

using Prism.Navigation;

namespace HLI.Forms.Prism.Interfaces
{
    /// <summary>
    ///     The HliViewModelBase interface.
    /// </summary>
    public interface IHliViewModelBase : IConfirmNavigation
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the add command.
        /// </summary>
        ICommand AddCommand { get; set; }

        /// <summary>
        ///     Gets or sets the copy command.
        /// </summary>
        ICommand CopyCommand { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether is busy.
        /// </summary>
        bool IsBusy { get; set; }

        /// <summary>
        ///     Gets or sets the is busy reason.
        /// </summary>
        string IsBusyReason { get; set; }

        /// <summary>
        ///     <para>Navigates using string command parameter such as a slash-separated Page path (FirstPage/SecondPage).</para>
        ///     <para>Also supports external targets such as URL and maps by using prefix http(s):, map:, mailto: or tel:</para>
        /// </summary>
        /// <example>
        ///     <para>
        ///         To navigate a page path
        ///         <code>
        ///     Command="NavigateCommand" CommandParameter="MyTabbedPage/MyDetailPage"
        /// </code>
        ///     </para>
        ///     <para>
        ///         To dial number, prefix with "tel:"
        ///         <code>
        ///     Command="NavigateCommand" CommandParameter="tel:64649XAMARIN"
        /// </code>
        ///     </para>
        ///     <para>
        ///         To open a website, prefix "http"
        ///         <code>
        ///     Command="NavigateCommand" CommandParameter="https://www.xamarin.com"
        /// </code>
        ///     </para>
        ///     <para>
        ///         To open an address in the local navigation app, prefix "map:"
        ///         <code>
        ///     Command="NavigateCommand" CommandParameter="map:34 Downing Street, London"
        /// </code>
        ///     </para>
        ///     <para>
        ///         To send a mail in the local app, prefix "mailto:"
        ///         <code>
        ///     Command="NavigateCommand" CommandParameter="mailto:support@xamarin.com"
        /// </code>
        ///     </para>
        /// </example>
        ICommand NavigateCommand { get; set; }

        /// <summary>
        ///     Gets or sets the navigation service.
        /// </summary>
        INavigationService NavigationService { get; set; }

        /// <summary>
        ///     ID used to refresh <see cref="HliViewModel{T}.Model" />
        /// </summary>
        Guid? QueryId { get; set; }

        /// <summary>
        ///     Gets or sets the refresh command.
        /// </summary>
        ICommand RefreshCommand { get; set; }

        /// <summary>
        ///     Gets or sets the remove command.
        /// </summary>
        ICommand RemoveCommand { get; set; }

        /// <summary>
        ///     Gets or sets the save command.
        /// </summary>
        ICommand SaveCommand { get; set; }

        /// <summary>
        ///     Gets or sets the title.
        /// </summary>
        string Title { get; set; }

        ICommand BackCommand { get; set; }

        /// <summary>
        ///     Gets or sets the cancel command.
        /// </summary>
        ICommand CancelCommand { get; set; }

        #endregion

        #region Public Methods and Operators
        
        /// <summary>
        ///     Occurs when <see cref="QueryId" /> is set to a new value
        /// </summary>
        void QueryIdChanged();

        /// <summary>
        ///     Refresh commands in the VM.
        /// </summary>
        void RefreshCommands();

        #endregion
    }
}