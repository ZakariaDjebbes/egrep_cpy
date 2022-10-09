# 1. What is it
This is a copy of the egrep command provided by some linux distributions. Made as part of a project for Sorbonne University.

# 2. How to run

### 2. Manually (Recommanded)

1. Install the dotnet SDK and runtime [here](https://dotnet.microsoft.com/en-us/download).
2. Go to the root folder (where the readme file is located).
3. Restore the project :

    > dotnet restore

4. Run the project using this command :

    > dotnet run -c Releaase --project egrep_cpy -- -f "resources/babylon.txt" -r "Sargon" -cw

Note that you can choose your file and Regex and have some flags to customise the output, here is some help

```
egrep_cpy 1.0.0
Copyright (C) 2022 egrep_cpy

  -r, --regex     Required. The regular expression to use for matching.

  -f, --file      Required. The file to search in.

  -d, --detail    (Default: false) Shows a detailed output with the RegEx tree, the NFA and the DFA results or the carry over table depending on the algorithm used.

  -c, --count     (Default: false) Shows the total count of matches found.

  -w, --watch     (Default: false) Shows the time elapsed for parsing and matching the RegEx (excluding the printing to console time).

  -p, --pretty    (Default: false) Outputs the whole input file with the matches highlighted in green.

  --help          Display this help screen.

  --version       Display version information. 
```



5. Enjoy 😊! (hopefully 🤞)

### 2. Using the shell script (build.sh)

1. Install the dotnet SDK and runtime [here](https://dotnet.microsoft.com/en-us/download).
2. Go to the root folder (where the readme file is located).
3. Run the shell script using the source command (IMPORTANT).

    > source build.sh

4. You should see the output for the regex "Sargon" on the text babylon. 
    You should now also be able to use the egrep command in your terminal. Obviously all previous flags still work here.

    > egrep -f "resources/babylon.txt" -r "Sargon" -cw

    You CANNOT use it outside of the folder where the build.sh script is located AND you CANNOT use it in another terminal window unless you run the script again.

5. Enjoy 😊! (hopefully 🤞)

### 3. Using the dockerfile (Discouraged but still possible)

1. Install docker [here](https://docs.docker.com/get-docker/).
2. Go to the root folder (where the dockerfile file is located).
3. Build the docker image using the following command :

    > docker build -t egrep_cpy .

4. Run the docker image.
5. Your docker image will run the default command which is the egrep command on the babylon text with the regex "Sargon" and the -cw flags and print outputs to the console. You might not see the colored matches like the previous methods because the docker console does not support colors (I guess). Which is why i recommand using one of the previous methods.

    You can change the file / regex / flags in the dockerfile directly and then rebuild/rerun.

