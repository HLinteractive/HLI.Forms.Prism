// // --------------------------------------------------------------------------------------------------------------------
// // <copyright file="HLI.Forms.Prism.ISerializableModel.cs" company="HL Interactive">
// //   Copyright © HL Interactive, Stockholm, Sweden, 2017
// // </copyright>
// // --------------------------------------------------------------------------------------------------------------------

namespace HLI.Forms.Prism.Interfaces
{
    /// <summary>
    ///     Describes a model that can be serialized and deserialized
    /// </summary>
    public interface ISerializableModel
    {
        #region Public Properties

        /// <summary>
        ///     Determines if the model is currently serializing. Use to avoid issues with serialization
        /// </summary>
        bool IsSerializing { get; set; }

        #endregion
    }
}