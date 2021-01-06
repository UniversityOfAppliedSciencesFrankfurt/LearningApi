using Deedle;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DecisionTreeForLearningAPI.BinaryDecisionTree
{
    /// <summary>
    /// Decision tree processor for a binary classification problem
    /// (i.e. the target attribute is a category of two classes) 
    /// based on fit and predict methods.
    /// </summary>
    public class BinaryDTProcessor
    {
        private Frame<int, string> data;

        private String target;

        private BinaryDTProcessor leftBranch;

        private int maxDepth;

        private int depth;

        private BinaryDTProcessor rightBranch;

        private List<String> independent = new List<string>();

        private decimal impurityScore;

        private String criteria;

        private String feature;

        private String informationGain;

        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Decision tree processor for a binary classification problem
        /// (i.e. the target attribute is a category of two classes) 
        /// based on fit and predict methods.
        /// </summary>
        /// <param name="maxDepth">Maximum depth of splitting data for branch creation.</param>
        /// <param name="depth">Initial splitvalue for branch creation.</param>
        public BinaryDTProcessor(int maxDepth, int depth)
        {
            this.maxDepth = maxDepth;
            this.depth = depth;
            this.leftBranch = null; 
            this.rightBranch = null;
        }

        /// <summary>
        /// Trains the data to generate left tree and right tree branches.
        /// Rows with values less than the criteria go to the left branch and 
        /// greater than the criteria go to the right branch.
        /// This happens recursively until the max depth of the split is reached.
        /// </summary>
        /// <param name="frame">Data frame to be trained.</param>
        /// <param name="target">Target feature column used for splitting data into branches.</param>
        public void fit(Frame<int, string> frame, String target)
        {
            if (this.depth <= this.maxDepth)
            {
                Console.WriteLine("Processing at depth : " + this.depth);
            }

            this.data = frame;
            this.target = target;

            this.independent = this.data.ColumnKeys.ToList<string>();
            this.independent.Remove(target);

            Series<int, int> targetRows = this.data.GetColumn<int>(target);
            List<int> values = targetRows.ValuesAll.ToList();
            this.impurityScore = calculateImpurityScore(values); ///0.48

            List<String> splitColInfogain = findBestSplit();

            if (splitColInfogain.Count == 3)
            {
                this.criteria = splitColInfogain[0];
                this.feature = splitColInfogain[1];
                this.informationGain = splitColInfogain[2];
            }

            if (this.criteria != "None" &&
                (this.informationGain != null && Decimal.Parse(this.informationGain) > 0))
            {
                createBranches();
            }
            else
            {
                _log.Info("Max depth reached. No more split");
            }
        }

        /// <summary>
        /// Predict the probabilities of the test data based on data trained. 
        /// </summary>
        /// <param name="testFrame">The data frame on which probability is computed.</param>
        public BinaryDTResult predict(Frame<int, string> testFrame)
        {
            BinaryDTResult binaryDTResult = new BinaryDTResult();

            RowSeries<int, string> rowSeries = testFrame.Rows;
            List<Deedle.ObjectSeries<String>> rowList = rowSeries.ValuesAll.ToList();
            foreach (ObjectSeries<String> o in rowList)
            {
                flowDataThruTree(o, binaryDTResult);
            }

            return binaryDTResult;
        }

        /// <summary>
        /// Calls findBestSplitForColumn for all the independent features, to find the best split across all of them.
        /// </summary>
        /// <returns>Split:Feature:InfoGain</returns>
        private List<String> findBestSplit()
        {
            List<String> splitColInfogain = new List<string>();
            foreach (String col in independent)
            {
                List<String> infoGainAndSplitList = findBestSplitForColumn(col);
                String informationGain = infoGainAndSplitList[0];
                String split = infoGainAndSplitList[1];

                if (split == "None")
                {
                    continue;
                }
                if (splitColInfogain.Count == 0 || (Decimal.Parse(splitColInfogain[2]) < Decimal.Parse(informationGain)))
                {
                    splitColInfogain.Clear();
                    splitColInfogain.Insert(0, split);
                    splitColInfogain.Insert(1, col);
                    splitColInfogain.Insert(2, informationGain);
                }
            }

            return splitColInfogain;
        }

        /// <summary>
        /// A split should be capable of decreasing the impurity in the child node with respect to the parent node and the 
        /// quantitative decrease of impurity in the child node is called the Information Gain.
        /// In order to find the best split, we need to get the best split in each features and use the one with the 
        /// most information gain.
        /// Given a feature, all the unique values are separated and for each of those values, 
        /// a split is made in such a way the data is either less than or equal to the value or greater than the value.
        /// The rows with value less than value in the independent feature would become “left” branch and 
        /// others would go to the “right” branch.
        /// Then we can calculate the impurity for the left branch and the right branch. 
        /// Once we have both, we can calculate the information gain by using calculateInformationGain.
        /// This is done for all the unique values in the feature and split with highest information is returned by the method.
        /// </summary>
        /// <param name="column"></param>
        /// <returns>InfoGain:Split</returns>
        private List<String> findBestSplitForColumn(String column)
        {
            List<String> infoGainAndSplitList = new List<string>();

            Series<int, int> targetRows = this.data.GetColumn<int>(column);

            List<int> rowValues = targetRows.ValuesAll.ToList();
            List<int> finaltargetRows = this.data.GetColumn<int>(target).ValuesAll.ToList();

            List<int> distinct = targetRows.ValuesAll.Distinct().ToList();

            int countOfDistinctValues = distinct.Count();

            if (countOfDistinctValues == 1)
            {
                infoGainAndSplitList.Insert(0, "None");
                infoGainAndSplitList.Insert(1, "None");
                return infoGainAndSplitList;
            }

            String informationGain = "None";
            String split = "None";

            List<int> leftValues = new List<int>();
            List<int> rightValues = new List<int>();

            foreach (int disctValue in distinct)
            {
                for (int i = 0; i < rowValues.Count; i++)
                {
                    if (rowValues[i] <= disctValue)
                    {
                        leftValues.Add(finaltargetRows[i]);

                    }
                    else if (rowValues[i] > disctValue)
                    {
                        rightValues.Add(finaltargetRows[i]);
                    }
                }

                decimal leftBranchImpurityScore = calculateImpurityScore(leftValues);
                decimal rightBranchImpurityScore = calculateImpurityScore(rightValues);

                decimal score = calculateInformationGain(leftValues.Count, leftBranchImpurityScore,
                    rightValues.Count, rightBranchImpurityScore);

                if (informationGain == "None" || score > Decimal.Parse(informationGain))
                {
                    informationGain = score.ToString();
                    split = disctValue.ToString();

                    infoGainAndSplitList.Insert(0, informationGain);
                    infoGainAndSplitList.Insert(1, split);
                }
            }
            return infoGainAndSplitList;
        }

        /// <summary>
        /// Gini Impurity : A unit of measure to quantify the impurity to help find the optimal split.
        /// Given a feature(target) or a section of it, this method finds the probability of multiple classes. 
        /// Since we are working on a Binary classification problem, we would get only two probabilities and since their summation is 1.
        /// So p(i) * (1 — p(i)) would be the same for both classes, hence the * 2
        /// </summary>
        /// <param name="values">List of feature column values.</param>
        /// <returns>Impurity score of the feature column values.</returns>
        private decimal calculateImpurityScore(List<int> values)
        {
            if (values == null || values.Count == 0)
            {
                return 0;
            }

            decimal zeroCount = 0;
            decimal oneCount = 0;

            for (int i = 0; i < values.Count; i++)
            {
                int value = values[i];
                if (value == 0)
                {
                    zeroCount = zeroCount + 1;
                }
                else if (value == 1)
                {
                    oneCount = oneCount + 1;
                }
                else
                {
                    throw new ArgumentException("Binary classification only !!");
                }
            }

            decimal zeroProbability = zeroCount / values.Count;
            decimal oneProbability = oneCount / values.Count;
            decimal impurityScore = oneProbability * (1 - oneProbability) * 2;

            return impurityScore;
        }

        /// <summary>
        /// Calulates the information gain by using the left and right branch impurity scores.
        /// </summary>
        /// <param name="leftCount"></param>
        /// <param name="leftImpurity"></param>
        /// <param name="rightCount"></param>
        /// <param name="rightImpurity"></param>
        /// <returns>Information gain</returns>
        private decimal calculateInformationGain(int leftCount, decimal leftImpurity,
           int rightCount, decimal rightImpurity)
        {
            return this.impurityScore - ((leftCount / this.data.RowCount) * leftImpurity +
                                      (rightCount / this.data.RowCount) * rightImpurity);
        }

        /// <summary>
        /// Instantiates a a decision tree for both the left and the right branch. 
        /// We pass in the rows with values less than the criteria to the left branch and greater than the criteria to the right branch. 
        /// Invoking this method from fit would recursively create the branches and with that we have built the tree.
        /// </summary>
        private void createBranches()
        {
            List<int> leftBranchIndices = new List<int>();
            List<int> rightBranchIndices = new List<int>();

            List<int> allValues = this.data.GetColumn<int>(this.feature).ValuesAll.ToList();
            for (int i = 0; i < allValues.Count; i++)
            {
                if (allValues[i] <= Decimal.Parse(this.criteria))
                {
                    leftBranchIndices.Add(i);
                }
                else if (allValues[i] > Decimal.Parse(this.criteria))
                {
                    rightBranchIndices.Add(i);
                }
            }

            Frame<int, string> leftFrame = this.data.GetRowsAt(leftBranchIndices.ToArray());
            Frame<int, string> rightFrame = this.data.GetRowsAt(rightBranchIndices.ToArray());

            this.leftBranch = new BinaryDTProcessor(this.maxDepth, this.depth + 1);
            this.leftBranch.fit(leftFrame, this.target);

            this.rightBranch = new BinaryDTProcessor(this.maxDepth, this.depth + 1);
            this.rightBranch.fit(rightFrame, this.target);
        }

        /// <summary>
        ///  Traverses the tree for a row and finds the prediction
        /// </summary>
        /// <param name="row"></param>
        /// <param name="binaryDTResult"></param>
        private void flowDataThruTree(ObjectSeries<String> row, BinaryDTResult binaryDTResult)
        {
            if (this.isLeafNode())
            {
                BinaryProbability binaryProbability = new BinaryProbability();
                binaryProbability.SetindexOfRow(row.Index.ToString());
                List<decimal> probabilities = this.calculateProbability();
                binaryProbability.SetprobabilityOfOccuranceOfZero((double)probabilities[0]);
                binaryProbability.SetprobabilityOfOccuranceOfOne((double)probabilities[1]);

                binaryDTResult.addToBinaryDTResult(binaryProbability);
                return;
            }

            BinaryDTProcessor binaryDTProcessor = null;
            int value = int.Parse(row.Get(this.feature).ToString());
            if (value <= Decimal.Parse(this.criteria))
            {
                binaryDTProcessor = this.leftBranch;
            }
            else
            {
                binaryDTProcessor = this.rightBranch;
            }
            binaryDTProcessor.flowDataThruTree(row, binaryDTResult); 
        }

        /// <summary>
        /// Checks for leaf node.
        /// </summary>
        /// <returns></returns>
        private Boolean isLeafNode()
        {
            return this.leftBranch == null;
        }

        /// <summary>
        /// Calculates the probability of occurance of zero and one.
        /// </summary>
        /// <returns>zeroProbability:oneProbability</returns>
        private List<decimal> calculateProbability()
        {
            List<decimal> probabilities = new List<decimal>();
            List<int> targetRows = this.data.GetColumn<int>(this.target).ValuesAll.ToList();
            decimal zeroCount = 0;
            decimal oneCount = 0;

            for (int i = 0; i < targetRows.Count; i++)
            {
                int value = targetRows[i];
                if (value == 0)
                {
                    zeroCount = zeroCount + 1;
                }
                else if (value == 1)
                {
                    oneCount = oneCount + 1;
                }
                else
                {
                    throw new ArgumentException("Binary classification only !!");
                }
            }

            decimal zeroProbability = zeroCount / this.data.RowCount;
            decimal oneProbability = oneCount / this.data.RowCount;

            probabilities.Insert(0, zeroProbability);
            probabilities.Insert(1, oneProbability);

            return probabilities;
        }

    }
}
