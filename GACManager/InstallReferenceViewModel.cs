using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apex.MVVM;

namespace GACManager
{
    public class InstallReferenceViewModel : ViewModel
    {
        
        /// <summary>
        /// The NotifyingProperty for the Identifier property.
        /// </summary>
        private readonly NotifyingProperty _identifierProperty =
          new NotifyingProperty("Identifier", typeof(string), default(string));

        /// <summary>
        /// Gets or sets Identifier.
        /// </summary>
        /// <value>The value of Identifier.</value>
        public string Identifier
        {
            get { return (string)GetValue(_identifierProperty); }
            set { SetValue(_identifierProperty, value); }
        }

        
        /// <summary>
        /// The NotifyingProperty for the Description property.
        /// </summary>
        private readonly NotifyingProperty _descriptionProperty =
          new NotifyingProperty("Description", typeof(string), default(string));

        /// <summary>
        /// Gets or sets Description.
        /// </summary>
        /// <value>The value of Description.</value>
        public string Description
        {
            get { return (string)GetValue(_descriptionProperty); }
            set { SetValue(_descriptionProperty, value); }
        }
    }
}
