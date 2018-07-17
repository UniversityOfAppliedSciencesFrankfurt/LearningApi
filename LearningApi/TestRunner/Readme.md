# Test Runner
*Test Runner* is toll for running of multiple DeepRbm tests at once.
To run test do following:
1. Create some folder (YourFolder) and clone the RB branch: 
git clone https://github.com/UniversityOfAppliedSciencesFrankfurt/LearningApi.git -b rbm

2. Build te test runner tool
Navigate to TestRunnerProject and start build:
>cd YourFolder\LearningApi\LearningApi\TestRunner
>dotnet build

3. To run a tool, prepare few test files. Example: Tests.txt

dotnet run <name of test file> <num of concurrent tests>
Example:
dotnet run testfile.txt 4

