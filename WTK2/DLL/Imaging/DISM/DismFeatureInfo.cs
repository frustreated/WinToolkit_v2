﻿using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace Microsoft.Dism
{
    public static partial class DismApi
    {
        /// <summary>
        ///     Describes advanced feature information, such as installed state and whether a restart is required after
        ///     installation.
        /// </summary>
        /// <remarks>
        ///     <a href="http://msdn.microsoft.com/en-us/library/windows/desktop/hh824793.aspx" />
        ///     typedef struct _DismFeatureInfo
        ///     {
        ///     PCWSTR FeatureName;
        ///     DismPackageFeatureState FeatureState;
        ///     PCWSTR DisplayName;
        ///     PCWSTR Description;
        ///     DismRestartType RestartRequired;
        ///     DismCustomProperty* CustomProperty;
        ///     UINT CustomPropertyCount;
        ///     } DismFeatureInfo;
        /// </remarks>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 4)]
        internal struct DismFeatureInfo_
        {
            /// <summary>
            ///     The name of the feature.
            /// </summary>
            public string FeatureName;

            /// <summary>
            ///     A valid DismPackageFeatureState Enumeration value such as DismStateInstalled or 7.
            /// </summary>
            public DismPackageFeatureState FeatureState;

            /// <summary>
            ///     The display name of the feature. This is not always unique across all features.
            /// </summary>
            public string DisplayName;

            /// <summary>
            ///     The description of the feature.
            /// </summary>
            public string Description;

            /// <summary>
            ///     A DismRestartType Enumeration value such as DismRestartPossible.
            /// </summary>
            public DismRestartType RestartRequired;

            /// <summary>
            ///     An array of DismCustomProperty Structure.m
            /// </summary>
            public IntPtr CustomProperty;

            /// <summary>
            ///     The number of elements in the CustomProperty array.
            /// </summary>
            public uint CustomPropertyCount;
        }
    }

    /// <summary>
    ///     Represents advanced feature information, such as installed state and whether a restart is required after
    ///     installation.
    /// </summary>
    public sealed class DismFeatureInfo
    {
        private readonly DismCustomPropertyCollection _customProperties = new DismCustomPropertyCollection();
        private readonly DismApi.DismFeatureInfo_ _featureInfo;

        /// <summary>
        ///     Initializes a new instance of the DismFeatureInfo class.
        /// </summary>
        /// <param name="featureInfoPtr">A pointer to a <see cref="DismApi.DismFeatureInfo_" /> struct.</param>
        internal DismFeatureInfo(IntPtr featureInfoPtr)
            : this(featureInfoPtr.ToStructure<DismApi.DismFeatureInfo_>())
        {
        }

        /// <summary>
        ///     Initializes a new instance of the DismFeatureInfo class.
        /// </summary>
        /// <param name="featureInfo">A <see cref="DismApi.DismFeatureInfo_" /> struct from the native DismApi.</param>
        internal DismFeatureInfo(DismApi.DismFeatureInfo_ featureInfo)
        {
            // Save a reference to the native struct
            _featureInfo = featureInfo;

            // See if there are any custom properties to load
            if (_featureInfo.CustomPropertyCount > 0 && _featureInfo.CustomProperty != IntPtr.Zero)
            {
                // Add the items
                _customProperties.AddRange<DismApi.DismCustomProperty_>(_featureInfo.CustomProperty,
                    (int) _featureInfo.CustomPropertyCount, i => new DismCustomProperty(i));
            }
        }

        /// <summary>
        ///     Gets a list of custom properties associated with the feature.
        /// </summary>
        public DismCustomPropertyCollection CustomProperties
        {
            get { return _customProperties; }
        }

        /// <summary>
        ///     Gets the description of the feature.
        /// </summary>
        public string Description
        {
            get { return _featureInfo.Description; }
        }

        /// <summary>
        ///     Gets the display name of the feature. This is not always unique across all features.
        /// </summary>
        public string DisplayName
        {
            get { return _featureInfo.DisplayName; }
        }

        /// <summary>
        ///     Gets the name of the feature.
        /// </summary>
        public string FeatureName
        {
            get { return _featureInfo.FeatureName; }
        }

        /// <summary>
        ///     Gets the state of the feature.
        /// </summary>
        public DismPackageFeatureState FeatureState
        {
            get { return _featureInfo.FeatureState; }
        }

        /// <summary>
        ///     Gets a value indicating if a restart is required when installing or uninstalling the feature.
        /// </summary>
        public DismRestartType RestartRequired
        {
            get { return _featureInfo.RestartRequired; }
        }

        /// <summary>
        ///     Determines whether the specified <see cref="T:System.Object" /> is equal to the current
        ///     <see cref="T:System.Object" />.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>
        ///     true if the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Object" />;
        ///     otherwise, false.
        /// </returns>
        public override bool Equals(object obj)
        {
            return obj != null && Equals(obj as DismFeatureInfo);
        }

        /// <summary>
        ///     Determines whether the specified <see cref="DismFeatureInfo" /> is equal to the current
        ///     <see cref="DismFeatureInfo" />.
        /// </summary>
        /// <param name="featureInfo">The <see cref="DismFeatureInfo" /> object to compare with the current object.</param>
        /// <returns>
        ///     true if the specified <see cref="DismFeatureInfo" /> is equal to the current <see cref="DismFeatureInfo" />;
        ///     otherwise, false.
        /// </returns>
        public bool Equals(DismFeatureInfo featureInfo)
        {
            return featureInfo != null
                   && CustomProperties.SequenceEqual(featureInfo.CustomProperties)
                   && Description == featureInfo.Description
                   && DisplayName == featureInfo.DisplayName
                   && FeatureName == featureInfo.FeatureName
                   && FeatureState == featureInfo.FeatureState
                   && RestartRequired == featureInfo.RestartRequired;
        }

        /// <summary>
        ///     Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>A hash code for the current <see cref="T:System.Object" />.</returns>
        public override int GetHashCode()
        {
            return CustomProperties.GetHashCode()
                   ^ (string.IsNullOrWhiteSpace(Description) ? 0 : Description.GetHashCode())
                   ^ (string.IsNullOrWhiteSpace(DisplayName) ? 0 : DisplayName.GetHashCode())
                   ^ (string.IsNullOrWhiteSpace(FeatureName) ? 0 : FeatureName.GetHashCode())
                   ^ FeatureState.GetHashCode()
                   ^ RestartRequired.GetHashCode();
        }
    }
}