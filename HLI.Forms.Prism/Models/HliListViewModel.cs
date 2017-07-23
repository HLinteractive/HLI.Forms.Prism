// // --------------------------------------------------------------------------------------------------------------------
// // <copyright file="HLI.Forms.HliListViewModel.cs" company="HL Interactive">
// //   Copyright © HL Interactive, Stockholm, Sweden, 2016
// // </copyright>
// // --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

using HLI.Core.Extensions;
using HLI.Core.Interfaces.Models;
using HLI.Core.Models;
using HLI.Forms.Core.Models;
using HLI.Forms.Prism.Events;
using HLI.Forms.Prism.Interfaces;
using HLI.Globalization.Dictionaries;

using Prism.Commands;
using Prism.Events;
using Prism.Navigation;

namespace HLI.Forms.Prism.Models
{
    //// ReSharper disable CompareNonConstrainedGenericWithNull

    /// <summary>
    ///     Base class for HLI list view models.
    /// </summary>
    /// <typeparam name="T">
    ///     Type of item in the <see cref="Models" /> collection
    /// </typeparam>
    public abstract class HliListViewModel<T> : HliViewModelBase
        where T : ModelBase, new()
    {
        #region Fields

        /// <summary>
        ///     The factory.
        /// </summary>
        private readonly IViewModelFactory<T> factory;

        private string filterText;

        private bool isEditing;

        /// <summary>
        ///     The models.
        /// </summary>
        private ObservableCollection<T> models = new ObservableCollection<T>();

        /// <summary>
        ///     The selected item.
        /// </summary>
        private IHliViewModel selectedItem;

        /// <summary>
        ///     The view.
        /// </summary>
        private HliCollectionViewSource<T> view;

        /// <summary>
        ///     The view models.
        /// </summary>
        private ObservableCollection<IHliViewModel> viewModels;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Causes the view to filter its items
        /// </summary>
        public ICommand FilterCommand { get; set; }

        /// <summary>
        ///     Text to filter on when <see cref="FilterCommand" /> is executed
        /// </summary>
        public string FilterText
        {
            get => this.filterText;

            set => this.SetProperty(ref this.filterText, value);
        }

        /// <summary>
        ///     Gets a value indicating if the viewmodel is currently editing an item
        /// </summary>
        public bool IsEditing
        {
            get => this.isEditing;

            set => this.SetProperty(ref this.isEditing, value);
        }

        /// <summary>
        ///     Gets the models.
        /// </summary>
        public ObservableCollection<T> Models
        {
            get => this.models;

            private set
            {
                this.ViewModels = new ObservableCollection<IHliViewModel>();

                if (!this.SetProperty(ref this.models, value) || this.Models == null)
                {
                    return;
                }

                // Models was set to non-NULL value
                this.OnNewModels();
            }
        }

        /// <summary>
        ///     Gets or sets the selected item.
        /// </summary>
        public IHliViewModel SelectedItem
        {
            get => this.selectedItem;

            set
            {
                if (!this.SetProperty(ref this.selectedItem, value) || value == null)
                {
                    return;
                }

                this.OnSelectedItemChanged();
            }
        }

        /// <summary>
        ///     Gets the view.
        /// </summary>
        public HliCollectionViewSource<T> View
        {
            get => this.view;

            private set => this.SetProperty(ref this.view, value);
        }

        /// <summary>
        ///     A collection of <see cref="IHliViewModel" /> created from <see cref="Models" />
        /// </summary>
        public ObservableCollection<IHliViewModel> ViewModels
        {
            get => this.viewModels;

            set
            {
                if (this.viewModels != null)
                {
                    this.viewModels.CollectionChanged -= this.ViewModelsOnCollectionChanged;
                }

                if (!this.SetProperty(ref this.viewModels, value) || value == null)
                {
                    return;
                }

                this.viewModels.CollectionChanged += this.ViewModelsOnCollectionChanged;
                this.ViewModelsOnCollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, this.viewModels));
            }
        }

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
            return this.Models != null && (this.SelectedItem == null || this.SelectedItem.Model.IsChanged == false);
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
            return this.SelectedItem != null;
        }

        /// <summary>
        ///     The can edit.
        /// </summary>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public virtual bool CanEdit()
        {
            return this.SelectedItem != null && this.SelectedItem.Model.IsChanged == false;
        }

        /// <summary>
        ///     The can refresh.
        /// </summary>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public virtual bool CanRefresh()
        {
            return this.SelectedItem == null || this.SelectedItem.Model.IsChanged == false;
        }

        /// <summary>
        ///     The can remove.
        /// </summary>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public virtual bool CanRemove()
        {
            return this.CanEdit();
        }

        /// <summary>
        ///     The can save.
        /// </summary>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public virtual bool CanSave()
        {
            return this.SelectedItem != null && this.SelectedItem.Model.IsChanged;
        }

        /// <summary>
        ///     Ensures <see cref="Models" /> are synced with <see cref="ViewModels" />
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="args">
        ///     The args.
        /// </param>
        public virtual void ModelOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            if (args.Action == NotifyCollectionChangedAction.Add && args.NewItems != null)
            {
                foreach (var newItem in args.NewItems.OfType<T>())
                {
                    this.SyncModelWithViewModels(newItem, args.Action);
                }
            }

            if (args.Action == NotifyCollectionChangedAction.Remove && args.OldItems != null)
            {
                foreach (var oldItem in args.OldItems.OfType<T>())
                {
                    this.SyncModelWithViewModels(oldItem, args.Action);
                }
            }
        }

        /// <summary>
        ///     Raises the OnAdded event
        /// </summary>
        public virtual void OnAdd()
        {
            var newItem = this.AddMethod != null ? this.AddMethod() : new T();

            this.Models.Add(newItem);
            var newVm = this.ViewModels.FirstOrDefault(vm => vm.Model.Equals(newItem));

            // This will accept changes and begin edit
            this.SelectedItem = newVm;

            this.SelectedItem.Model.Created = DateTime.Now;
            this.SelectedItem.Model.Updated = DateTime.Now;

            this.RefreshCommands();
        }

        /// <summary>
        ///     Occurs when editing is cancelled
        /// </summary>
        public virtual void OnCancel()
        {
            if (this.SelectedItem != null)
            {
                if (this.SelectedItem.CanCancel())
                {
                    this.SelectedItem.OnCancel();
                }
                else
                {
                    // Fallback cancel
                    this.SelectedItem.Model.CancelEdit();
                }

                if (this.SelectedItem.Model.IsPersisted == false)
                {
                    this.Models.Remove(this.SelectedItem.Model as T);
                    this.SelectedItem = null;
                }
            }

            this.RefreshCommands();
        }

        /// <summary>
        ///     Raises the OnAdded event
        /// </summary>
        public virtual void OnCopy()
        {
            var copy = this.SelectedItem.Model.DeepClone() as T;

            // ReSharper disable once PossibleNullReferenceException - Exception expected if null
            copy.Id = Guid.NewGuid();
            copy.Version = 0;
            this.Models.Add(copy);
            this.SelectedItem = this.viewModels.FirstOrDefault(vm => vm.Model.Equals(copy));
            this.RefreshCommands();
        }

        /// <summary>
        ///     The on edit.
        /// </summary>
        public virtual void OnEdit()
        {
            if (this.SelectedItem == null)
            {
                return;
            }

            this.SelectedItem.OnEdit();
            this.RefreshCommands();
        }

        /// <summary>
        ///     Sets IsBusy to true and IsBusyReason to "loading data"
        /// </summary>
        public virtual void OnRefresh()
        {
            if (this.RefreshCommand.CanExecute(null) == false)
            {
                return;
            }

            if (this.RefreshMethod != null)
            {
                this.IsBusy = true;
                this.IsBusyReason = HliDictionary.Loading;

                //this.Models = null;
                var newModels = this.RefreshMethod();

                this.Models = new ObservableCollection<T>(newModels);

                this.IsBusy = false;
                this.IsBusyReason = null;
            }

            this.RefreshCommands();
        }

        /// <summary>
        ///     Occurs when an item is removed
        /// </summary>
        public virtual async void OnRemove()
        {
            if (this.RemoveCommand.CanExecute(null) == false)
            {
                return;
            }

            var result = await this.MainPage.DisplayAlert(string.Empty, $"{HliDictionary.Delete}?", HliDictionary.Ok, HliDictionary.Cancel);
            if (result != true)
            {
                return;
            }

            this.DeleteMethod?.Invoke(this.SelectedItem.Model as T);

            this.Models.Remove(this.SelectedItem.Model as T);
            this.SelectedItem = null;

            this.RefreshCommands();
        }

        /// <summary>
        ///     Occurs when the current item is saved
        /// </summary>
        public virtual void OnSave()
        {
            // Use the single viewmodel's save
            if (this.SelectedItem.SaveCommand.CanExecute(null))
            {
                this.SelectedItem.SaveCommand.Execute(null);
            }
            else if (this.SaveCommand.CanExecute(null))
            {
                // Fallback save
                var itemBefore = this.SelectedItem.Model;
                itemBefore?.EndEdit();

                var itemAfter = this.SaveMethod?.Invoke(itemBefore as T);
                if (itemAfter == null)
                {
                    return;
                }

                this.SelectedItem.Model = itemAfter;
            }
        }

        /// <summary>
        ///     The query id changed.
        /// </summary>
        public override void QueryIdChanged()
        {
            base.QueryIdChanged();
            if (this.GetMethod != null && this.QueryId.HasValue && this.QueryId != Guid.Empty && this.SelectedItem != null
                && this.SelectedItem.Model.IsPersisted)
            {
                var model = this.GetMethod(this.QueryId.Value);
                if (model == null)
                {
                    return;
                }

                var match = this.viewModels.FirstOrDefault(vm => vm.Model.Id == model.Id);
                if (match != null)
                {
                    match.Model = model;
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Navigates backward if possible
        /// </summary>
        /// <returns>See <see cref="INavigationService.GoBackAsync" /></returns>
        protected virtual Task<bool> OnBack()
        {
            return this.NavigationService.GoBackAsync();
        }

        /// <summary>
        ///     Occurs when Models are set to a not null value. Constructs <see cref="View" />
        /// </summary>
        protected virtual void OnNewModels()
        {
            this.View = new HliCollectionViewSource<T>(this.Models);

            this.Models.CollectionChanged -= this.ModelOnCollectionChanged;
            this.Models.CollectionChanged += this.ModelOnCollectionChanged;
            this.ModelOnCollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, this.models));
            this.OnPropertyChanged(new PropertyChangedEventArgs(nameof(this.Models)));
        }

        /// <summary>
        ///     Occurs when <see cref="SelectedItem" /> is set to a non-null value. By default starts editing the item.
        /// </summary>
        protected virtual void OnSelectedItemChanged()
        {
            this.QueryId = this.selectedItem.Model.Id;

            var item = this.SelectedItem;
            if (item != null)
            {
                item.Model.AcceptChanges();
                item.OnEdit();
            }

            this.RefreshCommands();
        }

        private bool CanFilter()
        {
            return this.Models != null && this.Models.Any();
        }

        /// <summary>
        ///     Setup ICommand's to functions
        /// </summary>
        private void InitCommands()
        {
            this.AddCommand = new DelegateCommand(this.OnAdd, this.CanAdd);
            this.RemoveCommand = new DelegateCommand(this.OnRemove, this.CanRemove);
            this.CopyCommand = new DelegateCommand(this.OnCopy, this.CanCopy);
            this.RefreshCommand = new DelegateCommand(this.OnRefresh, this.CanRefresh);
            this.SaveCommand = new DelegateCommand(this.OnSave, this.CanSave);
            this.CancelCommand = new DelegateCommand(this.OnCancel, this.CanCancel);
            this.BackCommand = new DelegateCommand(() => this.OnBack());
            this.FilterCommand = new DelegateCommand(this.OnFilter, this.CanFilter);
            this.RefreshCommands();
        }

        /// <summary>
        ///     Set model as <see cref="SelectedItem" /> and <see cref="IsEditing" /> to <c>true</c>.
        /// </summary>
        /// <seealso cref="EditEvent" />
        /// <param name="modelBase">The model being edited</param>
        private void OnEditEvent(IModelBase modelBase)
        {
            var match = this.ViewModels?.FirstOrDefault(vm => vm.Model.Id == modelBase.Id);
            if (match != null)
            {
                this.SelectedItem = match;
                this.IsEditing = true;
            }
        }

        private void OnFilter()
        {
            this.View.Filter = obj => string.IsNullOrEmpty(this.FilterText) || (obj != null && obj.ToString().Contains(this.FilterText));
        }

        /// <summary>
        ///     Ensures a ViewModel has corresponding <see cref="IModelBase" /> in <see cref="Models" />
        /// </summary>
        /// <param name="model">
        ///     VM to sync
        /// </param>
        /// <param name="action">
        ///     Change action
        /// </param>
        private void SyncModelWithViewModels(T model, NotifyCollectionChangedAction action)
        {
            if (model == null || this.viewModels == null)
            {
                return;
            }

            this.ViewModels.CollectionChanged -= this.ViewModelsOnCollectionChanged;
            var match = this.viewModels.FirstOrDefault(m => m.Model.Id == model.Id);
            if (action == NotifyCollectionChangedAction.Remove && match != null)
            {
                this.ViewModels.Remove(match);
            }
            else if (match == null && this.factory != null)
            {
                var vm = this.factory.CreateViewModel(model);
                this.ViewModels.Add(vm);
            }

            this.ViewModels.CollectionChanged += this.ViewModelsOnCollectionChanged;
        }

        /// <summary>
        ///     Ensures a ViewModel has corresponding <see cref="IModelBase" /> in <see cref="Models" />
        /// </summary>
        /// <param name="viewModel">
        ///     VM to sync
        /// </param>
        /// <param name="action">
        ///     Change action
        /// </param>
        private void SyncViewModelWithModels(IHliViewModel viewModel, NotifyCollectionChangedAction action)
        {
            if (this.models == null || viewModel?.Model == null)
            {
                return;
            }

            this.Models.CollectionChanged -= this.ModelOnCollectionChanged;
            var match = this.models.FirstOrDefault(m => m.Id == viewModel.Model.Id);
            if (action == NotifyCollectionChangedAction.Remove && match != null)
            {
                this.Models.Remove(match);
            }
            else if (match == null)
            {
                this.Models.Add(viewModel.Model as T);
            }

            this.Models.CollectionChanged += this.ModelOnCollectionChanged;
        }

        /// <summary>
        ///     The view models on collection changed.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="args">
        ///     The args.
        /// </param>
        private void ViewModelsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            if (args.NewItems != null)
            {
                foreach (var newItem in args.NewItems.OfType<IHliViewModel>())
                {
                    this.SyncViewModelWithModels(newItem, args.Action);
                }
            }

            if (args.OldItems != null)
            {
                foreach (var oldItem in args.OldItems.OfType<IHliViewModel>())
                {
                    this.SyncViewModelWithModels(oldItem, args.Action);
                }
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="HliListViewModel{T}" /> class.
        /// </summary>
        /// <param name="navigationService"></param>
        protected HliListViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            this.InitCommands();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="HliListViewModel{T}" /> class.
        /// </summary>
        /// <param name="factory">
        ///     The factory.
        /// </param>
        /// <param name="eventAggregator">Used to publish/subscribe events</param>
        /// <param name="navigationService">Nav services</param>
        protected HliListViewModel(IViewModelFactory<T> factory, IEventAggregator eventAggregator, INavigationService navigationService)
            : this(navigationService)
        {
            this.factory = factory;
            var editEvent = eventAggregator.GetEvent<EditEvent>();
            editEvent.Unsubscribe(this.OnEditEvent);
            editEvent.Subscribe(this.OnEditEvent);
            var deletedEvent = eventAggregator.GetEvent<DeletedEvent>();
            deletedEvent.Unsubscribe(this.OnDeletedEvent);
            deletedEvent.Subscribe(this.OnDeletedEvent);
        }

        private void OnDeletedEvent(ModelBase entity)
        {
            // Remove the deleted ViewModel
            if (entity != null && entity.GetType() == typeof(T))
            {
                var match = this.ViewModels?.FirstOrDefault(vm => vm.Model == entity);
                if (match != null)
                {
                    this.ViewModels.Remove(match);
                }
            }
        }

        #endregion

        /// <summary>
        ///     Method used for saving a <see cref="T" /> from database
        /// </summary>
        // ReSharper disable MemberCanBePrivate.Global - accessable for implementations
        protected Func<T, T> SaveMethod { get; set; }

        /// <summary>
        ///     Optional method for creating a new Model. Else default <see cref="T" /> is used
        /// </summary>
        protected Func<T> AddMethod { get; set; }

        /// <summary>
        ///     Method used for getting an individual <see cref="T" /> from database
        /// </summary>
        protected Func<Guid, T> GetMethod { get; set; }

        /// <summary>
        ///     Method used for deleting a <see cref="T" /> from database
        /// </summary>
        protected Action<T> DeleteMethod { get; set; }

        /// <summary>
        ///     Method used for getting a collection of <see cref="T" /> from database
        /// </summary>
        protected Func<IEnumerable<T>> RefreshMethod { get; set; }

        //// ReSharper restore MemberCanBePrivate.Global
    }

    //// ReSharper restore CompareNonConstrainedGenericWithNull
}