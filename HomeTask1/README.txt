#-----------------------
The users part is called "example":
You can create 2 matrixes and provide the paths to them, you will get 
the results in the file path to that you also need to provide.
#-----------------------
In the project called "Lessons":
You can find code for research dedicated to the time consumption of parallel and non-parallel computation.
You can also find here some useful methods that were you to read/print/create random or create and print random matrixes.
#-----------------------
In the project called "parallel":
2 methods of matrix multiplications:
  parallel
  non-parallel
They were mainly used in part Lessons to explore the time difference between two ways of computing.
#------------------------
In the project called "Tests":
You can find tests that cover only and the parallel method, it was built on the assumption that a non-parallel algorithm is simple enough not to make mistakes in it.
What is more, it opened up a favourable opportunity not to write tests by hand and to generate test sets automatically using an already written solution that is supposed to be the right one. 


We have observed the results of parallel matrixes multiplication.
Please, find the test result in the file "experiment_results.txt".
We ran both non-parallel and parallel matrix multiplication for the same matrixes
one by one ten times for each kind of matrixes multiplication. 
The sizes of matrixes were between [1*32] * [32 * 70] and [10000*32] * [32 * 70].
For this we got the following results:
(computational time for 1 multiplication)
Non parallel     Parallel
0,150298        0,1496756
0,0838984        0,0822233
0,0899497        0,0701308
0,2956629        0,1269111
3,5171408        1,1495734

We have found for small data sets Parallel computation does not have any noticeable effect,
whereas for large data sets parallel computation was more than 2 times faster.


We also have to mention results about dispertion
Non parallel           Parallel
0,001841839836736      0,001825554979161
0,000637739599396      0,0006137625649
0,000578921635161      0,000343826258689
0,007060478008336      0,001274069790009
1,07733002916244       0,115126505008800