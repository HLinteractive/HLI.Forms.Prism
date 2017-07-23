// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HLI.Forms.DeletedEvent.cs" company="HL Interactive">
//   Copyright © HL Interactive, Stockholm, Sweden, 2015
// </copyright>
// <summary>
//   Carries a deleted item
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using HLI.Core.Models;

using Prism.Events;

namespace HLI.Forms.Prism.Events
{
    /// <summary>
    ///     Carries a deleted item
    /// </summary>
    public class DeletedEvent : PubSubEvent<ModelBase>
    {
    }
}