# NBA-Player-Data-Management-System

Database Schema (Chart)
<img width="1087" height="842" alt="image" src="https://github.com/user-attachments/assets/eb7cf0a6-ca58-414c-986c-1ad33982df12" />

**Project Overview**
This project loads NBA player information and player statistics from CSV files (attached in the project) into a SQL Server database using an EF Core–driven WPF application. The tool provides validation, logging, and robust exception handling to ensure data integrity during the import process.


**Database Schema**

Players Table
| Column Name   | Data Type            | Description                            |
| ------------- | -------------------- | -------------------------------------- |
|   PlayerKey   | `INT` (PK, Identity) | Surrogate primary key.                 |
|   player_id   | `NVARCHAR(50)`       | Unique player identifier from the CSV. |
|   player      | `NVARCHAR(100)`      | Player full name.                      |
|   age         | `INT`                | Age during the season.                 |
|   team        | `NVARCHAR(10)`       | Team abbreviation.                     |
|   pos         | `NVARCHAR(10)`       | Player position.                       |
|   season      | `NVARCHAR(10)`       | Season (e.g., `"2025"`).               |
|   lg          | `NVARCHAR(10)`       | League (`NBA`).                        |


PlayerStats Table
| Column Name      | Data Type            | Description                             |
| ---------------- | -------------------- | --------------------------------------- |
| StatsId          | INT (PK, Identity)   | Surrogate primary key.                  |
| player_id        | NVARCHAR(50)         | Player ID matching Players.player_id.   |
| Season           | INT                  | Season this stat row belongs to.        |
| G                | INT                  | Games played.                           |
| Gs               | INT                  | Games started.                          |
| Mp               | FLOAT                | Minutes played.                         |
| Fg               | INT                  | Field goals made.                       |
| Fga              | INT                  | Field goals attempted.                  |
| FgPercent        | FLOAT                | FG%.                                    |
| X3p              | INT                  | 3-point shots made.                     |
| X3pa             | INT                  | 3-point attempts.                       |
| X3pPercent       | FLOAT                | 3-point %.                               |
| X2p              | INT                  | 2-point field goals made.               |
| X2pa             | INT                  | 2-point attempts.                       |
| X2pPercent       | FLOAT                | 2-point %.                               |
| EFgPercent       | FLOAT                | Effective FG%.                           |
| Ft               | INT                  | Free throws made.                       |
| Fta              | INT                  | Free throw attempts.                    |
| FtPercent        | FLOAT                | FT%.                                    |
| Orb              | FLOAT                | Offensive rebounds.                     |
| Drb              | FLOAT                | Defensive rebounds.                     |
| Trb              | FLOAT                | Total rebounds.                         |
| Ast              | FLOAT                | Assists.                                |
| Stl              | FLOAT                | Steals.                                 |
| Blk              | FLOAT                | Blocks.                                 |
| Tov              | FLOAT                | Turnovers.                              |
| Pf               | FLOAT                | Personal fouls.                         |
| Pts              | INT                  | Total points.                           |
| TrpDbl           | FLOAT                | Triple-doubles.                         |
| PlayerKey        | INT (FK)             | Foreign key → Players.PlayerKey.        |





**Relationship**
Players (1) → PlayerStats (many)

A single Player row represents:
- one player
- one specific team
- one specific season

A PlayerStats row represents:
- that player’s statistics for that same season

Linking is done using:
- **PlayerKey** (foreign key)
- **Season** (for accuracy and UI display)

Because 2TM/3TM/TOT rows are removed, every Player row corresponds cleanly to one stat row.



**Data Loading Process**
1. Load Player Info 
- Reads Player_Info.csv
- Imports one row per player per team per season
- **Skips multi-team rollup rows:** 2TM, 3TM, 4TM, TOT
- Inserts only the true per-team rows so stats match correctly
- Prevents duplicate importing if Players table already contains data.


2. Load Player Stats
- Reads Player_Stats.csv.
- Groups stat rows by player_id.
- Reverses stat rows because the CSV lists newest → oldest seasons.
- Player seasons from Player_Info.csv are sorted oldest → newest.
- Each stat row is matched to the correct season using:
   • PlayerId  
   • Season  
   • PlayerKey  
- 2TM/3TM/TOT rows are skipped so each season/team pair is clean.
- Inserts one stat row per season per team.
- Prevents re-import if the table already contains data.



**Additional Behaviors**
Validation
-Prevents re-importing if tables already contain data.
-Verifies that all stats rows reference an existing player.
-Handles number parsing using safe ParseInt() and ParseDouble() methods.

Exception Handling
-All operations wrapped in try/catch.
-Any unhandled error is appended to the on-screen log.


Data Cleanup Rules (Updated)
- Multi-team summary rows (2TM, 3TM, 4TM, TOT) are removed before importing.
- Players table contains only:
  - one player
  - one team
  - one season
- PlayerStats contains:
  - one player
  - one season
  - correct stat values for that season
- Season column was added to PlayerStats to ensure accurate matching.
- Safe numeric parsing prevents crashes on invalid values.
  
- A Season column was added to PlayerStats to ensure each stat record is tied to the correct team/season combination.
- This allows players with multiple teams in a single year to have separate, accurate stat rows.



**Logging**
The application displays logs in a scrollable text box. Logged messages include:
-Start and end of each import.
-Number of rows processed (optional but recommended).
-Warnings for missing players.
-Errors with full exception messages.
-Stoppage messages when attempting to re-import data.

**How to Run**
-Open the solution in Visual Studio.
-Ensure SQL Server is running and the connection string in NbaContext.cs is correct.
-Place Player_Info.csv and Player_Stats.csv in the executable folder (already placed)
-Run the program and click buttons in order: Load Player Info, then press Load Player Stats

**WPF Application Features**
- Full CRUD (Create, Read, Update, Delete) interface for managing Players.
- DataGrid with NBA-themed styling for browsing player data.
- Search and filtering panel (PlayerId, Name, Age, Team, Season).
- Double-click any row to open a Player Card window:
   • Displays headshot (via Basketball Reference).
   • Flip card UI (front: photo, back: stats).
- Error handling and input validation across all UI actions.
- Live logs during CSV import.
