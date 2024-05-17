CREATE TABLE "Tour" (
                        "Tour_ID" SERIAL PRIMARY KEY,
                        "Name" varchar(50) UNIQUE NOT NULL,
                        "Description" varchar(2000),
                        "From" varchar(50),
                        "To" varchar(50),
                        "RouteType" varchar(50) 
);

CREATE TABLE "TourLog" (
                           "TourLog_ID" SERIAL PRIMARY KEY,
                           "Tour_ID" int NOT NULL,
                           "TourDate" timestamp NOT NULL,
                           "Distance" decimal NOT NULL,
                           "Difficulty" decimal NOT NULL,
                           "Steps" decimal NOT NULL,
                           "Weather" varchar(50),
                           "Comment" varchar(2000),
                           "Rating" int NOT NULL
                       
);

ALTER TABLE "TourLog" ADD FOREIGN KEY ("Tour_ID") REFERENCES "Tour" ("Tour_ID");




-- Insert sample data into Tour table
INSERT INTO "Tour" ("Name", "Description", "From", "To", "RouteType")
VALUES
    ('Mountain Hike', 'Explore the scenic mountain trails', 'Town A', 'Mountain', 'Bike'),
    ('City Bike Tour', 'Sightseeing tour around the city on bikes', 'City', 'City', 'Foot'),
    ('Coastal Walk', 'Enjoy a leisurely walk along the coastline', 'Beach', 'Beach', 'Hike'),
    ('Forest Trek', 'Trek through dense forest trails', 'Town B', 'Forest', 'all'),
    ('Urban Exploration', 'Discover hidden gems in the city', 'City', 'City', 'Walk');

-- Create TourLog table


-- Insert sample data into TourLog table
INSERT INTO "TourLog" ("Tour_ID", "TourDate", "Distance", "Difficulty", "Duration", "Steps", "Weather", "TotalTime", "Comment", "Rating")
VALUES
    (1, '2024-05-01 08:00:00', 10.5, 7.5, 3.5, 15000, 'Sunny', 4.5, 'Amazing views at the peak!', 5),
    (2, '2024-05-03 10:30:00', 15.2, 5.0, 2.0, 8000, 'Cloudy', 2.5, 'Enjoyed cycling through historic districts.', 4),
    (3, '2024-05-05 14:00:00', 8.7, 3.0, 1.5, 6000, 'Sunny', 2.0, 'Saw dolphins along the way!', 5),
    (4, '2024-05-07 09:00:00', 12.3, 6.8, 4.0, 10000, 'Rainy', 5.0, 'Trails were slippery after the rain.', 3),
    (5, '2024-05-10 11:00:00', 9.8, 4.5, 2.5, 7000, 'Sunny', 3.0, 'Discovered a cozy cafe in an alleyway.', 4);
