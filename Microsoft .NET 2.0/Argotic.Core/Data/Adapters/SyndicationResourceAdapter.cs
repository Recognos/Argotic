﻿/****************************************************************************
Modification History:
*****************************************************************************
Date		Author		Description
*****************************************************************************
12/06/2007	brian.kuhn	Created SyndicationResourceAdapter Class
****************************************************************************/
using System;
using System.Xml;
using System.Xml.XPath;

using Argotic.Common;
using Argotic.Syndication;
using Argotic.Syndication.Specialized;

namespace Argotic.Data.Adapters
{
    /// <summary>
    /// Represents a <see cref="XPathNavigator"/> and <see cref="SyndicationResourceLoadSettings"/> that are used to fill a <see cref="ISyndicationResource"/>.
    /// </summary>
    public class SyndicationResourceAdapter
    {
        //============================================================
        //	PUBLIC/PRIVATE/PROTECTED MEMBERS
        //============================================================
        #region PRIVATE/PROTECTED/PUBLIC MEMBERS
        /// <summary>
        /// Private member to hold the XPathNavigator used to load a syndication resource.
        /// </summary>
        private XPathNavigator adapterNavigator;
        /// <summary>
        /// Private member to hold the XPathNavigator used to configure the load of a syndication resource.
        /// </summary>
        private SyndicationResourceLoadSettings adapterSettings  = new SyndicationResourceLoadSettings();
        #endregion

        //============================================================
        //	CONSTRUCTORS
        //============================================================
        #region SyndicationResourceAdapter(XPathNavigator navigator, SyndicationResourceLoadSettings settings)
        /// <summary>
        /// Initializes a new instance of the <see cref="SyndicationResourceAdapter"/> class using the supplied <see cref="XPathNavigator"/> and <see cref="SyndicationResourceLoadSettings"/>.
        /// </summary>
        /// <param name="navigator">A read-only <see cref="XPathNavigator"/> object for navigating through the syndication resource information.</param>
        /// <param name="settings">The <see cref="SyndicationResourceLoadSettings"/> object used to configure the load operation of the <see cref="ISyndicationResource"/>.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="navigator"/> is a null reference (Nothing in Visual Basic).</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="settings"/> is a null reference (Nothing in Visual Basic).</exception>
        public SyndicationResourceAdapter(XPathNavigator navigator, SyndicationResourceLoadSettings settings)
        {
            //------------------------------------------------------------
            //	Validate parameters
            //------------------------------------------------------------
            Guard.ArgumentNotNull(navigator, "navigator");
            Guard.ArgumentNotNull(settings, "settings");

            adapterNavigator    = navigator;
            adapterSettings     = settings;
        }
        #endregion

        //============================================================
        //	PUBLIC PROPERTIES
        //============================================================
        #region Navigator
        /// <summary>
        /// Gets the <see cref="XPathNavigator"/> used to fill a syndication resource.
        /// </summary>
        /// <value>The <see cref="XPathNavigator"/> used to fill a syndication resource.</value>
        public XPathNavigator Navigator
        {
            get
            {
                return adapterNavigator;
            }
        }
        #endregion

        #region Settings
        /// <summary>
        /// Gets the <see cref="SyndicationResourceLoadSettings"/> used to configure the fill of a syndication resource.
        /// </summary>
        /// <value>The <see cref="SyndicationResourceLoadSettings"/> used to configure the fill of a syndication resource.</value>
        public SyndicationResourceLoadSettings Settings
        {
            get
            {
                return adapterSettings;
            }
        }
        #endregion

