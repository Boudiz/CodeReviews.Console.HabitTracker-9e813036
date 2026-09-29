# Habit Logger

This is a project that create a simple console interface linked to a sqLite database. \
It can be used to Create, update, delete and view regulars habits

### Getting started

To start the project execute : `dotnet run --project src/ ` \
If you want to use the text to speech option create a speech recognition service in Azure \
https://portal.azure.com/#home \
Then enter your key and region in config.json file located next to this file

### Documentation

This project was done following this tutorial:\
https://thecsharpacademy.com/project/12/habit-logger

### Issue 

For this project, the most challenging part was configuring the text to speech. \
Especially because I'm working on WSL, I needed to route my audio input into it. \
Also had issue with testing it as english is not my first language \
Also spent some time to be able to properly read the config.json file because the  \ 
compiled code is executed in bin/debug/...