using BLAM.Models.Assets;
using BLAM.Models.Metadata;
using Newtonsoft.Json;
using SDK;
using SDK.WorkflowIO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomWorkflowIOData
{
    internal class CustomWorkflowIODataBlidget : Blidget<CustomIO, AssetsIO>
    {
        protected override async Task<BlidgetOutput<AssetsIO>> RunAsync(CustomIO inputData)
        {
            // Fetch configuration
            var groupIds = GetArgument<IEnumerable<int>>("groups");

            var client = new BlamClient();

            // Alternatively the RAW input JSON can be referenced directly
            var customData = JsonConvert.DeserializeObject<CustomIO>(InputDataJson);

            // Search for media
            var searchQuery = new SearchQueryInputModel
            {
                Query = $"media_id = {inputData.MediaID} OR originalFilename = {inputData.Filename}",
                Limit = 100
            };
            var results = await client.Search(searchQuery);
            var foundIds = results.Items.Select(i => i.Id);
            LogDebug("Found {FoundAssetsCount} asset(s) with query '{SearchQuery}'.", results.TotalResults, searchQuery.Query);

            // Create placeholder if media not found
            if (results.TotalResults == 0)
            {
                var placeholder = await client.CreateAsset(new AssetInputModel
                {
                    Title = inputData.Title,
                    ExpectedType = inputData.ContentType,
                    GroupIds = groupIds
                });
                LogDebug("Created placeholder asset '{PlaceholderTitle}'.", placeholder.Title);
                foundIds = foundIds.Concat([placeholder.Id]);
            }

            // Update found asset metadata from input data
            var metadataValues = new Dictionary<string, object>
            {
                ["title"] = inputData.Title,
                ["title_id"] = inputData.TitleID,
                ["rights_expiry"] = inputData.RightsExpiry,
                ["vendor"] = inputData.Vendor,
                ["vendor_id"] = inputData.VendorID
            };
            LogDebug("Parsed metadata values: {MetadataValues}", JsonConvert.SerializeObject(metadataValues));
            for (var i = 0; i < foundIds.Count(); i++)
            {
                await client.CreateMetadataSnapshot(new AssetMetadataSnapshotInputModel
                {
                    AssetId = foundIds.ElementAt(i),
                    Values = metadataValues,
                    Merge = true,
                    Username = inputData.Vendor
                });
                UpdateProgress((i * 100) / 100, $"Updated {i + 1} of {foundIds.Count()} asset(s).");
            }

            // Return found media
            return Complete(new AssetsIO { AssetIds = foundIds });
        }
    }
}