        //============================================================
        //	PUBLIC METHODS
        //============================================================
        #region Fill(ISyndicationResource resource, SyndicationContentFormat format)
        /// <summary>
        /// Modifies the <see cref="ISyndicationResource"/> to match the data source.
        /// </summary>
        /// <param name="resource">The <see cref="ISyndicationResource"/> to be filled.</param>
        /// <param name="format">The <see cref="SyndicationContentFormat"/> enumeration value that indicates the type of syndication format that the <paramref name="resource"/> is expected to conform to.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="resource"/> is a null reference (Nothing in Visual Basic).</exception>
        /// <exception cref="ArgumentException">The <paramref name="format"/> is equal to <see cref="SyndicationContentFormat.None"/>.</exception>
        /// <exception cref="FormatException">The <paramref name="resource"/> data does not conform to the specified <paramref name="format"/>.</exception>
        public void Fill(ISyndicationResource resource, SyndicationContentFormat format)
        {
            //------------------------------------------------------------
            //	Validate parameters
            //------------------------------------------------------------
            Guard.ArgumentNotNull(resource, "resource");
            if (format == SyndicationContentFormat.None)
            {
                throw new ArgumentException(String.Format(null, "The specified syndication content format of {0} is invalid.", format), "format");
            }

            //------------------------------------------------------------
            //	Extract syndication resource meta-data
            //------------------------------------------------------------
            SyndicationResourceMetadata resourceMetadata    = new SyndicationResourceMetadata(this.Navigator);

            //------------------------------------------------------------
            //	Verify resource conforms to specified format
            //------------------------------------------------------------
            if (format != resourceMetadata.Format)
            {
                throw new FormatException(String.Format(null, "The supplied syndication resource has a content format of {0}, which does not match the expected content format of {1}.", resourceMetadata.Format, format));
            }

            //------------------------------------------------------------
            //	Fill syndication resource using appropriate data adapter
            //------------------------------------------------------------
            switch (format)
            {
                case SyndicationContentFormat.Apml:

                    this.FillApmlResource(resource, resourceMetadata);
                    break;

                case SyndicationContentFormat.Atom:

                    this.FillAtomResource(resource, resourceMetadata);
                    break;

                case SyndicationContentFormat.BlogML:

                    this.FillBlogMLResource(resource, resourceMetadata);
                    break;

                case SyndicationContentFormat.Opml:

                    this.FillOpmlResource(resource, resourceMetadata);
                    break;

                case SyndicationContentFormat.Rsd:

                    this.FillRsdResource(resource, resourceMetadata);
                    break;

                case SyndicationContentFormat.Rss:

                    this.FillRssResource(resource, resourceMetadata);
                    break;
            }
        }
        #endregion

        //============================================================
        //	PRIVATE METHODS
        //============================================================
        #region FillApmlResource(ISyndicationResource resource, SyndicationResourceMetadata resourceMetadata)
        /// <summary>
        /// Modifies the <see cref="ISyndicationResource"/> to match the data source.
        /// </summary>
        /// <param name="resource">The Attention Profiling Markup Language (APML) <see cref="ISyndicationResource"/> to be filled.</param>
        /// <param name="resourceMetadata">A <see cref="SyndicationResourceMetadata"/> object that represents the meta-data describing the <paramref name="resource"/>.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="resource"/> is a null reference (Nothing in Visual Basic).</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="resourceMetadata"/> is a null reference (Nothing in Visual Basic).</exception>
        private void FillApmlResource(ISyndicationResource resource, SyndicationResourceMetadata resourceMetadata)
        {
            //------------------------------------------------------------
            //	Validate parameter
            //------------------------------------------------------------
            Guard.ArgumentNotNull(resource, "resource");
            Guard.ArgumentNotNull(resourceMetadata, "resourceMetadata");

            //------------------------------------------------------------
            //	Fill syndication resource using appropriate data adapter
            //------------------------------------------------------------
            ApmlDocument apmlDocument   = resource as ApmlDocument;

            if (resourceMetadata.Version == new Version("0.6"))
            {
                Apml06SyndicationResourceAdapter apml06Adapter  = new Apml06SyndicationResourceAdapter(this.Navigator, this.Settings);
                apml06Adapter.Fill(apmlDocument);
            }
        }
        #endregion

