using System;
using System.Collections.Generic;
using Jellyfin.Plugin.HomeSections.Configuration;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Model.Plugins;
using MediaBrowser.Model.Serialization;

namespace Jellyfin.Plugin.HomeSections
{
    /// <summary>
    /// The main plugin class for Home Sections.
    /// </summary>
    public class Plugin : BasePlugin<PluginConfiguration>, IHasWebPages
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Plugin"/> class.
        /// </summary>
        /// <param name="applicationPaths">Instance of the <see cref="IApplicationPaths"/> interface.</param>
        /// <param name="xmlSerializer">Instance of the <see cref="IXmlSerializer"/> interface.</param>
        public Plugin(IApplicationPaths applicationPaths, IXmlSerializer xmlSerializer)
            : base(applicationPaths, xmlSerializer)
        {
            Instance = this;
            
            // Check if we need to add default sections
            if (!Configuration.DefaultSectionsAdded && Configuration.HomeSections.Count == 0)
            {
                AddDefaultSections();
                Configuration.DefaultSectionsAdded = true;
                SaveConfiguration();
            }
        }

        /// <summary>
        /// Gets the current plugin instance.
        /// </summary>
        public static Plugin Instance { get; private set; }

        /// <inheritdoc />
        public override string Name => "Home Sections";

        /// <inheritdoc />
        public override Guid Id => Guid.Parse("8a8b9759-3e5f-4c19-aaa3-fd7e7e9d1924");

        /// <inheritdoc />
        public override string Description => "Customize your home screen with configurable sections";

        /// <inheritdoc />
        public IEnumerable<PluginPageInfo> GetPages()
        {
            return new[]
            {
                new PluginPageInfo
                {
                    Name = Name,
                    EmbeddedResourcePath = GetType().Namespace + ".Configuration.configPage.html",
                }
            };
        }
        
        /// <summary>
        /// Adds default sections to the configuration.
        /// </summary>
        private void AddDefaultSections()
        {
            Configuration.HomeSections.Add(new HomeSection
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Library Tiles",
                Type = HomeSectionType.LibraryTiles,
                ViewType = "libraryButtons",
                Enabled = true,
                SortOrder = 0
            });
            
            Configuration.HomeSections.Add(new HomeSection
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
}
