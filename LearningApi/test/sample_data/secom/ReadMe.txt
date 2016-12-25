Predictive Maintenance Model
source: https://gallery.cortanaintelligence.com/Experiment/Predictive-Maintenance-Model-4


Summary
This is a predictive maintenance model that predicts yield failure in a semiconductor manufacturing process.

Description

This is a predictive maintenance model that predicts yield failure on a manufacturing process. 
This is a very important business problem for semiconductor manufacturers since their process can be complex
 and involves several stages from raw sand to the final integrated circuits. 
 Given the complexity, there are several factors that can lead to yield failures downstream in the manufacturing 
 process. 
 
Inputs:

The input data uses the SECOM dataset from the University of California at Irvine's machine learning database. 
This dataset from the semiconductor manufacturing industry was provided by Michael McCann and Adrian Johnston. 
The SECOM data set is available at the University of California at Irvine's Machine Learning Repository. 

- The dataset contains 1,567 examples, each with 591 features. 

- Of the 1,567 examples, 104 of them represent yield failures. 

- The features or columns represent sensor readings from 590 points in the manufacturing process.

Outputs:

- The training data has one output - a single binary variable showing the yield result (i.e. a simple pass or fail) for each example. 
- The output of the model is the probability of failure. 
- So for a given input the model shows the probability that the yield result will be a failure.

More details on the model are available in the book: Predictive Analytics with Microsoft Azure Machine Learning:
 Build and Deploy Actionable Solutions in Minutes.
