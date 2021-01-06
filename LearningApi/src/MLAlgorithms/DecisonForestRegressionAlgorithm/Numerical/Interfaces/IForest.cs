using RandomForest.Lib.Numerical.ItemSet.Item;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RandomForest.Lib.Numerical.Interfaces
{
    /// <summary>
    /// interface Iforest
    /// </summary>
    public interface IForest
    {
        /// <summary>
        /// Generate a number of trees based on learn data
        /// </summary>
        /// <param name="growParameters"></param>
        /// <returns>Count of trees</returns>
        int Grow(ForestGrowParameters growParameters);
        /// <summary>
        /// GrowAsync take parameter as input
        /// </summary>
        /// <param name="growParameters"></param>
        /// <returns></returns>
        Task<int> GrowAsync(ForestGrowParameters growParameters);

        /// <summary>
        /// Import a number of trees from Json files
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <returns>Count of trees</returns>
        int Grow(string directoryPath);

        /// <summary>
        /// Process exam object
        /// </summary>
        /// <param name="item">Exam object</param>
        /// <returns>Value of feature classification has been based on</returns>
        double Resolve(IItemNumerical item);

        /// <summary>
        /// Get list of feature names
        /// </summary>
        /// <returns></returns>
        List<string> GetFeatureNames();

        /// <summary>
        /// Create instance of item
        /// </summary>
        /// <returns></returns>
        IItemNumerical CreateItem();

        /// <summary>
        /// Number of trees
        /// </summary>
        /// <returns></returns>
        int TreeCount();
    }
}