        #region FillAtomResource(ISyndicationResource resource, SyndicationResourceMetadata resourceMetadata)
        /// <summary>
        /// Modifies the <see cref="ISyndicationResource"/> to match the data source.
        /// </summary>
        /// <param name="resource">The Atom <see cref="ISyndicationResource"/> to be filled.</param>
        /// <param name="resourceMetadata">A <see cref="SyndicationResourceMetadata"/> object that represents the meta-data describing the <paramref name="resource"/>.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="resource"/> is a null reference (Nothing in Visual Basic).</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="resourceMetadata"/> is a null reference (Nothing in Visual Basic).</exception>
        private void FillAtomResource(ISyndicationResource resource, SyndicationResourceMetadata resourceMetadata)
        {
            //------------------------------------------------------------
            //	Validate parameter
            //------------------------------------------------------------
            Guard.ArgumentNotNull(resource, "resource");
            Guard.ArgumentNotNull(resourceMetadata, "resourceMetadata");

            //------------------------------------------------------------
            //	Fill syndication resource using appropriate data adapter
            //------------------------------------------------------------
            AtomFeed atomFeed   = resource as AtomFeed;
            AtomEntry atomEntry = resource as AtomEntry;

            if (resourceMetadata.Version == new Version("1.0"))
            {
                Atom10SyndicationResourceAdapter atom10Adapter  = new Atom10SyndicationResourceAdapter(this.Navigator, this.Settings);
                if (atomFeed != null)
                {
                    atom10Adapter.Fill(atomFeed);
                }
                else if (atomEntry != null)
                {
                    atom10Adapter.Fill(atomEntry);
                }
            }

            if (resourceMetadata.Version == new Version("0.3"))
            {
                Atom03SyndicationResourceAdapter atom03Adapter  = new Atom03SyndicationResourceAdapter(this.Navigator, this.Settings);
                if (atomFeed != null)
                {
                    atom03Adapter.Fill(atomFeed);
                }
                else if (atomEntry != null)
                {
                    atom03Adapter.Fill(atomEntry);
                }
            }
        }
        #endregion

        #region FillBlogMLResource(ISyndicationResource resource, SyndicationResourceMetadata resourceMetadata)
        /// <summary>
        /// Modifies the <see cref="ISyndicationResource"/> to match the data source.
        /// </summary>
        /// <param name="resource">The BlogML <see cref="ISyndicationResource"/> to be filled.</param>
        /// <param name="resourceMetadata">A <see cref="SyndicationResourceMetadata"/> object that represents the meta-data describing the <paramref name="resource"/>.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="resource"/> is a null reference (Nothing in Visual Basic).</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="resourceMetadata"/> is a null reference (Nothing in Visual Basic).</exception>
        private void FillBlogMLResource(ISyndicationResource resource, SyndicationResourceMetadata resourceMetadata)
        {
            //------------------------------------------------------------
            //	Validate parameter
            //------------------------------------------------------------
            Guard.ArgumentNotNull(resource, "resource");
            Guard.ArgumentNotNull(resourceMetadata, "resourceMetadata");

            //------------------------------------------------------------
            //	Fill syndication resource using appropriate data adapter
            //------------------------------------------------------------
            BlogMLDocument blogMLDocument                       = resource as BlogMLDocument;
            BlogML20SyndicationResourceAdapter blogML20Adapter  = new BlogML20SyndicationResourceAdapter(this.Navigator, this.Settings);

            if (resourceMetadata.Version == new Version("2.0"))
            {
                blogML20Adapter.Fill(blogMLDocument);
            }
        }
        #endregion

        #region FillOpmlResource(ISyndicationResource resource, SyndicationResourceMetadata resourceMetadata)
        /// <summary>
        /// Modifies the <see cref="ISyndicationResource"/> to match the data source.
        /// </summary>
        /// <param name="resource">The Outline Processor Markup Language (OPML) <see cref="ISyndicationResource"/> to be filled.</param>
        /// <param name="resourceMetadata">A <see cref="SyndicationResourceMetadata"/> object that represents the meta-data describing the <paramref name="resource"/>.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="resource"/> is a null reference (Nothing in Visual Basic).</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="resourceMetadata"/> is a null reference (Nothing in Visual Basic).</exception>
        private void FillOpmlResource(ISyndicationResource resource, SyndicationResourceMetadata resourceMetadata)
        {
            //------------------------------------------------------------
            //	Validate parameter
            //------------------------------------------------------------
            Guard.ArgumentNotNull(resource, "resource");
            Guard.ArgumentNotNull(resourceMetadata, "resourceMetadata");

            //------------------------------------------------------------
            //	Fill syndication resource using appropriate data adapter
            //------------------------------------------------------------
            OpmlDocument opmlDocument                       = resource as OpmlDocument;
            Opml20SyndicationResourceAdapter opml20Adapter  = new Opml20SyndicationResourceAdapter(this.Navigator, this.Settings);

            if (resourceMetadata.Version == new Version("2.0"))
            {
                opml20Adapter.Fill(opmlDocument);
            }

            if (resourceMetadata.Version == new Version("1.1"))
            {
                opml20Adapter.Fill(opmlDocument);
            }

            if (resourceMetadata.Version == new Version("1.0"))
            {
                opml20Adapter.Fill(opmlDocument);
            }
        }
        #endregion

