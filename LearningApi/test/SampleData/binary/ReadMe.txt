Description of the data

Source: https://stats.idre.ucla.edu/r/dae/logit-regression/

A researcher is interested in how variables, such as GRE (Graduate Record Exam scores), GPA (grade point average) and prestige of the undergraduate institution, effect admission into graduate school. The response variable, admit/don’t admit, is a binary variable.
##   admit gre  gpa rank
## 1     0 380 3.61    3
## 2     1 660 3.67    3
## 3     1 800 4.00    1
## 4     1 640 3.19    4
## 5     0 520 2.93    4
## 6     1 760 3.00    2

This dataset has a binary response (outcome, dependent) variable called admit. There are three predictor variables: gre, gpa and rank. We will treat the variables gre and gpa as continuous. The variable rank takes on the values 1 through 4. Institutions with a rank of 1 have the highest prestige, while those with a rank of 4 have the lowest. We can get basic descriptives for the entire data set by using summary. To get the standard deviations, we use sapply to apply the sd function to each variable in the dataset.

summary(mydata)

##      admit            gre           gpa            rank     
##  Min.   :0.000   Min.   :220   Min.   :2.26   Min.   :1.00  
##  1st Qu.:0.000   1st Qu.:520   1st Qu.:3.13   1st Qu.:2.00  
##  Median :0.000   Median :580   Median :3.40   Median :2.00  
##  Mean   :0.318   Mean   :588   Mean   :3.39   Mean   :2.48  
##  3rd Qu.:1.000   3rd Qu.:660   3rd Qu.:3.67   3rd Qu.:3.00  
##  Max.   :1.000   Max.   :800   Max.   :4.00   Max.   :4.00

