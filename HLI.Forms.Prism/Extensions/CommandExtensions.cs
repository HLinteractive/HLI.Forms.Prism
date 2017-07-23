// // --------------------------------------------------------------------------------------------------------------------
// // <copyright file="HLI.Forms.Prism.CommandExtensions.cs" company="HL Interactive">
// //   Copyright © HL Interactive, Stockholm, Sweden, 2017
// // </copyright>
// // --------------------------------------------------------------------------------------------------------------------

using System.Windows.Input;

using Prism.Commands;

namespace HLI.Forms.Prism.Extensions
{
    public static class CommandExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Calls <see cref="DelegateCommandBase.RaiseCanExecuteChanged" /> on the <see cref="ICommand" />
        /// </summary>
        /// <param name="command"></param>
        public static void RaiseCanExecute(this ICommand command)
        {
            (command as DelegateCommandBase)?.RaiseCanExecuteChanged();
        }

        #endregion
    }
}