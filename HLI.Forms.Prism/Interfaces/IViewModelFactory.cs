// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HLI.Forms.IViewModelFactory.cs" company="HL Interactive">
//   Copyright © HL Interactive, Stockholm, Sweden, 2015
// </copyright>
// <summary>
//   A factory to create <see cref="IHliViewModel{T}" />s for a <see cref="IModelBase" />
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using HLI.Core.Interfaces.Models;

namespace HLI.Forms.Prism.Interfaces
{
    /// <summary>
    /// A factory to create <see cref="IHliViewModel"/>s for a <see cref="IModelBase"/>
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    public interface IViewModelFactory<T>
        where T: class, IModelBase, new ()
    {
        #region Public Methods and Operators

        /// <summary>
        /// Generate a <see cref="IHliViewModel"/> to contain a <paramref name="model"/>
        /// </summary>
        /// <param name="model">
        /// The Model to populate return value with
        /// </param>
        /// <returns>
        /// <see cref="IHliViewModel"/> where the Model property is set from <paramref name="model"/>
        /// </returns>
        IHliViewModel CreateViewModel(T model);

        #endregion
    }
}