        #region FillRsdResource(ISyndicationResource resource, SyndicationResourceMetadata resourceMetadata)
        /// <summary>
        /// Modifies the <see cref="ISyndicationResource"/> to match the data source.
        /// </summary>
        /// <param name="resource">The Really Simple Discovery (RSD) <see cref="ISyndicationResource"/> to be filled.</param>
        /// <param name="resourceMetadata">A <see cref="SyndicationResourceMetadata"/> object that represents the meta-data describing the <paramref name="resource"/>.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="resource"/> is a null reference (Nothing in Visual Basic).</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="resourceMetadata"/> is a null reference (Nothing in Visual Basic).</exception>
        private void FillRsdResource(ISyndicationResource resource, SyndicationResourceMetadata resourceMetadata)
        {
            //------------------------------------------------------------
            //	Validate parameter
            //------------------------------------------------------------
            Guard.ArgumentNotNull(resource, "resource");
            Guard.ArgumentNotNull(resourceMetadata, "resourceMetadata");

            //------------------------------------------------------------
            //	Fill syndication resource using appropriate data adapter
            //------------------------------------------------------------
            RsdDocument rsdDocument = resource as RsdDocument;

            if (resourceMetadata.Version == new Version("1.0"))
            {
                Rsd10SyndicationResourceAdapter rsd10Adapter    = new Rsd10SyndicationResourceAdapter(this.Navigator, this.Settings);
                rsd10Adapter.Fill(rsdDocument);
            }

            if (resourceMetadata.Version == new Version("0.6"))
            {
                Rsd06SyndicationResourceAdapter rsd06Adapter    = new Rsd06SyndicationResourceAdapter(this.Navigator, this.Settings);
                rsd06Adapter.Fill(rsdDocument);
            }
        }
        #endregion

        #region FillRssResource(ISyndicationResource resource, SyndicationResourceMetadata resourceMetadata)
        /// <summary>
        /// Modifies the <see cref="ISyndicationResource"/> to match the data source.
        /// </summary>
        /// <param name="resource">The Really Simple Syndication (RSS) <see cref="ISyndicationResource"/> to be filled.</param>
        /// <param name="resourceMetadata">A <see cref="SyndicationResourceMetadata"/> object that represents the meta-data describing the <paramref name="resource"/>.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="resource"/> is a null reference (Nothing in Visual Basic).</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="resourceMetadata"/> is a null reference (Nothing in Visual Basic).</exception>
        private void FillRssResource(ISyndicationResource resource, SyndicationResourceMetadata resourceMetadata)
        {
            //------------------------------------------------------------
            //	Validate parameter
            //------------------------------------------------------------
            Guard.ArgumentNotNull(resource, "resource");
            Guard.ArgumentNotNull(resourceMetadata, "resourceMetadata");

            //------------------------------------------------------------
            //	Fill syndication resource using appropriate data adapter
            //------------------------------------------------------------
            RssFeed rssFeed = resource as RssFeed;

            if (resourceMetadata.Version == new Version("2.0"))
            {
                Rss20SyndicationResourceAdapter rss20Adapter    = new Rss20SyndicationResourceAdapter(this.Navigator, this.Settings);
                rss20Adapter.Fill(rssFeed);
            }

            if (resourceMetadata.Version == new Version("1.0"))
            {
                Rss10SyndicationResourceAdapter rss10Adapter    = new Rss10SyndicationResourceAdapter(this.Navigator, this.Settings);
                rss10Adapter.Fill(rssFeed);
            }

            if (resourceMetadata.Version == new Version("0.92"))
            {
                Rss092SyndicationResourceAdapter rss092Adapter  = new Rss092SyndicationResourceAdapter(this.Navigator, this.Settings);
                rss092Adapter.Fill(rssFeed);
            }

            if (resourceMetadata.Version == new Version("0.91"))
            {
                Rss091SyndicationResourceAdapter rss091Adapter  = new Rss091SyndicationResourceAdapter(this.Navigator, this.Settings);
                rss091Adapter.Fill(rssFeed);
            }

            if (resourceMetadata.Version == new Version("0.9"))
            {
                Rss090SyndicationResourceAdapter rss090Adapter  = new Rss090SyndicationResourceAdapter(this.Navigator, this.Settings);
                rss090Adapter.Fill(rssFeed);
            }
        }
        #endregion
    }
}
