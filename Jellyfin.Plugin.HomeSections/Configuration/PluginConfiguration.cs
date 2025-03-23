using System;
using System.Collections.Generic;
using MediaBrowser.Model.Plugins;

namespace Jellyfin.Plugin.HomeSections.Configuration
{
    /// <summary>
    /// Plugin configuration.
    /// </summary>
    public class PluginConfiguration : BasePluginConfiguration
    {
        private bool _defaultSectionsAdded = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginConfiguration"/> class.
        /// </summary>
        public PluginConfiguration()
        {
            HomeSections = new List<HomeSection>();
            
            // Only add default sections if they haven't been added before
            if (!_defaultSectionsAdded)
            {
                AddDefaultSections();
                _defaultSectionsAdded = true;
            }
        }

        /// <summary>
        /// Gets or sets the home sections.
        /// </summary>
        public List<HomeSection> HomeSections { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether default sections have been added.
        /// </summary>
        public bool DefaultSectionsAdded 
        { 
            get => _defaultSectionsAdded; 
            set => _defaultSectionsAdded = value; 
        }
        
        /// <summary>
        /// Adds the default sections to the configuration.
        /// </summary>
        private void AddDefaultSections()
        {
            HomeSections.Add(new HomeSection
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Library Tiles",
                Type = HomeSectionType.LibraryTiles,
                ViewType = "libraryButtons",
                Enabled = true,
                SortOrder = 0
            });
            
            HomeSections.Add(new HomeSection
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Recently Added",
                Type = HomeSectionType.RecentlyAdded,
                ViewType = "posterCard",
                Enabled = true,
                SortOrder = 1
            });
        }
    }

    /// <summary>
    /// Home section configuration.
    /// </summary>
    public class HomeSection
    {
        /// <summary>
        /// Gets or sets the section ID.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the section name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the section type.
        /// </summary>
        public HomeSectionType Type { get; set; }

        /// <summary>
        /// Gets or sets the view type for rendering the section.
        /// </summary>
        public string ViewType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the section is enabled.
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets the sort order.
        /// </summary>
        public int SortOrder { get; set; }

        /// <summary>
        /// Gets or sets the collection ID for Pinned Collection type.
        /// </summary>
        public string CollectionId { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of items to display.
        /// </summary>
        public int? MaxItems { get; set; }
    }

    /// <summary>
    /// Home section types.
    /// </summary>
    public enum HomeSectionType
    {
        /// <summary>
        /// Library tiles section.
        /// </summary>
        LibraryTiles,

        /// <summary>
        /// Recently added media items section.
        /// </summary>
        RecentlyAdded,

        /// <summary>
        /// Pinned collection section.
        /// </summary>
        PinnedCollection
    }
}
