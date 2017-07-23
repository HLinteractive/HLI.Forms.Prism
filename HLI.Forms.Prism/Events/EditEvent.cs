// // --------------------------------------------------------------------------------------------------------------------
// // <copyright file="HLI.Forms.EditEvent.cs" company="HL Interactive">
// //   Copyright © HL Interactive, Stockholm, Sweden, 2016
// // </copyright>
// // --------------------------------------------------------------------------------------------------------------------

using HLI.Core.Interfaces.Models;

using Prism.Events;

namespace HLI.Forms.Prism.Events
{
    /// <summary>
    ///     Occurs when the use begins editing a <see cref="IModelBase" />
    /// </summary>
    public class EditEvent : PubSubEvent<IModelBase>
    {
    }
}