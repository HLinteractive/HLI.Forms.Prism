// // --------------------------------------------------------------------------------------------------------------------
// // <copyright file="HLI.Forms.HliViewModel.cs" company="HL Interactive">
// //   Copyright © HL Interactive, Stockholm, Sweden, 2016
// // </copyright>
// // --------------------------------------------------------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;

using HLI.Core.Extensions;
using HLI.Core.Interfaces.Models;
using HLI.Core.Models;
using HLI.Forms.Core.Services;
using HLI.Forms.Prism.Events;
using HLI.Forms.Prism.Extensions;
using HLI.Forms.Prism.Interfaces;
using HLI.Globalization.Dictionaries;

using Prism.Commands;
using Prism.Events;
using Prism.Navigation;

namespace HLI.Forms.Prism.Models
{
    /// <summary>
    ///     Provides logic for editing a single entity
    /// </summary>
    /// <typeparam name="T">
    ///     Type of the business object
    /// </typeparam>
    public abstract class HliViewModel<T> : HliViewModelBase, IHliViewModel
        where T : class, IModelBase, new()
    {
        #region Fields

        private readonly IEventAggregator eventAggregator;

        /// <summary>
        ///     The model.
        /// </summary>
        private IModelBase model;

        #endregion

        #region Constructors and Destructors

        #region Constructors

        /// <summary>
        ///     Constructs with <paramref name="eventAggregator" /> required by <see cref="EditCommand" />
        /// </summary>
        /// <param name="eventAggregator">Required by <see cref="EditCommand" /></param>
        /// <param name="navigationService">Nav service</param>
        protected HliViewModel(IEventAggregator eventAggregator, INavigationService navigationService)
            : base(navigationService)
        {
            this.eventAggregator = eventAggregator;
            this.InitCommands();
        }

        #endregion

        #endregion

        #region Public Properties

        /// <summary>
        ///     Begins editing the current item
        /// </summary>
        public ICommand EditCommand { get; set; }

        /// <summary>
        ///     Gets or sets the model.
        /// </summary>
        public IModelBase Model
        {
            get => this.model;

            set
            {
                if (this.model != null)
                {
                    this.model.PropertyChanged -= this.ModelOnPropertyChanged;
                }

                if (this.SetProperty(ref this.model, value) && this.model != null)
                {
                    this.OnNewModel();
                }

                if (this.model != null)
                {
                    this.model.PropertyChanged += this.ModelOnPropertyChanged;
                }
            }
        }

        /// <summary>
        ///     Returns <see cref="Model" /> as <see cref="T" />
        /// </summary>
        public T ModelAsType => this.Model as T;

        #endregion

        #region Properties

        /// <summary>
        ///     Method used for deleting a <see cref="T" /> from database
        /// </summary>
        protected Action<T> DeleteMethod { get; set; }

        /// <summary>
        ///     Method used for getting an individual <see cref="T" /> from database
        /// </summary>
        protected Func<Guid, T> GetMethod { get; set; }

        /// <summary>
        ///     Method used for saving a <see cref="T" /> from database
        /// </summary>
        protected Func<T, T> SaveMethod { get; set; }

        /// <summary>
        ///     Gets the model as model base.
        /// </summary>
        private ModelBase ModelAsModelBase => this.model as ModelBase;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The can add.
        /// </summary>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public virtual bool CanAdd()
        {
            return this.Model == null;
        }

        /// <summary>
        ///     The can cancel.
        /// </summary>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public virtual bool CanCancel()
        {
            return this.CanSave();
        }

        /// <summary>
        ///     The can copy.
        /// </summary>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public virtual bool CanCopy()
        {
            return this.Model != null;
        }

        /// <summary>
        ///     The can edit.
        /// </summary>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public virtual bool CanEdit()
        {
            return this.Model != null && this.Model.IsChanged == false;
        }

        /// <summary>
        ///     The can refresh.
        /// </summary>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public virtual bool CanRefresh()
        {
            return (this.Model == null || this.Model.IsChanged == false) && this.QueryId.HasValue;
        }

        /// <summary>
        ///     The can remove.
        /// </summary>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public virtual bool CanRemove()
        {
            return this.Model != null;
        }

        /// <summary>
        ///     The can save.
        /// </summary>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public virtual bool CanSave()
        {
            return this.Model != null && this.Model.IsChanged;
        }

        /// <summary>
        ///     Raises the OnAdded event
        /// </summary>
        public virtual void OnAdd()
        {
            this.Model = new T();
            this.RefreshCommands();
        }

        /// <summary>
        ///     Occurs when editing is cancelled
        /// </summary>
        public virtual void OnCancel()
        {
            if (this.CanCancel() == false)
            {
                return;
            }

            if (this.Model == null)
            {
                return;
            }

            try
            {
                // TODO: Fails for TimeLog w Geolocation?
                this.Model.CancelEdit();
            }
            catch (Exception ex)
            {
                AppService.WriteDebug(ex);
            }

            if (this.ModelAsModelBase != null)
            {
                this.ModelAsModelBase.IsChanged = false;
            }

            this.RefreshCommands();
        }

        /// <summary>
        ///     Raises the OnAdded event
        /// </summary>
        public virtual void OnCopy()
        {
            if (this.CanCopy() == false)
            {
                return;
            }

            this.Model = this.Model.DeepClone() as T;
            if (this.Model == null)
            {
                return;
            }

            this.Model.Id = Guid.NewGuid();
            var audited = this.Model as AuditedObject;
            if (audited != null)
            {
                audited.Version = 0;
            }

            this.RefreshCommands();
        }

        public virtual void OnEdit()
        {
            if (this.Model == null)
            {
                return;
            }

            var serializable = this.Model as ISerializableModel;
            if (serializable != null)
            {
                serializable.IsSerializing = true;
            }

            try
            {
                this.Model.BeginEdit();
            }
            catch (Exception ex)
            {
                AppService.WriteDebug(ex);
            }

            if (serializable != null)
            {
                serializable.IsSerializing = false;
            }

            if (this.ModelAsModelBase != null)
            {
                this.ModelAsModelBase.IsChanged = true;
            }

            this.RefreshCommands();
        }

        /// <summary>
        ///     Sets IsBusy to true and IsBusyReason to "loading data"
        /// </summary>
        public virtual void OnRefresh()
        {
            if (this.CanRefresh() == false)
            {
                return;
            }

            this.IsBusy = true;
            
            this.IsBusyReason = HliDictionary.Loading;
            if (this.GetMethod != null && this.QueryId.HasValue)
            {
                this.Model = this.GetMethod(this.QueryId.Value);
            }

            this.RefreshCommands();
        }

        /// <summary>
        ///     Occurs when an item is removed
        /// </summary>
        public virtual async void OnRemove()
        {
            if (this.CanRemove() == false)
            {
                return;
            }

            var result = await this.MainPage.DisplayAlert(string.Empty, $"{HliDictionary.Delete}?", HliDictionary.Ok, HliDictionary.Cancel);
            if (result != true)
            {
                return;
            }

            if (this.ModelAsModelBase != null)
            {
                this.ModelAsModelBase.IsChanged = false;
            }
            
            this.DeleteMethod?.Invoke(this.ModelAsType);
            this.RefreshCommands();
            this.eventAggregator.GetEvent<DeletedEvent>().Publish(this.ModelAsModelBase);
        }

        /// <summary>
        ///     Occurs when the current item is saved
        /// </summary>
        public virtual void OnSave()
        {
            if (this.CanSave() == false)
            {
                return;
            }

            if (this.Model != null)
            {
                this.Model.EndEdit();
                if (this.ModelAsModelBase != null)
                {
                    this.ModelAsModelBase.IsChanged = false;
                }
            }

            this.Model = this.SaveMethod?.Invoke(this.ModelAsType);

            this.RefreshCommands();
        }

        #region Overrides of Object

        public override string ToString()
        {
            // To string returns the model representation by default
            return this.Model?.ToString() ?? base.ToString();
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        ///     Occurs when a property in the <see cref="Model" /> changes
        /// </summary>
        /// <param name="sender">this</param>
        /// <param name="args">args</param>
        protected virtual void ModelOnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            this.RefreshCommands();
        }

        /// <summary>
        ///     Navigates backward if possible
        /// </summary>
        /// <returns>See <see cref="INavigationService.GoBackAsync" /></returns>
        protected virtual Task<bool> OnBack()
        {
            return this.NavigationService.GoBackAsync();
        }

        /// <summary>
        ///     Occurs when a new <see cref="Model" /> is set to non default value
        /// </summary>
        protected virtual void OnNewModel()
        {
            try
            {
                // This will fail if model can't be serialized
                this.OnEdit();
            }
            catch (Exception ex)
            {
                AppService.WriteDebug(ex);
            }

            this.RefreshCommands();
        }

        #endregion

        #region Other

        /// <summary>
        ///     Setup ICommand's to functions
        /// </summary>
        private void InitCommands()
        {
            this.AddCommand = new DelegateCommand(this.OnAdd, this.CanAdd);
            this.RemoveCommand = new DelegateCommand(this.OnRemove, this.CanRemove);
            this.BackCommand = new DelegateCommand(() => this.OnBack());
            this.CopyCommand = new DelegateCommand(this.OnCopy, this.CanCopy);
            this.RefreshCommand = new DelegateCommand(this.OnRefresh, this.CanRefresh);
            this.SaveCommand = new DelegateCommand(this.OnSave, this.CanSave);
            this.CancelCommand = new DelegateCommand(this.OnCancel, this.CanCancel);
            this.EditCommand = new DelegateCommand(this.OnEditing);
            this.RefreshCommands();
        }

        protected virtual void OnEditing()
        {
            if (this.Model.IsChanged)
            {
                // Already editing
                return;
            }

            this.OnEdit();
            var editEvent = this.eventAggregator.GetEvent<EditEvent>();
            editEvent.Publish(this.Model);
        }

        #region Overrides of HliViewModelBase

        public override void RefreshCommands()
        {
            base.RefreshCommands();
            this.EditCommand?.RaiseCanExecute();
        }

        #endregion

        #endregion
    }
}