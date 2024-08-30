-- Create the Tour table
-- Create the Tour table
CREATE TABLE "Tour" (
                        "Tour_ID" SERIAL PRIMARY KEY,
                        "Name" VARCHAR(50) UNIQUE NOT NULL,
                        "Description" VARCHAR(2000),
                        "From" VARCHAR(50),
                        "To" VARCHAR(50),
                        "RouteType" VARCHAR(50),
                        "StartLatitude" DOUBLE PRECISION,
                        "StartLongitude" DOUBLE PRECISION,
                        "EndLatitude" DOUBLE PRECISION,
                        "EndLongitude" DOUBLE PRECISION,
                        "EncodedRoute" TEXT
);

-- Add columns to the Tour table
ALTER TABLE "Tour"
    ADD COLUMN "Distance" double precision,  -- Adds the Distance column with type double precision
    ADD COLUMN "EstimatedTime" interval;    -- Adds the EstimatedTime column with type interval



-- Create the TourLog table
CREATE TABLE "TourLog" (
                           "TourLog_ID" SERIAL PRIMARY KEY,
                           "Tour_ID" INT NOT NULL,
                           "TourDate" TIMESTAMP NOT NULL,
                           "Distance" DECIMAL NOT NULL,
                           "Difficulty" DECIMAL NOT NULL,
                           "Steps" DECIMAL NOT NULL,
                           "Weather" VARCHAR(50),
                           "Comment" VARCHAR(2000),
                           "Rating" INT NOT NULL,
                           "Duration" DECIMAL NOT NULL,
                           "TotalTime" DECIMAL NOT NULL
);

-- Add a foreign key constraint to TourLog
ALTER TABLE "TourLog"
    ADD FOREIGN KEY ("Tour_ID") REFERENCES "Tour" ("Tour_ID") ON DELETE CASCADE;

-- Insert sample data into the Tour table
INSERT INTO "Tour" ("Name", "Description", "From", "To", "RouteType", "StartLatitude", "StartLongitude", "EndLatitude", "EndLongitude", "EncodedRoute")
VALUES
    ('Mountain Hike', 'Explore the scenic mountain trails', 'Town A', 'Mountain', 'Bike', 47.36865, 8.53918, 47.36965, 8.54018, NULL),
    ('City Bike Tour', 'Sightseeing tour around the city on bikes', 'City', 'City', 'Foot', 40.71278, -74.006, 40.71378, -74.007, NULL),
    ('Coastal Walk', 'Enjoy a leisurely walk along the coastline', 'Beach', 'Beach', 'Hike', 34.05223, -118.24368, 34.05293, -118.24468, NULL),
    ('Forest Trek', 'Trek through dense forest trails', 'Town B', 'Forest', 'All', 37.77493, -122.41942, 37.77593, -122.42042, NULL),
    ('Urban Exploration', 'Discover hidden gems in the city', 'City', 'City', 'Walk', 51.5074, -0.1278, 51.5084, -0.1288, NULL);

-- Insert sample data into the TourLog table
INSERT INTO "TourLog" ("Tour_ID", "TourDate", "Distance", "Difficulty", "Duration", "Steps", "Weather", "TotalTime", "Comment", "Rating")
VALUES
    (1, '2024-05-01 08:00:00', 10.5, 7.5, 3.5, 15000, 'Sunny', 4.5, 'Amazing views at the peak!', 5),
    (2, '2024-05-03 10:30:00', 15.2, 5.0, 2.0, 8000, 'Cloudy', 2.5, 'Enjoyed cycling through historic districts.', 4),
    (3, '2024-05-05 14:00:00', 8.7, 3.0, 1.5, 6000, 'Sunny', 2.0, 'Saw dolphins along the way!', 5),
    (4, '2024-05-07 09:00:00', 12.3, 6.8, 4.0, 10000, 'Rainy', 5.0, 'Trails were slippery after the rain.', 3),
    (5, '2024-05-10 11:00:00', 9.8, 4.5, 2.5, 7000, 'Sunny', 3.0, 'Discovered a cozy cafe in an alleyway.', 4);





--CLEANUP
TRUNCATE TABLE "TourLog" RESTART IDENTITY CASCADE;
TRUNCATE TABLE "Tour" RESTART IDENTITY CASCADE;

DELETE FROM "Tour" WHERE "Name" = 'Tour to Delete';
DELETE FROM "Tour" WHERE "Name" = 'Test Tour';
DELETE FROM "Tour" WHERE "Name" = 'Updated Tour';
DELETE FROM "Tour" WHERE "Name" = 'Old Tour';
DELETE FROM "Tour" WHERE "Name" = 'Specific Tour';
DELETE FROM "Tour" WHERE "Name" = 'TourWithRouteType';

DELETE FROM "Tour" WHERE "Name" = 'TourWithLogs';
DELETE FROM "Tour" WHERE "Name" = 'TourWithoutLogs';

DELETE FROM "TourLog" WHERE "Comment" = 'Initial comment';
DELETE FROM "TourLog" WHERE "Comment" = 'To be deleted';
DELETE FROM "Tour" WHERE "Name" = 'TourWithLog';
DELETE FROM "Tour" WHERE "Name" = 'TourWithLogToDelete';


DELETE FROM "Tour" WHERE "Name" IN ('Tour Name 1', 'Tour Name 2');
DELETE FROM "TourLog" WHERE "TourLog_ID" = 1;
DELETE FROM "TourLog" WHERE "Tour_ID" IN (SELECT "Tour_ID" FROM "Tour" WHERE "Name" IN ('Tour Name 1', 'Tour Name 2'));


DROP TABLE "TourLog";
DROP TABLE "Tour";

DROP TABLE IF EXISTS "Logs";
DROP TABLE IF EXISTS "Tours";


