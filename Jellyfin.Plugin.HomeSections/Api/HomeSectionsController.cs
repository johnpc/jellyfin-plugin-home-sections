using System;
using System.Collections.Generic;
using System.Linq;
using Jellyfin.Data.Entities;
using Jellyfin.Plugin.HomeSections.Configuration;
using MediaBrowser.Controller.Collections;
using MediaBrowser.Controller.Dto;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Library;
using MediaBrowser.Model.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Jellyfin.Plugin.HomeSections.Api
{
    /// <summary>
    /// Controller for the Home Sections API.
    /// </summary>
    [ApiController]
    [Route("HomeSections")]
    public class HomeSectionsController : ControllerBase
    {
        private readonly ILibraryManager _libraryManager;
        private readonly ICollectionManager _collectionManager;
        private readonly IUserManager _userManager;
        private readonly IDtoService _dtoService;

        /// <summary>
        /// Initializes a new instance of the <see cref="HomeSectionsController"/> class.
        /// </summary>
        /// <param name="libraryManager">Instance of the <see cref="ILibraryManager"/> interface.</param>
        /// <param name="collectionManager">Instance of the <see cref="ICollectionManager"/> interface.</param>
        /// <param name="userManager">Instance of the <see cref="IUserManager"/> interface.</param>
        /// <param name="dtoService">Instance of the <see cref="IDtoService"/> interface.</param>
        public HomeSectionsController(
            ILibraryManager libraryManager,
            ICollectionManager collectionManager,
            IUserManager userManager,
            IDtoService dtoService)
        {
            _libraryManager = libraryManager;
            _collectionManager = collectionManager;
            _userManager = userManager;
            _dtoService = dtoService;
        }

        /// <summary>
        /// Gets all home sections for the current user.
        /// </summary>
        /// <returns>The home sections.</returns>
        [HttpGet("GetHomeSections")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<HomeSectionResult>> GetHomeSections()
        {
            // Use a default user or skip user-specific logic
            var config = Plugin.Instance.Configuration;
            var results = new List<HomeSectionResult>();

            foreach (var section in config.HomeSections.Where(s => s.Enabled).OrderBy(s => s.SortOrder))
            {
                var result = new HomeSectionResult
                {
                    Id = section.Id,
                    Name = section.Name,
                    Type = section.Type.ToString(),
                    ViewType = GetViewType(section),
                    Items = GetItemsForSection(section, null)
                };

                results.Add(result);
            }

            return Ok(results);
        }

        private string GetViewType(Configuration.HomeSection section)
        {
            // If the section has a custom view type, use it
            if (!string.IsNullOrEmpty(section.ViewType))
            {
                return section.ViewType;
            }
            
            // Otherwise, use the default for the section type
            return section.Type switch
            {
                Configuration.HomeSectionType.LibraryTiles => "libraryButtons",
                Configuration.HomeSectionType.RecentlyAdded => "posterCard",
                Configuration.HomeSectionType.PinnedCollection => "posterCard",
                _ => "posterCard"
            };
        }

        private IEnumerable<BaseItemDto> GetItemsForSection(Configuration.HomeSection section, User user)
        {
            try
            {
                switch (section.Type)
                {
                    case Configuration.HomeSectionType.LibraryTiles:
                        return GetLibraryTiles();

                    case Configuration.HomeSectionType.RecentlyAdded:
                        return GetRecentlyAddedItems(section.MaxItems ?? 12);

                    case Configuration.HomeSectionType.PinnedCollection:
                        return GetCollectionItems(section.CollectionId);

                    default:
                        return Array.Empty<BaseItemDto>();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting items for section {section.Name}: {ex}");
                return Array.Empty<BaseItemDto>();
            }
        }

        private IEnumerable<BaseItemDto> GetLibraryTiles()
        {
            var libraries = _libraryManager.GetUserRootFolder().Children
                .Where(i => i.IsFolder && !i.IsHidden)
                .OrderBy(i => i.SortName)
                .ToList();

            return libraries.Select(library => _dtoService.GetBaseItemDto(library, new DtoOptions()));
        }

        private IEnumerable<BaseItemDto> GetRecentlyAddedItems(int maxItems)
        {
            var query = new MediaBrowser.Controller.Entities.InternalItemsQuery
            {
                Recursive = true,
                Limit = maxItems,
                IsVirtualItem = false
            };
            
            var recentItems = _libraryManager.GetItemList(query);
            
            // Filter to only include media items and sort by date created
            var filteredItems = recentItems
                .Where(i => i.GetType().Name == "Movie" || 
                       i.GetType().Name == "Series" || 
                       i.GetType().Name == "Episode")
                .OrderByDescending(i => i.DateCreated)
                .Take(maxItems);
            
            return filteredItems.Select(item => _dtoService.GetBaseItemDto(item, new DtoOptions()));
        }

        private IEnumerable<BaseItemDto> GetCollectionItems(string collectionId)
        {
            if (string.IsNullOrEmpty(collectionId))
            {
                return Array.Empty<BaseItemDto>();
            }

            var collection = _libraryManager.GetItemById(collectionId);
            if (collection == null || !(collection is Folder folder))
            {
                return Array.Empty<BaseItemDto>();
            }

            var query = new MediaBrowser.Controller.Entities.InternalItemsQuery
            {
                Recursive = true
            };

            var items = folder.GetItemList(query);
            return items.Select(item => _dtoService.GetBaseItemDto(item, new DtoOptions()));
        }
    }

    /// <summary>
    /// Home section result.
    /// </summary>
    public class HomeSectionResult
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
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the view type.
        /// </summary>
        public string ViewType { get; set; }

        /// <summary>
        /// Gets or sets the items in the section.
        /// </summary>
        public IEnumerable<BaseItemDto> Items { get; set; }
    }
}
