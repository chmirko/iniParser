#build library and example usage projects
all: library examples

#clean both library and examples
clean:
	rm ConfigRW.dll UsageExamples.exe
	
#build only library
library:
	dmcs -target:library -out:ConfigRW.dll ConfigRW/*.cs ConfigRW/*/*.cs ConfigRW/*/*/*.cs 

#build only examples
examples:	
	dmcs -out:UsageExamples.exe -reference:ConfigRW.dll UsageExamples/*.cs 
