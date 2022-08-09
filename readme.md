# Cpppp Bonus Extractor

Either use the prebuild binaries for win-64 or linux-64 from the release

or

1. get the dotnet 6.0 Runtime
2. run `dotnet CppBonusExtractor.dll` from the portable directory in the realease

or

1. get dotnet 6.0 SDK
2. run `dotnet build` in the source
3. run `dotnet CppBonusExtractor.dll` in the resulting output

## Sample Output:
```none
+=================================+
|   Rennorbs Exercise Extractor   |
+=================================+

This tool is designed to extract and prepare an archive obtained from a moodle exercise when selecting 'show all solutions' on the exercise and then selecting and downloading a set of solutions with the option 'place in separate folders' checked.


Exercise number wasn't supplied via command line, please tell me wich exercise i should extract (e.g. '5' or '1_basics'): 2
No zip file was provided via the command line, but there is a singular zip file in the  directory. Assuming you want to use this file (CC++ Programm.-prak 18-su-1030-pr (SoSe 2022)-Bonusaufgabe C-907433.zip).
there is a directory called forced_2. I will copy and overwrite those files into each groups directory.
Since the exercice name is just a number I tried to parse the name from the name of the zip file, did i get it right :)?   name: 'Bonusaufgabe C'   just rename the dir later if i was wrong.


'Gruppe 112' supplied zip, i will extract that for you...
        I'm not gonna overwrite dst file 'CMakeLists.txt' since its a makefile and we allow groups to supply custom ones.
'Gruppe 127' supplied zip, i will extract that for you...
        Found presumed source file 'main.c' in root dir, guessing it should be in src/
        Found presumed source file 'registers.c' in root dir, guessing it should be in src/
        Found presumed source file 'strings.c' in root dir, guessing it should be in src/
        I'm not gonna overwrite dst file 'CMakeLists.txt' since its a makefile and we allow groups to supply custom ones.
'Gruppe 114' supplied zip, i will extract that for you...
        I'm not gonna overwrite dst file 'CMakeLists.txt' since its a makefile and we allow groups to supply custom ones.
'Gruppe 121' supplied zip, i will extract that for you...
        Zip entry 'Debug/05_c' is not inside src directory, ignoring files directly in 'Debug' from now on.
        I'm not gonna overwrite dst file 'CMakeLists.txt' since its a makefile and we allow groups to supply custom ones.
'Gruppe 130' supplied zip, i will extract that for you...
        I'm not gonna overwrite dst file 'CMakeLists.txt' since its a makefile and we allow groups to supply custom ones.
'Gruppe 109' supplied zip, i will extract that for you...
        They supplied an unusual file '05_c/src/Bonus_05.project', ill just not extract that.
        I'm not gonna overwrite dst file 'CMakeLists.txt' since its a makefile and we allow groups to supply custom ones.
'Gruppe 123' supplied zip, i will extract that for you...
        I'm not gonna overwrite dst file 'CMakeLists.txt' since its a makefile and we allow groups to supply custom ones.
'Gruppe 113' supplied zip, i will extract that for you...
        I'm not gonna overwrite dst file 'CMakeLists.txt' since its a makefile and we allow groups to supply custom ones.
'Gruppe 111' supplied zip, i will extract that for you...
        I'm not gonna overwrite dst file 'CMakeLists.txt' since its a makefile and we allow groups to supply custom ones.
'Gruppe 128' supplied zip, i will extract that for you...
        I'm not gonna overwrite dst file 'CMakeLists.txt' since its a makefile and we allow groups to supply custom ones.
'Gruppe 100' supplied zip, i will extract that for you...
        They included a build directory, PUNISH!
        I'm not gonna overwrite dst file 'CMakeLists.txt' since its a makefile and we allow groups to supply custom ones.
'Gruppe 107' supplied zip, i will extract that for you...
        I'm not gonna overwrite dst file 'CMakeLists.txt' since its a makefile and we allow groups to supply custom ones.
'Gruppe 102' supplied zip, i will extract that for you...
        They included a build directory, PUNISH!
'Gruppe 096' supplied zip, i will extract that for you...
        I'm not gonna overwrite dst file 'CMakeLists.txt' since its a makefile and we allow groups to supply custom ones.
'Gruppe 110' supplied zip, i will extract that for you...
        I'm not gonna overwrite dst file 'CMakeLists.txt' since its a makefile and we allow groups to supply custom ones.
'Gruppe 105' supplied zip, i will extract that for you...
        I'm not gonna overwrite dst file 'CMakeLists.txt' since its a makefile and we allow groups to supply custom ones.
'Gruppe 101' supplied zip, i will extract that for you...
        They included a build directory, PUNISH!
        I'm not gonna overwrite dst file 'CMakeLists.txt' since its a makefile and we allow groups to supply custom ones.
'Gruppe 103' supplied zip, i will extract that for you...
        Found presumed source file 'main.c' in root dir, guessing it should be in src/
        Found presumed source file 'registers.c' in root dir, guessing it should be in src/
        Found presumed source file 'registers.h' in root dir, guessing it should be in src/
        Found presumed source file 'registers.test.cpp' in root dir, guessing it should be in src/
        Found presumed source file 'strings.c' in root dir, guessing it should be in src/
        Found presumed source file 'strings.h' in root dir, guessing it should be in src/
        Found presumed source file 'strings.test.cpp' in root dir, guessing it should be in src/
```
