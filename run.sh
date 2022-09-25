#!/bin/sh
cd egrep_cpy
echo "Running egrep_cpy"
dotnet run "a|bdc*|dzc*"
echo "Exiting..."