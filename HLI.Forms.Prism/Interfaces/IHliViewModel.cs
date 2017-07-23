// // --------------------------------------------------------------------------------------------------------------------
// // <copyright file="HLI.Forms.IHliViewModel.cs" company="HL Interactive">
// //   Copyright © HL Interactive, Stockholm, Sweden, 2016
// // </copyright>
// // --------------------------------------------------------------------------------------------------------------------

using System.Windows.Input;

using HLI.Core.Interfaces.Models;

namespace HLI.Forms.Prism.Interfaces
{
    /// <summary>
    ///     Base interface for HLI ViewModels.
    /// </summary>
    public interface IHliViewModel
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
        ///     Gets or sets the model.
        /// </summary>
        IModelBase Model { get; set; }

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
        
        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The can add.
        /// </summary>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        bool CanAdd();

        /// <summary>
        ///     The can cancel.
        /// </summary>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        bool CanCancel();

        /// <summary>
        ///     The can copy.
        /// </summary>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        bool CanCopy();

        /// <summary>
        ///     The can edit.
        /// </summary>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        bool CanEdit();

        /// <summary>
        ///     The can refresh.
        /// </summary>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        bool CanRefresh();

        /// <summary>
        ///     The can remove.
        /// </summary>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        bool CanRemove();

        /// <summary>
        ///     The can save.
        /// </summary>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        bool CanSave();

        /// <summary>
        ///     Raises the OnAdded event
        /// </summary>
        void OnAdd();

        /// <summary>
        ///     Occurs when editing is cancelled
        /// </summary>
        void OnCancel();

        /// <summary>
        ///     Raises the OnAdded event
        /// </summary>
        void OnCopy();

        /// <summary>
        ///     Begins editing the current item by activating change tracking
        /// </summary>
        void OnEdit();

        /// <summary>
        ///     Sets IsBusy to true and IsBusyReason to "loading data"
        /// </summary>
        void OnRefresh();

        /// <summary>
        ///     Occurs when an item is removed
        /// </summary>
        void OnRemove();

        /// <summary>
        ///     Occurs when the current item is saved
        /// </summary>
        void OnSave();

        /// <summary>
        ///     The refresh commands.
        /// </summary>
        void RefreshCommands();

        #endregion
    }
}