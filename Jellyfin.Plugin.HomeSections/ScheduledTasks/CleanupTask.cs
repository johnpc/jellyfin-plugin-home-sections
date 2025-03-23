using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Controller.Library;
using MediaBrowser.Model.Tasks;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.HomeSections.ScheduledTasks
{
    /// <summary>
    /// Cleanup task for the Home Sections plugin.
    /// </summary>
    public class CleanupTask : IScheduledTask
    {
        private readonly ILogger<CleanupTask> _logger;
        private readonly ILibraryManager _libraryManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="CleanupTask"/> class.
        /// </summary>
        /// <param name="logger">Instance of the <see cref="ILogger{CleanupTask}"/> interface.</param>
        /// <param name="libraryManager">Instance of the <see cref="ILibraryManager"/> interface.</param>
        public CleanupTask(ILogger<CleanupTask> logger, ILibraryManager libraryManager)
        {
            _logger = logger;
            _libraryManager = libraryManager;
        }

        /// <inheritdoc />
        public string Name => "Clean up Home Sections configuration";

        /// <inheritdoc />
        public string Key => "HomeSectionsCleanup";

        /// <inheritdoc />
        public string Description => "Removes references to deleted collections and validates configuration";

        /// <inheritdoc />
        public string Category => "Home Sections";

        /// <inheritdoc />
        public IEnumerable<TaskTriggerInfo> GetDefaultTriggers()
        {
            return new[]
            {
                new TaskTriggerInfo
                {
                    Type = TaskTriggerInfo.TriggerInterval,
                    IntervalTicks = TimeSpan.FromDays(1).Ticks
                }
            };
        }

        /// <inheritdoc />
        public Task ExecuteAsync(IProgress<double> progress, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting Home Sections cleanup task");
            
            var config = Plugin.Instance.Configuration;
            var modified = false;
            
            for (var i = config.HomeSections.Count - 1; i >= 0; i--)
            {
                var section = config.HomeSections[i];
                
                // Check for pinned collections that no longer exist
                if (section.Type == Configuration.HomeSectionType.PinnedCollection && 
                    !string.IsNullOrEmpty(section.CollectionId))
                {
                    var collection = _libraryManager.GetItemById(section.CollectionId);
                    if (collection == null)
                    {
                        _logger.LogInformation("Removing reference to deleted collection ID: {CollectionId}", section.CollectionId);
                        section.CollectionId = null;
                        modified = true;
                    }
                }
                
                progress.Report((double)(config.HomeSections.Count - i) / config.HomeSections.Count * 100);
                
                if (cancellationToken.IsCancellationRequested)
                {
                    return Task.CompletedTask;
                }
            }
            
            if (modified)
            {
                Plugin.Instance.UpdateConfiguration(config);
                _logger.LogInformation("Home Sections configuration updated");
            }
            
            _logger.LogInformation("Home Sections cleanup task completed");
            return Task.CompletedTask;
        }
    }
}
