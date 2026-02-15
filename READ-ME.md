# DailyGrandPrix
Welcome to my DailyGrandPrix repository!
![RedBull Racing 2026 Formula 1 car](https://cdn-8.motorsport.com/images/amp/2eZg5l3Y/s1000/max-verstappen-red-bull-racing.jpg)
## About
This is a console Formula 1 board-like game. It was created in January 19th, 2026, with an initial version which stored race information inside the driver's class, which made it incapable of storing multiple races. From February 4th, 2026, it got revamped in order to be capable of storing and processing multiple races simultaneously, fixed many bugs, and also added new features, such as tracks and CRUD operations in drivers, tracks, and championships.

I use this project to make a series on Reddit, in a subreddit called r/DailyGames (hence "Daily" in the name). This series was originally made by [u/Aartvb](https://www.reddit.com/user/Aartvb/) ([link to his GitHub](https://github.com/AartvB)) in 2025, and remade by me in early 2026 with the idea of making it more similar to Formula 1.

This project uses CSV to store data. This is because, when I started making this series, it was the only "database" I knew how to use.
## Functionality
This is a console application. In order to run it, open the folder where `DailyGrandPrix.csproj` and `Program.cs` in your terminal of preference, and type the command `dotnet run`. This will make it start running on your terminal. You can also run it without opening the folder in the terminal with `dotnet run --project path`, replacing `path` with the path for the folder said above.

When opening the program for the first time, it will ask you to include the path for the folder where you wish to store the data. You must write insert the path for the folder. It will then save the path for this folder in `path.txt`, in the bin folder.

After writing the path, the program will create three folders inside the database folder: Championships, Drivers, and Tracks. The program always checks if these folders exist, and creating them if they don't.

Finally, the main code starts. It shows you 20 choices you can choose from, each of them representing an action. You type the number of the action you want to make.

The 20 choices are:

1. Create driver: asks for a name, username, number, and team (enum). This creates a new driver with the information given.
2. See all drivers: shows all the drivers stored in the program.
3. Edit driver: asks you for the Id of the driver you want to edit and the information you want to edit (the same ones informed in 1). It changes the information of the existing driver.
4. Save drivers in database: creates or overwrites a .txt file for each driver, with the file being named after the respective driver's name. This files are stored in the Drivers folder, in the database. If a driver had been deleted as in 5, this process will delete its file.
5. Delete driver: asks for the Id of a driver, and then deletes it.
6. Create track: asks for a name and the amount of "steps" (explained in the section dedicated for the race) per lap the track has. It creates a new track.
7. See all tracks: shows all tracks.
8. Edit track: asks for the Id of a track and the information to edit (name or steps per lap). It edits the respective information.
9. Save tracks in database: creates or overwrites a .txt file for each track, with the file being named after the track's name. Stored in the Tracks folder. Any deleted track's files are deleted in this process.
10. Delete track: asks for the Id of a track, then deletes it.
11. Create championship: asks for the name of the championship, then creates it. 
12. See all championships: show all championships.
13. Edit championship: asks for the Id of a championship and then asks for its new name. Edits the name of the championship.
14. Save championships in database: creates a folder inside of the Championships folder for each championship if it doesn't exists. Then writes its information in a `about.txt` file in the folder.
15. Delete championship: asks for the Id of a championship, then deletes it.
16. Create race: asks for the Id of a championship and for the Id of a track, then creates a race in the championship, which will be named after its track's name plus "-race".
17. See races: asks for the Id of a championship, then shows all races in that championship.
18. Process race: asks for the Id of a championships and for the Id of a race within that championship. It then processes the race. This is the most complicated action, so it's explained in its own section below.
19. See a championship's standings: asks for the Id of a championship, then show its standings. It awards points for all drivers who finished races not lower than P10, following Formula 1's points system, from P1 to P10: 25, 18, 15, 12, 10, 8, 6, 4, 2, 1.
20. Generate usernames for pings: asks for the Id of a championship, then for the Id of a race within that championship. It writes the usernames of all its drivers. I use this to notify the drivers on Reddit when I make a new post.

There is also a "(100) Close program". It, well, closes the program.

## Race
To-do: write ts