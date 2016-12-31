The Iris flower data set or Fisher's Iris data set is a multivariate data set introduced by Ronald Fisher in his 1936 paper.
The use of multiple measurements in taxonomic problems as an example of linear discriminant analysis.
It is sometimes called Anderson's Iris data set because Edgar Anderson collected the data to quantify the morphologic variation
 of Iris flowers of three related species. Two of the three species were collected in the Gaspé Peninsula "all from the same pasture,
 and picked on the same day and measured at the same time by the same person with the same apparatus".

 -Four features were measured from each sample: the length and the width of the sepals and petals, in centimetres. 
 Based on the combination of these four features, Fisher developed a linear discriminant model to distinguish the species from each other.
- The data set consists of samples from each of three species of Iris (Iris setosa, Iris virginica and Iris versicolor).


//#line which begins with # is comment and should not be parsed
//
//#######         LearningAPI data mapper file for iris data set       ########
//
//# col:f- feature, l - label (only one is alowed), i - ignore column
//# - colType, num - numerical column, bin - binary column, catx - category column with x - categories,
//# str - string column which shoub be avoided during mapping
//#normalisation: minmax, gaus, none, ....
//#missingvalue:min, max, mean, ......
//
//col:f,f,f,f,l
//coltype:num,num,num,num,cat3
//normalisation:minmax,minmax,minmax,minmax,none 
//missingvalue:mean,mean,mean,mean,mean