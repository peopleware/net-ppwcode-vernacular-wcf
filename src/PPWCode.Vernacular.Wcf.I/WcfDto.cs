using System;
using System.Runtime.Serialization;

namespace PPWCode.Vernacular.Wcf.I
{
    [DataContract(IsReference = true), Serializable]
    public class WcfDto : IExtensibleDataObject
    {
        /// <summary>
        /// Gets or sets the structure that contains extra data.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Runtime.Serialization.ExtensionDataObject"/> that contains data that is not recognized as belonging to the data contract.
        /// </returns>
        public ExtensionDataObject ExtensionData { get; set; }
    }
}