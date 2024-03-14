using System;
using System.Collections;
using System.Collections.Generic;

namespace Enscape.Integration.CodingChallenge;

/* Challenge 2 */

public record PixelCluster((int x, int y)[] Pixels);

/// <summary>
/// Creates clusters of pixels from an existing pixel grid.
/// </summary>
public interface IPixelClusterizer
{
    /// <summary>
    /// Groups neighboring pixels into clusters.
    /// </summary>
    /// <param name="inputGrid">The input pixel grid.</param>
    /// <returns>All pixel clusters found in the grid.</returns>
    /// <remarks>Two pixels are not considered neighbors if they are only connected diagonally.</remarks>
    PixelCluster[] CreateClusters(IPixelGrid inputGrid);
}

public class PixelClusterizer : IPixelClusterizer
{
    public PixelCluster[] CreateClusters(IPixelGrid inputGrid)
    {
        int clusterIndex = 0;

        //Dictionary<(int x, int y), int clusterIndex > : track the cluster each pixel belongs to
        Dictionary<(int, int), int> pixelToClusterMap = new Dictionary<(int, int), int>();

        for (int x = 0; x < inputGrid.Width; x++)
        {
            for (int y = 0; y < inputGrid.Height; y++)
            {
                if (!inputGrid.Get(x, y)) continue; //if Pixel is empty 

                int leftClusterId = 0;
                int aboveClusterId = 0;

                bool leftClusterFound = x > 0 && pixelToClusterMap.TryGetValue((x - 1, y), out leftClusterId);
                bool aboveClusterFound = y > 0 && pixelToClusterMap.TryGetValue((x, y - 1), out aboveClusterId);

                if (leftClusterFound && aboveClusterFound && leftClusterId != aboveClusterId)
                {
                    // Merge the two clusters
                    pixelToClusterMap.Keys.ToList().ForEach(key => pixelToClusterMap[key] = pixelToClusterMap[key] == aboveClusterId ? leftClusterId : pixelToClusterMap[key]);
                }

                int assignedClusterId = leftClusterFound ? leftClusterId : aboveClusterFound ? aboveClusterId : clusterIndex++;
                pixelToClusterMap[(x, y)] = assignedClusterId;

            }
        }

        //Convert Dictionary of Pixels to List
        var clusters = new List<PixelCluster>();
        for (int i = 0; i < clusterIndex; i++)
        {
            var clusterPixels = pixelToClusterMap
                .Where(pair => pair.Value == i)
                .Select(pair => pair.Key)
                .ToArray();

            if (clusterPixels.Length > 0)
                clusters.Add(new PixelCluster(clusterPixels));
        }


        return clusters.ToArray();
    }



}